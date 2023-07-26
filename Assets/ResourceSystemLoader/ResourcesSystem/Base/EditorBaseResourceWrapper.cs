using System;

namespace Assets.Src.ResourcesSystem.Base
{
#if UNITY_EDITOR

    public class EditorBaseResourceWrapper<T> : IEquatable<EditorBaseResourceWrapper<T>> where T : class, IResource
    {
        private readonly T _resource;

        public EditorBaseResourceWrapper(T resource)
        {
            _resource = resource;
        }

        public T Resource => _resource;

        public bool Equals(EditorBaseResourceWrapper<T> other) => Equals(this, other);

        public override bool Equals(object obj) => Equals(this, (EditorBaseResourceWrapper<T>) obj);

        public override int GetHashCode() => _resource != null ? _resource.Address.GetHashCode() : 0;

        public static bool operator ==(EditorBaseResourceWrapper<T> a, EditorBaseResourceWrapper<T> b) => Equals(a, b);

        public static bool operator !=(EditorBaseResourceWrapper<T> a, EditorBaseResourceWrapper<T> b) => !Equals(a, b);
        
//        public static implicit operator T(EditorBaseResourceWrapper<T> w) => w._resource;

//        public static implicit operator EditorBaseResourceWrapper<T>(T r) => new EditorBaseResourceWrapper<T>(r);

        public string ____GetDebugAddress() => (_resource as BaseResource)?.____GetDebugAddress();

        public string ____GetDebugShortName() => (_resource as BaseResource)?.____GetDebugShortName();

        public string ____GetDebugRootName() => (_resource as BaseResource)?.____GetDebugRootName();

        public override string ToString() =>  _resource != null ? _resource.ToString() : "null";


        private static bool Equals(EditorBaseResourceWrapper<T> a, EditorBaseResourceWrapper<T> b)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, b)) return true;
            if (ReferenceEquals(null, a) || ReferenceEquals(null, b)) return false;
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(null, a._resource) && ReferenceEquals(null, b._resource)) return true;
            if (ReferenceEquals(null, a._resource) || ReferenceEquals(null, b._resource)) return false;
            if (ReferenceEquals(a._resource, b._resource)) return true;
            return a._resource.Address.Equals(b._resource.Address);
        }
    }
#endif    
}