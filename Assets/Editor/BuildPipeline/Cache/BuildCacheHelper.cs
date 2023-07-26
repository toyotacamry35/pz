using System;
using System.IO;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using NLog;

namespace Assets.Test.Src.Editor
{
    public static class BuildCacheHelper
    {
        private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");

        public static readonly string CacheFolderName = "CustomBuildCache";

        public static IRemoteCacheProvider remoteCacheProvider = new NasCacheProvider();

        public static string ProducePath(string name) => $"{UnityEditorUtils.LocalCachePath(CacheFolderName)}/{name}.json";

        public static bool IsCacheExist(string name)
        {
            var path = ProducePath(name);
            return File.Exists(path) ||
                   (BuildPipelineManager.GetBuildPipelineSettings().RemoteDependencyCache && remoteCacheProvider.Check(name));
        }

        public static bool WriteCache<T>(string name, T cachingObject)
        {
            var fullPath = ProducePath(name);
            if (File.Exists(fullPath))
                Logger.IfInfo()?.Message($"Cached file {fullPath} will be ovveriden").Write();

            var serialized = JsonConvert.SerializeObject(cachingObject);

            try
            {
                File.WriteAllText(fullPath, serialized);
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message($"Cannot write local cache because of: {e.Message}").Write();
                return false;
            }

            if (BuildPipelineManager.GetBuildPipelineSettings().RemoteDependencyCache)
                remoteCacheProvider.Upload(name, serialized);
            return true;
        }

        public static T ReadCache<T>(string name)
        {
            var fullPath = ProducePath(name);
            if (File.Exists(fullPath))
            {
                try
                {
                    var serialized = File.ReadAllText(fullPath);
                    return JsonConvert.DeserializeObject<T>(serialized);
                }
                catch (Exception e)
                {
                    Logger.IfWarn()?.Message($"Cannot use cache because of: {e.Message}").Write();
                    return default;
                }
            }

            if (!BuildPipelineManager.GetBuildPipelineSettings().RemoteDependencyCache)
                return default;

            var nasCache = remoteCacheProvider.Download(name);
            return string.IsNullOrWhiteSpace(nasCache) ? default : JsonConvert.DeserializeObject<T>(nasCache);
        }

        public static bool RemoveCache(string name)
        {
            var fullPath = ProducePath(name);
            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }

        public static bool RemoveCacheAll()
        {
            var cacheFolder = new DirectoryInfo(UnityEditorUtils.LocalCachePath(CacheFolderName));
            cacheFolder.Delete(true);

            if (BuildPipelineManager.GetBuildPipelineSettings().RemoteDependencyCache)
                remoteCacheProvider.Clean();

            return true;
        }
    }
}