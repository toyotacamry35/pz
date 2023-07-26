using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor.Tools
{
    public class VariousObjectOperations : EditorWindow
    {
        [MenuItem("Tools/Tech Art/Destroy Object %&d")]
        private static void DestroySelectedObject()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                //if (obj == null)
                //    Debug.LogError("One of selected objects is 'null'!");
                //else
                DestroyImmediate(obj, true);
            }
        }

        [MenuItem("Tools/Tech Art/Find Assets with 'null' dependencies")]
        private static void FindAssetsWithNullDeps()
        {
            var allAssetsWithDeps = AssetDatabase.FindAssets("")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Distinct()
                .Select(assetPath => new { assetPath = assetPath, assetReprs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath) });
            foreach (var assetWithDeps in allAssetsWithDeps)
            {
                if (assetWithDeps.assetReprs.Any(x => x == null))
                {
                    Debug.Log($"{assetWithDeps.assetPath}");
                }
            }
        }

        [MenuItem("Tools/Tech Art/Fix RAM 2019 scripts")]
        private static void FixRAM2019Scripts()
        {
            var ramObjects = Object.FindObjectsOfType<RamSpline>();
            HashSet<Scene> scenes = new HashSet<Scene>();
            foreach (var ramObject in ramObjects)
            {
                scenes.Add(ramObject.gameObject.scene);
                if (ramObject.controlPoints.Count > ramObject.controlPointsSnap.Count)
                {
                    var difference = ramObject.controlPoints.Count - ramObject.controlPointsSnap.Count;
                    for (int i = 0; i < difference; i++)
                    {
                        ramObject.controlPointsSnap.Add(0);
                    }
                }
                if (ramObject.controlPoints.Count > ramObject.controlPointsMeshCurves.Count)
                {
                    var difference = ramObject.controlPoints.Count - ramObject.controlPointsMeshCurves.Count;
                    for (int i = 0; i < difference; i++)
                    {
                        ramObject.controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }));
                    }
                }
                RamSplineEditor.CheckRotations(ramObject);
                ramObject.GenerateSpline();
            }
            foreach (var scene in scenes)
                EditorSceneManager.MarkSceneDirty(scene);
        }

        [MenuItem("Tools/Tech Art/Check textures compression")]
        private static void CheckAllTextures()
        {
            var textureAssetsGuids = AssetDatabase.FindAssets("t:Texture");
            int i = 0;
            foreach (var textureGuid in textureAssetsGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(textureGuid);
                if (!path.StartsWith("Assets/Content/") || path.StartsWith("Assets/Content/RegionsData/"))
                    continue;
                i++;
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Debug.LogError(path + "\t" + texture.graphicsFormat);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                var platformSettings = textureImporter.GetPlatformTextureSettings("Standalone");
                platformSettings.overridden = true;
                platformSettings.format = TextureImporterFormat.DXT5Crunched;
                textureImporter.SetPlatformTextureSettings(platformSettings);
                textureImporter.SaveAndReimport();
            }
            Debug.Log($"Total count: {i}");
            AssetDatabase.SaveAssets();
        }
    }
}
