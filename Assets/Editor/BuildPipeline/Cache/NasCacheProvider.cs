using System;
using System.IO;
using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.Test.Src.Editor
{
    /// <summary>
    /// Наивная имплементация кеша на сетевой папке
    /// </summary>
    public class NasCacheProvider : IRemoteCacheProvider
    {
        private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");

        public readonly string nasFolder = "\\\\nas.enplexgames.com\\QA\\BuildCache";
        
        public string CurrentCacheVersion => GitCacheStatus.CurrentCacheVersion();

        public bool Check(string name)
        {
            var cacheVersion = CurrentCacheVersion;
            if (string.IsNullOrWhiteSpace(cacheVersion))
                return false;

            if (!Directory.Exists(nasFolder)) return false;
            var cacheFolderPath = nasFolder + "\\" + cacheVersion;
            return Directory.Exists(cacheFolderPath) && File.Exists($"{cacheFolderPath}\\{name}");
        }

        public void Clean()
        {
            if (!Directory.Exists(nasFolder))
                return;

            try
            {
                var directories = Directory.GetDirectories(nasFolder);
                foreach (var directory in directories)
                {
                    var info = new DirectoryInfo(directory);
                    if (info.LastAccessTime < DateTime.Now.AddMonths(-1))
                        Directory.Delete(directory);
                }
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message($"Cannot remove cache from nas because of: {e.Message}").Write();
                return;
            }
        }

        public void Upload(string name, string serialized)
        {
            try
            {
                var cacheVersion = CurrentCacheVersion;
                if (string.IsNullOrWhiteSpace(cacheVersion))
                    return;

                if (!Directory.Exists(nasFolder)) return;
                var cacheFolderPath = nasFolder + "\\" + cacheVersion;
                if (!Directory.Exists(cacheFolderPath))
                    Directory.CreateDirectory(cacheFolderPath);

                var filePath = $"{cacheFolderPath}\\{name}";
                var fileInfo = new FileInfo(filePath);
                if (!File.Exists(filePath) || !IsFileLocked(fileInfo))
                {
                    File.WriteAllText(filePath, serialized);
                }
                else
                {
                    Logger.IfWarn()?.Message($"IO Exception: Cannot save cache to nas because file is locked.").Write();
                }
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message($"Cannot upload cache to nas because of: {e.Message}").Write();
                return;
            }
        }

        public string Download(string name)
        {
            try
            {
                var cacheVersion = CurrentCacheVersion;
                if (string.IsNullOrWhiteSpace(cacheVersion))
                    return "";

                if (!Directory.Exists("\\\\nas\\QA\\BuildCache"))
                    return "";
                var cacheFolderPath = nasFolder + "\\" + cacheVersion;
                if (!Directory.Exists(cacheFolderPath) || !File.Exists($"{cacheFolderPath}\\{name}"))
                    return "";
                else
                {
                    var filePath = $"{cacheFolderPath}\\{name}";
                    var fileInfo = new FileInfo(filePath);
                    if (!IsFileLocked(fileInfo))
                        return File.ReadAllText($"{cacheFolderPath}\\{name}");
                    else
                    {
                        Logger.IfWarn()?.Message($"IO Exception: Cannot load cache from nas because file is locked.").Write();
                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfWarn()?.Message($"Cannot download cache from nas because of: {e.Message}").Write();
                return "";
            }
        }

        public bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}