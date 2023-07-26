using System.IO;

namespace Assets.Src.App
{
    public static class PathExtension
    {
        public static string GetPathWithoutExtension(string path) =>
            !string.IsNullOrWhiteSpace(Path.GetExtension(path)) ? Path.ChangeExtension(path, null) : path;

        public static string GetDirectoryPath(string filePath) => filePath.Substring(0, filePath.Length - Path.GetFileName(filePath).Length);

        public static string GetRelativePath(string filePath, string folderPath)
        {
            var absoluteFilePath = Path.GetFullPath(filePath);
            var absoluteFolderPath = Path.GetFullPath(folderPath);
            return absoluteFilePath.Substring(absoluteFolderPath.Length + 1);
        }
    }
}