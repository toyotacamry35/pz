using System.Collections.Generic;
using System.Text;
using System;
using JetBrains.Annotations;

namespace Assets.Src.Lib.Extensions
{
    public static class DictionaryExtension
    {
        public static string ToStringCustom<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> dic, string keyToValueDelim = ":  ", string entriesDelim = ",\n")
        {
            if (dic.Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            bool isFirstEntry = true;
            foreach (var kvp in dic)
                if (!isFirstEntry)
                    sb.Append($"{entriesDelim}{kvp.Key.ToString()}{keyToValueDelim}{kvp.Value.ToString()}");
                else
                {
                    isFirstEntry = false;
                    sb.Append($"{kvp.Key.ToString()}{keyToValueDelim}{kvp.Value.ToString()}");
                }
            return sb.ToString();
        }

        public static List<KeyValuePair<TKey, TValue>> Fetch<TKey, TValue>(this Dictionary<TKey, TValue> self, Predicate<KeyValuePair<TKey,TValue>> predicate)
        {
            var fetched = new List<KeyValuePair<TKey, TValue>>();
            foreach (var kvp in self)
                if (predicate(kvp))
                    fetched.Add(kvp);
            foreach (var kvp in fetched)
                self.Remove(kvp.Key);
            return fetched;
        }
    }
}
