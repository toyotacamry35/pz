using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ResourceSystemLoader.ResourcesSystem.Utils
{
    public class RiverExportMaterialSettings : UnityEngine.ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/River Material To Interaction Type Preset")]
        public static void CreateMyAsset()
        {
            RiverExportMaterialSettings asset = CreateInstance<RiverExportMaterialSettings>();

            AssetDatabase.CreateAsset(asset, "Assets/RiverMaterialToInteractionType.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
#endif

        public List<RiverMaterialToInteractionType> MaterialToInteractionType;
    }


    [Serializable]
    public struct RiverMaterialToInteractionType
    {
        public UnityEngine.Material Material;
        public RiverInteractionType Interaction;
    }
    public enum RiverInteractionType
    {
        None,
        Clear,
        Toxic
    }
}