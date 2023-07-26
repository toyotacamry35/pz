using Assets.Src.Cartographer;
using AwesomeTechnologies.VegetationStudio;
using Core.Cheats;
using NLog;
using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Src.BuildingSystem
{
    public class GfxCheat
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool IsGameObjectFolder(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>();
            return (components.Length == 1);
        }

        private static List<GameObject> GetRootGameObjects(Scene scene)
        {
            var rootGameObjects = new List<GameObject>();
            var allRootGameObjects = scene.GetRootGameObjects();
            if ((allRootGameObjects != null) && (allRootGameObjects.Length > 0))
            {
                var rootGameObjectCount = allRootGameObjects.Length;
                for (var rootGameObjectIndex = 0; rootGameObjectIndex < rootGameObjectCount; ++rootGameObjectIndex)
                {
                    var rootGameObject = allRootGameObjects[rootGameObjectIndex];
                    rootGameObjects.Add(rootGameObject);
                }
            }
            return rootGameObjects;
        }

        private static List<GameObject> GetChildren(GameObject gameObject)
        {
            var children = new List<GameObject>();
            var childCount = gameObject.transform.childCount;
            if (childCount > 0)
            {
                for (var childIndex = 0; childIndex < childCount; ++childIndex)
                {
                    children.Add(gameObject.transform.GetChild(childIndex).gameObject);
                }
            }
            return children;
        }

        private static void GetAllGameObjects(GameObject gameObject, List<GameObject> result)
        {
            var addGameObject = true;
            if (IsGameObjectFolder(gameObject))
            {
                addGameObject = false;
            }
            if (addGameObject)
            {
                var sceneLoaderBehaviour = gameObject.GetComponent<SceneLoaderBehaviour>();
                var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                if ((sceneLoaderBehaviour != null) || (vegetationStudioManager != null))
                {
                    addGameObject = false;
                }
            }
            if (addGameObject)
            {
                result.Add(gameObject);
            }
            else
            {
                var childrenGameObjects = GetChildren(gameObject);
                var childrenCount = childrenGameObjects.Count;
                if (childrenCount > 0)
                {
                    for (var childIndex = 0; childIndex < childrenCount; ++childIndex)
                    {
                        var childGameObject = childrenGameObjects[childIndex];
                        GetAllGameObjects(childGameObject, result);
                    }
                }
            }
        }

        private static List<GameObject> GetAllGameObjects(Scene scene)
        {
            var result = new List<GameObject>();
            var rootGameObjects = GetRootGameObjects(scene);
            var rootCount = rootGameObjects.Count;
            if (rootCount > 0)
            {
                for (var rootIndex = 0; rootIndex < rootCount; ++rootIndex)
                {
                    var rootGameObject = rootGameObjects[rootIndex];
                    GetAllGameObjects(rootGameObject, result);
                }
            }
            return result;
        }

        // GfxCommands ----------------------------------------------------------------------------
        private class GfxCommand
        {
            public string Key { get; set; } = string.Empty;

            public Action<GfxCommand> ShowAction { get; set; } = DefaultShowAction;
            public Action<GfxCommand> HideAction { get; set; } = DefaultHideAction;

            public Dictionary<string, List<GameObject>> GameObjects { get; set; } = new Dictionary<string, List<GameObject>>();
            public Dictionary<string, List<Behaviour>> Behaviours { get; set; } = new Dictionary<string, List<Behaviour>>();
            public Dictionary<string, List<Renderer>> Renderers { get; set; } = new Dictionary<string, List<Renderer>>();

            private void Enable(bool enable)
            {
                foreach (var gameObjectsCollection in GameObjects)
                {
                    var gameObjectsCount = gameObjectsCollection.Value.Count;
                    if (gameObjectsCount > 0)
                    {
                        for (var gameObjectIndex = 0; gameObjectIndex < gameObjectsCount; ++gameObjectIndex)
                        {
                            var gameObject = gameObjectsCollection.Value[gameObjectIndex];
                            if (gameObject != null)
                            {
                                gameObject.SetActive(enable);
                            }
                        }
                    }
                }
                foreach (var behavioursCollection in Behaviours)
                {

                    var behavioursCount = behavioursCollection.Value.Count;
                    if (behavioursCount > 0)
                    {
                        for (var behaviourIndex = 0; behaviourIndex < behavioursCount; ++behaviourIndex)
                        {
                            var behaviour = behavioursCollection.Value[behaviourIndex];
                            if (behaviour != null)
                            {
                                behaviour.enabled = enable;
                            }
                        }
                    }
                }
                foreach (var renderersCollection in Renderers)
                {

                    var renderersCount = renderersCollection.Value.Count;
                    if (renderersCount > 0)
                    {
                        for (var rendererIndex = 0; rendererIndex < renderersCount; ++rendererIndex)
                        {
                            var renderer = renderersCollection.Value[rendererIndex];
                            if (renderer != null)
                            {
                                renderer.enabled = enable;
                            }
                        }
                    }
                }
            }

            private static void DefaultShowAction(GfxCommand command)
            {
                if (command != null)
                {
                    command.Enable(true);
                }
            }

            private static void DefaultHideAction(GfxCommand command)
            {
                if (command != null)
                {
                    command.Enable(false);
                }
            }

            public void Hide()
            {
                HideAction?.Invoke(this);
            }

            public void Show()
            {
                ShowAction?.Invoke(this);
            }
        }

        private static Dictionary<string, GfxCommand> registeredGfxCommands = new Dictionary<string, GfxCommand>();

        private static void HideHaze(GfxCommand command)
        {
            TOD.ASkyLighting.TimeOfDayHazeCheat(false);
        }

        private static void ShowHaze(GfxCommand command)
        {
            TOD.ASkyLighting.TimeOfDayHazeCheat(true);
        }

        private static void HideStreamTerrain(GfxCommand command)
        {
            SceneStreamerSystem.SetShowTerrainCheat(false);
        }

        private static void ShowStreamTerrain(GfxCommand command)
        {
            SceneStreamerSystem.SetShowTerrainCheat(true);
        }

        private static void HideStreamFoliage(GfxCommand command)
        {
            SceneStreamerSystem.SetShowFoliageCheat(false);
        }

        private static void ShowStreamFoliage(GfxCommand command)
        {
            SceneStreamerSystem.SetShowFoliageCheat(true);
        }

        private static void HideStreamObjects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowObjectsCheat(false);
        }

        private static void ShowStreamObjects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowObjectsCheat(true);
        }

        private static void HideStreamEffects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowEffectsCheat(false);
        }

        private static void ShowStreamEffects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowEffectsCheat(true);
        }

        private static void HideBackgroundTerrain(GfxCommand command)
        {
            SceneStreamerSystem.SetShowBackgroundTerrainCheat(false);
        }

        private static void ShowBackgroundTerrain(GfxCommand command)
        {
            SceneStreamerSystem.SetShowBackgroundTerrainCheat(true);
        }

        private static void HideBackgroundObjects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowBackgroundObjectsCheat(false);
        }

        private static void ShowBackgroundObjects(GfxCommand command)
        {
            SceneStreamerSystem.SetShowBackgroundObjectsCheat(true);
        }

        private static void HideShadows(GfxCommand command)
        {
            foreach (var behavioursCollection in command.Behaviours)
            {
                var behavioursCount = behavioursCollection.Value.Count;
                if (behavioursCount > 0)
                {
                    for (var behaviourIndex = 0; behaviourIndex < behavioursCount; ++behaviourIndex)
                    {
                        var light = behavioursCollection.Value[behaviourIndex] as Light;
                        if (light != null)
                        {
                            light.shadows = LightShadows.None;
                        }
                    }
                }
            }
        }

        private static void ShowShadows(GfxCommand command)
        {
            foreach (var behavioursCollection in command.Behaviours)
            {
                var soft = behavioursCollection.Key.Equals("soft");
                var behavioursCount = behavioursCollection.Value.Count;
                if (behavioursCount > 0)
                {
                    for (var behaviourIndex = 0; behaviourIndex < behavioursCount; ++behaviourIndex)
                    {
                        var light = behavioursCollection.Value[behaviourIndex] as Light;
                        if (light != null)
                        {
                            light.shadows = soft ? LightShadows.Soft : LightShadows.Hard;
                        }
                    }
                }
            }
        }

        private static void FillComponents(GfxCommand command)
        {
            var behaviourKey = string.Empty;
            var rendereKey = string.Empty;
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var rootGameObject in rootGameObjects)
                {
                    var behaviours = rootGameObject.GetComponentsInChildren<Behaviour>();
                    if (behaviours != null)
                    {
                        foreach (var behaviour in behaviours)
                        {
                            if (behaviour.enabled)
                            {
                                var name = behaviour.GetType().Name.ToLower();
                                if (name.Contains(command.Key) || command.Key.Contains(name))
                                {
                                    List<Behaviour> behavioursCollection;
                                    if (!command.Behaviours.TryGetValue(behaviourKey, out behavioursCollection))
                                    {
                                        behavioursCollection = new List<Behaviour>();
                                        command.Behaviours.Add(behaviourKey, behavioursCollection);
                                    }
                                    behavioursCollection.Add(behaviour);
                                }
                            }
                        }
                    }
                    var renderers = rootGameObject.GetComponentsInChildren<Renderer>();
                    if (renderers != null)
                    {
                        foreach (var renderer in renderers)
                        {
                            if (renderer.enabled)
                            {
                                var name = renderer.GetType().Name.ToLower();
                                if (name.Contains(command.Key) || command.Key.Contains(name))
                                {
                                    List<Renderer> renderersCollection;
                                    if (!command.Renderers.TryGetValue(rendereKey, out renderersCollection))
                                    {
                                        renderersCollection = new List<Renderer>();
                                        command.Renderers.Add(rendereKey, renderersCollection);
                                    }
                                    renderersCollection.Add(renderer);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void FillGameObjects(GfxCommand command)
        {
            var gameObjectKey = string.Empty;
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var gameObjects = GetAllGameObjects(scene);
                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.activeSelf)
                    {
                        var name = gameObject.name.ToLower();
                        if (name.Contains(command.Key) || command.Key.Contains(name))
                        {
                            List<GameObject> gameObjectsCollection;
                            if (!command.GameObjects.TryGetValue(gameObjectKey, out gameObjectsCollection))
                            {
                                gameObjectsCollection = new List<GameObject>();
                                command.GameObjects.Add(gameObjectKey, gameObjectsCollection);
                            }
                            gameObjectsCollection.Add(gameObject);
                        }
                    }
                }
            }
        }

        private static void FillShadows(GfxCommand command)
        {
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var rootGameObject in rootGameObjects)
                {
                    var lights = rootGameObject.GetComponentsInChildren<Light>();
                    if (lights != null)
                    {
                        foreach (var light in lights)
                        {
                            var lightsCollectionKey = string.Empty;
                            if (light.shadows == LightShadows.Soft)
                            {
                                lightsCollectionKey = "soft";
                            }
                            else if (light.shadows == LightShadows.Hard)
                            {
                                lightsCollectionKey = "hard";
                            }
                            if (!string.IsNullOrEmpty(lightsCollectionKey))
                            {
                                List<Behaviour> lightsCollection;
                                if (!command.Behaviours.TryGetValue(lightsCollectionKey, out lightsCollection))
                                {
                                    lightsCollection = new List<Behaviour>();
                                    command.Behaviours.Add(lightsCollectionKey, lightsCollection);
                                }
                                lightsCollection.Add(light);
                            }
                        }
                    }
                }
            }
        }

        private static GfxCommand CreateGfxCommand(string key)
        {
            var newGfxCommand = new GfxCommand();
            newGfxCommand.Key = key;
            if (key.StartsWith("haze"))
            {
                newGfxCommand.HideAction = HideHaze;
                newGfxCommand.ShowAction = ShowHaze;
            }
            else if (key.StartsWith("shadow"))
            {
                FillShadows(newGfxCommand);
                newGfxCommand.HideAction = HideShadows;
                newGfxCommand.ShowAction = ShowShadows;
            }
            else if (key.Equals("st"))
            {
                newGfxCommand.HideAction = HideStreamTerrain;
                newGfxCommand.ShowAction = ShowStreamTerrain;
            }
            else if (key.Equals("sf"))
            {
                newGfxCommand.HideAction = HideStreamFoliage;
                newGfxCommand.ShowAction = ShowStreamFoliage;
            }
            else if (key.Equals("so"))
            {
                newGfxCommand.HideAction = HideStreamObjects;
                newGfxCommand.ShowAction = ShowStreamObjects;
            }
            else if (key.Equals("se"))
            {
                newGfxCommand.HideAction = HideStreamEffects;
                newGfxCommand.ShowAction = ShowStreamEffects;
            }
            else if (key.Equals("bt"))
            {
                newGfxCommand.HideAction = HideBackgroundTerrain;
                newGfxCommand.ShowAction = ShowBackgroundTerrain;
            }
            else if (key.Equals("bo"))
            {
                newGfxCommand.HideAction = HideBackgroundObjects;
                newGfxCommand.ShowAction = ShowBackgroundObjects;
            }
            else if ((key.Length > 1) && (key[0] == '.'))
            {
                newGfxCommand.Key = key.Substring(1);
                FillComponents(newGfxCommand);
            }
            else if (key.Length > 0)
            {
                FillGameObjects(newGfxCommand);
            }
            else
            {
                newGfxCommand = null;
            }
            return newGfxCommand;
        }

        private static GfxCommand RegisterGfxCommand(string key)
        {
            if (registeredGfxCommands.ContainsKey(key))
            {
                return null;
            }
            var newGfxCommand = CreateGfxCommand(key);
            if (newGfxCommand != null)
            {
                registeredGfxCommands.Add(newGfxCommand.Key, newGfxCommand);
            }
            return newGfxCommand;
        }

        private static GfxCommand RemoveGfxCommand(string key, bool similar)
        {
            if (registeredGfxCommands.TryGetValue(key, out GfxCommand registeredGfxCommand))
            {
                registeredGfxCommands.Remove(key);
                return registeredGfxCommand;
            }
            if (similar)
            {
                foreach (var similarRegisteredGfxCommand in registeredGfxCommands)
                {
                    var registeredKey = similarRegisteredGfxCommand.Key;
                    var registeredKey1 = registeredKey.Substring(1);
                    var key1 = key.Substring(1);
                    if (registeredKey.StartsWith(key) || registeredKey1.StartsWith(key) || key.StartsWith(registeredKey) || key1.StartsWith(registeredKey))
                    {
                        registeredGfxCommands.Remove(similarRegisteredGfxCommand.Key);
                        return similarRegisteredGfxCommand.Value;
                    }
                }
            }
            return null;
        }

        private static void ListAllGfxCommands()
        {
            foreach (var registeredGfxCommand in registeredGfxCommands)
            {
                Logger.IfInfo()?.Message($"gfx: {registeredGfxCommand.Key} is hidden").Write();
            }
        }

        private static void ShowAndRemoveAllGfxCommands()
        {
            foreach (var registeredGfxCommand in registeredGfxCommands)
            {
                registeredGfxCommand.Value.Show();
            }
            registeredGfxCommands.Clear();
        }

        // Cheats ---------------------------------------------------------------------------------
        [Cheat]
        public static void GfxList()
        {
            Logger.IfInfo()?.Message($"gfx list").Write();
            ListAllGfxCommands();
        }

        [Cheat]
        public static void GfxHide(string key)
        {
            key = key.ToLower();
            var command = RegisterGfxCommand(key);
            if (command != null)
            {
                Logger.IfInfo()?.Message($"gfx hide: {key}").Write();
                command.Hide();
            }
            else
            {
                Logger.IfInfo()?.Message($"gfx returns error").Write();
            }
        }

        [Cheat]
        public static void GfxShow(string key)
        {
            key = key.ToLower();
            var command = RemoveGfxCommand(key, true);
            if (command != null)
            {
                Logger.IfInfo()?.Message($"gfx show: {key}").Write();
                command.Show();
            }
            else
            {
                Logger.IfInfo()?.Message($"gfx returns error").Write();
            }
        }

        [Cheat]
        public static void GfxReset()
        {
            Logger.IfInfo()?.Message($"gfx reset").Write();
            ShowAndRemoveAllGfxCommands();
        }
    }
}