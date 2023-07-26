using Assets.Src.Lib.ProfileTools;
using UnityEditor;
using UnityEngine;

namespace Assets.Instancenator.Editor
{
    public class Utils
    {
        public static void OnChange(InstanceComposition ic)
        {
            EditorUtility.SetDirty(ic);
            ProcessAllRenderers(true, obj => { if (obj.composition == ic) obj.OnChangeComposition(); });
        }

        public static InstanceComposition LoadOrCreate(string assetPath)
        {
            InstanceComposition instanceComposition = AssetDatabase.LoadAssetAtPath<InstanceComposition>(assetPath);
            if (instanceComposition == null)
            {
                instanceComposition = ScriptableObject.CreateInstance<InstanceComposition>();
                AssetDatabase.CreateAsset(instanceComposition, assetPath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                instanceComposition = AssetDatabase.LoadAssetAtPath<InstanceComposition>(assetPath);
            }
            return instanceComposition;
        }

        [MenuItem("Level Design/Instansinator/Enable in editor")]
        private static void EnableInEditor()
        {
            ProcessAllRenderers(true, obj => obj.isEnableDrawInEditor = true);
            SceneView.RepaintAll();
        }

        [MenuItem("Level Design/Instansinator/Disable in editor")]
        public static void DisableInEditor()
        {
            ProcessAllRenderers(true, obj => obj.isEnableDrawInEditor = false);
            SceneView.RepaintAll();
        }

        [MenuItem("Level Design/Instansinator/Show bounds (gismo)")]
        private static void ShowBoundsInEditor()
        {
            ProcessAllRenderers(true, obj => obj.isEnableDrawBlocksBound = true);
            SceneView.RepaintAll();
        }

        [MenuItem("Level Design/Instansinator/Hide bounds (gismo)")]
        public static void HideBoundsInEditor()
        {
            ProcessAllRenderers(true, obj => obj.isEnableDrawBlocksBound = false);
            SceneView.RepaintAll();
        }


        [MenuItem("Level Design/Instansinator/Refresh")]
        public static void RefreshRenderers()
        {
            ProcessAllRenderers(true, obj => { obj.OnChangeComposition(); });
            SceneView.RepaintAll();
        }

        private delegate void DoSomething(InstanceCompositionRenderer renderer);
        private static void ProcessAllRenderers(bool isSetDirty, DoSomething doSomething)
        {
            InstanceCompositionRenderer[] renderers = Profile.FindObjectsOfTypeAll<InstanceCompositionRenderer>();
            foreach (InstanceCompositionRenderer r in renderers)
            {
                doSomething(r);
                if (isSetDirty)
                {
                    EditorUtility.SetDirty(r);
                }
            }
        }
    }

}

