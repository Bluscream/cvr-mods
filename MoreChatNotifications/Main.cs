using ABI_RC.Core.Networking.IO.Instancing;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;
using HarmonyLib;
using ABI_RC.Core.Player;
using ABI_RC.Systems.VRModeSwitch;
using ABI_RC.Core.Base;
using ABI_RC.Core.Savior;

namespace Bluscream.MoreChatNotifications;
public class MoreChatNotifications : MelonMod {
    private static protected readonly TimeSpan ms500 = TimeSpan.FromMilliseconds(500);
    public static MelonLogger.Instance Logger;
    //public static object DelayCoroutine = null;
    public static DateTime LastWorldTime = DateTime.Now;
    public static float LastWorldPercent = 0f;

    public override void OnInitializeMelon() {
        Logger = new MelonLogger.Instance(AssemblyInfoParams.Name, color: System.Drawing.Color.DarkCyan);
        ModConfig.InitializeMelonPrefs();

        if (RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") is null) {
            Logger.BigError("Chatbox mod not found! Make sure it is properly installed");
            return;
        }

        VRModeSwitchEvents.OnPostVRModeSwitch.AddListener(vr => {
            if (!ModConfig.EnableMod.Value || !ModConfig.VRModeSwitchNotificationsEnabled.Value) return;
            CommonMethods.SendChatNotification(
                text: string.Format(vr ? ModConfig.VRModeSwitchNotificationsTemplateVR.Value : ModConfig.VRModeSwitchNotificationsTemplateDesktop.Value),
                sendSoundNotification: ModConfig.VRModeSwitchNotificationsSoundEnabled.Value
            );
        });
    }

    internal static class CommonMethods {
        internal static void SendChatNotification(object text, bool sendSoundNotification = false, bool displayInHistory = false) {
            if (!ModConfig.EnableMod.Value) return;
            Kafe.ChatBox.API.SendMessage(text.ToString(), sendSoundNotification: sendSoundNotification, displayInChatBox: true, displayInHistory: displayInHistory);
        }
        //public static System.Collections.IEnumerator Delay1S() {
        //    yield return new WaitForSeconds(1f);
        //    SendChatNotification();
        //}
    }

    [HarmonyPatch]
    internal class HarmonyPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HudOperations), nameof(HudOperations.LoadWorldIndicator))]
        internal static void AfterLoadWorldIndicator(bool reset, int stage, float value) {
            if (!ModConfig.EnableMod.Value || !ModConfig.WorldDownloadNotificationsEnabled.Value || reset || stage != 1 || value == LastWorldPercent) return; // stage 1 = download
            //if (DelayCoroutine != null) MelonCoroutines.Stop(DelayCoroutine);
            var now = DateTime.Now;
            if (now - LastWorldTime > ms500) { // value == 0 || 
                CommonMethods.SendChatNotification(
                    text: string.Format(ModConfig.WorldDownloadNotificationsTemplate.Value, value.ToString())
                );
                //DelayCoroutine = MelonCoroutines.Start(CommonMethods.Delay1S());
            }
            LastWorldPercent = value; LastWorldTime = now;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Content), nameof(Content.LoadIntoWorld))]
        internal static void AfterLoadIntoWorld(string worldId, bool isHomeRequested = false) {
            if (!ModConfig.EnableMod.Value || !ModConfig.InstanceSwitchNotificationsEnabled.Value || !ModConfig.InstanceRejoinNotificationsEnabled.Value) return;
            var isRejoin = Instances.RequestedInstance == MetaPort.Instance.CurrentInstanceId;
            if (isRejoin && ModConfig.InstanceRejoinNotificationsEnabled.Value) {
                CommonMethods.SendChatNotification(
                    text: string.Format(ModConfig.InstanceRejoinNotificationsTemplate.Value,
                        Instances.RequestedInstance, worldId, isHomeRequested ? "Home" : string.Empty
                    ),
                    sendSoundNotification: ModConfig.InstanceSwitchNotificationsSoundEnabled.Value
                );
            } else if (!isRejoin && !ModConfig.InstanceSwitchNotificationsEnabled.Value) {
                CommonMethods.SendChatNotification(
                    text: string.Format(ModConfig.InstanceSwitchNotificationsTemplate.Value,
                        Instances.RequestedInstance, worldId, isHomeRequested ? "Home" : string.Empty
                    ),
                    sendSoundNotification: ModConfig.InstanceRejoinNotificationsSoundEnabled.Value
                );
            }
        }
    }
}
