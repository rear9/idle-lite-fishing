using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class WeightedRandom
{
    public static T Pick<T>(IList<T> entries, Func<T, float> weightSelector)
    {
        float total = entries.Sum(weightSelector);
        if (total <= 0f) return default;

        float roll = Random.value * total;
        float acc = 0f;
        foreach (T e in entries)
        {
            acc += weightSelector(e);
            if (roll <= acc) return e;
        }
        return entries[^1];
    }
}