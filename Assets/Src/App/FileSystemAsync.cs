using System.IO;
using System.Threading.Tasks;

namespace Assets.Src.App
{
    public class FileSystemAsync
    {
        public static async Task<string> Load(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true))
            using (var sr = new StreamReader(stream))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}