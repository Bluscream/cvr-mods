using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using MelonLoader;

namespace Bluscream.ChatLog;

public class ChatLog : MelonMod {
    public static MelonLogger.Instance Logger;

    public override void OnInitializeMelon() {
        Logger = new MelonLogger.Instance("Chat");
        ModConfig.InitializeMelonPrefs();

        if (RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") != null) {
            Logger.BigError("Chatbox mod not found! Make sure it is properly installed.");
            return;
        }

        Kafe.ChatBox.API.OnMessageReceived += OnChatMessageRecieved;
        Kafe.ChatBox.API.OnMessageSent += OnChatMessageSent;
    }

    private string GetPlayerNameById(string playerId) {
        if (playerId == MetaPort.Instance.ownerId) {
            return "You";
        }
        return "\"" + CVRPlayerManager.Instance.TryGetPlayerName(playerId) + "\"";
    }

    private void OnChatMessageSent(Kafe.ChatBox.API.ChatBoxMessage msg) {
        if (!ModConfig.EnableMod.Value) return;
        var isPlayerMessage = msg.DisplayOnChatBox && msg.Source != Kafe.ChatBox.API.MessageSource.Mod;
        var senderName = GetPlayerNameById(msg.SenderGuid);
        if (ModConfig.LogOutgoingChat.Value && isPlayerMessage) {
            Logger.Msg(ModConfig.GetColor(ModConfig.LogOutgoingChatColorARGB.Value),
                string.Format(ModConfig.LogOutgoingChatTemplate.Value,
                                      msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
        } else if (ModConfig.LogOutgoingMod.Value && !isPlayerMessage) {
            Logger.Msg(ModConfig.GetColor(ModConfig.LogOutgoingModColorARGB.Value),
                string.Format(ModConfig.LogOutgoingModTemplate.Value,
                                      msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
        }
    }

    private void OnChatMessageRecieved(Kafe.ChatBox.API.ChatBoxMessage msg) {
        if (!ModConfig.EnableMod.Value) return;
        var isPlayerMessage = msg.DisplayOnChatBox && msg.Source != Kafe.ChatBox.API.MessageSource.Mod;
        var senderName = GetPlayerNameById(msg.SenderGuid);
        if (ModConfig.LogIncomingChat.Value && isPlayerMessage) {
            Logger.Msg(ModConfig.GetColor(ModConfig.LogIncomingChatColorARGB.Value),
                string.Format(ModConfig.LogIncomingChatTemplate.Value,
                                      msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
        } else if (ModConfig.LogIncomingMod.Value && !isPlayerMessage) {
            Logger.Msg(ModConfig.GetColor(ModConfig.LogIncomingModColorARGB.Value),
                string.Format(ModConfig.LogIncomingModTemplate.Value,
                                      msg.Message, senderName, msg.SenderGuid, msg.Source, msg.ModName));
        }
    }
}
