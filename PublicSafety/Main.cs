using ABI_RC.Core.Networking.IO.Instancing;
using ABI_RC.Core.Networking.API.Responses;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using UnityEngine.SceneManagement;
using MelonLoader;
using static ABI_RC.Core.Networking.IO.Instancing.Instances;

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
            // MelonLogger.Msg($"[CVRGameEventSystem.Instance.OnConnected] Waiting for next RPC update");
            //if (instance != MetaPort.Instance.CurrentInstanceId)
            //    MelonLogger.Warning($"instance {instance} does not match {MetaPort.Instance.CurrentInstanceId}");
            Task.Factory.StartNew(() => CommonMethods.SetSettings(instance));
            //WaitingForRPC = true;
        });
    }

    internal static class CommonMethods {
        //internal static void LogStatusAsync(string? instanceId = null, bool api = true) {
        //    Task.Factory.StartNew(() => LogStatus(instanceId, api));
        //}
        //internal static void LogStatus(string? instanceId = null, bool useApi = true) {
        //    instanceId ??= MetaPort.Instance.CurrentInstanceId;
        //    var scene = SceneManager.GetActiveScene().name;
        //    var privacy = GetPrivacy(MetaPort.Instance.CurrentInstancePrivacy);
        //    string apiPrivacy = "";
        //    if (useApi) {
        //        var details = GetInstanceDetails(instanceId);
        //        apiPrivacy = details.Data.InstanceSettingPrivacy;
        //    }
        //    MelonLogger.Msg($"instanceId: {instanceId}\nScene: {scene}\nMetaPort.Instance.CurrentInstancePrivacy: {privacy}\napiPrivacy: {apiPrivacy}\ncurrent: {CurrentInstancePrivacyType}");
        //}
        //internal static BaseResponse<InstanceDetailsResponse> GetInstanceDetails(string instanceId) { // CURSED !!!
        //    BaseResponse<InstanceDetailsResponse> baseResponse = ApiConnection.MakeRequest<InstanceDetailsResponse>(ApiConnection.ApiOperation.InstanceDetail, new {
        //        instanceID = instanceId
        //    }, null, false).Result;
        //    CurrentInstance = baseResponse;
        //    CurrentInstancePrivacyType = GetPrivacy(baseResponse.Data.InstanceSettingPrivacy);
        //    MetaPort.Instance.CurrentInstancePrivacy = baseResponse.Data.InstanceSettingPrivacy;
        //    return baseResponse;
        //}
        internal static void SetSettings(string? instanceId = null, Instances.InstancePrivacyType? instancePrivacyType = null) {
            if (!ModConfig.EnableMod.Value) return;
            //instanceId ??= MetaPort.Instance.CurrentInstanceId;
            //var details = GetInstanceDetails(_instanceId);
            //var instancePrivacyString = details.Data.InstanceSettingPrivacy;
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
        }
    }

    //[HarmonyPatch]
    //internal class HarmonyPatches {
    //    [HarmonyPrefix]
    //    [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
    //    public static void Before_CVRWorld_Start() {
    //        MelonLogger.Msg("[Before_CVRWorld_Start]:");
    //        CommonMethods.LogStatusAsync();
    //    }

    //    [HarmonyPostfix]
    //    [HarmonyPatch(typeof(CVRWorld), nameof(CVRWorld.Start))]
    //    public static void After_CVRWorld_Start() {
    //        MelonLogger.Msg("[After_CVRWorld_Start]:");
    //        CommonMethods.LogStatusAsync();
    //    }

    //    [HarmonyPostfix]
    //    [HarmonyPatch(typeof(RichPresence), nameof(RichPresence.ReadPresenceUpdateFromNetwork))]
    //    public static void After_RichPresence_ReadPresenceUpdateFromNetwork() {
    //        try {
    //            MelonLogger.Warning("After_RichPresence_ReadPresenceUpdateFromNetwork:");
    //            CommonMethods.LogStatusAsync();
    //            if (WaitingForRPC) {
    //                WaitingForRPC = false;
    //                var privacy = GetPrivacy(MetaPort.Instance.CurrentInstancePrivacy);
    //                CommonMethods.SetSettings(privacy);
    //            }
    //        } catch (Exception e) {
    //            MelonLogger.Error($"Error during the patched function {nameof(After_RichPresence_ReadPresenceUpdateFromNetwork)}");
    //            MelonLogger.Error(e);
    //        }
    //    }
    //}
}
