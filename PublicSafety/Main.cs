using ABI_RC.Core.Networking.IO.Instancing;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using MelonLoader;

namespace Bluscream.PublicSafety;

public class PublicSafety : MelonMod {

    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            var privacy = Instances.GetPrivacy(MetaPort.Instance.CurrentInstancePrivacy);
            CommonMethods.SetSettings(privacy);
        });
    }

    internal static class CommonMethods {
        internal static void SetSettings(Instances.InstancePrivacyType instancePrivacyType) {
            if (!ModConfig.EnableMod.Value) return;
            switch (instancePrivacyType) {
                case Instances.InstancePrivacyType.Public:
                    MelonLogger.Msg($"Joined public instance, enforcing settings!");
                    if (ModConfig.EnableURLWhitelist.Value) MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", true);
                    break;
                default:
                    MelonLogger.Msg($"Joined non-public instance, relaxing settings!");
                    if (ModConfig.DisableURLWhitelist.Value) MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", false); // instancePrivacyType == Instances.InstancePrivacyType.Public
                    break;
            }
        }
    }
}
