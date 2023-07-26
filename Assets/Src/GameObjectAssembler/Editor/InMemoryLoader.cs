using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assets.Src.RubiconAI.Editor
{
    public class InMemoryLoader : global::ResourcesSystem.Loader.ILoader
    {
        private readonly Dictionary<string, MemoryStream> _pathToFileContents = new Dictionary<string, MemoryStream>();

        public Stream OpenRead(string path)
        {
            return _pathToFileContents[path];
        }

        public Stream OpenWrite(string path)
        {
            if (!_pathToFileContents.TryGetValue(path, out var ret))
            {
                ret = new MemoryStream();
                _pathToFileContents.Add(path, ret);
            }
            
            return ret;
        }

        public IEnumerable<string> ListAllIn(string path)
        {
            throw new System.NotImplementedException();
        }

        public string GetRoot()
        {
            throw new System.NotImplementedException();
        }

        public bool IsExists(string path)
        {
            return true;
        }

        public bool IsBinary(string path)
        {
            return false;
        }

        public IEnumerable<string> AllPossibleRoots => _pathToFileContents.Keys;

        public void KnownFilesAdd(string path, StringBuilder stringBuilder)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            _pathToFileContents[path] = ms;
        }
    }
}
