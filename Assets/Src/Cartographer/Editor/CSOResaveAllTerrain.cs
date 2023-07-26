using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOResaveAllTerrain : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Resave Terrains"; } }
            public string RunQuestion { get { return "Are you sure you want to resave terrains?"; } }
            public string WelcomeMessage { get { return "Resave terrains"; } }
            public string OnScenePrefix { get { return "Resave terrains"; } }
        }
        public static MessagesClass Messages = new MessagesClass();

        private static void ResaveTerrain(Scene scene)
        {
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour == null)
                {
                    CartographerCommon.ReportError($"CreateCraterServer, Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
                else
                {
                    if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 0)
                    {
                        if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 1)
                        {
                            CartographerCommon.ReportError($"CreateCraterServer, SceneLoaderBehaviour has more the one registered terrain: {sceneLoaderBehaviour.GameObjectsTerrain.Count}, scene: {scene.name}");
                        }
                        foreach (var gameObjectTerrain in sceneLoaderBehaviour.GameObjectsTerrain)
                        {
                            var terrain = gameObjectTerrain.GetComponent<Terrain>();
                            if (terrain != null)
                            {
                                //terrain.materialType = Terrain.MaterialType.BuiltInStandard;
                                //terrain.materialTemplate = null;
                                //terrain.basemapDistance = 1000;
                                EditorUtility.SetDirty(terrain.terrainData);
                            }
                            else
                            {
                                CartographerCommon.ReportError($"CreateCraterServer, registered terrain {gameObjectTerrain.name} don't contain Terrain, scene: {scene.name}");
                            }
                        }
                    }
                    else
                    {
                        CartographerCommon.ReportError($"CreateCraterServer, SceneLoaderBehaviour have no any terrain registered: {rootGameObject.name}, scene: {scene.name}");
                    }
                }
            }
        }
        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            Vector3Int checkedCoordinates;
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
            //ResaveTerrain(scene);
            //EditorSceneManager.SaveScene(scene);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
        }
    }
};