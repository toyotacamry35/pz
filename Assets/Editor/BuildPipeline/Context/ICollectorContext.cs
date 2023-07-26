using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class AssetBundleReference
    {
        public AssetBundleBuild AssetBundleBuild;
        public List<MapBundle> Bundles;
    }

    public class MapBundleReference
    {
        public Dictionary<string, List<AssetBundleReference>> References = new Dictionary<string, List<AssetBundleReference>>();
    }

    public struct MapBundle
    {
        public string Map;
        public string Bundle;
    }
    
    public interface ICollectorContext : IContextObject
    {
        Dictionary<string, AssetBundleBuild[]> ProjectDependencies { get; set; }
        Dictionary<string, AssetBundleReference> SharedBundleToMapReferences { get; set; }
        Dictionary<string, MapBundleReference> SharedMapToBundleReferences { get; set; }
    }
}