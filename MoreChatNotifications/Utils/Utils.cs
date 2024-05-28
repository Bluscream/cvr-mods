using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using MelonLoader;
using Bluscream.MoreChatNotifications;

namespace Bluscream;

public static partial class Utils {
    public static void Debug(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
#if DEBUG
        MelonLogger.Warning("DEBUG: "+message.ToString(), parms);
#endif
    }
    public static void Log(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        MelonLogger.Msg(message.ToString(), parms);
    }
    public static void Error(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        MelonLogger.Error(message.ToString(), parms);
    }
    public static void BigError(object message) {
        if (!ModConfig.EnableLogging.Value) return;
        MelonLogger.BigError(MoreChatNotifications.Properties.AssemblyInfoParams.Name, message.ToString());
    }
    public static void Warn(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        MelonLogger.Warning(message.ToString(), parms);
    }
    public static void HUDNotify(string header = null, string subtext = null, string cat = null, float? time = null) {
        if (!ModConfig.EnableMod.Value || !ModConfig.EnableHUDNotifications.Value) return;
        cat ??= $"(Local) {MoreChatNotifications.Properties.AssemblyInfoParams.Name}";
        if (time != null) {
            ViewManager.Instance.NotifyUser(cat, subtext, time.Value);
        } else {
            ViewManager.Instance.NotifyUserAlert(cat, header, subtext);
        }
    }
    public static void SendChatNotification(object text, bool sendSoundNotification = false, bool displayInHistory = false) {
        if (!ModConfig.EnableMod.Value || !ModConfig.EnableChatNotifications.Value) return;
        Kafe.ChatBox.API.SendMessage(text.ToString(), sendSoundNotification: sendSoundNotification, displayInChatBox: true, displayInHistory: displayInHistory);
    }
}
