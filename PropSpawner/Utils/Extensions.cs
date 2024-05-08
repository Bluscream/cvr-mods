using System;
using System.Collections.Generic;

namespace Bluscream.PropSpawner;

public static partial class Extensions {
    private static Random rnd = new Random();

    public static T PickRandom<T>(this IList<T> source) {
        int randIndex = rnd.Next(source.Count);
        return source[randIndex];
    }
}
