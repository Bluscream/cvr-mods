using MelonLoader;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using ABI_RC.Core.Player;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Security.Cryptography;

namespace Bluscream.PropSpawner;

public static partial class Extensions {
    private static System.Random rnd = new System.Random();
    #region list
    public static T PickRandom<T>(this IList<T> source) {
        int randIndex = rnd.Next(source.Count);
        return source[randIndex];
    }
    public static string ToString(this IList<float>? floats) {
        return floats != null ? string.Join(", ", floats) : string.Empty;
    }
    public static Vector3? ToVector3(this IList<float> floats) {
        var cnt = floats.Count();
        if (cnt != 3) {
            // Utils.Warning($"Tried to convert list with {cnt} floats to Vector3!");
            return null;
        }
        return new Vector3(floats[0], floats[1], floats[2]);
    }
    public static Quaternion? ToQuaternion(this IList<float> floats) {
        var cnt = floats.Count();
        switch (cnt) {
            case 3: return new Quaternion() { x = floats[0], y = floats[1], z = floats[2] };
            case 4: return new Quaternion() { x = floats[0], y = floats[1], z = floats[2], w = floats[3] };
            default: Utils.Warn($"Tried to convert list with {cnt} floats to Quaternion!"); return null;
        }
    }
    #endregion list
    #region unity
    public static List<float> ToList(this Vector3 vec) {
        return vec != null ? new List<float>() { vec.x, vec.y, vec.z } : new List<float>();
    }
    public static List<float> ToList(this Quaternion quat) {
        return quat != null ? new List<float>() { quat.x, quat.y, quat.z, quat.w } : new List<float>();
    }
    public static string ToString(this Vector3 v) {
        return $"X:{v.x} Y:{v.y} Z:{v.z}";
    }
    public static string ToString(this Quaternion q) {
        return $"X:{q.x} Y:{q.y} Z:{q.z} W:{q.w}";
    }
    #endregion unity
    #region io
    public static string str(this FileInfo file) {
        return "\"" + file.FullName + "\"";
    }
    public static string str(this DirectoryInfo directory) {
        return "\"" + directory.FullName + "\"";
    }
    #endregion io
    #region Reflection

    public static Dictionary<string, object> ToDictionary(this object instanceToConvert) {
        return instanceToConvert.GetType()
          .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
          .ToDictionary(
          propertyInfo => propertyInfo.Name,
          propertyInfo => Extensions.ConvertPropertyToDictionary(propertyInfo, instanceToConvert));

    }

    private static object ConvertPropertyToDictionary(PropertyInfo propertyInfo, object owner) {
        Type propertyType = propertyInfo.PropertyType;
        object propertyValue = propertyInfo.GetValue(owner);

        // If property is a collection don't traverse collection properties but the items instead
        if (!propertyType.Equals(typeof(string)) && (typeof(ICollection<>).Name.Equals(propertyValue.GetType().BaseType.Name) || typeof(System.Collections.ObjectModel.Collection<>).Name.Equals(propertyValue.GetType().BaseType.Name))) {
            var collectionItems = new List<Dictionary<string, object>>();
            var count = (int)propertyType.GetProperty("Count").GetValue(propertyValue);
            PropertyInfo indexerProperty = propertyType.GetProperty("Item");

            // Convert collection items to dictionary
            for (var index = 0; index < count; index++) {
                object item = indexerProperty.GetValue(propertyValue, new object[] { index });
                PropertyInfo[] itemProperties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

                if (itemProperties.Any()) {
                    Dictionary<string, object> dictionary = itemProperties
                      .ToDictionary(
                        subtypePropertyInfo => subtypePropertyInfo.Name,
                        subtypePropertyInfo => Extensions.ConvertPropertyToDictionary(subtypePropertyInfo, item));
                    collectionItems.Add(dictionary);
                }
            }

            return collectionItems;
        }

        // If property is a string stop traversal (ignore that string is a char[])
        if (propertyType.IsPrimitive || propertyType.Equals(typeof(string))) {
            return propertyValue;
        }

        PropertyInfo[] properties = propertyType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        if (properties.Any()) {
            return properties.ToDictionary(
                                subtypePropertyInfo => subtypePropertyInfo.Name,
                                subtypePropertyInfo => (object)Extensions.ConvertPropertyToDictionary(subtypePropertyInfo, propertyValue));
        }

