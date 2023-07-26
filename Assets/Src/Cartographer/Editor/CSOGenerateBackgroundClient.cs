using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using VacuumShaders.TerrainToMesh;
using System.Text;
using SharedCode.Aspects.Cartographer;
using Assets.Src.Shared;
using UnityEngine.ProBuilder;
using Assets.TerrainBaker;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOGenerateBackgroundClient : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title
            {
                get { return "Generate Client Background"; }
            }

            public string RunQuestion
            {
                get { return "Are you sure you want to generate client background?"; }
            }

            public string WelcomeMessage
            {
                get { return "Generate client background"; }
            }

            public string OnScenePrefix
            {
                get { return "Generate client background"; }
            }
        }

        public static MessagesClass Messages = new MessagesClass();

        private List<GameObject> gameObjectFoldersToFinish = new List<GameObject>();
        private Vector3Int checkedCoordinates = new Vector3Int();
        private Scene craterScene = new Scene();
        private bool craterSceneWasNotLoaded = false;

        // Create prefab with texture and mesh by unity terrain -----------------------------------
        private static void ExtractBasemapInEditor(
            Terrain terrain,
            Texture2D[] alphamapTextures,
            out Texture2D diffuseMap,
            out Texture2D normalMap,
            int textureSize,
            Vector4 splitOffsetScale,
            bool extractNormalMap)
        {
            diffuseMap = null;
            normalMap = null;
            Shader shader = Shader.Find("Hidden/VacuumShaders/Terrain To Mesh/Basemap"); // //TODO cartographer make const
            if (shader == null)
            {
                //TODO cartographer report ERROR
                //ReportWarning("'Hidden/VacuumShaders/Terrain To Mesh/Basemap' shader not found\n");
                return;
            }

            var material = new Material(shader);

            Texture2D[] diffuseTextures;
            Texture2D[] normalTextures;
            Vector2[] uvScale;
            Vector2[] uvOffset;
            float[] metalic;
            float[] smoothness;
            int num = TerrainToMeshConverter.ExtractTexturesInfo(terrain, out diffuseTextures, out normalTextures, out uvScale, out uvOffset, out metalic, out smoothness);
            if (num == 0)
            {
                //TODO cartographer report ERROR
                //ReportWarning("Terrain has no enough data for Basemap generating\n");
                return;
            }

            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false, PlayerSettings.colorSpace == ColorSpace.Linear);
            texture2D.SetPixels(new Color[] {Color.clear, Color.clear, Color.clear, Color.clear});
            texture2D.Apply();
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = null;
            RenderTexture renderTexture = TerrainToMeshConverter.CreateTemporaryRenderTexture(textureSize, textureSize, PlayerSettings.colorSpace == ColorSpace.Linear);
            renderTexture.DiscardContents();
            RenderTexture renderTexture2 = TerrainToMeshConverter.CreateTemporaryRenderTexture(textureSize, textureSize, PlayerSettings.colorSpace == ColorSpace.Linear);
            renderTexture2.DiscardContents();
            material.SetVector("_V_T2M_Splat_uvOffset", splitOffsetScale);
            if (diffuseTextures != null)
            {
                TerrainToMeshConverter.Blit(texture2D, renderTexture, false);
                for (int i = 0; i < num; i++)
                {
                    if (i % 4 == 0)
                    {
                        material.SetTexture("_V_T2M_Control", alphamapTextures[i / 4]);
                    }

                    material.SetInt("_V_T2M_ChannelIndex", i % 4);
                    if (diffuseTextures[i] == null)
                    {
                        //ReportWarning($"Terrain {_terrain.name} is missing diffuse texture {i}"); //TODO cartographer report ERROR
                        material.SetTexture("_V_T2M_Splat_D", null);
                    }
                    else
                    {
                        material.SetTexture("_V_T2M_Splat_D", diffuseTextures[i]);
                        material.SetVector("_V_T2M_Splat_uvScale", new Vector4(uvScale[i].x, uvScale[i].y, uvOffset[i].x, uvOffset[i].y));
                    }

                    TerrainToMeshConverter.Blit(renderTexture, renderTexture2, material, PlayerSettings.colorSpace == ColorSpace.Linear, 0);
                    TerrainToMeshConverter.Blit(renderTexture2, renderTexture, PlayerSettings.colorSpace == ColorSpace.Linear);
                }

                RenderTexture.active = renderTexture;
                diffuseMap = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true, PlayerSettings.colorSpace == ColorSpace.Linear);
                diffuseMap.ReadPixels(new Rect(0f, 0f, (float) textureSize, (float) textureSize), 0, 0);
                diffuseMap.Apply();
            }

            renderTexture.DiscardContents();
            renderTexture2.DiscardContents();

            if (extractNormalMap && (normalTextures != null))
            {
                TerrainToMeshConverter.Blit(texture2D, renderTexture, false);
                for (int j = 0; j < num; j++)
                {
                    if (j % 4 == 0)
                    {
                        material.SetTexture("_V_T2M_Control", alphamapTextures[j / 4]);
                    }

                    material.SetInt("_V_T2M_ChannelIndex", j % 4);
                    if (normalTextures[j] == null)
                    {
                        //ReportWarning($"Terrain {_terrain.name} is missing normal texture {i}"); //TODO cartographer report ERROR
                        material.SetTexture("_V_T2M_Splat_N", null);
                        material.SetVector("_V_T2M_Splat_uvScale", Vector4.zero);
                    }
                    else
                    {
                        material.SetTexture("_V_T2M_Splat_N", normalTextures[j]);
                        material.SetVector("_V_T2M_Splat_uvScale", new Vector4(uvScale[j].x, uvScale[j].y, uvOffset[j].x, uvOffset[j].y));
                    }

                    TerrainToMeshConverter.Blit(renderTexture, renderTexture2, material, PlayerSettings.colorSpace == ColorSpace.Linear, 1);
                    TerrainToMeshConverter.Blit(renderTexture2, renderTexture, PlayerSettings.colorSpace == ColorSpace.Linear);
                }

                TerrainToMeshConverter.Blit(renderTexture, renderTexture2, material, false, 2);
                RenderTexture.active = renderTexture2;

                normalMap = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, true, false);
                normalMap.ReadPixels(new Rect(0f, 0f, (float) textureSize, (float) textureSize), 0, 0);
                normalMap.Apply();
                renderTexture.DiscardContents();
                renderTexture2.DiscardContents();
            }

            RenderTexture.active = active;
            UnityEngine.Object.DestroyImmediate(material);
            UnityEngine.Object.DestroyImmediate(texture2D);
            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.ReleaseTemporary(renderTexture2);
        }

        private static readonly List<char> unsupportedSymbols = new List<char>()
            {'`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '-', '=', '+', '\\', '|', ';', ':', '\'', '\"', ',', '<', '.', '>', '?', '/'};

        private static string RemoveUnsupportedSymbols(string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            foreach (var symbol in buffer)
            {
                if (unsupportedSymbols.Contains(symbol))
                {
                    continue;
                }

                stringBuilder.Append(symbol);
            }

            return stringBuilder.ToString();
        }

        private static Bounds GetGameObjectBounds(GameObject gameObject)
        {
            var boundsCollector = new CartographerBoundsCollectior();
            if (gameObject != null)
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                for (var index = 0; index < renderers.Length; ++index)
                {
                    boundsCollector.Collect(renderers[index].bounds);
                }
            }

            return boundsCollector.Bounds;
        }

        public static bool IsBackgroundGameObject(GameObject gameObject, CartographerParamsDef.BackgroundClientCreation craterCreationParams)
        {
            var bigGameObjectMarker = gameObject.GetComponent<BigGameObjectMarkerBehaviour>();
            if (bigGameObjectMarker != null)
            {
                return bigGameObjectMarker.AddToClientBackground;
            }

            var aProBuilderMeshComponent = gameObject.GetComponent<ProBuilderMesh>();
            if (aProBuilderMeshComponent != null)
            {
                return false;
            }

            var aLODGroupComponent = gameObject.GetComponent<LODGroup>();
            if (aLODGroupComponent == null)
            {
                return false;
            }

            var bounds = GetGameObjectBounds(gameObject);
            if (((bounds.extents.x * 2.0f) >= craterCreationParams.MinimalBounds.x) ||
                ((bounds.extents.y * 2.0f) >= craterCreationParams.MinimalBounds.y) ||
                ((bounds.extents.z * 2.0f) >= craterCreationParams.MinimalBounds.z))
            {
                return true;
            }

            return false;
        }

        private static bool ValidateGameObject(Scene scene, GameObject gameObject)
        {
            var keep = true;
            if (keep)
            {
                var components = gameObject.GetComponentsInChildren<Component>(true);
                if ((components != null) && (components.Length > 0))
                {
                    var attempts = 0;
                    var somethingDestroyed = true;
                    while (somethingDestroyed && (attempts < 6))
                    {
                        ++attempts;
                        somethingDestroyed = false;
                        foreach (var component in components)
                        {
                            if (component != null)
                            {
                                if (component is Transform)
                                {
                                    continue;
                                }

                                if (!component.gameObject.activeInHierarchy)
                                {
                                    if (component.gameObject.CanDestroy(component.GetType()))
                                    {
                                        UnityEngine.Object.DestroyImmediate(component);
                                    }

                                    somethingDestroyed = true;
                                }
                                else
                                {
                                    if ((component is LODGroup) || (component is MeshFilter) || (component is MeshRenderer))
                                    {
                                        continue;
                                    }

                                    if (component.gameObject.CanDestroy(component.GetType()))
                                    {
                                        UnityEngine.Object.DestroyImmediate(component);
                                    }

                                    somethingDestroyed = true;
                                }
                            }
                        }
                    }

                    if (somethingDestroyed)
                    {
                        foreach (var component in components)
                        {
                            if (component != null)
                            {
                                if ((component is Transform) || (component is LODGroup) || (component is MeshFilter) || (component is MeshRenderer))
                                {
                                    continue;
                                }

                                CartographerCommon.ReportError($"ValidateGameObject, can't destroy component: {component.GetType().ToString()}, game object: {gameObject.name}, scene: {scene.name}");
                            }
                        }
                    }
                }
            }

            if (keep)
            {
                if (CartographerCommon.CleanObjectAndCheckIsEmpty(gameObject))
                {
                    keep = false;
                }
            }

            return keep;
        }

        //TODO make it private after remove obsolete functions
        public static void CreateTerrainMeshPrefab(
            Scene craterScene,
            GameObject gameObjectFolder,
            string gameObjectName,
            Terrain terrain,
            string prefabName,
            string prefabPath,
            string meshPath,
            string materialPath,
            string diffuseTexturePath,
            string normalTexturePath,
            int textureSize,
            int vertexCount)
        {
            var useNormalTexture = !string.IsNullOrEmpty(normalTexturePath);
            var alphamapTextures = TerrainToMeshConverter.ExtractSplatmaps(terrain);
            if ((alphamapTextures == null) || (alphamapTextures.Length == 0))
            {
                CartographerCommon.ReportError($"CreateCraterBackground, terrain have no any alphamap trextures: {terrain.name}");
            }

            Texture2D diffuseTexture;
            Texture2D normalTexture;
            ExtractBasemapInEditor(terrain, alphamapTextures, out diffuseTexture, out normalTexture, textureSize, new Vector4(1f, 1f, 0f, 0f), useNormalTexture);
            if (diffuseTexture != null)
            {
                var arrayPNG = ImageConversion.EncodeToPNG(diffuseTexture);
                if (arrayPNG != null)
                {
                    File.WriteAllBytes(diffuseTexturePath, arrayPNG);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(diffuseTexturePath) as TextureImporter;
                    if (textureImporter == null)
                    {
                        AssetDatabase.ImportAsset(diffuseTexturePath);
                        textureImporter = AssetImporter.GetAtPath(diffuseTexturePath) as TextureImporter;
                    }

                    if (textureImporter != null)
                    {
                        textureImporter.textureType = TextureImporterType.Default;
                        textureImporter.alphaIsTransparency = false;
                        textureImporter.alphaSource = TextureImporterAlphaSource.None;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.maxTextureSize = textureSize;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.SaveAndReimport();
                    }
                }

                UnityEngine.Object.DestroyImmediate(diffuseTexture);
                diffuseTexture = AssetDatabase.LoadAssetAtPath(diffuseTexturePath, typeof(Texture2D)) as Texture2D;
            }

            if (useNormalTexture && (normalTexture != null))
            {
                var arrayPNG = ImageConversion.EncodeToPNG(normalTexture);
                if (arrayPNG != null)
                {
                    File.WriteAllBytes(normalTexturePath, arrayPNG);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(normalTexturePath) as TextureImporter;
                    if (textureImporter == null)
                    {
                        AssetDatabase.ImportAsset(normalTexturePath);
                        textureImporter = AssetImporter.GetAtPath(normalTexturePath) as TextureImporter;
                    }

                    if (textureImporter != null)
                    {
                        textureImporter.textureType = TextureImporterType.Default;
                        textureImporter.alphaIsTransparency = false;
                        textureImporter.alphaSource = TextureImporterAlphaSource.None;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.maxTextureSize = textureSize;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.SaveAndReimport();
                    }
                }

                UnityEngine.Object.DestroyImmediate(normalTexture);
                normalTexture = AssetDatabase.LoadAssetAtPath(normalTexturePath, typeof(Texture2D)) as Texture2D;
            }

            var shader = Shader.Find("Standard");
            var material = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
                material = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
            }
            else
            {
                material.shader = shader;
            }

            if (material != null)
            {
                material.SetFloat("_Glossiness", 0.35f);
                material.mainTextureScale = Vector2.one;
                material.mainTextureOffset = Vector2.zero;
                material.mainTexture = diffuseTexture;
            }

            EditorUtility.SetDirty(material);

            var prefab = File.Exists(prefabPath) ? PrefabUtility.LoadPrefabContents(prefabPath) : null;
            var prefabInstance = prefab ?? new GameObject(prefabName);
            prefabInstance.transform.rotation = terrain.transform.rotation;
            prefabInstance.transform.localScale = terrain.transform.localScale;

            var terrainConvertInfo = new TerrainConvertInfo();
            terrainConvertInfo.chunkCountHorizontal = 1;
            terrainConvertInfo.chunkCountVertical = 1;
            terrainConvertInfo.vertexCountHorizontal = vertexCount;
            terrainConvertInfo.vertexCountVertical = vertexCount;
            terrainConvertInfo.generateSkirt = true;
            terrainConvertInfo.skirtGroundLevel = terrain.terrainData.bounds.min.y - (terrain.terrainData.size.x / vertexCount);

            string name = terrain.name;
            terrain.name = RemoveUnsupportedSymbols(terrain.name);
            Mesh[] array = null;
            array = TerrainToMeshConverter.Convert(terrain, terrainConvertInfo, false, null);
            terrain.name = name;

            if ((array == null || array.Length == 0) || (array.Length > 1))
            {
                //TODO cartographer, report error
                //TODO cartographer, array.Length == 1
                return;
            }

            array[0].hideFlags = 0;
            UnityEditor.MeshUtility.SetMeshCompression(array[0], ModelImporterMeshCompression.High);
            AssetDatabase.CreateAsset(array[0], meshPath);

            var meshFilter = CartographerCommon.GetOrAddComponent<MeshFilter>(prefabInstance);
            meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;
            var meshRenderer = CartographerCommon.GetOrAddComponent<MeshRenderer>(prefabInstance);
            meshRenderer.sharedMaterial = material;
            prefabInstance.transform.parent = null;
            prefabInstance.SetLayerRecursively(PhysicsLayers.Terrain);
            if (prefab == null)
            {
                prefabInstance.name = prefabName;
                prefab = PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                UnityEngine.Object.DestroyImmediate(prefabInstance);
            }
            else
            {
                prefab = PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }

            if (prefab == null)
            {
                //TODO cartographer, report error
                return;
            }

            prefabInstance = CartographerCommon.FindChild(gameObjectName, craterScene, gameObjectFolder, false);
            if (prefabInstance == null)
            {
                prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                prefabInstance.name = gameObjectName;
            }

            prefabInstance.transform.position = terrain.transform.position;
            prefabInstance.transform.parent = gameObjectFolder.transform;
            prefabInstance.SetLayerRecursively(PhysicsLayers.Terrain);

            EditorUtility.UnloadUnusedAssetsImmediate();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void StartCraterForGameObjectFolder(Scene craterScene, Vector3Int coordinates, GameObject gameObjectFolder, CartographerParamsDef.BackgroundClientCreation craterCreationParams)
        {
            if (CartographerCommon.InsideRect(coordinates, craterCreationParams.CleanupTerrainColliders))
            {
                var gameObjectToDestroy = CartographerCommon.FindChild(craterCreationParams.TerrainCollidersGameObjectName, craterScene, gameObjectFolder, false);
                if (gameObjectToDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
                }
            }

            if (CartographerCommon.InsideRect(coordinates, craterCreationParams.CleanupTerrainVisuals))
            {
                var gameObjectToDestroy = CartographerCommon.FindChild(craterCreationParams.TerrainVisualsGameObjectName, craterScene, gameObjectFolder, false);
                if (gameObjectToDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
                }
            }

            if (CartographerCommon.InsideRect(coordinates, craterCreationParams.CleanupStaticObjectVisuals))
            {
                var gameObjectToDestroy = CartographerCommon.FindChild(craterCreationParams.StaticObjectVisualsGameObjectName, craterScene, gameObjectFolder, false);
                if (gameObjectToDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
                }
            }
        }

        private static void StartCrater(Scene craterScene, CartographerParamsDef.BackgroundClientCreation craterCreationParams, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            if (openedScenesOnly)
            {
                return;
            }

            var gameObjectFolders = craterScene.GetRootGameObjects();
            foreach (var gameObjectFolder in gameObjectFolders)
            {
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(gameObjectFolder.name, out coordinates))
                {
                    StartCraterForGameObjectFolder(craterScene, coordinates, gameObjectFolder, craterCreationParams);
                }
            }
        }

        private static bool CollectMaterialsFromRenderers(Renderer[] renderers, string message, HashSet<Material> materials)
        {
            bool result = false;
            if ((renderers != null) && (renderers.Length > 0))
            {
                foreach (var renderer in renderers)
                {
                    var sharedMaterials = renderer.sharedMaterials;
                    if ((sharedMaterials != null) && (sharedMaterials.Length > 0))
                    {
                        foreach (var sharedMaterial in sharedMaterials)
                        {
                            if ((sharedMaterial != null) && (sharedMaterial.shader != null) && !string.IsNullOrEmpty(sharedMaterial.shader.name))
                            {
                                if (sharedMaterial.shader.name.Contains("Meshblending"))
                                {
                                    if (!materials.Contains(sharedMaterial))
                                    {
                                        materials.Add(sharedMaterial);
                                        result = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static void FinishCrater(
            Scene craterScene,
            CartographerParamsDef.BackgroundClientCreation craterCreationParams,
            bool openedScenesOnly,
            List<GameObject> gameObjectFoldersToFinish,
            ICartographerProgressCallback progressCallback)
        {
            var materialsToReplace = new HashSet<Material>();
            var gameObjectFolders = openedScenesOnly ? gameObjectFoldersToFinish.ToArray() : craterScene.GetRootGameObjects();
            foreach (var gameObjectFolder in gameObjectFolders)
            {
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(gameObjectFolder.name, out coordinates))
                {
                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateStaticObjectVisuals))
                    {
                        var staticObjectsFolder = CartographerCommon.FindChild(craterCreationParams.StaticObjectVisualsGameObjectName, craterScene, gameObjectFolder, false);
                        if (staticObjectsFolder != null)
                        {
                            var gameObjectsToSet = CartographerCommon.GetChildren(staticObjectsFolder);
                            if (gameObjectsToSet.Count != 0)
                            {
                                foreach (var gameObjectToSet in gameObjectsToSet)
                                {
                                    var renderers = gameObjectToSet.GetComponentsInChildren<Renderer>(true);
                                    CollectMaterialsFromRenderers(renderers, $"CreateCraterBackground, name: {gameObjectToSet.name}, scene: {gameObjectFolder.name}", materialsToReplace);
                                }
                            }
                        }
                    }
                }
            }

            if (materialsToReplace.Count > 0)
            {
                if (!Directory.Exists(craterCreationParams.StaticObjectsMaterialsFolder))
                {
                    Directory.CreateDirectory(craterCreationParams.StaticObjectsMaterialsFolder);
                }

                var materialsMap = new Dictionary<Material, Material>();
                var shader = Shader.Find("Alloy/Lightweight Core");
                foreach (var materialToReplace in materialsToReplace)
                {
                    progressCallback?.OnProgress(Messages.Title, $"Update material: {materialsMap.Count + 1}/{materialsToReplace.Count}, name: {materialToReplace.name}", (materialsMap.Count + 1) * 1.0f / materialsToReplace.Count);
                    string newMaterialPath = CartographerCommon.CombineAssetPath(craterCreationParams.StaticObjectsMaterialsFolder, materialToReplace.name + craterCreationParams.StaticObjectsMaterialPostfix, CartographerCommon.MatExtension);
                    var fixedMaterial = AssetDatabase.LoadAssetAtPath(newMaterialPath, typeof(Material)) as Material;
                    if (fixedMaterial == null)
                    {
                        fixedMaterial = new Material(materialToReplace) {shader = shader, enableInstancing = true};
                        AssetDatabase.CreateAsset(fixedMaterial, newMaterialPath);
                        fixedMaterial = AssetDatabase.LoadAssetAtPath(newMaterialPath, typeof(Material)) as Material;
                    }
                    else
                    {
                        fixedMaterial.CopyPropertiesFromMaterial(materialToReplace);
                        fixedMaterial.shader = shader;
                        fixedMaterial.enableInstancing = true;
                        EditorUtility.SetDirty(fixedMaterial);
                    }

                    materialsMap.Add(materialToReplace, fixedMaterial);
                }

                AssetDatabase.SaveAssets();
                var rootObjectIndex = 0;
                foreach (var gameObjectFolder in gameObjectFolders)
                {
                    ++rootObjectIndex;
                    progressCallback?.OnProgress(
                        Messages.Title,
                        $"Update materials in scene objects: {rootObjectIndex}/{gameObjectFolders.Length}, name: {gameObjectFolder.name}",
                        rootObjectIndex * 1.0f / gameObjectFolders.Length);
                    Vector3Int coordinates;
                    if (CartographerCommon.IsSceneForStreaming(gameObjectFolder.name, out coordinates))
                    {
                        if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateStaticObjectVisuals))
                        {
                            var staticObjectsFolder = CartographerCommon.FindChild(craterCreationParams.StaticObjectVisualsGameObjectName, craterScene, gameObjectFolder, false);
                            if (staticObjectsFolder != null)
                            {
                                var staticObjects = CartographerCommon.GetChildren(staticObjectsFolder);
                                if (staticObjects.Count != 0)
                                {
                                    foreach (var staticObject in staticObjects)
                                    {
                                        var somethingChanged = false;
                                        var renderers = staticObject.GetComponentsInChildren<Renderer>(true);
                                        foreach (var renderer in renderers)
                                        {
                                            var materialChanged = false;
                                            var newSharedMaterials = new Material[renderer.sharedMaterials.Length];
                                            var count = renderer.sharedMaterials.Length;
                                            for (var index = 0; index < count; ++index)
                                            {
                                                var material = renderer.sharedMaterials[index];
                                                newSharedMaterials[index] = material;
                                                if (material != null)
                                                {
                                                    Material newMaterial = null;
                                                    if (materialsMap.TryGetValue(material, out newMaterial))
                                                    {
                                                        newSharedMaterials[index] = newMaterial;
                                                        materialChanged = true;
                                                    }
                                                }
                                            }

                                            if (materialChanged)
                                            {
                                                somethingChanged = true;
                                                renderer.sharedMaterials = newSharedMaterials;
                                            }
                                        }

                                        if (somethingChanged)
                                        {
                                            EditorUtility.SetDirty(staticObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void GenerateCrater(
            Scene craterScene,
            Scene scene,
            Vector3Int coordinates,
            CartographerParamsDef.BackgroundClientCreation craterCreationParams,
            bool openedScenesOnly,
            List<GameObject> gameObjectFoldersToFinish,
            ICartographerProgressCallback progressCallback)
        {
            var somethingChanged = false;
            var gameObjectFolder = CartographerCommon.FindOrCreateChild(scene.name, craterScene, null, true);
            if (openedScenesOnly)
            {
                StartCraterForGameObjectFolder(craterScene, coordinates, gameObjectFolder, craterCreationParams);
                gameObjectFoldersToFinish.Add(gameObjectFolder);
            }

            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour != null)
                {
                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateTerrainColliders) ||
                        CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateTerrainVisuals))
                    {
                        if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 0)
                        {
                            foreach (var gameObjectTerrain in sceneLoaderBehaviour.GameObjectsTerrain)
                            {
                                var materialSupport = gameObjectTerrain.GetComponent<TerrainBakerMaterialSupport>();
                                if (materialSupport != null)
                                {
                                    gameObjectFolder.transform.position = materialSupport.transform.position;
                                    if (!CartographerCommon.IsCollidersIgnored(gameObjectTerrain) && CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateTerrainColliders))
                                    {
                                        var mesh = materialSupport.terrainMesh;
                                        if (mesh != null)
                                        {
                                            var terrainCollider = gameObjectTerrain.GetComponent<TerrainCollider>();
                                            var physicMaterial = terrainCollider.sharedMaterial;
                                            if (physicMaterial == null)
                                            {
                                                CartographerCommon.ReportError($"CreateCraterServer, registered terrain {gameObjectTerrain.name} don't contain physicMaterial, scene: {scene.name}");
                                            }

                                            var terrainGameObject = CartographerCommon.FindOrCreateChild(craterCreationParams.TerrainCollidersGameObjectName, craterScene, gameObjectFolder, false);
                                            terrainGameObject.transform.position = materialSupport.transform.position;
                                            terrainGameObject.transform.rotation = materialSupport.transform.rotation;
                                            terrainGameObject.transform.localScale = materialSupport.transform.localScale;
                                            terrainGameObject.SetLayerRecursively(PhysicsLayers.Terrain);
                                            terrainGameObject.SetActive(true);
                                            var terrainHolder = terrainGameObject.GetComponent<TerrainHolderBehaviour>();
                                            if (terrainHolder == null)
                                            {
                                                terrainHolder = terrainGameObject.AddComponent<TerrainHolderBehaviour>();
                                            }

                                            terrainHolder.Terrain = null;
                                            var meshCollider = terrainGameObject.GetComponent<MeshCollider>();
                                            if (meshCollider == null)
                                            {
                                                meshCollider = terrainGameObject.AddComponent<MeshCollider>();
                                            }

                                            meshCollider.convex = false;
                                            meshCollider.sharedMaterial = physicMaterial;
                                            meshCollider.sharedMesh = mesh;
                                            meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.WeldColocatedVertices;
                                        }
                                        else
                                        {
                                            CartographerCommon.ReportError($"CreateCraterBackground, registered terrain {gameObjectTerrain.name} don't contain mesh, scene: {scene.name}");
                                        }
                                    }

                                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateTerrainVisuals))
                                    {
                                        var terrain = gameObjectTerrain.GetComponent<Terrain>();
                                        if (terrain != null)
                                        {
                                            string terrainPrefabFolder = CartographerCommon.CombineAssetPath(craterCreationParams.TerrainPrefabsFolder, scene.name, string.Empty);
                                            if (!Directory.Exists(terrainPrefabFolder))
                                            {
                                                Directory.CreateDirectory(terrainPrefabFolder);
                                            }

                                            var gameObjectName = craterCreationParams.TerrainVisualsGameObjectName;
                                            var prefabName = CartographerCommon.CombineAssetPath(craterCreationParams.TerrainPrefabName, string.Empty, string.Empty);
                                            var prefabPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, craterCreationParams.TerrainPrefabName, CartographerCommon.PrefabExtension);
                                            var meshPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, craterCreationParams.TerrainMeshName, CartographerCommon.AssetExtension);
                                            var materialPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, craterCreationParams.TerrainMaterialName, CartographerCommon.MatExtension);
                                            var diffuseTexturePath = CartographerCommon.CombineAssetPath(
                                                terrainPrefabFolder,
                                                craterCreationParams.TerrainDiffuseTextureName,
                                                CartographerCommon.PngExtension);
                                            CreateTerrainMeshPrefab(
                                                craterScene,
                                                gameObjectFolder,
                                                gameObjectName,
                                                terrain,
                                                prefabName,
                                                prefabPath,
                                                meshPath,
                                                materialPath,
                                                diffuseTexturePath,
                                                string.Empty,
                                                craterCreationParams.TerrainTextureSize,
                                                craterCreationParams.TerrainVertexCount);
                                        }
                                        else
                                        {
                                            CartographerCommon.ReportError(
                                                $"CreateCraterBackground, SceneLoaderBehaviour registered terrain have no any actual terrain: {gameObjectTerrain.name}, scene: {scene.name}");
                                        }
                                    }
                                }
                                else
                                {
                                    CartographerCommon.ReportError(
                                        $"CreateCraterBackground, registered terrain {gameObjectTerrain.name} don't contain TerrainBakerMaterialSupport, scene: {scene.name}");
                                }

                                break;
                            }

                            if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 1)
                            {
                                CartographerCommon.ReportError(
                                    $"CreateCraterBackground, SceneLoaderBehaviour has more the one registered terrain: {sceneLoaderBehaviour.GameObjectsTerrain.Count}, scene: {scene.name}");
                            }
                        }
                        else
                        {
                            CartographerCommon.ReportError($"CreateCraterBackground, SceneLoaderBehaviour have no any terrain registered: {rootGameObject.name}, scene: {scene.name}");
                        }
                    }

                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateStaticObjectVisuals))
                    {
                        var staticObjectFolder = CartographerCommon.FindOrCreateChild(craterCreationParams.StaticObjectVisualsGameObjectName, craterScene, gameObjectFolder, true);
                        if (sceneLoaderBehaviour.GameObjectsStatic.Count > 0)
                        {
                            staticObjectFolder.transform.position = gameObjectFolder.transform.position;
                            foreach (var gameObjectStatic in sceneLoaderBehaviour.GameObjectsStatic)
                            {
                                if (gameObjectStatic.activeSelf && IsBackgroundGameObject(gameObjectStatic, craterCreationParams))
                                {
                                    if (gameObjectStatic.layer != PhysicsLayers.Terrain)
                                    {
                                        gameObjectStatic.SetLayerRecursively(PhysicsLayers.Terrain);
                                        somethingChanged = true;
                                    }

                                    var newGameObject = GameObject.Instantiate(gameObjectStatic) as GameObject;
                                    if (newGameObject != null)
                                    {
                                        if (ValidateGameObject(scene, newGameObject))
                                        {
                                            newGameObject.transform.position = gameObjectStatic.transform.position;
                                            newGameObject.transform.rotation = gameObjectStatic.transform.rotation;
                                            newGameObject.transform.localScale = gameObjectStatic.transform.localScale;
                                            newGameObject.transform.parent = staticObjectFolder.transform;
                                            newGameObject.name = gameObjectStatic.name;
                                            newGameObject.SetLayerRecursively(PhysicsLayers.Terrain);
                                            newGameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            UnityEngine.Object.DestroyImmediate(newGameObject);
                                        }
                                    }
                                    else
                                    {
                                        CartographerCommon.ReportError(
                                            $"CreateCraterBackground, SceneLoaderBehaviour cant't clone non prefab registered static object: {gameObjectStatic.name}, scene: {scene.name}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            UnityEngine.Object.DestroyImmediate(staticObjectFolder);
                        }
                    }
                }
                else
                {
                    CartographerCommon.ReportError($"CreateCraterBackground, Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
            }

            if (somethingChanged)
            {
                progressCallback?.OnProgress(Messages.Title, $"Saving scene: {scene.name}", 1.0f);
                EditorSceneManager.SaveScene(scene);
            }
        }

        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages
        {
            get { return Messages; }
        }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            if (((cartographerScene.TypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection) &&
                CartographerCommon.IsSceneForStreaming(cartographerScene.Name, out checkedCoordinates))
            {
                if (CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateTerrainColliders) ||
                    CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateTerrainVisuals) ||
                    CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundClientCreationParams.GenerateStaticObjectVisuals))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            progressCallback?.OnBegin(Messages.Title, "Load crater background scene");
            gameObjectFoldersToFinish.Clear();
            var craterScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundClientSceneName, string.Empty, CartographerCommon.UnityExtension);
            craterScene = CartographerCommon.FindLoadedScene(craterScenePath);
            craterSceneWasNotLoaded = !craterScene.isLoaded;
            if (craterSceneWasNotLoaded)
            {
                craterScene = EditorSceneManager.OpenScene(craterScenePath, OpenSceneMode.Additive);
            }

            StartCrater(craterScene, cartographerParams.BackgroundClientCreationParams, openedScenesOnly, progressCallback);
            return true;
        }

        public bool Operate(
            Scene scene,
            CartographerSceneType cartographerSceneTypeMask,
            CartographerParamsDef cartographerParams,
            SceneCollectionDef sceneCollection,
            bool openedScenesOnly,
            ICartographerProgressCallback progressCallback)
        {
            GenerateCrater(craterScene, scene, checkedCoordinates, cartographerParams.BackgroundClientCreationParams, openedScenesOnly, gameObjectFoldersToFinish, progressCallback);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            FinishCrater(craterScene, cartographerParams.BackgroundClientCreationParams, openedScenesOnly, gameObjectFoldersToFinish, progressCallback);
            gameObjectFoldersToFinish.Clear();
            progressCallback?.OnProgress(Messages.Title, $"Saving crater background scene: {craterScene.name}", 1.0f);
            EditorSceneManager.SaveScene(craterScene);
            if (craterSceneWasNotLoaded)
            {
                EditorSceneManager.CloseScene(craterScene, true);
            }
        }
    }
};