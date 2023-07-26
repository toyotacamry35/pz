using System.IO;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Utility
{
    [CustomEditor(typeof(MaskBackgroundCreator))]
    public class MaskBackgroundCreatorEditor : VegetationStudioBaseEditor
    {
        public override void OnInspectorGUI()
        {
            HelpTopic = "background-mask-creator";
            base.OnInspectorGUI();

            MaskBackgroundCreator maskBackgroundCreator = (MaskBackgroundCreator) target;
            VegetationSystem vegetationSystem = maskBackgroundCreator.gameObject.GetComponent<VegetationSystem>();
            if (vegetationSystem)
            {

                maskBackgroundCreator.BackgroundMaskQuality = (BackgroundMaskQuality)EditorGUILayout.EnumPopup("Mask resolution", maskBackgroundCreator.BackgroundMaskQuality);
                EditorGUILayout.HelpBox("Pixel resolution of the mask background. Low = 1024x1024, Normal = 2048x2048 and High =4096x4096", MessageType.Info);


                if (GUILayout.Button("Generate mask background/template"))
                {
                    GenerateMaskBackground(vegetationSystem);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add this component to a GameObject with a VegetationSystem component.",
                    MessageType.Error);

            }          
        }

        void GenerateMaskBackground(VegetationSystem vegetationSystem)
        {

            string path = EditorUtility.SaveFilePanelInProject("Save mask background", "", "png",
                "Please enter a file name to save the mask background to");
            if (path.Length != 0)
            {

                MaskBackgroundCreator maskBackgroundCreator = (MaskBackgroundCreator) target;

                Terrain selectedTerrain = vegetationSystem.currentTerrain;
                if (selectedTerrain == null) return;

                GameObject cameraObject = new GameObject {name = "Mask Background camera"};

                int textureResolution =
                    maskBackgroundCreator.GetBackgroundMaskQualityPixelResolution(maskBackgroundCreator
                        .BackgroundMaskQuality);

                Camera backgroundCamera = cameraObject.AddComponent<Camera>();
                backgroundCamera.orthographic = true;
                backgroundCamera.orthographicSize = selectedTerrain.terrainData.size.x / 2f;
                backgroundCamera.farClipPlane = 20000;

                RenderTexture rt =
                    new RenderTexture(textureResolution, textureResolution, 24, RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Linear)
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Trilinear,
                        autoGenerateMips = false
                    };
                backgroundCamera.targetTexture = rt;

                cameraObject.transform.position = selectedTerrain.transform.position +
                                                  new Vector3(selectedTerrain.terrainData.size.x / 2f, 1000,
                                                      selectedTerrain.terrainData.size.z / 2f);
                cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);

                backgroundCamera.Render();

                Texture2D newTexture = new Texture2D(textureResolution, textureResolution);
                RenderTexture.active = rt;
                newTexture.ReadPixels(new Rect(0, 0, textureResolution, textureResolution),0,0);
                RenderTexture.active = null;
                newTexture.Apply();
                SaveTexture(newTexture, path);

                backgroundCamera.targetTexture = null;
                DestroyImmediate(rt);
                DestroyImmediate(cameraObject);
            }
        }

        public static void SaveTexture(Texture2D tex, string name)
        {
            string savePath = Application.dataPath + name.Replace("Assets", "");
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
        }
    }
}
