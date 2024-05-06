using ABI_RC.Systems.GameEventSystem;
using HarmonyLib;
using MelonLoader;
using UnityEngine.SceneManagement;

namespace Bluscream.NotificationLog;

public class NotificationLog : MelonMod {

    public override void OnInitializeMelon() {

        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Player.OnJoin.AddListener(player => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogPlayerJoinLeaves.Value) return;
            var tag = player.userStaffTag;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userRank;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userClanTag;
            MelonLogger.MsgDirect(ModConfig.GetColor(ModConfig.LogPlayerJoinColorARGB.Value),
                string.Format(ModConfig.LogPlayerJoinTemplate.Value,
                                      player.userName, tag, player.ownerId));
        });
        CVRGameEventSystem.Player.OnLeave.AddListener(player => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogPlayerJoinLeaves.Value) return;
            var tag = player.userStaffTag;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userRank;
            if (string.IsNullOrWhiteSpace(tag)) tag = player.userClanTag;
            MelonLogger.MsgDirect(ModConfig.GetColor(ModConfig.LogPlayerLeaveColorARGB.Value),
                string.Format(ModConfig.LogPlayerLeaveTemplate.Value,
                                      player.userName, tag, player.ownerId));
        });

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value || !ModConfig.LogInstanceJoins.Value) return;
            var privacy = ABI_RC.Core.Networking.IO.Instancing.Instances.GetPrivacy(ABI_RC.Core.Savior.MetaPort.Instance.CurrentInstancePrivacy);
            var scene = SceneManager.GetActiveScene();
            var players = ABI_RC.Core.Player.CVRPlayerManager.Instance.NetworkPlayers.Count;
            MelonLogger.MsgDirect(ModConfig.GetColor(ModConfig.LogInstanceJoinsColorARGB.Value),
                string.Format(ModConfig.LogInstanceJoinsTemplate.Value,
                                      instance, privacy, players, scene.name, scene.path, scene.buildIndex));
        });

        //CVRGameEventSystem.World.OnLoad.AddListener(world => {
        //    MelonLogger.MsgDirect($"[CVRGameEventSystem.World.OnLoad] {CVRPlayerManager.Instance.NetworkPlayers.Count} {world} Type: {MetaPort.Instance.CurrentInstancePrivacy}");
        //});

        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropText", new[] {
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(NotificationLog), nameof(HUDNotificationRecieved)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropText", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(NotificationLog), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextImmediate", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(NotificationLog), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextLong", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(NotificationLog), nameof(HUDNotificationRecievedCat)))
        );
        HarmonyInstance.Patch(
            AccessTools.Method(AccessTools.TypeByName("CohtmlHud"), "ViewDropTextLonger", new[] {
                typeof(string), // cat
                typeof(string), // headline
                typeof(string) // small
            }),
            prefix: new HarmonyMethod(AccessTools.Method(typeof(NotificationLog), nameof(HUDNotificationRecievedCat)))
        );
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
                MelonLogger.MsgDirect(ModConfig.GetColor(ModConfig.LogHUDNotificationsColorARGB.Value),
                    string.Format(ModConfig.LogHUDNotificationsTemplate.Value,
                                          cat, headline, small)
                );
            }
        } catch (Exception e) {
            MelonLogger.Error(e);
        }
    }
}

//    [HarmonyPatch]
//    internal class HarmonyPatches {
//        [HarmonyPrefix]
//        [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
//        public static void Before_CVRWorld_Start() {
//            MelonLogger.Msg($"[Before_CVRWorld_Start] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
//        public static void After_CVRWorld_Start() {
//            MelonLogger.Msg($"[After_CVRWorld_Start] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(Content), nameof(Content.LoadIntoWorld))]
//        public static void After_Content_LoadIntoWorld() {
//            try {
//                MelonLogger.Msg($"[After_Content_LoadIntoWorld] Type: {MetaPort.Instance.CurrentInstancePrivacy}, CurrentWorldId: {MetaPort.Instance.CurrentWorldId}, CurrentInstanceId: {MetaPort.Instance.CurrentInstanceId}, Name: {SceneManager.GetActiveScene().name}");
//            } catch (Exception e) {
//                MelonLogger.Error($"Error during the patched function {nameof(After_Content_LoadIntoWorld)}");
//                MelonLogger.Error(e);
//            }
//        }
//    }
//}
