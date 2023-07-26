using System;
using System.IO;

namespace Assets.Src.App
{
    public static class AssetBundleHelper
    {
        public static string DependenciesCatalog => "dependencies.json";
        public static string SharedName => "Shared";
        public static string SystemName => "System";

        public static string ProduceMapBundleFolderName(string mapName) => $"{mapName}-Pack";

        public static string ProduceBundleFolderName(string mapName) => mapName != SharedName && mapName != SystemName ? ProduceMapBundleFolderName(mapName) : mapName;

        public static string ProduceBuiltinShaderBundleName(string mapName) => $"{mapName}-UnityShaders.bundle";
        public static string ProduceSharedBuiltinShaderBundleName() => $"Shared-A-UnityShaders.bundle";
        public static string ProduceCustomShaderBundleName(string mapName) => $"{mapName}-Custom-UnityShaders.bundle";

        public static string ProduceBundleNameByPath(string name, string guid)
        {
            var filename = Path.GetFileNameWithoutExtension(name);
            if (string.IsNullOrWhiteSpace(filename))
                throw new InvalidOperationException($"Cannot produce filename for file with null filename without extension:{name}");

            if (filename.Length + guid.Length + 1 <= 250)
                filename = $"{filename}_{guid}";
            else
            {
                var currentNumber = filename.Length + guid.Length + 1;
                var otherNumber = filename.Length + 1;
                filename = $"{filename}_{guid.Substring(currentNumber - otherNumber)}";
            }

            return $"{filename}.bundle";
        }
    }
}