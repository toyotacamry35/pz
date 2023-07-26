using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOFixTerrainLayers : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Fix Terrain Layers"; } }
            public string RunQuestion { get { return "Are you sure you want to fix terrain layers?"; } }
            public string WelcomeMessage { get { return "Fix Terrain Layers"; } }
            public string OnScenePrefix { get { return "Fix Terrain Layers"; } }
        }
        public static MessagesClass Messages = new MessagesClass();

        internal class TerrainLayerUsage
        {
            public string guid { set; get; } = string.Empty;
            public string path { set; get; } = string.Empty;
            public string name { set; get; } = string.Empty;
            public string diffuseTextureGuid { set; get; } = string.Empty;
            public string normalMapTextureGuid { set; get; } = string.Empty;
            public bool good { set; get; } = false;
            public TerrainLayerUsage goodTerrainLayer { set; get; } = null;

            public string goodTerrainLayerGuid { set; get; } = string.Empty;
            public string goodTerrainLayerPath { set; get; } = string.Empty;

            public TerrainLayer terrainLayer { set; get; } = null;
        }

        private Dictionary<string, TerrainLayerUsage> terrainLayerData = new Dictionary<string, TerrainLayerUsage>();

        private void FixTerrainLayers(Scene scene)
        {
            bool somethingIsDirty = false;
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour == null)
                {
                    CartographerCommon.ReportError($"FixTerrainLayersOperation, Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
                else
                {
                    if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 0)
                    {
                        if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 1)
                        {
                            CartographerCommon.ReportError($"FixTerrainLayersOperation, SceneLoaderBehaviour has more the one registered terrain: {sceneLoaderBehaviour.GameObjectsTerrain.Count}, scene: {scene.name}");
                        }
                        foreach (var gameObjectTerrain in sceneLoaderBehaviour.GameObjectsTerrain)
                        {
                            var terrain = gameObjectTerrain.GetComponent<Terrain>();
                            if (terrain != null)
                            {
                                var terrainData = terrain.terrainData;
                                if ((terrainData != null) && (terrainData.terrainLayers != null) && (terrainData.terrainLayers.Length > 0))
                                {
                                    var newTerrainLayers = new TerrainLayer[terrainData.terrainLayers.Length];
                                    for (var index = 0; index < terrainData.terrainLayers.Length; ++index)
                                    {
                                        newTerrainLayers[index] = terrainData.terrainLayers[index];
                                        foreach (var data in terrainLayerData)
                                        {
                                            if (terrainData.terrainLayers[index] == data.Value.terrainLayer)
                                            {
                                                if (!data.Value.good && (data.Value.goodTerrainLayer != null) && (data.Value.goodTerrainLayer.terrainLayer != null))
                                                {
                                                    newTerrainLayers[index] = data.Value.goodTerrainLayer.terrainLayer;
                                                    somethingIsDirty = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (somethingIsDirty)
                                    {
                                        terrainData.terrainLayers = newTerrainLayers;
                                        EditorUtility.SetDirty(terrainData);
                                        EditorUtility.SetDirty(terrain);
                                        EditorUtility.SetDirty(gameObjectTerrain);
                                        EditorSceneManager.MarkSceneDirty(scene);
                                    }
                                }
                            }
                            else
                            {
                                CartographerCommon.ReportError($"CreateCraterServer, registered terrain {gameObjectTerrain.name} don't contain Terrain, scene: {scene.name}");
                            }
                        }
                    }
                }
            }
            if (somethingIsDirty)
            {
                EditorSceneManager.SaveScene(scene);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }

        public CSOFixTerrainLayers( string layersDataFilePath)
        {
            var lines = File.ReadAllLines(layersDataFilePath);
            foreach( var line in lines)
            {
                var segments = line.Split('\t');
                if (segments.Length != 9)
                {
                    break;
                }
                var terrainLayerUsage = new TerrainLayerUsage();

                terrainLayerUsage.guid = segments[0];
                terrainLayerUsage.path = segments[5];
                terrainLayerUsage.name = segments[4];

                terrainLayerUsage.diffuseTextureGuid = segments[6];
                terrainLayerUsage.normalMapTextureGuid = segments[7];

                terrainLayerUsage.good = (segments[1].Length == 4);

                terrainLayerUsage.goodTerrainLayerGuid = segments[2];
                terrainLayerUsage.goodTerrainLayerPath = segments[3];

                terrainLayerUsage.terrainLayer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(AssetDatabase.GUIDToAssetPath(terrainLayerUsage.guid));

                terrainLayerData.Add(terrainLayerUsage.guid, terrainLayerUsage);
            }

            foreach (var data in terrainLayerData)
            {
                if (!data.Value.good && !string.IsNullOrEmpty(data.Value.goodTerrainLayerGuid))
                {
                    TerrainLayerUsage goodTerrainLayerUsage = null;
                    if (terrainLayerData.TryGetValue(data.Value.goodTerrainLayerGuid, out goodTerrainLayerUsage))
                    {
                        data.Value.goodTerrainLayer = goodTerrainLayerUsage;
                    }
                }
            }
        }

        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            var checkedCoordinates = new Vector3Int();
            if (((cartographerScene.TypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection) && CartographerCommon.IsSceneForStreaming(cartographerScene.Name, out checkedCoordinates))
            {
                return true;
            }
            return false;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            FixTerrainLayers(scene);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
        }
    }
};