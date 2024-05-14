using MelonLoader;
using System.Data;
using System.Numerics;

namespace Bluscream.PropSpawner {
    internal class PropConfigManager {
        internal protected const string ExampleFileName = "example.json";
        internal protected const string SavedPropsFileName = "Saved Props.json";
        internal static protected readonly DirectoryInfo ConfigsDirectory = new DirectoryInfo(MelonLoader.Utils.MelonEnvironment.UserDataDirectory).Combine("PropConfigs");
        internal static List<PropRule> Rules = new();

        internal static void Initialize() {
            Utils.Log($"Initializing PropConfigManager for {ConfigsDirectory.str()}");
            if (!ConfigsDirectory.Exists) {
                Utils.Log($"{ConfigsDirectory.str()} does not exist, creating it now...");
                ConfigsDirectory.Create();
            }
            LoadConfigs();
        }
        internal static void LoadConfigs() {
            var cfgFiles = GetValidConfigFiles();
            if (cfgFiles.Count < 1) {
                Utils.Warn($"Could not find any prop configs in {ConfigsDirectory.str()}, generating examples");
                GenerateExamples();
            } else {
                Utils.Log($"Found {cfgFiles.Count} json files in {ConfigsDirectory.str()}");
                Rules.Clear();
                foreach (var (file, rules) in cfgFiles) {
                    if (!file.Exists) continue;
                    Utils.Log($"Processing file: {file.str()}");
                    foreach (var rule in rules) {
                        try {
                            rule.File = file;
                            Rules.Add(rule);
                        } catch (Exception ex) {
                            Utils.Error($"Failed to load rule {rule}: {ex.Message}");
                        }
                    }
                    Utils.Log($"Loaded {rules.Count} rules from {file.str()}");
                }
                Utils.Log($"Loaded {Rules.Count} rules from {cfgFiles.Count} config files.");
            }
        }
        internal static Dictionary<FileInfo, List<PropRule>> GetValidConfigFiles() {
            var ret = new Dictionary<FileInfo, List<PropRule>>();
            var jsonFiles = ConfigsDirectory.GetFiles("*.json");
            foreach (var jsonFile in jsonFiles) {
                if (!jsonFile.Exists) continue;
                Utils.Log($"Processing file: {jsonFile.str()}");
                try {
                    var cfg = PropConfig.FromFile(jsonFile);
                    ret.Add(jsonFile, cfg);
                } catch (Exception ex) {
                    Utils.Error($"Failed to load file {jsonFile.str()}: {ex.Message}");
                }
            }
            Utils.Log($"Loaded {Rules.Count} rules from {jsonFiles.Length} config files.");
            return ret;
        }
        internal static HashSet<FileInfo> GetFilesFromRules() => Rules.Select(r => r.File).ToHashSet();
        internal static List<PropRule> GetRulesByFile(FileInfo file) => Rules.Where(r => r.File == file).ToList();
        internal static List<PropRule> Matches(string worldId = null, string worldName = null, string sceneName = null, string instancePrivacy = null) {
            //Utils.Warning("debug PropConfigManager.Matches start");
            var results = new List<PropRule>();
            foreach (var rule in Rules) {
                var matches = rule.Matches(worldId, worldName, sceneName, instancePrivacy);
                // Utils.Warning($"debug List<PropRule> Matches {rule} matches {matches}");
                if (matches) results.Add(rule);
            }
            //Utils.Warning($"debug PropConfigManager.Matches end {results.Count}");
            return results;
        }
        internal static void GenerateExamples() {
            var exampleConfig = new List<PropRule> {
                    new PropRule() {
                        Name = "Spawn a random NavMeshFollower when joining any world",
                        PropSelectionRandom = 1,
                        Props = new List<Prop>() {
                            new Prop() { Id = "6cfe9b93-27d7-43dd-a0b5-63900c2872f2", Name = "[NMF] Awtter" },
                            new Prop() { Id = "d9f0d320-13c0-4a19-ba76-f85f04e9704b", Name = "[NMF] Rantichi" },
                            new Prop() { Id = "9c5366ad-8e33-4f87-a6d6-db2c642a1cf1", Name = "[NMF] Caninesmol" },
                            new Prop() { Id = "26bd606e-a44c-4e95-8b44-e84f456aee65", Name = "[NMF] star wars droid" },
                            new Prop() { Id = "dc0663a7-7c88-43ca-a52c-9b645ad1e860", Name = "[NMF] Nanachi" }
                        }
                    },
                    new PropRule() {
                        WorldId = "406acf24-99b1-4119-8883-4fcda4250743",
                        SceneName = "ThePurpleFoxV2",
                        Props = new List<Prop>() {
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959", Name = "Granny", Position = new() {483.75f,-4.36f,100.51f}, Rotation = new() { 0f,-0.2f,0f } },
                            new  Prop() { Id = "16cb1cef-fc83-4ddb-8c98-007e2455d970", Name = "Smoking Room", Position = new() {-8.162736f,-2.499999f,3.811112f}, Rotation = new() { 0f,0.45f,0f } },
                        }
                    },
                    new PropRule() {
                        Name = "Let the grannies rule ✊",
                        WorldId = "6ae87c55-7159-4829-b94a-484fe030c766",
                        WorldName = "Forge World",
                        SceneName = "Forge World",
                        Props = new List<Prop>() {
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959", Name = "Granny", Position = new() { -169.4302f, 122.4796f, -652.9823f } },
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959", Name = "Granny", Position = new() { -171.3268f, 122.4796f, -652.9365f } },
                            new  Prop() { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959", Name = "Granny", Position = new() { -176.0865f, 122.4796f, -652.8627f } },
                        }
                    }
                };
            exampleConfig.ToFile(ConfigsDirectory.CombineFile(ExampleFileName));
            LoadConfigs();
        }
        //internal static void SaveProp(string propId, Vector3? position = null, 
        internal static void SaveProp(Prop prop, string worldId = null, string worldName = null, string sceneName = null, string instancePrivacy = null, bool reloadAfterSave = true) {
            //Utils.Warning("debug PropConfigManager.SaveProp start");
            var jsonFile = ConfigsDirectory.CombineFile(SavedPropsFileName);
            var cfg = new List<PropRule>();
            if (jsonFile.Exists) {
                try {
                    cfg = PropConfig.FromFile(jsonFile);
                    Utils.Log($"Loaded {cfg.Count} rules from {jsonFile.str()}");
                } catch (Exception ex) {
                    Utils.Error($"Failed to load {jsonFile.str()}: {ex.Message}");
                    jsonFile.CopyTo(jsonFile + ".broken", true);
                }
            }
            var matched = false;
            foreach (PropRule rule in cfg) {
                if (rule.Matches(worldId, worldName, sceneName, instancePrivacy)) {
                    matched = true;
                    rule.Props.Add(prop);
                    break;
                }
            }
            if (!matched) {
                var rule = new PropRule() {
                    WorldId = worldId,
                    WorldName = worldName,
                    SceneName = sceneName,
                    Props = new() { prop }
                };
                cfg.Add(rule);
            }
            cfg.ToFile(jsonFile);
            if (reloadAfterSave) LoadConfigs();
            // Utils.Warning("debug PropConfigManager.SaveProp end");
        }
    }
}
