using Bluscream.VRModeSwitchHotkey.Properties;
using MelonLoader;
using UnityEngine;
using ABI_RC.Systems.VRModeSwitch;
using System.Collections;

namespace Bluscream.VRModeSwitchHotkey;
public class VRModeSwitchHotkey : MelonMod {
    public static MelonLogger.Instance Logger;
    private protected object RestartDelayRoutine = null;

    public override void OnInitializeMelon() {
        Logger = new MelonLogger.Instance(AssemblyInfoParams.Name, color: System.Drawing.Color.DarkCyan);
        ModConfig.InitializeMelonPrefs();
    }

    private static bool modifierKeyPressedOrMissing(KeyCode keyCode) => keyCode == KeyCode.None || Input.GetKeyDown(keyCode);

    public override void OnLateUpdate() {
        if (modifierKeyPressedOrMissing(ModConfig.RestartVRModeModifierKey.Value) && Input.GetKeyDown(ModConfig.RestartVRModeKey.Value)) {
            Logger.Msg($"Hotkey [{ModConfig.RestartVRModeModifierKey.Value}+{ModConfig.RestartVRModeKey.Value}] pressed, restarting VR mode...");
            if (RestartDelayRoutine != null) MelonCoroutines.Stop(RestartDelayRoutine); // Cancel previous
            RestartDelayRoutine = MelonCoroutines.Start(DelayRestartVRMode());
            ToggleVRMode();
        } else if (modifierKeyPressedOrMissing(ModConfig.SwitchVRModeModifierKey.Value) && Input.GetKeyDown(ModConfig.SwitchVRModeKey.Value)) {
            Logger.Msg($"Hotkey [{ModConfig.SwitchVRModeModifierKey.Value}+{ModConfig.SwitchVRModeKey.Value}] pressed, switching VR mode...");
            ToggleVRMode();
        }
    }

    internal static void ToggleVRMode() {
        if (!ModConfig.EnableMod.Value) return;
        //CVRInputManager.Instance.switchMode = true;
        VRModeSwitchManager.Instance.AttemptSwitch();
        //VRModeSwitchManager.Instance.StartSwitch();
    }

    private IEnumerator DelayRestartVRMode() {
        if (!ModConfig.EnableMod.Value) yield break;
        Logger.Msg($"Waiting {ModConfig.RestartVRModeDelay.Value}s before switching back to VR mode...");
        yield return new WaitForSeconds(ModConfig.RestartVRModeDelay.Value);
        ToggleVRMode();
    }
}
