using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Assets.Src.ResourceSystem.Editor
{
    [ScriptedImporter(15, "jdb", 10)]
    public class JsonCustomImporter : ScriptedImporter
    {
        private GameResources gameResources;
        private string _path;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            _path = ctx.assetPath;
            _path = _path.Substring("Assets".Length);
            _path = _path.Substring(0, _path.Length - ".jdb".Length);

            var metadata = ScriptableObject.CreateInstance<JdbMetadata>();
            metadata.RootPath = _path;
            metadata.name = "Metadata";

            ctx.AddObjectToAsset("Metadata", metadata);
            ctx.SetMainObject(metadata);

            var refCollector = new RefCollector(_path);

            metadata.Type = refCollector.GetTypeAndCollect();

            metadata.UnityRefs = refCollector.UnityRefs.Distinct().ToArray();

            foreach (var brokenRef in refCollector.BrokenUnityRefs)
            {
                //юнити почему-то не выводит логи ошибок
                //ctx.LogImportError($"Broken Unity link from {_path} to {brokenRef.Item1}({brokenRef.Item2})");
                Debug.LogError($"Broken Unity link from {_path} to {brokenRef.Item1}({brokenRef.Item2})");
            }

            foreach (var brokenRef in refCollector.BrokenDefRefs)
            {
                //юнити почему-то не выводит логи ошибок
                //ctx.LogImportError($"Broken Def link from {_path} to {brokenRef.Item1}({brokenRef.Item2})");
                Debug.LogError($"Broken Def link from {_path} to {brokenRef.Item1}({brokenRef.Item2})");
            }

            if (!Application.isPlaying)
            {
                GameResourcesHolder.Instance.Reload(metadata.RootPath);
            }
        }

        [MenuItem("Build/Reimport JDB")]
        public static void ReimportAll()
        {
            var assets = AssetDatabase.FindAssets("").Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith(".jdb")).ToArray();

            var filesToReimport = new List<string>();

            foreach (var asset in assets)
            {
                var progress = (float) System.Array.IndexOf(assets, asset) / assets.Length;
                if (EditorUtility.DisplayCancelableProgressBar(
                    $"Working, {System.Array.IndexOf(assets, asset)}/{assets.Length}",
                    $"Checking {asset}", progress))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }

                var path = asset;
                path = path.Substring("Assets".Length);
                path = path.Substring(0, path.Length - ".jdb".Length);

                var meta = AssetDatabase.LoadAssetAtPath<JdbMetadata>(asset);

                if (meta == null)
                {
                    Debug.LogWarning($"Broken structure of jdb {asset}, reimporting", meta);
                    filesToReimport.Add(asset);
                    continue;
                }

                var refCollector = new RefCollector(path);
                var type = refCollector.GetTypeAndCollect();

                if (meta.Type != type)
                {
                    Debug.LogWarning($"Invalid root type of jdb {asset}, reimporting", meta);
                    filesToReimport.Add(asset);
                    continue;
                }

                var importer = GetAtPath(asset) as JsonCustomImporter;
                if (importer == null)
                {
                    Debug.LogWarning($"Broken link to importer of jdb {asset}, reimporting", meta);
                    filesToReimport.Add(asset);
                    continue;
                }

                var newUnityRefs = refCollector.UnityRefs.Distinct().ToArray();

                if (Enumerable.SequenceEqual(newUnityRefs, meta.UnityRefs) && !refCollector.BrokenDefRefs.Any() &&
                    !refCollector.BrokenUnityRefs.Any())
                    continue;

                Debug.LogWarning($"Incorrect refs of jdb {asset}, reimporting", meta);
                filesToReimport.Add(asset);
            }

            if (!filesToReimport.Any())
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            AssetDatabase.StartAssetEditing();
            foreach (var asset in filesToReimport)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Working, {filesToReimport.IndexOf(asset)}/{filesToReimport.Count}",
                    $"Reimporting {asset}", (float) filesToReimport.IndexOf(asset) / filesToReimport.Count))
                {
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.StopAssetEditing();
                    return;
                }

                AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
        }
    }

    public class RefCollector
    {
        private readonly string _root;

        public List<ReferenceMapping> UnityRefs { get; } = new List<ReferenceMapping>();
        public HashSet<ValueTuple<string, Type>> BrokenUnityRefs { get; } = new HashSet<ValueTuple<string, Type>>();

        private readonly HashSet<ValueTuple<string, Type>> _rawDefRefs = new HashSet<ValueTuple<string, Type>>();
        public HashSet<ValueTuple<string, Type>> DefRefs { get; } = new HashSet<ValueTuple<string, Type>>();
        public HashSet<ValueTuple<string, Type>> BrokenDefRefs { get; } = new HashSet<ValueTuple<string, Type>>();

        private GameResources gameResources;

        public RefCollector(string root)
        {
            _root = root;
        }

        public string GetTypeAndCollect()
        {
            try
            {
                gameResources = new GameResources(new FolderLoader(Application.dataPath));
                gameResources.ConfigureForUnityImport();

                UnityRuntimeObjectConverter.OnUnityRefFound += OnUnityRefFound;
                DefReferenceConverter.OnDefReferenceFound += OnDefRefFound;

                var tgt = gameResources.LoadResource<IResource>(_root);
                var type = tgt.GetType().FullName;

                UnityRuntimeObjectConverter.OnUnityRefFound -= OnUnityRefFound;
                DefReferenceConverter.OnDefReferenceFound -= OnDefRefFound;
                ValidateDefRefs();

                return type;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}: {1}", _root, e);
            }
            finally
            {
                UnityRuntimeObjectConverter.OnUnityRefFound -= OnUnityRefFound;
                DefReferenceConverter.OnDefReferenceFound -= OnDefRefFound;
            }

            return "";
        }

        private void ValidateDefRefs()
        {
            var refs = _rawDefRefs.ToArray();
            foreach (var dRef in refs)
            {
                try
                {
                    var tgt = gameResources.LoadResource(dRef.Item1, dRef.Item2);
                    if (tgt == null)
                    {
                        BrokenDefRefs.Add(dRef);
                    }
                    else if (!dRef.Item2.IsInstanceOfType(tgt))
                    {
                        GameResources.ThrowError($"{tgt.Address.ToString()} ({tgt.GetType()}) is not instance of {dRef.Item2}");
                        BrokenDefRefs.Add(dRef);
                    }
                    else
                    {
                        DefRefs.Add(dRef);
                    }
                }
                catch (Exception e)
                {
                    GameResources.ThrowError(e.Message);
                    BrokenDefRefs.Add(dRef);
                }
            }
        }

        private void OnUnityRefFound(string documentPosition, string path, Type targetType)
        {
            try
            {
                if (!documentPosition.Contains(_root))
                    return;

                var resolved =
                    AssetDatabase.GetAssetPath(Assets.Editor.ResourceSystem.EditorTimeResolver.Instance.Resolve(path, targetType));
                UnityRefs.Add(new ReferenceMapping(path, resolved));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                BrokenUnityRefs.Add(ValueTuple.Create(path, targetType));
            }
        }

        private void OnDefRefFound(string path, Type refType)
        {
            if (!_root.Contains(gameResources.Context.RootAddress))
                return;

            _rawDefRefs.Add(ValueTuple.Create(path, refType));
        }
    }
}