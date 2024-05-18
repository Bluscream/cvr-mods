using Bluscream.VRModeSwitchHotkey.Properties;
using MelonLoader;
using UnityEngine;
using ABI_RC.Systems.VRModeSwitch;

namespace Bluscream.VRModeSwitchHotkey;
public class VRModeSwitchHotkey : MelonMod {
    public static MelonLogger.Instance Logger;

    public override void OnInitializeMelon() {
        Logger = new MelonLogger.Instance(AssemblyInfoParams.Name, color: System.Drawing.Color.DarkCyan);
        ModConfig.InitializeMelonPrefs();
    }

    public override void OnLateUpdate() {
        var modifierKeyPressedOrMissing = ModConfig.ModifierKey.Value == KeyCode.None || Input.GetKeyDown(ModConfig.ModifierKey.Value);
        if (modifierKeyPressedOrMissing && Input.GetKeyDown(ModConfig.HotKey.Value)) {
            ToggleVRMode();
        }
    }

    internal static void ToggleVRMode() {
        if (!ModConfig.EnableMod.Value) return;
        Logger.Msg($"Hotkey pressed, switching VR mode...");
        //CVRInputManager.Instance.switchMode = true;
        VRModeSwitchManager.Instance.AttemptSwitch();
        //VRModeSwitchManager.Instance.StartSwitch();
    }
}
