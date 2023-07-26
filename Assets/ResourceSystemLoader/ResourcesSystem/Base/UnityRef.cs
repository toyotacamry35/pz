using ResourcesSystem.Loader;

namespace Assets.ColonyShared.SharedCode.ResourcesSystem.Base
{
    [KnownToGameResources]
    public class UnityRef<T> : IUnityRef where T : UnityEngine.Object
    {
        public string TargetPath { get; }

        private T _targetCached;
        private bool _targetResolved = false;
        public T Target
        {
            get
            {
                if (!_targetResolved)
                {
                    _targetCached = UnityObjectResolverHolder.Instance.Resolve(TargetPath, typeof(T)) as T;
                    _targetResolved = true;
                }
                return _targetCached;
            }
        }

        public object TargetObj => Target;

        public string Path => TargetPath;

        public UnityRef(string assetPath)
        {
            TargetPath = assetPath;
        }

        public override string ToString() => $"UnityRef<{typeof(T).Name}>:{TargetPath}";
    }


    public interface IUnityRef
    {
        string Path { get; }
        object TargetObj { get; }
    }



    
}
