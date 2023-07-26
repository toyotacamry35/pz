using System.Collections.Generic;
using Assets.Src.BuildScripts;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public interface IBuildDependencyCollector
    {
        AssetBundleBuild[] Collect(string mapName, BuildConfig config);
    }
}