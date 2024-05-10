using MelonLoader;
using UnityEngine;

namespace Bluscream.PropSpawner;

public static partial class Extensions {
    private static System.Random rnd = new System.Random();

    public static T PickRandom<T>(this IList<T> source) {
        int randIndex = rnd.Next(source.Count);
        return source[randIndex];
    }

    public static Vector3? ToVector3(this IList<float> floats) {
        var cnt = floats.Count();
        if (cnt != 3) {
            // MelonLogger.Warning($"Tried to convert list with {cnt} floats to Vector3!");
            return null;
        }
        return new Vector3(floats[0], floats[1], floats[2]);
    }
    public static Quaternion? ToQuaternion(this IList<float> floats) {
        var cnt = floats.Count();
        switch (cnt) {
            case 3: return new Quaternion() { x = floats[0], y = floats[1], z = floats[2] };
            case 4: return new Quaternion() { x = floats[0], y = floats[1], z = floats[2], w = floats[3] };
            default: MelonLogger.Warning($"Tried to convert list with {cnt} floats to Quaternion!"); return null;
        }
    }
    public static string ToString(this IList<float>? floats) {
        return floats != null ? string.Join(", ", floats) : string.Empty;
    }

    public static string ToString(this Vector3 v) {
        return $"X:{v.x} Y:{v.y} Z:{v.z}";
    }
    public static string ToString(this Quaternion q) {
        return $"X:{q.x} Y:{q.y} Z:{q.z} W:{q.w}";
    }
}
