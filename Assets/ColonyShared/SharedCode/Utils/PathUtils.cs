using System.IO;
using System.Threading.Tasks;

namespace SharedCode.Utils
{
    public static class PathUtils
    {
        public static string DataPath { get; set; } = System.Environment.CurrentDirectory;

        public static string GetStartupDirectory() => DataPath;

        public static Task<string> GetDumpDirectory()
        {
            var dumpDirectory = System.IO.Path.Combine(GetStartupDirectory(), "RepositoryDumps");
            if (!System.IO.Directory.Exists(dumpDirectory))
                System.IO.Directory.CreateDirectory(dumpDirectory);
            return Task.FromResult(dumpDirectory);
        }

        public static Task<string> GetEntityDiffsDirectory()
        {
            var dumpDirectory = System.IO.Path.Combine(GetStartupDirectory(), "EntityDiffs");
            if (!System.IO.Directory.Exists(dumpDirectory))
                System.IO.Directory.CreateDirectory(dumpDirectory);
            return Task.FromResult(dumpDirectory);
        }

        public static async Task DumpData(string fileName, byte[] data)
        {
            var fullFileName = System.IO.Path.Combine(await PathUtils.GetDumpDirectory(), fileName + ".repdmp");
            if (File.Exists(fullFileName))
                File.Delete(fullFileName);
            using (var fileStream = File.Create(fullFileName))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
                await fileStream.FlushAsync();
            }
        }
    }
}
