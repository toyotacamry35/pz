using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.ResourcesSystem.Base
{
    public class ResourceArray<T> : IReadOnlyList<T> where T : class, IResource
    {
        private readonly ResourceRef<T>[] _source;
        private List<T> _cache;

        public ResourceArray(IEnumerable<IRefBase> source)
        {
            _source = source.Cast<ResourceRef<T>>().ToArray();
        }
        
        private ResourceArray() {}

        public int Length => _source.Length;

        public int Count => _source.Length;

        public T this[int idx] => Array[idx];
        
        public static readonly ResourceArray<T> Empty = new ResourceArray<T>(Enumerable.Empty<IRefBase>()); 
        
        public static implicit operator List<T>(ResourceArray<T> self) => self.Array;

        public bool Contains(T item) => Array.Contains(item);

        public int IndexOf(T item) => Array.IndexOf(item);

        public List<T>.Enumerator GetEnumerator() => Array.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Array.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => Array.GetEnumerator();

        private List<T> Array => _cache ?? (_cache = _source.Select(x => x.Target).ToList());
    }
}