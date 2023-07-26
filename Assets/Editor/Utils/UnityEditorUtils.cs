using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class UnityEditorUtils
{
    /// <summary>
    /// Возвращает полный путь до папки Library
    /// </summary>
    /// <returns>Полный путь</returns>
    /// <exception cref="InvalidOperationException">Выбрасывает ошибку если не может найти папку Library</exception>
    public static string LibraryPath()
    {
        var assetFolder = new DirectoryInfo(Application.dataPath);
        var libraryFolder = assetFolder.Parent?.GetDirectories("Library").Single().FullName;
        if (string.IsNullOrWhiteSpace(libraryFolder))
        {
            throw new InvalidOperationException("Cannot find library folder.");
        }

        return libraryFolder;
    }

    /// <summary>
    /// Get the absolute path from a local path.
    /// </summary>
    /// <param name="localPath">The path contained within the "Assets" direction. Eg: "Models/Model.fbx"</param>
    /// <example>Passing "Models/Model.fbx" will return "C:/Users/John/MyProject/Assets/Models/Model.fbx"</example>
    /// <returns>Returns the absolute/full system path from the local "Assets" inclusive path.</returns>
    public static string GetAbsolutePathFromLocalPath(string localPath)
    {
        return Application.dataPath.Remove(Application.dataPath.Length - 6, 6) + localPath;
    }

    /// <summary>
    /// Get the local path from a absolute path.
    /// </summary>
    /// <param name="absolutePath">the absolute/full system path from the local "Assets" inclusive path.</param>
    /// <example>Passing "C:/Users/John/MyProject/Assets/Models/Model.fbx" will return "Assets/Models/Model.fbx"</example>
    /// <returns>Returns the path contained starts from "Assets". Eg: "Assets/Models/Model.fbx"</returns>
    public static string GetLocalPathFromLocalPath(string absolutePath)
    {
        return absolutePath.Remove(0, Application.dataPath.Remove(Application.dataPath.Length - 6, 6).Length);
    }


    /// <summary>
    /// Возвращает полный путь до папки кеша в Library. Если её не существует, то создаёт.
    /// </summary>
    /// <returns>Полный путь папки кеша</returns>
    /// <exception cref="InvalidOperationException">Выбрасывает ошибку если не может найти папку Library или имя папки образует невалидный путь.</exception>
    public static string LocalCachePath(string cacheFolderName)
    {
        if (string.IsNullOrWhiteSpace(cacheFolderName))
            throw new InvalidOperationException("cacheFolderName cannot be null or whitespaces");

        var cacheFolder = $"{LibraryPath()}/{cacheFolderName}";
        try
        {
            Path.GetFullPath(cacheFolder);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Path: {cacheFolder} is invalid path");
        }

        if (!Directory.Exists(cacheFolder))
            Directory.CreateDirectory(cacheFolder);

        return cacheFolder;
    }
}