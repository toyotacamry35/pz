using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    internal static class DependencyCollectorHelper
    {
        internal static IEnumerable<string> ExceptMaterialsAndShaders(this IEnumerable<string> assets)
            => assets.Where(
                v =>
                {
                    var type = AssetDatabase.GetMainAssetTypeAtPath(v);
                    return type != typeof(Shader) && type != typeof(Material);
                });

        internal static IEnumerable<string> FilterByAssetType(this IEnumerable<string> inList)
        {
            return inList.Where(
                t =>
                {
                    var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(t);
                    return mainAssetType != null &&
                           mainAssetType != typeof(MonoScript) &&
                           mainAssetType != typeof(SceneAsset) &&
                           mainAssetType != typeof(DefaultAsset) &&
                           mainAssetType != typeof(Terrain) &&
                           mainAssetType != typeof(TerrainCollider) &&
                           mainAssetType != typeof(TerrainData) &&
                           mainAssetType.FullName != "AwesomeTechnologies.VegetationPackage" &&
                           mainAssetType.FullName != "Assets.TerrainBaker.TerrainBaker" &&
                           mainAssetType.FullName != "JesseStiller.TerrainFormerExtension.TerrainFormer" &&
                           mainAssetType.FullName != "JesseStiller.TerrainFormerExtension.TerrainSetNeighbours";
                });
        }

        internal static List<string> FilterByAssetType(List<string> inList)
        {
            return inList.Where(
                    t =>
                    {
                        var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(t);
                        return mainAssetType != null &&
                               mainAssetType != typeof(MonoScript) &&
                               mainAssetType != typeof(SceneAsset) &&
                               mainAssetType != typeof(DefaultAsset) &&
                               mainAssetType != typeof(Terrain) &&
                               mainAssetType != typeof(TerrainCollider) &&
                               mainAssetType != typeof(TerrainData) &&
                               mainAssetType.FullName != "AwesomeTechnologies.VegetationPackage" &&
                               mainAssetType.FullName != "Assets.TerrainBaker.TerrainBaker" &&
                               mainAssetType.FullName != "JesseStiller.TerrainFormerExtension.TerrainFormer" &&
                               mainAssetType.FullName != "JesseStiller.TerrainFormerExtension.TerrainSetNeighbours";
                    })
                .ToList();
        }
    }
}