using System;
using System.IO;
using System.Linq;
using ResourcesSystem.Loader;
using UnityEngine;

namespace Assets.Src.BuildScripts.Editor
{
    class GameResourcesUpdate
    {
        public static void CopyGameResources(string targetFolder)
        {
            var location = targetFolder;
            foreach (string dirPath in Directory.GetDirectories(Application.dataPath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Application.dataPath, location));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories)
                .Where(s => FolderLoader.Extensions.Contains(Path.GetExtension(s)))
            )
                File.Copy(newPath, newPath.Replace(Application.dataPath, location), true);

            foreach (string newPath in Directory.GetFiles(Path.Combine(Application.dataPath, "System"), "*.xml",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Application.dataPath, location), true);

            DeleteEmptyDirs(location);
            Debug.Log("Finish copy game resources");
        }
        static void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string",
                    "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
