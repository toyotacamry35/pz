using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Assets.Src.Lib.Extensions
{
    public static class ListExtensions
    {
        public static T Fetch<T>(this List<T> list, Predicate<T> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            T rv = default(T);
            var idx = list.FindIndex(predicate);
            if (idx != -1)
            {
                rv = list[idx];
                list.RemoveAt(idx);
            }
            return rv;
        }
        
        public static IEnumerable<T> FetchAll<T>(this List<T> list, Predicate<T> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var rv = new List<T>(); 
            for (int i = list.Count - 1; i >= 0; --i)
            {
                var item = list[i];
                if (predicate(item))
                {
                    rv.Add(item);
                    list.RemoveAt(i);
                }
            }
            return rv;
        }

        public static IEnumerable<T> FetchAll<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            var rv = new T[list.Count];
            list.CopyTo(rv);
            list.Clear();
            return rv;

        }

        public static void InsertSorted<T>(this List<T> list, T element, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            for (int i = 0, cnt = list.Count; i < cnt; ++i)
            {
                if (comparer.Compare(list[i], element) > 0)
                {
                    list.Insert(i, element);
                    return;
                }
            }
            list.Add(element);
        }
    }
}