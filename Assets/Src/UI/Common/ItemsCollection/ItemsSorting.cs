using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uins
{
    public static class ItemsSorting
    {
        /// <summary>
        /// Сортировка по порядку в списке elems
        /// </summary>
        public static void SortSiblings<T>(IList<T> elems) where T : MonoBehaviour
        {
            if (elems == null || elems.Count == 0)
                return;

            var sortedList = new SortedList<int, T>();
            for (int j = 0; j < elems.Count; j++)
                AddWithUniqueIndex(sortedList, j, elems[j]);

            int i = 0;
            foreach (var elem in sortedList.Values)
                elem.transform.SetSiblingIndex(i++);
        }

        /// <summary>
        /// Сортировка по функции getIndex от элемента
        /// </summary>
        public static void SortSiblings<T>(ICollection<T> elems, Func<T, int> getIndex) where T : MonoBehaviour
        {
            if (elems == null || elems.Count == 0)
                return;

            var sortedList = new SortedList<int, T>();
            foreach (var elem in elems)
                AddWithUniqueIndex(sortedList, getIndex(elem), elem);

            int i = 0;
            foreach (var elem in sortedList.Values)
                elem.transform.SetSiblingIndex(i++);
        }

        private static void AddWithUniqueIndex<T>(SortedList<int, T> sortedList, int recipeSortIndex, T elem)
        {
            while (sortedList.ContainsKey(recipeSortIndex))
                recipeSortIndex++;

            sortedList.Add(recipeSortIndex, elem);
        }
    }
}