using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using NLog;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using Logger = NLog.Logger;
using Object = UnityEngine.Object;

public class ReferenceBacktracer : EditorWindow
{
    static readonly GUID k_BuiltInGuid = new GUID("0000000000000000f000000000000000");
    private static Logger Logger = LogManager.GetCurrentClassLogger();

    private Dictionary<string, IEnumerable<JdbMetadata>> _jdbDependencies;
    private Dictionary<string, IEnumerable<string>> _assetDependencies;
    private IEnumerable<(string, ObjectIdentifier[])> _defaultDependencies;
    private IEnumerable<string> _allAssets;
    private bool _excludeScenes;
    private bool _useDiskCache;
    private bool _filtered;
    private Object obj;

    private string CachePath() => $"{UnityEditorUtils.LibraryPath()}/dependency.json";
    private bool IsCacheExist() => File.Exists(CachePath());


    [MenuItem("Tools/Backtrace reference")]
    private static void MenuItem()
    {
        var window = GetWindow<ReferenceBacktracer>("Reference backtrace");
        window.position = new Rect(100, 100, 500, 300);
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Collect JDB dependencies"))
                    CollectJdbDependencies();
            }

            if (_jdbDependencies != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Print JDB dependencies to console"))
                        PrintDependencies(_jdbDependencies);
                }
            }
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField("Asset dependency backtrace");

            using (new EditorGUILayout.HorizontalScope())
            {
                _excludeScenes = EditorGUILayout.ToggleLeft("Exclude scenes", _excludeScenes);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _useDiskCache = EditorGUILayout.ToggleLeft("Use Disk Cache", _useDiskCache);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                var buttonText = _assetDependencies != null ? "Update asset dependencies" : "Collect asset dependencies";

                if (GUILayout.Button(buttonText))
                {
                    CollectAssetDependencies();
                    if (_useDiskCache && _assetDependencies != null)
                        SaveDependencyToCache(_assetDependencies);
                }

                if (_assetDependencies == null && _useDiskCache && IsCacheExist() && GUILayout.Button("Load dependencies from cache"))
                    _assetDependencies = LoadDependencyFromCache();
            }

            if (_assetDependencies != null)
            {
                _filtered = EditorGUILayout.ToggleLeft("Filter by selection", _filtered);
                EditorGUILayout.LabelField($"Dependency total count: {_assetDependencies.Count}");
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Print asset dependencies to console"))
                        PrintDependencies(_filtered ? FilterDependency(Selection.objects) : _assetDependencies);
                }
            }
        }
        
        if (_jdbDependencies != null && _assetDependencies != null)
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Print JDBs that reference (even implicitly) this object") && obj != default)
                        FindJdbRefsToObject(obj);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    obj = EditorGUILayout.ObjectField(
                        new GUIContent("Object", ""),
                        obj,
                        typeof(Object),
                        allowSceneObjects: false);
                }
            }
        }

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            EditorGUILayout.LabelField("Default material finder");
            using (new EditorGUILayout.VerticalScope())
            {
                if (Selection.gameObjects != null && GUILayout.Button("Select default material objects"))
                {
                    Selection.objects = Selection.gameObjects.SelectMany(
                            x =>
                                x.GetComponentsInChildren<MeshRenderer>()
                                    .Where(m => m.sharedMaterials.Where(sm => sm != null).Any(sm => sm.name.Contains("Default-Material")))
                                    .Select(mr => mr.gameObject)
                                    .Concat(
                                        x.GetComponentsInChildren<SkinnedMeshRenderer>()
                                            .Where(m => m.sharedMaterials.Where(sm => sm != null).Any(sm => sm.name.Contains("Default-Material")))
                                            .Select(mr => mr.gameObject)))
                        .ToArray();
                }

                if (GUILayout.Button("Collect default dependencies to console"))
                {
                    _defaultDependencies = FindDefaultReferencedAssets();
                }

                if (_defaultDependencies != null && GUILayout.Button("Print Defaults dependencies to console"))
                {
                    PrintDependencies(_defaultDependencies.ToDictionary(a => a.Item1, a => a.Item2.Select(x => $"{x.guid} - {x.localIdentifierInFile}")));
                }

                if (_defaultDependencies != null && GUILayout.Button("Print Default Materials dependencies to console"))
                {
                    PrintDependencies(
                        _defaultDependencies.Where(a => a.Item2.Any(x => x.localIdentifierInFile == 10303))
                            .ToDictionary(a => a.Item1, a => a.Item2.Select(x => $"{x.guid} - {x.localIdentifierInFile}")));
                }

                if (_allAssets != null && GUILayout.Button("Print unextracted materials to console"))
                {
                    var nonExtracted = _allAssets
                        .Where(a =>
                        {
                            return a.ToLower().Contains("fbx");
                        })
                        .Select(s =>
                        {
                            return ValueTuple.Create(s, AssetDatabase.LoadAllAssetsAtPath(s));
                        })
                        .Where(assets =>
                        {
                            return assets.Item2.Any(a => a.GetType() == typeof(Material));
                        })
                        .ToDictionary(s =>s.Item1,s =>s.Item2.Select(obj => obj.name));
                    PrintDependencies(nonExtracted);
                }
            }
        }

        
    }

    private IEnumerable<(string, ObjectIdentifier[])> FindDefaultReferencedAssets()
    {
        _allAssets = AssetDatabase.FindAssets("")
            .Except(AssetDatabase.FindAssets("t: Script"))
            .Select(AssetDatabase.GUIDToAssetPath);
        return _allAssets
            .Select(a => ValueTuple.Create(a, ContentBuildInterface.GetPlayerObjectIdentifiersInAsset(new GUID(AssetDatabase.AssetPathToGUID(a)), BuildTarget.StandaloneWindows64)))
            .Select(a => ValueTuple.Create(a.Item1, ContentBuildInterface.GetPlayerDependenciesForObjects(a.Item2, BuildTarget.StandaloneWindows64, null)))
            .Where(a => a.Item2.Any(x => x.guid == k_BuiltInGuid));
    }

    private void FindJdbRefsToObject(Object obj)
    {
        var objPath = AssetDatabase.GetAssetPath(obj);
        if (objPath == default)
        {
            Logger.IfInfo()?.Message($"Cant't get path from AssetDatabase for object {obj}").Write();
            return;
        }

        Dictionary<JdbMetadata, IEnumerable<string>> jdbReferencingThisObj = new Dictionary<JdbMetadata, IEnumerable<string>>();

        if (_jdbDependencies.TryGetValue(objPath, out IEnumerable<JdbMetadata> metadatas))
        {
            foreach (var metadata in metadatas)
                jdbReferencingThisObj.Add(metadata, new List<string>() {objPath});
        }

        if (_assetDependencies.TryGetValue(AssetDatabase.GetAssetPath(obj), out IEnumerable<string> assetPaths))
        {
            var referencingAssetPaths = assetPaths;
            var jdbsRefThisPaths = referencingAssetPaths
                .Where(referencingAssetPath => _jdbDependencies.ContainsKey(referencingAssetPath))
                .SelectMany(
                    referencingAssetPath => _jdbDependencies[referencingAssetPath],
                    (referencingAssetPath, jdbMeta) => new {referencingAssetPath, jdbMeta});

            foreach (var jdbToAssetPath in jdbsRefThisPaths)
            {
                if (!jdbReferencingThisObj.TryGetValue(jdbToAssetPath.jdbMeta, out IEnumerable<string> pathList))
                    pathList = new List<string>();
                pathList = pathList.Append(jdbToAssetPath.referencingAssetPath);
                jdbReferencingThisObj[jdbToAssetPath.jdbMeta] = pathList;
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (var jdb in jdbReferencingThisObj)
        {
            string s = "";
            s += $"'{jdb.Key.name}' through: ";
            foreach (var objRefList in jdb.Value)
                s += $" '{objRefList}',";
            s = s.Remove(s.Length - 1);
            sb.AppendLine(s);
        }

        Logger.IfInfo()?.Message($"JDBs referencing this object '{obj}': {sb.ToString()}").Write();
    }

    private void CollectJdbDependencies()
    {
        _jdbDependencies =
            AssetDatabase.FindAssets("t: JdbMetadata")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(v => AssetDatabase.LoadAssetAtPath<JdbMetadata>(v))
                .SelectMany(jdbMetas => jdbMetas.UnityRefs, (JdbMeta, RefMapping) => new {JdbMeta, RefMapping.Resolved})
                .GroupBy(x => x.Resolved, y => y.JdbMeta)
                .ToDictionary(k => k.Key, v => v.AsEnumerable());
    }

    private void CollectAssetDependencies()
    {
        _assetDependencies = AssetDatabase.FindAssets("")
            .Except(AssetDatabase.FindAssets("t: Script"))
            .Except(_excludeScenes ? AssetDatabase.FindAssets("t: Scene") : new string[0])
            .Select(AssetDatabase.GUIDToAssetPath)
            .SelectMany(
                assetPath => AssetDatabase.GetDependencies(assetPath, true)
                    .Except(new string[] {assetPath}),
                (assetPath, dependency) => new {assetPath, dependency})
            .GroupBy(k => k.dependency, g => g.assetPath)
            .ToDictionary(k => k.Key, v => v.AsEnumerable());
    }

    private Dictionary<string, IEnumerable<string>> FilterDependency(Object[] objects)
    {
        var pathes = objects
            .Select(o => AssetDatabase.GetAssetPath(o))
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return _assetDependencies.Keys.Intersect(pathes).ToDictionary(key => key, key => _assetDependencies[key]);
    }

    private void SaveDependencyToCache(Dictionary<string, IEnumerable<string>> dependencyList)
    {
        var serialized = JsonConvert.SerializeObject(dependencyList);
        File.WriteAllText(CachePath(), serialized);
    }

    private Dictionary<string, IEnumerable<string>> LoadDependencyFromCache()
    {
        if (!IsCacheExist())
        {
            Logger.IfWarn()?.Message($"Dependency cache did not exist.").Write();
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(File.ReadAllText(CachePath()));
        }
        catch (Exception)
        {
            Logger.IfWarn()?.Message($"Invalid dependency cache.").Write();
            return null;
        }
    }


    private void PrintDependencies<T>(Dictionary<string, IEnumerable<T>> dict)
    {
        foreach (var dep in dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(dep.Key);
            foreach (var val in dep.Value)
                sb.AppendLine($" '{val.ToString()}'");
            Logger.IfInfo()?.Message(sb.ToString()).Write();
        }
    }
}