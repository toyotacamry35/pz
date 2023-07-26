using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public class ReplaceBuiltinShaders : IBuildTask
    {
        static readonly GUID k_BuiltInGuid = new GUID("0000000000000000f000000000000000");

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public int Version
        {
            get { return 1; }
        }

        [InjectContext(ContextUsage.InOut)]
        private ICollectorContext _collectorContext;

        [InjectContext(ContextUsage.In)]
        IProfileContext _profileContext;

        private List<Shader> _preloadedShaders = new List<Shader>();
        private Dictionary<string, string> _shaderNameToShaderPath = new Dictionary<string, string>();
        private List<Material> _changedMaterials = new List<Material>();
        private List<GUID> _buildingShaders = new List<GUID>();


        public ReturnCode Run()
        {
            _preloadedShaders = ShaderHelper.PreloadBuiltinShaders();
            _shaderNameToShaderPath = ShaderHelper.ParseShaderFolder();
            var result = ReplaceShadersForMaterials();
            if (result > 0)
                return ReturnCode.Exception;
            
            _profileContext.Profiler.SaveProfileInfo("replaced_materials", _changedMaterials.Select(m => ValueTuple.Create(m.name, m.shader.name)).ToList());

            return ReturnCode.Success;
        }

        private int ReplaceShadersForMaterials()
        {
            var assetNames = _collectorContext
                .ProjectDependencies
                .SelectMany(b => b.Value)
                .SelectMany(b => b.assetNames);

            foreach (var assetPath in assetNames)
            {
                if (!assetPath.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                    continue;

                var result = ShaderHelper.CheckAndReplaceShader(assetPath, _shaderNameToShaderPath, _preloadedShaders, _buildingShaders, _changedMaterials);
                if(result > 0)
                    return result;
            }

            AssetDatabase.SaveAssets();
            return 0;
        }
    }
}