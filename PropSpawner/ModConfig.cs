using Bluscream.PropSpawner.Properties;
using MelonLoader;

namespace Bluscream.PropSpawner;

public static class ModConfig {
    private static MelonPreferences_Category _melonCategory;
    internal static MelonPreferences_Entry<bool> EnableMod;
    internal static MelonPreferences_Entry<bool> EnableLogging;
    internal static MelonPreferences_Entry<bool> EnableHUDNotifications;

    internal static MelonPreferences_Entry<bool> AutoSaveSpawnedProps;

    private static readonly int _MinSpawnDelay = 1;
    internal static MelonPreferences_Entry<int> AutoSpawnDelay;


    public static void InitializeMelonPrefs() {
        _melonCategory = MelonPreferences.CreateCategory(AssemblyInfoParams.Name);
        EnableMod = _melonCategory.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while this is disabled");
        EnableLogging = _melonCategory.CreateEntry("Enable Logging", true,
            description: "The mod will write nothing to the MelonLoader Console/Logfile while this is disabled");
        EnableHUDNotifications = _melonCategory.CreateEntry("Enable HUD Notifications", true,
            description: "The mod will show no HUD notifications while this is disabled");

        AutoSaveSpawnedProps = _melonCategory.CreateEntry("Auto Save Spawned Props", false,
            description: $"Will automatically save all manually spawned props to \"{PropConfigManager.SavedPropsFileName}\" while enabled");

        AutoSpawnDelay = _melonCategory.CreateEntry("Spawn Delay (Seconds)", 3,
            description: "Time in seconds before spawning saved props after joining an instance.");
        if (AutoSpawnDelay.Value < _MinSpawnDelay) {
            Utils.Warn($"AutoSpawnDelay is below {_MinSpawnDelay}s! Resetting to default {AutoSpawnDelay.DefaultValue}s");
            AutoSpawnDelay.Value = AutoSpawnDelay.DefaultValue;
        }
    }
}
