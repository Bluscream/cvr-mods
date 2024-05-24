using MelonLoader;
using Bluscream.HTTPServer;

namespace Bluscream;

public static partial class Utils {
    internal static MelonLogger.Instance Logger = new(HTTPServer.Properties.AssemblyInfoParams.Name, color: System.Drawing.Color.DarkCyan);
    public static void Debug(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
#if DEBUG
        Logger.Warning("DEBUG: "+message.ToString(), parms);
#endif
    }
    public static void Log(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        Logger.Msg(message.ToString(), parms);
    }
    public static void Error(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        Logger.Error(message.ToString(), parms);
    }
    public static void BigError(object message) {
        if (!ModConfig.EnableLogging.Value) return;
        Logger.BigError(message.ToString());
    }
    public static void Warn(object message, params object[] parms) => Warn(message, parms);
    public static void Warning(object message, params object[] parms) {
        if (!ModConfig.EnableLogging.Value) return;
        Logger.Warning(message.ToString(), parms);
    }
}
