using System.IO;
using System.Reflection;

namespace Core.Environment.Resources
{
    public static class ResourceSystemInit
    {
        public static DirectoryInfo ResourceRootDefaultLocation
        {
            get
            {
                var executableLocation = new FileInfo(Assembly.GetExecutingAssembly().Location);
                var dirInfo = executableLocation.Directory;
                while (dirInfo != null)
                {
                    var gameResourcesDir = new DirectoryInfo(Path.Combine(dirInfo.FullName, "GameResources"));
                    if (gameResourcesDir.Exists)
                        return gameResourcesDir;

                    var assetsDir = new DirectoryInfo(Path.Combine(dirInfo.FullName, "Assets"));
                    if (assetsDir.Exists)
                        return assetsDir;

                    dirInfo = dirInfo.Parent;
                }

                throw new DirectoryNotFoundException($"Can't find Resource System Root from dll {executableLocation}");
            }
        }
    }
}
