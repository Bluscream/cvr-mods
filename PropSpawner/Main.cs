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
    private protected const ushort PropLimitPerRule = 3;
    private Queue<Prop> propSpawnQueue = new();
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
        MelonLogger.Warning("debug OnUpdate start");
        var prop = propSpawnQueue.Dequeue();
        MelonLogger.Warning("debug OnUpdate 1");
        if (prop is null) return;
        MelonLogger.Warning("debug OnUpdate 2");
        if (prop.Position != null) {
            MelonLogger.Warning("debug OnUpdate 3");
            var p = prop.Position.ToVector3();
            MelonLogger.Warning("debug OnUpdate 4");
            if (prop.Rotation != null) {
                MelonLogger.Warning("debug OnUpdate 5");
                var r = prop.Rotation.ToQuaternion();
                MelonLogger.Warning("debug OnUpdate 6");
                SpawnProp(prop.Id, p, r, useTargetLocationGravity: false);
                MelonLogger.Warning("debug OnUpdate 7");
            } else {
                MelonLogger.Warning("debug OnUpdate 8");
                SpawnProp(prop.Id, p, useTargetLocationGravity: false);
                MelonLogger.Warning("debug OnUpdate 9");
            }
        } else {
            MelonLogger.Warning("debug OnUpdate 10");
            SpawnProp(prop.Id, useTargetLocationGravity: false); // PlayerSetup.Instance.SpawnProp(prop.Id, PlayerSetup.Instance.gameObject.transform.position);
            MelonLogger.Warning("debug OnUpdate 11");
        }
        MelonLogger.Msg($"Spawned prop {prop}");
    }
    internal void QueueProp(Prop prop, int delay = 1) {
        MelonLogger.Warning("debug QueueProp start");
        propSpawnQueue.Enqueue(prop);
        for (int i = 0; i < delay; i++) {
            propSpawnQueue.Enqueue(null);
        }
        MelonLogger.Warning("debug QueueProp end");
    }
    internal void QueueProps(string? worldId = null, string? worldName = null, string? sceneName = null) {
        MelonLogger.Warning($"debug QueueProps start");
        if (!ModConfig.EnableMod.Value) return;
        worldId ??= MetaPort.Instance.CurrentWorldId;
        sceneName ??= SceneManager.GetActiveScene().name;
        var validRules = PropConfigManager.Matches(worldId, worldName, sceneName);
        MelonLogger.Warning($"debug QueueProps validRules {validRules.Count}");
        foreach (var rule in validRules) {
            if (rule.PropSelectionRandom != null && rule.PropSelectionRandom.Value) {
                var randomProp = rule.Props.PickRandom();
                QueueProp(randomProp, 3);
                MelonLogger.Msg($"Added prop {randomProp} to queue");
            } else {
                if (rule.Props.Count > PropLimitPerRule) {
                    MelonLogger.Warning($"Exceeded prop autospawn limit of {PropLimitPerRule}, can't continue");
                    continue;
                }
                foreach (var prop in rule.Props) {
                    QueueProp(prop, 3);
                    MelonLogger.Msg($"Added prop {prop} to queue");
                }
            }
        }
        MelonLogger.Warning($"debug QueueProps end");
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
        if (MetaPort.Instance.settings.GetSettingsBool("HUDCustomizationPropSpawned", false)) {
            ViewManager.Instance.NotifyUser("(Synced) Client", $"{propGuid} spawned", .5f);
        }
        MelonLogger.Warning("debug SpawnProp 1");
        pos ??= PlayerSetup.Instance.gameObject.transform.position;
        MelonLogger.Warning("debug SpawnProp 2");
        if (useTargetLocationGravity) {
            MelonLogger.Warning("debug SpawnProp 3.1.1");
            pos = GravitySystem.TryGetResultingGravity(pos.Value, false).AppliedGravity.normalized;
            MelonLogger.Warning("debug SpawnProp 3.1.2");
        } else {
            MelonLogger.Warning("debug SpawnProp 3.2.1");
            pos = BetterBetterCharacterController.Instance.GetGravityDirection();
            MelonLogger.Warning("debug SpawnProp 3.2.2");
        }
        MelonLogger.Warning("debug SpawnProp 4");
        rot ??= Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - pos).Value.normalized, -pos.Value), -pos.Value);
        MelonLogger.Warning("debug SpawnProp 5");
        MelonLogger.Msg($"Trying to spawn prop {propGuid} at {pos} with rotation {rot}");
        using (DarkRiftWriter darkRiftWriter = DarkRiftWriter.Create()) {
            darkRiftWriter.Write(propGuid);
            darkRiftWriter.Write(pos.Value.x);
            darkRiftWriter.Write(pos.Value.y);
            darkRiftWriter.Write(pos.Value.z);
            darkRiftWriter.Write(rot.Value.eulerAngles.x);
            darkRiftWriter.Write(rot.Value.eulerAngles.y);
            darkRiftWriter.Write(rot.Value.eulerAngles.z);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(1f);
            darkRiftWriter.Write(0f);
            using (Message message = Message.Create(10050, darkRiftWriter)) {
                // NetworkManager.Instance.GameNetwork.SendMessage(message, SendMode.Reliable);
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
