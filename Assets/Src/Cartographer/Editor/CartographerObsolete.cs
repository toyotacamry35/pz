using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using SharedCode.Aspects.Cartographer;
using UnityEditor.SceneManagement;
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine.SceneManagement;
using System.Text;
using Assets.Src.Shared;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerObsolete
    {
        private static readonly string progressSaveScenesTitle = "Save Scenes";
        private static readonly string progressCheckScenesTitle = "Check Scenes";
        private static readonly string progressСreateSceneCollectionTitle = "Create Scene Collection";
        private static readonly string progressCreateVegetationTitle = "Create vegetation";
        private static readonly string progressSaveAllPrefabsTitle = "Save All Prefabs";

        // Helpers old ----------------------------------------------------------------------------
        private static void ActivateAllSceneObjects(GameObject gameObject, bool ignore, bool activate)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(activate);
                if ((ignore || CartographerCommon.IsGameObjectFolder(gameObject)) && (gameObject.transform.childCount > 0))
                {
                    var children = CartographerCommon.GetChildren(gameObject);
                    foreach (var child in children)
                    {
                        ActivateAllSceneObjects(child, false, activate);
                    }
                }
            }
        }

        private static void ActivateAllSceneObjects(Scene scene, bool activate)
        {
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                ActivateAllSceneObjects(rootGameObject, true, activate);
            }
        }

        private static void CheckScene(Scene scene)
        {
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour == null)
                {
                    CartographerCommon.ReportError($"Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
                else
                {
                    if (sceneLoaderBehaviour.GameObjectsTerrain.Count <= 0)
                    {
                        CartographerCommon.ReportError($"SceneLoaderBehaviour have no any terrain registered: {rootGameObject.name}, scene: {scene.name}");
                    }
                    else
                    {
                        foreach (var gameObjectTerrain in sceneLoaderBehaviour.GameObjectsTerrain)
                        {
                            var vegetationStudioManager = gameObjectTerrain.GetComponentInParent<VegetationStudioManager>();
                            if (vegetationStudioManager == null)
                            {
                                CartographerCommon.ReportError($"vegetationStudioManager is missing, scene: {scene.name}");
                            }
#if PERSISTENT_VEGETATION
                            var vegetationSystem = gameObjectTerrain.GetComponent<VegetationSystem>();
                            var persistentVegetationStorage = gameObjectTerrain.GetComponent<PersistentVegetationStorage>();
                            if ((vegetationSystem == null) || (persistentVegetationStorage == null))
                            {
                                CartographerCommon.ReportError($"Some terrain components are missing, system: {((vegetationSystem == null) ? "missing" : "ok")}, storage: {((persistentVegetationStorage == null) ? "missing" : "ok")}, scene: {scene.name}");
                            }
                            else
                            {
                                var package = persistentVegetationStorage.PersistentVegetationStoragePackage;
                                if (package != null)
                                {
                                    var instanceList = package.GetPersistentVegetationInstanceInfoList();
                                    var totalCount = 0;
                                    var bakedCount = 0;
                                    var paintCount = 0;
                                    var importCount = 0;
                                    for (var i = 0; i < instanceList.Count; i++)
                                    {
                                        totalCount += instanceList[i].Count;
                                        for (var j = 0; j < instanceList[i].SourceCountList.Count; j++)
                                        {
                                            switch (instanceList[i].SourceCountList[j].VegetationSourceID)
                                            {
                                                case (byte)0:
                                                    bakedCount += instanceList[i].SourceCountList[j].Count;
                                                    break;
                                                case (byte)1:
                                                    paintCount += instanceList[i].SourceCountList[j].Count;
                                                    break;
                                                case (byte)2:
                                                    importCount += instanceList[i].SourceCountList[j].Count;
                                                    break;
                                                case (byte)5:
                                                    paintCount += instanceList[i].SourceCountList[j].Count;
                                                    break;
                                            }
                                        }
                                    }
                                    CartographerCommon.ReportError($"Scene: {scene.name}, totalCount: {totalCount}, bakedCount: {bakedCount}, paintCount: {paintCount}, importCount:{importCount}");
                                }
                                else
                                {
                                    CartographerCommon.ReportError($"vegetation pagage is null, scene: {scene.name}");
                                }
                            }
#endif
                        }
                    }
                }
            }
        }

        private static void FindAllMeshColliders()
        {
            EditorUtility.DisplayProgressBar("Collect All Mesh Colliders", "Starting...", 0.0f);

            var lines = new List<string>();
            var models = new List<string>();
            var badCols = new List<string>();

            var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
            var modelGUIDs = AssetDatabase.FindAssets("t:Model");
            float prefabsCount = prefabGUIDs.Length;
            float modelsCount = modelGUIDs.Length;
            float totalCount = prefabsCount + modelsCount;
            float prefabsLoaded = 0.0f;
            float modelsLoaded = 0.0f;
            //HashSet<string>
            lines.Add("=== Prefabs! ================================================================================================");
            foreach (var prerfabGIUD in prefabGUIDs)
            {
                prefabsLoaded += 1.0f;
                var prefabPath = AssetDatabase.GUIDToAssetPath(prerfabGIUD);
                EditorUtility.DisplayProgressBar("Coolect All Mesh Colliders", $"Loading prefab: {(int)(prefabsLoaded)}/{(int)(prefabsCount)} ...", (prefabsLoaded + modelsLoaded) / totalCount);
                var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
                if (prefab != null)
                {
                    var meshColliders = prefab.GetComponentsInChildren<MeshCollider>(true);
                    if (meshColliders != null)
                    {
                        foreach (var meshCollider in meshColliders)
                        {
                            var sharedMesh = meshCollider.sharedMesh;
                            var meshAssetNamePath = AssetDatabase.GetAssetPath(sharedMesh);

                            if (sharedMesh != null)
                            {
                                if (meshAssetNamePath.ToLower().Contains(".fbx"))
                                {
                                    models.Add(meshAssetNamePath);
                                }
                                if (sharedMesh.name.Contains("LOD"))
                                {
                                    lines.Add($"{sharedMesh.triangles.Length}\t{sharedMesh.name}\t'true'\t{meshAssetNamePath}\t{prefabPath}");
                                    badCols.Add($"{sharedMesh.name}\t{meshAssetNamePath}\t{prefabPath}");
                                }
                                else
                                {
                                    lines.Add($"{sharedMesh.triangles.Length}\t{(string.IsNullOrEmpty(sharedMesh.name) ? "<Empty>" : sharedMesh.name)}\t'false'\t{meshAssetNamePath}\t{prefabPath}");

                                }
                            }
                            else
                            {
                                lines.Add($"ERROR!: collider mesh is null:\t\t\t{prefabPath}");
                            }

                        }
                    }
                    PrefabUtility.UnloadPrefabContents(prefab);
                    prefab = null;
                }
                else
                {
                    lines.Add($"ERROR!:, can't load asset: {prefabPath}");
                }
                //if (prefabsLoaded > 10.0f)
                //{
                //    break;
                //}
            }

            lines.Add("=== Models! =================================================================================================");
            foreach (var model in models)
            {
                modelsLoaded += 1.0f;
                var modelPath = model + ".meta";
                EditorUtility.DisplayProgressBar("Coolect All Mesh Colliders", $"Loading model: {(int)(modelsLoaded)}/{(int)(modelsCount)} ...", (prefabsLoaded + modelsLoaded) / totalCount);
                var modelMeta = File.ReadAllLines(modelPath);
                var modelMetaNew = new List<string>();
                foreach (var metaInfo in modelMeta)
                {

                    if (metaInfo.Contains("isReadable: 0"))
                    {
                        var newMetaInfo = metaInfo.Replace("isReadable: 0", "isReadable: 1");
                        modelMetaNew.Add(newMetaInfo);
                        lines.Add($"{modelPath}\t{metaInfo}\t{newMetaInfo}");
                    }
                    else
                    {
                        modelMetaNew.Add(metaInfo);
                    }
                }
                //File.WriteAllLines(modelPath, modelMetaNew)
                //if (modelsLoaded > 100.0f)
                //{
                //    break;
                //}
            }

            var linesFilePath = "LODGroups.txt";
            var badColsFilePath = "BadCollisions.txt";
            File.WriteAllLines(linesFilePath, lines);
            File.WriteAllLines(badColsFilePath, badCols);

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Collect All Mesh Colliders", $"All Mesh colliders are collected in file {linesFilePath}, prefabs found: {prefabGUIDs.Length}, models found: {modelGUIDs.Length}", "OK");
        }

        private static void CheckTerrainInScene(Scene scene)
        {
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                List<GameObject> terrainGameObjects = null;
                terrainGameObjects = new List<GameObject>();
                if (rootGameObject.transform.childCount > 0)
                {
                    for (var index = 0; index < rootGameObject.transform.childCount; ++index)
                    {
                        var child = rootGameObject.transform.GetChild(index).gameObject;
                        if (child != null)
                        {
                            var terrain = child.GetComponent<Terrain>();
                            if (terrain != null)
                            {
                                terrainGameObjects.Add(child);
                            }
                        }
                    }
                }
                if ((terrainGameObjects != null) && (terrainGameObjects.Count > 0))
                {
                    foreach (var terrainGameObject in terrainGameObjects)
                    {
                        var terrain = terrainGameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            var stringBuilder = new StringBuilder();
                            var tc = terrainGameObject.GetComponent<TerrainCollider>();
                            if (tc != null)
                            {
                                stringBuilder.Append($"TerrainCollider: {tc.material}, ");
                            }
                            /*
                            var rt = terrainGameObject.GetComponent<ReliefTerrain>();
                            if (rt != null)
                            {
                                stringBuilder.Append($"ReliefTerrain, ");
                            }
							*/
                            var vs = terrainGameObject.GetComponent<VegetationSystem>();
                            if (vs != null)
                            {
                                stringBuilder.Append($"VegetationSystem, ");
                            }
                            var pvs = terrainGameObject.GetComponent<PersistentVegetationStorage>();
                            if (pvs != null)
                            {
                                stringBuilder.Append($"PersistentVegetationStorage, ");
                            }
                            stringBuilder.Append($"terrain layers: {terrain.terrainData.terrainLayers.Length}, detail: {terrain.terrainData.detailPrototypes.Length}, tree: {terrain.terrainData.treePrototypes.Length}, instance: {terrain.terrainData.treeInstanceCount}({terrain.terrainData.treeInstances.Length})");
                            CartographerCommon.ReportError($"Terrainfound:\t{scene.name}\t{terrainGameObject.name}\t{stringBuilder.ToString()}");
                        }
                    }
                }
            }
        }

        private static void ClearTerrainInScene(Scene scene, bool removeReliefTerrain, bool removeVegetation, bool removeTiles, bool removeDetailandTrees)
        {
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                List<GameObject> terrainGameObjects = null;
                {
                    terrainGameObjects = new List<GameObject>();
                    if (rootGameObject.transform.childCount > 0)
                    {
                        for (var index = 0; index < rootGameObject.transform.childCount; ++index)
                        {
                            var child = rootGameObject.transform.GetChild(index).gameObject;
                            if (child != null)
                            {
                                var terrain = child.GetComponent<Terrain>();
                                if (terrain != null)
                                {
                                    terrainGameObjects.Add(child);
                                }
                            }
                        }
                    }
                }
                if ((terrainGameObjects != null) && (terrainGameObjects.Count > 0))
                {
                    foreach (var terrainGameObject in terrainGameObjects)
                    {
                        var terrain = terrainGameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            /*
                            if (removeReliefTerrain)
                            {
                                var rt = terrainGameObject.GetComponent<ReliefTerrain>();
                                if (rt != null) { UnityEngine.Object.DestroyImmediate(rt); }
                                terrain.materialType = Terrain.MaterialType.BuiltInStandard;
                            }
							*/
                            if (removeVegetation)
                            {
                                var vs = terrainGameObject.GetComponent<VegetationSystem>();
                                if (vs != null) { UnityEngine.Object.DestroyImmediate(vs); }
                                var pvs = terrainGameObject.GetComponent<PersistentVegetationStorage>();
                                if (pvs != null) { UnityEngine.Object.DestroyImmediate(pvs); }
                            }
                            if (removeTiles)
                            {
                                terrain.terrainData.terrainLayers = new TerrainLayer[0];
                            }
                            if (removeDetailandTrees)
                            {
                                terrain.terrainData.detailPrototypes = new DetailPrototype[0];
                                terrain.terrainData.treeInstances = new TreeInstance[0];
                                terrain.terrainData.treePrototypes = new TreePrototype[0];
                                terrain.terrainData.SetDetailResolution(0, 8);
                            }
                            if (removeTiles || removeDetailandTrees)
                            {
                                terrain.terrainData.RefreshPrototypes();
                            }
                        }
                    }
                }
            }
        }

        private static void ShrinkAllTerrains(ICartographerProgressCallback progressCallback, int pow)
        {
            progressCallback?.OnBegin(progressCheckScenesTitle, "Begin TEST");
            var sourceTerrains = Terrain.activeTerrains;

            var commonSplitCount = 1 << pow;
            var splitSideXCount = commonSplitCount;
            var splitSideZCount = commonSplitCount;
            foreach (var sourceTerrain in sourceTerrains)
            {
                progressCallback?.OnProgress(progressCheckScenesTitle, $"TEST terrain: {sourceTerrain.name}", 1.0f);

                var sourceTerrainData = sourceTerrain.terrainData;
                var sourceTerrainCollider = sourceTerrain.gameObject.GetComponent<TerrainCollider>();

                var newTerrainData_BaseMapResolution = sourceTerrainData.baseMapResolution / commonSplitCount;
                var newTerrainData_HeightmapResolution = sourceTerrainData.heightmapResolution / commonSplitCount;
                var newTerrainData_AlphamapResolution = sourceTerrainData.alphamapResolution / commonSplitCount;
                var newTerrainData_HeightmapResolutionAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_SizeX = sourceTerrainData.size.x / splitSideXCount;
                var newTerrainData_SizeZ = sourceTerrainData.size.z / splitSideZCount;
                var newTerrainData_HeightsX = sourceTerrainData.heightmapResolution / splitSideXCount;
                var newTerrainData_HeightsZ = sourceTerrainData.heightmapResolution / splitSideZCount;
                var newTerrainData_HeightsXAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_HeightsZAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_AlphamapX = sourceTerrainData.alphamapWidth / splitSideXCount;
                var newTerrainData_AlphamapZ = sourceTerrainData.alphamapHeight / splitSideZCount;
                var newTerrainData_DetailX = sourceTerrainData.detailWidth / splitSideXCount;
                var newTerrainData_DetailZ = sourceTerrainData.detailHeight / splitSideZCount;

                var newGameObject = Terrain.CreateTerrainGameObject(null);
                newGameObject.transform.SetParent(sourceTerrain.transform, true);

                newGameObject.name = sourceTerrain.name + "_" + pow;
                newGameObject.SetLayerRecursively(PhysicsLayers.Terrain);

                var newTerrain = newGameObject.GetComponent<Terrain>();
                var newTerrainData = new TerrainData();

                var newTerrainDataAssetPath = CartographerCommon.CombineAssetPath("Assets/Test", newGameObject.name, CartographerCommon.AssetExtension);
                AssetDatabase.CreateAsset(newTerrainData, newTerrainDataAssetPath);

                // copy all terrain fields
                newTerrain.preserveTreePrototypeLayers = sourceTerrain.preserveTreePrototypeLayers; //bool, Allows you to specify how Unity chooses the for tree instances.
                newTerrain.realtimeLightmapIndex = sourceTerrain.realtimeLightmapIndex; //int, The index of the realtime lightmap applied to this terrain.
                newTerrain.lightmapScaleOffset = sourceTerrain.lightmapScaleOffset; //Vector4, The UV scale & offset used for a baked lightmap.
                newTerrain.realtimeLightmapScaleOffset = sourceTerrain.realtimeLightmapScaleOffset; //Vector4, The UV scale & offset used for a realtime lightmap.
                newTerrain.freeUnusedRenderingResources = sourceTerrain.freeUnusedRenderingResources; //bool, Whether some per-camera rendering resources for the terrain should be freed after
                newTerrain.shadowCastingMode = sourceTerrain.shadowCastingMode; //*bool, Should terrain cast shadows?.
                newTerrain.reflectionProbeUsage = sourceTerrain.reflectionProbeUsage; //enum ReflectionProbeUsage, How reflection probes are used for terrain. See Rendering.ReflectionProbeUsage.
                newTerrain.materialType = sourceTerrain.materialType; //enum MaterialType, The type of the material used to render the terrain. Could be one of the built-in types or custom. See Terrain.MaterialType.
                                                                      //we make copy of reliefTerrain
                                                                      //newTerrain.materialTemplate = sourceTerrain.materialTemplate; //Material, The custom material used to render the terrain.
                newTerrain.legacySpecular = sourceTerrain.legacySpecular; //Color, The specular color of the terrain.
                newTerrain.drawHeightmap = sourceTerrain.drawHeightmap; //bool, Specify if terrain heightmap should be drawn.
                newTerrain.drawTreesAndFoliage = sourceTerrain.drawTreesAndFoliage; //bool, Specify if terrain trees and details should be drawn.
                newTerrain.patchBoundsMultiplier = sourceTerrain.patchBoundsMultiplier; //Vector3, Set the terrain bounding box scale.
                newTerrain.treeLODBiasMultiplier = sourceTerrain.treeLODBiasMultiplier; //float, The multiplier to the current LOD bias used for rendering LOD trees (i.e. SpeedTree trees).
                newTerrain.collectDetailPatches = sourceTerrain.collectDetailPatches; //bool, Collect detail patches from memory.
                newTerrain.editorRenderFlags = sourceTerrain.editorRenderFlags; //enum TerrainRenderFlags, Controls what part of the terrain should be rendered.
                newTerrain.bakeLightProbesForTrees = sourceTerrain.bakeLightProbesForTrees; //bool, Specifies if an array of internal light probes should be baked for terrain trees. Available only in editor.
                newTerrain.lightmapIndex = sourceTerrain.lightmapIndex; //int, The index of the baked lightmap applied to this terrain.
                newTerrain.legacyShininess = sourceTerrain.legacyShininess; //float, The shininess value of the terrain.
                                                                            //newTerrain.splatmapDistance = sourceTerrain.splatmapDistance; //float, [Obsolete("splatmapDistance is deprecated, please use basemapDistance instead. (UnityUpgradable) -> basemapDistance", true)]
                newTerrain.heightmapMaximumLOD = sourceTerrain.heightmapMaximumLOD; //*int, Lets you essentially lower the heightmap resolution used for rendering.
                newTerrain.heightmapPixelError = sourceTerrain.heightmapPixelError; //*float, An approximation of how many pixels the terrain will pop in the worst case when switching lod.
                newTerrain.detailObjectDensity = sourceTerrain.detailObjectDensity; //*float, Density of detail objects.
                newTerrain.detailObjectDistance = sourceTerrain.detailObjectDistance; //*float, Detail objects will be displayed up to this distance.
                newTerrain.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount; //*int, Maximum number of trees rendered at full LOD.
                newTerrain.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength; //*float, Total distance delta that trees will use to transition from billboard orientation to mesh orientation.
                newTerrain.treeBillboardDistance = sourceTerrain.treeBillboardDistance; //*float, Distance from the camera where trees will be rendered as billboards only.
                newTerrain.treeDistance = sourceTerrain.treeDistance; //*float, The maximum distance at which trees are rendered.
                newTerrain.terrainData = newTerrainData; //TerrainData, The Terrain Data that stores heightmaps, terrain textures, detail meshes and trees.
                newTerrain.basemapDistance = sourceTerrain.basemapDistance;  //*float, Heightmap patches beyond basemap distance will use a precomputed low res basemap.

                //copy all terrain data fields
                newTerrainData.thickness = sourceTerrainData.thickness;//float, The thickness of the terrain used for collision detection.
                newTerrainData.wavingGrassStrength = sourceTerrainData.wavingGrassStrength; //float, Strength of the waving grass in the terrain.
                newTerrainData.wavingGrassAmount = sourceTerrainData.wavingGrassAmount;//float, Amount of waving grass in the terrain. 
                newTerrainData.wavingGrassSpeed = sourceTerrainData.wavingGrassSpeed;//float, Speed of the waving grass.
                newTerrainData.wavingGrassTint = sourceTerrainData.wavingGrassTint; //Color, Color of the waving grass that the terrain has.

                //no detail and trees
                //newTerrainData.detailPrototypes = sourceTerrainData.detailPrototypes; //*DetailPrototype[], Contains the detail texture/meshes that the terrain has. 
                //newTerrainData.treeInstances = sourceTerrainData.treeInstances; //TreeInstance[], Contains the current trees placed in the terrain.
                //newTerrainData.treePrototypes = sourceTerrainData.treePrototypes; //TreePrototype[], The list of tree prototypes this are the ones available in the inspector.
                newTerrainData.terrainLayers = sourceTerrainData.terrainLayers; //*TerrainLayer[], Splat texture used by the terrain.

                var sourcePosition = sourceTerrain.GetPosition();

                newGameObject.transform.position = sourcePosition;

                newTerrainData.baseMapResolution = newTerrainData_BaseMapResolution;
                newTerrainData.heightmapResolution = newTerrainData_HeightmapResolution + newTerrainData_HeightmapResolutionAddition;
                newTerrainData.size = new Vector3(newTerrainData_SizeX, sourceTerrainData.size.y, newTerrainData_SizeZ);
                var heights = sourceTerrainData.GetHeights(0, 0, sourceTerrainData.heightmapResolution, sourceTerrainData.heightmapResolution);
                heights = CartographerCommon.Resample(heights, pow);
                newTerrainData.SetHeights(0, 0, heights);

                newTerrainData.alphamapResolution = newTerrainData_AlphamapResolution;
                var alphamaps = sourceTerrainData.GetAlphamaps(0, 0, sourceTerrainData.alphamapWidth, sourceTerrainData.alphamapHeight);
                alphamaps = CartographerCommon.Resample(alphamaps, pow);
                newTerrainData.SetAlphamaps(0, 0, alphamaps);

                newTerrainData.SetDetailResolution(0, 8);

                if (sourceTerrainCollider != null)
                {
                    var newTerrainCollider = newTerrain.gameObject.GetComponent<TerrainCollider>();
                    if (newTerrainCollider != null)
                    {
                        UnityEngine.Object.DestroyImmediate(newTerrainCollider, true);
                    }
                    UnityEditorInternal.ComponentUtility.CopyComponent(sourceTerrainCollider);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
                    newTerrainCollider = newTerrain.gameObject.GetComponent<TerrainCollider>();
                    if (newTerrainCollider != null)
                    {
                        newTerrainCollider.terrainData = newTerrainData;
                    }
                }
                /*
                if (sourceReliefTerrain != null)
                {
                    var newReliefTerrain = newTerrain.gameObject.GetComponent<ReliefTerrain>();
                    if (newReliefTerrain != null)
                    {
                        UnityEngine.Object.DestroyImmediate(newReliefTerrain, true);
                    }
                    UnityEditorInternal.ComponentUtility.CopyComponent(sourceReliefTerrain);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
                    newReliefTerrain = newTerrain.gameObject.GetComponent<ReliefTerrain>();
                    if ((sourceTerrain.materialTemplate != null) && (newTerrain.materialTemplate != null))
                    {
                        newTerrain.materialTemplate.CopyPropertiesFromMaterial(sourceTerrain.materialTemplate);
                    }
                }
                */
                newGameObject.SetActive(false);
            }
            progressCallback?.OnEnd();
        }

        private static void CopyGameObjects(CartographerParamsDef cartographerParams, Scene scene, ICartographerProgressCallback progressCallback)
        {
            var sourceSceneCount = cartographerParams.SourceCollection.SceneNames.Count;
            if (sourceSceneCount > 0)
            {
                for (var sourceSceneIndex = 0; sourceSceneIndex < sourceSceneCount; ++sourceSceneIndex)
                {
                    var sourceSceneName = cartographerParams.SourceCollection.SceneNames[sourceSceneIndex];
                    progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Copying data from scene: {sourceSceneName}", (sourceSceneIndex + 1) * 1.0f / sourceSceneCount);
                    var sourceScenePath = CartographerCommon.CombineAssetPath(cartographerParams.SourceCollection.SceneFolder, sourceSceneName, CartographerCommon.UnityExtension);
                    var temporarySourceScenePath = CartographerCommon.CombineAssetPath(cartographerParams.SourceCollection.SceneFolder, cartographerParams.TemporarySceneName, CartographerCommon.UnityExtension);
                    AssetDatabase.CopyAsset(sourceScenePath, temporarySourceScenePath);
                    Scene temporarySourceScene = EditorSceneManager.OpenScene(temporarySourceScenePath, OpenSceneMode.Additive);
                    var sourceRootGameObjects = CartographerCommon.GetRootGameObjects(temporarySourceScene);
                    foreach (var sourceRootGameObject in sourceRootGameObjects)
                    {
                        CartographerCommon.MoveGameObject(sourceRootGameObject, scene, null);
                    }
                    EditorSceneManager.CloseScene(temporarySourceScene, true);
                    AssetDatabase.DeleteAsset(temporarySourceScenePath);
                }
            }
        }

        private static void SplitTerrains(List<KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>> vegetationTargets, CartographerParamsDef cartographerParams, Scene scene, ICartographerProgressCallback progressCallback)
        {
            var terrainGameObjectFolder = CartographerCommon.FindOrCreateChild(cartographerParams.RootTerrainGameObjectName, scene, null, true);
            var sourceTerrains = Terrain.activeTerrains;

            if ((sourceTerrains == null) || (sourceTerrains.Length == 0))
            {
                CartographerCommon.ReportError("Scene contains no terrains");
                return;
            }

            var somethingWrong = false;
            var totalSplitCount = 0;
            foreach (var sourceTerrain in sourceTerrains)
            {
                var sourceTerrainData = sourceTerrain.terrainData;
                if (sourceTerrainData.size.x != sourceTerrainData.size.z)
                {
                    CartographerCommon.ReportError($"Can't split non cubic terrain {sourceTerrain.name}, x: {sourceTerrainData.size.x}, z: {sourceTerrainData.size.z}");
                    somethingWrong = true;
                }
                else
                {
                }
                var splitSideXCount = (int)(sourceTerrainData.size.x / cartographerParams.SourceCollection.SceneSize.x);
                var splitSideZCount = (int)(sourceTerrainData.size.z / cartographerParams.SourceCollection.SceneSize.z);
                totalSplitCount += splitSideXCount * splitSideZCount;
            }
            if (somethingWrong)
            {
                return;
            }

            var terrainIndex = 0;
            var totalSplitIndex = 0;
            var terrainCount = sourceTerrains.Length;

            var newTerrainDataAssetFolder = CartographerCommon.CombineAssetPath(cartographerParams.ResultCollectionFolder, cartographerParams.TerrainCollectionSubFolder, string.Empty);
            if (!Directory.Exists(newTerrainDataAssetFolder))
            {
                Directory.CreateDirectory(newTerrainDataAssetFolder);
            }

            var newGameObjects = new List<GameObject>();

            foreach (var sourceTerrain in sourceTerrains)
            {
                EditorUtility.DisplayProgressBar($"Split terrain", $"Split terrain: {sourceTerrain.name} - {terrainIndex + 1}/{terrainCount}", (terrainIndex + 1.0f) / terrainCount);

                var sourceTerrainData = sourceTerrain.terrainData;
                var sourceTerrainCollider = sourceTerrain.gameObject.GetComponent<TerrainCollider>();
                //var sourceReliefTerrain = sourceTerrain.gameObject.GetComponent<ReliefTerrain>();

                var vegetationTerrainGameObjects = new List<GameObject>();
                var vegetationPaints = new List<PersistentVegetationInfo>();

                var sourceVegetationSystem = sourceTerrain.gameObject.GetComponent<VegetationSystem>();
                var sourceStorage = sourceTerrain.gameObject.GetComponent<PersistentVegetationStorage>();
#if PERSISTENT_VEGETATION
                if ((vegetationTargets != null) && (sourceVegetationSystem != null) && (sourceStorage != null) && (sourceStorage.PersistentVegetationStoragePackage != null))
                {
                    var terrainPosition = sourceTerrain.transform.position;
                    var sourcePackage = sourceStorage.PersistentVegetationStoragePackage;
                    var cellCount = sourcePackage.PersistentVegetationCellList.Count;
                    for (var cellIndex = 0; cellIndex < cellCount; ++cellIndex)
                    {
                        var cell = sourcePackage.PersistentVegetationCellList[cellIndex];
                        var persistentVegetationInfoCount = cell.PersistentVegetationInfoList.Count;
                        for (var persistentVegetationInfoIndex = 0; persistentVegetationInfoIndex < persistentVegetationInfoCount; ++persistentVegetationInfoIndex)
                        {
                            var persistentVegetationInfo = cell.PersistentVegetationInfoList[persistentVegetationInfoIndex];
                            bool somePaintedExists = false;
                            var sourceCount = persistentVegetationInfo.SourceCountList.Count;
                            for (var sourceIndex = 0; sourceIndex < sourceCount; ++sourceIndex)
                            {
                                var vegetationSourceID = persistentVegetationInfo.SourceCountList[sourceIndex].VegetationSourceID;
                                if ((vegetationSourceID == 1) || (vegetationSourceID == 5))
                                {
                                    somePaintedExists = true;
                                    break;
                                }
                            }
                            if (somePaintedExists)
                            {
                                PersistentVegetationInfo newPersistentVegetationInfo = null;
                                var resultCount = vegetationPaints.Count;
                                for (var resultIndex = 0; resultIndex < resultCount; ++resultIndex)
                                {
                                    if (vegetationPaints[resultIndex].VegetationItemID.Equals(persistentVegetationInfo.VegetationItemID))
                                    {
                                        newPersistentVegetationInfo = vegetationPaints[resultIndex];
                                        break;
                                    }
                                }
                                if (newPersistentVegetationInfo == null)
                                {
                                    newPersistentVegetationInfo = new PersistentVegetationInfo();
                                    vegetationPaints.Add(newPersistentVegetationInfo);
                                }
                                newPersistentVegetationInfo.VegetationItemID = persistentVegetationInfo.VegetationItemID;

                                var itemCount = persistentVegetationInfo.VegetationItemList.Count;
                                for (var itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                                {
                                    var vegetationSourceID = persistentVegetationInfo.VegetationItemList[itemIndex].VegetationSourceID;
                                    if ((vegetationSourceID == 1) || (vegetationSourceID == 5))
                                    {
                                        var newPersistentVegetationItem = persistentVegetationInfo.VegetationItemList[itemIndex];
                                        newPersistentVegetationItem.Position += terrainPosition;
                                        newPersistentVegetationInfo.VegetationItemList.Add(newPersistentVegetationItem);
                                    }
                                }
                            }
                        }
                    }
                }
#endif
                var splitSideXCount = (int)(sourceTerrainData.size.x / cartographerParams.SourceCollection.SceneSize.x);
                var splitSideZCount = (int)(sourceTerrainData.size.z / cartographerParams.SourceCollection.SceneSize.z);
                if ((splitSideXCount <= 1) && (splitSideZCount <= 1))
                {
                    continue;
                }

                var commonSplitCount = splitSideXCount;

                var newTerrainData_BaseMapResolution = sourceTerrainData.baseMapResolution / commonSplitCount;
                var newTerrainData_HeightmapResolution = sourceTerrainData.heightmapResolution / commonSplitCount;
                var newTerrainData_AlphamapResolution = sourceTerrainData.alphamapResolution / commonSplitCount;
                var newTerrainData_HeightmapResolutionAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_SizeX = sourceTerrainData.size.x / splitSideXCount;
                var newTerrainData_SizeZ = sourceTerrainData.size.z / splitSideZCount;
                var newTerrainData_HeightsX = sourceTerrainData.heightmapResolution / splitSideXCount;
                var newTerrainData_HeightsZ = sourceTerrainData.heightmapResolution / splitSideZCount;
                var newTerrainData_HeightsXAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_HeightsZAddition = (sourceTerrainData.heightmapResolution & 0x1);
                var newTerrainData_AlphamapX = sourceTerrainData.alphamapWidth / splitSideXCount;
                var newTerrainData_AlphamapZ = sourceTerrainData.alphamapHeight / splitSideZCount;
                var newTerrainData_DetailX = sourceTerrainData.detailWidth / splitSideXCount;
                var newTerrainData_DetailZ = sourceTerrainData.detailHeight / splitSideZCount;

                for (var terrainZ = 0; terrainZ < splitSideZCount; ++terrainZ)
                {
                    for (var terrainX = 0; terrainX < splitSideXCount; ++terrainX)
                    {
                        var splitIndex = terrainZ * splitSideXCount + terrainX;
                        progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Split terrain: {sourceTerrain.name} - {terrainIndex + 1}/{terrainCount}, index: [x: {terrainX}, z: {terrainZ}] - {splitIndex + 1}/{splitSideXCount * splitSideZCount}", (totalSplitIndex + 1.0f) / totalSplitCount);

                        var newGameObject = Terrain.CreateTerrainGameObject(null);
                        newGameObject.transform.SetParent(terrainGameObjectFolder.transform, true);

                        newGameObject.name = sourceTerrain.name + "_" + terrainX + "_" + terrainZ;
                        newGameObject.SetLayerRecursively(PhysicsLayers.Terrain);

                        var newTerrain = newGameObject.GetComponent<Terrain>();
                        var newTerrainData = new TerrainData();

                        var newTerrainDataAssetPath = CartographerCommon.CombineAssetPath(newTerrainDataAssetFolder, newGameObject.name, CartographerCommon.AssetExtension);
                        AssetDatabase.CreateAsset(newTerrainData, newTerrainDataAssetPath);

                        // copy all terrain fields
                        newTerrain.preserveTreePrototypeLayers = sourceTerrain.preserveTreePrototypeLayers; //bool, Allows you to specify how Unity chooses the for tree instances.
                        newTerrain.realtimeLightmapIndex = sourceTerrain.realtimeLightmapIndex; //int, The index of the realtime lightmap applied to this terrain.
                        newTerrain.lightmapScaleOffset = sourceTerrain.lightmapScaleOffset; //Vector4, The UV scale & offset used for a baked lightmap.
                        newTerrain.realtimeLightmapScaleOffset = sourceTerrain.realtimeLightmapScaleOffset; //Vector4, The UV scale & offset used for a realtime lightmap.
                        newTerrain.freeUnusedRenderingResources = sourceTerrain.freeUnusedRenderingResources; //bool, Whether some per-camera rendering resources for the terrain should be freed after
                        newTerrain.shadowCastingMode = sourceTerrain.shadowCastingMode; //*bool, Should terrain cast shadows?.
                        newTerrain.reflectionProbeUsage = sourceTerrain.reflectionProbeUsage; //enum ReflectionProbeUsage, How reflection probes are used for terrain. See Rendering.ReflectionProbeUsage.
                        newTerrain.materialType = sourceTerrain.materialType; //enum MaterialType, The type of the material used to render the terrain. Could be one of the built-in types or custom. See Terrain.MaterialType.
                        //we make copy of reliefTerrain
                        //newTerrain.materialTemplate = sourceTerrain.materialTemplate; //Material, The custom material used to render the terrain.
                        newTerrain.legacySpecular = sourceTerrain.legacySpecular; //Color, The specular color of the terrain.
                        newTerrain.drawHeightmap = sourceTerrain.drawHeightmap; //bool, Specify if terrain heightmap should be drawn.
                        newTerrain.drawTreesAndFoliage = sourceTerrain.drawTreesAndFoliage; //bool, Specify if terrain trees and details should be drawn.
                        newTerrain.patchBoundsMultiplier = sourceTerrain.patchBoundsMultiplier; //Vector3, Set the terrain bounding box scale.
                        newTerrain.treeLODBiasMultiplier = sourceTerrain.treeLODBiasMultiplier; //float, The multiplier to the current LOD bias used for rendering LOD trees (i.e. SpeedTree trees).
                        newTerrain.collectDetailPatches = sourceTerrain.collectDetailPatches; //bool, Collect detail patches from memory.
                        newTerrain.editorRenderFlags = sourceTerrain.editorRenderFlags; //enum TerrainRenderFlags, Controls what part of the terrain should be rendered.
                        newTerrain.bakeLightProbesForTrees = sourceTerrain.bakeLightProbesForTrees; //bool, Specifies if an array of internal light probes should be baked for terrain trees. Available only in editor.
                        newTerrain.lightmapIndex = sourceTerrain.lightmapIndex; //int, The index of the baked lightmap applied to this terrain.
                        newTerrain.legacyShininess = sourceTerrain.legacyShininess; //float, The shininess value of the terrain.
                        //newTerrain.splatmapDistance = sourceTerrain.splatmapDistance; //float, [Obsolete("splatmapDistance is deprecated, please use basemapDistance instead. (UnityUpgradable) -> basemapDistance", true)]
                        newTerrain.heightmapMaximumLOD = sourceTerrain.heightmapMaximumLOD; //*int, Lets you essentially lower the heightmap resolution used for rendering.
                        newTerrain.heightmapPixelError = sourceTerrain.heightmapPixelError; //*float, An approximation of how many pixels the terrain will pop in the worst case when switching lod.
                        newTerrain.detailObjectDensity = sourceTerrain.detailObjectDensity; //*float, Density of detail objects.
                        newTerrain.detailObjectDistance = sourceTerrain.detailObjectDistance; //*float, Detail objects will be displayed up to this distance.
                        newTerrain.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount; //*int, Maximum number of trees rendered at full LOD.
                        newTerrain.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength; //*float, Total distance delta that trees will use to transition from billboard orientation to mesh orientation.
                        newTerrain.treeBillboardDistance = sourceTerrain.treeBillboardDistance; //*float, Distance from the camera where trees will be rendered as billboards only.
                        newTerrain.treeDistance = sourceTerrain.treeDistance; //*float, The maximum distance at which trees are rendered.
                        newTerrain.terrainData = newTerrainData; //TerrainData, The Terrain Data that stores heightmaps, terrain textures, detail meshes and trees.
                        newTerrain.basemapDistance = sourceTerrain.basemapDistance;  //*float, Heightmap patches beyond basemap distance will use a precomputed low res basemap.

                        //copy all terrain data fields
                        newTerrainData.thickness = sourceTerrainData.thickness;//float, The thickness of the terrain used for collision detection.
                        newTerrainData.wavingGrassStrength = sourceTerrainData.wavingGrassStrength; //float, Strength of the waving grass in the terrain.
                        newTerrainData.wavingGrassAmount = sourceTerrainData.wavingGrassAmount;//float, Amount of waving grass in the terrain. 
                        newTerrainData.wavingGrassSpeed = sourceTerrainData.wavingGrassSpeed;//float, Speed of the waving grass.
                        newTerrainData.wavingGrassTint = sourceTerrainData.wavingGrassTint; //Color, Color of the waving grass that the terrain has.

                        //no detail and trees
                        //newTerrainData.detailPrototypes = sourceTerrainData.detailPrototypes; //*DetailPrototype[], Contains the detail texture/meshes that the terrain has. 
                        //newTerrainData.treeInstances = sourceTerrainData.treeInstances; //TreeInstance[], Contains the current trees placed in the terrain.
                        //newTerrainData.treePrototypes = sourceTerrainData.treePrototypes; //TreePrototype[], The list of tree prototypes this are the ones available in the inspector.
                        newTerrainData.terrainLayers = sourceTerrainData.terrainLayers; //*TerrainLayer[], Splat texture used by the terrain.

                        var sourcePosition = sourceTerrain.GetPosition();

                        newGameObject.transform.position = sourcePosition + new Vector3(terrainX * newTerrainData_SizeX, 0.0f, terrainZ * newTerrainData_SizeZ);

                        newTerrainData.baseMapResolution = newTerrainData_BaseMapResolution;
                        newTerrainData.heightmapResolution = newTerrainData_HeightmapResolution + newTerrainData_HeightmapResolutionAddition;
                        newTerrainData.size = new Vector3(newTerrainData_SizeX, sourceTerrainData.size.y, newTerrainData_SizeZ);
                        var heights = sourceTerrainData.GetHeights(terrainX * newTerrainData_HeightsX, terrainZ * newTerrainData_HeightsZ, newTerrainData_HeightsX + newTerrainData_HeightsXAddition, newTerrainData_HeightsZ + newTerrainData_HeightsZAddition);
                        newTerrainData.SetHeights(0, 0, heights);

                        newTerrainData.alphamapResolution = newTerrainData_AlphamapResolution;
                        var alphamaps = sourceTerrainData.GetAlphamaps(terrainX * newTerrainData_AlphamapX, terrainZ * newTerrainData_AlphamapZ, newTerrainData_AlphamapX, newTerrainData_AlphamapZ);
                        newTerrainData.SetAlphamaps(0, 0, alphamaps);

                        newTerrainData.SetDetailResolution(0, 8);

                        if (sourceTerrainCollider != null)
                        {
                            var newTerrainCollider = newTerrain.gameObject.GetComponent<TerrainCollider>();
                            if (newTerrainCollider != null)
                            {
                                UnityEngine.Object.DestroyImmediate(newTerrainCollider, true);
                            }
                            UnityEditorInternal.ComponentUtility.CopyComponent(sourceTerrainCollider);
                            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
                            newTerrainCollider = newTerrain.gameObject.GetComponent<TerrainCollider>();
                            if (newTerrainCollider != null)
                            {
                                newTerrainCollider.terrainData = newTerrainData;
                            }
                        }
                        /*
                        if (sourceReliefTerrain != null)
                        {
                            var newReliefTerrain = newTerrain.gameObject.GetComponent<ReliefTerrain>();
                            if (newReliefTerrain != null)
                            {
                                UnityEngine.Object.DestroyImmediate(newReliefTerrain, true);
                            }
                            UnityEditorInternal.ComponentUtility.CopyComponent(sourceReliefTerrain);
                            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
                            newReliefTerrain = newTerrain.gameObject.GetComponent<ReliefTerrain>();
                            if ((sourceTerrain.materialTemplate != null) && (newTerrain.materialTemplate != null))
                            {
                                newTerrain.materialTemplate.CopyPropertiesFromMaterial(sourceTerrain.materialTemplate);
                            }
                        }
						*/
                        newGameObject.SetActive(false);
                        newGameObjects.Add(newGameObject);
                        if (vegetationTargets != null)
                        {
                            vegetationTerrainGameObjects.Add(newGameObject);
                        }
                        ++totalSplitIndex;
                    }
                }
                if (vegetationTargets != null)
                {
                    vegetationTargets.Add(new KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>(vegetationTerrainGameObjects, vegetationPaints));
                }
                ++terrainIndex;
                AssetDatabase.SaveAssets();
                CartographerCommon.CleanupMemory();
            }
            // destroy all source terrain game objects
            var sourceGameObjects = new List<GameObject>();
            foreach (var sourceTerrain in sourceTerrains)
            {
                if (!sourceGameObjects.Contains(sourceTerrain.gameObject))
                {
                    sourceGameObjects.Add(sourceTerrain.gameObject);
                }
            }
            foreach (var sourceGameObject in sourceGameObjects)
            {
                UnityEngine.Object.DestroyImmediate(sourceGameObject);
            }
            //activate all new terrain game objects
            for (var index = 0; index < newGameObjects.Count; ++index)
            {
                newGameObjects[index].SetActive(true);
            }

            CartographerCommon.CleanupMemory();
        }

        private static void CreateVegetation(List<KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>> vegetationTargets, CartographerParamsDef cartographerParams, Scene scene, ICartographerProgressCallback progressCallback)
        {
            if ((vegetationTargets != null) && (cartographerParams != null))
            {
                var vegetationTargetCount = vegetationTargets.Count;

                var totalTerrainGameObjectCount = 0;
                for (var vegetationTargetIndex = 0; vegetationTargetIndex < vegetationTargetCount; ++vegetationTargetIndex)
                {
                    totalTerrainGameObjectCount += vegetationTargets[vegetationTargetIndex].Key.Count;
                }
                var totalTerrainGameObjectIndex = 0;
                for (var vegetationTargetIndex = 0; vegetationTargetIndex < vegetationTargetCount; ++vegetationTargetIndex)
                {
                    var vegetationTarget = vegetationTargets[vegetationTargetIndex];
                    var terrainGameObjectCount = vegetationTarget.Key.Count;
                    for (var terrainGameObjectIndex = 0; terrainGameObjectIndex < terrainGameObjectCount; ++terrainGameObjectIndex)
                    {
                        var terrainGameObject = vegetationTarget.Key[terrainGameObjectIndex];

                        ++totalTerrainGameObjectIndex;
                        progressCallback?.OnProgress(progressCreateVegetationTitle, $"Creating vegetation: {totalTerrainGameObjectIndex}/{totalTerrainGameObjectCount}, name: {terrainGameObject.name}", totalTerrainGameObjectIndex * 1.0f / totalTerrainGameObjectCount);

                        var parentTerrainGameObject = terrainGameObject.transform.parent?.gameObject ?? null;
                        if ((parentTerrainGameObject != null) && CartographerCommon.IsGameObjectFolder(parentTerrainGameObject) && parentTerrainGameObject.name.Equals(cartographerParams.RootTerrainGameObjectName))
                        {
                            var newVegetationStudioManager = parentTerrainGameObject.AddComponent<VegetationStudioManager>();
                            string vegetationPackagePath = CartographerCommon.CombineAssetPath(cartographerParams.VegetationAsset, string.Empty, CartographerCommon.AssetExtension);
                            var vegetationPackage = AssetDatabase.LoadAssetAtPath<VegetationPackage>(vegetationPackagePath);
                            if (vegetationPackage != null)
                            {
                                newVegetationStudioManager.LoadFromContextPreset(vegetationPackage);
                                newVegetationStudioManager.AddToTerrains();
                                newVegetationStudioManager.BakeAllGrass();
                                CartographerCommon.CleanupMemory();
                            }
                        }
                    }
                }
                CartographerCommon.CleanupMemory();
                progressCallback?.OnProgress(progressCreateVegetationTitle, $"Adding painted vegetation", 1.0f);
                for (var vegetationTargetIndex = 0; vegetationTargetIndex < vegetationTargetCount; ++vegetationTargetIndex)
                {
                    var vegetationTarget = vegetationTargets[vegetationTargetIndex];
                    var destinationCount = vegetationTarget.Key.Count;
                    for (var destinationIndex = 0; destinationIndex < destinationCount; ++destinationIndex)
                    {
                        var destination = vegetationTarget.Key[destinationIndex];
                        var destinationStorage = destination.GetComponent<PersistentVegetationStorage>();
                        if (destinationStorage != null)
                        {
                            var xmin = destination.transform.position.x;
                            var zmin = destination.transform.position.z;
                            var xmax = destination.transform.position.x + cartographerParams.SourceCollection.SceneSize.x;
                            var zmax = destination.transform.position.z + cartographerParams.SourceCollection.SceneSize.z;

                            var vegetationInfoCount = vegetationTarget.Value.Count;
                            for (var vegetationInfoIndex = 0; vegetationInfoIndex < vegetationInfoCount; ++vegetationInfoIndex)
                            {
                                var vegetationInfo = vegetationTarget.Value[vegetationInfoIndex];
                                var itemCount = vegetationInfo.VegetationItemList.Count;
                                for (var itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                                {
                                    var item = vegetationInfo.VegetationItemList[itemIndex];
                                    if ((item.Position.x >= xmin) && (item.Position.x < xmax) && (item.Position.z >= zmin) && (item.Position.z < zmax))
                                    {
#if PERSISTENT_VEGETATION
                                        destinationStorage.AddVegetationItemInstance(vegetationInfo.VegetationItemID, item.Position, item.Scale, item.Rotation, false, item.VegetationSourceID);
#endif
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        /*private static List<string> CreateRootSceneCollection(List<KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>> vegetationTargets, CartographerParamsDef cartographerParams, Scene scene, Scene backupScene, ICartographerProgressCallback progressCallback, bool createScenes)
        {
            SceneManager.SetActiveScene(scene);
            var result = new List<string>();
            var rootSceneGameObjects = new Dictionary<Vector3, GameObject>();
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            // copy root game objects
            var rootGameObjectCount = rootGameObjects.Count;
            var rootGameObjectIndex = 0;
            foreach (var rootGameObject in rootGameObjects)
            {
                ++rootGameObjectIndex;
                progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Sorting game objects: {rootGameObjectIndex}/{rootGameObjectCount}, name: {rootGameObject.name}", rootGameObjectIndex * 1.0f / rootGameObjectCount);
                var folders = new List<string>();
                CartographerSaveLoad.AddGameObjectToRootSceneCollection(rootGameObject, cartographerParams, scene, rootSceneGameObjects, folders, true);
            }

            // remove game objects that stays root (empty folders)
            foreach (var rootGameObject in rootGameObjects)
            {
                if ((rootGameObject != null) && (rootGameObject.transform.parent == null))
                {
                    UnityEngine.Object.DestroyImmediate(rootGameObject);
                }
            }

            CreateVegetation(vegetationTargets, cartographerParams, scene, progressCallback);

            var rootSceneAssetFolder = CartographerCommon.CombineAssetPath(cartographerParams.ResultCollectionFolder, cartographerParams.SceneCollectionSubFolder, string.Empty);
            if (!Directory.Exists(rootSceneAssetFolder))
            {
                Directory.CreateDirectory(rootSceneAssetFolder);
            }
            var sceneCount = rootSceneGameObjects.Count;
            var sceneIndex = 0;
            foreach (var rootSceneGameObject in rootSceneGameObjects)
            {
                ++sceneIndex;
                var sceneLoaderBehaviour = rootSceneGameObject.Value.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour != null)
                {
                    progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Creating scene: {sceneIndex}/{sceneCount}, name: {sceneLoaderBehaviour.SceneName}", sceneIndex * 1.0f / sceneCount);
                    result.Add(sceneLoaderBehaviour.SceneName);
                    if (createScenes)
                    {
                        var rootSceneAssetPath = CartographerCommon.CombineAssetPath(rootSceneAssetFolder, sceneLoaderBehaviour.SceneName, CartographerCommon.UnityExtension);
                        if (!File.Exists(rootSceneAssetPath))
                        {
                            var rootScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                            SceneManager.MoveGameObjectToScene(rootSceneGameObject.Value, rootScene);
                            CartographerSaveLoad.ProcessStreamSceneBeforeSaving(rootScene, rootSceneAssetPath);
                            SceneManager.SetActiveScene(rootScene);
                            Lightmapping.realtimeGI = false;
                            Lightmapping.bakedGI = false;
                            EditorSceneManager.SaveScene(rootScene, rootSceneAssetPath);
                            SceneManager.SetActiveScene(scene);
                            EditorSceneManager.CloseScene(rootScene, true);
                        }
                        else
                        {
                            SceneManager.MoveGameObjectToScene(rootSceneGameObject.Value, backupScene);
                        }
                        CartographerCommon.CleanupMemory();
                    }
                }
            }
            return result;
        }*/

        private static bool CreateTextMinimap(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection)
        {
            sceneCollection.SceneNames = new List<string>();

            var textMminimapPath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, cartographerParams.ResultSceneCollectionMinimap, CartographerCommon.TxtExtension);
            var textMinimapResult = new StringBuilder();

            var points = new List<Vector3Int>();
            var minPoint = new Vector3Int();
            var maxPoint = new Vector3Int();

            var badSceneNames = new List<string>();
            var scenes = Directory.EnumerateFiles(sceneCollection.SceneFolder, $"*{CartographerCommon.UnityExtension}", SearchOption.TopDirectoryOnly);
            foreach (var scene in scenes)
            {
                var sceneName = CartographerCommon.GetAssetNameFromAssetPath(scene, CartographerCommon.UnityExtension);
                Vector3Int point;
                if (CartographerCommon.IsSceneForStreaming(sceneName, out point))
                {
                    if (points.Count > 0)
                    {
                        minPoint.x = Math.Min(minPoint.x, point.x);
                        minPoint.y = Math.Min(minPoint.y, point.y);
                        minPoint.z = Math.Min(minPoint.z, point.z);
                        maxPoint.x = Math.Max(maxPoint.x, point.x);
                        maxPoint.y = Math.Max(maxPoint.y, point.y);
                        maxPoint.z = Math.Max(maxPoint.z, point.z);
                    }
                    else
                    {
                        minPoint = point;
                        maxPoint = point;
                    }
                    points.Add(point);
                }
                else
                {
                    badSceneNames.Add(sceneName);
                }
            }
            var pointSize = maxPoint - minPoint + Vector3Int.one;
            var pointArray = new int[pointSize.z, pointSize.x];
            foreach (var point in points)
            {
                pointArray[point.z - minPoint.z, point.x - minPoint.x] = 1;
            }
            var noSceneNames = new List<string>();
            for (var x = 0; x < pointSize.x; ++x)
            {
                var _x = x + minPoint.x;
                for (var z = 0; z < pointSize.z; ++z)
                {
                    var _z = z + minPoint.z;
                    if (pointArray[z, x] > 0)
                    {
                        var point = new Vector3Int(_x, 0, _z);
                        var sceneName = CartographerCommon.GetStreamSceneAssetName(point, sceneCollection);
                        if (File.Exists(CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension)))
                        {
                            sceneCollection.SceneNames.Add(sceneName);
                        }
                        else
                        {
                            noSceneNames.Add(sceneName);
                        }
                    }
                }
            }
            for (var z = pointSize.z - 1; z >= 0; --z)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                var _z = z + minPoint.z;
                if (_z < 0)
                {
                    textMinimapResult.Append($"{_z:00} ");
                }
                else
                {
                    textMinimapResult.Append($" {_z:00} ");
                }
                for (var x = 0; x < pointSize.x; ++x)
                {
                    textMinimapResult.Append(((pointArray[z, x] > 0) ? "[X]" : "[ ]"));
                }
            }
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append("    ");
            for (var x = 0; x < pointSize.x; ++x)
            {
                var _x = x + minPoint.x;
                if (_x < 0)
                {
                    textMinimapResult.Append($"{_x:00}");
                }
                else
                {
                    textMinimapResult.Append($" {_x:00}");
                }
            }
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append($"start: {minPoint.x}, {minPoint.y}, {minPoint.z}");
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append($"size: {pointSize.x}, {pointSize.y}, {pointSize.z}");
            foreach (var badSceneName in badSceneNames)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                textMinimapResult.Append($"bad: {badSceneName}");
            }
            foreach (var noSceneName in noSceneNames)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                textMinimapResult.Append($"non: {noSceneName}");
            }

            File.WriteAllText(textMminimapPath, textMinimapResult.ToString());

            sceneCollection.SceneStart = new SharedCode.Utils.Vector3Int(minPoint);
            sceneCollection.SceneCount = new SharedCode.Utils.Vector3Int(pointSize);
            return true;
        }

        private static bool CreateTextMinimap(SceneCollectionDef sceneCollection)
        {
            var textMminimapPath = CartographerCommon.CombineAssetPath(GameResourcesLikeFileSaver.GetFilePath(sceneCollection), string.Empty, CartographerCommon.TxtExtension);
            var textMinimapResult = new StringBuilder();

            var points = new List<Vector3Int>();
            var minPoint = new Vector3Int();
            var maxPoint = new Vector3Int();

            var badSceneNames = new List<string>();
            foreach (var sceneName in sceneCollection.SceneNames)
            {
                Vector3Int point;
                if (CartographerCommon.IsSceneForStreaming(sceneName, out point))
                {
                    if (points.Count > 0)
                    {
                        minPoint.x = Math.Min(minPoint.x, point.x);
                        minPoint.y = Math.Min(minPoint.y, point.y);
                        minPoint.z = Math.Min(minPoint.z, point.z);
                        maxPoint.x = Math.Max(maxPoint.x, point.x);
                        maxPoint.y = Math.Max(maxPoint.y, point.y);
                        maxPoint.z = Math.Max(maxPoint.z, point.z);
                    }
                    else
                    {
                        minPoint = point;
                        maxPoint = point;
                    }
                    points.Add(point);
                }
                else
                {
                    badSceneNames.Add(sceneName);
                }
            }
            var pointSize = maxPoint - minPoint + Vector3Int.one;
            var pointArray = new int[pointSize.z, pointSize.x];
            foreach (var point in points)
            {
                pointArray[point.z - minPoint.z, point.x - minPoint.x] = 1;
            }
            var noSceneNames = new List<string>();
            for (var x = 0; x < pointSize.x; ++x)
            {
                var _x = x + minPoint.x;
                for (var z = 0; z < pointSize.z; ++z)
                {
                    var _z = z + minPoint.z;
                    if (pointArray[z, x] > 0)
                    {
                        var point = new Vector3Int(_x, 0, _z);
                        var sceneName = CartographerCommon.GetStreamSceneAssetName(point, sceneCollection);
                        if (!File.Exists(CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension)))
                        {
                            noSceneNames.Add(sceneName);
                        }
                    }
                }
            }
            for (var z = pointSize.z - 1; z >= 0; --z)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                var _z = z + minPoint.z;
                if (_z < 0)
                {
                    textMinimapResult.Append($"{_z:00} ");
                }
                else
                {
                    textMinimapResult.Append($" {_z:00} ");
                }
                for (var x = 0; x < pointSize.x; ++x)
                {
                    textMinimapResult.Append(((pointArray[z, x] > 0) ? "[X]" : "[ ]"));
                }
            }
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append("    ");
            for (var x = 0; x < pointSize.x; ++x)
            {
                var _x = x + minPoint.x;
                if (_x < 0)
                {
                    textMinimapResult.Append($"{_x:00}");
                }
                else
                {
                    textMinimapResult.Append($" {_x:00}");
                }
            }
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append($"start: {minPoint.x}, {minPoint.y}, {minPoint.z}");
            CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
            textMinimapResult.Append($"size: {pointSize.x}, {pointSize.y}, {pointSize.z}");
            foreach (var badSceneName in badSceneNames)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                textMinimapResult.Append($"bad: {badSceneName}");
            }
            foreach (var noSceneName in noSceneNames)
            {
                CartographerCommon.AppendIfNotEmpty(textMinimapResult, "\r\n");
                textMinimapResult.Append($"non: {noSceneName}");
            }

            File.WriteAllText(textMminimapPath, textMinimapResult.ToString());

            sceneCollection.SceneStart = new SharedCode.Utils.Vector3Int(minPoint);
            sceneCollection.SceneCount = new SharedCode.Utils.Vector3Int(pointSize);
            return true;
        }

        // Public helpers old ---------------------------------------------------------------------
        [Obsolete("Doesn't call this right now")]
        public static bool CreateSceneCollectionFile(CartographerParamsDef cartographerParams)
        {
            var resultDef = new SceneCollectionDef();
            resultDef.CollectByX = cartographerParams.SourceCollection.CollectByX;
            resultDef.CollectByY = cartographerParams.SourceCollection.CollectByY;
            resultDef.CollectByZ = cartographerParams.SourceCollection.CollectByZ;
            resultDef.ScenePrefix = cartographerParams.SourceCollection.ScenePrefix;
            resultDef.SceneFolder = CartographerCommon.CombineAssetPath(cartographerParams.ResultCollectionFolder, cartographerParams.SceneCollectionSubFolder, string.Empty);
            resultDef.SceneSize = cartographerParams.SourceCollection.SceneSize;
            CreateTextMinimap(cartographerParams, resultDef);
            GameResourcesLikeFileSaver.SaveFile(resultDef.SceneFolder, cartographerParams.ResultSceneCollectionDefName, resultDef);
            return true;
        }

        [Obsolete("Doesn't call this right now")]
        public static bool FixSceneCollectionFile(SceneCollectionDef sceneCollection)
        {
            CreateTextMinimap(sceneCollection);
            GameResourcesLikeFileSaver.SaveFile(sceneCollection);
            return true;
        }

        [Obsolete("Doesn't call this right now")]
        public static bool ForceSaveAllPrefabs(ICartographerProgressCallback progressCallback)
        {
            var title = "Force save all prefabs and models";

            progressCallback?.OnBegin(progressSaveAllPrefabsTitle, title);

            var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
            var modelGUIDs = AssetDatabase.FindAssets("t:Model");

            var progressCount = prefabGUIDs.Length + modelGUIDs.Length;
            var progressIndex = 0;
            var saveIndex = 0;
            var saveThreshold = 1000;

            foreach (var prefabGUID in prefabGUIDs)
            {
                ++progressIndex;
                progressCallback.OnProgress(title, $"Prefabs... {progressIndex} / {progressCount}", progressIndex * 1.0f / progressCount);

                var path = AssetDatabase.GUIDToAssetPath(prefabGUID);
                var resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                EditorUtility.SetDirty(resource);

                if (saveIndex > saveThreshold)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    CartographerCommon.CleanupMemory();
                    saveIndex = 0;
                }
                else
                {
                    ++saveIndex;
                }
            }

            foreach (var modelGUID in modelGUIDs)
            {
                ++progressIndex;
                progressCallback.OnProgress(title, $"Models... {progressIndex} / {progressCount}", progressIndex * 1.0f / progressCount);

                var path = AssetDatabase.GUIDToAssetPath(modelGUID);
                var resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                EditorUtility.SetDirty(resource);

                if (saveIndex > saveThreshold)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    CartographerCommon.CleanupMemory();
                    saveIndex = 0;
                }
                else
                {
                    ++saveIndex;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CartographerCommon.CleanupMemory();

            progressCallback?.OnEnd();
            return true;
        }
        
        [Obsolete("Very old code, doesn't work")]
        public static void CheckTerrain(bool activeScene, string scenesPath)
        {
            if (activeScene)
            {
                var scene = SceneManager.GetActiveScene();
                CheckTerrainInScene(scene);
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(scenesPath);
                FileInfo[] sceneFileInfos = dir.GetFiles($"*{CartographerCommon.UnityExtension}", SearchOption.AllDirectories);
                var cancel = false;
                var sceneIndex = 0;

                var sceneHolder = new CartographerSceneHolder();
                sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);
                foreach (var sceneFileInfo in sceneFileInfos)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("RemoveReliefTerrain", $"Scene: {sceneFileInfo.Name} - {sceneIndex + 1}/{sceneFileInfos.Length}", (sceneIndex + 1.0f) / sceneFileInfos.Length))
                    {
                        cancel = true;
                    }

                    var scene = sceneHolder.OpenScene(sceneFileInfo.FullName, false);

                    CheckTerrainInScene(scene);

                    ++sceneIndex;
                    if (cancel)
                    {
                        break;
                    }
                }
                sceneHolder.Deinitialize();
            }
            EditorUtility.ClearProgressBar();
        }

        [Obsolete("Very old code, doesn't work")]
        public static void ClearTerrain(bool activeScene, string scenesPath, bool removeReliefTerrain, bool removeVegetation, bool removeTiles, bool removeDetailandTrees)
        {
            if (activeScene)
            {
                var scene = SceneManager.GetActiveScene();
                ClearTerrainInScene(scene, removeReliefTerrain, removeVegetation, removeTiles, removeDetailandTrees);
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(scenesPath);
                FileInfo[] sceneFileInfos = dir.GetFiles($"*{CartographerCommon.UnityExtension}", SearchOption.AllDirectories);
                var cancel = false;
                var sceneIndex = 0;
                var sceneHolder = new CartographerSceneHolder();
                sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);
                foreach (var sceneFileInfo in sceneFileInfos)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("RemoveReliefTerrain", $"Scene: {sceneFileInfo.Name} - {sceneIndex + 1}/{sceneFileInfos.Length}", (sceneIndex + 1.0f) / sceneFileInfos.Length))
                    {
                        cancel = true;
                    }

                    var scene = sceneHolder.OpenScene(sceneFileInfo.FullName, false);

                    ClearTerrainInScene(scene, removeReliefTerrain, removeVegetation, removeTiles, removeDetailandTrees);

                    EditorSceneManager.SaveScene(scene);

                    ++sceneIndex;
                    if (cancel)
                    {
                        break;
                    }
                }
                sceneHolder.Deinitialize();
            }
            EditorUtility.ClearProgressBar();
        }

        // Create Terrain Meshes for active scene
        [Obsolete("Used only for TerrainToMesh tests")]
        public static void CreateTerrainMeshes(Scene cartographerScene)
        {
            var activeScene = SceneManager.GetActiveScene();
            var gameObjectFolder = CartographerCommon.FindOrCreateChild("TerrainToMesh", activeScene, null, true);
            var sourceTerrains = Terrain.activeTerrains;
            foreach (var sourceTerrain in sourceTerrains)
            {
                var terrainPrefabsFolder = "Assets/_TEMORARY_TerrainToMesh";
                string terrainPrefabFolder = CartographerCommon.CombineAssetPath(terrainPrefabsFolder, string.Empty, string.Empty);
                if (!Directory.Exists(terrainPrefabFolder))
                {
                    Directory.CreateDirectory(terrainPrefabFolder);
                }
                var gameObjectName = sourceTerrain.name;
                var prefabName = sourceTerrain.name;
                var prefabPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, sourceTerrain.name, CartographerCommon.PrefabExtension);
                var meshPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, sourceTerrain.name + "_mesh", CartographerCommon.AssetExtension);
                var materialPath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, sourceTerrain.name + "_material", CartographerCommon.MatExtension);
                var normalTexturePath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, sourceTerrain.name + "_normal", CartographerCommon.PngExtension);
                var diffuseTexturePath = CartographerCommon.CombineAssetPath(terrainPrefabFolder, sourceTerrain.name + "_diffuse", CartographerCommon.PngExtension);
                var terrainTextureSize = 512;
                var terrainVertexCount = 128;
                CSOGenerateBackgroundClient.CreateTerrainMeshPrefab(cartographerScene, gameObjectFolder, gameObjectName, sourceTerrain, prefabName, prefabPath, meshPath, materialPath, diffuseTexturePath, normalTexturePath, terrainTextureSize, terrainVertexCount);
            }
        }

        [Obsolete("Outdated version, now commented in Cartographer editor")]
        public static bool ResaveSceneCollection(SceneCollectionDef sceneCollection, ICartographerProgressCallback progressCallback)
        {
            if (sceneCollection == null)
            {
                return false;
            }
            progressCallback?.OnBegin(progressSaveScenesTitle, "Begin resave scenes");
            var sceneCount = sceneCollection.SceneNames.Count;
            {
                var sceneHolder = new CartographerSceneHolder();
                sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var sceneName = sceneCollection.SceneNames[sceneIndex];

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Load scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    var scenePath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension);
                    var scene = sceneHolder.OpenScene(scenePath, false);

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Fix scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    ActivateAllSceneObjects(scene, true);
                    CartographerSaveLoad.ProcessStreamSceneBeforeSaving(scene, scenePath);

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Save scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    EditorSceneManager.SaveScene(scene);
                }
                sceneHolder.Deinitialize();
            }
            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return true;
        }

        [Obsolete("Outdated version, now commented in Cartographer editor")]
        public static bool CleanSceneCollection(SceneCollectionDef sceneCollection, ICartographerProgressCallback progressCallback)
        {
            if (sceneCollection == null)
            {
                return false;
            }
            progressCallback?.OnBegin(progressSaveScenesTitle, "Begin resave scenes");
            var sceneCount = sceneCollection.SceneNames.Count;
            {
                var sceneHolder = new CartographerSceneHolder();
                sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var sceneName = sceneCollection.SceneNames[sceneIndex];

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Load scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    var scenePath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension);
                    var scene = sceneHolder.OpenScene(scenePath, false);

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Fix scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    ActivateAllSceneObjects(scene, true);

                    var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
                    foreach (var rootGameObject in rootGameObjects)
                    {
                        var vegetationStudioManagers = rootGameObject.GetComponentsInChildren<VegetationStudioManager>(true);
                        var vegetationSystems = rootGameObject.GetComponentsInChildren<VegetationSystem>(true);
                        var persistentVegetationStorages = rootGameObject.GetComponentsInChildren<PersistentVegetationStorage>(true);

                        foreach (var vegetationStudioManager in vegetationStudioManagers)
                        {
                            UnityEngine.GameObject.DestroyImmediate(vegetationStudioManager);
                        }
                        foreach (var vegetationSystem in vegetationSystems)
                        {
                            UnityEngine.GameObject.DestroyImmediate(vegetationSystem);
                        }
                        foreach (var persistentVegetationStorage in persistentVegetationStorages)
                        {
                            UnityEngine.GameObject.DestroyImmediate(persistentVegetationStorage);
                        }
                    }

                    CartographerSaveLoad.ProcessStreamSceneBeforeSaving(scene, scenePath);

                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Save scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    EditorSceneManager.SaveScene(scene);
                }
                sceneHolder.Deinitialize();
            }
            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return true;
        }

        [Obsolete("Outdated version, now commented in Cartographer editor")]
        public static bool CheckSceneCollection(SceneCollectionDef sceneCollection, ICartographerProgressCallback progressCallback)
        {
            if (sceneCollection == null)
            {
                return false;
            }
            progressCallback?.OnBegin(progressCheckScenesTitle, "Begin resave scenes");
            var sceneCount = sceneCollection.SceneNames.Count;
            {
                var sceneHolder = new CartographerSceneHolder();
                sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var sceneName = sceneCollection.SceneNames[sceneIndex];

                    progressCallback?.OnProgress(progressCheckScenesTitle, $"Load scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    var scenePath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension);
                    var scene = sceneHolder.OpenScene(scenePath, false);

                    progressCallback?.OnProgress(progressCheckScenesTitle, $"Check scene: {sceneIndex + 1}/{sceneCount}, name: {sceneName}", (sceneIndex + 1) * 1.0f / sceneCount);

                    CheckScene(scene);
                }
                sceneHolder.Deinitialize();
            }
            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return true;
        }

        /*[Obsolete("Outdated version, used only for initial scene creation")]
        public static bool CreateSceneCollection(CartographerParamsDef cartographerParams, ICartographerProgressCallback progressCallback)
        {
            if (cartographerParams == null)
            {
                return false;
            }

            var backupScene = SceneManager.GetActiveScene();

            progressCallback?.OnBegin(progressСreateSceneCollectionTitle, "Begin creating scene collection");

            var temporaryScenePath = CartographerCommon.CombineAssetPath(cartographerParams.SourceCollection.SceneFolder, cartographerParams.TemporarySceneName, CartographerCommon.UnityExtension);
            progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Creating temporary scene: {cartographerParams.TemporarySceneName}", 0.0f);
            Scene temporaryScene = cartographerParams.CreateTemporaryScene ? EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive) : SceneManager.GetActiveScene();
            if (cartographerParams.CreateTemporaryScene)
            {
                EditorSceneManager.SaveScene(temporaryScene, temporaryScenePath);
                SceneManager.SetActiveScene(temporaryScene);
                CopyGameObjects(cartographerParams, temporaryScene, progressCallback);
                EditorSceneManager.SaveScene(temporaryScene);
                CartographerCommon.CleanupMemory();
            }
            List<KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>> vegetationTargets = (cartographerParams.CreateVegetation ? (new List<KeyValuePair<List<GameObject>, List<PersistentVegetationInfo>>>()) : null);
            if (cartographerParams.SplitTerrain)
            {
                SplitTerrains(vegetationTargets, cartographerParams, temporaryScene, progressCallback);
                EditorSceneManager.SaveScene(temporaryScene);
                CartographerCommon.CleanupMemory();
            }
            if (cartographerParams.CreateSceneCollection)
            {
                CreateRootSceneCollection(vegetationTargets, cartographerParams, temporaryScene, backupScene, progressCallback, cartographerParams.CreateScenes);
            }

            CartographerCommon.CleanupMemory();

            CreateSceneCollectionFile(cartographerParams);

            progressCallback?.OnProgress(progressСreateSceneCollectionTitle, $"Removing temporary scene: {cartographerParams.TemporarySceneName}", 0.0f);
            if (cartographerParams.CreateTemporaryScene)
            {
                EditorSceneManager.CloseScene(temporaryScene, true);
                AssetDatabase.DeleteAsset(temporaryScenePath);
            }

            CartographerCommon.CleanupMemory();

            SceneManager.SetActiveScene(backupScene);

            progressCallback?.OnEnd();
            return true;
        }*/
    }
};