        return propertyValue;
    }
    #endregion
    #region DateTime
    public static bool ExpiredSince(this DateTime dateTime, int minutes) {
        return (dateTime - DateTime.Now).TotalMinutes < minutes;
    }
    public static TimeSpan StripMilliseconds(this TimeSpan time) {
        return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
    }
    #endregion
    #region DirectoryInfo
    public static DirectoryInfo Combine(this DirectoryInfo dir, params string[] paths) {
        var final = dir.FullName;
        foreach (var path in paths) {
            final = Path.Combine(final, path);
        }
        return new DirectoryInfo(final);
    }
    #endregion
    #region FileInfo
    public static FileInfo CombineFile(this DirectoryInfo dir, params string[] paths) {
        var final = dir.FullName;
        foreach (var path in paths) {
            final = Path.Combine(final, path);
        }
        return new FileInfo(final);
    }
    public static FileInfo Combine(this FileInfo file, params string[] paths) {
        var final = file.DirectoryName;
        foreach (var path in paths) {
            final = Path.Combine(final, path);
        }
        return new FileInfo(final);
    }
    public static string FileNameWithoutExtension(this FileInfo file) {
        return Path.GetFileNameWithoutExtension(file.Name);
    }
    /*public static string Extension(this FileInfo file) {
        return Path.GetExtension(file.Name);
    }*/
    public static void AppendLine(this FileInfo file, string line) {
        try {
            if (!file.Exists) file.Create();
            File.AppendAllLines(file.FullName, new string[] { line });
        } catch { }
    }
    public static void WriteAllText(this FileInfo file, string text) => File.WriteAllText(file.FullName, text);
    public static string ReadAllText(this FileInfo file) => File.ReadAllText(file.FullName);
    public static List<string> ReadAllLines(this FileInfo file) => File.ReadAllLines(file.FullName).ToList();
    #endregion
    #region UI
    #endregion

