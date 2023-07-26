using UnityEngine;
using UnityEditor;

namespace Assets.TerrainBaker.Editor
{
    public class CreateTerrainAtlas
    {
        [MenuItem("Assets/Create/Terrain/Terrain Atlas")]
        public static void CreateMyAsset()
        {
            string path = "";
            Object obj = Selection.activeObject;
            if (obj != null)
                path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            else
                path = "Assets";

            string atlasPath;
            string atlasBuilderPath;
            TerrainAtlas atlas;
            TerrainAtlasBuilder atlasBuilder;
            for (int i = -1; ; i++)
            {
                if (i > 100)
                {
                    return;
                }
                string num = i >= 0 ? i.ToString() : "";
                atlasPath = path + "/" + "TerrainAtlas" + num + ".asset";
                if (AssetDatabase.LoadAssetAtPath(atlasPath, typeof(TerrainAtlas)) != null)
                {
                    continue;
                }
                atlasBuilderPath = path + "/" + "TerrainAtlasBuilder" + num + ".asset";
                if (AssetDatabase.LoadAssetAtPath(atlasBuilderPath, typeof(TerrainAtlasBuilder)) != null)
                {
                    continue;
                }
                break;
            }

            atlas = ScriptableObject.CreateInstance<TerrainAtlas>();
            atlasBuilder = ScriptableObject.CreateInstance<TerrainAtlasBuilder>();

            AssetDatabase.CreateAsset(atlasBuilder, atlasBuilderPath);
            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();


            atlas = null;
            atlas = (TerrainAtlas)AssetDatabase.LoadAssetAtPath(atlasPath, typeof(TerrainAtlas));
            atlasBuilder.terrainAtlas = atlas;
            EditorUtility.SetDirty(atlas);
            EditorUtility.SetDirty(atlasBuilder);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = atlasBuilder;
        }
    }
}
