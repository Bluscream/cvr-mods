using HarmonyLib;
using MelonLoader;
using System.Drawing;

namespace Bluscream.NotificationLog;

public class NotificationLog : MelonMod {

    public override void OnInitializeMelon() {

        ModConfig.InitializeMelonPrefs();


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
            if (ModConfig.MeLogHUDNotifications.Value) {
                var _c = ModConfig.MeLogHUDNotificationsColorARGB.Value;
                MelonLogger.Msg(Color.FromArgb(_c[0], _c[1], _c[2], _c[3]), // cursed
                    ModConfig.MeLogHUDNotificationsTemplate.Value.Replace("{category}", cat).Replace("{headline}", headline).Replace("{small}", small) // cursed, use format pls
                );
            }
        } catch (Exception e) {
            MelonLogger.Error(e);
        }
    }
}
