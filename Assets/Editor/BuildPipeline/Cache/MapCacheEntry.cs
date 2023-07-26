using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assets.Test.Src.Editor
{
    public interface ICacheEntry
    {
        bool IsValid { get; set; }

        bool Check();
    }

    public class MapCacheEntry : ICacheEntry
    {
        public List<string> Name;
        public List<string[]> Dependencies;
        public bool IsValid { get; set; }

        public bool Check()
        {
            return IsValid &&
                   Dependencies.All(
                       paths =>
                           paths.All(path => File.Exists(UnityEditorUtils.GetAbsolutePathFromLocalPath(path))));
        }
    }

    public class SystemCacheEntry : ICacheEntry
    {
        public List<(string, string)> Addresses;
        public string[] SceneDeps;
        public string[] Scenes;
        public bool IsValid { get; set; }

        public bool Check()
        {
            return IsValid && Addresses.All(path => File.Exists(UnityEditorUtils.GetAbsolutePathFromLocalPath(path.Item1))) 
                           && Scenes.All(s => File.Exists(UnityEditorUtils.GetAbsolutePathFromLocalPath(s))) 
                           && SceneDeps.All(s => File.Exists(UnityEditorUtils.GetAbsolutePathFromLocalPath(s)));
        }
    }
}