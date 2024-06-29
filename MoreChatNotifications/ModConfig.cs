using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications;
public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;
    internal static MelonPreferences_Entry<bool> EnableLogging;
    internal static MelonPreferences_Entry<bool> EnableHUDNotifications;
    internal static MelonPreferences_Entry<bool> EnableChatNotifications;

    internal static MelonPreferences_Entry<bool> WorldDownloadNotificationsEnabled;
    internal static MelonPreferences_Entry<string> WorldDownloadNotificationsTemplate;
    internal static MelonPreferences_Entry<int> WorldDownloadNotificationsIntervalMS;
    private static readonly int WorldDownloadNotificationsIntervalMinMS = 250;

    internal static MelonPreferences_Entry<bool> InstanceSwitchNotificationsEnabled;
    internal static MelonPreferences_Entry<string> InstanceSwitchNotificationsTemplate;
    internal static MelonPreferences_Entry<bool> InstanceSwitchNotificationsSoundEnabled;

    internal static MelonPreferences_Entry<bool> InstanceRejoinNotificationsEnabled;
    internal static MelonPreferences_Entry<string> InstanceRejoinNotificationsTemplate;
    internal static MelonPreferences_Entry<bool> InstanceRejoinNotificationsSoundEnabled;


    internal static MelonPreferences_Entry<bool> FlightModeSwitchNotificationEnabled;
    internal static MelonPreferences_Entry<string> FlightModeSwitchNotificationTemplateEnabled;
    internal static MelonPreferences_Entry<string> FlightModeSwitchNotificationTemplateDisabled;
    internal static MelonPreferences_Entry<bool> FlightModeSwitchNotificationSoundEnabled;

    internal static MelonPreferences_Entry<bool> VRModeSwitchNotificationsEnabled;
    internal static MelonPreferences_Entry<string> VRModeSwitchNotificationsTemplateVR;
    internal static MelonPreferences_Entry<string> VRModeSwitchNotificationsTemplateDesktop;
    internal static MelonPreferences_Entry<bool> VRModeSwitchNotificationsSoundEnabled;

    internal static MelonPreferences_Entry<bool> SeatedModeSwitchNotificationEnabled;
    internal static MelonPreferences_Entry<string> SeatedModeSwitchNotificationTemplateEnabled;
    internal static MelonPreferences_Entry<string> SeatedModeSwitchNotificationTemplateDisabled;
    internal static MelonPreferences_Entry<bool> SeatedModeSwitchNotificationSoundEnabled;

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

    internal static MelonPreferences_Entry<bool> InviteNotificationEnabled;
    internal static MelonPreferences_Entry<string> InviteNotificationTemplateInvited;
    internal static MelonPreferences_Entry<string> InviteNotificationTemplateInviteRequested;
    internal static MelonPreferences_Entry<string> InviteNotificationTemplateFriendRequest;
    internal static MelonPreferences_Entry<bool> InviteNotificationSoundEnabled;



    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");
        EnableLogging = _melonCategory.CreateEntry("Enable Logging", true,
            description: "The mod will write nothing to the MelonLoader Console/Logfile while this is disabled");
        EnableHUDNotifications = _melonCategory.CreateEntry("Enable HUD Notifications", true,
            description: "The mod will show no HUD notifications while this is disabled");
        EnableChatNotifications = _melonCategory.CreateEntry("Enable ChatBox Notifications", true,
            description: "The mod will send no Chat notifications while this is disabled");

        WorldDownloadNotificationsEnabled = _melonCategory.CreateEntry("World download notifications", false,
            description: "Will automatically send ChatBox notifications while you download a world");
        WorldDownloadNotificationsTemplate = _melonCategory.CreateEntry("World download template", "Loading World ({0}%)",
            description: "Template for world download notifications ({0}=percentage)");
        WorldDownloadNotificationsIntervalMS = _melonCategory.CreateEntry("World download interval (ms)", 1000,
            description: $"Delay to use between update intervals in milliseconds (min: {WorldDownloadNotificationsIntervalMinMS}ms)");
        if (WorldDownloadNotificationsIntervalMS.Value < WorldDownloadNotificationsIntervalMinMS)
            WorldDownloadNotificationsIntervalMS.Value = WorldDownloadNotificationsIntervalMinMS;

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

        FlightModeSwitchNotificationEnabled = _melonCategory.CreateEntry("Flight Mode switch notifications", false,
            description: "Will automatically send ChatBox notifications when you toggle Flight mode");
        FlightModeSwitchNotificationTemplateEnabled = _melonCategory.CreateEntry("Flight mode enable template", "Enabled Flight Mode",
            description: "Template for Flight mode disable notifications");
        FlightModeSwitchNotificationTemplateDisabled = _melonCategory.CreateEntry("Flight mode disable template", "Disabled Flight Mode",
            description: "Template for Flight mode enable notifications");
        FlightModeSwitchNotificationSoundEnabled = _melonCategory.CreateEntry("Flight mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent", is_hidden: true);

        VRModeSwitchNotificationsEnabled = _melonCategory.CreateEntry("VR Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between VR/Desktop mode");
        VRModeSwitchNotificationsTemplateVR = _melonCategory.CreateEntry("VR mode switch template", "Switched to VR",
            description: "Template for VR mode switch notifications");
        VRModeSwitchNotificationsTemplateDesktop = _melonCategory.CreateEntry("Desktop mode switch template", "Switched to Desktop",
            description: "Template for Desktop mode switch notifications");
        VRModeSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("VR Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        SeatedModeSwitchNotificationEnabled = _melonCategory.CreateEntry("Seated Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between Seated/Standing mode");
        SeatedModeSwitchNotificationTemplateEnabled = _melonCategory.CreateEntry("Seated mode template", "Switched to Seated Mode",
            description: "Template for Seated mode switch notifications");
        SeatedModeSwitchNotificationTemplateDisabled = _melonCategory.CreateEntry("Standing mode template", "Switched to Standing Mode",
            description: "Template for Standing mode switch notifications");
        SeatedModeSwitchNotificationSoundEnabled = _melonCategory.CreateEntry("Seated Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        FBTModeSwitchNotificationsEnabled = _melonCategory.CreateEntry("FBT Mode switch notifications", true,
            description: "Will automatically send ChatBox notifications when you switch between FBT/Halfbody mode");
        FBTModeSwitchNotificationsTemplateFBT = _melonCategory.CreateEntry("FBT switch template", "Switched to FBT",
            description: "Template for FBT mode switch notifications");
        FBTModeSwitchNotificationsTemplateHalfBody = _melonCategory.CreateEntry("Halfbody switch template", "Switched to Halfbody",
            description: "Template for Halfbody mode switch notifications");
        FBTModeSwitchNotificationsSoundEnabled = _melonCategory.CreateEntry("FBT Mode switch notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        VoiceConnectionLostNotificationEnabled = _melonCategory.CreateEntry("Voice lost notifications", false,
            description: "Will automatically send ChatBox notifications when you loose connection to the Voice System (preferably use the VoiceConnectionStatus mod instead)");
        VoiceConnectionLostNotificationTemplateLost = _melonCategory.CreateEntry("Voice lost template", "Voice Disconnected",
            description: "Template for voice lost notifications");
        VoiceConnectionLostNotificationTemplateGained = _melonCategory.CreateEntry("Voice regained template", "Voice Connected",
            description: "Template for voice connection regained notifications");
        VoiceConnectionLostNotificationSoundEnabled = _melonCategory.CreateEntry("Voice lost notification sound", false,
            description: "Will play a sound to other users when the notification is sent");

        MicrophoneNotificationEnabled = _melonCategory.CreateEntry("Microphone notifications", false,
            description: "Will automatically send ChatBox notifications when you loose connection to the Voice System (preferably use the VoiceConnectionStatus mod instead)");
        MicrophoneNotificationTemplateMuted = _melonCategory.CreateEntry("Microphone muted template", "Microphone muted",
            description: "Template for Microphone muted notifications");
        MicrophoneNotificationTemplateUnmuted = _melonCategory.CreateEntry("Microphone unmuted template", "Microphone unmuted",
            description: "Template for Microphone unmuted notifications");
        MicrophoneNotificationSoundEnabled = _melonCategory.CreateEntry("Microphone notification sound", false,
            description: "Will play a sound to other users when the notification is sent", is_hidden: true);

        InviteNotificationEnabled = _melonCategory.CreateEntry("Invite notifications", false,
            description: "Will automatically send ChatBox notifications when you get invited to another instance");
        InviteNotificationTemplateInvited = _melonCategory.CreateEntry("Invited template", "Got Invited", description: "Template for invites");
        InviteNotificationTemplateInviteRequested = _melonCategory.CreateEntry("Invite request template", "Got Invite Request", description: "Template for invite requests");
        InviteNotificationTemplateFriendRequest = _melonCategory.CreateEntry("Friend request template", "Got Friend Request", description: "Template for friend requests");
        //InviteNotificationTemplateInvited = _melonCategory.CreateEntry("Invited template", "Got Invited to {0} by {1}", description: "Template for invites ({0}=world name,{1}=requester name)");
        //InviteNotificationTemplateInviteRequested = _melonCategory.CreateEntry("Invite request template", "Got Invite Request from {0}", description: "Template for invite requests ({0}=requester name)");
        //InviteNotificationTemplateFriendRequest = _melonCategory.CreateEntry("Friend request template", "Got Friend Request from {0}", description: "Template for friend requests ({0}=requester name)");
        InviteNotificationSoundEnabled = _melonCategory.CreateEntry("Invite notification sound", false,
            description: "Will play a sound to other users when the notification is sent");
    }
}
