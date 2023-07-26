using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResourcesSystem.Loader
{
    public class FolderLoader : ILoader
    {
        public const string JdbExtension = ".jdb";
        public const string BinaryExtension = ".bdb";

        public static readonly HashSet<string> Extensions = new HashSet<string> { JdbExtension, BinaryExtension, };

        private readonly string _root;

        public FolderLoader(string root)
        {
            if (!string.IsNullOrEmpty(root))
                _root = root.TrimEnd('\\', '/');
        }

        public static string CleanUpRelativePath(string relativePath)
        {
            relativePath = Path.ChangeExtension(
                relativePath.Replace("\\", "/").TrimStart('/'),
                null
            );
            const string trimString = "Assets";
            if (relativePath.StartsWith(trimString))
                relativePath = relativePath.Substring(trimString.Length);


            return "/" + relativePath;
        }

        public static string ToFullPath(string root, string path)
        {
            var fullPathJdb = BuildFullPath(root, path, JdbExtension);
            if (File.Exists(fullPathJdb))
                return fullPathJdb;

            var fullPathBdb = BuildFullPath(root, path, BinaryExtension);
            if (File.Exists(fullPathBdb))
                return fullPathBdb;

            return fullPathJdb;
        }

        private static string BuildFullPath(string root, string relativePath, string extension)
        {
            return ToFullPathNoJdb(root, relativePath) + extension;
        }

        private static string ToFullPathNoJdb(string root, string relativePath)
        {
            return root == null ? relativePath : new Uri(Path.Combine(root, relativePath.TrimStart('/'))).LocalPath;
        }

        public Stream OpenRead(string path)
        {
            if (_root == null)
                GameResources.ThrowError<NullReferenceException>($"Root in {path} is null");

            var fullPath = ToFullPath(_root, path);
            if (!File.Exists(fullPath))
                GameResources.ThrowError($"{path} doesn't exit");

            return File.OpenRead(fullPath);
        }

        public bool IsExists(string path)
        {
            return File.Exists(ToFullPath(_root, path));
        }

        public bool IsBinary(string path)
        {
            return File.Exists(BuildFullPath(_root, path, BinaryExtension));
        }

        List<string> _allRoots;
        public IEnumerable<string> AllPossibleRoots { get
        {
            _allRoots = GetAllFiles(_root).ToList();
            return _allRoots;
        } }
        private static IEnumerable<string> GetAllFiles(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
                yield break;

            var enumerator = Directory
                .EnumerateFiles(root, "*.jdb", SearchOption.AllDirectories)
                .Where(s => Extensions.Contains(Path.GetExtension(s)))
                .Select(x => x.Substring(root.Length, x.Length - root.Length - ".jdb".Length).Replace("\\", "/"));
            foreach (var file in enumerator)
                yield return file;

        }

        public Stream OpenWrite(string path)
        {
            if (_root == null)
                GameResources.ThrowError<NullReferenceException>($"Root in {path} is null");

            var fullPath = ToFullPath(_root, path);
            return File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public string GetRoot()
        {
            return _root;
        }

        public override string ToString()
        {
            return $"{GetType().Name} {_root}";
        }
    }
}