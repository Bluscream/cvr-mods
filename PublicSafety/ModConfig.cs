using Bluscream.PublicSafety.Properties;
using MelonLoader;

namespace Bluscream.PublicSafety;

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> EnableURLWhitelist;
    internal static MelonPreferences_Entry<bool> DisableURLWhitelist;

    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Public Safety", true,
            description: "The mod will do nothing while disabled");

        EnableURLWhitelist = _melonCategory.CreateEntry("Enable URL Whitelist in publics", true,
            description: "Will automatically switch on URL Whitelist when you join a public instance");
        DisableURLWhitelist = _melonCategory.CreateEntry("Disable URL Whitelist in non-publics", false,
            description: "Will automatically switch off URL Whitelist when you join anything other than a public instance");

    }

}
