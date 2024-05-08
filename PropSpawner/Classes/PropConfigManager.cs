using MelonLoader;

namespace Bluscream.PropSpawner {
    internal class PropConfigManager {
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
                var exampleConfig = new List<PropRule>();
                exampleConfig.Add(new PropRule() {
                    PropSelectionRandom = true,
                    Props = new List<Prop>() {
                        new Prop() { Id = "6cfe9b93-27d7-43dd-a0b5-63900c2872f2" /* [NMF] Awtter */ },
                        new Prop() { Id = "d9f0d320-13c0-4a19-ba76-f85f04e9704b" /* [NMF] Rantichi */ },
                        new Prop() { Id = "9c5366ad-8e33-4f87-a6d6-db2c642a1cf1" /* [NMF] Caninesmol */ },
                        new Prop() { Id = "26bd606e-a44c-4e95-8b44-e84f456aee65" /* [NMF] star wars droid */ },
                        new Prop() { Id = "dc0663a7-7c88-43ca-a52c-9b645ad1e860" /* [NMF] Nanachi */ }
                    }
                });
                exampleConfig.Add(new PropRule() {
                    WorldId = "406acf24-99b1-4119-8883-4fcda4250743",
                    SceneName = "ThePurpleFoxV2",
                    Props = new List<Prop>() {
                        new  Prop () { Id = "1f0aa960-e4ac-44fe-82f4-334eb4eb4959" /* Granny */, _Position = new Prop.Tion() {X=483.75f,Y=-4.36f,Z=100.51f}, _Rotation = new Prop.Tion() { X=0f,Y=-0.7177276f,Z=0f } },
                        new  Prop () { Id = "16cb1cef-fc83-4ddb-8c98-007e2455d970" /* Smoking Room */, _Position = new Prop.Tion() {X=-8.162736f,Y=-2.499999f,Z=3.811112f}, _Rotation = new Prop.Tion() { X=0f,Y=0.9999966f,Z=0f } },
                    }
                });
                exampleConfig.ToFile(Path.Combine(ConfigsDirectory, "example.json"));
            } else {
                MelonLogger.Msg($"Found {jsonFiles.Length} json files in {ConfigsDirectory}");
                foreach (string jsonFile in jsonFiles) {
                    Console.WriteLine($"Processing file: {jsonFile}");
                    Rules.AddRange(PropConfig.FromFile(jsonFile));
                }
                MelonLogger.Msg($"Loaded {Rules.Count} rules from {jsonFiles.Length} config files.");
            }
        }
    }
}
