using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using AwesomeTechnologies.Utility.Extentions;

namespace AwesomeTechnologies.Utility
{
    [CustomEditor(typeof(VegetationColorMaskCreator))]
    public class VegetationColorMaskCreatorEditor : VegetationStudioBaseEditor
    {
        private RenderTexture _rt;
        private int _textureResolution;
        private string _path;
        private GameObject _cameraObject;
        private Camera _backgroundCamera;
        private VegetationSystem _vegetationSystem;

        public override void OnInspectorGUI()
        {
            HelpTopic = "vegetation-color-mask-creator";

            VegetationColorMaskCreator colorMaskCreator = (VegetationColorMaskCreator)target;
            VegetationSystem vegetationSystem = colorMaskCreator.gameObject.GetComponent<VegetationSystem>();

            if (vegetationSystem)
            {
                if (vegetationSystem.GetSleepMode() || !vegetationSystem.enabled)
                {
                    EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                    return;
                }
            }
            else
            {
                {
                    EditorGUILayout.HelpBox("Add this component to a GameObject with a VegetationSystem component.",
                        MessageType.Error);
                    return;
                }
            }          

            base.OnInspectorGUI();

            if (vegetationSystem)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Settings", LabelStyle);
                colorMaskCreator.VegetationColorMaskQuality = (VegetationColorMaskQuality)EditorGUILayout.EnumPopup("Mask resolution", colorMaskCreator.VegetationColorMaskQuality);
                EditorGUILayout.HelpBox("Pixel resolution of the mask background. Low = 1024x1024, Normal = 2048x2048, High = 4096x4096 and Ultra = 8192x8192", MessageType.Info);

                colorMaskCreator.InvisibleLayer = EditorGUILayout.IntSlider("Mask render layer", colorMaskCreator.InvisibleLayer, 0, 30);
                EditorGUILayout.HelpBox("Select a empty layer with no scene objects. This is used to render the color mask.", MessageType.Info);

                colorMaskCreator.VegetationScale = EditorGUILayout.Slider("Grass/Plant scale", colorMaskCreator.VegetationScale, 1f, 3f);
                EditorGUILayout.HelpBox("This will increase the scale of each individual grass/plant patch to compensate for grass plane orientation", MessageType.Info);
                GUILayout.EndVertical();

               // GUILayout.BeginVertical("box");
//                EditorGUILayout.LabelField("Background", LabelStyle);
//                colorMaskCreator.BackgroundSource = (VegetationColorMaskBackgroundSource)EditorGUILayout.EnumPopup("", colorMaskCreator.BackgroundSource);
//                if (colorMaskCreator.BackgroundSource == VegetationColorMaskBackgroundSource.Color)
//                {
//                    colorMaskCreator.BackgroundColor = EditorGUILayout.ColorField("Mask background color", colorMaskCreator.BackgroundColor);
//                    EditorGUILayout.HelpBox("This color will be used as the background when rendering the color mask.", MessageType.Info);
//                }

//                if (colorMaskCreator.BackgroundSource == VegetationColorMaskBackgroundSource.Image)
//                {
//                    colorMaskCreator.BackgroundTexture = (Texture2D)EditorGUILayout.ObjectField("Background texture", colorMaskCreator.BackgroundTexture, typeof(Texture2D), false);
//                    EditorGUILayout.HelpBox("Use this as background for the vegetation color mask", MessageType.Info);
//                }

//                if (colorMaskCreator.BackgroundSource == VegetationColorMaskBackgroundSource.MicrosplatTerrain)
//                {
//#if __MICROSPLAT__

//                    MicroSplatTerrain microSplatTerrain = vegetationSystem.currentTerrain.gameObject
//                        .GetComponent<MicroSplatTerrain>();
//                    if (microSplatTerrain)
//                    {
//                        EditorGUILayout.HelpBox("The background image will be generated automatic from microsplat terrain shader.", MessageType.Info);
//                    }
//                    else
//                    {
//                        EditorGUILayout.HelpBox("No microsplat terrain shader detected. Background color will be used.", MessageType.Warning);
//                    }
//#else
//                    EditorGUILayout.HelpBox("No microsplat terrain shader detected. Background color will be used.", MessageType.Warning);
//#endif 
//                }

                //colorMaskCreator.RenderWithoutLight = EditorGUILayout.Toggle("Render color only", colorMaskCreator.RenderWithoutLight);
                //EditorGUILayout.HelpBox("This sets the default background color of the mask including alpha", MessageType.Info);

               // GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Included vegetation", LabelStyle);
                colorMaskCreator.IncludeGrass = EditorGUILayout.Toggle("Include Grass", colorMaskCreator.IncludeGrass);
                colorMaskCreator.IncludePlants = EditorGUILayout.Toggle("Include Plants", colorMaskCreator.IncludePlants);
                colorMaskCreator.IncludeTrees = EditorGUILayout.Toggle("Include Trees", colorMaskCreator.IncludeTrees);
                colorMaskCreator.IncludeObjects = EditorGUILayout.Toggle("Include Objects", colorMaskCreator.IncludeObjects);
                colorMaskCreator.IncludeLargeObjects = EditorGUILayout.Toggle("Include Large Objects", colorMaskCreator.IncludeLargeObjects);
                GUILayout.EndVertical();

                if (GUILayout.Button("Generate vegetation color mask"))
                {
                    GenerateVegetationColorMask(vegetationSystem);
                }
            }            
        }

