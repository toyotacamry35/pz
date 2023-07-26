using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ResourcesSystem.Loader
{
    public class ZipFileLoader : ILoader
    {
        private readonly ZipArchive archive;
        private readonly string rootPathInArchive;

        public ZipFileLoader(ZipArchive archive, string rootPathInArchive)
        {
            this.archive = archive;
            this.rootPathInArchive = rootPathInArchive;
            this.rootPathInArchive = this.rootPathInArchive.Replace('\\', Path.DirectorySeparatorChar);
            this.rootPathInArchive = this.rootPathInArchive.Replace('/', Path.AltDirectorySeparatorChar);
            this.rootPathInArchive = this.rootPathInArchive.TrimEnd(Path.DirectorySeparatorChar);
            this.rootPathInArchive = this.rootPathInArchive.TrimEnd(Path.AltDirectorySeparatorChar);
        }

        private string ToFullPath(string root, string path)
        {
            var fullPath = BuildFullPath(root, path, FolderLoader.JdbExtension);
            return archive.GetEntry(fullPath) != null ? fullPath : BuildFullPath(root, path, FolderLoader.BinaryExtension);
        }

        private static string BuildFullPath(string root, string relativePath, string extension)
        {
            return ToFullPathNoJdb(root, relativePath) + extension;
        }

        private static string ToFullPathNoJdb(string root, string relativePath)
        {
            if (root == null)
                return relativePath;
            var relPath = relativePath;
            if (Path.IsPathRooted(relativePath))
                relPath = relativePath.TrimStart('/');
            var result = root + "/" + relPath;
            var uri = new Uri("file:///" + result);
            var escaped = uri.AbsolutePath.TrimStart('/');
            var unescaped = Uri.UnescapeDataString(escaped);
            return unescaped;
        }

        public Stream OpenRead(string path)
        {
            var fullPath = ToFullPath(rootPathInArchive, path);
            var entry = archive.GetEntry(fullPath);
            if (entry == null)
                GameResources.ThrowError($"{path} doesn't exit");

            return entry.Open();
        }

        public bool IsExists(string path)
        {
            return archive.GetEntry(ToFullPath(rootPathInArchive, path)) != null;
        }

        public bool IsBinary(string path)
        {
            return archive.GetEntry(BuildFullPath(rootPathInArchive, path, FolderLoader.BinaryExtension)) != null;
        }

        public IEnumerable<string> AllPossibleRoots => GetAllFiles();

        private IEnumerable<string> GetAllFiles()
        {
            var e1 = archive.Entries.Where(v => v.FullName.StartsWith(rootPathInArchive));
            var e2 = e1.Select(v => v.FullName);
            var e3 = e2.Where(v => FolderLoader.Extensions.Contains(Path.GetExtension(v)));
            return e3.Select(v => Path.ChangeExtension(v, null).Remove(0, rootPathInArchive.Length));
        }

        public Stream OpenWrite(string path) => throw new NotImplementedException();

        public string GetRoot() => throw new InvalidOperationException("Zip file loader has no root");
        
        public override string ToString()
        {
            return $"{GetType().Name} {archive} {rootPathInArchive}";
        }
    }
}