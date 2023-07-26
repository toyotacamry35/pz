using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Src.Tools.Editor
{
    public class DedicBuildPreprocess : IPreprocessBuild
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            Debug.Log("Preprocess");
        }
    }
}
