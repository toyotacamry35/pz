using Assets.ResourceSystem.ResourcesSystem.Base;
using ResourcesSystem.Loader;

namespace Assets.Src.ResourcesSystem.Base
{
    public interface IRefBase
    {
        IResource TargetBase { get; }
        ResourceRef<T2> To<T2>() where T2 : class, IResource;
    }

    public interface IResourceRef<out T> where T : class, IResource
    {
        T Target { get; }
        bool IsValid { get; }
    }

    public struct ResourceRef<T> : IRefBase, IResourceRef<T> where T : class, IResource
    {
        private T _object;
        private bool _loaded;

        private readonly IResourcesRepository OwningRepository;
        private readonly ResourceIDFull Address;

        public IResource TargetBase => Target;

        public T Target
        {
            get
            {
                Load();
                return _object;
            }
        }

        public bool IsValid => Target != null;

        public ResourceRef(IResourcesRepository resourcesRepository, in ResourceIDFull id)
        {
            OwningRepository = resourcesRepository;
            Address = id;
            _object = null;
            _loaded = false;
        }
        
        public ResourceRef(T obj)
        {
            OwningRepository = obj?.OwningRepository;
            Address = obj?.Address ?? default;
            _object = obj;
            _loaded = true;
        }

        public ResourceRef(string path) : this(ResourceIDFull.Parse(path))
        {}

        public ResourceRef(in ResourceIDFull resId) : this(ResourceRefFactory.Instance, resId)
        {}

        private void Load()
        {
            if (_loaded)
                return;

            if (OwningRepository != null && Address != default)
                _object = OwningRepository.LoadResource<T>(Address);

            _loaded = true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ResourceRef<T>))
                return false;
            var other = (ResourceRef<T>)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            if (Target == null)
                return base.GetHashCode();

            return Target.GetHashCode();
        }

        public override string ToString()
        {
            return $"Ref -> {Target}";
        }

        public static bool operator ==(ResourceRef<T> a, ResourceRef<T> b)
        {
            return a.Target == b.Target;
        }

        public static bool operator !=(ResourceRef<T> a, ResourceRef<T> b)
        {
            return !(a == b);
        }

        public static implicit operator ResourceRef<T>(T obj)
        {
            return new ResourceRef<T>(obj);
        }

        public static implicit operator T(ResourceRef<T> obj)
        {
            return obj.Target;
        }

        public ResourceRef<T2> To<T2>() where T2 : class, IResource
        {
            var res = new ResourceRef<T2>(OwningRepository, Address);
            IResource obj = _object;
            res._object = (T2)obj;
            res._loaded = _loaded;
            return res;
        }

    }
}