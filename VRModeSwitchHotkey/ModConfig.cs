using Bluscream.VRModeSwitchHotkey.Properties;
using MelonLoader;
using UnityEngine;

namespace Bluscream.VRModeSwitchHotkey;
public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<KeyCode> SwitchVRModeModifierKey;
    internal static MelonPreferences_Entry<KeyCode> SwitchVRModeKey;

    internal static MelonPreferences_Entry<KeyCode> RestartVRModeModifierKey;
    internal static MelonPreferences_Entry<KeyCode> RestartVRModeKey;
    internal static MelonPreferences_Entry<int> RestartVRModeDelay;


    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        SwitchVRModeModifierKey = _melonCategory.CreateEntry("Switch VR Modifier Key", KeyCode.None,
            description: "Key that needs to be depressed together with Hotkey (KeyCode.None will disable the need for an additional key)");
        SwitchVRModeKey = _melonCategory.CreateEntry("Switch VR HotKey", KeyCode.F10,
            description: "Key that switches between desktop/vr mod");

        RestartVRModeModifierKey = _melonCategory.CreateEntry("Restart VR Modifier Key", KeyCode.None,
            description: "Key that needs to be depressed together with Hotkey (KeyCode.None will disable the need for an additional key)");
        RestartVRModeKey = _melonCategory.CreateEntry("Restart VR HotKey", KeyCode.F9,
            description: "Key that switches between desktop/vr mod and switches back after the delay defined below");
        RestartVRModeDelay = _melonCategory.CreateEntry("Restart VR Delay (s)", 10,
            description: "Delay to wait before switching back to VR when the restart key is pressed");
    }
}
