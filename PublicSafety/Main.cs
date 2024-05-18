using ABI_RC.Core.Networking.IO.Instancing;
using ABI_RC.Core.Networking.API.Responses;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using UnityEngine.SceneManagement;
using MelonLoader;
using static ABI_RC.Core.Networking.IO.Instancing.Instances;
using ABI_RC.Core.InteractionSystem;

namespace Bluscream.PublicSafety;

public class PublicSafety : MelonMod {

    public static InstancePrivacyType GetPrivacy(string privacy) {
        return privacy.ToLowerInvariant() switch {
            "public" => InstancePrivacyType.Public,
            "friendsoffriends" => InstancePrivacyType.FriendsOfFriends,
            "friends" => InstancePrivacyType.Friends,
            "group" => InstancePrivacyType.Group,
            "ownermustinvite" => InstancePrivacyType.OwnerMustInvite,
            "everyonecaninvite" => InstancePrivacyType.EveryoneCanInvite,
            _ => InstancePrivacyType.OwnerMustInvite,
        };
    }

    public static BaseResponse<InstanceDetailsResponse> CurrentInstance;
    public static Instances.InstancePrivacyType CurrentInstancePrivacyType;
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            //Task.Factory.StartNew(() => CommonMethods.SetSettings(instance));
            CommonMethods.SetSettings();
        });
    }

    internal static class CommonMethods {
        internal static void SetSettings(Instances.InstancePrivacyType? instancePrivacyType = null) {
            if (!ModConfig.EnableMod.Value) return;
            instancePrivacyType ??= GetPrivacy(MetaPort.Instance.CurrentInstancePrivacy);
            var scene = SceneManager.GetActiveScene().name;
            switch (instancePrivacyType) {
                case Instances.InstancePrivacyType.Public:
                    MelonLogger.Msg($"Joined {instancePrivacyType} instance {scene}, enforcing settings!");
                    if (ModConfig.EnableURLWhitelist.Value) MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", true);
                    if (ModConfig.EnableAdvancedSafety.Value) MetaPort.Instance.settings.SetSettingsBool("ExperimentalAdvancedSafetyEnabled", true);
                    if (ModConfig.EnableRichPresence.Value) {
                        MetaPort.Instance.settings.SetSettingsBool("ImplementationRichPresenceDiscordEnabled", true);
                        MetaPort.Instance.settings.SetSettingsBool("ImplementationRichPresenceSteamEnabled", true);
                    }
                    break;
                default:
                    MelonLogger.Msg($"Joined {instancePrivacyType} instance {scene}, relaxing settings!");
                    if (ModConfig.DisableURLWhitelist.Value) MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", false); // instancePrivacyType == Instances.InstancePrivacyType.Public
                    if (ModConfig.DisableAdvancedSafety.Value) MetaPort.Instance.settings.SetSettingsBool("ExperimentalAdvancedSafetyEnabled", false);
                    if (ModConfig.DisableRichPresence.Value) {
                        MetaPort.Instance.settings.SetSettingsBool("ImplementationRichPresenceDiscordEnabled", false);
                        MetaPort.Instance.settings.SetSettingsBool("ImplementationRichPresenceSteamEnabled", false);
                    }
                    break;
            }
            SaveChanges();
        }
        public static void SaveChanges() {
            MetaPort.Instance.SaveGameConfig();
            ViewManager.Instance.gameMenuView.View.TriggerEvent("CVRAppActionLoadSettings");
        }
    }
}
