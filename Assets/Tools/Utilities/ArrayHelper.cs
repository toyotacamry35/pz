using System;
using UnityEngine;
using System.Collections;
namespace Utilities
{
    public class ArrayHelper<T>
    {
        public T[] array;
        public int currentCount;
        public void Add(T t)
        {
            if (array == null || array.Length == 0)
            {
                array = new T[10];
                currentCount = 0;
            }
            if (currentCount >= array.Length)
                Resize();
            array[currentCount] = t;
            currentCount++;
        }

        void Resize()
        {
            var newLength = (int)((float)array.Length * 1.3f);
            var newArray = new T[newLength];
            for (int i = 0; i < array.Length; i++)
                newArray[i] = array[i];
            array = newArray;
        }

        public void Clear()
        {
            currentCount = 0;
        }
    }

    public static class FindArrayExt
    {
        public static int IndexOf<T>(this T[] array, T obj)
        {
            for(int i = 0; i < array.Length; i++)
                if (array[i].Equals(obj))
                    return i;
            return -1;
        }

        public static int IndexOf<T>(this T[] array, Func<T, bool> checker)
        {
            for (int i = 0; i < array.Length; i++)
                if (checker(array[i]))
                    return i;
            return -1;
        }
    }
}