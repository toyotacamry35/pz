using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public static class DependencyCacheController<T> where T: class, ICacheEntry
    {
        private static Dictionary<string, T> _dependencies = new Dictionary<string, T>();

        public static bool UseCache => BuildPipelineManager.GetBuildPipelineSettings().DependencyCache;
        
        private static bool CheckExistedCache(string name)
        {
            var dep = _dependencies[name];
            if (dep == null)
            {
                _dependencies.Remove(name);
                return false;
            }
            dep.IsValid =  dep.Check();
            return dep.IsValid ;
        }

        public static void InvalidateCache(string name)
        {
            if (_dependencies.ContainsKey(name))
                _dependencies[name].IsValid = false;

            if (BuildCacheHelper.IsCacheExist(name))
                BuildCacheHelper.RemoveCache(name);
        }
        
        public static void InvalidateCacheAll()
        {
            _dependencies.Clear();

            BuildCacheHelper.RemoveCacheAll();
        }

        public static T ReadCache(string name)
        {
            return Validate(name) ? _dependencies[name] : default;
        }

        public static bool WriteCache(string name, T cacheEntry)
        {
            if (_dependencies.ContainsKey(name))
                _dependencies.Remove(name);
            
            _dependencies.Add(name,cacheEntry);
            return BuildCacheHelper.WriteCache(name,cacheEntry);
        }

        public static bool Validate(string name)
        {
            if (_dependencies.ContainsKey(name))
                return CheckExistedCache(name);

            if (!BuildCacheHelper.IsCacheExist(name))
                return false;

            var cache = BuildCacheHelper.ReadCache<T>(name);
            _dependencies.Add(name, cache);
            return CheckExistedCache(name);
        }
    }
}