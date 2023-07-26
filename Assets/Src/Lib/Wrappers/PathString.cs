using System;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;

namespace Assets.Src.Lib.Wrappers
{
    [Serializable]
    public struct PathString
    {
        public string Path;

        public static implicit operator string(PathString val)
        {
            return val.Path;
        }

        public T GetDef<T>() where T : class, IResource
        {
            if (string.IsNullOrEmpty(Path))
                return null;
            return GameResourcesHolder.Instance.LoadResource<T>(Path);
        }
    }
}
