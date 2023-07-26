using System.Collections.Generic;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public class CollectorContext : ICollectorContext
    {
        public Dictionary<string, AssetBundleBuild[]> ProjectDependencies { get; set; }
        public Dictionary<string, AssetBundleReference> SharedBundleToMapReferences { get; set; }
        public Dictionary<string, MapBundleReference> SharedMapToBundleReferences { get; set; }
    }
}