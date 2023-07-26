using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.WorldFiller.Editor
{
    class LayerFillingEditor : EditorWindow
    {
        [MenuItem("Assets/Create/Terrain/Terrain Layer Filling")]

        public static void CreateLayerFilling()
        {
            LayerFillingData filling = ScriptableObject.CreateInstance<LayerFillingData>();

            AssetDatabase.CreateAsset(filling, "Assets/TerrainLayerFilling.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = filling;
        }


        public ScriptableObject layerFilling;

        [MenuItem("Window/Terrain layer filling")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow(typeof(LayerFillingEditor));
        }


        void OnEnable()
        {
            if (EditorPrefs.HasKey("ObjectPath"))
            {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                layerFilling = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ScriptableObject)) as ScriptableObject;
            }

        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();




            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(layerFilling);
            }
        }


        void OnGUINode()
        {
        }

    }
}
