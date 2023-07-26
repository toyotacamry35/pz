using System;
using Newtonsoft.Json;
using System.IO;
using ResourcesSystem.Loader;
using UnityEditor;
using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.Converters;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;

public static class GameResourcesLikeFileSaver
{

    public static JdbMetadata GetFile(string dir, string refPath)
    {
        string path;
        if (dir[dir.Length - 1] != '/' && refPath[0] != '/')
            path = dir + "/" + refPath + ".jdb";
        else
            path = dir + refPath + ".jdb";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        var assetPath = path.Substring(path.IndexOf("Assets", StringComparison.InvariantCulture));
        return AssetDatabase.LoadAssetAtPath<JdbMetadata>(assetPath);
    }

    public static string GetFilePath(IResource data)
    {
        var root = data.Address.Root;
        return "Assets" + root;
    }

    public static JdbMetadata SaveFile(IResource data)
    {
        var root = data.Address.Root;
        var index = root.LastIndexOf('/');
        var dir = "Assets" + root.Substring(0, index);
        var refPath = root.Substring(index + 1);
        return SaveFile(dir, refPath, data);
    }

    public static JdbMetadata SaveFile(string dir, string refPath, IResource data)
    {
        string path;
        GameResources.SimpleSave(dir, refPath, data, out path, UnityRuntimeRefConverter.Instance);
        var assetPath = path.Substring(path.IndexOf("Assets", StringComparison.InvariantCulture));
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<JdbMetadata>(assetPath);
    }

    public static void Clear(string dir, string refPath, IResource data)
    {
        GameResources.SimpleSave(dir, refPath, null, out var path, UnityRuntimeRefConverter.Instance);
    }
}
