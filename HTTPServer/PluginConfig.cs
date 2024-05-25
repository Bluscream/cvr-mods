using Aardwolf;
using MelonLoader;

namespace Bluscream.HTTPServer;
public static class PluginConfig {
    private static MelonPreferences_Category Category;
    internal static MelonPreferences_Entry<bool> Enabled;
    internal static MelonPreferences_Entry<bool> EnableLogging;
    internal static MelonPreferences_Entry<bool> EnableHUDNotifications;

    internal static MelonPreferences_Entry<string[]> Prefixes;
    internal static MelonPreferences_Entry<ushort> ConnectionsPerCore;
    internal static MelonPreferences_Entry<bool> Respond;


    public static void InitializeMelonPrefs() {
        Category = MelonPreferences.CreateCategory(Properties.AssemblyInfoParams.Name);
        Enabled = Category.CreateEntry("Enable Mod", true,
            description: "The mod will do nothing while disabled");
        EnableLogging = Category.CreateEntry("Enable Logging", true,
            description: "The mod will write nothing to the MelonLoader Console/Logfile while this is disabled");
        EnableHUDNotifications = Category.CreateEntry("Enable HUD Notifications", true,
            description: "The mod will show no HUD notifications while this is disabled");
        Prefixes = Category.CreateEntry("Prefixes", new[] { "http://*:5111/" },
            description: "Which Server adresses + ports to listen to");
        ConnectionsPerCore = Category.CreateEntry("Connections per CPU core", (ushort)2,
            description: "How many connections to accept simultanously, less means more performance but less reliability");
        Respond = Category.CreateEntry("Respond", false,
            description: "Enables HTTP Responses");
        Enabled.OnEntryValueChanged.Subscribe((_, newValue) => { Server.Toggle(newValue); });
        Prefixes.OnEntryValueChanged.Subscribe((_, _) => { Server.LoadPrefixes(); });
    }
    public static ConfigurationDictionary ToConfigurationDictionary() {
        var configValues = new Dictionary<string, List<string>>();
        if (PluginConfig.Category != null) {
            configValues["Enabled"] = new List<string> { PluginConfig.Enabled.Value.ToString() };
            configValues["EnableLogging"] = new List<string> { PluginConfig.EnableLogging.Value.ToString() };
            configValues["EnableHUDNotifications"] = new List<string> { PluginConfig.EnableHUDNotifications.Value.ToString() };
            configValues["Prefixes"] = PluginConfig.Prefixes.Value.ToList();
            configValues["ConnectionsPerCore"] = new List<string> { PluginConfig.ConnectionsPerCore.Value.ToString() };
            configValues["Respond"] = new List<string> { PluginConfig.Respond.Value.ToString() };
        }
        return new ConfigurationDictionary(configValues);
    }
}
