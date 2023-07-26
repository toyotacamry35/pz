using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.App;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    public class CreateShadersBundle : IBuildTask
    {
        private static readonly GUID KBuiltInGuid = new GUID("0000000000000000f000000000000000");

        [InjectContext(ContextUsage.In, false)]
        private IDependencyData _dependencyData;

        [InjectContext(ContextUsage.In, false)]
        private ICollectorContext _collectorContext;

        [InjectContext(ContextUsage.InOut, true)]
        private UnityEditor.Build.Pipeline.Interfaces.IBundleExplictObjectLayout m_Layout;

        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;

        [InjectContext(ContextUsage.In)]
        private IBuildPipelineSettings _pipelineSettings;

        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        public int Version => 1;

        public string ShaderBundleName { get; set; }

        private List<string> guids = new List<string>();

        public CreateShadersBundle(string bundleName)
        {
            ShaderBundleName = bundleName;
        }

        private void FindGuidForSharedMaterials()
        {
            var assetNames = _collectorContext.SharedBundleToMapReferences.Values
                .ToArray()
                .SelectMany(b => b.AssetBundleBuild.assetNames)
                .ToArray();

            foreach (var assetPath in assetNames)
            {
                if (!assetPath.EndsWith(".shader", StringComparison.OrdinalIgnoreCase))
                    continue;

                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (!guids.Contains(guid) && guid != KBuiltInGuid.ToString())
                    guids.Add(guid);
            }
        }

        public ReturnCode Run()
        {
            var sharedObjects = new HashSet<ObjectIdentifier>();
            FindGuidForSharedMaterials();

            var hashSet = new HashSet<ObjectIdentifier>();
            foreach (var value in _dependencyData.AssetInfo.Values)
            {
                hashSet.UnionWith(
                    value.referencedObjects
                        .Where(x => x.guid != KBuiltInGuid)
                        .Where(
                            x =>
                            {
                                if (_pipelineConfigurationContext.MapName == AssetBundleHelper.SystemName)
                                    return true;
                                if (guids.Contains(x.guid.ToString()))
                                    return true;
                                sharedObjects.Add(x);
                                return false;
                            }));
            }

            foreach (var value2 in _dependencyData.SceneInfo.Values)
            {
                hashSet.UnionWith(
                    value2.referencedObjects
                        .Where(x => x.guid != KBuiltInGuid)
                        .Where(
                            x =>
                            {
                                if (_pipelineConfigurationContext.MapName == AssetBundleHelper.SystemName)
                                    return true;
                                if (guids.Contains(x.guid.ToString()))
                                    return true;
                                sharedObjects.Add(x);
                                return false;
                            }));
            }

            AddToExplicitLayout("Shared-Custom-Shaders.bundle", sharedObjects);
            AddToExplicitLayout(ShaderBundleName,hashSet);

            return ReturnCode.Success;
        }

        private void AddToExplicitLayout(string name, HashSet<ObjectIdentifier> hashSet)
        {
            var array = hashSet.ToArray();
            var typeForObjects = ContentBuildInterface.GetTypeForObjects(array);
            if (m_Layout == null)
                m_Layout = new UnityEditor.Build.Pipeline.BundleExplictObjectLayout();

            var typeFromHandle = typeof(Shader);
            for (var i = 0; i < typeForObjects.Length; i++)
            {
                if (typeForObjects[i] != typeFromHandle)
                    continue;
                m_Layout.ExplicitObjectLocation.Add(array[i], name);
            }

            if (m_Layout.ExplicitObjectLocation.Count == 0)
                m_Layout = null;
        }
    }
}