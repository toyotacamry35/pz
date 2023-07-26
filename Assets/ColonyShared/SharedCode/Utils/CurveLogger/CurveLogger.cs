using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using Newtonsoft.Json;
using SharedCode.Extensions;
using SharedCode.Utils.Threads;
using TimeUnits = System.Int64;
using SharedCode.Serializers;

namespace SharedCode.Utils
{
    // Used to avoid asking CurveLogger internal dictionary of loggers everywhere (& every time) we try add data to logger.
    // [Ru] Идея такая: где-то в одном месте на ноде организуешь наследника этого interface'а. В нём подписка на OnPropChanged 
    //.. ILogableEntty.IsCurveLoggerEnable. Когда ему по этому каналу прилетает true, он создаёт нужный логгер (by CurveLogger.Get(_))
    //.. Понимания имени логгера достаточно только ему. Все пользователи получают ссылку на provider и пушат д-е через конструкцию вида 
    //.. _curveLoggerProvider.CurveLogger?.IfActive()?.AddData(...);
    //.. На кластере ск.вс. ты в коде мастер-entity, поэтому там можно просто проверить флаг `ILogableEntty.IsCurveLoggerEnable`
    public interface ICurveLoggerProvider
    {
        CurveLogger CurveLogger { get; }
    }

    public class CurveLogger
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly CurveLogger Default;
        public static Action<string> OpenLogsHandler;

        private Queue<CurveLogData> _dataQueue = new Queue<CurveLogData>(MaxLength);
        private readonly HashSet<Guid> _processedDumps = new HashSet<Guid>(); 
        //private ConcurrentQueue<>
        private readonly object _lock = new object();
        // To prevent very big size in memory:
        private const int MaxLength = 30 * 60 * 5 * 10; // 9k == 30 (fps) * 60 (sec) * 5 (min) * 10(data sources (i.e. curves)). - Max store time before dump - 5 min.
        private bool _active; 

        public const string DefaultFolderPath = "/_Logs/" + LogFolderName;
        public const string LogFolderName = "_CurveLogs";
        public const string LogFileNameBase = "CurveLog_";
        public const string FileNameSuffix_Server = "S";
        public const string FileNameSuffix_Client = "Cl";
        public const string PidPrefix = "_PID";
        public const string LogFileExt = "json";
        public const string LoggerNameTagBegin = " LoggerId((";
        public const string LoggerNameTagBegin_ForRegexp = @" LoggerId\(\(";
        public const string LoggerNameTagEnd = "))";
        public const string LoggerNameTagEnd_ForRegexp = @"\)\)";
        public static string ServerFileNameCommonPart => LogFileNameBase + FileNameSuffix_Server;
        public static string ClientFileNameCommonPart => LogFileNameBase + FileNameSuffix_Client;

        public readonly string LoggerName;
        public string ServerFileName => ServerFileNameCommonPart + LoggerNameTagBegin + LoggerName + LoggerNameTagEnd + PidPrefix + Process.GetCurrentProcess().Id + '.' + LogFileExt;
        public string ClientFileName => ClientFileNameCommonPart + LoggerNameTagBegin + LoggerName + LoggerNameTagEnd + PidPrefix + Process.GetCurrentProcess().Id + '.' + LogFileExt;

        // Is used for convenient visualization of velo - as dPos by const dt, based on curr pos.
        public static Vector3 VeloAsDltPos(Vector3 pos, Vector3 velo) => pos + velo * GlobalConstsHolder.GlobalConstsDef.LocoPosLogAgentFixedDtToCalcVeloAsDltPos;

        // --- Ctors: -----------------------------------------

        static CurveLogger()
        {
            Default = new CurveLogger(nameof(Default));
        }

        private CurveLogger(string loggerName)
        {
            LoggerName = loggerName;
        }

        // --- API: ----------------------------

