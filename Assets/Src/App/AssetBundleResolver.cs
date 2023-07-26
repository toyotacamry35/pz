using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Src.App
{
    public class AssetBundleResolver
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();
        
        public string[] MapsInCatalog { get; private set; }

        private Dictionary<string, string[]> _catalog;

        public async Task Load(string rootAssetBundleFolder)
        {
            var dependenciesCatalog = new DependenciesCatalog();
            await dependenciesCatalog.LoadCatalog(rootAssetBundleFolder);

            MapsInCatalog = dependenciesCatalog.GetMaps();
            _catalog = dependenciesCatalog.GetCatalog();
        }
        public void Prepare(Dictionary<string, AssetBundleCreateRequest> bundles, string rootAssetBundleFolder)
        {
            _bundles.Clear();
            
            foreach (var request in bundles)
            {
                var path = request.Key;
                var bundle = request.Value.assetBundle;
                var relative = PathExtension.GetRelativePath(path, rootAssetBundleFolder).Replace("\\", "/");
                if (!_catalog.ContainsKey(relative))
                {
                    Logger.IfWarn()?.Message($"Cannot find bundle {relative} in dependencies catalog.").Write();
                    continue;
                }

                var assets = _catalog[relative];
                foreach (var asset in assets)
                {
                    _bundles.Add(asset, bundle);
                }
            }
        }

        public AssetBundle Resolve(string path)
        {
            if (_bundles.ContainsKey(path))
                return _bundles[path];

            var withoutExtension = PathExtension.GetPathWithoutExtension(path);
            return _bundles.ContainsKey(withoutExtension) ? _bundles[withoutExtension] : null;
        }
    }
}