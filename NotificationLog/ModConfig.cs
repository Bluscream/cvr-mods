using Bluscream.NotificationLog.Properties;
using MelonLoader;
using System.Drawing;
using System.Collections.Generic;

namespace Bluscream.NotificationLog;

public static class ModConfig {

    // Melon Prefs
    private static MelonPreferences_Category _melonCategory;

    internal static MelonPreferences_Entry<bool> MeLogHUDNotifications;
    internal static MelonPreferences_Entry<List<ushort>> MeLogHUDNotificationsColorARGB;
    internal static MelonPreferences_Entry<string> MeLogHUDNotificationsTemplate;

    public static void InitializeMelonPrefs() {

        // Melon Config
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);

        MeLogHUDNotifications = _melonCategory.CreateEntry("Log HUD Notifications", true,
            description: "Whether to log HUD notifications to MelonLoader console/log or not.");
        MeLogHUDNotificationsColorARGB = _melonCategory.CreateEntry("HUD Notification Log Color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
            description: "The color to use in the MelonLoader Console when logging HUD Notifications");
        MeLogHUDNotificationsTemplate = _melonCategory.CreateEntry("HUD Notification Log Template", "[{category}] {headline}: {small}",
            description: "The template to use for logging HUD Notifications (The following replacements are available: {category},{headline},{small})");

    }

}
