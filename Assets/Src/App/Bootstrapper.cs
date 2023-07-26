using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.App;
using Assets.Src.BuildScripts;
using Assets.Src.Scenes;
using Core.Environment.Logging.Extension;
using NLog;
using Uins;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = NLog.Logger;

public class Bootstrapper : MonoBehaviour
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string MainSceneName = "Main";

    public BuildConfig BuildConfig;

    private static string _assetBundleRootPath { get; set; } = null;

    private static AssetBundleResolver _bundleResolver;

    private static readonly Dictionary<string, AssetBundleCreateRequest> _loadedBundles = new Dictionary<string, AssetBundleCreateRequest>();

    public void Start()
    {
        //DevelopLobbyGuiNode.Init_Log_System();

        _assetBundleRootPath = BuildConfig.RuntimeBundlesFolderPath;
        LoadSystem(MainSceneName).WrapErrors();
    }

    private static Task<AssetBundleCreateRequest> GetOrLoad(string path)
    {
        AssetBundleCreateRequest request;
        if (_loadedBundles.TryGetValue(path, out request))
        {
            return Task.FromResult(request);
        }

        var tcs = new TaskCompletionSource<AssetBundleCreateRequest>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            request = AssetBundle.LoadFromFileAsync(path);
            request.completed += (v) =>
            {
                if (request.assetBundle == null) throw new Exception($"Can't load asset bundle '{path}'");
                _loadedBundles.Add(path, request);
                var scenes = request.assetBundle.GetAllScenePaths();
                ScenesInBuild.Scenes = ScenesInBuild.Scenes.Concat(scenes).ToArray();
                tcs.SetResult(request);
            };
        }
        catch (Exception e)
        {
            tcs.SetException(e);
        }

        return tcs.Task;
    }

    private static async Task LoadBundle(string path)
    {
         Logger.IfInfo()?.Message("Loading asset bundle {0}",  path).Write();
        try
        {
            var loadOp = await UnityQueueHelper.RunInUnityThread(() => GetOrLoad(path));
        }
        catch (Exception e)
        {
            Logger.IfError()?.Message(e, "Cant load asset bundle {0}", path).Write();
        }
    }

    public static async Task LoadMapFolder(string folder)
    {
        await LoadBundlesInFolder(folder);
    }

    private static async Task LoadBundlesInFolder(string folder)
    {
        if (_assetBundleRootPath == null)
            return;
        string whereToSearchForAssets = Path.Combine(_assetBundleRootPath, folder);
        if (!Directory.Exists(whereToSearchForAssets))
            throw new DirectoryNotFoundException($"Directory {whereToSearchForAssets} not found");

        var bundles = Directory.GetFiles(whereToSearchForAssets, "*.bundle", SearchOption.AllDirectories).ToList();

        await Task.WhenAll(bundles.Select(LoadBundle));
    }

    public static async Task PrepareResolver()
    {
        _bundleResolver = new AssetBundleResolver();
        UnityObjectResolverHolder.Instance = new RuntimeObjectResolver(_bundleResolver);

        await _bundleResolver.Load(_assetBundleRootPath);
        _bundleResolver.Prepare(_loadedBundles, _assetBundleRootPath);
    }

    public static async Task LoadSharedFolder()
    {
        await LoadBundlesInFolder(AssetBundleHelper.SharedName);
    }

    private static async Task LoadSystem(string mainScene)
    {
        if (await UnityQueueHelper.RunInUnityThread(() => Application.isEditor))
        {
            await AwaitExtensions.AwaitAsyncOp(() => SceneManager.LoadSceneAsync(mainScene));
            return;
        }

        await UnityQueueHelper.RunInUnityThread(
            () =>
            {
                AssetBundle.UnloadAllAssetBundles(true);
                ScenesInBuild.Scenes = Array.Empty<string>();
            });

        await LoadSharedFolder();
        await LoadBundlesInFolder(AssetBundleHelper.SystemName);
        await PrepareResolver();

        await UnityQueueHelper.RunInUnityThread(
            () => { });

        await AwaitExtensions.AwaitAsyncOp(() => SceneManager.LoadSceneAsync(mainScene));
    }
}