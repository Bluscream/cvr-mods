using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using MelonLoader;

namespace Bluscream.PropSpawner;

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
        MelonLogger.BigError(Properties.AssemblyInfoParams.Name, message.ToString());
    }
    public static void Warn(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        MelonLogger.Warning(message.ToString(), parms);
    }
    public static void HUDNotify(string header = null, string subtext = null, string cat = null, float? time = null) {
        if (!ModConfig.EnableHUDNotifications.Value) return;
        cat ??= $"(Local) {Properties.AssemblyInfoParams.Name}";
        if (time != null) {
            ViewManager.Instance.NotifyUser(cat, subtext, time.Value);
        } else {
            ViewManager.Instance.NotifyUserAlert(cat, header, subtext);
        }
    }

    public static bool PropsAllowed() {
        if (!ModConfig.EnableMod.Value) return false;
        if (!CVRSyncHelper.IsConnectedToGameNetwork()) {
            HUDNotify("Cannot spawn prop", "Not connected to an online Instance");
            return false;
        } else if (!MetaPort.Instance.worldAllowProps) {
            HUDNotify("Cannot spawn prop", "Props are not allowed in this world");
            return false;
        } else if (!MetaPort.Instance.settings.GetSettingsBool("ContentFilterPropsEnabled", false)) {
            HUDNotify("Cannot spawn prop", "Props are disabled in content filter");
            return false;
        }
        return true;
    }
}
