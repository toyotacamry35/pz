using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets.Src.App
{
    public class DependenciesCatalog
    {
        public struct DependencyCatalog
        {
            public Dictionary<string, string[]> Catalog;
            public string[] MapNames;
        }

        private DependencyCatalog _catalog;
        
        public async Task LoadCatalog(string catalogFolder)
        {
            var filePath = Path.Combine(catalogFolder, AssetBundleHelper.DependenciesCatalog);
            var catalog = await FileSystemAsync.Load(filePath);
            _catalog = JsonConvert.DeserializeObject<DependencyCatalog>(catalog);
        }

        public void SaveCatalog(string catalogFolder)
        {
            if (!Directory.Exists(catalogFolder))
                Directory.CreateDirectory(catalogFolder);
            
            var filePath = Path.Combine(catalogFolder, AssetBundleHelper.DependenciesCatalog);
            File.WriteAllText(filePath,JsonConvert.SerializeObject(_catalog));
        }
        
        public Dictionary<string, string[]> GetCatalog() => _catalog.Catalog;
        public string[] GetMaps() => _catalog.MapNames;

        public void FillCatalog(DependencyCatalog catalog) => _catalog = catalog;
    }
}