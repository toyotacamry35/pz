using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using SharedCode.Aspects.Cartographer;

namespace Assets.Src.Cartographer.Editor
{
    [Flags]
    public enum CartographerSceneType
    {
        None = 0x00,
        StreamCollection = 0x01,
        BackgroundClient = 0x02,
        BackgroundServer = 0x04,
        MapDefClient = 0x08,
        MapDefServer = 0x10,
        Unknown = 0x20,
    }

    public class CartographerScene
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public CartographerSceneType TypeMask { get; set; } = CartographerSceneType.None;
    }

    public interface IProgressMessages
    {
        string Title { get; }
        string WelcomeMessage { get; }
        string OnScenePrefix { get; }
    }

    public interface ICartographerSceneOperation
    {
        IProgressMessages ProgressMessages { get; }

        //Scene can be not loaded at this moment, check only by scene name on scene collection type
        bool CanOperate(CartographerScene сartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly);

        bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback);
        bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback);
        void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback);
    }

    public class CartographerSceneOperation
    {
        private static void AddToCartographerScenes(List<CartographerScene> cartographerScenes, string scenePathToAdd, CartographerSceneType cartographerSceneTypeMask)
        {
            if ((cartographerScenes != null) && !string.IsNullOrEmpty(scenePathToAdd))
            {
                foreach (var cartographerScene in cartographerScenes)
                {
                    if (string.Compare(cartographerScene.Path, scenePathToAdd, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        cartographerScene.TypeMask = cartographerScene.TypeMask | cartographerSceneTypeMask;
                        return;
                    }
                }
                var newСartographerScene = new CartographerScene();
                newСartographerScene.Name = CartographerCommon.GetAssetNameFromAssetPath(scenePathToAdd, CartographerCommon.UnityExtension);
                newСartographerScene.Path = scenePathToAdd;
                newСartographerScene.TypeMask = cartographerSceneTypeMask;
                cartographerScenes.Add(newСartographerScene);
            }
        }

        public static CartographerSceneType GetCartographerSceneTypeMask(string scenePath, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection)
        {
            var cartographerSceneTypeMask = CartographerSceneType.None;
            if (sceneCollection != null)
            {
                foreach (var sceneName in sceneCollection.SceneNames)
                {
                    var streamScenePath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, sceneName, CartographerCommon.UnityExtension);
                    if (string.Compare(streamScenePath, scenePath, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.StreamCollection;
                        break;
                    }
                }
            }
            if (cartographerParams != null)
            {
                var backgroundClientScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundClientSceneName, string.Empty, CartographerCommon.UnityExtension);
                if (string.Compare(backgroundClientScenePath, scenePath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.BackgroundClient;
                }
                var backgroundServerScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundServerSceneName, string.Empty, CartographerCommon.UnityExtension);
                if (string.Compare(backgroundServerScenePath, scenePath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.BackgroundServer;
                }
                if (cartographerParams.MapDef.Target != null)
                {
                    var mapDef = cartographerParams.MapDef.Target;
                    foreach (var mapDefClientScenePath in mapDef.GlobalScenesClient)
                    {
                        var _mapDefClientScenePath = CartographerCommon.CombineAssetPath(mapDefClientScenePath, string.Empty, CartographerCommon.UnityExtension);
                        if (string.Compare(_mapDefClientScenePath, scenePath, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.MapDefClient;
                            break;
                        }
                    }
                    foreach (var mapDefServerScenePath in mapDef.GlobalScenesServer)
                    {
                        var _mapDefServerScenePath = CartographerCommon.CombineAssetPath(mapDefServerScenePath, string.Empty, CartographerCommon.UnityExtension);
                        if (string.Compare(_mapDefServerScenePath, scenePath, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.MapDefServer;
                            break;
                        }
                    }
                }
            }
            if (cartographerSceneTypeMask == CartographerSceneType.None)
            {
                cartographerSceneTypeMask = cartographerSceneTypeMask | CartographerSceneType.Unknown;
            }
            return cartographerSceneTypeMask;
        }

        // Common operate scene collection method -------------------------------------------------
        public static bool Operate(ICartographerSceneOperation operation, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, CartographerSceneType cartographerSceneTypeMask, ICartographerProgressCallback progressCallback)
        {
            if (sceneCollection == null)
            {
                return false;
            }
            if (operation != null)
            {
                var progressMessages = operation.ProgressMessages;
                if ((progressMessages != null) && (progressCallback != null)) { progressCallback.OnBegin(progressMessages.Title, progressMessages.WelcomeMessage); }
                if (operation.Start(cartographerParams, sceneCollection, openedScenesOnly, progressCallback))
                {
                    if (openedScenesOnly)
                    {
                        var sceneCount = SceneManager.sceneCount;
                        for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
                        {
                            var scene = SceneManager.GetSceneAt(sceneIndex);
                            var newCartographerScene = new CartographerScene();
                            newCartographerScene.Name = scene.name;
                            newCartographerScene.Path = CartographerCommon.CombineAssetPath(scene.path, string.Empty, CartographerCommon.UnityExtension);
                            newCartographerScene.TypeMask = GetCartographerSceneTypeMask(newCartographerScene.Path, cartographerParams, sceneCollection);
                            if (scene.isLoaded && operation.CanOperate(newCartographerScene, cartographerParams, sceneCollection, openedScenesOnly))
                            {
                                if ((progressMessages != null) && (progressCallback != null)) { progressCallback.OnProgress(progressMessages.Title, $"{progressMessages.OnScenePrefix}: {sceneIndex + 1}/{sceneCount}, name: {scene.name}", (sceneIndex + 1) * 1.0f / sceneCount); }
                                if (!operation.Operate(scene, newCartographerScene.TypeMask, cartographerParams, sceneCollection, openedScenesOnly, progressCallback))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var sceneHolder = new CartographerSceneHolder();
                        sceneHolder.Initialize(CartographerSceneHolder.DefaultKeepCount, true);

                        var newCartographerScenes = new List<CartographerScene>();

                        if (((cartographerSceneTypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection) && (sceneCollection != null))
                        {
                            foreach (var streamSceneName in sceneCollection.SceneNames)
                            {
                                var streamScenePath = CartographerCommon.CombineAssetPath(sceneCollection.SceneFolder, streamSceneName, CartographerCommon.UnityExtension);
                                AddToCartographerScenes(newCartographerScenes, streamScenePath, CartographerSceneType.StreamCollection);
                            }
                        }
                        if (cartographerParams != null)
                        {
                            if ((cartographerSceneTypeMask & CartographerSceneType.BackgroundClient) == CartographerSceneType.BackgroundClient)
                            {
                                var backgroundClientScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundClientSceneName, string.Empty, CartographerCommon.UnityExtension);
                                AddToCartographerScenes(newCartographerScenes, backgroundClientScenePath, CartographerSceneType.BackgroundClient);
                            }
                            if ((cartographerSceneTypeMask & CartographerSceneType.BackgroundServer) == CartographerSceneType.BackgroundServer)
                            {
                                var backgroundServerScenePath = CartographerCommon.CombineAssetPath(cartographerParams.BackgroundServerSceneName, string.Empty, CartographerCommon.UnityExtension);
                                AddToCartographerScenes(newCartographerScenes, backgroundServerScenePath, CartographerSceneType.BackgroundServer);
                            }
                            if (cartographerParams.MapDef.Target != null)
                            {
                                var mapDef = cartographerParams.MapDef.Target;
                                if ((cartographerSceneTypeMask & CartographerSceneType.MapDefClient) == CartographerSceneType.MapDefClient)
                                {
                                    foreach (var mapDefClientScenePath in mapDef.GlobalScenesClient)
                                    {
                                        var _mapDefClientScenePath = CartographerCommon.CombineAssetPath(mapDefClientScenePath, string.Empty, CartographerCommon.UnityExtension);
                                        AddToCartographerScenes(newCartographerScenes, _mapDefClientScenePath, CartographerSceneType.MapDefClient);
                                    }
                                }
                                if ((cartographerSceneTypeMask & CartographerSceneType.MapDefServer) == CartographerSceneType.MapDefServer)
                                {
                                    foreach (var mapDefServerScenePath in mapDef.GlobalScenesServer)
                                    {
                                        var _mapDefServerScenePath = CartographerCommon.CombineAssetPath(mapDefServerScenePath, string.Empty, CartographerCommon.UnityExtension);
                                        AddToCartographerScenes(newCartographerScenes, _mapDefServerScenePath, CartographerSceneType.MapDefServer);
                                    }
                                }
                            }
                        }

                        var newCartographerScenesCount = newCartographerScenes.Count;
                        for (var newCartographerSceneIndex = 0; newCartographerSceneIndex < newCartographerScenesCount; ++newCartographerSceneIndex)
                        {
                            var cartographerScene = newCartographerScenes[newCartographerSceneIndex];
                            if (operation.CanOperate(cartographerScene, cartographerParams, sceneCollection, openedScenesOnly))
                            {
                                if ((progressMessages != null) && (progressCallback != null)) { progressCallback.OnProgress(progressMessages.Title, $"Load scene: {newCartographerSceneIndex + 1}/{newCartographerScenesCount}, name: {cartographerScene.Name}", (newCartographerSceneIndex + 1) * 1.0f / newCartographerScenesCount); }
                                var scene = sceneHolder.OpenScene(cartographerScene.Path, (cartographerScene.TypeMask & CartographerSceneType.StreamCollection) == 0);
                                if ((progressMessages != null) && (progressCallback != null)) { progressCallback.OnProgress(progressMessages.Title, $"{progressMessages.OnScenePrefix}: {newCartographerSceneIndex + 1}/{newCartographerScenesCount}, name: {scene.name}", (newCartographerSceneIndex + 1) * 1.0f / newCartographerScenesCount); }
                                if (!operation.Operate(scene, cartographerScene.TypeMask, cartographerParams, sceneCollection, openedScenesOnly, progressCallback))
                                {
                                    break;
                                }
                            }
                        }
                        sceneHolder.Deinitialize();
                    }
                }
                operation.Finish(cartographerParams, sceneCollection, openedScenesOnly, progressCallback);
            }
            CartographerCommon.CleanupMemory();
            progressCallback?.OnEnd();
            return true;
        }
    }
};