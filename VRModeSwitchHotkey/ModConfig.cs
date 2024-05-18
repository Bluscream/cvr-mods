using Bluscream.VRModeSwitchHotkey.Properties;
using MelonLoader;
using UnityEngine;

namespace Bluscream.VRModeSwitchHotkey;
public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<KeyCode> ModifierKey;
    internal static MelonPreferences_Entry<KeyCode> HotKey;


    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        ModifierKey = _melonCategory.CreateEntry("Switch VR Modifier Key", KeyCode.None,
            description: "Key that needs to be depressed together with Hotkey (KeyCode.None will disable the check)");
        HotKey = _melonCategory.CreateEntry("Switch VR HotKey", KeyCode.F12,
            description: "Key that switches between desktop/vr mod");
    }
}
