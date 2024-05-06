using ABI_RC.Core.Networking.IO.Instancing;
using ABI_RC.Core.Networking;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using MelonLoader;
using HarmonyLib;

namespace Bluscream.PublicSafety;

public class PublicSafety : MelonMod {
    internal static bool WaitingForRPC = false;
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            WaitingForRPC = true;
        });
    }

    internal static class CommonMethods {
        internal static void SetSettings(Instances.InstancePrivacyType instancePrivacyType) {
            if (!ModConfig.EnableMod.Value) return;
            switch (instancePrivacyType) {
                case Instances.InstancePrivacyType.Public:
                    MelonLogger.Msg($"Joined public instance, enforcing settings!");
                    if (ModConfig.EnableURLWhitelist.Value) {
                        MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", true);
                    }
                    break;
                default:
                    MelonLogger.Msg($"Joined non-public instance, relaxing settings!");
                    if (ModConfig.DisableURLWhitelist.Value) {
                        MetaPort.Instance.settings.SetSettingsBool("GeneralVideoPlayerEnableWhitelist", false); // instancePrivacyType == Instances.InstancePrivacyType.Public
                    }
                    break;
            }
        }
    }

    [HarmonyPatch]
    internal class HarmonyPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RichPresence), nameof(RichPresence.ReadPresenceUpdateFromNetwork))]
        public static void After_RichPresence_ReadPresenceUpdateFromNetwork() {
            try {
                if (WaitingForRPC) {
                    WaitingForRPC = false;
                    var privacy = Instances.GetPrivacy(MetaPort.Instance.CurrentInstancePrivacy);
                    CommonMethods.SetSettings(privacy);
                }
            } catch (Exception e) {
                MelonLogger.Error($"Error during the patched function {nameof(After_RichPresence_ReadPresenceUpdateFromNetwork)}");
                MelonLogger.Error(e);
            }
        }
    }
}
