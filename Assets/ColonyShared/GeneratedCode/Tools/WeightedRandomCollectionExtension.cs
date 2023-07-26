using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Assets.Src.Tools
{
    public static class WeightedRandomCollectionExtension
    {
        
        [ThreadStatic]
        static Random _rand = new Random(42);
        struct WeightedElement
        {
            public int ElementWeight;
            public object obj;
        }
        [ThreadStatic]
        static WeightedElement[] Buffer = new WeightedElement[100];
        public static T WeightedChoice<T>([NotNull] this IList<T> objects, [NotNull] Func<T, int> weightFunc) where T : class
        {
            if (Buffer.Length < objects.Count)
                Buffer = new WeightedElement[objects.Count + 10];
            int acc = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                acc += weightFunc(objects[i]);
                Buffer[i] = new WeightedElement { ElementWeight = acc, obj = objects[i] };
            }
            int choice = _rand.Next(acc) + 1;
            for (int i = 0; i < objects.Count; i++)
                if (Buffer[i].ElementWeight >= choice)
                    return (T)Buffer[i].obj;
            return null;
        }
    }
    public static class WeightedRandomCollectionExtensionAsync
    {

        [ThreadStatic]
        static Random _rand;
        struct WeightedElement
        {
            public int ElementWeight;
            public object obj;
        }
        static ConcurrentStack<WeightedElement[]> Buffers = new ConcurrentStack<WeightedElement[]>();
        public static async Task<T> WeightedChoice<T>([NotNull] this IList<T> objects, [NotNull] Func<T, Task<int>> weightFunc) where T : class
        {
            if (!Buffers.TryPop(out var Buffer))
                Buffer = new WeightedElement[Math.Max(100, objects.Count + 10)];
            else
                if(Buffer.Length < objects.Count + 10)
                    Buffer = new WeightedElement[Math.Max(100, objects.Count + 10)];
            if (Buffer.Length < objects.Count)
                Buffer = new WeightedElement[objects.Count + 10];
            int acc = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                acc += await weightFunc(objects[i]);
                Buffer[i] = new WeightedElement { ElementWeight = acc, obj = objects[i] };
            }
            if (_rand == null)
                _rand = new Random();
            int choice = _rand.Next(acc) + 1;
            for (int i = 0; i < objects.Count; i++)
                if (Buffer[i].ElementWeight >= choice)
                {
                    var o = (T)Buffer[i].obj;
                    Buffers.Push(Buffer);
                    return o;
                }
            Buffers.Push(Buffer);
            return null;
        }
    }
}