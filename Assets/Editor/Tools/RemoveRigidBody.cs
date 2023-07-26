using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Tools.Editor
{
    public static class RemoveRigidBody
    {
        [MenuItem("Tools/Remove Rigid Body")]
        public static void Do()
        {
            var pathes = new List<string>();
            foreach (var obj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                if (!string.IsNullOrEmpty(path))
                {
                    if (Directory.Exists(path))
                        GatherPathes(path, pathes);
                    else if (path.EndsWith(".prefab"))
                        pathes.Add(path);
                    else
                        Debug.LogFormat("Not a prefab: {0}", path);
                }
            }
            foreach (var path in pathes)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                FindAndRemoveRigidBody(go.transform, path);
                EditorUtility.SetDirty(go);
            }
            AssetDatabase.SaveAssets();
        }

        private static void GatherPathes(string root, List<string> pathes)
        {
            var dir = new DirectoryInfo(root);
            var dataPath = CanonicalizePath(Application.dataPath);
            foreach (var file in dir.GetFiles())
            {
                var relPath = CanonicalizePath(file.FullName);
                relPath = relPath.Replace(dataPath, "");
                relPath = "Assets" + relPath;
                if (relPath.EndsWith(".prefab"))
                    pathes.Add(relPath);
            }

            foreach (var subdir in AssetDatabase.GetSubFolders(root))
                GatherPathes(subdir, pathes);
        }

        private static void FindAndRemoveRigidBody(Transform root, string path)
        {
            foreach (var rb in root.GetComponents<Rigidbody>())
            {
                Debug.LogFormat("Remove rigid body from '{0}'", path);
                GameObject.DestroyImmediate(rb, true);
            }
            foreach (Transform child in root)
                FindAndRemoveRigidBody(child, path + "/" + child.name);
        }

        private static string CanonicalizePath(string path)
        {
            return Path.GetFullPath(path).Replace("\\", "/").Replace("//", "/");
        }
    }
}