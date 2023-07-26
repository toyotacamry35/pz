using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static partial class Extensions
{
    public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match)
    {
        foreach (var key in dict.Keys.ToArray()
                .Where(key => match(key, dict[key])))
            dict.Remove(key);
    }

    public static void RemoveAllNonAlloc<K, V>(this ConcurrentDictionary<K, V> dict, Func<K, V, bool> match, List<K> keysToRemove)
    {
        V unused;
        keysToRemove.Clear();
        foreach (var e in dict)
            if (match(e.Key, e.Value))
                keysToRemove.Add(e.Key);
        foreach (var k in keysToRemove)
            dict.TryRemove(k, out unused);
    }

    public static void RemoveAllNonAlloc<K, V>(this Dictionary<K, V> dict, Func<K, V, bool> match, List<K> keysToRemove)
    {
        keysToRemove.Clear();
        foreach (var e in dict)
            if (match(e.Key, e.Value))
                keysToRemove.Add(e.Key);
        foreach (var k in keysToRemove)
            dict.Remove(k);
        keysToRemove.Clear();
    }
    public static void RemoveAllNonAlloc<K, V>(this Dictionary<K, V> dict, Func<K, V, bool> match, List<K> keysToRemove, Action<K,V> onRemove)
    {
        keysToRemove.Clear();
        foreach (var e in dict)
            if (match(e.Key, e.Value))
                keysToRemove.Add(e.Key);
        foreach (var k in keysToRemove)
        {
            onRemove(k, dict[k]);
            dict.Remove(k);
        }
        keysToRemove.Clear();
    }

    public static async Task RemoveAllNonAlloc<K, V>(this Dictionary<K, V> dict, Func<K, V, bool> match, List<K> keysToRemove, Func<K, V, Task> onRemove)
    {
        keysToRemove.Clear();
        foreach (var e in dict)
            if (match(e.Key, e.Value))
                keysToRemove.Add(e.Key);
        foreach (var k in keysToRemove)
        {
            await onRemove(k, dict[k]);
            dict.Remove(k);
        }

        keysToRemove.Clear();
    }
}
