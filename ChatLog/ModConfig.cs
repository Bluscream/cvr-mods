using Bluscream.ChatLog.Properties;
using MelonLoader;
using System.Drawing;

namespace Bluscream.ChatLog;

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> LogIncomingChat;
    internal static MelonPreferences_Entry<List<ushort>> LogIncomingChatColorARGB;
    internal static MelonPreferences_Entry<string> LogIncomingChatTemplate;

    internal static MelonPreferences_Entry<bool> LogIncomingMod;
    internal static MelonPreferences_Entry<List<ushort>> LogIncomingModColorARGB;
    internal static MelonPreferences_Entry<string> LogIncomingModTemplate;

    internal static MelonPreferences_Entry<bool> LogOutgoingChat;
    internal static MelonPreferences_Entry<List<ushort>> LogOutgoingChatColorARGB;
    internal static MelonPreferences_Entry<string> LogOutgoingChatTemplate;

    internal static MelonPreferences_Entry<bool> LogOutgoingMod;
    internal static MelonPreferences_Entry<List<ushort>> LogOutgoingModColorARGB;
    internal static MelonPreferences_Entry<string> LogOutgoingModTemplate;

    internal static Color GetColor(List<ushort> _c) => Color.FromArgb(_c[0], _c[1], _c[2], _c[3]);  // cursed

    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        LogIncomingChat = _melonCategory.CreateEntry("Log incoming chat messages", true,
            description: "Whether to log incoming chat messages to MelonLoader console/log or not.");
        LogIncomingChatColorARGB = _melonCategory.CreateEntry("Incoming chat messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging incoming chat messages");
        LogIncomingChatTemplate = _melonCategory.CreateEntry("Incoming chat messages log template", "Message from {1}: \"{0}\"",
            description: "The template to use for logging incoming chat messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");

        LogIncomingMod = _melonCategory.CreateEntry("Log incoming mod messages", false,
            description: "Whether to log incoming mod messages to MelonLoader console/log or not.");
        LogIncomingModColorARGB = _melonCategory.CreateEntry("Incoming mod messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging incoming mod messages");
        LogIncomingModTemplate = _melonCategory.CreateEntry("Incoming mod messages log template", "Mod Message from {1} via \"{4}\": \"{0}\"",
            description: "The template to use for logging incoming mod messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");

        LogOutgoingChat = _melonCategory.CreateEntry("Log outgoing chat messages", true,
            description: "Whether to log outgoing chat messages to MelonLoader console/log or not.");
        LogOutgoingChatColorARGB = _melonCategory.CreateEntry("Outgoing chat messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging outgoing chat messages");
        LogOutgoingChatTemplate = _melonCategory.CreateEntry("Outgoing chat messages log template", "Message from {1}: \"{0}\"",
            description: "The template to use for logging outgoing chat messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");

        LogOutgoingMod = _melonCategory.CreateEntry("Log outgoing mod messages", false,
            description: "Whether to log outgoing mod messages to MelonLoader console/log or not.");
        LogOutgoingModColorARGB = _melonCategory.CreateEntry("Outgoing mod messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging outgoing mod messages");
        LogOutgoingModTemplate = _melonCategory.CreateEntry("Outgoing mod messages log template", "Mod Message from {1} via \"{4}\": \"{0}\"",
            description: "The template to use for logging outgoing mod messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");
    }

}
