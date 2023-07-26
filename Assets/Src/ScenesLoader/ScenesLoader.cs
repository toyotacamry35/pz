using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NLog;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using Assets.Src.Server.Impl;
using System;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Logging;
using SharedCode.Serializers;

namespace Assets.Src.Scenes
{
    public static class ScenesLoader
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static AsyncOperation _operation;


        //=== Props ===============================================================

        private static readonly ConcurrentDictionary<string, Task> _loadingScenes = new ConcurrentDictionary<string, Task>();

        //=== Public ==============================================================

        public static async Task<bool[]> LoadScenesAsync(IEnumerable<string> sceneNames)
        {
            List<bool> results = new List<bool>();
            foreach (var scene in sceneNames)
                if (!string.IsNullOrEmpty(scene))
                    results.Add(await LoadSceneRefAsync(scene));
            return results.ToArray();
        }
        
        //Not call this function out of main thread
        public static void ClearAfterSceneLoading()
        {
//             TaskCompletionSource<AsyncOperation> tcs =
//                 new TaskCompletionSource<AsyncOperation>(TaskCreationOptions.RunContinuationsAsynchronously);
//
//             await UnityQueueHelper.RunInUnityThread(() =>
//             {
// #if UNITY_EDITOR
//                 //UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();
// #endif
//                 //Resources.UnloadUnusedAssets().completed += (op) => tcs.SetResult(op);
//             });
//             //await tcs.Task;
            /*if (_operation==null || _operation.isDone)
            {
                _operation = Resources.UnloadUnusedAssets();
                _operation.completed += (op) => System.GC.Collect();
            }*/
            System.GC.Collect();
        }

        public static async Task UnloadScenesAsync(IEnumerable<string> sceneNames)
        {
            var tasks = sceneNames.Where(s => !string.IsNullOrEmpty(s)).Select(s => UnloadSceneRefAsync(s));
            await Task.WhenAll(tasks);
        }

        public static SuspendingAwaitable<bool> IsLoadedScene(string scene)
        {
            //Attention: if scene isn't loaded, GetSceneByPath() returns not filled Scene struct
            return UnityQueueHelper.RunInUnityThread(() => SceneManager.GetSceneByPath(scene).isLoaded);
        }

        private static async Task<AsyncOperation> LoadSceneAsync(TaskCompletionSource<AsyncOperation> tcs, string scenePath, LoadSceneMode mode)
        {
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                var opc = SceneManager.LoadSceneAsync(scenePath, mode);
                if (opc == null)
                {
                    tcs.SetResult(null);
                    return;
                }
                opc.completed += (op) => tcs.SetResult(op);
            });
            return await tcs.Task;
        }

        private static async Task<AsyncOperation> UnloadSceneAsync(string scenePath)
        {
            TaskCompletionSource<AsyncOperation> tcs =
                new TaskCompletionSource<AsyncOperation>(TaskCreationOptions.RunContinuationsAsynchronously);

            await UnityQueueHelper.RunInUnityThread(() => { SceneManager.UnloadSceneAsync(scenePath).completed += (op) => tcs.SetResult(op); });
            return await tcs.Task;
        }

        private static async Task<bool> LoadSceneRefAsync(string scene)
        {
            Logger.IfInfo()?.Message($"Loading {scene}...").Write();

            if (string.IsNullOrWhiteSpace(scene))
            {
                 Logger.IfError()?.Message("Scene is empty").Write();;
                return false;
            }

            if (!IsSceneInBuildSettings(scene))
            {
                Logger.IfError()?.Message($"{scene}: Scene is not present in build settings").Write();
                return false;
            }

            if (await IsLoadedScene(scene))
                return false;

            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>(TaskCreationOptions.RunContinuationsAsynchronously);

            var task = _loadingScenes.GetOrAdd(scene, tcs.Task);

            var watch = Stopwatch.StartNew();

            if (task == tcs.Task)
            {
                await LoadSceneAsync(tcs, scene, LoadSceneMode.Additive);
                _loadingScenes.TryRemove(scene, out task);
            }
            else
                await task;

            watch.Stop();

            if (await IsLoadedScene(scene))
            {
                Logger.IfInfo()?.Message($"{scene} is loaded successfully in {watch.Elapsed.TotalSeconds:f2} sec").Write();
                return task == tcs.Task;
            }
            else
            {
                Logger.IfError()?.Message($"{scene} is failed to load in {watch.Elapsed.TotalSeconds:f2} sec").Write();
                return false;
            }
        }

        private static async Task UnloadSceneRefAsync(string scene)
        {
            if (string.IsNullOrWhiteSpace(scene))
            {
                 Logger.IfError()?.Message("Scene is empty").Write();;
                return;
            }

            if (!IsSceneInBuildSettings(scene))
            {
                Logger.IfError()?.Message($"{scene}: Scene is not present in build settings").Write();
                return;
            }

            if (!await IsLoadedScene(scene))
            {
                Logger.IfWarn()?.Message($"{scene}: is not loaded").Write();
                return;
            }

            var watch = Stopwatch.StartNew();

            await UnloadSceneAsync(scene);

            watch.Stop();
            if (!await IsLoadedScene(scene))
            {
                Logger.IfInfo()?.Message($"{scene} is unloaded successfully in {watch.Elapsed.TotalSeconds:f2} sec").Write();
            }
            else
            {
                Logger.IfError()?.Message($"{scene} failed to unload in {watch.Elapsed.TotalSeconds:f2} sec").Write();
            }
        }

        public static bool IsSceneInBuildSettings(string scenePath)
        {
            return ScenesInBuild.Scenes.Contains(scenePath);
        }

        //=== Private =============================================================
    }

    public static class ScenesInBuild
    {
        [NotNull]
        public static IReadOnlyList<string> Scenes { get; set; } = ScenesFromSceneManager().ToArray();

        private static IEnumerable<string> ScenesFromSceneManager() => Enumerable.Range(0, SceneManager.sceneCountInBuildSettings).Select(SceneUtility.GetScenePathByBuildIndex);
    }
}