using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Tools.Editor
{
    public class CyrFilenamesFinder
    {
        private const float RepaintPeriod = 0.5f;
        private const float BatchFilesCount = 100;

        private IEnumerator _searchRoutine;

        private int _passedFilesCount;
        private int _totalFilesCount;
        private int _wrongFilesCount;
        private IEnumerator _enumerator;
        private float _lastRepaintTime;


        //=== Props ===========================================================

        private bool IsRunning => _enumerator != null;

        private float Progress => (float) _passedFilesCount / _totalFilesCount;


        //=== Unity ===========================================================

        [MenuItem("Tools/Find non-latin symbols in project filenames", false, 12), UsedImplicitly]
        static void FindCyrFilenames()
        {
            new CyrFilenamesFinder().Run();
        }

        private void Update()
        {
            if (!IsRunning)
                return;

            if (!_enumerator.MoveNext())
            {
                AtEnd();
                return;
            }

            if (Time.realtimeSinceStartup - _lastRepaintTime > RepaintPeriod)
            {
                _lastRepaintTime = Time.realtimeSinceStartup;
                EditorUtility.DisplayProgressBar(
                    "Поиск файлов с неправильными символами:", $"{_passedFilesCount} из {_totalFilesCount}", Progress);
            }
        }


        //=== Public ==============================================================

        private void Run()
        {
            var allFilesPaths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
            _enumerator = Check(allFilesPaths);
            EditorApplication.update += Update;
        }


        //=== Private =========================================================

        private IEnumerator Check(string[] checkedFullPaths)
        {
            if ((checkedFullPaths?.Length ?? 0) == 0)
                yield break;

            _wrongFilesCount = 0;
            _passedFilesCount = 0;
            _totalFilesCount = checkedFullPaths.Length;
            foreach (var filename in checkedFullPaths)
            {
                _passedFilesCount++;
                if (filename.EndsWith(".meta"))
                    continue;

                if (IsContainsNonLatinSymbols(filename, out var descr, out var go))
                {
                    _wrongFilesCount++;
                    Debug.LogError($"File/folder contains wrong symbols '{filename}': {descr}", go);
                }

                if (_passedFilesCount % BatchFilesCount == 0)
                    yield return null;
            }
        }

        private void AtEnd()
        {
            if (EditorApplication.update != null)
                EditorApplication.update -= Update;
            _enumerator = null;
            EditorUtility.ClearProgressBar();

            if (_wrongFilesCount > 0)
                Debug.LogError($"Total wrong filenames: {_wrongFilesCount}");
            else
                Debug.Log("No wrong filenames found. OK");
        }

        private bool IsContainsNonLatinSymbols(string fullPath, out string description, out UnityEngine.Object go)
        {
            description = null;
            go = null;
            var lastSlashIndex = fullPath.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase);
            var name = lastSlashIndex < 0 || lastSlashIndex + 1 >= fullPath.Length
                ? fullPath
                : fullPath.Substring(lastSlashIndex + 1);

            if (name.All(IsLatinChar))
                return false;

            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var chr = name[i];
                if (IsLatinChar(chr))
                    sb.Append(chr);
                else
                    sb.Append($"<{chr}>");
            }

            go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetRelPath(fullPath));
            description = sb.ToString();
            return true;
        }

        private bool IsLatinChar(char chr)
        {
            return chr >= ' ' && chr <= '~'; //latin part
        }

        private string GetRelPath(string fullPath)
        {
            if (!fullPath.StartsWith(Application.dataPath)) // X:/path/projectName/Assets
                return fullPath;

            return "Assets" + fullPath.Substring(Application.dataPath.Length);
        }
    }
}