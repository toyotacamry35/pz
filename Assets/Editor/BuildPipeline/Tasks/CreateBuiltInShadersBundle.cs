using System;
using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;
using NLog;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    /// <summary>
    /// Optional build task that extracts Unity's built in shaders and assigns them to the specified bundle 
    /// </summary>
    public class CreateBuiltInShadersBundle : IBuildTask
    {
        static readonly GUID k_BuiltInGuid = new GUID("0000000000000000f000000000000000");

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public int Version
        {
            get { return 1; }
        }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.InOut)]
        private ICollectorContext _collectorContext;

        [InjectContext(ContextUsage.InOut, true)]
        private IBundleExplictObjectLayout m_Layout;

        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;

        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IProfileContext _profileContext;
#pragma warning restore 649

        public string SharedBundleName { get; set; }

        private readonly List<GUID> _builtinMaterials = new List<GUID>();
        private List<long> _builtinShadersFileIDs = new List<long>();


        private List<Shader> _preloadedShaders = new List<Shader>();
        private List<Material> _changedMaterials = new List<Material>();
        private List<GUID> _checkedMaterials = new List<GUID>();
        private List<GUID> _buildingShaders = new List<GUID>();

        public CreateBuiltInShadersBundle(string shared)
        {
            SharedBundleName = shared;
        }

        public ReturnCode Run()
        {
            CollectShadersByInfos();

            FindShadersByMaterials();

            var sharedObjects = new HashSet<ObjectIdentifier>();
            AddToAllSharedShaders(sharedObjects);
            AddToExplicit(SharedBundleName, sharedObjects);


            AssetDatabase.SaveAssets();

            if (m_Layout.ExplicitObjectLocation.Count == 0)
                m_Layout = null;

            _profileContext.Profiler.SaveProfileInfo("explicit_builtinshaders", m_Layout?.ExplicitObjectLocation);

            return ReturnCode.Success;
        }

        private void FindShadersByMaterials()
        {
            var assetNames = _collectorContext
                .ProjectDependencies
                .SelectMany(b => b.Value)
                .SelectMany(b => b.assetNames);

            foreach (var assetPath in assetNames)
            {
                if (!assetPath.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                    continue;

                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                var shader = material.shader;

                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(shader, out var guid, out long localId);
                if (guid != k_BuiltInGuid.ToString())
                    continue;
                _builtinShadersFileIDs.Add(localId);
                _builtinMaterials.Add(new GUID(AssetDatabase.AssetPathToGUID(assetPath)));
            }

            _builtinShadersFileIDs = _builtinShadersFileIDs.Distinct().ToList();
        }

        private void CollectShadersByInfos()
        {
            HashSet<ObjectIdentifier> buildInObjects = new HashSet<ObjectIdentifier>();
            foreach (AssetLoadInfo dependencyInfo in m_DependencyData.AssetInfo.Values)
                buildInObjects.UnionWith(dependencyInfo.referencedObjects.Where(x => x.guid == k_BuiltInGuid));

            foreach (SceneDependencyInfo dependencyInfo in m_DependencyData.SceneInfo.Values)
                buildInObjects.UnionWith(dependencyInfo.referencedObjects.Where(x => x.guid == k_BuiltInGuid));

            ObjectIdentifier[] usedSet = buildInObjects.ToArray();
            Type[] usedTypes = ContentBuildInterface.GetTypeForObjects(usedSet);

            if (m_Layout == null)
                m_Layout = new BundleExplictObjectLayout();

            Type shader = typeof(Shader);
            Type defaultMaterial = typeof(Material);
            for (int i = 0; i < usedTypes.Length; i++)
            {
                if (usedTypes[i] != shader)
                    continue;

                m_Layout.ExplicitObjectLocation.Add(usedSet[i], SharedBundleName);
            }

            if (m_Layout.ExplicitObjectLocation.Count == 0)
                m_Layout = null;
        }

        private void AddToAllSharedShaders(HashSet<ObjectIdentifier> sharedObjects)
        {
            _builtinMaterials
                .Select(guid => ContentBuildInterface.GetPlayerObjectIdentifiersInAsset(guid, m_Parameters.Target))
                .Select(materialIdentifier => ContentBuildInterface.GetPlayerDependenciesForObjects(materialIdentifier, m_Parameters.Target, m_Parameters.ScriptInfo))
                .Select(objectIdentifiers => objectIdentifiers.Where(o => _builtinShadersFileIDs.Any(sharedObject => o.localIdentifierInFile == sharedObject)))
                .SelectMany(shaderObjectIdentifier => shaderObjectIdentifier)
                .Distinct()
                .ForEach(identifier => sharedObjects.Add(identifier));
        }

        private void AddToExplicit(string name, HashSet<ObjectIdentifier> objects)
        {
            var usedSet = objects.ToArray();

            if (m_Layout == null)
                m_Layout = new BundleExplictObjectLayout();

            foreach (var o in usedSet)
            {
                if (m_Layout.ExplicitObjectLocation.ContainsKey(o))
                    continue;

                m_Layout.ExplicitObjectLocation.Add(o, name);
            }
        }
    }
}