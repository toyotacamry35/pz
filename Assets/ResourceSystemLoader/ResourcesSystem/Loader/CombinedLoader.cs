using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResourcesSystem.Loader
{
    public class CombinedLoader : ILoader
    {
        private readonly ILoader[] _loaders;

        public CombinedLoader(params ILoader[] loaders)
        {
            _loaders = loaders;
        }

        public IEnumerable<string> AllPossibleRoots => _loaders.SelectMany(v => v.AllPossibleRoots).Distinct();

        public string GetRoot() => throw new System.NotImplementedException();

        public bool IsExists(string path) => _loaders.Any(v => v.IsExists(path));
        public bool IsBinary(string path) => _loaders.Any(v => v.IsBinary(path));

        public Stream OpenRead(string path)
        {
            foreach (var loader in _loaders)
            {
                if (loader.IsExists(path))
                    return loader.OpenRead(path);
            }

            throw new FileNotFoundException("Jdb not found: ", path);
        }

        public Stream OpenWrite(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}