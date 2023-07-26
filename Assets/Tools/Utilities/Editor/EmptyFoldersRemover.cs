//Made by Vitaly Bulanenkov
//while working at and for Enplex Games
//in 2017
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class RemoveEmptyFolders
{
    [MenuItem("Tools/Remove empty folders", false, 40)]
    private static void RemoveEmptyFoldersMenuItem()
    {
        var index = Application.dataPath.IndexOf("/Assets");
        var projectSubfolders = Directory.EnumerateDirectories(Application.dataPath, "*", SearchOption.AllDirectories);

        // Create a list of all the empty subfolders under Assets.
        var emptyFolders = projectSubfolders.Where(path => IsEmptyRecursive(path)).ToArray();

        foreach (var folder in emptyFolders)
        {
            // Verify that the folder exists (may have been already removed).
            if (Directory.Exists(folder))
            {
                // Remove dir (recursively)
                Directory.Delete(folder, true);

                // Sync AssetDatabase with the delete operation.
                AssetDatabase.DeleteAsset(folder.Substring(index + 1));
            }
        }

        // Refresh the asset database once we're done.
        AssetDatabase.Refresh();
        Debug.Log(emptyFolders.Length == 0 ? "No empty folders found" : $"Deleted empty folders: {emptyFolders.Aggregate((i,j) => $"{i},\n{j}")}");
    }
    /// <summary>
    /// A helper method for determining if a folder is empty or not.
    /// </summary>
    private static bool IsEmptyRecursive(string path)
    {
        // A folder is empty if it (and all its subdirs) have no files (ignore .meta files)
        return !Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Any(file => !file.EndsWith(".meta"));
    }
}
