using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications;
public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> WorldDownloadNotificationsEnabled;
    internal static MelonPreferences_Entry<string> WorldDownloadNotificationsTemplate;

    internal static MelonPreferences_Entry<bool> InstanceSwitchNotificationsEnabled;
    internal static MelonPreferences_Entry<string> InstanceSwitchNotificationsTemplate;
    internal static MelonPreferences_Entry<bool> InstanceSwitchNotificationsSoundEnabled;

    internal static MelonPreferences_Entry<bool> InstanceRejoinNotificationsEnabled;
    internal static MelonPreferences_Entry<string> InstanceRejoinNotificationsTemplate;
    internal static MelonPreferences_Entry<bool> InstanceRejoinNotificationsSoundEnabled;

    internal static MelonPreferences_Entry<bool> VRModeSwitchNotificationsEnabled;
    internal static MelonPreferences_Entry<string> VRModeSwitchNotificationsTemplateVR;
    internal static MelonPreferences_Entry<string> VRModeSwitchNotificationsTemplateDesktop;
    internal static MelonPreferences_Entry<bool> VRModeSwitchNotificationsSoundEnabled;


    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        WorldDownloadNotificationsEnabled = _melonCategory.CreateEntry("Enable world download notifications", false,
            description: "Will automatically send ChatBox notifications while you download a world");
        WorldDownloadNotificationsTemplate = _melonCategory.CreateEntry("World download template", "Loading World ({0}%)",
            description: "Template for world download notifications ({0}=percentage)");

        InstanceSwitchNotificationsEnabled = _melonCategory.CreateEntry("Enable instance switching notifications", true,
            description: "Will automatically send ChatBox notifications while when you switch to a different instance");
        InstanceSwitchNotificationsTemplate = _melonCategory.CreateEntry("Instance switching template", "Switching instance",
            description: "Template for instance switching notifications ({0}=instanceId,{1}=worldId,{2}=isHome)");
        InstanceSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("Enable instance switching notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        InstanceRejoinNotificationsEnabled = _melonCategory.CreateEntry("Enable instance rejoin notifications", true,
            description: "Will automatically send ChatBox notifications when you rejoin the current instance");
        InstanceRejoinNotificationsTemplate = _melonCategory.CreateEntry("Instance rejoin template", "Rejoining",
            description: "Template for instance rejoin notifications ({0}=instanceId,{1}=worldId,{2}=isHome)");
        InstanceRejoinNotificationsSoundEnabled = _melonCategory.CreateEntry("Enable instance rejoin notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        VRModeSwitchNotificationsEnabled = _melonCategory.CreateEntry("Enable VR Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between VR/Desktop mode");
        VRModeSwitchNotificationsTemplateVR = _melonCategory.CreateEntry("VR mode switch template", "Switched to VR",
            description: "Template for VR mode switch notifications");
        VRModeSwitchNotificationsTemplateDesktop = _melonCategory.CreateEntry("Desktop mode switch template", "Switched to Desktop",
            description: "Template for Desktop mode switch notifications");
        VRModeSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("Enable VR Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");
    }
}
