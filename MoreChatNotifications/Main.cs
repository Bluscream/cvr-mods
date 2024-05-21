using ABI_RC.Core.Networking.IO.Instancing;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;
using HarmonyLib;
using ABI_RC.Core.Player;
using ABI_RC.Systems.VRModeSwitch;
using ABI_RC.Core.Base;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.IK.SubSystems;
using ABI_RC.Systems.IK;
using ABI_RC.Core.UI;
using Bluscream.MoreChatNotifications.Modules;

namespace Bluscream.MoreChatNotifications;
public class Mod : MelonMod {
    public static MelonLogger.Instance Logger;
    //public static object DelayCoroutine = null;
    public static DateTime LastWorldTime = DateTime.Now;
    public static float LastWorldPercent = 0f;
    private static bool FirstWorldLoaded = false;
    private static VirtualDesktopModule VirtualDesktopModule = new();

    public override void OnInitializeMelon() {
        Logger = new MelonLogger.Instance(AssemblyInfoParams.Name, color: System.Drawing.Color.DarkCyan);
        ModConfig.InitializeMelonPrefs();
        VirtualDesktopModule.Initialize();

        if (RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") is null) {
            Logger.BigError("Chatbox mod not found! Make sure it is properly installed");
            return;
        }

        VRModeSwitchEvents.OnPostVRModeSwitch.AddListener(vr => {
            if (!ModConfig.EnableMod.Value || !ModConfig.VRModeSwitchNotificationsEnabled.Value) return;
            SendChatNotification(
                text: string.Format(vr ? ModConfig.VRModeSwitchNotificationsTemplateVR.Value : ModConfig.VRModeSwitchNotificationsTemplateDesktop.Value),
                sendSoundNotification: ModConfig.VRModeSwitchNotificationsSoundEnabled.Value
            );
        });
    }

    internal static void SendChatNotification(object text, bool sendSoundNotification = false, bool displayInHistory = false) {
        if (!ModConfig.EnableMod.Value) return;
        Kafe.ChatBox.API.SendMessage(text.ToString(), sendSoundNotification: sendSoundNotification, displayInChatBox: true, displayInHistory: displayInHistory);
    }

    internal static void OnFirstWorldLoaded() {
        if (!ModConfig.EnableMod.Value || FirstWorldLoaded) return;
        FirstWorldLoaded = true;
        Logger.Msg("OnFirstWorldLoaded");
        if (VirtualDesktopModule.Config.Enabled.Value) VirtualDesktopModule.ToggleMonitor();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
        if (!ModConfig.EnableMod.Value) return;
        switch (buildIndex) {
            case 0: break;
            case 1: break;
            case 2: break;
            default:
                if (!FirstWorldLoaded) OnFirstWorldLoaded();
                break;
        }
    }

    [HarmonyPatch]
    internal class HarmonyPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HudOperations), nameof(HudOperations.LoadWorldIndicator))]
        internal static void AfterLoadWorldIndicator(bool reset, int stage, float value) {
            if (!ModConfig.EnableMod.Value || !ModConfig.WorldDownloadNotificationsEnabled.Value || reset || stage != 1 || value == LastWorldPercent) return; // stage 1 = download
            //if (DelayCoroutine != null) MelonCoroutines.Stop(DelayCoroutine);
            var now = DateTime.Now;
            if ((now - LastWorldTime).TotalMilliseconds > ModConfig.WorldDownloadNotificationsIntervalMS.Value) { // value == 0 || 
                SendChatNotification(
                    text: string.Format(ModConfig.WorldDownloadNotificationsTemplate.Value, value.ToString())
                );
                //DelayCoroutine = MelonCoroutines.Start(CommonMethods.Delay1S());
            }
            LastWorldPercent = value; LastWorldTime = now;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Content), nameof(Content.LoadIntoWorld))]
        internal static void AfterLoadIntoWorld(string worldId, bool isHomeRequested = false) {
            if (!ModConfig.EnableMod.Value) return;
            if (!FirstWorldLoaded) {
                OnFirstWorldLoaded();
            }
            if (!ModConfig.InstanceSwitchNotificationsEnabled.Value || !ModConfig.InstanceRejoinNotificationsEnabled.Value) return;
            var isRejoin = Instances.RequestedInstance == MetaPort.Instance.CurrentInstanceId;
            if (isRejoin && ModConfig.InstanceRejoinNotificationsEnabled.Value) {
                SendChatNotification(
                    text: string.Format(ModConfig.InstanceRejoinNotificationsTemplate.Value,
                        Instances.RequestedInstance, worldId, isHomeRequested ? "Home" : string.Empty
                    ),
                    sendSoundNotification: ModConfig.InstanceSwitchNotificationsSoundEnabled.Value
                );
            } else if (!isRejoin && !ModConfig.InstanceSwitchNotificationsEnabled.Value) {
                SendChatNotification(
                    text: string.Format(ModConfig.InstanceSwitchNotificationsTemplate.Value,
                        Instances.RequestedInstance, worldId, isHomeRequested ? "Home" : string.Empty
                    ),
                    sendSoundNotification: ModConfig.InstanceRejoinNotificationsSoundEnabled.Value
                );
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BodySystem), nameof(BodySystem.ToggleFullBody))]
        internal static void AfterToggleFullBody() {
            if (!ModConfig.EnableMod.Value || !ModConfig.FBTModeSwitchNotificationsEnabled.Value) return;
            SendChatNotification(
                text: IKSystem.Instance.BodySystem.FullBodyEnabled ? ModConfig.FBTModeSwitchNotificationsTemplateFBT.Value : ModConfig.FBTModeSwitchNotificationsTemplateHalfBody.Value,
                sendSoundNotification: ModConfig.FBTModeSwitchNotificationsSoundEnabled.Value
            );
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CohtmlHud), nameof(CohtmlHud.SetCommsIndicator))]
        internal static void AfterSetCommsIndicator(bool shown) {
            if (!ModConfig.EnableMod.Value || !ModConfig.VoiceConnectionLostNotificationEnabled.Value) return;
            SendChatNotification(
                text: string.Format(shown ? ModConfig.VoiceConnectionLostNotificationTemplateGained.Value : ModConfig.VoiceConnectionLostNotificationTemplateLost.Value),
                sendSoundNotification: ModConfig.VoiceConnectionLostNotificationSoundEnabled.Value
            );
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AudioManagement), nameof(AudioManagement.SetMicrophoneActive))]
        internal static void AfterSetMicrophoneActive(bool active) {
            if (!ModConfig.EnableMod.Value || !ModConfig.MicrophoneNotificationEnabled.Value) return;
            SendChatNotification(
                text: string.Format(active ? ModConfig.MicrophoneNotificationTemplateUnmuted.Value : ModConfig.MicrophoneNotificationTemplateMuted.Value),
                sendSoundNotification: ModConfig.MicrophoneNotificationSoundEnabled.Value
            );
        }
    }
}
