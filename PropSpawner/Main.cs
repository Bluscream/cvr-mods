using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using UnityEngine.SceneManagement;
using MelonLoader;
using ABI_RC.Core.Player;
using UnityEngine;
using ABI_RC.Core.Util;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking;
using ABI_RC.Systems.Gravity;
using ABI_RC.Systems.Movement;
using DarkRift;
using HarmonyLib;

namespace Bluscream.PropSpawner;

public class PropSpawner : MelonMod {
    private protected const ushort PropLimitRule = 5;
    private protected const ushort PropLimitTotal = 10;
    private Queue<Prop> propSpawnQueue = new();
    private int propSpawnQueueCount = 0;
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            Task.Factory.StartNew(() => QueueProps());
        });
        PropConfigManager.Initialize();
    }

    public override void OnUpdate() {
        if (propSpawnQueue.Count == 0 || !ModConfig.EnableMod.Value) return;
        var prop = propSpawnQueue.Dequeue();
        if (propSpawnQueue.Count == 0) {
            if (MetaPort.Instance.settings.GetSettingsBool("HUDCustomizationPropSpawned", false)) {
                ViewManager.Instance.NotifyUser("PropSpawner", $"Automatically spawned {propSpawnQueueCount} props", 1f);
            }
            propSpawnQueueCount = 0;
        }
        if (prop is null) return;
        if (prop.Position != null) {
            var pos = prop.Position.ToVector3();
            if (prop.Rotation != null) {
                var rot = prop.Rotation.ToQuaternion();
                SpawnProp(prop.Id, pos, rot, useTargetLocationGravity: false);
            } else {
                SpawnProp(prop.Id, pos, useTargetLocationGravity: false);
            }
        } else {
            SpawnProp(prop.Id, useTargetLocationGravity: false); // PlayerSetup.Instance.SpawnProp(prop.Id, PlayerSetup.Instance.gameObject.transform.position);
        }
        //MelonLogger.Msg($"Spawned prop {prop}");
    }
    internal void QueueProp(Prop prop, int delay = 1) {
        //MelonLogger.Warning("debug QueueProp start");
        propSpawnQueue.Enqueue(prop);
        for (int i = 0; i < delay; i++) {
            propSpawnQueue.Enqueue(null);
        }
        //MelonLogger.Warning("debug QueueProp end");
    }
    internal void QueueProps(string? worldId = null, string? worldName = null, string? sceneName = null, string? instancePrivacy = null) {
        //MelonLogger.Warning($"debug QueueProps start");
        if (!ModConfig.EnableMod.Value) return;
        worldId ??= MetaPort.Instance.CurrentWorldId;
        sceneName ??= SceneManager.GetActiveScene().name;
        instancePrivacy ??= MetaPort.Instance.CurrentInstancePrivacy;
        var validRules = PropConfigManager.Matches(worldId, worldName, sceneName, instancePrivacy);
        // MelonLogger.Warning($"debug QueueProps validRules {validRules.Count}");
        foreach (var rule in validRules) {
            if (rule.PropSelectionRandom.HasValue && rule.PropSelectionRandom.Value > 0) {
                if (rule.PropSelectionRandom.Value > rule.Props.Count) {
                    MelonLogger.Error($"PropSelectionRandom for rule {rule} is larger than amount of specified props: {rule.PropSelectionRandom.Value}/{rule.Props.Count}");
                    continue;
                }
                for (int i = 0; i < rule.PropSelectionRandom.Value; i++) {
                    var randomProp = rule.Props.PickRandom();
                    QueueProp(randomProp, 0);
                    //MelonLogger.Msg($"Added prop {randomProp} to queue");
                }
            } else {
                if (rule.Props.Count > PropLimitRule) {
                    MelonLogger.Warning($"Exceeded prop autospawn rule limit of {PropLimitRule}, can't continue");
                    continue;
                }
                foreach (var prop in rule.Props) {
                    QueueProp(prop, 0);
                    MelonLogger.Msg($"Added prop {prop} to queue");
                }
            }
        }
        propSpawnQueueCount = propSpawnQueue.Count;
        if (propSpawnQueueCount > PropLimitTotal) {
            MelonLogger.BigError("PropSpawner", $"You have more props in queue than the mod allows ({propSpawnQueueCount}/{PropLimitTotal})");
            propSpawnQueue.Clear();
            propSpawnQueueCount = 0;
            return;
        }
        //MelonLogger.Warning($"debug QueueProps end");
    }
    //public static void SpawnProp(string propGuid, bool useTargetLocationGravity = false) => SpawnProp(propGuid, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Vector3? pos, bool useTargetLocationGravity = false) => SpawnProp(propGuid, pos: pos, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Quaternion rot, bool useTargetLocationGravity = false) => SpawnProp(propGuid, rotX: rot.x, rotY: rot.y, rotZ: rot.z, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Vector3 pos, Quaternion rot, bool useTargetLocationGravity = false) => SpawnProp(propGuid, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, float? posX = null, float? posY = null, float? posZ = null, float? rotX = null, float? rotY = null, float? rotZ = null, bool useTargetLocationGravity = false) { // THIS IS CURSED PLEASE END ME
    public static void SpawnProp(string propGuid, Vector3? pos = null, Quaternion? rot = null, bool useTargetLocationGravity = false) {
        if (!CVRSyncHelper.IsConnectedToGameNetwork()) {
            ViewManager.Instance.NotifyUserAlert("(Local) Client", "Cannot spawn prop", "Not connected to an online Instance");
            return;
        }
        if (!MetaPort.Instance.worldAllowProps) {
            ViewManager.Instance.NotifyUserAlert("(Local) Client", "Cannot spawn prop", "Props are not allowed in this world");
            return;
        }
        if (!MetaPort.Instance.settings.GetSettingsBool("ContentFilterPropsEnabled", false)) {
            ViewManager.Instance.NotifyUserAlert("(Local) Client", "Cannot spawn prop", "Props are disabled in content filter");
            return;
        }
        pos ??= PlayerSetup.Instance.gameObject.transform.position;
        //if (useTargetLocationGravity) { // Todo: Fix
        //    pos = GravitySystem.TryGetResultingGravity(pos.Value, false).AppliedGravity.normalized;
        //} else {
        //    pos = BetterBetterCharacterController.Instance.GetGravityDirection();
        //}
        rot ??= Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - pos).Value.normalized, -pos.Value), -pos.Value);
        MelonLogger.Msg($"Trying to spawn prop {propGuid} at {pos} with rotation {rot}");
        using (DarkRiftWriter darkRiftWriter = DarkRiftWriter.Create()) {
            darkRiftWriter.Write(propGuid);
            darkRiftWriter.Write(pos.Value.x);
            darkRiftWriter.Write(pos.Value.y);
            darkRiftWriter.Write(pos.Value.z);
            darkRiftWriter.Write(rot.Value.x);
            darkRiftWriter.Write(rot.Value.y);
            darkRiftWriter.Write(rot.Value.z);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(0f);
            using (Message message = Message.Create(10050, darkRiftWriter)) {
                NetworkManager.Instance.GameNetwork.SendMessage(message, SendMode.Reliable);
            }
        }
    }
}
[HarmonyPatch]
internal class HarmonyPatches {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnProp))]
    public static void After_SpawnProp(string propGuid, float posX, float posY, float posZ, bool useTargetLocationGravity) {
        try {
            if (!ModConfig.EnableMod.Value || !ModConfig.AutoSaveSpawnedProps.Value) return;
            Vector3 vector;
            if (useTargetLocationGravity) {
                vector = GravitySystem.TryGetResultingGravity(new Vector3(posX, posY, posZ), false).AppliedGravity.normalized;
            } else {
                vector = BetterBetterCharacterController.Instance.GetGravityDirection();
            }
            Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - new Vector3(posX, posY, posZ)).normalized, -vector), -vector);
            var instanceName = MetaPort.Instance.CurrentInstanceName;
            var worldName = instanceName.Split(" (#").First();
            PropConfigManager.SaveProp(
                new Prop() {
                    Id = propGuid,
                    Position = new() { posX, posY, posZ },
                    Rotation = new() { rot.x, rot.y, rot.z }
                },
                MetaPort.Instance.CurrentWorldId,
                worldName,
                SceneManager.GetActiveScene().name,
                MetaPort.Instance.CurrentInstancePrivacy
            );
        } catch (Exception e) {
            MelonLogger.Error(e);
        }
    }
}
