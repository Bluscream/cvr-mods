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
            MelonLogger.Warning($"Tried to convert list with {cnt} floats to Vector3!");
            return null;
        }
        return new Vector3(floats[0], floats[1], floats[3]);
    }

    public static Quaternion? ToQuaternion(this IList<float> floats) {
        var cnt = floats.Count();
        if (cnt < 3 || cnt > 4) {
            MelonLogger.Warning($"Tried to convert list with {cnt} floats to Quaternion!");
            return null;
        }
        return new Quaternion() { x = floats[0], y = floats[1], z = floats[2], w = floats[3] };
    }
}
