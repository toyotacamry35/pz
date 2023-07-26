using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using System;
using AwesomeTechnologies.VegetationStudio;
using SharedCode.Aspects.Cartographer;
using System.Linq;
using Assets.Src.Effects.Step;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;

namespace Assets.Src.Cartographer.Editor
{
    public class CSOCollectSceneStats : ICartographerSceneOperation
    {
        // interfaces -----------------------------------------------------------------------------
        private interface IAssetStats
        {
            string ID { get; }
            string Path { get; }
            string Name { get; }
            long Size { get; }
        }

        private interface IAssetUsage
        {
            PrefabLinks PrefabLinks { get; }
            CartographerSceneTypeUsage ScenesUsage { get; }
        }

        private static int CompareAssetStats(int xCount, int yCount, IAssetStats x, IAssetStats y)
        {
            var countCompare = yCount.CompareTo(xCount);
            if (countCompare == 0)
            {
                var pathCompare = x.Path.CompareTo(y.Path);
                if (pathCompare == 0)
                {
                    return x.Name.CompareTo(y.Name);
                }
                return pathCompare;
            }
            return countCompare;
        }

        // classes --------------------------------------------------------------------------------
        private class Usage
        {
            public int[] Counters { get; } = null;

            public Usage(int countersCount)
            {
                if (countersCount > 0)
                {
                    Counters = new int[countersCount];
                }
            }

            public void Collect(int counterIndex)
            {
                if ((counterIndex >= 0) && (counterIndex < Counters.Length))
                {
                    Counters[counterIndex] += 1;
                }
            }

            public int GetTotal()
            {
                var result = 0;
                for (var index = 0; index < Counters.Length; ++index)
                {
                    result += Counters[index];
                }
                return result;
            }
        }

        private class CartographerSceneTypeUsage : Usage
        {
            public CartographerSceneTypeUsage() : base(6) { }

            public int GetIndex(CartographerSceneType cartographerSceneTypeMask)
            {
                if ((cartographerSceneTypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection)
                {
                    return 0;
                }
                else if ((cartographerSceneTypeMask & CartographerSceneType.BackgroundClient) == CartographerSceneType.BackgroundClient)
                {
                    return 1;
                }
                else if ((cartographerSceneTypeMask & CartographerSceneType.BackgroundServer) == CartographerSceneType.BackgroundServer)
                {
                    return 2;
                }
                else if ((cartographerSceneTypeMask & CartographerSceneType.MapDefClient) == CartographerSceneType.MapDefClient)
                {
                    return 3;
                }
                else if ((cartographerSceneTypeMask & CartographerSceneType.MapDefServer) == CartographerSceneType.MapDefServer)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }

            public void Collect(CartographerSceneType cartographerSceneTypeMask)
            {
                Collect(GetIndex(cartographerSceneTypeMask));
            }

            public string GetTitle(CartographerCommon.FormatArguments formatArguments)
            {
                return $"Total\tS/CB/SB/CR/SR/U";
            }

            public string GetPrint(CartographerCommon.FormatArguments formatArguments)
            {
                return $"{GetTotal()}\t{Counters[0]}/{Counters[1]}/{Counters[2]}/{Counters[3]}/{Counters[4]}/{Counters[5]}";
            }
        }

        private class PhysicMaterialStats : IAssetStats, IAssetUsage, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public long Size { get; set; } = 0;

            public PrefabLinks PrefabLinks { get; } = new PrefabLinks();
            public CartographerSceneTypeUsage ScenesUsage { get; } = new CartographerSceneTypeUsage();

            public PhysicMaterialStats(PhysicMaterial physicMaterial, string id, string path, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                if (physicMaterial != null)
                {
                    Name = physicMaterial.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(physicMaterial);
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetTitle(formatArguments)}\tName\tSize\t{PrefabLinks.GetTitle(formatArguments)}\tPath\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetPrint(formatArguments)}\t{Name}\t{Size}\t{PrefabLinks.GetPrint(formatArguments)}\t{Path}\t{ID}";
            }
        }

        private class MeshStats : IAssetStats, IAssetUsage, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public long Size { get; set; } = 0;

            public int Vertices { get; private set; } = 0;
            public int Triangles { get; private set; } = 0;
            public int AsColliderUsageCount { get; set; } = 0;
            public PrefabLinks PrefabLinks { get; } = new PrefabLinks();
            public CartographerSceneTypeUsage ScenesUsage { get; } = new CartographerSceneTypeUsage();

