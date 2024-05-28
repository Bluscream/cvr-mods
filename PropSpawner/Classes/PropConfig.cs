namespace Bluscream.PropSpawner;

using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class PropRule {
    [JsonIgnore]
    /// <summary>
    /// Path of the json file the rule came from or belongs to
    /// </summary>
    public virtual FileInfo File { get; set; }

    [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains a descriptive name for the rule
    /// </summary>
    public virtual bool? Enabled { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains a descriptive name for the rule
    /// </summary>
    public virtual string? Name { get; set; }

    [JsonProperty("worldId", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the gid of the world where this rule will take effect
    /// </summary>
    public virtual string? WorldId { get; set; }

    [JsonProperty("worldName", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the name of the world where this rule will take effect
    /// </summary>
    public virtual string? WorldName { get; set; }

    [JsonProperty("sceneName", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the name of the unity scene where this rule will take effect
    /// </summary>
    public virtual string? SceneName { get; set; }

    [JsonProperty("instancePrivacy", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the instance privacy where this rule will take effect
    /// </summary>
    public virtual string? InstancePrivacy { get; set; }

    [JsonProperty("propSelectionRandom", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that when set will force a random prop from the list to be selected everytime
    /// </summary>
    internal virtual int? PropSelectionRandom { get; set; }

    [JsonProperty("props", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Required field that contains the list of props that spawn if this rule applies
    /// </summary>
    public virtual List<Prop> Props { get; set; }

    [JsonIgnore]
    public virtual string Hash { get { return this.ToJSON().ToMd5(); } }


    public override string ToString() {
        var msg = $"PropRule \"{File.Name}\" > \"{GetName()}\" (";
        if (PropSelectionRandom.HasValue && PropSelectionRandom.Value > 0) msg += $"{PropSelectionRandom.Value}/";
        return msg += $"{Props.Count} props)";
    }
    public string GetName() => Name ?? WorldName ?? SceneName ?? WorldId ?? InstancePrivacy;

    public string MatchStr() => WorldName ?? SceneName ?? WorldId ?? InstancePrivacy ?? (MatchesEverywhere() ? "Everwhere" : "Unknown");
    public bool MatchesEverywhere() {
        var worldMissing = WorldId is null && SceneName is null && WorldName is null && InstancePrivacy is null;
        var worldWildcard = WorldId == "*" || SceneName == "*" || WorldName == "*" || InstancePrivacy == "*";
        return (worldMissing || worldWildcard); // Enabled.GetValueOrDefault(true) && 
    }
    public bool Matches(string worldId = null, string worldName = null, string sceneName = null, string instancePrivacy = null) {
        // Utils.Warning($"debug PropRule.Matches start {worldId} {worldName} {sceneName} {instancePrivacy}");
        // Utils.Warning($"debug PropRule.Matches rule {Name} {WorldId} {WorldName} {SceneName} {InstancePrivacy}");
        //var worldWildcard = WorldId.Any(w => w == "*") && SceneName.Any(w => w == "*") && WorldName.Any(w => w == "*") && InstancePrivacy.Any(w => w == "*");
        // Utils.Warning($"debug PropRule.Matches worldId > rule={WorldId} request={worldId}");
        var worldIdValid = WorldId != null && WorldId == worldId;
        // Utils.Warning($"debug PropRule.Matches worldName > rule={WorldName} request={worldName}");
        var worldNameValid = WorldName != null && WorldName == worldName;
        // Utils.Warning($"debug PropRule.Matches sceneName > rule={SceneName} request={sceneName}");
        var sceneNameValid = SceneName != null && SceneName == sceneName;
        var instancePrivacyValid = InstancePrivacy != null && SceneName == instancePrivacy;
        // Utils.Warning($"debug PropRule.Matches end worldMissing={worldMissing} worldIdValid={worldIdValid} worldNameValid={worldNameValid} sceneNameValid={sceneNameValid} instancePrivacyValid={instancePrivacyValid}");
        if (!Enabled.GetValueOrDefault(true)) return false;
        if (MatchesEverywhere() || worldIdValid || worldNameValid || sceneNameValid || instancePrivacyValid) return true;
        return false;
    }
}

public partial class Prop {
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Required field that contains the gid of the prop
    /// </summary>
    public virtual string Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the name of the prop (unused)
    /// </summary>
    public virtual string Name { get; set; }

    [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the Vector3 Position of the prop to spawn. If omitted, it will spawn at player position
    /// </summary>
    internal virtual List<float>? Position { get; set; }

    [JsonProperty("rotation", NullValueHandling = NullValueHandling.Ignore)]
    /// <summary>
    /// Optional field that contains the Quaternion Rotation of the prop to spawn. If omitted it will spawn with player angles
    /// </summary>
    internal virtual List<float>? Rotation { get; set; }

    public override string ToString() {
        return $"\"{Name}\" ({Id}) at {Extensions.ToString(Position)} ({Extensions.ToString(Rotation)})";
    }
}

public partial class PropConfig {
    public static List<PropRule> FromFile(FileInfo file) => FromFile(file.FullName);
    public static List<PropRule> FromFile(string filePath) => FromJson(File.ReadAllText(filePath));
    public static List<PropRule> FromJson(string json) => JsonConvert.DeserializeObject<List<PropRule>>(json, Bluscream.PropSpawner.Converter.Settings);
}

public static class Serialize {
    public static string ToJson(this List<PropRule> self) => JsonConvert.SerializeObject(self, Bluscream.PropSpawner.Converter.Settings);
    public static void ToFile(this List<PropRule> self, FileInfo file) => ToFile(self, file.FullName);
    public static void ToFile(this List<PropRule> self, string filePath) => File.WriteAllText(filePath, self.ToJson());
}

internal static class Converter {
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Formatting = Formatting.Indented,
        Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}
