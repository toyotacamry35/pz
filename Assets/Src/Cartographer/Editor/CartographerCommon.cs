using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using System;
using SharedCode.Aspects.Cartographer;
using System.Linq;
using System.Runtime.InteropServices;
using AwesomeTechnologies.VegetationStudio;
using Assets.TerrainBlend;
using System.Reflection;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Cartographer.Editor
{
    public enum SceneLoaderGameObjectType
    {
        GameObjectTerrain,
        GameObjectStatic,
        GameObjectFX
    }

    static class GameObjectExtensions
    {
        private static bool Requires(Type obj, Type requirement)
        {
            //also check for m_Type1 and m_Type2 if required
            return Attribute.IsDefined(obj, typeof(RequireComponent)) && Attribute.GetCustomAttributes(obj, typeof(RequireComponent)).OfType<RequireComponent>().Any(rc => rc.m_Type0.IsAssignableFrom(requirement));
        }

        internal static bool CanDestroy(this GameObject gameObject, Type t)
        {
            if (gameObject != null)
            {
                return !(gameObject.GetComponents<Component>().Any(c => { if (c != null) {return Requires(c.GetType(), t);} else {return false;}}));
            }
            return true;
        }
    }

    public static class CopyComponentHelper
    {
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        public static T AddCopyOfComponent<T>(this GameObject target, T sourceComponent) where T : Component
        {
            var targetComponent = target.GetComponent<T>();
            if (targetComponent == null)
            {
                targetComponent = target.AddComponent<T>();
            }
            if (targetComponent != null)
            {
                targetComponent.GetCopyOf(sourceComponent);
            }
            return targetComponent as T;
        }
    }

    public class CartographerCommon
    {
        public class KeyCount<KeyType>
        {
            public KeyType Key { get; set; } = default(KeyType);
            public int Count { get; set; } = 0;
        }

        public static void AddKey<KeyType>(KeyType key, Dictionary<KeyType, KeyCount<KeyType>> keys)
        {
            KeyCount<KeyType> value;
            if (keys.TryGetValue(key, out value))
            {
                value.Count += 1;
            }
            else
            {
                keys.Add(key, new KeyCount<KeyType>() { Key = key, Count = 1 });
            }
        }

        public class FormatArguments
        {
            public bool Verboose { get; set; } = false;
            public bool InsertSpaces { get; set; } = true;
            public int Spaces { get; set; } = 4;
            public string Delimiter { get; set; } = ", ";

            public void CopyFrom(FormatArguments from)
            {
                if (from != null)
                {
                    Verboose = from.Verboose;
                    InsertSpaces = from.InsertSpaces;
                    Spaces = from.Spaces;
                    Delimiter = from.Delimiter;
                }
            }
        }

        public interface IPrintable
        {
            string GetTitle(object context, FormatArguments formatArguments);
            string GetPrint(object context, FormatArguments formatArguments);
        }

        public class MessagesClass
        {
            public string YesButton { get { return "Yes"; } }
            public string NoButton { get { return "No"; } }
        }
        public static MessagesClass Messages { get; } = new MessagesClass();

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly string UnityExtension = ".unity";
        public static readonly string AssetExtension = ".asset";
        public static readonly string TxtExtension = ".txt";
        public static readonly string PrefabExtension = ".prefab";
        public static readonly string MatExtension = ".mat";
        public static readonly string PngExtension = ".png";
        public static readonly string FbxExtension = ".fbx";

        private static readonly char[] streamSceneNameSegmentDelimiters = new char[] { '_', '.' };
        private static readonly char[] assetPathDelimiters = new char[] { '\\', '/' };

        // Copy helper ----------------------------------------------------------------------------
        public static void MakeAdditionalReplacement(GameObject destinationGameObject, GameObject sourceGameObject, GameObject sourcePrefab)
        {
            {
                var meshBlending = sourceGameObject.GetComponent<MeshBlending>();
                if (meshBlending != null)
                {
                    destinationGameObject.AddCopyOfComponent(meshBlending);
                }
            }
            {
                var light = sourceGameObject.GetComponent<Light>();
                if (light != null)
                {
                    destinationGameObject.AddCopyOfComponent(light);
                }
            }
            {
                var particleSystem = sourceGameObject.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    destinationGameObject.AddCopyOfComponent(particleSystem);
                }
            }
        }

        // GameObject identification helpers ------------------------------------------------------
        public static string GetFullName(GameObject gameObject)
        {
            var result = string.Empty;
            var parent = gameObject;
            while (parent != null)
            {
                if (result.Length > 0)
                {
                    result = $"{parent.name}/{result}";
                }
                else
                {
                    result = parent.name;
                }
                parent = parent.transform.parent?.gameObject ?? null;
            }
            return result.ToString();
        }

        public static string GetAssetPath(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                return AssetDatabase.GUIDToAssetPath(guid);
            }
            return string.Empty;
        }

        public static string GetAssetPath(UnityEngine.Object assetInstance)
        {
            if (assetInstance != null)
            {
                return AssetDatabase.GetAssetPath(assetInstance);
            }
            return string.Empty;
        }

        public static string GetAssetID(UnityEngine.Object assetInstance, bool guidOnly)
        {
            if (assetInstance != null)
            {
                string guid;
                long localId;
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(assetInstance, out guid, out localId);
                return guidOnly ? $"{guid}" : $"{guid}_{localId}";
            }
            return string.Empty;
        }

        public class PrefabSearch
        {
            private static char[] prefabSearchParrern = new char[] { '(' };

            public static string FixNameForPrefabSearch(string gameObjectName)
            {
                var index = gameObjectName.IndexOfAny(prefabSearchParrern);
                if (index > 0)
                {
                    return gameObjectName.Substring(0, index).Trim();
                }
                else
                {
                    return gameObjectName.Trim();
                }
            }

            private Dictionary<string, string[]> findAssetsCache = new Dictionary<string, string[]>();

            public string[] FindAssets(string searchPattern)
            {
                string[] result;
                if (!findAssetsCache.TryGetValue(searchPattern, out result))
                {
                    result = AssetDatabase.FindAssets(searchPattern);
                    findAssetsCache.Add(searchPattern, result);
                }
                return result;
            }
        }

        // Print helpers --------------------------------------------------------------------------
        public static void AppendIfNotEmpty(StringBuilder buffer, string delimiter)
        {
            if (buffer.Length > 0)
            {
                buffer.Append(delimiter);
            }
        }

        public static List<TValue> GetListFromDictionary<Tkey, TValue>(Dictionary<Tkey, TValue> dictionary, Func<TValue, TValue, int> comparer, Action<TValue> counter)
        {
            var result = new List<TValue>(dictionary.Values);
            if (comparer != null)
            {
                result.Sort((x, y) => { return comparer(x, y); });
            }
            if (counter != null)
            {
                foreach (var value in result)
                {
                    counter(value);
                }
            }
            return result;
        }

        public static void CollectIntDictionary<TKey>(Dictionary<TKey, int> dictionary, TKey value)
        {
            if (dictionary.ContainsKey(value))
            {
                dictionary[value] += 1;
            }
            else
            {
                dictionary.Add(value, 1);
            }
        }

        public static List<string> FixLines(List<string> lines, FormatArguments formatArguments)
        {
            var fixedLines = new List<string>();
            if ((lines != null) && (lines.Count > 0))
            {
                if (formatArguments.InsertSpaces)
                {
                    var sizes = new List<int>();
                    foreach (var line in lines)
                    {
                        int sizeIndex = 0;
                        int size = 0;
                        for (int index = 0; index < line.Length; ++index)
                        {
                            if (line[index] == '\t')
                            {
                                if (sizeIndex >= sizes.Count)
                                {
                                    sizes.Add(0);
                                }
                                if (sizes[sizeIndex] < size)
                                {
                                    sizes[sizeIndex] = size;
                                }
                                size = 0;
                                ++sizeIndex;
                            }
                            else
                            {
                                ++size;
                            }
                        }
                    }
                    var newLine = new StringBuilder();
                    foreach (var line in lines)
                    {
                        newLine.Clear();
                        int sizeIndex = 0;
                        int size = 0;
                        for (int index = 0; index < line.Length; ++index)
                        {
                            if (line[index] == '\t')
                            {
                                int spaceCount = sizes[sizeIndex] - size + formatArguments.Spaces;
                                for (int spaceIndex = 0; spaceIndex < spaceCount; ++spaceIndex)
                                {
                                    newLine.Append(' ');
                                }
                                size = 0;
                                ++sizeIndex;
                            }
                            else
                            {
                                newLine.Append(line[index]);
                                ++size;
                            }
                        }
                        fixedLines.Add(newLine.ToString());
                    }
                }
                else
                {
                    foreach (var line in lines)
                    {
                        fixedLines.Add(line);
                    }
                }
            }
            return fixedLines;
        }

        public static void PrintPrintableList<StatsType>(List<string> lines, List<StatsType> elements, string label, object context, FormatArguments formatArguments, bool addEmptyLine) where StatsType : IPrintable
        {
            if (elements.Count > 0)
            {
                if (addEmptyLine) { lines.Add(string.Empty); }
                if (!string.IsNullOrEmpty(label)) { lines.Add(label); }
                var newLines = new List<string>();
                newLines.Add(elements[0].GetTitle(context,formatArguments));
                foreach (var element in elements)
                {
                    newLines.Add(element.GetPrint(context, formatArguments));
                }
                lines.AddRange(FixLines(newLines, formatArguments));
            }
        }

        public static void PrintIntDictionary<Tkey>(List<string> lines, Dictionary<Tkey, int> dictionary, string label, bool sort, FormatArguments formatArguments, bool addEmptyLine)
        {
            if (dictionary.Count > 0)
            {
                if (addEmptyLine) { lines.Add($""); }
                if (!string.IsNullOrEmpty(label)) { lines.Add(label); }
                var list = dictionary.ToList();
                if (sort)
                {
                    list.Sort((x, y) =>
                    {
                        var compare = y.Value.CompareTo(x.Value);
                        if (compare == 0)
                        {
                            return x.Key.ToString().CompareTo(y.Key.ToString());
                        }
                        else
                        {
                            return compare;
                        }
                    });
                }
                var newLines = new List<string>();
                foreach (var element in list)
                {
                    newLines.Add($"{element.Value}\t{element.Key.ToString()}");
                }
                lines.AddRange(FixLines(newLines, formatArguments));
            }
        }

        public static void PrintList(List<string> lines, List<string> list, string label, bool sort, FormatArguments formatArguments, bool addEmptyLine)
        {
            if (list.Count > 0)
            {
                if (addEmptyLine) { lines.Add($""); }
                if (!string.IsNullOrEmpty(label)) { lines.Add(label); }
                if (sort) { list.Sort((x, y) => y.CompareTo(x)); }
                var newLines = new List<string>();
                foreach (var element in list)
                {
                    newLines.Add($"{element}");
                }
                lines.AddRange(FixLines(newLines, formatArguments));
            }
        }

        public static string GetGameObjectPrint(GameObject gameObject)
        {
            var sceneName = gameObject.scene.name;
            var name = gameObject.name;
            var fullName = GetFullName(gameObject);
            return $"{sceneName}\t{name}\t{fullName}";
        }

        public static string GetGameObjectPrintAsPrefab(GameObject gameObject)
        {
            var sceneName = gameObject.scene.name;
            var name = gameObject.name;
            var fullName = GetFullName(gameObject);
            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            var prefabType = PrefabUtility.GetPrefabAssetType(gameObject);
            var prefabPath = string.Empty;
            var prefabGameObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabGameObject != null)
            {
                prefabPath = GetAssetPath(prefabGameObject);
            }
            return $"{sceneName}\t{name}\t{fullName}\t{prefabStatus}\t{prefabType}\t{prefabPath}";
        }

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void ShowTextFile(string resultFilePath, bool checkSize, bool bringToTop)
        {
            if (checkSize)
            {
                var fileInfo = new FileInfo(resultFilePath);
                if (fileInfo.Length == 0)
                {
                    return;
                }
            }
            var shellProcess = new System.Diagnostics.Process();
            shellProcess.StartInfo.UseShellExecute = true;
            shellProcess.StartInfo.FileName = resultFilePath;
            shellProcess.Start();
            if (bringToTop)
            {
                shellProcess.WaitForInputIdle();
                if (shellProcess.MainWindowHandle != null)
                {
                    SetForegroundWindow(shellProcess.MainWindowHandle);
                }
            }
        }

        // Is editor ------------------------------------------------------------------------------
        public static bool IsEditor()
        {
            return Application.isEditor && !Application.isPlaying;
        }

        // Message reporting ----------------------------------------------------------------------
        public static void ReportError(string message)
        {
            Logger.IfError()?.Message(message).Write();
        }

        public static void ReportWarning(string message)
        {
            Logger.IfWarn()?.Message(message).Write();
        }

        public static void ReportInfo(string message)
        {
            Logger.IfInfo()?.Message(message);
        }

        // Asset file names and paths helpers -----------------------------------------------------
        public static string GetAssetExtensionFromAssetPath(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                var index = assetPath.LastIndexOf('.');
                if (index >= 0)
                {
                    return assetPath.Substring(index);
                }
            }
            return string.Empty;
        }

        public static string GetAssetNameFromAssetPath(string assetPath, string extension)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return string.Empty;
            }
            else
            {
                if (assetPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    assetPath = assetPath.Substring(0, assetPath.Length - extension.Length);
                }
                var index = assetPath.LastIndexOfAny(assetPathDelimiters);
                if (index >= 0)
                {
                    if (index == (assetPath.Length - 1))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return assetPath.Substring(index + 1);
                    }
                }
                else
                {
                    return assetPath;
                }
            }
        }

        public static string CombineAssetPath(string path, string additionalPath, string checkForExtension)
        {
            var result = path;
            if (string.IsNullOrEmpty(result))
            {
                result = string.Empty;
            }
            else
            {
                result = result.Replace('\\', '/');
            }
            if (!string.IsNullOrEmpty(additionalPath))
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = additionalPath.Replace('\\', '/');
                }
                else
                {
                    result = Path.Combine(result, additionalPath).Replace('\\', '/');
                }
            }
            if (!string.IsNullOrEmpty(checkForExtension) && !string.IsNullOrEmpty(result))
            {
                if (!result.EndsWith(checkForExtension, StringComparison.OrdinalIgnoreCase))
                {
                    result = result + checkForExtension;
                }
            }
            return result;
        }

        // Memory cleaning ------------------------------------------------------------------------
        public static void CleanupMemory()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            GC.Collect();
        }

        // Stream scene names helpers -------------------------------------------------------------
        public static bool IsSceneForStreaming(string sceneName, out Vector3Int coordinates)
        {
            coordinates = new Vector3Int(0, 0, 0);
            if (!string.IsNullOrEmpty(sceneName))
            {
                var segments = sceneName.Split(streamSceneNameSegmentDelimiters, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 1)
                {
                    var coordinatesAquired = 0;
                    for (var index = 1; index < segments.Length; ++index)
                    {
                        var coordinate = segments[index][0];
                        var value = segments[index].Substring(1);
                        if (coordinate == 'x' || coordinate == 'X')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                coordinates.x = _value;
                                ++coordinatesAquired;
                            }
                        }
                        else if (coordinate == 'y' || coordinate == 'Y')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                coordinates.y = _value;
                                ++coordinatesAquired;
                            }
                        }
                        else if (coordinate == 'z' || coordinate == 'Z')
                        {
                            int _value;
                            if (int.TryParse(value, out _value))
                            {
                                coordinates.z = _value;
                                ++coordinatesAquired;
                            }
                        }
                    }
                    if (coordinatesAquired > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsSceneForStreaming(string sceneName)
        {
            Vector3Int coordinates;
            return IsSceneForStreaming(sceneName, out coordinates);
        }

        public static string GetStreamSceneAssetName(Vector3Int sceneCoordinates, SceneCollectionDef sceneCollection)
        {
            return sceneCollection.ScenePrefix + (sceneCollection.CollectByX ? $"_x{sceneCoordinates.x}" : "") + (sceneCollection.CollectByY ? $"_y{sceneCoordinates.y}" : "") + (sceneCollection.CollectByZ ? $"_z{sceneCoordinates.z}" : "");
        }

        public static Vector3Int GetStreamSceneCoordinates(Vector3 position, SceneCollectionDef sceneCollection)
        {
            var result = new Vector3Int(0, 0, 0);
            if (sceneCollection.CollectByX)
            {
                result.x = Mathf.FloorToInt(position.x / sceneCollection.SceneSize.x);
            }
            if (sceneCollection.CollectByY)
            {
                result.y = Mathf.FloorToInt(position.y / sceneCollection.SceneSize.y);
            }
            if (sceneCollection.CollectByZ)
            {
                result.z = Mathf.FloorToInt(position.z / sceneCollection.SceneSize.z);
            }
            return result;
        }

        public static int GetStreamScenePackedIndex(Vector3Int coordinates, SceneCollectionDef sceneCollection)
        {
            return (coordinates.x - sceneCollection.SceneStart.x) + ((coordinates.z - sceneCollection.SceneStart.z) << 16);
        }

        public static Vector3Int GetStreamSceneCoordinates(int packedIndex, SceneCollectionDef sceneCollection)
        {
            var z = packedIndex >> 16;
            var x = packedIndex - (z << 16);
            return new Vector3Int(x + sceneCollection.SceneStart.x, 0, z + sceneCollection.SceneStart.z);
        }

        // Geometry helpers -----------------------------------------------------------------------
        private static float Lerp(float s, float e, float t)
        {
            return s + (e - s) * t;
        }

        private static float Blerp(float c00, float c10, float c01, float c11, float t0, float t1)
        {
            return Lerp(Lerp(c00, c10, t0), Lerp(c01, c11, t0), t1);
        }

        public static float[,] Resample(float[,] input, int pow)
        {
            var delimiter = 1 << pow;

            var size0 = input.GetLength(0);
            var size1 = input.GetLength(1);

            var resultSize0 = input.GetLength(0) / delimiter + (size0 & 0x01);
            var resultSize1 = input.GetLength(1) / delimiter + (size1 & 0x01);

            float[,] result = new float[resultSize0, resultSize1];
            for (var index0 = 0; index0 < resultSize0; ++index0)
            {
                for (var index1 = 0; index1 < resultSize1; ++index1)
                {
                    var g0 = ((float)index0) / resultSize0 * (size0 - 1);
                    var g1 = ((float)index1) / resultSize1 * (size1 - 1);
                    var g0i = (int)g0;
                    var g1i = (int)g1;

                    var c00 = input[g0i, g1i];
                    var c10 = input[g0i + 1, g1i];
                    var c01 = input[g0i, g1i + 1];
                    var c11 = input[g0i + 1, g1i + 1];

                    result[index0, index1] = Blerp(c00, c10, c01, c11, g0 - g0i, g1 - g1i);
                }
            }
            return result;
        }

        public static float[,,] Resample(float[,,] input, int pow)
        {
            var delimiter = 1 << pow;

            var size0 = input.GetLength(0);
            var size1 = input.GetLength(1);
            var size2 = input.GetLength(2);

            var resultSize0 = input.GetLength(0) / delimiter + (size0 & 0x01);
            var resultSize1 = input.GetLength(1) / delimiter + (size1 & 0x01);

            float[,,] result = new float[resultSize0, resultSize1, size2];
            for (var index0 = 0; index0 < resultSize0; ++index0)
            {
                for (var index1 = 0; index1 < resultSize1; ++index1)
                {
                    for (var index2 = 0; index2 < size2; ++index2)
                    {
                        var g0 = ((float)index0) / resultSize0 * (size0 - 1);
                        var g1 = ((float)index1) / resultSize1 * (size1 - 1);
                        var g0i = (int)g0;
                        var g1i = (int)g1;

                        var c00 = input[g0i, g1i, index2];
                        var c10 = input[g0i + 1, g1i, index2];
                        var c01 = input[g0i, g1i + 1, index2];
                        var c11 = input[g0i + 1, g1i + 1, index2];

                        result[index0, index1, index2] = Blerp(c00, c10, c01, c11, g0 - g0i, g1 - g1i);
                    }
                }
            }
            return result;
        }

        public static float GetVectorDifference(Vector3 target)
        {
            var x = Mathf.Abs(target.x);
            var y = Mathf.Abs(target.y);
            var z = Mathf.Abs(target.z);
            var max = Mathf.Max(x, Mathf.Max(y, z));
            var min = Mathf.Min(x, Mathf.Min(y, z));
            if  (min < Mathf.Epsilon)
            {
                return (max < Mathf.Epsilon) ? 1.0f : float.MaxValue;
            }
            return max / min;
        }

        // Inside helpers -------------------------------------------------------------------------
        public static bool InsideRect(Vector3Int coordinates, CartographerParamsDef.OperationArea operationArea)
        {
            if (operationArea.Active)
            {
                return InsideRect(coordinates, operationArea.Start, operationArea.Count);
            }
            return false;
        }

        public static bool InsideRect(Vector3Int coordinates, SharedCode.Utils.Vector3Int start, SharedCode.Utils.Vector3Int count)
        {
            return (coordinates.x >= start.x) && (coordinates.z >= start.z) && (coordinates.x < (start.x + count.x)) && (coordinates.z < (start.z + count.z));
        }

        public static bool InsideRect(Vector3Int coordinates, RectInt rect, SceneCollectionDef sceneCollection)
        {
            return (!sceneCollection.CollectByX || ((rect.xMin <= coordinates.x) && (rect.xMax > coordinates.x))) &&
                   (!sceneCollection.CollectByZ || ((rect.yMin <= coordinates.z) && (rect.yMax > coordinates.z)));
        }

        public static bool InsideSceneCollection(Vector3Int coordinates, SceneCollectionDef sceneCollection)
        {
            return (!sceneCollection.CollectByX || ((sceneCollection.SceneStart.x <= coordinates.x) && ((sceneCollection.SceneStart.x + sceneCollection.SceneCount.x) > coordinates.x))) &&
                   (!sceneCollection.CollectByY || ((sceneCollection.SceneStart.y <= coordinates.y) && ((sceneCollection.SceneStart.y + sceneCollection.SceneCount.y) > coordinates.y))) &&
                   (!sceneCollection.CollectByZ || ((sceneCollection.SceneStart.z <= coordinates.z) && ((sceneCollection.SceneStart.z + sceneCollection.SceneCount.z) > coordinates.z)));
        }

        public static bool IsCollidersIgnored(GameObject gameObject)
        {
            var bigGameObjectMarker = gameObject.GetComponent<BigGameObjectMarkerBehaviour>();
            if (bigGameObjectMarker != null)
            {
                return bigGameObjectMarker.IgnoreColliders;
            }
            return false;
        }
        
        // Game object manipulations helpers ------------------------------------------------------
        public static bool IsGameObjectFolder(GameObject gameObject)
        {
            var prefabType = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (prefabType != PrefabInstanceStatus.NotAPrefab)
            {
                return false;
            }
            var components = gameObject.GetComponents<Component>();
            return (components.Length == 1);
        }

        public static List<GameObject> GetRootGameObjects(Scene scene)
        {
            var rootGameObjects = new List<GameObject>();
            var allRootGameObjects = scene.GetRootGameObjects();
            if ((allRootGameObjects != null) && (allRootGameObjects.Length > 0))
            {
                var rootGameObjectCount = allRootGameObjects.Length;
                for (var rootGameObjectIndex = 0; rootGameObjectIndex < rootGameObjectCount; ++rootGameObjectIndex)
                {
                    var rootGameObject = allRootGameObjects[rootGameObjectIndex];
                    rootGameObjects.Add(rootGameObject);
                }
            }
            return rootGameObjects;
        }

        public static List<GameObject> GetChildren(GameObject gameObject)
        {
            var children = new List<GameObject>();
            var childCount = gameObject.transform.childCount;
            if (childCount > 0)
            {
                for (var childIndex = 0; childIndex < childCount; ++childIndex)
                {
                    children.Add(gameObject.transform.GetChild(childIndex).gameObject);
                }
            }
            return children;
        }

        // Get all GameObjects, recursive non folders
        public static void GetAllGameObjects(GameObject gameObject, List<GameObject> result)
        {
            var addGameObject = true;
            if (IsGameObjectFolder(gameObject))
            {
                addGameObject = false;
            }
            if (addGameObject)
            {
                var sceneLoaderBehaviour = gameObject.GetComponent<SceneLoaderBehaviour>();
                var vegetationStudioManager = gameObject.GetComponent<VegetationStudioManager>();
                if ((sceneLoaderBehaviour != null) || (vegetationStudioManager != null))
                {
                    addGameObject = false;
                }
            }
            if (addGameObject)
            {
                result.Add(gameObject);
            }
            else
            {
                var childrenGameObjects = GetChildren(gameObject);
                var childrenCount = childrenGameObjects.Count;
                if (childrenCount > 0)
                {
                    for (var childIndex = 0; childIndex < childrenCount; ++childIndex)
                    {
                        var childGameObject = childrenGameObjects[childIndex];
                        GetAllGameObjects(childGameObject, result);
                    }
                }
            }
        }

        public static List<GameObject> GetAllGameObjects(Scene scene)
        {
            var result = new List<GameObject>();
            var rootGameObjects = GetRootGameObjects(scene);
            var rootCount = rootGameObjects.Count;
            if (rootCount > 0)
            {
                for (var rootIndex = 0; rootIndex < rootCount; ++rootIndex)
                {
                    var rootGameObject = rootGameObjects[rootIndex];
                    GetAllGameObjects(rootGameObject, result);
                }
            }
            return result;
        }

        public static GameObject FindChild(string name, Scene scene, GameObject parentGameObject, bool folderOnly)
        {
            if (parentGameObject == null)
            {
                var rootGameObjects = GetRootGameObjects(scene);
                foreach (var rootGameObject in rootGameObjects)
                {
                    if ((!folderOnly || IsGameObjectFolder(rootGameObject)) && rootGameObject.name.Equals(name))
                    {
                        return rootGameObject;
                    }
                }
            }
            else
            {
                var childCount = parentGameObject.transform.childCount;
                if (childCount > 0)
                {
                    for (var childIndex = 0; childIndex < childCount; ++childIndex)
                    {
                        var child = parentGameObject.transform.GetChild(childIndex).gameObject;
                        if ((!folderOnly || IsGameObjectFolder(child)) && child.name.Equals(name))
                        {
                            return child;
                        }
                    }
                }
            }
            return null;
        }

        public static GameObject FindOrCreateChild(string name, Scene scene, GameObject parentGameObject, bool folderOnly)
        {
            if (parentGameObject == null)
            {
                var rootGameObjects = GetRootGameObjects(scene);
                foreach (var rootGameObject in rootGameObjects)
                {
                    if ((!folderOnly || IsGameObjectFolder(rootGameObject)) && rootGameObject.name.Equals(name))
                    {
                        return rootGameObject;
                    }
                }
                var newGameObjct = new GameObject(name);
                SceneManager.MoveGameObjectToScene(newGameObjct, scene);
                return newGameObjct;
            }
            else
            {
                var childCount = parentGameObject.transform.childCount;
                if (childCount > 0)
                {
                    for (var childIndex = 0; childIndex < childCount; ++childIndex)
                    {
                        var child = parentGameObject.transform.GetChild(childIndex).gameObject;
                        if ((!folderOnly || IsGameObjectFolder(child)) && child.name.Equals(name))
                        {
                            return child;
                        }
                    }
                }
                var newGameObjct = new GameObject(name);
                newGameObjct.transform.SetParent(parentGameObject.transform);
                return newGameObjct;
            }
        }

        public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static void MoveGameObject(GameObject gameObject, Scene scene, GameObject temporaryFolder)
        {
            if (IsGameObjectFolder(gameObject))
            {
                if (gameObject.transform.childCount > 0)
                {
                    var children = GetChildren(gameObject);
                    var _temporaryFolder = FindOrCreateChild(gameObject.name, scene, temporaryFolder, true);
                    foreach (var child in children)
                    {
                        MoveGameObject(child, scene, _temporaryFolder);
                    }
                }
            }
            else
            {
                if (gameObject.transform.parent != null)
                {
                    gameObject.transform.SetParent(null, true);
                }
                SceneManager.MoveGameObjectToScene(gameObject, scene);
                if (temporaryFolder != null)
                {
                    gameObject.transform.SetParent(temporaryFolder.transform, true);
                }
            }
        }

        public static bool CleanObjectAndCheckIsEmpty(GameObject gameObject)
        {
            var children = GetChildren(gameObject);
            bool allChildrenAreEmpty = true;
            foreach (var child in children)
            {
                var childIsEmpty = CleanObjectAndCheckIsEmpty(child);
                if (childIsEmpty)
                {
                    UnityEngine.Object.DestroyImmediate(child);
                }
                else
                {
                    allChildrenAreEmpty = false;
                }
            }
            if (allChildrenAreEmpty)
            {
                var components = gameObject.GetComponents<Component>();
                var nonEmptyComponents = 0;
                foreach (var component in components)
                {
                    if ((component != null) && (!(component is Transform)))
                    {
                        ++nonEmptyComponents;
                    }
                }
                return (nonEmptyComponents == 0);
            }
            return false;
        }

        // Scene helpers --------------------------------------------------------------------------
        public static Scene FindLoadedScene(string scenePathToFind)
        {
            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.isLoaded)
                {
                    var scenePath = CombineAssetPath(scene.path, string.Empty, UnityExtension);
                    if (string.Compare(scenePath, scenePathToFind, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return scene;
                    }
                }
            }
            return new Scene();
        }

        public static string GetSceneNamesLabel(List<string> scenePaths, int count)
        {
            var result = new StringBuilder();
            if ((scenePaths != null) && (scenePaths.Count > 0))
            {
                for (var index = 0; index < scenePaths.Count; ++index)
                {
                    AppendIfNotEmpty(result, "\r\n");
                    if ((index < (count - 1)) || index == (scenePaths.Count - 1))
                    {
                        result.Append(scenePaths[index]);
                    }
                    else if (index == count)
                    {
                        result.Append("...");
                    }
                }
            }
            return result.ToString();
        }
    }
};