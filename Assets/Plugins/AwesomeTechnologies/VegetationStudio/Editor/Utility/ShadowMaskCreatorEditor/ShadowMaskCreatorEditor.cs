using Assets.Src.Lib.ProfileTools;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Utility
{

    public struct ShadowMapData
    {
        public float TopHeight;
        public float BottomHeight;
    }

    [CustomEditor(typeof(ShadowMaskCreator))]
    public class ShadowMaskCreatorEditor : VegetationStudioBaseEditor
    {
        public override void OnInspectorGUI()
        {
            HelpTopic = "vegetation-color-mask-creator";

            ShadowMaskCreator shadowMaskCreator = (ShadowMaskCreator)target;
            VegetationSystem vegetationSystem = shadowMaskCreator.gameObject.GetComponent<VegetationSystem>();

            if (vegetationSystem)
            {
                if (vegetationSystem.GetSleepMode() || !vegetationSystem.enabled)
                {
                    EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings",
                        MessageType.Info);
                    return;
                }

                if (!SystemInfo.supportsComputeShaders)
                {
                    EditorGUILayout.HelpBox("This component needs compute shader support.",MessageType.Warning);
                    return;
                }
            }
            else
            {
                    EditorGUILayout.HelpBox("Add this component to a GameObject with a VegetationSystem component.",
                        MessageType.Error);
                    return;
            }

            base.OnInspectorGUI();

            if (vegetationSystem)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Settings", LabelStyle);
                shadowMaskCreator.ShadowMaskQuality =
                    (ShadowMaskQuality)EditorGUILayout.EnumPopup("Mask resolution",
                        shadowMaskCreator.ShadowMaskQuality);
                EditorGUILayout.HelpBox(
                    "Pixel resolution of the shadow mask. Low = 1024x1024, Normal = 2048x2048, High = 4096x4096 and Ultra = 8192x8192",
                    MessageType.Info);

                //shadowMaskCreator.InvisibleLayer = EditorGUILayout.IntSlider("Mask render layer", shadowMaskCreator.InvisibleLayer, 0, 30);
                //EditorGUILayout.HelpBox("Select a empty layer with no scene objects. This is used to render the color mask.", MessageType.Info);

                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Included vegetation", LabelStyle);
                shadowMaskCreator.IncludeTrees = EditorGUILayout.Toggle("Include Trees", shadowMaskCreator.IncludeTrees);
                shadowMaskCreator.IncludeLargeObjects = EditorGUILayout.Toggle("Include Large Objects", shadowMaskCreator.IncludeLargeObjects);
                GUILayout.EndVertical();

                if (GUILayout.Button("Generate shadow mask"))
                {
                    GenerateVegetationShadowMask(vegetationSystem);
                }

            }
        }

        bool RenderVegetationType(VegetationType vegetationType, ShadowMaskCreator shadowMaskCreator)
        {
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    return false;
                case VegetationType.Plant:
                    return false;
                case VegetationType.Tree:
                    return shadowMaskCreator.IncludeTrees;
                case VegetationType.Objects:
                    return false;
                case VegetationType.LargeObjects:
                    return shadowMaskCreator.IncludeLargeObjects;
            }
            return false;
        }

        private void GenerateVegetationShadowMask(VegetationSystem vegetationSystem)
        {
            string path = EditorUtility.SaveFilePanelInProject("Save mask background", "", "png",
                "Please enter a file name to save the mask background to");

            if (path.Length != 0)
            {
                ShadowMaskCreator shadowMaskCreator = (ShadowMaskCreator)target;

                Terrain selectedTerrain = vegetationSystem.currentTerrain;
                if (selectedTerrain == null) return;

                GameObject cameraObject = new GameObject { name = "Mask Background camera" };

                int textureResolution = shadowMaskCreator.GetShadowMaskQualityPixelResolution(shadowMaskCreator.ShadowMaskQuality);
                Shader heightShader = Shader.Find("AwesomeTechnologies/Shadows/ShadowHeight");

                RenderTexture rtDown =
                    new RenderTexture(textureResolution, textureResolution, 24, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear)
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Trilinear,
                        autoGenerateMips = false
                    };

                RenderTexture rtUp =
                    new RenderTexture(textureResolution, textureResolution, 24, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear)
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Trilinear,
                        autoGenerateMips = false
                    };

                Camera backgroundCamera = cameraObject.AddComponent<Camera>();
                backgroundCamera.targetTexture = rtDown;

                backgroundCamera.clearFlags = CameraClearFlags.Color;
                backgroundCamera.backgroundColor = Color.black;
                backgroundCamera.orthographic = true;
                backgroundCamera.orthographicSize = selectedTerrain.terrainData.size.x / 2f;
                backgroundCamera.farClipPlane = 20000;
                backgroundCamera.cullingMask = 1 << shadowMaskCreator.InvisibleLayer;
                backgroundCamera.SetReplacementShader(heightShader, "");

                cameraObject.transform.position = selectedTerrain.transform.position +
                                                  new Vector3(selectedTerrain.terrainData.size.x / 2f, 1000,
                                                      selectedTerrain.terrainData.size.z / 2f);
                cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);

                RenderTexture.active = rtDown;


                Graphics.SetRenderTarget(rtDown);
                GL.Viewport(new Rect(0, 0, rtDown.width, rtDown.height));
                GL.Clear(true, true, new Color(0, 0, 0, 0), 1f);

                GL.PushMatrix();
                GL.LoadProjectionMatrix(backgroundCamera.projectionMatrix);
                GL.modelview = backgroundCamera.worldToCameraMatrix;
                GL.PushMatrix();
                RenderVegetationNow(vegetationSystem, shadowMaskCreator);

                

                GL.PopMatrix();
                GL.PopMatrix();

                //RenderVegetation(vegetationSystem, shadowMaskCreator);
                //backgroundCamera.Render();

                cameraObject.transform.position = selectedTerrain.transform.position +
                                                  new Vector3(selectedTerrain.terrainData.size.x / 2f, -1000,
                                                      selectedTerrain.terrainData.size.z / 2f);
                cameraObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
                RenderTexture.active = rtUp;
                backgroundCamera.targetTexture = rtUp;


                Graphics.SetRenderTarget(rtUp);
                GL.Viewport(new Rect(0, 0, rtUp.width, rtUp.height));
                GL.Clear(true, true, new Color(0, 0, 0, 0), 1f);

                GL.PushMatrix();
                GL.LoadProjectionMatrix(backgroundCamera.projectionMatrix);
                GL.modelview = backgroundCamera.worldToCameraMatrix;
                GL.PushMatrix();
                RenderVegetationNow(vegetationSystem, shadowMaskCreator);

                GL.PopMatrix();
                GL.PopMatrix();

                //RenderVegetation(vegetationSystem, shadowMaskCreator);
                //backgroundCamera.Render();


                EditorUtility.DisplayProgressBar("Create shadow mask", "Render vegetation", 0f);
                RenderTexture.active = null;

                ShadowMapData[] outputHeights = new ShadowMapData[textureResolution* textureResolution];
                ComputeBuffer outputHeightBuffer = new ComputeBuffer(textureResolution * textureResolution, 8);

                ComputeShader decodeShader = Profile.Load<ComputeShader>("DecodeShadowHeight");
                int decodeKernelHandle = decodeShader.FindKernel("CSMain");
                decodeShader.SetTexture(decodeKernelHandle,"InputDown", rtDown);
                decodeShader.SetTexture(decodeKernelHandle, "InputUp", rtUp);
                decodeShader.SetBuffer(decodeKernelHandle, "OutputHeightBuffer", outputHeightBuffer);
                decodeShader.SetInt("TextureResolution", textureResolution);
                decodeShader.Dispatch(decodeKernelHandle,textureResolution / 8, textureResolution / 8, 1);
                outputHeightBuffer.GetData(outputHeights);
                outputHeightBuffer.Dispose();

                EditorUtility.DisplayProgressBar("Create shadow mask", "Calculate heights", 0.33f);
                Texture2D outputTexture = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, true);              
                Color32[] outputColors = new Color32[outputHeights.Length];

                for (int x = 0; x <= textureResolution - 1; x++)
                {
                    for (int y = 0; y <= textureResolution - 1; y++)
                    {
                        int i = x + y * textureResolution;

                        float xNormalized = (float) x / textureResolution;
                        float yNormalized = (float) y / textureResolution;

                        float terrainHeight =
                            vegetationSystem.UnityTerrainData.GetInterpolatedHeight(xNormalized, yNormalized);

                        float vegetationHeightUp = outputHeights[i].BottomHeight - vegetationSystem.UnityTerrainData.terrainPosition.y;
                        float relativeHeightUp = Mathf.Clamp((vegetationHeightUp - terrainHeight)*4,0,255);

                        float vegetationHeightDown = outputHeights[i].TopHeight - vegetationSystem.UnityTerrainData.terrainPosition.y;
                        float relativeHeightDown = Mathf.Clamp((vegetationHeightDown - terrainHeight) * 4, 0, 255);

                        if (relativeHeightUp > relativeHeightDown) relativeHeightUp = relativeHeightDown;

                        outputColors[i].a = 255;
                        outputColors[i].r = (byte)relativeHeightDown;
                        outputColors[i].g = 0;
                        //if (relativeHeightUp > 0)
                        //{
                        //    outputColors[i].g = 1;
                        //}
                        //else
                        //{
                        //outputColors[i].g = (byte)relativeHeightUp;
                        //}
                       
                        outputColors[i].b = 0;
                    }
                }

                outputTexture.SetPixels32(outputColors);
                outputTexture.Apply();

                backgroundCamera.targetTexture = null;
                DestroyImmediate(rtDown);
                DestroyImmediate(rtUp);

                SaveTexture(outputTexture, path);
                DestroyImmediate(cameraObject);
                DestroyImmediate(outputTexture);
                //GC.Collect();

                EditorUtility.DisplayProgressBar("Create shadow mask", "importing asset", 0.66f);
                string assetPath = path;
                var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (tImporter != null)
                {
                    tImporter.textureType = TextureImporterType.Default;
                    tImporter.filterMode = FilterMode.Point;
                    tImporter.maxTextureSize = 8192;
                    tImporter.SaveAndReimport();
                }
                EditorUtility.ClearProgressBar();
            }
        } 

        public static void SaveTexture(Texture2D tex, string name)
        {
            string savePath = Application.dataPath + name.Replace("Assets", "");
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
        }
        //void RenderVegetation(VegetationSystem vegetationSystem, ShadowMaskCreator shadowMaskCreator)
        //{
        //    for (int i = 0; i <= vegetationSystem.CurrentVegetationPackage.VegetationInfoList.Count - 1; i++)
        //    {
        //        VegetationItemInfo vegetationItemInfo = vegetationSystem.CurrentVegetationPackage.VegetationInfoList[i];
        //        for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationMeshLod0.subMeshCount - 1; l++)
        //        {
        //            vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l].SetFloat("_CullFarStart", 50000);
        //            vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l].SetFloat("_CullFarDistance", 20);
        //        }

        //        if (!RenderVegetationType(vegetationItemInfo.VegetationType, shadowMaskCreator)) continue;

        //        for (int j = 0; j <= vegetationSystem.VegetationCellList.Count - 1; j++)
        //        {
        //            if (j % 100 == 0)
        //            {
        //                EditorUtility.DisplayProgressBar("Render vegetation item: " + vegetationItemInfo.Name, "Render cell " + j + "/" + (vegetationSystem.VegetationCellList.Count - 1), j / ((float)vegetationSystem.VegetationCellList.Count - 1));
        //            }

        //            VegetationCell vegetationCell = vegetationSystem.VegetationCellList[j];
        //            List<Matrix4x4> instanceList =
        //                vegetationCell.DirectSpawnVegetation(vegetationItemInfo.VegetationItemID, true);
        //            for (int k = 0; k <= instanceList.Count - 1; k++)
        //            {
        //                Vector3 position = MatrixTools.ExtractTranslationFromMatrix(instanceList[k]);
        //                Vector3 scale = MatrixTools.ExtractScaleFromMatrix(instanceList[k]);
        //                Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(instanceList[k]);
        //                Vector3 newPosition = new Vector3(position.x, position.y, position.z);
        //                Vector3 newScale = new Vector3(scale.x , scale.y, scale.z );
        //                Matrix4x4 newMatrix = Matrix4x4.TRS(newPosition, rotation, newScale);

        //                for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationMeshLod0.subMeshCount - 1; l++)
        //                {
        //                    Graphics.DrawMesh(vegetationSystem.VegetationModelInfoList[i].VegetationMeshLod0, newMatrix, vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l], shadowMaskCreator.InvisibleLayer, null, l);
        //                }
        //            }
        //        }
        //        EditorUtility.ClearProgressBar();
        //    }
        //}

        void RenderVegetationNow(VegetationSystem vegetationSystem, ShadowMaskCreator shadowMaskCreator)
        {
            Shader overrideShader = Shader.Find("AwesomeTechnologies/Shadows/ShadowHeight");

            for (int i = 0; i <= vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = vegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1; l++)
                {
                    vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l].SetFloat("_CullFarStart", 50000);
                    vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l].SetFloat("_CullFarDistance", 20);
                }

                if (!RenderVegetationType(vegetationItemInfo.VegetationType, shadowMaskCreator)) continue;

                for (int j = 0; j <= vegetationSystem.VegetationCellList.Count - 1; j++)
                {
                    if (j % 100 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Render vegetation item: " + vegetationItemInfo.Name, "Render cell " + j + "/" + (vegetationSystem.VegetationCellList.Count - 1), j / ((float)vegetationSystem.VegetationCellList.Count - 1));
                    }

                    VegetationCell vegetationCell = vegetationSystem.VegetationCellList[j];
                    List<Matrix4x4> instanceList =
                        vegetationCell.DirectSpawnVegetation(vegetationItemInfo.VegetationItemID, true);
                    for (int k = 0; k <= instanceList.Count - 1; k++)
                    {
                        Vector3 position = MatrixTools.ExtractTranslationFromMatrix(instanceList[k]);
                        Vector3 scale = MatrixTools.ExtractScaleFromMatrix(instanceList[k]);
                        Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(instanceList[k]);
                        Vector3 newPosition = new Vector3(position.x, position.y, position.z);
                        Vector3 newScale = new Vector3(scale.x, scale.y, scale.z);
                        Matrix4x4 newMatrix = Matrix4x4.TRS(newPosition, rotation, newScale);

                        for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1; l++)
                        {
                            Material tempMaterial = new Material(vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l]);
                            tempMaterial.shader = overrideShader;
                            tempMaterial.SetPass(0);
                            Graphics.DrawMeshNow(vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMesh, newMatrix);
                            DestroyImmediate(tempMaterial);
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

