using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications;
public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> WorldDownloadNotificationsEnabled;
    internal static MelonPreferences_Entry<string> WorldDownloadNotificationsTemplate;
    internal static MelonPreferences_Entry<TimeSpan> WorldDownloadNotificationsInterval;
    private static readonly TimeSpan WorldDownloadNotificationsIntervalMin = TimeSpan.FromMilliseconds(500);

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

    internal static MelonPreferences_Entry<bool> FBTModeSwitchNotificationsEnabled;
    internal static MelonPreferences_Entry<string> FBTModeSwitchNotificationsTemplateFBT;
    internal static MelonPreferences_Entry<string> FBTModeSwitchNotificationsTemplateHalfBody;
    internal static MelonPreferences_Entry<bool> FBTModeSwitchNotificationsSoundEnabled;

    internal static MelonPreferences_Entry<bool> VoiceConnectionLostNotificationEnabled;
    internal static MelonPreferences_Entry<string> VoiceConnectionLostNotificationTemplateLost;
    internal static MelonPreferences_Entry<string> VoiceConnectionLostNotificationTemplateGained;
    internal static MelonPreferences_Entry<bool> VoiceConnectionLostNotificationSoundEnabled;

    internal static MelonPreferences_Entry<bool> MicrophoneNotificationEnabled;
    internal static MelonPreferences_Entry<string> MicrophoneNotificationTemplateMuted;
    internal static MelonPreferences_Entry<string> MicrophoneNotificationTemplateUnmuted;
    internal static MelonPreferences_Entry<bool> MicrophoneNotificationSoundEnabled;



    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        WorldDownloadNotificationsEnabled = _melonCategory.CreateEntry("World download notifications", false,
            description: "Will automatically send ChatBox notifications while you download a world");
        WorldDownloadNotificationsTemplate = _melonCategory.CreateEntry("World download template", "Loading World ({0}%)",
            description: "Template for world download notifications ({0}=percentage)");
        WorldDownloadNotificationsInterval = _melonCategory.CreateEntry("World download interval", TimeSpan.FromSeconds(1),
            description: "Delay to use between update intervals (min: 500ms)");
        if (WorldDownloadNotificationsInterval.Value < WorldDownloadNotificationsIntervalMin)
            WorldDownloadNotificationsInterval.Value = WorldDownloadNotificationsIntervalMin;

        InstanceSwitchNotificationsEnabled = _melonCategory.CreateEntry("Instance switching notifications", true,
            description: "Will automatically send ChatBox notifications while when you switch to a different instance");
        InstanceSwitchNotificationsTemplate = _melonCategory.CreateEntry("Instance switching template", "Switching Instance",
            description: "Template for instance switching notifications ({0}=instanceId,{1}=worldId,{2}=isHome)");
        InstanceSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("Instance switching notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        InstanceRejoinNotificationsEnabled = _melonCategory.CreateEntry("Instance rejoin notifications", true,
            description: "Will automatically send ChatBox notifications when you rejoin the current instance");
        InstanceRejoinNotificationsTemplate = _melonCategory.CreateEntry("Instance rejoin template", "Rejoining",
            description: "Template for instance rejoin notifications ({0}=instanceId,{1}=worldId,{2}=isHome)");
        InstanceRejoinNotificationsSoundEnabled = _melonCategory.CreateEntry("Instance rejoin notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        VRModeSwitchNotificationsEnabled = _melonCategory.CreateEntry("VR Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between VR/Desktop mode");
        VRModeSwitchNotificationsTemplateVR = _melonCategory.CreateEntry("VR mode switch template", "Switched to VR",
            description: "Template for VR mode switch notifications");
        VRModeSwitchNotificationsTemplateDesktop = _melonCategory.CreateEntry("Desktop mode switch template", "Switched to Desktop",
            description: "Template for Desktop mode switch notifications");
        VRModeSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("VR Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        FBTModeSwitchNotificationsEnabled = _melonCategory.CreateEntry("FBT Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between FBT/Halfbody mode");
        FBTModeSwitchNotificationsTemplateFBT = _melonCategory.CreateEntry("FBT mode switch template", "Switched to FBT",
            description: "Template for FBT mode switch notifications");
        FBTModeSwitchNotificationsTemplateHalfBody = _melonCategory.CreateEntry("Halfbody mode switch template", "Switched to Halfbody",
            description: "Template for Halfbody mode switch notifications");
        FBTModeSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("FBT Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        VoiceConnectionLostNotificationEnabled = _melonCategory.CreateEntry("Voice connection lost notifications", false,
            description: "Will automatically send ChatBox notifications when you loose connection to the Voice System (preferably use the VoiceConnectionStatus mod instead)");
        VoiceConnectionLostNotificationTemplateLost = _melonCategory.CreateEntry("Voice connection lost template", "Voice Disconnected",
            description: "Template for voice connection lost notifications");
        VoiceConnectionLostNotificationTemplateGained = _melonCategory.CreateEntry("Voice connection regained template", "Voice Connected",
            description: "Template for voice connection regained notifications");
        VoiceConnectionLostNotificationSoundEnabled = _melonCategory.CreateEntry("Voice connection lost notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        MicrophoneNotificationEnabled = _melonCategory.CreateEntry("Microphone notifications", false,
            description: "Will automatically send ChatBox notifications when you loose connection to the Voice System (preferably use the VoiceConnectionStatus mod instead)");
        MicrophoneNotificationTemplateMuted = _melonCategory.CreateEntry("Microphone muted template", "Microphone muted",
            description: "Template for Microphone muted notifications");
        MicrophoneNotificationTemplateUnmuted = _melonCategory.CreateEntry("Microphone unmuted template", "Microphone unmuted",
            description: "Template for Microphone unmuted notifications");
        MicrophoneNotificationSoundEnabled = _melonCategory.CreateEntry("Microphone notification sound", false,
            description: "Will play a sound to other users when the notification is sent");
    }
}
