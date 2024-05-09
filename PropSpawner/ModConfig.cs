using Bluscream.PropSpawner.Properties;
using MelonLoader;

namespace Bluscream.PropSpawner;

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;

    internal static MelonPreferences_Entry<bool> AutoSaveSpawnedProps;


    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");

        AutoSaveSpawnedProps = _melonCategory.CreateEntry("Auto Save Spawned Props", false,
            description: $"Will automatically save all manually spawned props to \"{PropConfigManager.SavedPropsFileName}\" while enabled");
    }
}
