using System;
using System.Linq;
using Assets.Src.App;
using Nest;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    
    public class FilterWriteSerializedFiles : IBuildTask
    {
        public int Version { get { return 2; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In)]
        IBundleWriteData m_WriteData;

        [InjectContext]
        IBuildResults m_Results;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;

        [InjectContext(ContextUsage.In, true)]
        IBuildCache m_Cache;
        
        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;
        
        [InjectContext(ContextUsage.In)]
        private ICollectorContext _collectorContext;
        
        [InjectContext(ContextUsage.In)]
        IProfileContext _profileContext;
#pragma warning restore 649

        public ReturnCode Run()
        {
            var bundleToMap = _collectorContext
                .ProjectDependencies.SelectMany(x => x.Value, (list, dependencies) => ValueTuple.Create(list.Key, dependencies))
                .ToDictionary(x => x.Item2.assetBundleName, x => x.Item1);

            var operations= m_WriteData.WriteOperations.Where(
                    w =>
                    {
                        var bundle = m_WriteData.FileToBundle[w.Command.internalName];
                        if (!bundleToMap.ContainsKey(bundle))
                        {
                            _profileContext.Profiler.Log($"Doesn't exist in bundle map: {bundle}");
                            return true;
                        }
                        return bundleToMap[bundle] == _pipelineConfigurationContext.MapName;
                    })
                .ToList();
            
            m_WriteData.WriteOperations.Clear();
            m_WriteData.WriteOperations.AddRange(operations);

            return ReturnCode.Success;
        }
    }
}