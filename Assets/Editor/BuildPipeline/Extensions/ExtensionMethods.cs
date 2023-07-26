using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    static class ExtensionMethods
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static void GetOrAdd<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, out List<TValue> value)
        {
            if (dictionary.TryGetValue(key, out value))
                return;

            value = new List<TValue>();
            dictionary.Add(key, value);
        }

        public static void GetOrAdd<TKey, TValue>(this IDictionary<TKey, HashSet<TValue>> dictionary, TKey key, out HashSet<TValue> value)
        {
            if (dictionary.TryGetValue(key, out value))
                return;

            value = new HashSet<TValue>();
            dictionary.Add(key, value);
        }

        public static Hash128 GetHash128(this BuildSettings settings)
        {
            if (settings.typeDB == null)
                return HashingMethods.Calculate(settings.target, settings.group, settings.buildFlags).ToHash128();
            return HashingMethods.Calculate(settings.target, settings.group, settings.buildFlags, settings.typeDB.GetHash128()).ToHash128();
        }
    }
}
