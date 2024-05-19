using MelonLoader;
using Bluscream.MoreLogging.Properties;
using Bluscream.MoreLogging;

namespace Bluscream.MoreLogging.Integrations;
internal class ChatBox {
    public static MelonLogger.Instance Logger;
    internal class UiEvent {
        event Action m_action;
        public void AddHandler(Action p_listener) => m_action += p_listener;
        public void RemoveHandler(Action p_listener) => m_action -= p_listener;
        public void Invoke() => m_action?.Invoke();
    }

    internal static void Initialize() {
        if (MelonMod.RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") is null) return;
        IntegrationConfig.InitializeMelonPrefs();
        Logger = new MelonLogger.Instance(IntegrationConfig.LoggerName.Value, IntegrationConfig.LoggerColorARGB.Value.ToColor());

        Kafe.ChatBox.API.OnMessageReceived += OnChatMessage;
        Kafe.ChatBox.API.OnMessageSent += OnChatMessage;
    }

    private static void OnChatMessage(Kafe.ChatBox.API.ChatBoxMessage msg) {
        if (!ModConfig.EnableMod.Value) return;
        var isPlayerMessage = msg.DisplayOnChatBox && msg.Source != Kafe.ChatBox.API.MessageSource.Mod;
        var senderIsLocalPlayer = Utils.IsLocalPlayer(msg.SenderGuid);
        var senderName = Utils.GetPlayerNameById(msg.SenderGuid);
        if (senderIsLocalPlayer) {
            if (IntegrationConfig.LogOutgoingChat.Value && isPlayerMessage) {
                Logger.Msg(IntegrationConfig.LogOutgoingChatColorARGB.Value.ToColor(),
                    string.Format(IntegrationConfig.LogOutgoingChatTemplate.Value,
                                          msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
            } else if (IntegrationConfig.LogOutgoingMod.Value && !isPlayerMessage) {
                Logger.Msg(IntegrationConfig.LogOutgoingModColorARGB.Value.ToColor(),
                    string.Format(IntegrationConfig.LogOutgoingModTemplate.Value,
                                          msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
            }
        } else {
            if (isPlayerMessage) {
                if (!msg.TriggerNotification && IntegrationConfig.LogIncomingChatToHUD.Value) Utils.HUDNotify(senderName, msg.Message, "Chat", 5f);
                if (IntegrationConfig.LogIncomingChat.Value) {
                    Logger.Msg(IntegrationConfig.LogIncomingChatColorARGB.Value.ToColor(),
                        string.Format(IntegrationConfig.LogIncomingChatTemplate.Value,
                                              msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
                }
            } else if (!isPlayerMessage && IntegrationConfig.LogIncomingMod.Value) {
                Logger.Msg(IntegrationConfig.LogIncomingModColorARGB.Value.ToColor(),
                    string.Format(IntegrationConfig.LogIncomingModTemplate.Value,
                                          msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
            }
        }
    }

    public static class IntegrationConfig {
        private static MelonPreferences_Category _melonCategory;

        internal static MelonPreferences_Entry<string> LoggerName;
        internal static MelonPreferences_Entry<List<ushort>> LoggerColorARGB;

        internal static MelonPreferences_Entry<bool> LogIncomingChat;
        internal static MelonPreferences_Entry<bool> LogIncomingChatToHUD;
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


        public static void InitializeMelonPrefs() {
            _melonCategory = MelonPreferences.GetCategory(AssemblyInfoParams.Name);

            LoggerName = _melonCategory.CreateEntry("Chat logger prefix", "Chat",
                description: "The prefix to use in the MelonLoader Console when logging chat messages");
            LoggerColorARGB = _melonCategory.CreateEntry("Chat logger prefix color", new List<ushort> { 255, 255, 60, 0 },
                description: "The prefix color to use in the MelonLoader Console when logging chat messages");

            LogIncomingChat = _melonCategory.CreateEntry("Log incoming chat messages", true,
                description: "Whether to log incoming chat messages to MelonLoader console/log or not.");
            LogIncomingChatColorARGB = _melonCategory.CreateEntry("Incoming chat messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
                description: "The color to use in the MelonLoader Console when logging incoming chat messages");
            LogIncomingChatTemplate = _melonCategory.CreateEntry("Incoming chat messages template", "Message from {1}: \"{0}\"",
                description: "The template to use for logging incoming chat messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");
            LogIncomingChatToHUD = _melonCategory.CreateEntry("Incoming chat to HUD Notification", false,
                description: $"Whether to show incoming chat messages as HUD notifications or not (does not require enabling {LogIncomingChat.DisplayName}");

            LogIncomingMod = _melonCategory.CreateEntry("Log incoming mod messages", false,
                description: "Whether to log incoming mod messages to MelonLoader console/log or not.");
            LogIncomingModColorARGB = _melonCategory.CreateEntry("Incoming mod messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
                description: "The color to use in the MelonLoader Console when logging incoming mod messages");
            LogIncomingModTemplate = _melonCategory.CreateEntry("Incoming mod messages template", "Mod Message from {1} via \"{4}\": \"{0}\"",
                description: "The template to use for logging incoming mod messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");

            LogOutgoingChat = _melonCategory.CreateEntry("Log outgoing chat messages", true,
                description: "Whether to log outgoing chat messages to MelonLoader console/log or not.");
            LogOutgoingChatColorARGB = _melonCategory.CreateEntry("Outgoing chat messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
                description: "The color to use in the MelonLoader Console when logging outgoing chat messages");
            LogOutgoingChatTemplate = _melonCategory.CreateEntry("Outgoing chat messages template", "Message from {1}: \"{0}\"",
                description: "The template to use for logging outgoing chat messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");

            LogOutgoingMod = _melonCategory.CreateEntry("Log outgoing mod messages", false,
                description: "Whether to log outgoing mod messages to MelonLoader console/log or not.");
            LogOutgoingModColorARGB = _melonCategory.CreateEntry("Outgoing mod messages color (ARGB)", new List<ushort> { 255, 255, 60, 0 },
                description: "The color to use in the MelonLoader Console when logging outgoing mod messages");
            LogOutgoingModTemplate = _melonCategory.CreateEntry("Outgoing mod messages template", "Mod Message from {1} via \"{4}\": \"{0}\"",
                description: "The template to use for logging outgoing mod messages (The following replacements are available: {0}=message,{1}=userName,{2}=userId,{3}=source,{4}=modname)");
        }

    }

}
