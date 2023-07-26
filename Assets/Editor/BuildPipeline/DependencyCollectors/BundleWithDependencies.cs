using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public struct BundleWithDependencies
    {
        public string MapName;
        public string Name;
        public string[] Dependencies;

        public BundleWithDependencies(string mapName, string name, string[] dependencies)
        {
            MapName = mapName;
            Name = name;
            Dependencies = dependencies;
        }

        public AssetBundleBuild GetBundle(string[] addressableNames = null)
        {
            return new AssetBundleBuild{assetBundleName = Name, assetNames = Dependencies, addressableNames = addressableNames};
        }
    }
}