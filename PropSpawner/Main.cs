using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using UnityEngine.SceneManagement;
using MelonLoader;
using ABI_RC.Core.Player;
using UnityEngine;
using ABI_RC.Core.Util;
using ABI_RC.Core.Networking;
using ABI_RC.Systems.Gravity;
using ABI_RC.Systems.Movement;
using DarkRift;
using HarmonyLib;
using System.Collections;

namespace Bluscream.PropSpawner;

public class PropSpawner : MelonMod {
    private protected const ushort PropLimitRule = 5;
    private protected const ushort PropLimitTotal = 10;
    private Queue<Prop> propSpawnQueue = new();
    private int propSpawnQueueCount = 0;
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();
        PropConfigManager.Initialize();
        UI.BTK.Initialize();

        object spawning = null;
        #region events
        CVRGameEventSystem.Instance.OnConnected.AddListener(instanceId => {
            if (!Utils.PropsAllowed()) return; // Handle can't spawn notification
            //Utils.Debug("CVRGameEventSystem.Instance.OnConnected start");
            if (spawning != null) MelonCoroutines.Stop(spawning); // Cancel previous
            if (ModConfig.UseAsyncTask.Value) {
                Task.Factory.StartNew(() => QueueProps(instanceId));
            } else {
                spawning = MelonCoroutines.Start(DelaySpawnProps(instanceId)); // Start the spawning coroutine
            }
            //Utils.Debug("CVRGameEventSystem.Instance.OnConnected end");
        });
    }
    public override void OnUpdate() {
        if (propSpawnQueue.Count == 0 || !ModConfig.EnableMod.Value) return;
        var prop = propSpawnQueue.Dequeue();
        if (propSpawnQueue.Count == 0) {
            if (MetaPort.Instance.settings.GetSettingsBool("HUDCustomizationPropSpawned", false)) {
                Utils.HUDNotify(null, $"Automatically spawned {propSpawnQueueCount} props", time: 1f);
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
        Utils.Log($"Spawned prop {prop}");
    }
    #endregion events
    #region methods
    //public static void SpawnProp(string propGuid, bool useTargetLocationGravity = false) => SpawnProp(propGuid, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Vector3? pos, bool useTargetLocationGravity = false) => SpawnProp(propGuid, pos: pos, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Quaternion rot, bool useTargetLocationGravity = false) => SpawnProp(propGuid, rotX: rot.x, rotY: rot.y, rotZ: rot.z, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, Vector3 pos, Quaternion rot, bool useTargetLocationGravity = false) => SpawnProp(propGuid, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, useTargetLocationGravity: useTargetLocationGravity);
    //public static void SpawnProp(string propGuid, float? posX = null, float? posY = null, float? posZ = null, float? rotX = null, float? rotY = null, float? rotZ = null, bool useTargetLocationGravity = false) { // THIS IS CURSED PLEASE END ME
    public static void SpawnProp(string propGuid, Vector3? pos = null, Quaternion? rot = null, bool useTargetLocationGravity = false) {
        if (!Utils.PropsAllowed()) return;
        pos ??= PlayerSetup.Instance.gameObject.transform.position;
        //if (useTargetLocationGravity) { // Todo: Fix
        //    pos = GravitySystem.TryGetResultingGravity(pos.Value, false).AppliedGravity.normalized;
        //} else {
        //    pos = BetterBetterCharacterController.Instance.GetGravityDirection();
        //}
        rot ??= Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - pos).Value.normalized, -pos.Value), -pos.Value);
        Utils.Log($"Trying to spawn prop {propGuid} at {pos} with rotation {rot}");
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
    internal void QueueProp(Prop prop, int delay = 1) {
        if (!ModConfig.EnableMod.Value) return;
        //Utils.Warning("debug QueueProp start");
        propSpawnQueue.Enqueue(prop);
        for (int i = 0; i < delay; i++) {
            propSpawnQueue.Enqueue(null);
        }
        //Utils.Debug("QueueProp end");
    }
    internal void QueueProps(string? worldId = null, string? worldName = null, string? sceneName = null, string? instancePrivacy = null) {
        if (!ModConfig.EnableMod.Value) return;
        //Utils.Debug($"QueueProps start");
        worldId ??= MetaPort.Instance.CurrentWorldId;
        sceneName ??= SceneManager.GetActiveScene().name;
        instancePrivacy ??= MetaPort.Instance.CurrentInstancePrivacy;
        var validRules = PropConfigManager.Matches(worldId, worldName, sceneName, instancePrivacy);
        // Utils.Debug($"QueueProps validRules {validRules.Count}");
        foreach (var rule in validRules) {
            if (rule.PropSelectionRandom.HasValue && rule.PropSelectionRandom.Value > 0) {
                if (rule.PropSelectionRandom.Value > rule.Props.Count) {
                    Utils.Error($"PropSelectionRandom for rule {rule} is larger than amount of specified props: {rule.PropSelectionRandom.Value}/{rule.Props.Count}");
                    continue;
                }
                for (int i = 0; i < rule.PropSelectionRandom.Value; i++) {
                    var randomProp = rule.Props.PickRandom();
                    QueueProp(randomProp, 0);
                    //Utils.Msg($"Added prop {randomProp} to queue");
                }
            } else {
                if (rule.Props.Count > PropLimitRule) {
                    Utils.Warn($"Exceeded prop autospawn rule limit of {PropLimitRule}, can't continue");
                    continue;
                }
                foreach (var prop in rule.Props) {
                    QueueProp(prop, 0);
                    Utils.Log($"Added prop {prop} to queue");
                }
            }
        }
        propSpawnQueueCount = propSpawnQueue.Count;
        if (propSpawnQueueCount > PropLimitTotal) {
            Utils.BigError($"You have more props in queue than the mod allows ({propSpawnQueueCount}/{PropLimitTotal})");
            propSpawnQueue.Clear();
            propSpawnQueueCount = 0;
            return;
        }
        //Utils.Debug($"QueueProps end");
    }
    private IEnumerator DelaySpawnProps(string instanceId) {
        // Utils.Debug("DelaySpawnProps start");
        if (!ModConfig.EnableMod.Value) yield break;
        instanceId ??= MetaPort.Instance.CurrentInstanceId;
        Utils.Log($"Joined instance {instanceId}, waiting {ModConfig.AutoSpawnDelay.Value}s before spawning props.");
        yield return new WaitForSeconds(ModConfig.AutoSpawnDelay.Value); // is there a better way to do this but keep using TimeSpan?

        if (instanceId != MetaPort.Instance.CurrentInstanceId) {
            Utils.Warn($"Prop spawning started when we were on the instance {instanceId}, but now we're on {MetaPort.Instance.CurrentInstanceId}. Ignoring...");
            yield break;
        }
        QueueProps();
        // Utils.Debug("DelaySpawnProps end");
    }
    #endregion methods
}
#region patches
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
            Utils.Error(e);
        }
    }
}
#endregion patches
