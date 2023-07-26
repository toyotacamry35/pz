using System;
using System.IO;
using System.Linq;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using UnityEditor;

namespace Assets.Editor.ResourceSystem
{
    [InitializeOnLoad]
    public static class UnityObjectResolverInitializer
    {
        static UnityObjectResolverInitializer()
        {
            UnityObjectResolverHolder.Instance = EditorTimeResolver.Instance;
        }
    }

    public static class UnityObjectResolverInitializerPlayMode
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PlayModeInit()
        {
            UnityObjectResolverHolder.Instance = EditorTimeResolver.Instance;
        }
    }

    public class EditorTimeResolver : IUnityObjectResolver
    {
        public static EditorTimeResolver Instance { get; } = new EditorTimeResolver();
        public UnityEngine.Object Resolve(string path, Type type)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (!string.IsNullOrWhiteSpace(Path.GetExtension(path)))
                return AssetDatabase.LoadAssetAtPath(path, type);

            if (typeof(UnityEngine.GameObject).IsAssignableFrom(type))
                return AssetDatabase.LoadAssetAtPath(path + ".prefab", type);

            if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                return AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path + ".prefab")?.GetComponent(type);

            var folder = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);

            var assets = UnityEditor.AssetDatabase.FindAssets(name, new string[] { folder })
            .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
            .Distinct()
            .Where(v => v.Contains(path) && Path.GetFileNameWithoutExtension(v) == name && Path.GetExtension(v) != ".jdb")
            .Select(v => UnityEditor.AssetDatabase.LoadAssetAtPath(v, type))
            .Where(v => v != null);

            return assets.Single();
        }

        private EditorTimeResolver() { }
    }
}