    #region Object
    public static string ToJSON(this object obj, bool indented = true) {
        // return JsonConvert.SerializeObject(obj, (indented ? Formatting.Indented : Formatting.None), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateFormatString = "yyyy-MM-dd hh:mm:ss"}) ; // , new JsonConverter[] { new StringEnumConverter() }
        return JsonConvert.SerializeObject(obj, (indented ? Formatting.Indented : Formatting.None), [new StringEnumConverter(), new IPAddressConverter(), new IPEndPointConverter()]);
    }
    #endregion
    #region String
    public static string ToMd5(this string input) {
        using (MD5 md5 = MD5.Create()) {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++) {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public static UnityEngine.Vector3? ParseVector3(this string source) {
        var split = source.Split(",");
        switch (split.Length) {
            case 3:
                return new UnityEngine.Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            case 2:
                return new UnityEngine.Vector3(float.Parse(split[0]), PlayerSetup.Instance.gameObject.transform.position.y, float.Parse(split[2]));
            case 1:
                return new UnityEngine.Vector3(PlayerSetup.Instance.gameObject.transform.position.x, float.Parse(split[0]), PlayerSetup.Instance.gameObject.transform.position.z);
        }
        return null;
    }
    public static UnityEngine.Quaternion? ParseQuaternion(this string source) {
        var split = source.Split(",");
        switch (split.Length) {
            case 4:
                return new UnityEngine.Quaternion(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
            case 3:
                return new UnityEngine.Quaternion(float.Parse(split[0]), PlayerSetup.Instance.gameObject.transform.rotation.y, float.Parse(split[1]), float.Parse(split[2]));
            case 2:
                return new UnityEngine.Quaternion(PlayerSetup.Instance.gameObject.transform.rotation.x, float.Parse(split[0]), PlayerSetup.Instance.gameObject.transform.rotation.z, float.Parse(split[1]));
            case 1:
                return new UnityEngine.Quaternion(PlayerSetup.Instance.gameObject.transform.rotation.x, float.Parse(split[0]), PlayerSetup.Instance.gameObject.transform.rotation.z, PlayerSetup.Instance.gameObject.transform.rotation.w);
        }
        return null;
    }
    public static IEnumerable<string> SplitToLines(this string input) {
        if (input == null) {
            yield break;
        }

        using (System.IO.StringReader reader = new System.IO.StringReader(input)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                yield return line;
            }
        }
    }
    public static string ToTitleCase(this string source, string langCode = "en-US") {
        return new CultureInfo(langCode, false).TextInfo.ToTitleCase(source);
    }
    public static bool Contains(this string source, string toCheck, StringComparison comp) {
        return source?.IndexOf(toCheck, comp) >= 0;
    }
    public static bool IsNullOrEmpty(this string source) {
        return string.IsNullOrEmpty(source);
    }
    public static string[] Split(this string source, string split, int count = -1, StringSplitOptions options = StringSplitOptions.None) {
        if (count != -1) return source.Split(new string[] { split }, count, options);
        return source.Split(new string[] { split }, options);
    }
    public static string Remove(this string Source, string Replace) {
        return Source.Replace(Replace, string.Empty);
    }
    public static string ReplaceLastOccurrence(this string Source, string Find, string Replace) {
        int place = Source.LastIndexOf(Find);
        if (place == -1)
            return Source;
        string result = Source.Remove(place, Find.Length).Insert(place, Replace);
        return result;
    }
    public static string EscapeLineBreaks(this string source) {
        return Regex.Replace(source, @"\r\n?|\n", @"\$&");
    }
    public static string Ext(this string text, string extension) {
        return text + "." + extension;
    }
    public static string Quote(this string text) {
        return SurroundWith(text, "\"");
    }
    public static string Enclose(this string text) {
        return SurroundWith(text, "(", ")");
    }
    public static string Brackets(this string text) {
        return SurroundWith(text, "[", "]");
    }
    public static string SurroundWith(this string text, string surrounds) {
        return surrounds + text + surrounds;
    }
    public static string SurroundWith(this string text, string starts, string ends) {
        return starts + text + ends;
    }
    #endregion
    #region Dict
    public static void AddSafe(this IDictionary<string, string> dictionary, string key, string value) {
        if (!dictionary.ContainsKey(key))
            dictionary.Add(key, value);
    }
    #endregion
    #region List
    public static string ToQueryString(this NameValueCollection nvc) {
        if (nvc == null) return string.Empty;

        StringBuilder sb = new StringBuilder();

        foreach (string key in nvc.Keys) {
            if (string.IsNullOrWhiteSpace(key)) continue;

            string[] values = nvc.GetValues(key);
            if (values == null) continue;

            foreach (string value in values) {
                sb.Append(sb.Length == 0 ? "?" : "&");
                sb.AppendFormat("{0}={1}", key, value);
            }
        }

        return sb.ToString();
    }
    public static bool GetBool(this NameValueCollection collection, string key, bool defaultValue = false) {
        if (!collection.AllKeys.Contains(key, StringComparer.OrdinalIgnoreCase)) return false;
        var trueValues = new string[] { true.ToString(), "yes", "1" };
        if (trueValues.Contains(collection[key], StringComparer.OrdinalIgnoreCase)) return true;
        var falseValues = new string[] { false.ToString(), "no", "0" };
        if (falseValues.Contains(collection[key], StringComparer.OrdinalIgnoreCase)) return true;
        return defaultValue;
    }
    public static string GetString(this NameValueCollection collection, string key) {
        if (!collection.AllKeys.Contains(key)) return collection[key];
        return null;
    }
    public static T PopFirst<T>(this IEnumerable<T> list) => list.ToList().PopAt(0);
    public static T PopLast<T>(this IEnumerable<T> list) => list.ToList().PopAt(list.Count() - 1);
    public static T PopAt<T>(this List<T> list, int index) {
        T r = list.ElementAt<T>(index);
        list.RemoveAt(index);
        return r;
    }
    #endregion
    #region Uri
    private static readonly Regex QueryRegex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");
    public static IReadOnlyDictionary<string, string> ParseQueryString(this Uri uri) {
        var match = QueryRegex.Match(uri.PathAndQuery);
        var paramaters = new Dictionary<string, string>();
        while (match.Success) {
            paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
            match = match.NextMatch();
        }
        return paramaters;
    }
    #endregion
    #region Enum
    #endregion
    #region Task
    public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout) {
        using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task) {
                timeoutCancellationTokenSource.Cancel();
                return await task;  // Very important in order to propagate exceptions
            } else {
                return default(TResult);
            }
        }
    }
    #endregion
    #region bool
    public static string ToYesNo(this bool input) => input ? "Yes" : "No";
    public static string ToEnabledDisabled(this bool input) => input ? "Enabled" : "Disabled";
    public static string ToOnOff(this bool input) => input ? "On" : "Off";
    #endregion
    internal class IPAddressConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return IPAddress.Parse((string)reader.Value);
        }
    }

    internal class IPEndPointConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            IPEndPoint ep = (IPEndPoint)value;
            JObject jo = new JObject();
            jo.Add("Address", JToken.FromObject(ep.Address, serializer));
            jo.Add("Port", ep.Port);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            return new IPEndPoint(address, port);
        }
    }
}