            public MeshStats(Mesh mesh, string id, string path, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                if (mesh != null)
                {
                    Name = mesh.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(mesh);
                    Vertices = mesh.vertexCount;
                    Triangles = mesh.triangles.Length / 3;
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetTitle(formatArguments)}\tName\tSize\tVert/Tri\tAsC\t{PrefabLinks.GetTitle(formatArguments)}\tPath\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetPrint(formatArguments)}\t{Name}\t{Size}\t{Vertices}/{Triangles}\t{AsColliderUsageCount}\t{PrefabLinks.GetPrint(formatArguments)}\t{Path}\t{ID}";
            }
        }

        private class TextureStats : IAssetStats, IAssetUsage, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public long Size { get; set; } = 0;

            public int Width { get; set; } = 0;
            public int Height { get; set; } = 0;
            public int MipMapCount { get; set; } = 0;
            public GraphicsFormat GraphicsFormat { get; set; } = GraphicsFormat.None;
            public TextureFormat Format { get; set; } = (TextureFormat)(0);

            public PrefabLinks PrefabLinks { get; } = new PrefabLinks();
            public CartographerSceneTypeUsage ScenesUsage { get; } = new CartographerSceneTypeUsage();

            public TextureStats(Texture texture, string id, string path, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                if (texture != null)
                {
                    Name = texture.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(texture);
                    Width = texture.width;
                    Height = texture.height;
                    MipMapCount = texture.mipmapCount;
                    GraphicsFormat = texture.graphicsFormat;
                    if (texture is Texture2D texture2D)
                    {
                        Format = texture2D.format;
                    }
                    if (texture is Texture3D texture3D)
                    {
                        Format = texture3D.format;
                    }
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetTitle(formatArguments)}\tName\tSize\tWxH/Mips\tGraphicsFormat\tFormat\t{PrefabLinks.GetTitle(formatArguments)}\tPath\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetPrint(formatArguments)}\t{Name}\t{Size}\t{Width}x{Height}/{MipMapCount}\t{GraphicsFormat}\t{Format}\t{PrefabLinks.GetPrint(formatArguments)}\t{Path}\t{ID}";
            }
        }

        private class ShaderStats : IAssetStats, IAssetUsage, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public long Size { get; private set; } = 0;

            public PrefabLinks PrefabLinks { get; } = new PrefabLinks();
            public CartographerSceneTypeUsage ScenesUsage { get; } = new CartographerSceneTypeUsage();

            public ShaderStats(Shader shader, string id, string path, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                if (shader != null)
                {
                    Name = shader.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(shader);
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetTitle(formatArguments)}\tName\tSize\t{PrefabLinks.GetTitle(formatArguments)}\tPath\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetPrint(formatArguments)}\t{Name}\t{Size}\t{PrefabLinks.GetPrint(formatArguments)}\t{Path}\t{ID}";
            }
        }

        private class MaterialStats : IAssetStats, IAssetUsage, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public long Size { get; private set; } = 0;

            public string[] ShaderKeywords { get; set; } = new string[0];
            public ShaderStats ShaderStats { get; private set; } = null;
            public List<KeyValuePair<string, TextureStats>> TexturesStats { get; } = new List<KeyValuePair<string, TextureStats>>();

            public PrefabLinks PrefabLinks { get; } = new PrefabLinks();
            public CartographerSceneTypeUsage ScenesUsage { get; } = new CartographerSceneTypeUsage();

            public string GetShaderKeywords(string delimiter)
            {
                var result = new StringBuilder();
                foreach (var shaderKeyword in ShaderKeywords)
                {
                    CartographerCommon.AppendIfNotEmpty(result, delimiter);
                    result.Append($"{shaderKeyword}");
                }
                if (result.Length == 0)
                {
                    result.Append("none");
                }
                return result.ToString();
            }

            public string GetTextures(string delimiter)
            {
                var result = new StringBuilder();
                foreach (var textureStats in TexturesStats)
                {
                    CartographerCommon.AppendIfNotEmpty(result, delimiter);
                    result.Append($"{textureStats.Key}:{textureStats.Value?.Name ?? "none"}");
                }
                if (result.Length == 0)
                {
                    result.Append("none");
                }
                return result.ToString();
            }

            public MaterialStats(Material material, string id, string path, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                if (material != null)
                {
                    Name = material.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(material);
                    ShaderKeywords = material.shaderKeywords;
                    if (material.shader != null)
                    {
                        ShaderStats = graphicsUsage.CollectShader(material.shader, prefabStats, cartographerSceneTypeMask, errorStats);
                    }
                    else
                    {
                        CartographerCommon.CollectIntDictionary(errorStats.MaterialNullShader, Path);
                    }
                    var texturePropertyNames = material.GetTexturePropertyNames();
                    foreach (var texturePropertyName in texturePropertyNames)
                    {
                        var texture = material.GetTexture(texturePropertyName);
                        if (texture != null)
                        {
                            var textureStats = graphicsUsage.CollectTexture(texture, prefabStats, cartographerSceneTypeMask, errorStats);
                            TexturesStats.Add(new KeyValuePair<string, TextureStats>(texturePropertyName, textureStats));
                        }
                        else
                        {
                            CartographerCommon.CollectIntDictionary(errorStats.MaterialNullTexture, $"{Path}\t{texturePropertyName}");
                        }
                    }
                }
            }

            public void Collect(PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                if (prefabStats == null) // yes it is right == null
                {
                    if (ShaderStats != null)
                    {
                        ShaderStats.ScenesUsage.Collect(cartographerSceneTypeMask);
                    }
                    if ((TexturesStats != null) && (TexturesStats.Count > 0))
                    {
                        foreach (var textureStats in TexturesStats)
                        {
                            if (textureStats.Value != null)
                            {
                                textureStats.Value.ScenesUsage.Collect(cartographerSceneTypeMask);
                            }
                        }
                    }
                }
            }

            public void Link(PrefabStats prefabStats, ErrorStats errorStats)
            {
                if (prefabStats != null)
                {
                    if (ShaderStats != null)
                    {
                        ShaderStats.PrefabLinks.Collect(prefabStats, errorStats);
                    }
                    if ((TexturesStats != null) && (TexturesStats.Count > 0))
                    {
                        foreach (var textureStats in TexturesStats)
                        {
                            if (textureStats.Value != null)
                            {
                                textureStats.Value.PrefabLinks.Collect(prefabStats, errorStats);
                            }
                        }
                    }
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetTitle(formatArguments)}\tName\tSize\t" +
                       $"KC\tKeywords\t" +
                       $"Shader\t" +
                       $"TC\tTextures\t" +
                       $"{PrefabLinks.GetTitle(formatArguments)}\t" +
                       $"Path\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{ScenesUsage.GetPrint(formatArguments)}\t{Name}\t{Size}\t" +
                       $"{ShaderKeywords.Length}\t{GetShaderKeywords(formatArguments.Delimiter)}\t" +
                       $"{ShaderStats?.Name ?? "none"}\t" +
                       $"{TexturesStats.Count}\t{GetTextures(formatArguments.Delimiter)}\t" +
                       $"{PrefabLinks.GetPrint(formatArguments)}\t" +
                       $"{Path}\t{ID}";
            }
        }

        private class ColliderStats
        {
            public string ColliderType { get; private set; }

            public ColliderStats(Collider collider, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                if (collider != null)
                {
                    ColliderType = collider.GetType().Name;
                    graphicsUsage.CollectPhysicMaterial(collider.sharedMaterial, prefabStats, cartographerSceneTypeMask, errorStats);
                    if (collider is MeshCollider meshCollider)
                    {
                        if ((meshCollider != null) && (meshCollider.sharedMesh != null))
                        {
                            var meshStats = graphicsUsage.CollectMesh(meshCollider.sharedMesh, prefabStats, cartographerSceneTypeMask, errorStats);
                            if ((meshStats != null) && (prefabStats == null)) // yes it is right == null
                            {
                                meshStats.AsColliderUsageCount += 1;
                            }
                            else
                            {
                                errorStats.GameObjectNullMeshStats.Add($"{collider.name}\t{ColliderType}\t{CartographerCommon.GetGameObjectPrintAsPrefab(collider.gameObject)}");
                            }
                        }
                        else
                        {
                            errorStats.GameObjectNullMeshCollider.Add($"{collider.name}\t{ColliderType}\t{CartographerCommon.GetGameObjectPrintAsPrefab(collider.gameObject)}");
                        }
                    }
                }
            }
        }

        private class RendererStats
        {
            private Renderer linkedRenderer = null;

            public List<MaterialStats> MaterialsStats { get; } = new List<MaterialStats>();
            public MeshStats MeshStats { get; private set; } = null;
            public LODGroupStats LODGroupStats { get; private set; } = null;
            public string RendererType { get; private set; }

            public RendererStats(Renderer renderer, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                linkedRenderer = renderer;
                if (linkedRenderer != null)
                {
                    RendererType = linkedRenderer.GetType().Name;

                    if (linkedRenderer is MeshRenderer meshRenderer)
                    {
                        var meshFilters = meshRenderer.gameObject.GetComponents<MeshFilter>();
                        if ((meshFilters != null) && (meshFilters.Length > 0))
                        {
                            foreach (var meshFilter in meshFilters)
                            {
                                if ((meshFilter != null) && (meshFilter.sharedMesh != null))
                                {
                                    MeshStats = graphicsUsage.CollectMesh(meshFilter.sharedMesh, prefabStats, cartographerSceneTypeMask, errorStats);
                                }
                                else
                                {
                                    errorStats.GameObjectNullMeshFilter.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                                }
                            }
                            if (meshFilters.Length > 1)
                            {
                                errorStats.GameObjectMultipleMeshFilters.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                            }
                        }
                        else
                        {
                            errorStats.GameObjectNoMeshFilters.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                        }
                    }
                    else if (linkedRenderer is SkinnedMeshRenderer skinnedMeshRenderer)
                    {
                        errorStats.GameObjectSkinnedMeshRenderer.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                    }
                    else
                    {
                        errorStats.GameObjectOtherRenderer.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                    }

                    foreach (var material in linkedRenderer.sharedMaterials)
                    {
                        if (material != null)
                        {
                            var materialStats = graphicsUsage.CollectMaterial(material, prefabStats, cartographerSceneTypeMask, errorStats);
                            if (materialStats != null)
                            {
                                MaterialsStats.Add(materialStats);
                            }
                        }
                        else
                        {
                            errorStats.GameObjectNullSharedMaterialInRenderer.Add(CartographerCommon.GetGameObjectPrintAsPrefab(renderer.gameObject));
                        }
                    }
                }
            }

            public void Update(LODGroupStats lodGroupStats, GameObjectStats gameObjectStats)
            {
                LODGroupStats = lodGroupStats;
            }
            public bool Match(Renderer renderer)
            {
                if (linkedRenderer == null)
                {
                    return false;
                }
                return (linkedRenderer == renderer);
            }
            public void Finish(GameObjectStats gameObjectStats)
            {
                linkedRenderer = null;
            }
        }

        private class LODStats
        {
            public int Index { get; set; } = -1;
            public float ScreenRelativeTransitionHeight { get; set; } = 0.0f;
            public List<RendererStats> RenderersStats { get; } = new List<RendererStats>();

            public LODStats(LODGroup lodGroup, int index, ref LOD lod, LODGroupStats lodGroupStats, GameObjectStats gameObjectStats, ErrorStats errorStats)
            {
                Index = index;
                ScreenRelativeTransitionHeight = lod.screenRelativeTransitionHeight;
                foreach (var renderer in lod.renderers)
                {
                    if (renderer != null)
                    {
                        var rendererStatsFound = gameObjectStats.RenderersStats.FirstOrDefault(rendererStats => rendererStats.Match(renderer));
                        if (rendererStatsFound != null)
                        {
                            rendererStatsFound.Update(lodGroupStats, gameObjectStats);
                            RenderersStats.Add(rendererStatsFound);
                        }
                        else
                        {
                            errorStats.GameObjectUnknownRendererInLOD.Add($"{lodGroup.name}\t{Index}\t{CartographerCommon.GetGameObjectPrintAsPrefab(lodGroup.gameObject)}");
                        }
                    }
                    else
                    {
                        errorStats.GameObjectNullRendererInLOD.Add($"{lodGroup.name}\t{Index}\t{CartographerCommon.GetGameObjectPrintAsPrefab(lodGroup.gameObject)}");
                    }
                }
            }

            public string GetMeshPrint(string meshDelimiter)
            {
                var meshStats = new StringBuilder();
                if (RenderersStats.Count == 1)
                {
                    var triangles = RenderersStats[0].MeshStats?.Triangles ?? 0;
                    meshStats.Append(triangles);
                }
                else if (RenderersStats.Count > 1)
                {
                    var totalTriangles = 0;
                    foreach (var rendererStats in RenderersStats)
                    {
                        CartographerCommon.AppendIfNotEmpty(meshStats, meshDelimiter);
                        var triangles = rendererStats.MeshStats?.Triangles ?? 0;
                        meshStats.Append(triangles);
                        totalTriangles += triangles;
                    }
                    meshStats.Insert(0, $"{totalTriangles}(");
                    meshStats.Append(')');
                }
                else
                {
                    meshStats.Append("NONE");
                }
                meshStats.Append($"<{ScreenRelativeTransitionHeight}>");
                return meshStats.ToString();
            }

            public override string ToString()
            {
                return $"";
            }
        }

        private class LODGroupStats
        {
            public List<LODStats> LODsStats { get; } = new List<LODStats>();

            public LODGroupStats(LODGroup lodGroup, GameObjectStats gameObjectStats, ErrorStats errorStats)
            {
                if (lodGroup != null)
                {
                    var lods = lodGroup.GetLODs();
                    if ((lods != null) && (lods.Length > 0))
                    {
                        for (int index = 0; index < lods.Length; ++index)
                        {
                            var lodStats = new LODStats(lodGroup, index, ref lods[index], this, gameObjectStats, errorStats);
                            LODsStats.Add(lodStats);
                        }
                    }
                    else
                    {
                        errorStats.GameObjectZeroLODs.Add($"{lodGroup.name}\t{CartographerCommon.GetGameObjectPrintAsPrefab(lodGroup.gameObject)}");
                    }
                }
            }

            public string GetMeshPrint(string lodDelimiter, string meshDelimiter)
            {
                var meshStats = new StringBuilder();
                var count = LODsStats.Count;
                if (count > 0)
                {
                    for (int index = 0; index < count; ++index)
                    {
                        CartographerCommon.AppendIfNotEmpty(meshStats, lodDelimiter);
                        meshStats.Append($"LOD{index}:{LODsStats[index].GetMeshPrint(meshDelimiter)}");
                    }
                }
                meshStats.Insert(0, $"LODS:{count} [");
                meshStats.Append("]");
                return meshStats.ToString();
            }

            public override string ToString()
            {
                return $"";
            }
        }

        private class ParticleSystemStats
        {
            //TODO
            public ParticleSystemStats(ParticleSystem particleSystem, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                if (particleSystem != null)
                {
                    //particleSystem.shape.sprite;
                    //particleSystem.shape.spriteRenderer;
                    //particleSystem.shape.meshRenderer;
                    //particleSystem.shape.skinnedMeshRenderer;
                }
            }
        }

        private class TerrainStats
        {
            //TODO
            public TerrainStats(Terrain terrain, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                if (terrain != null)
                {
                }
            }
        }

        private class GameObjectStats
        {
            public string Name { get; private set; } = string.Empty;
            public string Tag { get; private set; } = string.Empty;
            public int Layer { get; private set; } = 0;
            public StaticEditorFlags Static { get; private set; } = 0;

            public Dictionary<string, CartographerCommon.KeyCount<string>> Tags { get; } = new Dictionary<string, CartographerCommon.KeyCount<string>>();
            public Dictionary<int, CartographerCommon.KeyCount<int>> Layers { get; } = new Dictionary<int, CartographerCommon.KeyCount<int>>();
            public Dictionary<StaticEditorFlags, CartographerCommon.KeyCount<StaticEditorFlags>> Statics { get; } = new Dictionary<StaticEditorFlags, CartographerCommon.KeyCount<StaticEditorFlags>>();

            public Bounds ColliderBounds { get; private set; } = new Bounds();
            public Bounds RendererBounds { get; private set; } = new Bounds();
            public Bounds TerrainBounds { get; private set; } = new Bounds();

            public int NavMeshObstaclesCount { get; private set; } = 0;
            public int FXMarkersCount { get; private set; } = 0;
            public int MeshBlendingsCount { get; private set; } = 0;

            public List<ColliderStats> CollidersStats { get; } = new List<ColliderStats>();
            public List<RendererStats> RenderersStats { get; } = new List<RendererStats>();
            public List<LODGroupStats> LODGroupsStats { get; } = new List<LODGroupStats>();
            public List<TerrainStats> TerrainsStats { get; } = new List<TerrainStats>();
            public List<ParticleSystemStats> ParticleSystemsStats { get; } = new List<ParticleSystemStats>();

            private static string GetBoundsString(Bounds bounds, string label)
            {
                return $"{label}[{bounds.extents.x * 2.0f}, {bounds.extents.y * 2.0f}, {bounds.extents.z * 2.0f}]";
            }

            private int GetNonLodGroupRenderersCount()
            {
                var result = 0;
                foreach (var rendererStats in RenderersStats)
                {
                    if (rendererStats.LODGroupStats == null)
                    {
                        ++result;
                    }
                }
                return result;
            }

            private int GetNonMeshRenderersCount()
            {
                var result = 0;
                foreach (var rendererStats in RenderersStats)
                {
                    if (!rendererStats.RendererType.Equals(typeof(MeshRenderer).Name))
                    {
                        ++result;
                    }
                }
                return result;
            }

            public GameObjectStats(GameObject gameObject, GraphicsUsage graphicsUsage, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                if (gameObject != null)
                {
                    Name = gameObject.name;
                    Tag = gameObject.tag;
                    Layer = gameObject.layer;
                    Static = GameObjectUtility.GetStaticEditorFlags(gameObject);

                    var transforms = gameObject.GetComponentsInChildren<Transform>(true);
                    foreach (var transform in transforms)
                    {
                        var childGameObject = transform.gameObject;
                        if (childGameObject != null)
                        {
                            CartographerCommon.AddKey(childGameObject.tag, Tags);
                            CartographerCommon.AddKey(childGameObject.layer, Layers);
                            CartographerCommon.AddKey(GameObjectUtility.GetStaticEditorFlags(childGameObject), Statics);
                        }
                    }

                    var boundsCollector = new CartographerBoundsCollectior();

                    var colliders = gameObject.GetComponentsInChildren<Collider>(true);
                    if ((colliders != null) && (colliders.Length > 0))
                    {
                        foreach (var collider in colliders)
                        {
                            if (collider != null)
                            {
                                var colliderStats = new ColliderStats(collider, graphicsUsage, prefabStats, cartographerSceneTypeMask, errorStats);
                                CollidersStats.Add(colliderStats);
                                boundsCollector.Collect(collider.bounds);
                            }
                            else
                            {
                                errorStats.GameObjectNullCollider.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                            }
                        }
                        ColliderBounds = boundsCollector.Bounds;
                        boundsCollector.Reset();
                    }

                    var navMeshObstacles = gameObject.GetComponentsInChildren<NavMeshObstacle>(true);
                    NavMeshObstaclesCount = navMeshObstacles.Length;

                    var fxMarkers = gameObject.GetComponentsInChildren<FXMarkerOnObj>(true);
                    FXMarkersCount = fxMarkers.Length;

                    var meshBlendings = gameObject.GetComponentsInChildren<TerrainBlend.MeshBlending>(true);
                    MeshBlendingsCount = meshBlendings.Length;

                    var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                    if ((renderers != null) && (renderers.Length > 0))
                    {
                        foreach (var renderer in renderers)
                        {
                            if (renderer != null)
                            {
                                var rendererStats = new RendererStats(renderer, graphicsUsage, prefabStats, cartographerSceneTypeMask, errorStats);
                                RenderersStats.Add(rendererStats);
                                boundsCollector.Collect(renderer.bounds);
                            }
                            else
                            {
                                errorStats.GameObjectNullRenderer.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                            }
                        }
                        RendererBounds = boundsCollector.Bounds;
                        boundsCollector.Reset();
                    }
                    var lodGroups = gameObject.GetComponentsInChildren<LODGroup>(true);
                    if ((lodGroups != null) && (lodGroups.Length > 0))
                    {
                        foreach (var lodGroup in lodGroups)
                        {
                            if (lodGroup != null)
                            {
                                var lodGroupStats = new LODGroupStats(lodGroup, this, errorStats);
                                LODGroupsStats.Add(lodGroupStats);
                            }
                            else
                            {
                                errorStats.GameObjectNullLODGroup.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                            }
                        }
                    }

                    foreach (var rendererStats in RenderersStats)
                    {
                        rendererStats.Finish(this);
                    }

                    var terrains = gameObject.GetComponentsInChildren<Terrain>(true);
                    if ((terrains != null) && (terrains.Length > 0))
                    {
                        foreach (var terrain in terrains)
                        {
                            if (terrain != null)
                            {
                                var terrainStats = new TerrainStats(terrain, graphicsUsage, prefabStats, cartographerSceneTypeMask, errorStats);
                                TerrainsStats.Add(terrainStats);
                                if (terrain.terrainData != null)
                                {
                                    var terrainBounds = terrain.terrainData.bounds;
                                    terrainBounds.center += terrain.transform.position;
                                    boundsCollector.Collect(terrainBounds);
                                }
                            }
                            else
                            {
                                errorStats.GameObjectNullTerrain.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                            }
                        }
                        TerrainBounds = boundsCollector.Bounds;
                        boundsCollector.Reset();
                    }

                    var particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
                    if ((particleSystems != null) && (particleSystems.Length > 0))
                    {
                        foreach (var particleSystem in particleSystems)
                        {
                            if (particleSystem != null)
                            {
                                var particleSystemStats = new ParticleSystemStats(particleSystem, graphicsUsage, prefabStats, cartographerSceneTypeMask, errorStats);
                                ParticleSystemsStats.Add(particleSystemStats);
                            }
                            else
                            {
                                errorStats.GameObjectNullParticleSystem.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                            }
                        }
                    }
                }
            }

            public string GetCountsTitle(CartographerCommon.FormatArguments formatArguments)
            {
                return $"NonLod_C\t" +
                       $"Collider_C/NavMesh_C/FXMarker_C/MeshBlending_C\t" +
                       $"Renderer_C/LODGroup_C/Terrain_C/Particle_C";
            }

            public string GetCountsPrint(CartographerCommon.FormatArguments formatArguments)
            {
                return $"NonLod_{GetNonLodGroupRenderersCount()}\t" +
                       $"Collider_{CollidersStats.Count}/NavMesh_{NavMeshObstaclesCount}/FXMarker_{FXMarkersCount}/MeshBlending_{MeshBlendingsCount}\t" +
                       $"Renderer_{RenderersStats.Count}/LODGroup_{LODGroupsStats.Count}/Terrain_{TerrainsStats.Count}/Particle_{ParticleSystemsStats.Count}";
            }

            public string GetEditorTitle(CartographerCommon.FormatArguments formatArguments)
            {
                return "Tag\tLayer\tStatic";
            }

            public string GetEditorPrint(CartographerCommon.FormatArguments formatArguments)
            {
                return $"{Tags.Count}: {Tag}\t{Layers.Count}: {LayerMask.LayerToName(Layer)}\t{Statics.Count}: {Static}";
            }

            public string GetMeshStatus(MeshRestrictions meshRestrictions, CartographerCommon.FormatArguments formatArguments)
            {
                var meshStatus = new StringBuilder();
                foreach (var lodGroupStats in LODGroupsStats)
                {
                    if (lodGroupStats.LODsStats.Count > 0)
                    {
                        var lodStats = lodGroupStats.LODsStats[0];
                        if (lodStats.ScreenRelativeTransitionHeight < meshRestrictions.MinLOD0)
                        {
                            CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                            meshStatus.Append($"MinLOD0: {lodStats.ScreenRelativeTransitionHeight} < {meshRestrictions.MinLOD0}");
                        }
                    }
                }
                foreach (var lodGroupStats in LODGroupsStats)
                {
                    if (lodGroupStats.LODsStats.Count > 0)
                    {
                        var lodStats = lodGroupStats.LODsStats[lodGroupStats.LODsStats.Count - 1];
                        if (lodStats.ScreenRelativeTransitionHeight < meshRestrictions.MinCutoff)
                        {
                            CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                            meshStatus.Append($"MinCutoff: {lodStats.ScreenRelativeTransitionHeight} < {meshRestrictions.MinCutoff}");
                        }
                        if (lodStats.ScreenRelativeTransitionHeight > meshRestrictions.MaxCutoff)
                        {
                            CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                            meshStatus.Append($"MaxCutoff: {lodStats.ScreenRelativeTransitionHeight} > {meshRestrictions.MaxCutoff}");
                        }
                    }
                }
                foreach (var lodGroupStats in LODGroupsStats)
                {
                    if (lodGroupStats.LODsStats.Count < meshRestrictions.MinLODCount)
                    {
                        CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                        meshStatus.Append($"MinLODCount: {lodGroupStats.LODsStats.Count} < {meshRestrictions.MinLODCount}");
                    }
                }
                if (RenderersStats.Count > meshRestrictions.MaxRenderersCount)
                {
                    CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                    meshStatus.Append($"MaxRenderersCount: {RenderersStats.Count} > {meshRestrictions.MaxRenderersCount}");
                }
                if (LODGroupsStats.Count > meshRestrictions.MaxLODGroupsCount)
                {
                    CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                    meshStatus.Append($"MaxLODGroupsCount: {LODGroupsStats.Count} > {meshRestrictions.MaxLODGroupsCount}");
                }
                foreach (var lodGroupStats in LODGroupsStats)
                {
                    if (lodGroupStats.LODsStats.Count > 0)
                    {
                        var lodStats = lodGroupStats.LODsStats[lodGroupStats.LODsStats.Count - 1];
                        var totalTriangles = 0;
                        foreach (var rendererStats in lodStats.RenderersStats)
                        {
                            var triangles = rendererStats.MeshStats?.Triangles ?? 0;
                            totalTriangles += triangles;
                        }
                        if (totalTriangles > meshRestrictions.MaxLastLODPolycount)
                        {
                            CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                            meshStatus.Append($"MaxLastLODPolycount: {totalTriangles} > {meshRestrictions.MaxLastLODPolycount}");
                        }
                    }
                }
                if (!meshRestrictions.AllowNonLODRenderers)
                {
                    var nonLodGroupRenderersCount = GetNonLodGroupRenderersCount();
                    if (nonLodGroupRenderersCount > 0)
                    {
                        CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                        meshStatus.Append($"NonLODRenderers: {nonLodGroupRenderersCount} != 0");
                    }
                }
                if (!meshRestrictions.AllowNonMeshRenderers)
                {
                    var nonMeshRenderersCount = GetNonMeshRenderersCount();
                    if (nonMeshRenderersCount > 0)
                    {
                        CartographerCommon.AppendIfNotEmpty(meshStatus, formatArguments.Delimiter);
                        meshStatus.Append($"NonMeshRenderers: {nonMeshRenderersCount} != 0");
                    }
                }
                return meshStatus.ToString();
            }

            public string GetMeshTitle(MeshRestrictions meshRestrictions, CartographerCommon.FormatArguments formatArguments)
            {
                return "Status\tLODGroups Meshes";
            }

            public string GetMeshPrint(MeshRestrictions meshRestrictions, CartographerCommon.FormatArguments formatArguments)
            {
                var meshPrint = new StringBuilder();
                foreach (var lodGroupStats in LODGroupsStats)
                {
                    CartographerCommon.AppendIfNotEmpty(meshPrint, formatArguments.Delimiter);
                    meshPrint.Append(lodGroupStats.GetMeshPrint("/", "|"));
                }
                return $"{GetMeshStatus(meshRestrictions, formatArguments)}\t{meshPrint.ToString()}";
            }
        }

        private class PrefabStats : IAssetStats, CartographerCommon.IPrintable
        {
            public string ID { get; private set; } = string.Empty;
            public string Path { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public long Size { get; set; } = 0;

            public PrefabAssetType Type { get; private set; } = PrefabAssetType.NotAPrefab;
            public CartographerSceneTypeUsage ConnectedUsage { get; } = new CartographerSceneTypeUsage();
            public CartographerSceneTypeUsage DisconnectedUsage { get; } = new CartographerSceneTypeUsage();
            public CartographerSceneTypeUsage SimilarUsage { get; } = new CartographerSceneTypeUsage();

            public GraphicsUsage GraphicsUsage { get; } = new GraphicsUsage();

            public GameObjectStats GameObjectStats { get; private set; } = null;

            public PrefabStats(string id, string path, GraphicsUsage graphicsUsage, ErrorStats errorStats)
            {
                ID = id;
                Path = path;
                GameObject rootGameObject = null;
                try
                {
                    rootGameObject = PrefabUtility.LoadPrefabContents(Path);
                }
                catch (Exception e)
                {
                    CartographerCommon.CollectIntDictionary(errorStats.PrefabStatsCantLoadPrefab, Path);
                    return;
                }
                if (rootGameObject != null)
                {
                    Type = PrefabUtility.GetPrefabAssetType(rootGameObject);
                    Name = rootGameObject.name;
                    Size = Profiler.GetRuntimeMemorySizeLong(rootGameObject);

                    GameObjectStats = new GameObjectStats(rootGameObject, graphicsUsage, this, CartographerSceneType.None, errorStats);
                    PrefabUtility.UnloadPrefabContents(rootGameObject);
                }
                else
                {
                    CartographerCommon.CollectIntDictionary(errorStats.PrefabStatsErrorLoadPrefab, Path);
                }
            }

            public int GetTotal()
            {
                return ConnectedUsage.GetTotal() + DisconnectedUsage.GetTotal() + SimilarUsage.GetTotal();
            }

            public void Collect(CartographerSceneType cartographerSceneTypeMask, PrefabInstanceStatus status)
            {
                if (status == PrefabInstanceStatus.Connected)
                {
                    ConnectedUsage.Collect(cartographerSceneTypeMask);
                }
                else if (status == PrefabInstanceStatus.Disconnected)
                {
                    DisconnectedUsage.Collect(cartographerSceneTypeMask);
                }
                else
                {
                    SimilarUsage.Collect(cartographerSceneTypeMask);
                }
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"Total\t" +
                       $"S:C/D/S\t" +
                       $"CB:C/D/S\t" +
                       $"SB:C/D/S\t" +
                       $"CR:C/D/S\t" +
                       $"SR:C/D/S\t" +
                       $"U:C/D/S\t" +
                       $"Name\tSize\t{GameObjectStats?.GetMeshTitle(context as MeshRestrictions, formatArguments) ?? string.Empty}\t{GameObjectStats?.GetEditorTitle(formatArguments) ?? string.Empty}\t{GraphicsUsage.GetTitle(formatArguments)}\t{GameObjectStats?.GetCountsTitle(formatArguments) ?? string.Empty}\tPath\tID";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{GetTotal()}\t" +
                       $"{ConnectedUsage.Counters[0] + DisconnectedUsage.Counters[0] + SimilarUsage.Counters[0]}:{ConnectedUsage.Counters[0]}/{DisconnectedUsage.Counters[0]}/{SimilarUsage.Counters[0]}\t" +
                       $"{ConnectedUsage.Counters[1] + DisconnectedUsage.Counters[1] + SimilarUsage.Counters[1]}:{ConnectedUsage.Counters[1]}/{DisconnectedUsage.Counters[1]}/{SimilarUsage.Counters[1]}\t" +
                       $"{ConnectedUsage.Counters[2] + DisconnectedUsage.Counters[2] + SimilarUsage.Counters[2]}:{ConnectedUsage.Counters[2]}/{DisconnectedUsage.Counters[2]}/{SimilarUsage.Counters[2]}\t" +
                       $"{ConnectedUsage.Counters[3] + DisconnectedUsage.Counters[3] + SimilarUsage.Counters[3]}:{ConnectedUsage.Counters[3]}/{DisconnectedUsage.Counters[3]}/{SimilarUsage.Counters[3]}\t" +
                       $"{ConnectedUsage.Counters[4] + DisconnectedUsage.Counters[4] + SimilarUsage.Counters[4]}:{ConnectedUsage.Counters[4]}/{DisconnectedUsage.Counters[4]}/{SimilarUsage.Counters[4]}\t" +
                       $"{ConnectedUsage.Counters[5] + DisconnectedUsage.Counters[5] + SimilarUsage.Counters[5]}:{ConnectedUsage.Counters[5]}/{DisconnectedUsage.Counters[5]}/{SimilarUsage.Counters[5]}\t" +
                       $"{Name}\t{Size}\t{GameObjectStats?.GetMeshPrint(context as MeshRestrictions, formatArguments) ?? string.Empty}\t{GameObjectStats?.GetEditorPrint(formatArguments) ?? string.Empty}\t{GraphicsUsage.GetPrint(this, formatArguments)}\t{GameObjectStats?.GetCountsPrint(formatArguments) ?? string.Empty}\t{Path}\t{ID}";
            }
        }

        private class ComponentStats : CartographerCommon.IPrintable
        {
            public Type Type { get; } = null;
            public CartographerSceneTypeUsage Usage { get; } = new CartographerSceneTypeUsage();

            public ComponentStats(Type type)
            {
                Type = type;
            }

            public void Collect(CartographerSceneType cartographerSceneTypeMask)
            {
                Usage.Collect(cartographerSceneTypeMask);
            }

            public string GetTitle(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{Usage.GetTitle(formatArguments)}\tType";
            }

            public string GetPrint(object context, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{Usage.GetPrint(formatArguments)}\t{Type?.FullName ?? string.Empty}";
            }
        }

        private class PrefabLink
        {
            public int Count { get; set; } = 0;
            public PrefabStats PrefabStats { get; } = null;

            public PrefabLink(PrefabStats prefabStats)
            {
                PrefabStats = prefabStats;
            }
        }

        private class PrefabLinks
        {
            public Dictionary<string, PrefabLink> Links { get; } = new Dictionary<string, PrefabLink>();

            public PrefabLink Get(PrefabStats prefabStats)
            {
                PrefabLink result;
                if (Links.TryGetValue(prefabStats.ID, out result))
                {
                    return result;
                }
                return null;
            }

            private string GetPrefabNames(string delimiter)
            {
                var result = new StringBuilder();
                foreach (var link in Links)
                {
                    CartographerCommon.AppendIfNotEmpty(result, delimiter);
                    result.Append($"{link.Value.Count}:{link.Value.PrefabStats.Name}");
                }
                if (result.Length == 0)
                {
                    result.Append("none");
                }
                return result.ToString();
            }

            public bool Collect(PrefabStats prefabStats, ErrorStats errorStats)
            {
                if (prefabStats != null)
                {
                    PrefabLink link;
                    if (!Links.TryGetValue(prefabStats.ID, out link))
                    {
                        link = new PrefabLink(prefabStats);
                        Links.Add(prefabStats.ID, link);
                    }
                    link.Count += 1;
                }
                return true;
            }

            public string GetTitle(CartographerCommon.FormatArguments formatArguments)
            {
                return $"PL\tPNames";
            }

            public string GetPrint(CartographerCommon.FormatArguments formatArguments)
            {
                return $"{Links.Count}\t{GetPrefabNames(formatArguments.Delimiter)}";
            }
        }

        private class GraphicsUsage
        {
            public Dictionary<string, PhysicMaterialStats> PhysicMaterials { get; } = new Dictionary<string, PhysicMaterialStats>();
            public Dictionary<string, MeshStats> Meshes { get; } = new Dictionary<string, MeshStats>();
            public Dictionary<string, TextureStats> Textures { get; } = new Dictionary<string, TextureStats>();
            public Dictionary<string, ShaderStats> Shaders { get; } = new Dictionary<string, ShaderStats>();
            public Dictionary<string, MaterialStats> Materials { get; } = new Dictionary<string, MaterialStats>();

            private static string GetList<TStats>(Dictionary<string, TStats> assetsStats, PrefabStats prefabStats, string delimiter) where TStats : IAssetUsage, IAssetStats
            {
                var result = new StringBuilder();
                var totalCount = 0;
                foreach (var assetStats in assetsStats)
                {
                    CartographerCommon.AppendIfNotEmpty(result, delimiter);
                    if (prefabStats != null)
                    {
                        var count = assetStats.Value.PrefabLinks.Get(prefabStats)?.Count ?? 0;
                        totalCount += count;
                        result.Append($"{count}:{assetStats.Value.Name}");
                    }
                    else
                    {
                        result.Append($"{assetStats.Value.Name}");
                    }
                }
                if (result.Length == 0)
                {
                    result.Append("none");
                }
                else if (prefabStats != null)
                {
                    result.Insert(0, $"{totalCount}{delimiter}");
                }
                return result.ToString();
            }

            private static TStats Collect<T, TStats>(T assetInstance,
                                                     Dictionary<string, TStats> assetsStats,
                                                     Dictionary<string, TStats> prefabAssetsStats,
                                                     PrefabStats prefabStats,
                                                     CartographerSceneType cartographerSceneTypeMask,
                                                     ErrorStats errorStats,
                                                     Func<string, string, TStats> Creator,
                                                     Action<TStats> Collector,
                                                     Action<TStats> Linker) where T : UnityEngine.Object where TStats : IAssetUsage
            {
                var id = CartographerCommon.GetAssetID(assetInstance, false);
                var path = CartographerCommon.GetAssetPath(assetInstance);
                //report error
                if (prefabAssetsStats != null)
                {
                    TStats assetStats;
                    if (!prefabAssetsStats.TryGetValue(id, out assetStats))
                    {
                        if (!assetsStats.TryGetValue(id, out assetStats))
                        {
                            assetStats = Creator(id, path);
                            assetsStats.Add(id, assetStats);
                        }
                        else
                        {
                            Linker?.Invoke(assetStats);
                        }
                        prefabAssetsStats.Add(id, assetStats);
                    }
                    else
                    {
                        Linker?.Invoke(assetStats);
                    }
                    assetStats.PrefabLinks.Collect(prefabStats, errorStats);
                    return assetStats;
                }
                else
                {
                    TStats assetStats;
                    if (!assetsStats.TryGetValue(id, out assetStats))
                    {
                        assetStats = Creator(id, path);
                        assetsStats.Add(id, assetStats);
                    }
                    else
                    {
                        Collector?.Invoke(assetStats);
                    }
                    assetStats.ScenesUsage.Collect(cartographerSceneTypeMask);
                    return assetStats;
                }
            }

            public PhysicMaterialStats CollectPhysicMaterial(PhysicMaterial physicMaterial, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                return Collect(physicMaterial,
                               PhysicMaterials,
                               prefabStats?.GraphicsUsage.PhysicMaterials ?? null,
                               prefabStats,
                               cartographerSceneTypeMask,
                               errorStats,
                               (id, path) => new PhysicMaterialStats(physicMaterial, id, path, errorStats),
                               null,
                               null);
            }

            public MeshStats CollectMesh(Mesh mesh, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                return Collect(mesh,
                               Meshes,
                               prefabStats?.GraphicsUsage.Meshes ?? null,
                               prefabStats,
                               cartographerSceneTypeMask,
                               errorStats,
                               (id, path) => new MeshStats(mesh, id, path, errorStats),
                               null,
                               null);
            }

            public TextureStats CollectTexture(Texture texture, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                return Collect(texture,
                               Textures,
                               prefabStats?.GraphicsUsage.Textures ?? null,
                               prefabStats,
                               cartographerSceneTypeMask,
                               errorStats,
                               (id, path) => new TextureStats(texture, id, path, errorStats),
                               null,
                               null);
            }

            public ShaderStats CollectShader(Shader shader, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                return Collect(shader,
                               Shaders,
                               prefabStats?.GraphicsUsage.Shaders ?? null,
                               prefabStats,
                               cartographerSceneTypeMask,
                               errorStats,
                               (id, path) => new ShaderStats(shader, id, path, errorStats),
                               null,
                               null);
            }

            public MaterialStats CollectMaterial(Material material, PrefabStats prefabStats, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                return Collect(material,
                               Materials,
                               prefabStats?.GraphicsUsage.Materials ?? null,
                               prefabStats,
                               cartographerSceneTypeMask,
                               errorStats,
                               (id, path) => new MaterialStats(material, id, path, this, prefabStats, cartographerSceneTypeMask, errorStats),
                               (materialStats) => materialStats.Collect(prefabStats, cartographerSceneTypeMask, errorStats),
                               (materialStats) => materialStats.Link(prefabStats, errorStats));
            }

            public void Print(List<string> lines, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                var physicMaterialsTotal = 0;
                var physicMaterialsList = CartographerCommon.GetListFromDictionary(PhysicMaterials, (x, y) => CompareAssetStats(x.ScenesUsage.GetTotal(), y.ScenesUsage.GetTotal(), x, y), (x) => physicMaterialsTotal += x.ScenesUsage.GetTotal());
                CartographerCommon.PrintPrintableList(lines, physicMaterialsList, $"--- PhysicMaterials, unique: {physicMaterialsList.Count}, total: {physicMaterialsTotal}", null, formatArguments, addEmptyLine);

                var meshesTotal = 0;
                var meshesList = CartographerCommon.GetListFromDictionary(Meshes, (x, y) => CompareAssetStats(x.ScenesUsage.GetTotal(), y.ScenesUsage.GetTotal(), x, y), (x) => meshesTotal += x.ScenesUsage.GetTotal());
                CartographerCommon.PrintPrintableList(lines, meshesList, $"--- Meshes, unique: {meshesList.Count}, total: {meshesTotal}", null, formatArguments, true);

                var texturesTotal = 0;
                var texturesList = CartographerCommon.GetListFromDictionary(Textures, (x, y) => CompareAssetStats(x.ScenesUsage.GetTotal(), y.ScenesUsage.GetTotal(), x, y), (x) => texturesTotal += x.ScenesUsage.GetTotal());
                CartographerCommon.PrintPrintableList(lines, texturesList, $"--- Textures, unique: {texturesList.Count}, total: {texturesTotal}", null, formatArguments, true);

                var shadersTotal = 0;
                var shadersList = CartographerCommon.GetListFromDictionary(Shaders, (x, y) => CompareAssetStats(x.ScenesUsage.GetTotal(), y.ScenesUsage.GetTotal(), x, y), (x) => shadersTotal += x.ScenesUsage.GetTotal());
                CartographerCommon.PrintPrintableList(lines, shadersList, $"--- Shaders, unique: {shadersList.Count}, total: {shadersTotal}", null, formatArguments, true);

                var materialsTotal = 0;
                var materialsList = CartographerCommon.GetListFromDictionary(Materials, (x, y) => CompareAssetStats(x.ScenesUsage.GetTotal(), y.ScenesUsage.GetTotal(), x, y), (x) => materialsTotal += x.ScenesUsage.GetTotal());
                CartographerCommon.PrintPrintableList(lines, materialsList, $"--- Materials, unique: {materialsList.Count}, total: {materialsTotal}", null, formatArguments, true);
            }

            public string GetTitle(CartographerCommon.FormatArguments formatArguments)
            {
                return $"PMC\tPMaterials\tMC\tMeshes\tTC\tTextures\tSC\tShaders\tMC\tMaterials";
            }

            public string GetPrint(CartographerCommon.FormatArguments formatArguments)
            {
                return GetPrint(null, formatArguments);
            }

            public string GetPrint(PrefabStats prefabStats, CartographerCommon.FormatArguments formatArguments)
            {
                return $"{PhysicMaterials.Count}\t{GetList(PhysicMaterials, prefabStats, formatArguments.Delimiter)}\t" +
                       $"{Meshes.Count}\t{GetList(Meshes, prefabStats, formatArguments.Delimiter)}\t" +
                       $"{Textures.Count}\t{GetList(Textures, prefabStats, formatArguments.Delimiter)}\t" +
                       $"{Shaders.Count}\t{GetList(Shaders, prefabStats, formatArguments.Delimiter)}\t" +
                       $"{Materials.Count}\t{GetList(Materials, prefabStats, formatArguments.Delimiter)}";
            }

        }

        private class PrefabsUsage
        {
            public Dictionary<string, PrefabStats> Prefabs { get; } = new Dictionary<string, PrefabStats>();

            public bool Collect(string id, string path, GraphicsUsage graphicsUsage, CartographerSceneType cartographerSceneTypeMask, PrefabInstanceStatus status, ErrorStats errorStats)
            {
                PrefabStats prefabStats;
                if (!Prefabs.TryGetValue(id, out prefabStats))
                {
                    prefabStats = new PrefabStats(id, path, graphicsUsage, errorStats);
                    Prefabs.Add(prefabStats.ID, prefabStats);
                }
                prefabStats.Collect(cartographerSceneTypeMask, status);
                return true;
            }

            public void Print(List<string> lines, MeshRestrictions meshRestrictions, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                var prefabsTotal = 0;
                var prefabsStatsList = CartographerCommon.GetListFromDictionary(Prefabs, (x, y) => CompareAssetStats(x.GetTotal(), y.GetTotal(), x, y), (x) => prefabsTotal += x.GetTotal());
                CartographerCommon.PrintPrintableList(lines, prefabsStatsList, $"--- Prefabs, unique: {prefabsStatsList.Count}, total: {prefabsTotal}", meshRestrictions, formatArguments, addEmptyLine);
            }
        }

        private class ComponentsUsage
        {
            public Dictionary<Type, ComponentStats> Components { get; } = new Dictionary<Type, ComponentStats>();

            public bool Collect(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                var gameObjectComponents = gameObject.GetComponentsInChildren<Component>(true);
                var nullComponents = 0;
                foreach (var component in gameObjectComponents)
                {
                    if (component != null)
                    {
                        if (component is Transform)
                        {
                            continue;
                        }
                        var type = component.GetType();
                        ComponentStats componentStats = null;
                        if (!Components.TryGetValue(type, out componentStats))
                        {
                            componentStats = new ComponentStats(type);
                            Components.Add(type, componentStats);
                        }
                        componentStats.Collect(cartographerSceneTypeMask);
                    }
                    else
                    {
                        ++nullComponents;
                    }
                }
                if (nullComponents > 0)
                {
                    errorStats.NullComponent.Add($"{nullComponents}\t{CartographerCommon.GetGameObjectPrint(gameObject)}");
                }
                return true;
            }

            public void Print(List<string> lines, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                var totalComponents = 0;
                var componentsList = CartographerCommon.GetListFromDictionary(Components,
                (x, y) =>
                {
                    var xTotal = x.Usage.GetTotal();
                    var yTotal = y.Usage.GetTotal();
                    return ((xTotal == yTotal) ? x.Type.FullName.CompareTo(y.Type.FullName) : yTotal.CompareTo(xTotal));
                },
                (x) =>
                {
                    totalComponents += x.Usage.GetTotal();
                });
                CartographerCommon.PrintPrintableList(lines, componentsList, $"--- Components, unique: {componentsList.Count}, total: {totalComponents}", null, formatArguments, addEmptyLine);
            }
        }

        private class GameObjectsUsage
        {
            public CartographerSceneTypeUsage SceneUsage { get; } = new CartographerSceneTypeUsage();
            public List<CartographerSceneTypeUsage> PrefabUsage { get; } = new List<CartographerSceneTypeUsage>();
            public List<GameObjectStats> GameObjectsStats { get; } = new List<GameObjectStats>();

            private int GetIndex(PrefabInstanceStatus status)
            {
                if (status == PrefabInstanceStatus.Connected)
                {
                    return 0;
                }
                else if (status == PrefabInstanceStatus.Disconnected)
                {
                    return 1;
                }
                else if (status == PrefabInstanceStatus.MissingAsset)
                {
                    return 2;
                }
                return 3;
            }
            private int GetTotal()
            {
                return PrefabUsage[0].GetTotal() + PrefabUsage[1].GetTotal() + PrefabUsage[2].GetTotal() + PrefabUsage[3].GetTotal();
            }

            public GameObjectsUsage()
            {
                PrefabUsage.Add(new CartographerSceneTypeUsage());
                PrefabUsage.Add(new CartographerSceneTypeUsage());
                PrefabUsage.Add(new CartographerSceneTypeUsage());
                PrefabUsage.Add(new CartographerSceneTypeUsage());
            }
            public void Collect(Scene scene, GameObject gameObject, GraphicsUsage graphicsUsage, CartographerSceneType cartographerSceneTypeMask, ErrorStats errorStats)
            {
                var gameObjectStats = new GameObjectStats(gameObject, graphicsUsage, null, cartographerSceneTypeMask, errorStats);
                GameObjectsStats.Add(gameObjectStats);
                SceneUsage.Collect(cartographerSceneTypeMask);
            }

            public void Collect(PrefabInstanceStatus status, CartographerSceneType cartographerSceneTypeMask)
            {
                PrefabUsage[GetIndex(status)].Collect(cartographerSceneTypeMask);
            }

            public void Print(List<string> lines, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                if (addEmptyLine) { lines.Add($""); }
                lines.Add($"--- Game objects");
                {
                    var newLines = new List<string>();
                    newLines.Add($"Total\tStreamer\tClient background\tServer background\tClient rest\tServer rest\tUnknown");
                    newLines.Add($"{SceneUsage.GetTotal()}\t{SceneUsage.Counters[0]}\t{SceneUsage.Counters[1]}\t{SceneUsage.Counters[2]}\t{SceneUsage.Counters[3]}\t{SceneUsage.Counters[4]}\t{SceneUsage.Counters[5]}");
                    lines.AddRange(CartographerCommon.FixLines(newLines, formatArguments));
                }
                {
                    var newLines = new List<string>();
                    newLines.Add($"Total\t" +
                                 $"Connected:\t{PrefabUsage[0].GetTitle(formatArguments)}\t" +
                                 $"Disconnected:\t{PrefabUsage[1].GetTitle(formatArguments)}\t" +
                                 $"Missing Asset:\t{PrefabUsage[2].GetTitle(formatArguments)}\t" +
                                 $"Not a prefab:\t{PrefabUsage[3].GetTitle(formatArguments)}");
                    newLines.Add($"{GetTotal()}\t" +
                                 $"Connected:\t{PrefabUsage[0].GetPrint(formatArguments)}\t" +
                                 $"Disconnected:\t{PrefabUsage[1].GetPrint(formatArguments)}\t" +
                                 $"Missing Asset:\t{PrefabUsage[2].GetPrint(formatArguments)}\t" +
                                 $"Not a prefab:\t{PrefabUsage[3].GetPrint(formatArguments)}");
                    lines.AddRange(CartographerCommon.FixLines(newLines, formatArguments));
                }
            }
        }

        private class ErrorStats
        {
            public Dictionary<string, int> MaterialNullShader { get; } = new Dictionary<string, int>();
            public Dictionary<string, int> MaterialNullTexture { get; } = new Dictionary<string, int>();
            public Dictionary<string, int> PrefabStatsCantLoadPrefab { get; } = new Dictionary<string, int>();
            public Dictionary<string, int> PrefabStatsErrorLoadPrefab { get; } = new Dictionary<string, int>();

            public List<string> GameObjectNullMeshStats { get; } = new List<string>();
            public List<string> GameObjectNullMeshCollider { get; } = new List<string>();
            public List<string> GameObjectNoMeshFilters { get; } = new List<string>();
            public List<string> GameObjectMultipleMeshFilters { get; } = new List<string>();
            public List<string> GameObjectNullMeshFilter { get; } = new List<string>();
            public List<string> GameObjectSkinnedMeshRenderer { get; } = new List<string>();
            public List<string> GameObjectOtherRenderer { get; } = new List<string>();
            public List<string> GameObjectNullSharedMaterialInRenderer { get; } = new List<string>();
            public List<string> GameObjectNullRendererInLOD { get; } = new List<string>();
            public List<string> GameObjectUnknownRendererInLOD { get; } = new List<string>();
            public List<string> GameObjectZeroLODs { get; } = new List<string>();
            public List<string> GameObjectNullCollider { get; } = new List<string>();
            public List<string> GameObjectNullRenderer { get; } = new List<string>();
            public List<string> GameObjectNullLODGroup { get; } = new List<string>();
            public List<string> GameObjectNullTerrain { get; } = new List<string>();
            public List<string> GameObjectNullParticleSystem { get; } = new List<string>();

            public List<string> NullComponent { get; } = new List<string>();
            public List<string> CantGetCorrespondingPrefabOrModel { get; } = new List<string>();

            public Dictionary<string, int> CantGetAnyPrefabOrModel { get; } = new Dictionary<string, int>();
            public Dictionary<string, int> AmbiguousPrefabNames { get; } = new Dictionary<string, int>();
            public Dictionary<string, int> AmbiguousModelNames { get; } = new Dictionary<string, int>();

            public void Print(List<string> lines, CartographerCommon.FormatArguments formatArguments, bool addEmptyLine)
            {
                CartographerCommon.PrintList(lines, NullComponent, "--- Null components found", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, CantGetCorrespondingPrefabOrModel, "--- Can't get corresonding prefab or model", true, formatArguments, true);

                CartographerCommon.PrintIntDictionary(lines, CantGetAnyPrefabOrModel, "--- Can't get any prefab or model", true, formatArguments, true);
                CartographerCommon.PrintIntDictionary(lines, AmbiguousPrefabNames, "--- Ambiguous prefab names", true, formatArguments, true);
                CartographerCommon.PrintIntDictionary(lines, AmbiguousModelNames, "--- Ambiguous model names", true, formatArguments, true);

                CartographerCommon.PrintIntDictionary(lines, MaterialNullShader, "--- Material has null shader", true, formatArguments, true);
                CartographerCommon.PrintIntDictionary(lines, PrefabStatsCantLoadPrefab, "--- Can't load prefab", true, formatArguments, true);
                CartographerCommon.PrintIntDictionary(lines, PrefabStatsErrorLoadPrefab, "--- Error load prefab", true, formatArguments, true);

                CartographerCommon.PrintList(lines, GameObjectNullMeshCollider, "--- Null mesh collider", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNoMeshFilters, "--- No any mesh fiter", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectMultipleMeshFilters, "--- Multiple mesh filters", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullMeshFilter, "--- Null mesh filter", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectSkinnedMeshRenderer, "--- Skinned mesh renderer", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullSharedMaterialInRenderer, "--- Null material in renderer", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullRendererInLOD, "--- Null renderer in LOD", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectUnknownRendererInLOD, "--- Null unknown renderr in LOD", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectZeroLODs, "--- Zero LODs in LODGroup", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullCollider, "--- Null collider", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullRenderer, "--- Null renderer", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullLODGroup, "--- Null LOD group", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullTerrain, "--- Null terrain", true, formatArguments, addEmptyLine);
                CartographerCommon.PrintList(lines, GameObjectNullParticleSystem, "--- Null particle system", true, formatArguments, addEmptyLine);

                if (formatArguments.Verboose)
                {
                    CartographerCommon.PrintIntDictionary(lines, MaterialNullTexture, "--- Material has null texture", true, formatArguments, true);
                    CartographerCommon.PrintList(lines, GameObjectNullMeshStats, "--- Null mesh stats", true, formatArguments, addEmptyLine);
                    CartographerCommon.PrintList(lines, GameObjectOtherRenderer, "--- Unknown renderer", true, formatArguments, addEmptyLine);
                }
            }
        }

        private class SceneStats
        {
            private CartographerCommon.PrefabSearch prefabSearch = new CartographerCommon.PrefabSearch();

            public GraphicsUsage GraphicsUsage { get; } = new GraphicsUsage();
            public PrefabsUsage PrefabsUsage { get; } = new PrefabsUsage();
            public ComponentsUsage ComponentsUsage { get; } = new ComponentsUsage();
            public GameObjectsUsage GameObjectsUsage { get; } = new GameObjectsUsage();
            public ErrorStats ErrorStats { get; } = new ErrorStats();

            // private methods --------------------------------------------------------------------
            private void CollectPrefabsUsage(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
            {
                var status = PrefabUtility.GetPrefabInstanceStatus(gameObject);
                if ((status == PrefabInstanceStatus.Connected) || (status == PrefabInstanceStatus.Disconnected))
                {
                    var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                    if (prefabGameObject != null)
                    {
                        var id = CartographerCommon.GetAssetID(prefabGameObject, true);
                        var path = CartographerCommon.GetAssetPath(prefabGameObject);
                        PrefabsUsage.Collect(id, path, GraphicsUsage, cartographerSceneTypeMask, status, ErrorStats);
                        GameObjectsUsage.Collect(status, cartographerSceneTypeMask);
                    }
                    else
                    {
                        ErrorStats.CantGetCorrespondingPrefabOrModel.Add(CartographerCommon.GetGameObjectPrintAsPrefab(gameObject));
                    }
                }
                else
                {
                    var fixedName = CartographerCommon.PrefabSearch.FixNameForPrefabSearch(gameObject.name);
                    {
                        var prefabsFound = prefabSearch.FindAssets($"{fixedName} t:Prefab");
                        if ((prefabsFound != null) && (prefabsFound.Length > 0))
                        {
                            var id = prefabsFound[0];
                            var path = CartographerCommon.GetAssetPath(id);
                            PrefabsUsage.Collect(id, path, GraphicsUsage, cartographerSceneTypeMask, status, ErrorStats);
                            GameObjectsUsage.Collect(PrefabInstanceStatus.MissingAsset, cartographerSceneTypeMask);
                            if (prefabsFound.Length > 1)
                            {
                                CartographerCommon.CollectIntDictionary(ErrorStats.AmbiguousPrefabNames, fixedName);
                            }
                            return;
                        }
                    }
                    {
                        var modelsFound = prefabSearch.FindAssets($"{fixedName} t:Model");
                        if ((modelsFound != null) && (modelsFound.Length > 0))
                        {
                            var id = modelsFound[0];
                            var path = CartographerCommon.GetAssetPath(id);
                            PrefabsUsage.Collect(id, path, GraphicsUsage, cartographerSceneTypeMask, status, ErrorStats);
                            GameObjectsUsage.Collect(PrefabInstanceStatus.MissingAsset, cartographerSceneTypeMask);
                            if (modelsFound.Length > 1)
                            {
                                CartographerCommon.CollectIntDictionary(ErrorStats.AmbiguousModelNames, fixedName);
                            }
                            return;
                        }
                    }
                    CartographerCommon.CollectIntDictionary(ErrorStats.CantGetAnyPrefabOrModel, fixedName);
                    GameObjectsUsage.Collect(PrefabInstanceStatus.NotAPrefab, cartographerSceneTypeMask);
                }
            }

            private void CollectComponentsUsage(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
            {
                ComponentsUsage.Collect(scene, gameObject, cartographerSceneTypeMask, ErrorStats);
            }

            private void CollectGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
            {
                GameObjectsUsage.Collect(scene, gameObject, GraphicsUsage, cartographerSceneTypeMask, ErrorStats);
            }

            // public methods ---------------------------------------------------------------------
            public bool VisitGameObject(CSOCollectSceneStatsArguments arguments, Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask)
            {
                if (CartographerCommon.IsGameObjectFolder(gameObject))
                {
                    return true;
                }
                if ((cartographerSceneTypeMask & CartographerSceneType.StreamCollection) == CartographerSceneType.StreamCollection)
                {
                    var sceneLoaderBehaviour = gameObject.GetComponent<SceneLoaderBehaviour>();
                    var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                    if ((sceneLoaderBehaviour != null) || (vegetationStudioManager != null))
                    {
                        return true;
                    }
                }
                if (arguments.CollectPrefabsUsage)
                {
                    CollectPrefabsUsage(scene, gameObject, cartographerSceneTypeMask);
                }
                if (arguments.CollectComponentsUsage)
                {
                    CollectComponentsUsage(scene, gameObject, cartographerSceneTypeMask);
                }
                if (arguments.CollectGameObjects)
                {
                    CollectGameObject(scene, gameObject, cartographerSceneTypeMask);
                }
                return false;
            }

            public void Print(List<string> lines, CSOCollectSceneStatsArguments arguments)
            {
                GameObjectsUsage.Print(lines, arguments.FormatArguments, false);
                GraphicsUsage.Print(lines, arguments.FormatArguments, true);
                PrefabsUsage.Print(lines, arguments.MeshRestrictions, arguments.FormatArguments, true);
                ComponentsUsage.Print(lines, arguments.FormatArguments, true);
                ErrorStats.Print(lines, arguments.FormatArguments, true);
            }
        }

        // messages -------------------------------------------------------------------------------
        public class MessagesClass : IProgressMessages
        {
            public string Title { get { return "Collect Components"; } }
            public string RunQuestion { get { return "Are you sure you want to collect components?"; } }
            public string WelcomeMessage { get { return "Collect components"; } }
            public string OnScenePrefix { get { return "Collect components"; } }
            public string OnCollectPrefabsPrefix { get { return "Collect prefabs..."; } }
            public string OnCollectModelsPrefix { get { return "Collect models..."; } }
            public string OnCollectGameObjectPrefix { get { return "GameObject: "; } }
            public string OnCollectPrefabRootGameObjectsPrefix { get { return "Collect visuals..."; } }
        }

        public static MessagesClass Messages = new MessagesClass();

        // data -----------------------------------------------------------------------------------
        private CSOCollectSceneStatsArguments arguments = new CSOCollectSceneStatsArguments();
        private SceneStats sceneStats = new SceneStats();

        //main procedures -------------------------------------------------------------------------
        private bool VisitSceneGameObject(Scene scene, GameObject gameObject, CartographerSceneType cartographerSceneTypeMask, ICartographerProgressCallback progressCallback, ref int gameObjectsCollected)
        {
            var goDeeper = sceneStats.VisitGameObject(arguments, scene, gameObject, cartographerSceneTypeMask);
            ++gameObjectsCollected;
            progressCallback.OnProgress(Messages.Title, $"{Messages.OnCollectGameObjectPrefix} {gameObjectsCollected}, {gameObject.name}", progressCallback.Progress);
            return goDeeper;
        }

        private void PrintResults()
        {
            var lines = new List<string>();
            sceneStats.Print(lines, arguments);
            var filePath = arguments.ResultsFilePath;
            File.WriteAllLines(filePath, lines);
            CartographerCommon.ReportError($"File created: {filePath}");
        }

        // constructor ----------------------------------------------------------------------------
        public CSOCollectSceneStats(CSOCollectSceneStatsArguments newArguments)
        {
            if (newArguments != null)
            {
                arguments = newArguments;
            }
        }

        // ICartographerSceneOperation interface --------------------------------------------------
        public IProgressMessages ProgressMessages { get { return Messages; } }

        public bool CanOperate(CartographerScene cartographerScene, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly)
        {
            return (!arguments.IgnoreUnknownCartographerSceneTypes) ||
                   ((cartographerScene.TypeMask & (CartographerSceneType.StreamCollection |
                                                   CartographerSceneType.BackgroundClient |
                                                   CartographerSceneType.BackgroundServer |
                                                   CartographerSceneType.MapDefClient |
                                                   CartographerSceneType.MapDefServer)) > 0);
        }

        public bool Start(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            return true;
        }

        public bool Operate(Scene scene, CartographerSceneType cartographerSceneTypeMask, CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            var gameObjectsCollected = 0;
            CartographerSceneObjectVisitor.Visit(scene, gameObject => { return VisitSceneGameObject(scene, gameObject, cartographerSceneTypeMask, progressCallback, ref gameObjectsCollected); });
            return true;
        }

        public void Finish(CartographerParamsDef cartographerParams, SceneCollectionDef sceneCollection, bool openedScenesOnly, ICartographerProgressCallback progressCallback)
        {
            PrintResults();
        }
    }
};