using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEditor;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public class ShaderHelper
    {
        public static readonly GUID k_BuiltInGuid = new GUID("0000000000000000f000000000000000");

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static string GetBuiltinFolder()
        {
            if (!Directory.Exists("Assets/Data/BuiltinShaders"))
                Directory.CreateDirectory("Assets/Data/BuiltinShaders");
            return "Assets/Data/BuiltinShaders";
        }

        public static List<Shader> PreloadBuiltinShaders()
        {
            var path = GetBuiltinFolder();
            var pathes = AssetDatabase.FindAssets("t:Shader", new[] {path});
            return pathes.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Shader>).ToList();
        }

        public static Dictionary<string, string> ParseShaderFolder()
        {
            var shaderNameToShaderPath = new Dictionary<string, string>();
            var builtinDirectory = new DirectoryInfo("BuiltinShaders");
            var files = builtinDirectory.GetFiles("*.shader", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var shaderText = File.ReadAllText(file.FullName);
                shaderNameToShaderPath.Add(GetShaderName(shaderText), file.FullName);
            }

            return shaderNameToShaderPath;
        }

        public static Match GetShaderPartMatch(string shaderText)
        {
            var matches = Regex.Matches(shaderText, "Shader[\\s]+\"[^\"]+\"", RegexOptions.None);
            if (matches.Count != 1)
            {
                EditorUtility.ClearProgressBar();
                throw new InvalidOperationException($"Incorrect parsing of builtin shader. Go to Tech Art: {shaderText.Substring(0, shaderText.Length > 1000 ? 1000 : shaderText.Length)}");
            }

            return matches[0];
        }

        public static string GetShaderName(string shaderText)
        {
            var shaderNamePart = GetShaderPartMatch(shaderText).Value;
            var shaderName = shaderNamePart.Split('\"')[1];
            return shaderName;
        }

        public static string GetBuiltinName(string name) => $"Builtin/{name}";

        public static string ReplacedName(string shaderText)
        {
            var match = GetShaderPartMatch(shaderText);
            var shaderNamePart = match.Value;
            var shaderParts = shaderNamePart.Split('\"');
            var shaderName = shaderParts[1];
            return $"Shader \"{GetBuiltinName(shaderName)}\"";
        }


        public static string ReplaceShaderName(string shaderText)
        {
            var match = GetShaderPartMatch(shaderText);
            return shaderText.Replace(match.Value, ReplacedName(shaderText));
        }

        public static int CheckAndReplaceShader(
            string assetPath,
            Dictionary<string, string> _shaderNameToShaderPath,
            List<Shader> _preloadedShaders,
            List<GUID> _buildingShaders,
            List<Material> _changedMaterials,
            GUID assetGuid = default)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material == null)
            {
                Logger.IfError()?.Message($"Cannot load material: {assetPath} and guid {assetGuid}").Write();
                return 1;
            }

            var shader = material.shader;

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(shader, out var guid, out long localId);
            if (guid != k_BuiltInGuid.ToString())
            {
                //Logger.IfWarn()?.Message($"This shader {shader.name} is not builtin.").Write();
                return 0;
            }

            var preloadedShader = _preloadedShaders.FirstOrDefault(s => s.name == GetBuiltinName(shader.name));
            if (preloadedShader == null)
                preloadedShader = AddNewBuiltinShader(shader.name, _shaderNameToShaderPath, _preloadedShaders);

            if (preloadedShader == null)
            {
                Logger.IfError()?.Message($"Cannot add builtin shader to project: {shader.name}").Write();
                return 1;
            }

            material.shader = preloadedShader;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(preloadedShader, out var shaderGuid, out long shaderLocalId);
            _buildingShaders.Add(new GUID(shaderGuid));
            _changedMaterials.Add(material);
            return 0;
        }

        public static Shader AddNewBuiltinShader(string shaderName, Dictionary<string, string> _shaderNameToShaderPath, List<Shader> _preloadedShaders)
        {
            if (!_shaderNameToShaderPath.ContainsKey(shaderName))
            {
                EditorUtility.ClearProgressBar();
                Logger.IfError()?.Message($"'Cannot find shader {shaderName} in {_shaderNameToShaderPath.Keys.Aggregate("", (s, s1) => s + " - " + s1)}").Write();
                return null;
            }

            var shaderPath = _shaderNameToShaderPath[shaderName];
            var newPath = $"{GetBuiltinFolder()}/{Path.GetFileName(shaderPath)}";
            File.Copy(shaderPath, newPath);
            var shaderText = ReplaceShaderName(File.ReadAllText(newPath));
            File.WriteAllText(newPath, shaderText);
            AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var shader = AssetDatabase.LoadAssetAtPath<Shader>(newPath);
            if (shader == null)
            {
                EditorUtility.ClearProgressBar();
                 Logger.IfError()?.Message("Cannot copy shader or change shader name automatically. Possible CultureInfo exception or Database update.").Write();;
                return null;
            }

            _preloadedShaders.Add(shader);
            return shader;
        }
        
        
    }
}