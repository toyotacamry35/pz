using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SharedCode.Aspects.Cartographer;
using Assets.TerrainBaker;
using Assets.Src.Shared;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOGenerateBackgroundServer : ICartographerSceneOperation
    {
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Generate Server Background"; } }
            public string RunQuestion { get { return "Are you sure you want to generate server background?"; } }
            public string WelcomeMessage { get { return "Generate server background"; } }
            public string OnScenePrefix { get { return "Generate server background"; } }
        }
        public static MessagesClass Messages = new MessagesClass();

        private static float ColliderBoundsEpsilon = 0.1f;

        private Vector3Int checkedCoordinates = new Vector3Int();
        private Scene craterScene = new Scene();
        private bool craterSceneWasNotLoaded = false;

        private static bool ValidateGameObject(Scene scene, GameObject gameObject)
        {
            var keep = true;
            if (keep)
            {
                var fracturedObjects = gameObject.GetComponentsInChildren<FracturedObject>(true);
                if ((fracturedObjects != null) && (fracturedObjects.Length > 0))
                {
                    keep = false;
                }
            }
            if (keep)
            {
                var collidiers = gameObject.GetComponentsInChildren<Collider>(true);
                var nonTriggerColliders = 0;
                if ((collidiers != null) && (collidiers.Length > 0))
                {
                    foreach (var collider in collidiers)
                    {
                        if (collider.isTrigger)
                        {
                            UnityEngine.Object.DestroyImmediate(collider);
                        }
                        else if (!collider.gameObject.activeInHierarchy)
                        {
                            UnityEngine.Object.DestroyImmediate(collider);
                        }
                        else if (collider.bounds.extents.magnitude < ColliderBoundsEpsilon)
                        {
                            UnityEngine.Object.DestroyImmediate(collider);
                        }
                        else
                        {
                            ++nonTriggerColliders;
                        }
                    }
                }
                if (nonTriggerColliders == 0)
                {
                    keep = false;
                }
            }
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
                                    if (component is Collider)
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
                                if ((component is Transform) || (component is Collider))
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

        private static void StartCraterForGameObjectFolder(Scene craterScene, Vector3Int coordinates, GameObject gameObjectFolder, CartographerParamsDef.BackgroundServerCreation craterCreationParams)
        {
            if (CartographerCommon.InsideRect(coordinates, craterCreationParams.CleanupTerrainColliders))
            {
                var gameObjectToDestroy = CartographerCommon.FindChild(craterCreationParams.TerrainCollidersGameObjectName, craterScene, gameObjectFolder, false);
                if (gameObjectToDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
                }
            }
            if (CartographerCommon.InsideRect(coordinates, craterCreationParams.CleanupStaticObjectColliders))
            {
                var gameObjectToDestroy = CartographerCommon.FindChild(craterCreationParams.StaticObjectCollidersGameObjectName, craterScene, gameObjectFolder, false);
                if (gameObjectToDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
                }
            }
        }

        private static void StartCrater(Scene craterScene, CartographerParamsDef.BackgroundServerCreation craterCreationParams, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
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
             
        private static void FinishCrater(Scene craterScene, CartographerParamsDef.BackgroundServerCreation craterCreationParams, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
        }

        private static void GenerateCrater(Scene craterScene, Scene scene, Vector3Int coordinates, CartographerParamsDef.BackgroundServerCreation craterCreationParams, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            var gameObjectFolder = CartographerCommon.FindOrCreateChild(scene.name, craterScene, null, true);
            if (openedScenesOnly)
            {
                StartCraterForGameObjectFolder(craterScene, coordinates, gameObjectFolder, craterCreationParams);
            }
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour != null)
                {
                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateTerrainColliders))
                    {
                        if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 0)
                        {
                            foreach (var gameObjectTerrain in sceneLoaderBehaviour.GameObjectsTerrain)
                            {
                                var materialSupport = gameObjectTerrain.GetComponent<TerrainBakerMaterialSupport>();
                                if (materialSupport != null)
                                {
                                    gameObjectFolder.transform.position = materialSupport.transform.position;
                                    var mesh = materialSupport.terrainMesh;
                                    if (mesh != null)
                                    {
                                        if (!CartographerCommon.IsCollidersIgnored(gameObjectTerrain))
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
                                            var meshCollider = terrainGameObject.GetComponent<MeshCollider>();
                                            if (meshCollider == null)
                                            {
                                                meshCollider = terrainGameObject.AddComponent<MeshCollider>();
                                            }
                                            meshCollider.convex = false;
                                            meshCollider.sharedMaterial = physicMaterial;
                                            meshCollider.sharedMesh = mesh;
                                            meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.WeldColocatedVertices;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        CartographerCommon.ReportError($"CreateCraterServer, registered terrain {gameObjectTerrain.name} don't contain mesh, scene: {scene.name}");
                                    }
                                }
                                else
                                {
                                    CartographerCommon.ReportError($"CreateCraterServer, registered terrain {gameObjectTerrain.name} don't contain TerrainBakerMaterialSupport, scene: {scene.name}");
                                }
                            }
                            if (sceneLoaderBehaviour.GameObjectsTerrain.Count > 1)
                            {
                                CartographerCommon.ReportError($"CreateCraterServer, SceneLoaderBehaviour has more the one registered terrain: {sceneLoaderBehaviour.GameObjectsTerrain.Count}, scene: {scene.name}");
                            }
                        }
                        else
                        {
                            CartographerCommon.ReportError($"CreateCraterServer, SceneLoaderBehaviour have no any terrain registered: {rootGameObject.name}, scene: {scene.name}");
                        }
                    }
                    if (CartographerCommon.InsideRect(coordinates, craterCreationParams.GenerateStaticObjectColliders))
                    {
                        if (sceneLoaderBehaviour.GameObjectsStatic.Count > 0)
                        {
                            var staticObjectFolder = CartographerCommon.FindOrCreateChild(craterCreationParams.StaticObjectCollidersGameObjectName, craterScene, gameObjectFolder, true);
                            staticObjectFolder.transform.position = gameObjectFolder.transform.position;
                            foreach (var gameObjectStatic in sceneLoaderBehaviour.GameObjectsStatic)
                            {
                                if (gameObjectStatic.activeSelf)
                                {
                                    var newGameObject = UnityEngine.Object.Instantiate(gameObjectStatic);
                                    if (newGameObject != null)
                                    {
                                        if (ValidateGameObject(scene, newGameObject))
                                        {
                                            newGameObject.transform.position = gameObjectStatic.transform.position;
                                            newGameObject.transform.rotation = gameObjectStatic.transform.rotation;
                                            newGameObject.transform.localScale = gameObjectStatic.transform.localScale;
                                            newGameObject.transform.parent = staticObjectFolder.transform;
                                            newGameObject.name = gameObjectStatic.name;
                                            newGameObject.SetActive(true);
                                        }
                                        else
                                        {
                                            UnityEngine.Object.DestroyImmediate(newGameObject);
                                        }
                                    }
                                    else
                                    {
                                        CartographerCommon.ReportError($"CreateCraterServer, SceneLoaderBehaviour can't clone registered static object: {gameObjectStatic.name}, scene: {scene.name}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    CartographerCommon.ReportError($"CreateCraterServer, Unknown root object: {rootGameObject.name}, scene: {scene.name}");
                }
            }
        }

        // ICartographerSceneOperation interface ----------------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            if (((cartographerScene.TypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection) && CartographerCommon.IsSceneForStreaming(cartographerScene.Name, out checkedCoordinates))
            {
                if (CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundServerCreationParams.GenerateTerrainColliders) ||
                    CartographerCommon.InsideRect(checkedCoordinates, cartographerParams.BackgroundServerCreationParams.GenerateStaticObjectColliders))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            progressCallback?.OnBegin(Messages.Title, "Load crater server scene");
            var craterScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundServerSceneName, string.Empty, CartographerCommon.UnityExtension);
            craterScene = CartographerCommon.FindLoadedScene(craterScenePath);
            craterSceneWasNotLoaded = !craterScene.isLoaded;
            if (craterSceneWasNotLoaded)
            {
                craterScene = EditorSceneManager.OpenScene(craterScenePath, OpenSceneMode.Additive);
            }
            StartCrater(craterScene, cartographerParams.BackgroundServerCreationParams, openedScenesOnly, progressCallback);
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            GenerateCrater(craterScene, scene, checkedCoordinates, cartographerParams.BackgroundServerCreationParams, openedScenesOnly, progressCallback);
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            FinishCrater(craterScene, cartographerParams.BackgroundServerCreationParams, openedScenesOnly, progressCallback);
            progressCallback?.OnProgress(Messages.Title, $"Saving crater server scene: {craterScene.name}", 1.0f);
            EditorSceneManager.SaveScene(craterScene);
            if (craterSceneWasNotLoaded)
            {
                EditorSceneManager.CloseScene(craterScene, true);
            }
        }
    }
};