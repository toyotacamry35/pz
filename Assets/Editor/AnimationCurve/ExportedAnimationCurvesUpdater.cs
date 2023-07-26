using System;
using System.Collections.Generic;
using System.Linq;
using Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    public class ExportedAnimationCurvesUpdater : AssetPostprocessor
    {
        private static readonly Dictionary<Type, Action<IExportedAnimationCurve>> Updaters =
            new Dictionary<Type, Action<IExportedAnimationCurve>>
            {
                { typeof(ExportedAnimationCurve), ExportedAnimationCurveInspector.UpdateCurve },
                { typeof(ExportedAnimationCurve3), ExportedAnimationCurve3Inspector.UpdateCurve },
            };

        private static readonly string[] AnimationExtensions = {".fbx", ".anim"};
        
        private static HashSet<IExportedAnimationCurve> _Cache;        
                
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                if (AnimationExtensions.Any(x => importedAsset.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                {
                    var clipGuid = GetAssetGuid(importedAsset);
                    foreach (var clip in AssetDatabase.LoadAllAssetsAtPath(importedAsset).OfType<AnimationClip>().Where(x => x != null))
                    {
                        foreach (var eac in Cache.Where(x => x != null))
                        {
                            if (eac.AutoUpdate && Updaters.TryGetValue(eac.GetType(), out var updater))
                            {
                                foreach (var eacClip in eac.Clips.Where(x => x != null))
                                {
                                    var eacClipGuid = GetAssetGuid(eacClip);
                                    if (eacClipGuid == clipGuid && (eacClip == clip || eacClip.name == clip.name))
                                    {
                                        try
                                        {
                                            updater.Invoke(eac);
                                        }
                                        catch (Exception e)
                                        {
                                            Debug.LogError(e.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var assetType = AssetDatabase.GetMainAssetTypeAtPath(importedAsset);
                    if (assetType != null && Updaters.ContainsKey(assetType))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath(importedAsset, assetType) as IExportedAnimationCurve;
                        if (asset != null)
                            _Cache.Add(asset);
                    }
                }
            }
        }

        private static HashSet<IExportedAnimationCurve> Cache
        {
            get
            {
                if (_Cache == null)
                {
                    _Cache = new HashSet<IExportedAnimationCurve>();
                    foreach (var type in Updaters.Keys)
                    {
                        var guids = AssetDatabase.FindAssets($"t:{type.Name}");
                        foreach (var assetGuid in guids)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                            var asset = AssetDatabase.LoadAssetAtPath(assetPath, type) as IExportedAnimationCurve;
                            if (asset != null)
                                _Cache.Add(asset);
                        }
                    }
                }
                return _Cache;
            }
        }

        private static Guid GetAssetGuid(UnityEngine.Object obj) => new Guid(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
        
        private static Guid GetAssetGuid(string path) => new Guid(AssetDatabase.AssetPathToGUID(path));

    }
}