        private static readonly object InstacesLock = new object();
        private static readonly Dictionary<string, CurveLogger> Instances = new Dictionary<string, CurveLogger>();
        public static CurveLogger Get(string loggerName)
        {
            if (loggerName == null)
                return Default;

            lock (InstacesLock)
            {
                if (Instances.TryGetValue(loggerName, out var instnace))
                    return instnace;
                instnace = new CurveLogger(loggerName);
                Instances.Add(loggerName, instnace);
                return instnace;
            }
        }

        // Is set by cheats (see `CheatServiceEntity`.SetCurveLoggerStateDo(_) (switches on cluster) & `UnityCheatServiceEntity` called same method (switches on Unity-node).
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                Logger.IfInfo()?.Message($"Logger {LoggerName} {(_active ? "ACTIVATED" : "deactivated")}").Write();
            }
        }

        public CurveLogger IfActive => Active ? this : null;

        public static CurveLogger CreateLogger(string loggerName)
        {
            return new CurveLogger(loggerName);
        }

        public void AddData(string id, TimeUnits timeStamp, float x, float y = float.NaN, float z = float.NaN)
        {
            if (!Active)
                return;

            lock (_lock)
            {
                if (_dataQueue.Count == 0)
                    Logger.IfInfo()?.Message($"Logger {LoggerName}: Accumulating data began. (curve id: {id}).").Write();

                _dataQueue.Enqueue(new CurveLogData() {Id = id, X = x, Y = y, Z = z, TimeStamp = timeStamp});

                while (_dataQueue.Count > MaxLength)
                    _dataQueue.Dequeue();
            }
        }

        public void AddData(string id, TimeUnits timeStamp, Vector3 v)
        {
            AddData(id, timeStamp, v.x, v.y, v.z);
        }

        public void AddData(string id, TimeUnits timeStamp, Vector2 v)
        {
            AddData(id, timeStamp, v.x, v.y);
        }

        public void AddData(string id, TimeUnits timeStamp, Value v)
        {
            switch (v.ValueType)
            {
                case Value.Type.Bool:
                    AddData(id, timeStamp, v.Bool ? 1 : 0);
                    break;
                case Value.Type.Float:
                    AddData(id, timeStamp, v.Float);
                    break;
                case Value.Type.Int:
                    AddData(id, timeStamp, v.Int);
                    break;
                case Value.Type.Long:
                    AddData(id, timeStamp, SyncTime.ToSeconds(v.Long));
                    break;
                case Value.Type.Vector2:
                    AddData(id, timeStamp, v.Vector2);
                    break;
                case Value.Type.Vector3:
                    AddData(id, timeStamp, v.Vector3);
                    break;
            }
        }

        //@param `floderPath` - if `null`or empty, default path 'll be used
        public void DumpDataToFile(bool isServer, Guid dumpId, string folderPath = null, bool convertAndShow = false)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                folderPath = DefaultFolderPath;

            var fileName = isServer ? ServerFileName : ClientFileName;
            var logFilePath = Path.Combine(folderPath, fileName);

            Queue<CurveLogData> data;
            lock (_lock) // Looks like no need, but let it be
            {
                if (_processedDumps.Contains(dumpId))
                    return;
                _processedDumps.Add(dumpId);
                data = _dataQueue;
                _dataQueue = new Queue<CurveLogData>(MaxLength);
            }

            Logger.IfDebug()?.Message($"{nameof(CurveLogger)} 8. {nameof(DumpDataToFile)} started to {logFilePath}. Count:{data.Count}").Write();

            AsyncUtils.RunAsyncTask(() =>
            {
                // 'll not cre. folder, if exist (w/o explicit check)
                Directory.CreateDirectory(folderPath);
                using (StreamWriter str = File.CreateText(logFilePath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(str, data); 
                }
                Logger.IfDebug()?.Message($"Dump saved to \"{logFilePath}\". (Count: {data.Count} DumpId:{dumpId})").Write();
                if (convertAndShow)
                    OpenLogsHandler?.Invoke(folderPath);
            });
        }

    }

    // --- Util types: -----------------------------

    public struct CurveLogData
    {
        public string Id;
        public float X;
        public float Y;
        public float Z;
        public TimeUnits TimeStamp;
    }

}
