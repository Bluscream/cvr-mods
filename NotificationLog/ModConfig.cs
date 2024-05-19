using Bluscream.MoreLogging.Properties;
using MelonLoader;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Bluscream.MoreLogging;

internal static class RankColor {
    static Color Blocked = Color.FromArgb(155, 255, 255, 255); // dark grey
    static Color User = Color.FromArgb(255, 255, 255, 255); // white
    static Color QA = Color.FromArgb(200, 0, 255, 0); // green
    static Color CommunityGuide = Color.FromArgb(255, 255, 168, 0); // orange
    static Color Developer = Color.FromArgb(255, 255, 0, 66); // pink
    static Color Moderator = Color.FromArgb(255, 255, 0, 0); // red
}

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<string> LoggerName;
    internal static MelonPreferences_Entry<List<ushort>> LoggerColorARGB;

    internal static MelonPreferences_Entry<List<string>> _BlacklistRegexes;
    internal static List<Regex> BlacklistRegexes {
        get { return _BlacklistRegexes.Value.Select(r => new Regex(r)).ToList(); }
    }

    internal static MelonPreferences_Entry<string> NotificationLoggerName;
    internal static MelonPreferences_Entry<List<ushort>> NotificationLoggerColorARGB;
    internal static MelonPreferences_Entry<bool> LogHUDNotifications;
    internal static MelonPreferences_Entry<List<ushort>> LogHUDNotificationsColorARGB;
    internal static MelonPreferences_Entry<string> LogHUDNotificationsTemplate;
    internal static MelonPreferences_Entry<bool> LogHUDNotificationsPurgeNewlines;

    internal static MelonPreferences_Entry<bool> LogInstanceJoins;
    internal static MelonPreferences_Entry<List<ushort>> LogInstanceJoinsColorARGB;
    internal static MelonPreferences_Entry<string> LogInstanceJoinsTemplate;

    internal static MelonPreferences_Entry<bool> LogPlayerJoinLeaves;
    internal static MelonPreferences_Entry<bool> LogPlayerJoinLeaveRankColors;
    internal static MelonPreferences_Entry<List<ushort>> LogPlayerJoinColorARGB;
    internal static MelonPreferences_Entry<string> LogPlayerJoinTemplate;
    internal static MelonPreferences_Entry<List<ushort>> LogPlayerLeaveColorARGB;
    internal static MelonPreferences_Entry<string> LogPlayerLeaveTemplate;

    internal static MelonPreferences_Entry<bool> LogPropSpawns;
    internal static MelonPreferences_Entry<List<ushort>> LogPropSpawnsColorARGB;
    internal static MelonPreferences_Entry<string> LogPropSpawnsTemplate;

    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        LoggerName = _melonCategory.CreateEntry("Logger Prefix", AssemblyInfoParams.Name,
            description: "The prefix to use in the MelonLoader Console when logging", is_hidden: true);
        LoggerColorARGB = _melonCategory.CreateEntry("Logger Color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The prefix color to use in the MelonLoader Console when loggings", is_hidden: true);

        _BlacklistRegexes = _melonCategory.CreateEntry("Blacklist Regexes", new List<string>(),
            description: "List of regexes that will be ignored by the mod", is_hidden: true);

        NotificationLoggerName = _melonCategory.CreateEntry("HUD Notifications Logger Prefix", "Notification",
            description: "The prefix to use in the MelonLoader Console when logging HUD Notifications", is_hidden: true);
        NotificationLoggerColorARGB = _melonCategory.CreateEntry("HUD Notifications Logger Color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The prefix color to use in the MelonLoader Console when logging HUD Notifications", is_hidden: true);
        LogHUDNotifications = _melonCategory.CreateEntry("Log HUD Notifications", true,
            description: "Whether to log HUD notifications to MelonLoader console/log or not.");
        LogHUDNotificationsColorARGB = _melonCategory.CreateEntry("HUD Notification Log Color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging HUD Notifications");
        LogHUDNotificationsTemplate = _melonCategory.CreateEntry("HUD Notification Template", "[{0}] {1}: {2}",
            description: "The template to use for logging HUD Notifications (The following replacements are available: {0}=category,{1}=headline,{2}=small)");
        LogHUDNotificationsPurgeNewlines = _melonCategory.CreateEntry("Ignore HUD Notifications Newlines", true,
            description: "Whether to replace newlines \"\\n\" with spaces for HUD notifications.");

        LogInstanceJoins = _melonCategory.CreateEntry("Log joining Instances", false,
            description: "Whether to log switching instances (Uses CVRGameEventSystem.Instance.OnConnected)");
        LogInstanceJoinsColorARGB = _melonCategory.CreateEntry("Instance Join Log Color (ARGB)", new List<ushort> { 230, 255, 255, 255 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogInstanceJoinsTemplate = _melonCategory.CreateEntry("Instance Join Template", "[Instance] {1} ({0}) Privacy: {2} Players: {3} | Scene: {4} ({6}) | World: {8} ({7})",
            description: "The template to use for logging Instance Joins (The following replacements are available: {0}=instanceId,{1}=instanceName,{2}=instancePrivacy,{3}=playerCount,{4}=scene name,{5}=scene path,{6}=scene index,{7}=world id,{8}=world name)");
        // instanceId, instanceName, privacy, players, scene.name, scene.path, scene.buildIndex, worldId, worldName

        LogPlayerJoinLeaves = _melonCategory.CreateEntry("Log Players joining/leaving", false,
            description: "Whether to log players joining/leaving your instance (Uses CVRGameEventSystem.Player.OnJoin/OnLeave)");
        LogPlayerJoinLeaveRankColors = _melonCategory.CreateEntry("Use Rank Colors", false,
            description: "Wether to use rank colors when logging joins/leaves instead of custom colors", is_hidden: true);
        LogPlayerJoinColorARGB = _melonCategory.CreateEntry("Player Join Log Color (ARGB)", new List<ushort> { 230, 0, 255, 255 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogPlayerJoinTemplate = _melonCategory.CreateEntry("Player Join Template", "[+] {1} \"{0}\" [{2}]",
            description: "The template to use for logging players joining (The following replacements are available: {0}=name,{1}=rank,{2}=id)");
        LogPlayerLeaveColorARGB = _melonCategory.CreateEntry("Player Leave Log Color (ARGB)", new List<ushort> { 230, 255, 0, 0 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogPlayerLeaveTemplate = _melonCategory.CreateEntry("Player Leave Template", "[-] {1} \"{0}\" [{2}]",
            description: "The template to use for logging players leaving (The following replacements are available: {0}=name,{1}=rank,{2}=id)");

        LogPropSpawns = _melonCategory.CreateEntry("Log spawned props", false,
            description: "Wether to log props you spawn to console/MelonLoader log");
        LogPropSpawnsColorARGB = _melonCategory.CreateEntry("Prop Spawn Log Color (ARGB)", new List<ushort> { 250, 255, 255, 255 },
            description: "The color to use in the MelonLoader Console when logging prop spawns");
        LogPropSpawnsTemplate = _melonCategory.CreateEntry("Prop Spawn Template", "[Prop Spawned] {0} pos: {1} rot: {2}",
            description: "The template to use for logging prop spawns (The following replacements are available: {0}=id,{1}=position,{2}=rotation)");
    }

}
