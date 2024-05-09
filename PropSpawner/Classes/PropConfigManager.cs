using MelonLoader;
using System.Data;

namespace Bluscream.PropSpawner {
    internal class PropConfigManager {
        internal protected const string ExampleFileName = "example.json";
        internal protected const string SavedPropsFileName = "Saved Props.json";
        internal static protected readonly string ConfigsDirectory = Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "PropConfigs");
        internal static List<PropRule> Rules = new();

        internal static void Initialize() {
            MelonLogger.Msg($"Initializing PropConfigManager for {ConfigsDirectory}");
            if (!Directory.Exists(ConfigsDirectory)) {
                MelonLogger.Msg($"{ConfigsDirectory} does not exist, creating it now...");
                Directory.CreateDirectory(ConfigsDirectory);
            }
            LoadConfigs();
        }
        internal static void LoadConfigs() {
            Rules.Clear();
            string[] jsonFiles = Directory.GetFiles(ConfigsDirectory, "*.json");
            if (jsonFiles.Length < 1) {
                MelonLogger.Warning($"Could not find any prop configs in {ConfigsDirectory}, generating examples");
                GenerateExamples();
            } else {
                MelonLogger.Msg($"Found {jsonFiles.Length} json files in {ConfigsDirectory}");
                foreach (string jsonFile in jsonFiles) {
                    MelonLogger.Msg($"Processing file: {jsonFile}");
                    try {
                        var cfg = PropConfig.FromFile(jsonFile);
                        MelonLogger.Msg($"Loaded {cfg.Count} rules from {jsonFile}");
                        Rules.AddRange(cfg);
                    } catch (Exception ex) {
                        MelonLogger.Error($"Failed to load {jsonFile}: {ex.Message}");
                    }
                }
                MelonLogger.Msg($"Loaded {Rules.Count} rules from {jsonFiles.Length} config files.");
            }
        }
        internal static List<PropRule> Matches(string worldId = null, string worldName = null, string sceneName = null, string instancePrivacy = null) {
            var results = new List<PropRule>();
            foreach (var rule in Rules) {
                var worldWildcard = (rule.WorldId is null && rule.SceneName is null) || rule.WorldId.Any(w => w == "*" || rule.SceneName.Any(w => w == "*") || rule.WorldName.Any(w => w == "*"));
                var worldIdValid = rule.WorldId.Any(w => w == worldId);
                var worldNameValid = rule.WorldName.Any(w => w == worldName);
                var sceneNameValid = rule.SceneName.Any(s => s == sceneName);
                if (!worldWildcard && !worldIdValid && !worldNameValid && !sceneNameValid) continue;
                results.Add(rule);
            }
            return results;
        }
        internal static void GenerateExamples() {
            var exampleConfig = new List<PropRule> {
                    new PropRule() {
                        PropSelectionRandom = true,
                        Props = new List<Prop>() {
                            new Prop() { Id = "6cfe9b93-27d7-43dd-a0b5-63900c2872f2" /* [NMF] Awtter */ },
                            new Prop() { Id = "d9f0d320-13c0-4a19-ba76-f85f04e9704b" /* [NMF] Rantichi */ },
                            new Prop() { Id = "9c5366ad-8e33-4f87-a6d6-db2c642a1cf1" /* [NMF] Caninesmol */ },
                            new Prop() { Id = "26bd606e-a44c-4e95-8b44-e84f456aee65" /* [NMF] star wars droid */ },
                            new Prop() { Id = "dc0663a7-7c88-43ca-a52c-9b645ad1e860" /* [NMF] Nanachi */ }
                        }
                    },
                    new PropRule() {
                        WorldId = new() {"406acf24-99b1-4119-8883-4fcda4250743"},
                        SceneName = new() {"ThePurpleFoxV2"},
                        Props = new List<Prop>() {
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959" /* Granny */, Position = new() {483.75f,-4.36f,100.51f}, Rotation = new() { 0f,-0.7177276f,0f } },
                            new  Prop() { Id = "16cb1cef-fc83-4ddb-8c98-007e2455d970" /* Smoking Room */, Position = new() {-8.162736f,-2.499999f,3.811112f}, Rotation = new() { 0f,0.9999966f,0f } },
                        }
                    },
                    new PropRule() {
                        WorldId = new() {"406acf24-99b1-4119-8883-4fcda4250743"},
                        WorldName = new() {"Forge World"},
                        SceneName = new() {"Forge World"},
                        Props = new List<Prop>() {
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959" /* Granny */, Position = new() { -169.4302f, 122.4796f, -652.9823f } },
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959" /* Granny */, Position = new() { -171.3268f, 122.4796f, -652.9365f } },
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959" /* Granny */, Position = new() { -176.0865f, 122.4796f, -652.8627f } },
                        }
                    }
                };
            exampleConfig.ToFile(Path.Combine(ConfigsDirectory, ExampleFileName));
        }
        internal static void SaveProp(Prop prop, string worldId = null, string worldName = null, string sceneName = null, string instancePrivacy = null) {
            var jsonFile = Path.Combine(ConfigsDirectory, SavedPropsFileName);
            var cfg = new List<PropRule>();
            try {
                cfg = PropConfig.FromFile(jsonFile);
                MelonLogger.Msg($"Loaded {cfg.Count} rules from {jsonFile}");
            } catch (Exception ex) {
                MelonLogger.Error($"Failed to load {jsonFile}: {ex.Message}");
                File.Copy(jsonFile, jsonFile + ".broken", true);
            }
            foreach (PropRule rule in cfg) {
                if (Matches())
            }
        }
    }
}
