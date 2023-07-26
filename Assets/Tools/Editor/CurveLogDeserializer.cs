using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using NLog;
using SharedCode.Utils;
using Src.Animation;
using UnityEditor;
using UnityEngine;
using Utilities;
using Object = UnityEngine.Object;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Tools.Editor
{
    public class CurveLogDeserializer : EditorWindow
    {
        internal static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CurveLogDeserializer));

        private const string AssetsFolderName = "Assets";
        private static readonly string AssetSavePathRelToAssetsFolder = CurveLogger.LogFolderName;
        const string ClipAssetName = "CurveLogClip";
        const string ClipAssetExt = ".asset";
        private const int WindowWidth = 400;
        private const int WindowHeight = 100;
        public static readonly string DataPath = Application.dataPath;
        // Feed `baseDataPath` by "Application.dataPath"
        public static readonly string CurveLogFolderAssetsRelativePath = Path.Combine("..", /*LogsFolderName*/"Logs", CurveLogger.LogFolderName);
        public static string GetCurveLogFolderPath => Path.Combine(DataPath, CurveLogFolderAssetsRelativePath); // Looks like it's the best way to get into Logs folder (but I'm not sure)

        private static AnimationUtility.TangentMode _tangentMode;
        
        private static string _curveLogFolderPath
        {
            get => EditorPrefs.GetString("2CC6B1B8-4911-48C9-A456-053A30B14CEE", GetCurveLogFolderPath);
            set => EditorPrefs.SetString("2CC6B1B8-4911-48C9-A456-053A30B14CEE", value);
        }

        private static bool _curveLogNormalized
        {
            get => EditorPrefs.GetBool("7BBC458D-9123-426D-BDA8-131C43C8A4BA", false);
            set => EditorPrefs.SetBool("7BBC458D-9123-426D-BDA8-131C43C8A4BA", value);
        }
        
        [MenuItem("Tools/Open Curve Logs...")]
        public static void OpenCurveLogs()
        {
            var window = (CurveLogDeserializer)CreateInstance(typeof(CurveLogDeserializer));
            window.position = new Rect((Screen.currentResolution.width - WindowWidth) / 2, (Screen.currentResolution.height - WindowHeight) / 2, WindowWidth, WindowHeight);
            window.ShowUtility();
        }

        public static void OpenCurveLogs(string folderPath, bool normalized)
        {
            var clips = ConvertCurveLogs(folderPath, normalized);
            Selection.activeObject = clips.FirstOrDefault();
            Selection.objects = clips;
            foreach(var o in clips)
                EditorGUIUtility.PingObject(o);
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }
        
        public static AnimationClip[] ConvertCurveLogs(string folderPath, bool normalized)
        {
            try
            {
                _tangentMode = (AnimationUtility.TangentMode) GlobalConstsHolder.GlobalConstsDef.CurveTangentMode;
            }
            catch (Exception e)
            {
                Logger.Error($"Can't parse `{nameof(GlobalConstsHolder.GlobalConstsDef.CurveTangentMode)}` to `{nameof(AnimationUtility.TangentMode)}` " +
                             $"(value == {GlobalConstsHolder.GlobalConstsDef.CurveTangentMode}). So dflt 'll be used. Got exception: \"{e}\".");
            }

            var logFilesGrouped = Directory.GetFiles(folderPath, CurveLogger.LogFileNameBase + '*' + '.' + CurveLogger.LogFileExt)
                .Select(Path.GetFileName)
                .Select((fileName) =>
                {
                    // /// var replaced1 = fileName.Replace(CurveLogger.ServerFileNameCommonPart, string.Empty);
                    // /// var sharedNamePart = replaced1.Replace(CurveLogger.ClientFileNameCommonPart, string.Empty);
                    // var sharedNamePart = fileName.Replace(CurveLogger.ServerFileNameCommonPart, string.Empty)
                    //                              .Replace(CurveLogger.ClientFileNameCommonPart, string.Empty)
                    //                              .Replace('.' + CurveLogger.LogFileExt, string.Empty)
                    //                              .Split(new string[] {CurveLogger.PidPrefix}, StringSplitOptions.None)[0];
                    // 
                    // return (sharedNamePart, fileName);

                    var regexpPattern = $"{CurveLogger.LogFileNameBase}" +
                                        $"(.*){CurveLogger.LoggerNameTagBegin_ForRegexp}" +
                                        $"(.*){CurveLogger.LoggerNameTagEnd_ForRegexp}{CurveLogger.PidPrefix}" +
                                        $@"(.*)\.{CurveLogger.LogFileExt}";
                    //#note: 3 elems expected:
                    //  0) - FileNameSuffix ("S"/"Cl")
                    //  1) - LoggerName (enntyId, may be + additional text)
                    //  2) - PID (process id)
                    var substrings = Regex.Split(fileName, regexpPattern).Where(s => s != string.Empty).ToArray();
                    return new LogFileStrings()
                    {
                        LoggerName = substrings[(int)Substrings.LoggerName],
                        FileName = Path.Combine(folderPath, fileName),
                        CurveNamesPrefix = substrings[(int)Substrings.FileNameSuffix] + '_' 
                                         //+ substrings[(int)Substrings.LoggerName].Substring(0, 4) + '_' // short fragment of entityId
                                         + substrings[(int)Substrings.Pid]
                    };
                })
                .GroupBy((x) => x.LoggerName);

            return logFilesGrouped
                .Select(@group => ConvertCurvesToClip(@group.Key, @group.ToArray(), normalized))
                .Select(AssetDatabase.LoadAssetAtPath<AnimationClip>)
                .ToArray();
        }

                        /// Path.Combine(GetCurveLogFolderPath, filePaths[0])
        
        //@param `loggerName` - unique part of log files (& produced here clip asset file) names
        public static string ConvertCurvesToClip(string loggerName, LogFileStrings[] logFilesStrings, bool normalized)
        {
            // 1. Deserialize:

            // var PosLogFolderPath = Path.Combine(Application.dataPath, /*LocomotionPositionLogAgent*/.PosLogFolderAssetsRelativePath);
            // var pathServer = Path.Combine(PosLogFolderPath, CurveLogger.Default.ServerFileName);
            // var pathClient = Path.Combine(PosLogFolderPath, CurveLogger.Default.ClientFileName);

            var deserializedFiles = new List<Queue<CurveLogData>>(logFilesStrings.Length);
            void Deserialize(string path)
            {
                if (!File.Exists(path))
                {
                    Logger.IfError()?.Message($"File \"{path}\" is not present.").Write();
                    deserializedFiles.Add(null);
                    return;
                }

                StreamReader reader = null;
                try
                {
                    for (int t = 0; t < 10; ++t)
                    {
                        try
                        {
                            reader = new StreamReader(path);
                            break;
                        }
                        catch (IOException e)
                        {
                            int HResult = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                            const int SharingViolation = 32;
                            if ((HResult & 0xFFFF) != SharingViolation)
                                throw;
                            Thread.Sleep(1000);
                        }
                    }

                    var serializer = new JsonSerializer();
                    var jsr = new JsonTextReader(reader);
                    deserializedFiles.Add(serializer.Deserialize<Queue<CurveLogData>>(jsr));
                }
                finally
                {
                    reader?.Dispose();
                }
            }
            foreach (var strings in logFilesStrings)
                Deserialize(strings.FileName);

            // Assert:
            if (deserializedFiles.Count != logFilesStrings.Length)
            {
                Logger.IfError()?.Message($"deserializedFiles.Count({deserializedFiles.Count}) != logFilesStrings.Length({logFilesStrings.Length})").Write();
                return null;
            }

            // 2. Separate data by string-id:

            Dictionary<string/*Final curve name*/, Queue<CurveLogData>/*curve data*/> separated = new Dictionary<string, Queue<CurveLogData>>();
            Dictionary<string/*CurveLogData.Id*/, string/*Final curve name*/> aliases = new Dictionary<string, string>();
            long minTimeStamp = long.MaxValue;

            for (int i = 0;  i < logFilesStrings.Length;  ++i)
            {
                var fileStrings = logFilesStrings[i];
                var log = deserializedFiles[i];
                if (log == null)
                    continue;

                aliases.Clear();
                while (log.Count > 0)
                {
                    var item = log.Dequeue();
                    string alias;
                    if (!aliases.TryGetValue(item.Id, out alias))
                        aliases.Add(item.Id, alias = fileStrings.CurveNamesPrefix + '.' + item.Id); // Composing curve names

                    if (!separated.TryGetValue(alias, out var queue))
                        separated.Add(alias, queue = new Queue<CurveLogData>());

                    if (minTimeStamp > item.TimeStamp)
                        minTimeStamp = item.TimeStamp;

                    queue.Enqueue(item);
                }
            }

            // 3. Make clip (as graphs data storage):

            void AddCurveToClip(AnimationClip cl, AnimationCurve cu, string relPath, Type type, string propName)
            {
                if (_tangentMode != AnimationUtility.TangentMode.Free) // `Free` is default type.
                {
                    for (int i = 0;  i < cu.keys.Length;  ++i)
                    {
                        AnimationUtility.SetKeyLeftTangentMode (cu, i, _tangentMode);
                        AnimationUtility.SetKeyRightTangentMode(cu, i, _tangentMode);
                    }
                }
                cl.SetCurve(relPath, type, propName, cu);
            }

            var clip = new AnimationClip();
            foreach (var queue in separated)
            {
                var isVelocity = queue.Key.IndexOf("velocity", StringComparison.OrdinalIgnoreCase) != -1;
                var isPosition = queue.Key.IndexOf("position", StringComparison.OrdinalIgnoreCase) != -1;
                Tuple<AnimationCurve,string> x = null, y = null, z = null, w = null;
                long prevTimeStamp = -1;
                while (queue.Value.Count > 0)
                {
                    int AddKey(ref Tuple<AnimationCurve,string> curve, string prefix, long time, long prevTime, float value, ref float s, ref float d)
                    {
                        if (float.IsNaN(value))
                            return 0;

                        if (curve == null)
                            curve = Tuple.Create(new AnimationCurve(), prefix);

                        if (prevTime != -1 && time != prevTime)
                        {
                            var dd = (value - curve.Item1[curve.Item1.length - 1].value) / SyncTime.ToSeconds(time - prevTime);
                            d += dd * dd;
                        }

                        s += value * value;

                        curve.Item1.AddKey(SyncTime.ToSeconds(time - minTimeStamp), value);

                        return 1;
                    }

                    var item = queue.Value.Dequeue();
                    float sum = 0, dif = 0, dummy = 0;
                    int res = AddKey(ref x, ".x", item.TimeStamp, prevTimeStamp, item.X, ref sum, ref dif) 
                            + AddKey(ref y, ".y", item.TimeStamp, prevTimeStamp, item.Y, ref sum, ref dif) 
                            + AddKey(ref z, ".z", item.TimeStamp, prevTimeStamp, item.Z, ref sum, ref dif);

                    if (isVelocity)
                    {
                        if (res > 1)
                            AddKey(ref w, ".w", item.TimeStamp, -1, Mathf.Sqrt(sum), ref dummy, ref dummy);
                    }
                    else if (isPosition)
                    {
                        if (prevTimeStamp != -1 && res > 1)
                            AddKey(ref w, ".w", item.TimeStamp, -1, Mathf.Sqrt(dif), ref dummy, ref dummy);
                    }

                    prevTimeStamp = item.TimeStamp;
                }

                var curves = new List<Tuple<AnimationCurve,string>>();
                if (x != null)  curves.Add(x);
                if (y != null)  curves.Add(y);
                if (z != null)  curves.Add(z);
                if (w != null)  curves.Add(w);

                if (normalized)
                {
                    for (int i = 0; i < curves.Count; ++i)
                        curves[i] = Tuple.Create(AnimationCurveUtils.Normalize(curves[i].Item1, 0.1f), curves[i].Item2);
                }
                
                if (curves.Count == 1)
                    AddCurveToClip(clip, curves[0].Item1, string.Empty, typeof(___F57F8706_5B88_45E2_964B_6417C1102F72___._), queue.Key);
                else
                {
                    foreach (var curve in curves)
                        AddCurveToClip(clip, curve.Item1, string.Empty, typeof(___F57F8706_5B88_45E2_964B_6417C1102F72___._), queue.Key + curve.Item2);
                }
            }

            // 3.1 Save clip asset:
            var folderPathRelative = Path.Combine(AssetsFolderName, AssetSavePathRelToAssetsFolder);
            var folderPathAbsolute = Path.Combine(DataPath, AssetSavePathRelToAssetsFolder);
            Directory.CreateDirectory(folderPathAbsolute);
            var timeToName = SyncTime.TimeUnitsToDateTime(minTimeStamp).ToString("HH-mm-ss-fff", CultureInfo.InvariantCulture);
            var dateToName = SyncTime.TimeUnitsToDateTime(minTimeStamp).ToString("MM.dd", CultureInfo.InvariantCulture);
            var clipFileName = $"{dateToName} {ClipAssetName} {loggerName}_{timeToName}_(~{minTimeStamp})" + (normalized ? "_normalized" : "") + ClipAssetExt;
            var assetSavePath = Path.Combine(folderPathRelative, clipFileName);
            AssetDatabase.CreateAsset(clip, assetSavePath);

            Logger.IfInfo()?.Message($"{nameof(CurveLogDeserializer)} logs clip saved to \"{assetSavePath}\"").Write();
            return assetSavePath;
        }

        [InitializeOnLoadMethod]
        private static void InjectOpenLogHandler()
        {
            CurveLogger.OpenLogsHandler = path =>
            {
                EditorApplication.CallbackFunction action = null;
                action = () =>
                {
                    EditorApplication.update -= action;
                    Thread.Sleep(1000);
                    OpenCurveLogs(path, _curveLogNormalized);
                };
                EditorApplication.update += action;
            };
        }

        enum Substrings : byte
        {
            FileNameSuffix, //  0) - FileNameSuffix ("S"/"Cl")
            LoggerName,     //  1) - LoggerName (enntyId, may be + additional text)
            Pid             //  2) - PID (process id)
        }

        public struct LogFileStrings
        {
            internal string LoggerName;
            internal string FileName;
            internal string CurveNamesPrefix;
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _curveLogFolderPath = EditorGUILayout.TextField(_curveLogFolderPath);
                if(GUILayout.Button("Browse...", GUILayout.Width(80)))
                    _curveLogFolderPath = EditorUtility.OpenFolderPanel("Choose curves folder", _curveLogFolderPath, "");
            }

            _curveLogNormalized = EditorGUILayout.Toggle("Normalized", _curveLogNormalized);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Open"))
            {
                OpenCurveLogs(_curveLogFolderPath, _curveLogNormalized);
                Close();
            }
        }
    }
}