        bool RenderVegetationType(VegetationType vegetationType, VegetationColorMaskCreator colorMaskCreator)
        {
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    return colorMaskCreator.IncludeGrass;
                case VegetationType.Plant:
                    return colorMaskCreator.IncludePlants;
                case VegetationType.Tree:
                    return colorMaskCreator.IncludeTrees;
                case VegetationType.Objects:
                    return colorMaskCreator.IncludeObjects;
                case VegetationType.LargeObjects:
                    return colorMaskCreator.IncludeLargeObjects ;
            }
            return false;
        }

        void GenerateVegetationColorMask(VegetationSystem vegetationSystem)
        {
            _vegetationSystem = vegetationSystem;
            _path = EditorUtility.SaveFilePanelInProject("Save mask background", "", "png",
                "Please enter a file name to save the mask background to");

            if (_path.Length != 0)
            {
                VegetationColorMaskCreator colorMaskCreator = (VegetationColorMaskCreator) target;

                Terrain selectedTerrain = _vegetationSystem.currentTerrain;
                if (selectedTerrain == null) return;

                _cameraObject = new GameObject {name = "Mask Background camera"};

                _textureResolution = colorMaskCreator.GetVegetationColorMaskQualityPixelResolution(colorMaskCreator
                    .VegetationColorMaskQuality);

                //Shader diffuseShader = Shader.Find("AwesomeTechnologies/Vegetation/RenderVegetationColorMask");
                //Material alphaPostfilter = Profile.Load<Material>("TightenAlpha");

//                Texture2D backgroundTexture = CreateColorTexture(colorMaskCreator.BackgroundColor);

//                if (colorMaskCreator.BackgroundSource == VegetationColorMaskBackgroundSource.Image)
//                {
//                    backgroundTexture = colorMaskCreator.BackgroundTexture;
//                }

//                if (colorMaskCreator.BackgroundSource == VegetationColorMaskBackgroundSource.MicrosplatTerrain)
//                {
//#if __MICROSPLAT__
//                    MicroSplatTerrain microSplatTerrain = vegetationSystem.currentTerrain.gameObject
//                        .GetComponent<MicroSplatTerrain>();
//                    if (microSplatTerrain)
//                    {
//                        backgroundTexture = MicroSplatTerrainEditor.Bake(microSplatTerrain,MicroSplatTerrainEditor.BakingPasses.Albedo,textureResolution);
//                    }
//#endif
//                }

                //alphaPostfilter.SetTexture("_Background", backgroundTexture);

                _rt =
                    new RenderTexture(_textureResolution, _textureResolution, 24, RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear)
                    {
                        wrapMode = TextureWrapMode.Clamp,
                        filterMode = FilterMode.Trilinear,
                        autoGenerateMips = false
                    };

                _backgroundCamera = _cameraObject.AddComponent<Camera>();
                _backgroundCamera.targetTexture = _rt;

                _backgroundCamera.clearFlags = CameraClearFlags.Color;
                _backgroundCamera.backgroundColor = colorMaskCreator.BackgroundColor;
                _backgroundCamera.orthographic = true;
                _backgroundCamera.orthographicSize = selectedTerrain.terrainData.size.x / 2f;
                _backgroundCamera.farClipPlane = 20000;
                _backgroundCamera.cullingMask = 1 << colorMaskCreator.InvisibleLayer;

                _cameraObject.transform.position = selectedTerrain.transform.position +
                                                  new Vector3(selectedTerrain.terrainData.size.x / 2f, 1000,
                                                      selectedTerrain.terrainData.size.z / 2f);
                _cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);
            
                _backgroundCamera.Render();
                Graphics.SetRenderTarget(_rt);
                GL.Viewport(new Rect(0, 0, _rt.width, _rt.height));
                GL.Clear(true, true, new Color(0, 0, 0, 0), 1f);

                GL.PushMatrix();
                GL.LoadProjectionMatrix(_backgroundCamera.projectionMatrix);
                GL.modelview = _backgroundCamera.worldToCameraMatrix;
                GL.PushMatrix();

                RenderVegetation(vegetationSystem.VegetationCellList, vegetationSystem, colorMaskCreator,_backgroundCamera);

                GL.PopMatrix();
                GL.PopMatrix();
                PostProcessMask();
            }
        }

        void PostProcessMask()
        {
            RenderTexture.active = _rt;
            Texture2D newTexture = new Texture2D(_textureResolution, _textureResolution, TextureFormat.ARGB32, true);
            newTexture.ReadPixels(new Rect(0, 0, _textureResolution, _textureResolution), 0, 0);
            RenderTexture.active = null;
            newTexture.Apply();

            Texture2D paddedTexture = TextureExtention.CreatePaddedTexture(newTexture);
            if (paddedTexture == null)
            {
                paddedTexture = newTexture;
            }
            else
            {
                DestroyImmediate(newTexture);
            }

            _backgroundCamera.targetTexture = null;
            DestroyImmediate(_rt);
            //GC.Collect();

            EditorUtility.DisplayProgressBar("Create color mask", "Saving mask", 0.33f);
            SaveTexture(paddedTexture, _path);
            DestroyImmediate(_cameraObject);
            DestroyImmediate(paddedTexture);
            //GC.Collect();

            EditorUtility.DisplayProgressBar("Create color mask", "importing asset", 0.66f);
            string assetPath = _path;
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

        //Texture2D CreateColorTexture(Color color)
        //{
        //    Texture2D newTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
        //    newTexture.SetPixel(0, 0, color);
        //    newTexture.Apply();
        //    return newTexture;
        //}

        void DestroyBackgroundObject(GameObject go)
        {
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                Material material = meshRenderer.sharedMaterial;
                meshRenderer.sharedMaterial = null;
                DestroyImmediate(material);
            }
            DestroyImmediate(go);
        }

        GameObject CreateBackgroundObject(Vector3 position, float size, Texture2D backgroundTexture, int layer)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "BackgroundImage";
            go.transform.position = new Vector3(position.x,0,position.y);
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            go.transform.localScale = new Vector3(size, size, size);
            go.layer = layer;
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                Material material = new Material(Shader.Find("Standard"));
                material.SetTexture("_MainTex", backgroundTexture);
                meshRenderer.sharedMaterial = material;
            }

            return go;
        }      

        void RenderVegetation(List<VegetationCell> processCellList, VegetationSystem vegetationSystem, VegetationColorMaskCreator colorMaskCreator, Camera camera)
        {
            for (int i = 0; i <= vegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = vegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                //for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationMeshLod0.subMeshCount - 1; l++)
                //{
                //    vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l].SetFloat("_CullFarStart",50000);
                //    vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l].SetFloat("_CullFarDistance", 20);
                //}

                if (!RenderVegetationType(vegetationItemInfo.VegetationType,colorMaskCreator)) continue;

                for (int j = 0; j <= processCellList.Count - 1; j++)
                {
                    if (j % 100 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Render vegetation item: " + vegetationItemInfo.Name, "Render cell " + j + "/" + (processCellList.Count - 1), j / ((float)processCellList.Count - 1));
                    }                   

                    VegetationCell vegetationCell = processCellList[j];
                    List<Matrix4x4> instanceList =
                        vegetationCell.DirectSpawnVegetation(vegetationItemInfo.VegetationItemID, true);

                    for (int k = 0; k <= instanceList.Count - 1; k++)
                    {
                        Vector3 position = MatrixTools.ExtractTranslationFromMatrix(instanceList[k]);
                        Vector3 scale = MatrixTools.ExtractScaleFromMatrix(instanceList[k]);
                        Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(instanceList[k]);
                        Vector3 newPosition = new Vector3(position.x,0,position.z);
                        Vector3 newScale;
                        if (vegetationItemInfo.VegetationType == VegetationType.Grass ||
                            vegetationItemInfo.VegetationType == VegetationType.Plant)
                        {
                             newScale = new Vector3(scale.x * colorMaskCreator.VegetationScale, scale.y * colorMaskCreator.VegetationScale, scale.z * colorMaskCreator.VegetationScale);
                        }
                        else
                        {
                            newScale = scale;
                        }
                 
                        Matrix4x4 newMatrix = Matrix4x4.TRS(newPosition, rotation, newScale);
                        instanceList[k] = newMatrix;                       
                    }

                    for (int l = 0; l <= vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1; l++)
                    {

                            Material tempMaterial = new Material(vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l]);
                            tempMaterial.shader =  Shader.Find("AwesomeTechnologies/Vegetation/RenderVegetationColorMask");
                            tempMaterial.SetPass(0);
                            for (int k = 0; k <= instanceList.Count - 1; k++)
                            {
                                Graphics.DrawMeshNow(vegetationSystem.VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                    instanceList[k]);
                                //Graphics.DrawMesh(vegetationSystem.VegetationModelInfoList[i].VegetationMeshLod0, instanceList[k],
                                //    vegetationSystem.VegetationModelInfoList[i].VegetationMaterialsLOD0[l],
                                //    colorMaskCreator.InvisibleLayer, null, l);
                            }

                        DestroyImmediate(tempMaterial);                    
                    }
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

            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                Texture2D colorMaskTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(name);
                AssetUtility.SetTextureSGBA(colorMaskTexture, false);
            }
        }
    }
}
