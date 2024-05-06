﻿using Bluscream.NotificationLog.Properties;
using MelonLoader;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Bluscream.NotificationLog;

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

    internal static MelonPreferences_Entry<List<string>> _BlacklistRegexes;
    internal static List<Regex> BlacklistRegexes {
        get { return _BlacklistRegexes.Value.Select(r => new Regex(r)).ToList(); }
    }

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

    internal static Color GetColor(List<ushort> _c) => Color.FromArgb(_c[0], _c[1], _c[2], _c[3]);  // cursed

    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable NotificationLog", true,
            description: "The mod will do nothing while disabled");

        _BlacklistRegexes = _melonCategory.CreateEntry("Blacklist Regexes", new List<string>(),
            description: "List of regexes that will be ignored by the mod", is_hidden: true);

        LogHUDNotifications = _melonCategory.CreateEntry("Log HUD Notifications", true,
            description: "Whether to log HUD notifications to MelonLoader console/log or not.");
        LogHUDNotificationsColorARGB = _melonCategory.CreateEntry("HUD Notification Log Color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging HUD Notifications");
        LogHUDNotificationsTemplate = _melonCategory.CreateEntry("HUD Notification Log Template", "[{0}] {1}: {2}",
            description: "The template to use for logging HUD Notifications (The following replacements are available: {0}=category,{1}=headline,{2}=small)");
        LogHUDNotificationsPurgeNewlines = _melonCategory.CreateEntry("Ignore HUD Notifications Newlines", true,
            description: "Whether to replace newlines \"\\n\" with spaces for HUD notifications.");

        LogInstanceJoins = _melonCategory.CreateEntry("Log joining Instances", false,
            description: "Whether to log switching instances (Uses CVRGameEventSystem.Instance.OnConnected)");
        LogInstanceJoinsColorARGB = _melonCategory.CreateEntry("Instance Join Log Color (ARGB)", new List<ushort> { 230, 255, 255, 255 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogInstanceJoinsTemplate = _melonCategory.CreateEntry("Instance Join Log Template", "[Instance] {0} Privacy: {1} | Players: {2} | Scene: {3}/{4} ({5})",
            description: "The template to use for logging Instance Joins (The following replacements are available: {0}=name,{1}=privacy,{2}=players),{3}=scene name),{4}=scene path),{5}=scene id)");

        LogPlayerJoinLeaves = _melonCategory.CreateEntry("Log Players joining/leaving", false,
            description: "Whether to log players joining/leaving your instance (Uses CVRGameEventSystem.Player.OnJoin/OnLeave)");
        LogPlayerJoinLeaveRankColors = _melonCategory.CreateEntry("Use Rank Colors", false,
            description: "Wether to use rank colors when logging joins/leaves instead of custom colors", is_hidden: true);
        LogPlayerJoinColorARGB = _melonCategory.CreateEntry("Player Join Log Color (ARGB)", new List<ushort> { 230, 0, 255, 255 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogPlayerJoinTemplate = _melonCategory.CreateEntry("Player Join Log Template", "[+] {1} \"{0}\" [{2}]",
            description: "The template to use for logging players joining (The following replacements are available: {0}=name,{1}=rank,{2}=id)");
        LogPlayerLeaveColorARGB = _melonCategory.CreateEntry("Player Leave Log Color (ARGB)", new List<ushort> { 230, 255, 0, 0 },
            description: "The color to use in the MelonLoader Console when logging Instance Joins");
        LogPlayerLeaveTemplate = _melonCategory.CreateEntry("Player Leave Log Template", "[-] {1} \"{0}\" [{2}]",
            description: "The template to use for logging players leaving (The following replacements are available: {0}=name,{1}=rank,{2}=id)");
    }

}