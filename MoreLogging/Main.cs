using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.Systems.Gravity;
using ABI_RC.Systems.Movement;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bluscream.MoreLogging;

public class MoreLogging : MelonMod {
    public static MelonLogger.Instance Logger;
    public static MelonLogger.Instance NotificationLogger;

    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();
        Logger = new MelonLogger.Instance(ModConfig.LoggerName.Value, ModConfig.LoggerColorARGB.Value.ToColor());
        NotificationLogger = new MelonLogger.Instance(ModConfig.NotificationLoggerName.Value, ModConfig.NotificationLoggerColorARGB.Value.ToColor());
        #region Events
        CVRGameEventSystem.Player.OnJoin.AddListener(player => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogPlayerJoinLeaves.Value) return;
            var tag = player.userStaffTag;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userRank;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userClanTag;
            Logger.Msg(ModConfig.LogPlayerJoinColorARGB.Value.ToColor(),
                string.Format(ModConfig.LogPlayerJoinTemplate.Value,
                                      player.userName, tag, player.ownerId));
        });
        CVRGameEventSystem.Player.OnLeave.AddListener(player => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogPlayerJoinLeaves.Value) return;
            var tag = player.userStaffTag;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userRank;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userClanTag;
            Logger.Msg(ModConfig.LogPlayerLeaveColorARGB.Value.ToColor(),
                string.Format(ModConfig.LogPlayerLeaveTemplate.Value,
                                      player.userName, tag, player.ownerId));
        });

        CVRGameEventSystem.Instance.OnConnected.AddListener(instanceId => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogInstanceJoins.Value) return;
            var instancePrivacy = MetaPort.Instance.CurrentInstancePrivacy;
            var scene = SceneManager.GetActiveScene();
            var playerCount = CVRPlayerManager.Instance.NetworkPlayers.Count;
            var worldId = MetaPort.Instance.CurrentWorldId;
            var instanceName = MetaPort.Instance.CurrentInstanceName;
            var worldName = instanceName.Split(" (#").First();
            Logger.Msg(ModConfig.LogInstanceJoinsColorARGB.Value.ToColor(),
                string.Format(ModConfig.LogInstanceJoinsTemplate.Value,
                                      instanceId, instanceName, instancePrivacy, playerCount, scene.name, scene.path, scene.buildIndex, worldId, worldName));
        });

        //CVRGameEventSystem.World.OnLoad.AddListener(world => {
        //    Logger.Msg($"[CVRGameEventSystem.World.OnLoad] {CVRPlayerManager.Instance.NetworkPlayers.Count} {world} Type: {MetaPort.Instance.CurrentInstancePrivacy}");
        //});
        #endregion Events
        #region Patches
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropText", new[] {
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecieved)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropText", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextImmediate", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextLong", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextLonger", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CVRSyncHelper"), "SpawnProp", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(MoreLogging), nameof(HUDNotificationRecievedCat)))
        );
        #endregion Patches
        #region Integrations
        if (RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") != null) {
            NotificationLogger.Msg("Chatbox mod found! Enabling Integration...");
            Integrations.ChatBox.Initialize();
        } else {
            NotificationLogger.Warning("Chatbox mod not found! Make sure it is properly installed.");
        }
        #endregion Integrations
    }
    private static void HUDNotificationRecieved(string headline, string small) => HUDNotificationRecievedCat(null, headline, small);
    private static void HUDNotificationRecievedCat(string cat, string headline, string small) {
        try {
            if (!ModConfig.EnableMod.Value) return;
            if (ModConfig.LogHUDNotifications.Value) {
                if (ModConfig.LogHUDNotificationsPurgeNewlines.Value) {
                    cat = cat?.Replace("\n", " ").Trim();
                    headline = headline?.Replace("\n", " ").Trim();
                    small = small?.Replace("\n", " ").Trim();
                }
                NotificationLogger.Msg(ModConfig.LogHUDNotificationsColorARGB.Value.ToColor(),
                    string.Format(ModConfig.LogHUDNotificationsTemplate.Value,
                                          cat, headline, small)
                );
            }
        } catch (Exception e) {
            MelonLogger.Error(e);
        }
    }
    [HarmonyPatch]
    internal class HarmonyPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnProp))]
        public static void After_SpawnProp(string propGuid, float posX, float posY, float posZ, bool useTargetLocationGravity) {
            try {
                if (!ModConfig.EnableMod.Value) return;
                if (ModConfig.LogPropSpawns.Value) {
                    Vector3 vector;
                    if (useTargetLocationGravity) {
                        vector = GravitySystem.TryGetResultingGravity(new Vector3(posX, posY, posZ), false).AppliedGravity.normalized;
                    } else {
                        vector = BetterBetterCharacterController.Instance.GetGravityDirection();
                    }
                    Quaternion rot = Quaternion.LookRotation(Vector3.ProjectOnPlane((PlayerSetup.Instance.CharacterController.RotationPivot.position - new Vector3(posX, posY, posZ)).normalized, -vector), -vector);
                    Logger.Msg(ModConfig.LogPropSpawnsColorARGB.Value.ToColor(),
                        string.Format(ModConfig.LogPropSpawnsTemplate.Value,
                                              propGuid, $"X:{posX} Y:{posY} Z:{posZ}", $"X:{rot.x} Y:{rot.y} Z:{rot.z}")
                    );
                }
            } catch (Exception e) {
                MelonLogger.Error(e);
            }
        }
    }
}
//        [HarmonyPrefix]
//        [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
//        public static void Before_CVRWorld_Start() {
//            Logger.Msg($"[Before_CVRWorld_Start] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
//        public static void After_CVRWorld_Start() {
//            Logger.Msg($"[After_CVRWorld_Start] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(Content), nameof(Content.LoadIntoWorld))]
//        public static void After_Content_LoadIntoWorld() {
//            try {
//                Logger.Msg($"[After_Content_LoadIntoWorld] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//            } catch (Exception e) {
//                MelonLogger.Error($"Error during the patched function {nameof(After_Content_LoadIntoWorld)}");
//                MelonLogger.Error(e);
//            }
//        }
