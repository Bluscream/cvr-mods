using Bluscream.PropSpawner.Properties;
using MelonLoader;

namespace Bluscream.PropSpawner;

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> EnableURLWhitelist;
    internal static MelonPreferences_Entry<bool> DisableURLWhitelist;

    internal static MelonPreferences_Entry<bool> EnableAdvancedSafety;
    internal static MelonPreferences_Entry<bool> DisableAdvancedSafety;

    internal static MelonPreferences_Entry<bool> DisableRichPresence;
    internal static MelonPreferences_Entry<bool> EnableRichPresence;

    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Public Safety", true,
            description: "The mod will do nothing while disabled");

        EnableURLWhitelist = _melonCategory.CreateEntry("Enable URL Whitelist in publics", true,
            description: "Will automatically switch on URL Whitelist when you join a public instance");
        DisableURLWhitelist = _melonCategory.CreateEntry("Disable URL Whitelist in non-publics", false,
            description: "Will automatically switch off URL Whitelist when you join anything other than a public instance");

        EnableAdvancedSafety = _melonCategory.CreateEntry("Enable Advanced Safety in publics", true,
            description: "Will automatically switch on Advanced Safety when you join a public instance");
        DisableAdvancedSafety = _melonCategory.CreateEntry("Disable Advanced Safety in non-publics", false,
            description: "Will automatically switch off Advanced Safety when you join anything other than a public instance");

        DisableRichPresence = _melonCategory.CreateEntry("Disable Rich Presence in non-publics", true,
            description: "Will automatically switch off Rich Presence (Steam+Discord) when you join anything other than a public instance");
        EnableRichPresence = _melonCategory.CreateEntry("Enable Rich Presence in publics", false,
            description: "Will automatically switch on Rich Presence (Steam+Discord) when you join a public instance");
    }
}
