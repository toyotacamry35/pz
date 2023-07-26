using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.App;
using Assets.Src.BuildScripts;
using NLog;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Player;
using BuildCompression = UnityEngine.BuildCompression;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public class AssetBundleBuildPipeline
    {
        private readonly Logger _logger = LogManager.GetLogger("BuildProcess");

        private readonly PipelineConfigurationContext _pipelineConfigurationContext;

        private IBundleBuildParameters _parameters;
        private IBundleBuildContent _content;

        private IContextObject[] _contextObjects;
        private IBuildPipelineSettings _settings;
        private readonly IProfileContext _profileContext;

        public AssetBundleBuildPipeline(string mapName, DependencyCollectType dependencyCollectType, BuildConfig config)
        {
            var packName = dependencyCollectType == DependencyCollectType.Map
                ? AssetBundleHelper.ProduceMapBundleFolderName(mapName)
                : mapName;
            var bundleDir = Path.Combine(config.BundlesFolderPath, packName);
            var sharedDir = Path.Combine(config.BundlesFolderPath, AssetBundleHelper.SharedName);
            _pipelineConfigurationContext = new PipelineConfigurationContext(
                mapName,
                bundleDir,
                sharedDir,
                dependencyCollectType,
                config);
            _profileContext = new ProfileContext(config.BundlesFolderPath, mapName);
        }

        public void Setup()
        {
            _settings = BuildPipelineManager.GetBuildPipelineSettings();

            if (Directory.Exists(_pipelineConfigurationContext.TempPath))
                Directory.Delete(_pipelineConfigurationContext.TempPath, true);

            Directory.CreateDirectory(_pipelineConfigurationContext.TempPath);

            _parameters = new BundleBuildParameters(
                BuildTarget.StandaloneWindows64,
                BuildTargetGroup.Standalone,
                _pipelineConfigurationContext.TempPath)
            {
                BundleCompression = BuildCompression.Uncompressed,
                ScriptOptions = ScriptCompilationOptions.Assertions | ScriptCompilationOptions.DevelopmentBuild,
                UseCache = _settings.UseBuildCache,
                ContentBuildFlags = ContentBuildFlags.StripUnityVersion
            };

            //Not supported right now with Accelerator and i do not want to use it with cache server v1
            if (_settings.UseCacheServer)
            {
                //_parameters.CacheServerHost = "http://10.0.32.83";
                //_parameters.CacheServerPort = 10080;
            }

            _content = new EmptyBundleBuildContent();
            _contextObjects = new IContextObject[]
            {
                _pipelineConfigurationContext,
                new CollectorContext
                {
                    ProjectDependencies = new Dictionary<string, AssetBundleBuild[]>(),
                    SharedBundleToMapReferences = new Dictionary<string, AssetBundleReference>(),
                    SharedMapToBundleReferences = new Dictionary<string, MapBundleReference>()
                },
                new PrefabPackedIdentifiers(), _settings, _profileContext
            };
        }

        private IList<IBuildTask> CollectTasks()
        {
            var buildTasks = new List<IBuildTask>();


            //Collect All Dependencies
            buildTasks.Add(new CollectAllDependencyTask());
            
            buildTasks.Add(new ReplaceBuiltinShaders());
            
            buildTasks.Add(new FilterAllDependencies());
            buildTasks.Add(new WriteDependenciesCatalog());

            if (_settings.ReleaseMapNames.Contains(_pipelineConfigurationContext.MapName) || _pipelineConfigurationContext.MapName == AssetBundleHelper.SystemName)
            {
                buildTasks.Add(new ReleaseDependencyCollectTask());
            }
            else if (_pipelineConfigurationContext.MapName == AssetBundleHelper.SharedName)
            {
                buildTasks.Add(new SharedDependencyCollectTask());
            }
            else
            {
                buildTasks.Add(new TestDependencyCollectTask());
            }

            //Setup
            buildTasks.Add(new SwitchToBuildPlatform());
            buildTasks.Add(new RebuildSpriteAtlasCache());

            // Recalculate Dependency Data
            buildTasks.Add(new CalculateSceneDependencyData());
            buildTasks.Add(new CalculateAssetDependencyData());

            if (_settings.ReleaseMapNames.Contains(_pipelineConfigurationContext.MapName) ||
                _pipelineConfigurationContext.MapName == AssetBundleHelper.SystemName ||
                _pipelineConfigurationContext.MapName == AssetBundleHelper.SharedName)
                buildTasks.Add(new CreateBuiltInShadersBundle(AssetBundleHelper.ProduceSharedBuiltinShaderBundleName()));
            else
                buildTasks.Add(new UnityEditor.Build.Pipeline.Tasks.CreateBuiltInShadersBundle(AssetBundleHelper.ProduceBuiltinShaderBundleName(_pipelineConfigurationContext.MapName)));

            buildTasks.Add(new StripUnusedSpriteSources());
            buildTasks.Add(new PostDependencyCallback());

            // Packing
            buildTasks.Add(new GenerateBundlePacking());

            buildTasks.Add(new UpdateBundleObjectLayout());

            buildTasks.Add(new GenerateBundleCommands());
            buildTasks.Add(new GenerateSubAssetPathMaps());
            buildTasks.Add(new GenerateBundleMaps());
            buildTasks.Add(new PostPackingCallback());

            // Writing
            buildTasks.Add(new FilterWriteSerializedFiles());
            buildTasks.Add(new WriteSerializedFiles());
            buildTasks.Add(new ArchiveAndCompressBundles());
            buildTasks.Add(new AppendBundleHash());
            buildTasks.Add(new PostWritingCallback());
            
            if (_pipelineConfigurationContext.MapName == AssetBundleHelper.SharedName)
                buildTasks.Add(new TempSharedBundleCheckTask());

            //Cache
            buildTasks.Add(new MoveBundlesTask());

            return buildTasks;
        }

        public int BuildBundle()
        {
            var startTime = DateTime.Now;
            _profileContext.Profiler.Log($"Build process {_pipelineConfigurationContext.MapName}, started time: '{startTime}'");

            var tasks = CollectTasks();
            var code = ContentPipeline.BuildAssetBundles(_parameters, _content, out var buildReport, tasks, _contextObjects);
            
            _profileContext.Profiler.SaveProfileInfo("report",buildReport);

            if (code < ReturnCode.Success)
            {
                _logger.Error("Build process failed with error {0}", code);
                return 1;
            }

            var endTime = DateTime.Now;
            _profileContext.Profiler.Log(
                $"Build process {_pipelineConfigurationContext.MapName} finished successfully, started time: '{startTime}', ended time: '{endTime}', elapsed time: '{endTime - startTime}'");
            
            return 0;
        }
    }
}