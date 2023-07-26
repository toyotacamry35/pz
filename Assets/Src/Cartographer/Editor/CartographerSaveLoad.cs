using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationStudio;
using Assets.TerrainBaker;
using SharedCode.Aspects.Cartographer;
using JesseStiller.TerrainFormerExtension;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerSaveLoad
    {
        // Constants ------------------------------------------------------------------------------
        private static readonly bool defaultDisableSceneCallbacks = false;
        private static readonly Vector3 defaultStreamSceneSize = new Vector3(256.0f, 0.0f, 256.0f);
        private static readonly Vector3 defaultMinimalOcculderBounds = new Vector3(256.0f, 256.0f, 256.0f);

        private static readonly string progressSaveScenesTitle = "Save Scenes";
        private static readonly string progressLoadScenesTitle = "Load Scenes";
        private static readonly string progressRemoveScenesTitle = "Remove Scenes";

        // Public properties ----------------------------------------------------------------------
        public static bool DisableSceneCallbacks { get; set; } = defaultDisableSceneCallbacks;
        public static Vector3 StreamSceneSize { get; set; } = defaultStreamSceneSize;
        public static Vector3 MinimalOcculderBounds { get; set; } = defaultMinimalOcculderBounds;

        // Scene callbacks ------------------------------------------------------------------------
        private static void OnSceneSaving(Scene scene, string path)
        {
            ProcessStreamSceneBeforeSaving(scene, path);
        }

        private static void OnSceneSaved(Scene scene)
        {
            ProcessStreamSceneAfterSave(scene);
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            ProcessStreamSceneAfterOpen(scene, mode);
        }

        // Private methods ------------------------------------------------------------------------
        private static bool SetFlagForGameObject(GameObject gameObject, StaticEditorFlags flagsToSet)
        {
            var flags = GameObjectUtility.GetStaticEditorFlags(gameObject);
            if ((flags & flagsToSet) != flagsToSet)
            {
                flags = flags | flagsToSet;
                GameObjectUtility.SetStaticEditorFlags(gameObject, flags);
                return true;
            }
            return false;
        }

        private static bool ClearFlagForGameObject(GameObject gameObject, StaticEditorFlags flagsToSet)
        {
            var flags = GameObjectUtility.GetStaticEditorFlags(gameObject);
            if ((flags & flagsToSet) > 0)
            {
                flags = flags & ~flagsToSet;
                GameObjectUtility.SetStaticEditorFlags(gameObject, flags);
                return true;
            }
            return false;
        }

        private static bool IsOccluderGameObject(Renderer renderer)
        {
            var result = false;
            //var bounds = renderer.bounds; // temporary turned off automatic occuder addition
            //var result = (((bounds.extents.x * 2.0f) >= MinimalOcculderBounds.x) ||
            //              ((bounds.extents.y * 2.0f) >= MinimalOcculderBounds.y) ||
            //              ((bounds.extents.z * 2.0f) >= MinimalOcculderBounds.z));
            if (!result)
            {
                var bigGameObjectMarker = renderer.gameObject.GetComponentInParent<BigGameObjectMarkerBehaviour>();
                if (bigGameObjectMarker != null)
                {
                    result = bigGameObjectMarker.AddToOccuders;
                }
            }
            return result;
        }

        private static bool FixFlagsBeforeSave(GameObject gameObject, SceneLoaderGameObjectType gameObjectType)
        {
            var result = false;
            GameObjectUtility.SetStaticEditorFlags(gameObject, 0);
            if (SetFlagForGameObject(gameObject, StaticEditorFlags.NavigationStatic | StaticEditorFlags.ReflectionProbeStatic | StaticEditorFlags.OccludeeStatic))
            {
                result = true;
            }
            if (gameObjectType == SceneLoaderGameObjectType.GameObjectTerrain)
            {
                if (SetFlagForGameObject(gameObject, StaticEditorFlags.OccluderStatic))
                {
                    result = true;
                }
            }
            else if ( gameObjectType == SceneLoaderGameObjectType.GameObjectStatic)
            {
                var lodGroups = gameObject.GetComponentsInChildren<LODGroup>(true);
                if ((lodGroups != null) && (lodGroups.Length > 0))
                {
                    foreach (var lodGroup in lodGroups)
                    {
                        var lods = lodGroup.GetLODs();
                        if ((lods != null) && (lods.Length > 0))
                        {
                            for( int index = 0; index <lods.Length; ++index)
                            {
                                var renderers = lods[index].renderers;
                                if ((renderers != null) && (renderers.Length > 0))
                                {
                                    foreach (var renderer in renderers)
                                    {
                                        if (renderer != null)
                                        {
                                            if (SetFlagForGameObject(renderer.gameObject, StaticEditorFlags.OccludeeStatic))
                                            {
                                                result = true;
                                            }
                                            if (index == 0)
                                            {
                                                if (IsOccluderGameObject(renderer))
                                                {
                                                    if (SetFlagForGameObject(renderer.gameObject, StaticEditorFlags.OccluderStatic))
                                                    {
                                                        result = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (ClearFlagForGameObject(renderer.gameObject, StaticEditorFlags.OccluderStatic))
                                                    {
                                                        result = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        //TODO make it private after remove obsolete functions
        public static void ProcessStreamSceneBeforeSaving(Scene scene, string path)
        {
            if (CartographerCommon.IsEditor())
            {
                //var debugTimer = new System.Diagnostics.Stopwatch();
                //debugTimer.Start();

                var sceneName = string.Empty;
                if (!string.IsNullOrEmpty(sceneName))
                {
                    sceneName = scene.name;
                }
                else
                {
                    var fileInfo = new FileInfo(path);
                    sceneName = fileInfo.Name;
                }
                Vector3Int coordinates;
                if (!DisableSceneCallbacks && CartographerCommon.IsSceneForStreaming(sceneName, out coordinates))
                {
                    CreateSceneActivator(scene, coordinates, StreamSceneSize);
                    ActivateSceneActivator(scene, false);
                }

                //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
                //Debug.LogError($"OnSceneSaving, time: {seconds} sec, scene: {scene.name}");
            }
        }

        private static void ProcessStreamSceneAfterSave(Scene scene)
        {
            if (CartographerCommon.IsEditor())
            {
                //var debugTimer = new System.Diagnostics.Stopwatch();
                //debugTimer.Start();

                if (!DisableSceneCallbacks && CartographerCommon.IsSceneForStreaming(scene.name))
                {
                    ActivateSceneActivator(scene, true);
                }

                //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
                //Debug.LogError($"OnSceneSaved, time: {seconds} sec, scene: {scene.name}");
            }
        }

        private static void ProcessStreamSceneAfterOpen(Scene scene, OpenSceneMode mode)
        {
            //Debug.LogError($"CartographerUtils.OnSceneOpened() isEditor: {IsEditor()}, DisableSceneCallbacks: :{DisableSceneCallbacks}, scene: {scene.name}, mode: {mode}");

            if (CartographerCommon.IsEditor())
            {
                //var debugTimer = new System.Diagnostics.Stopwatch();
                //debugTimer.Start();

                if (!DisableSceneCallbacks && CartographerCommon.IsSceneForStreaming(scene.name))
                {
                    ActivateSceneActivator(scene, true);
                }

                //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
                //Debug.LogError($"OnSceneOpened, time: {seconds} sec, scene: {scene.name}");
            }
        }

        private static void CollectGameObjectsToActivate(GameObject gameObject, bool ignore, List<GameObject> terrainGameObjects, List<GameObject> staticGameObjects, List<GameObject> fxGameObjects)
        {
            if (ignore || CartographerCommon.IsGameObjectFolder(gameObject))
            {
                var children = CartographerCommon.GetChildren(gameObject);
                foreach (var child in children)
                {
                    CollectGameObjectsToActivate(child, false, terrainGameObjects, staticGameObjects, fxGameObjects);
                }
            }
            else if (!ignore)
            {
                // sort VegetationStudioManager holders 
                var components = gameObject.GetComponents<Component>();
                if (components.Length == 2)
                {
                    var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                    if (vegetationStudioManager != null)
                    {
                        var children = CartographerCommon.GetChildren(gameObject);
                        foreach (var child in children)
                        {
                            CollectGameObjectsToActivate(child, false, terrainGameObjects, staticGameObjects, fxGameObjects);
                        }
                        return;
                    }
                }
                // objects with components
                var terrainBakerMaterialSupport = gameObject.GetComponent<TerrainBakerMaterialSupport>();
                var fx = gameObject.GetComponent<ParticleSystem>();
                if (terrainBakerMaterialSupport != null)
                {
                    // terrain
                    terrainGameObjects.Add(gameObject);
                }
                else if (fx != null)
                {
                    // fx
                    fxGameObjects.Add(gameObject);
                }
                else
                {
                    // other
                    staticGameObjects.Add(gameObject);
                }
            }
        }

        private static void ActivateSceneActivator(Scene scene, bool activate)
        {
            //ReportError($"CartographerUtils.ActivateSceneActivator(), scene: {scene.name}, activate: {activate}");
            SceneStreamerSystem.DebugReport(false)?.Invoke(false, $"CartographerUtils.ActivateSceneActivator(), scene: {scene.name}, activate: {activate}");
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            foreach (var rootGameObject in rootGameObjects)
            {
                CartographerSceneLoaderVisitor.Visit(rootGameObject, (gameObject, gameObjectType) =>
                {
                    if (gameObject != null)
                    {
                        gameObject.SetActive(activate);
                    }
                    return true;
                });
            }
        }

        private static bool CreateSceneActivator(Scene scene, Vector3Int coordinates, Vector3 sceneSize)
        {
            bool result = false;
            SceneLoaderBehaviour activeSceneLoaderBehaviour = null;
            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
            var gameObjectsToAdd = new List<GameObject>();
            foreach (var rootGameObject in rootGameObjects)
            {
                var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                if (sceneLoaderBehaviour != null)
                {
                    if (activeSceneLoaderBehaviour == null)
                    {
                        activeSceneLoaderBehaviour = sceneLoaderBehaviour;
                    }
                    else
                    {
                        CartographerCommon.ReportError($"CartographerUtils.CreateSceneActivator(), found another SceneLoader, scene: {scene.name}, object: {rootGameObject}");
                        UnityEngine.Object.DestroyImmediate(sceneLoaderBehaviour);
                        gameObjectsToAdd.Add(rootGameObject);
                        result = true;
                    }
                }
                else
                {
                    gameObjectsToAdd.Add(rootGameObject);
                }
            }
            if (activeSceneLoaderBehaviour == null)
            {
                CartographerCommon.ReportError($"CartographerUtils.CreateSceneActivator(), create SceneLoader, scene: {scene.name}");
                var sceneLoaderBehaviourGameObject = new GameObject("SceneLoader");
                SceneManager.MoveGameObjectToScene(sceneLoaderBehaviourGameObject, scene);
                sceneLoaderBehaviourGameObject.transform.position = Vector3.zero;
                sceneLoaderBehaviourGameObject.transform.rotation = Quaternion.identity;
                sceneLoaderBehaviourGameObject.transform.localScale = Vector3.one;
                activeSceneLoaderBehaviour = sceneLoaderBehaviourGameObject.AddComponent<SceneLoaderBehaviour>();
                result = true;
            }
            if (activeSceneLoaderBehaviour != null)
            {
                if (gameObjectsToAdd.Count > 0)
                {
                    CartographerCommon.ReportError($"CartographerUtils.CreateSceneActivator(), move {gameObjectsToAdd.Count} game objects to SceneLoader, scene: {scene.name}");
                    foreach (var gameObjectToAdd in gameObjectsToAdd)
                    {
                        gameObjectToAdd.transform.SetParent(activeSceneLoaderBehaviour.gameObject.transform);
                    }
                    result = true;
                }
                var terrainGameObjectsToActivate = new List<GameObject>();
                var staticGameObjectsToActivate = new List<GameObject>();
                var fxGameObjectsToActivate = new List<GameObject>();
                CollectGameObjectsToActivate(activeSceneLoaderBehaviour.gameObject, true, terrainGameObjectsToActivate, staticGameObjectsToActivate, fxGameObjectsToActivate);
                activeSceneLoaderBehaviour.enabled = true;
                activeSceneLoaderBehaviour.GameObjectsTerrain = terrainGameObjectsToActivate;
                activeSceneLoaderBehaviour.GameObjectsStatic = staticGameObjectsToActivate;
                activeSceneLoaderBehaviour.GameObjectsFX = fxGameObjectsToActivate;
                // fix flags
                CartographerSceneLoaderVisitor.Visit(activeSceneLoaderBehaviour.gameObject, (gameObject, gameObjectType) =>
                {
                    if (FixFlagsBeforeSave(gameObject, gameObjectType))
                    {
                        result = true;
                    }
                    return true;
                });
                // fix terrain positions
                foreach (var terrainGameObject in activeSceneLoaderBehaviour.GameObjectsTerrain)
                {
                    var position = new Vector3(sceneSize.x * coordinates.x, sceneSize.y * coordinates.y, sceneSize.z * coordinates.z);
                    if (terrainGameObject.transform.position != position)
                    {
                        CartographerCommon.ReportError($"CartographerUtils.CreateSceneActivator(), terrain: {terrainGameObject.name} is not in right position: {terrainGameObject.transform.position}, must be: {position}: , scene: {scene.name}, fixing now...");
                        terrainGameObject.transform.position = position;
                    }
                }
            }
            else
            {
                CartographerCommon.ReportError($"CartographerUtils.CreateSceneActivator(), can't find SceneLoader, scene: {scene.name}");
            }
            return result;
        }

        private static void MoveSceneGameObjectToSceneCollection(Scene scene, GameObject sceneGameObject, SceneCollectionDef resultDef)
        {
            Vector3Int sceneGameObjectCoordinates;
            if (CartographerCommon.IsSceneForStreaming(sceneGameObject.name, out sceneGameObjectCoordinates))
            {
                var sceneCount = SceneManager.sceneCount;
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var sceneInCollection = SceneManager.GetSceneAt(sceneIndex);
                    Vector3Int sceneCoordinates;
                    if (sceneInCollection.isLoaded && CartographerCommon.IsSceneForStreaming(sceneInCollection.name, out sceneCoordinates))
                    {
                        if (sceneGameObjectCoordinates == sceneCoordinates)
                        {
                            var rootGameObjects = CartographerCommon.GetRootGameObjects(sceneInCollection);
                            foreach (var rootGameObject in rootGameObjects)
                            {
                                if (rootGameObject.GetComponent<SceneLoaderBehaviour>() != null)
                                {
                                    var children = CartographerCommon.GetChildren(sceneGameObject);
                                    foreach (var child in children)
                                    {
                                        var folders = new List<string>();
                                        MoveGameObjectToRootGameObjct(child, sceneInCollection, rootGameObject, folders);
                                        EditorSceneManager.MarkSceneDirty(sceneInCollection);
                                    }
                                    UnityEngine.Object.DestroyImmediate(sceneGameObject);
                                    EditorSceneManager.MarkSceneDirty(scene);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        //Comment it because it was using patheon and demiurge scripts and it seems obsolete
        /*
        private static bool CheckGameObject(GameObject gameObject)
        {
            if (gameObject.name.StartsWith("New Game Object"))
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            else if (gameObject.name.StartsWith("Missing Prefab"))
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            else if (gameObject.GetComponent<TerrainFormer>() != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            if (gameObject.transform.childCount > 0)
            {
                var children = CartographerCommon.GetChildren(gameObject);
                foreach (var child in children)
                {
                    CheckGameObject(child);
                }
            }
            if (CartographerCommon.IsGameObjectFolder(gameObject) && (gameObject.transform.childCount == 0))
            {
                UnityEngine.Object.DestroyImmediate(gameObject);
                return false;
            }
            return true;
        }

        
        public static void AddGameObjectToRootSceneCollection(GameObject gameObject, CartographerParamsDef cartographerParams, Scene scene, Dictionary<Vector3, GameObject> rootSceneGameObjects, List<string> folders, bool checkGameObect)
        {
            if (!checkGameObect || CheckGameObject(gameObject))
            {
                if (CartographerCommon.IsGameObjectFolder(gameObject))
                {
                    if (gameObject.transform.childCount > 0)
                    {
                        var children = CartographerCommon.GetChildren(gameObject);
                        folders.Add(gameObject.name);
                        foreach (var child in children)
                        {
                            AddGameObjectToRootSceneCollection(child, cartographerParams, scene, rootSceneGameObjects, folders, false);
                        }
                        folders.RemoveAt(folders.Count - 1);
                    }
                }
                else
                {
                    AddNonFolderGameObjectToRootSceneCollection(gameObject, cartographerParams, scene, rootSceneGameObjects, folders);
                }
            }
        }*/

        private static void AddNonFolderGameObjectToRootSceneCollection(GameObject gameObject, CartographerParamsDef cartographerParams, Scene scene, Dictionary<Vector3, GameObject> rootSceneGameObjects, List<string> folders)
        {
            var rootSceneIndex = CartographerCommon.GetStreamSceneCoordinates(gameObject.transform.position, cartographerParams.SourceCollection);
            GameObject rootSceneGameObject;
            if (!rootSceneGameObjects.TryGetValue(rootSceneIndex, out rootSceneGameObject))
            {
                rootSceneGameObject = new GameObject(cartographerParams.RootSceneGameObjectName);
                SceneManager.MoveGameObjectToScene(rootSceneGameObject, scene);
                var sceneLoaderBehaviour = rootSceneGameObject.AddComponent<SceneLoaderBehaviour>();
                sceneLoaderBehaviour.SceneName = CartographerCommon.GetStreamSceneAssetName(rootSceneIndex, cartographerParams.SourceCollection);
                sceneLoaderBehaviour.SceneIndex = rootSceneIndex;
                sceneLoaderBehaviour.SceneStart = new Vector3(rootSceneIndex.x * cartographerParams.SourceCollection.SceneSize.x, rootSceneIndex.y * cartographerParams.SourceCollection.SceneSize.y, rootSceneIndex.z * cartographerParams.SourceCollection.SceneSize.z);
                sceneLoaderBehaviour.SceneSize = new Vector3(cartographerParams.SourceCollection.SceneSize.x, cartographerParams.SourceCollection.SceneSize.y, cartographerParams.SourceCollection.SceneSize.z);
                rootSceneGameObjects.Add(rootSceneIndex, rootSceneGameObject);
            }
            MoveNonFolderGameObjectToRootGameObjct(gameObject, scene, rootSceneGameObject, folders);
        }

        private static void MoveNonFolderGameObjectToRootGameObjct(GameObject gameObject, Scene scene, GameObject rootGameObject, List<string> folders)
        {
            GameObject currentParent = rootGameObject;
            if ((folders != null) && (folders.Count > 0))
            {
                for (var folderIndex = 0; folderIndex < folders.Count; ++folderIndex)
                {
                    currentParent = CartographerCommon.FindOrCreateChild(folders[folderIndex], scene, currentParent, true);
                }
            }
            gameObject.transform.SetParent(currentParent.transform, true);
        }

        private static void MoveGameObjectToRootGameObjct(GameObject gameObject, Scene scene, GameObject rootGameObject, List<string> folders)
        {
            if (CartographerCommon.IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount > 0)
                {
                    var children = CartographerCommon.GetChildren(gameObject);
                    folders.Add(gameObject.name);
                    foreach (var child in children)
                    {
                        MoveGameObjectToRootGameObjct(child, scene, rootGameObject, folders);
                    }
                    folders.RemoveAt(folders.Count - 1);
                }
            }
            else
            {
                MoveNonFolderGameObjectToRootGameObjct(gameObject, scene, rootGameObject, folders);
            }
        }

        private static void CheckGameObjectInRightSceneIgnoreTerrain(Scene scene, GameObject gameObject, Vector3Int coordinates, bool ignore, Scene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, List<string> folders)
        {
            if (ignore || CartographerCommon.IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount > 0)
                {
                    var children = CartographerCommon.GetChildren(gameObject);
                    if (!ignore) { folders.Add(gameObject.name); }
                    foreach (var child in children)
                    {
                        CheckGameObjectInRightSceneIgnoreTerrain(scene, child, coordinates, false, cartographerScene, cartographerParams, sceneCollection, folders);
                    }
                    if (!ignore) { folders.RemoveAt(folders.Count - 1); }
                }
            }
            else if (!gameObject.name.Equals(cartographerParams.RootTerrainGameObjectName))
            {

                var sceneIndex = CartographerCommon.GetStreamSceneCoordinates(gameObject.transform.position, sceneCollection);
                if (sceneIndex != coordinates)
                {
                    var rootGameObjectName = CartographerCommon.GetStreamSceneAssetName(sceneIndex, sceneCollection);
                    var rootGameObject = CartographerCommon.FindOrCreateChild(rootGameObjectName, cartographerScene, null, true);
                    MoveNonFolderGameObjectToRootGameObjct(gameObject, cartographerScene, rootGameObject, folders);
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.MarkSceneDirty(cartographerScene);
                }
            }
        }

        // Interface ------------------------------------------------------------------------------
        public static List<string> GetStreamScenePaths(HashSet<int> selection, SceneCollectionDef sceneCollection)
        {
            var result = new List<string>();
            var scenes = Directory.EnumerateFiles(sceneCollection.SceneFolder, $"*{CartographerCommon.UnityExtension}", SearchOption.TopDirectoryOnly);
            foreach (var scene in scenes)
            {
                var sceneName = CartographerCommon.GetAssetNameFromAssetPath(scene, CartographerCommon.UnityExtension);
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(sceneName, out coordinates))
                {
                    var item = CartographerCommon.GetStreamScenePackedIndex(coordinates, sceneCollection);
                    if (selection.Contains(item))
                    {
                        result.Add(CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension));
                    }
                }
            }
            return result;
        }

        public static List<string> GetStreamScenePaths(RectInt rect, SceneCollectionDef sceneCollection)
        {
            var result = new List<string>();
            var scenes = Directory.EnumerateFiles(sceneCollection.SceneFolder, $"*{CartographerCommon.UnityExtension}", SearchOption.TopDirectoryOnly);
            foreach (var scene in scenes)
            {
                var sceneName = CartographerCommon.GetAssetNameFromAssetPath(scene, CartographerCommon.UnityExtension);
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(sceneName, out coordinates))
                {
                    if (CartographerCommon.InsideRect(coordinates, rect, sceneCollection))
                    {
                        result.Add(CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension));
                    }
                }
            }
            return result;
        }

        public static List<string> GetStreamScenePaths(CartographerParamsDef.SceneLoadArea area, bool ignoreCachedSceneNames)
        {
            if (area == null) { return null; }
            var result = new List<string>();
            if (!ignoreCachedSceneNames && (area.Collection.SceneNames != null) && (area.Collection.SceneNames.Count > 0))
            {
                foreach (var sceneName in area.Collection.SceneNames)
                {
                    result.Add(CartographerCommon.CombineAssetPath(area.Collection.SceneFolder, sceneName, CartographerCommon.UnityExtension));
                }
            }
            else
            {
                area.Collection.SceneNames = new List<string>();
                var scenes = Directory.EnumerateFiles(area.Collection.SceneFolder, $"*{CartographerCommon.UnityExtension}", SearchOption.TopDirectoryOnly);
                foreach (var scene in scenes)
                {
                    var sceneName = CartographerCommon.GetAssetNameFromAssetPath(scene, CartographerCommon.UnityExtension);
                    Vector3Int coordinates;
                    if (CartographerCommon.IsSceneForStreaming(sceneName, out coordinates))
                    {
                        if (CartographerCommon.InsideSceneCollection(coordinates, area.Collection))
                        {
                            area.Collection.SceneNames.Add(sceneName);
                            result.Add(CartographerCommon.CombineAssetPath(area.Collection.SceneFolder, sceneName, CartographerCommon.UnityExtension));
                        }
                    }
                }
            }
            return result;
        }

        public static int GetLoadedStreamSceneCoordinates(List<Vector3Int> loadedStreamSceneCoordinates)
        {
            int added = 0;
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                Vector3Int coordinates;
                if (CartographerCommon.IsSceneForStreaming(scene.name, out coordinates))
                {
                    loadedStreamSceneCoordinates.Add(coordinates);
                    ++added;
                }
            }
            return added;
        }

        public static void TouchAllOpenedStreamScenes()
        {
            if (CartographerCommon.IsEditor())
            {
                var scenesCount = SceneManager.sceneCount;
                CartographerCommon.ReportInfo($"CartographerUtils.InitializeOnLoad().CheckAllOpenedScenes(), scenesCount: {scenesCount}");
                for (var sceneIndex = 0; sceneIndex < scenesCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    if (scene.isLoaded)
                    {
                        OnSceneOpened(scene, OpenSceneMode.Additive);
                    }
                }
            }
        }

        public static void SaveAllOpenedStreamScenes(Scene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool checkGameObjects, bool moveGameObjects, bool saveScenes, bool forceSaveScenes, List<string> scenePaths, ICartographerProgressCallback progressCallback)
        {
            progressCallback?.OnBegin(progressSaveScenesTitle, "Begin save scenes");

            SceneManager.SetActiveScene(cartographerScene);
            var sceneCount = SceneManager.sceneCount;
            if (checkGameObjects || moveGameObjects || saveScenes)
            {
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    Vector3Int coordinates;
                    if (scene.isLoaded && CartographerCommon.IsSceneForStreaming(scene.name, out coordinates))
                    {
                        var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                        if ((scenePaths == null) || scenePaths.Contains(scenePath))
                        {
                            if (CreateSceneActivator(scene, coordinates, StreamSceneSize))
                            {
                                EditorSceneManager.MarkSceneDirty(scene);
                            }
                        }
                    }
                }
            }
            if (checkGameObjects)
            {
                progressCallback?.OnProgress(progressSaveScenesTitle, "Check scene objects", 0.0f);
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    Vector3Int coordinates;
                    if (scene.isLoaded && CartographerCommon.IsSceneForStreaming(scene.name, out coordinates))
                    {
                        var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                        if ((scenePaths == null) || scenePaths.Contains(scenePath))
                        {
                            progressCallback?.OnProgress(progressSaveScenesTitle, $"Check scene objects: {sceneIndex + 1}/{sceneCount}, name: {scene.name}", (sceneIndex + 1) * 1.0f / sceneCount);
                            var rootGameObjects = CartographerCommon.GetRootGameObjects(scene);
                            foreach (var rootGameObject in rootGameObjects)
                            {
                                var sceneLoader = rootGameObject.GetComponent<SceneLoaderBehaviour>() != null;
                                var folders = new List<string>();
                                CheckGameObjectInRightSceneIgnoreTerrain(scene, rootGameObject, coordinates, sceneLoader, cartographerScene, cartographerParams, sceneCollection, folders);
                            }
                        }
                    }
                }
            }
            if (moveGameObjects)
            {
                progressCallback?.OnProgress(progressSaveScenesTitle, "Move objects if needed", 0.0f);
                var cartographerSceneRootGameObjects = CartographerCommon.GetRootGameObjects(cartographerScene);
                var objectsMoved = 0;
                var objectsCount = cartographerSceneRootGameObjects.Count;
                foreach (var cartographerSceneRootGameObject in cartographerSceneRootGameObjects)
                {
                    ++objectsMoved;
                    var cartographerBehaviour = cartographerSceneRootGameObject.GetComponent<CartographerBehaviour>();
                    var terrainFormer = cartographerSceneRootGameObject.GetComponent<TerrainFormer>();
                    if ((cartographerBehaviour == null) && (terrainFormer == null))
                    {
                        progressCallback?.OnProgress(progressSaveScenesTitle, $"Move objects to scene: {objectsMoved}/{sceneCount}, name: {cartographerSceneRootGameObject.name}", objectsMoved * 1.0f / objectsCount);
                        MoveSceneGameObjectToSceneCollection(cartographerScene, cartographerSceneRootGameObject, sceneCollection);
                    }
                }
            }
            if (saveScenes)
            {
                progressCallback?.OnProgress(progressSaveScenesTitle, "Save scene if needed", 0.0f);
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    Vector3Int coordinates;
                    if (scene.isLoaded && CartographerCommon.IsSceneForStreaming(scene.name, out coordinates))
                    {
                        var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                        if ((scenePaths == null) || scenePaths.Contains(scenePath))
                        {
                            if (forceSaveScenes || scene.isDirty)
                            {
                                progressCallback?.OnProgress(progressSaveScenesTitle, $"Save scene if needed: {sceneIndex + 1}/{sceneCount + 1}, name: {scene.name}", (sceneIndex + 1) * 1.0f / (sceneCount + 1));
                                EditorSceneManager.SaveScene(scene);
                            }
                        }
                    }
                }
                if (cartographerScene.isDirty)
                {
                    progressCallback?.OnProgress(progressSaveScenesTitle, $"Save scene if needed: {sceneCount + 1}/{sceneCount + 1}, name: {cartographerScene.name}", (sceneCount + 1) * 1.0f / (sceneCount + 1));
                    EditorSceneManager.SaveScene(cartographerScene);
                }
            }
            progressCallback?.OnEnd();
        }

        public static bool LoadStreamScenes(List<string> scenePaths, ICartographerProgressCallback progressCallback)
        {
            progressCallback?.OnBegin(progressLoadScenesTitle, "Begin loading scenes");

            var loaddedScenes = new List<string>();
            var loaddedScenesCount = 0;

            var result = false;
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                if (scenePaths.Contains(scenePath))
                {
                    ++loaddedScenesCount;
                    progressCallback?.OnProgress(progressLoadScenesTitle, $"Load scene: {loaddedScenesCount}/{scenePaths.Count}, name: {scene.name}", loaddedScenesCount * 1.0f / scenePaths.Count);
                    if (!scene.isLoaded)
                    {
                        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                        result = true;
                    }
                    loaddedScenes.Add(scenePath);
                }
            }

            foreach (var scenePath in scenePaths)
            {
                if (!loaddedScenes.Contains(scenePath))
                {
                    ++loaddedScenesCount;
                    var sceneName = CartographerCommon.GetAssetNameFromAssetPath(scenePath, CartographerCommon.UnityExtension);
                    progressCallback?.OnProgress(progressLoadScenesTitle, $"Load scene: {loaddedScenesCount}/{scenePaths.Count}, name: {sceneName}", loaddedScenesCount * 1.0f / scenePaths.Count);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    result = true;
                }
            }

            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return result;
        }

        public static bool RemoveOpenedStreamScenes(List<string> scenePaths, ICartographerProgressCallback progressCallback)
        {
            progressCallback?.OnBegin(progressRemoveScenesTitle, "Begin removing scenes");

            var result = false;
            var removedScenesCount = 0;
            var somethingRemoved = true;
            while (somethingRemoved)
            {
                somethingRemoved = false;
                var sceneCount = SceneManager.sceneCount;
                for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                {
                    var scene = SceneManager.GetSceneAt(sceneIndex);
                    var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                    if (scenePaths.Contains(scenePath))
                    {
                        ++removedScenesCount;
                        progressCallback?.OnProgress(progressRemoveScenesTitle, $"Remove scene: {removedScenesCount}/{scenePaths.Count}, name: {scene.name}", removedScenesCount * 1.0f / scenePaths.Count);
                        EditorSceneManager.CloseScene(scene, true);
                        somethingRemoved = true;
                        result = true;
                        break;
                    }
                }
            }

            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return result;
        }

        public static List<string> CheckOpenedStreamScenesForChanges(List<string> scenePaths)
        {
            var result = new List<string>();
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.isLoaded && CartographerCommon.IsSceneForStreaming(scene.name))
                {
                    var scenePath = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                    if ((scenePaths == null) || scenePaths.Contains(scenePath))
                    {
                        if (scene.isDirty)
                        {
                            result.Add(scenePath);
                        }
                    }
                }
            }
            return result;
        }

        // InitializeOnLoadMethod -----------------------------------------------------------------
        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            //Debug.LogError("CartographerUtils.InitializeOnLoad()");
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            TouchAllOpenedStreamScenes();
        }
    }
};