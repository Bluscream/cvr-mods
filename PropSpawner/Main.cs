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

namespace Bluscream.PropSpawner;

public class PropSpawner : MelonMod {
    private Queue<Prop> propSpawnQueue = new();
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            var worldId = MetaPort.Instance.CurrentWorldId;
            Task.Factory.StartNew(() => QueueProps(worldId));
        });

        PropConfigManager.Initialize();
    }

    public override void OnUpdate() {
        if (propSpawnQueue.Count == 0 || !ModConfig.EnableMod.Value) return;
        var prop = propSpawnQueue.Dequeue();
        if (prop._Position != null) {
            var p = prop.Position.Value;
            if (prop._Rotation != null) {
                var r = prop.Rotation.Value;
                SpawnProp(prop.Id, p.x, p.y, p.z, true, r.x, r.y, r.z);
            } else {
                SpawnProp(prop.Id, p.x, p.y, p.z, true);
            }
        } else {
            SpawnProp(prop.Id, null, null, null, true); // PlayerSetup.Instance.SpawnProp(prop.Id, PlayerSetup.Instance.gameObject.transform.position);
        }
        MelonLogger.Msg($"Spawned prop {prop}");
    }

    internal void QueueProps(string? worldId = null, string? worldName = null, string? sceneName = null) {
        if (!ModConfig.EnableMod.Value) return;
        worldId ??= MetaPort.Instance.CurrentWorldId;
        sceneName ??= SceneManager.GetActiveScene().name;
        foreach (var rule in PropConfigManager.Rules) {
            var worldWildcard = ((rule.WorldId is null && rule.SceneName is null) || (rule.WorldId == "*" || rule.SceneName == "*"));
            var worldIdValid = (rule.WorldId == worldId);
            var sceneNameValid = (rule.SceneName == sceneName);
            if (!worldWildcard && !worldIdValid && !sceneNameValid) continue;
            if (rule.PropSelectionRandom) {
                var randomProp = rule.Props.PickRandom();
                propSpawnQueue.Enqueue(randomProp);
                MelonLogger.Msg($"Added prop {randomProp} to queue");
            } else {
                if (rule.Props.Count > 3) throw new Exception("Exceeded prop autospawn limit of 3, can't continue");
                foreach (var prop in rule.Props) {
                    propSpawnQueue.Enqueue(prop);
                    MelonLogger.Msg($"Added prop {prop} to queue");
                }
            }
        }
    }

    public static void SpawnProp(string propGuid, float? posX, float? posY, float? posZ, bool useTargetLocationGravity, float? rotX = null, float? rotY = null, float? rotZ = null) { // THIS IS CURSED PLEASE END ME
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
            ViewManager.Instance.NotifyUser("(Synced) Client", "Prop spawned", 1f);
        }
        Vector3 vector;
        Vector3 playerPosition = PlayerSetup.Instance.gameObject.transform.position;
        posX ??= playerPosition.x;
        posY ??= playerPosition.y;
        posZ ??= playerPosition.z;
        if (useTargetLocationGravity) {
            vector = GravitySystem.TryGetResultingGravity(new Vector3(posX.Value, posY.Value, posZ.Value), false).AppliedGravity.normalized;
        } else {
            vector = BetterBetterCharacterController.Instance.GetGravityDirection();
        }
        Quaternion playerRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - new Vector3(posX.Value, posY.Value, posZ.Value)).normalized, -vector), -vector);
        rotX ??= playerRotation.eulerAngles.x;
        rotY ??= playerRotation.eulerAngles.y;
        rotZ ??= playerRotation.eulerAngles.z;
        using (DarkRiftWriter darkRiftWriter = DarkRiftWriter.Create()) {
            darkRiftWriter.Write(propGuid);
            darkRiftWriter.Write(posX.Value);
            darkRiftWriter.Write(posY.Value);
            darkRiftWriter.Write(posZ.Value);
            darkRiftWriter.Write(rotX.Value);
            darkRiftWriter.Write(rotY.Value);
            darkRiftWriter.Write(rotZ.Value);
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
