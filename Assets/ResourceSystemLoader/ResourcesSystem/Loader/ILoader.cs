using System.Collections.Generic;
using System.IO;

namespace ResourcesSystem.Loader
{
    public interface ILoader
    {
        IEnumerable<string> AllPossibleRoots { get; }
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        string GetRoot();
        bool IsExists(string path);
        bool IsBinary(string path);
    }
}