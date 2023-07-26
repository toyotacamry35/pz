using System.Collections.Generic;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public class AssetBundleBuildAggregator
    {
        public string assetBundleName;
        public string assetBundleVariant;

        public List<string> assetNames;
        public List<string> addressableNames;

        public AssetBundleBuild GetBundle()
        {
            return addressableNames == null
                ? new AssetBundleBuild {assetBundleName = assetBundleName, assetBundleVariant = assetBundleVariant, assetNames = assetNames.ToArray()}
                : new AssetBundleBuild {assetBundleName = assetBundleName, assetBundleVariant = assetBundleVariant, assetNames = assetNames.ToArray(), addressableNames = addressableNames.ToArray()};
        }
    }
}