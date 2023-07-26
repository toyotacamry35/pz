using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace SharedCode.Aspects.Utils
{
    public interface IHasLogableEntity
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        ILogableEntity LogableEntity { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILogableEntity : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Always), BsonIgnore]
        bool IsCurveLoggerEnable { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetCurveLoggerEnable(bool val);
    }

    public static class LogableHelper
    {
        /// <summary>
        /// Returns logger IfActive (& if ent is ILogableEntity, also checks ILogableEntity.IsCurveLoggerEnable) 
        /// </summary>
        public static CurveLogger GetLoggerIfEnableAndActive(ref CurveLogger logger, ILogableEntityClientBroadcast logableClBroadcast, string loggerName = null)
        {
            if (logger == null)
            {
                //if (logableClBroadcast == null) if (DbgLog.Enabled) DbgLog.Log($"#P3-11221: arg `{nameof(ILogableEntityClientBroadcast)}` is null: {loggerName}");
                if (logableClBroadcast == null || logableClBroadcast.IsCurveLoggerEnable)
                {
                    //if (DbgLog.Enabled) DbgLog.Log($"#P3-11221: {nameof(LogableHelper)}.GetLoggerIfEnableAndActive({logger}, {logableClBroadcast}, {loggerName}). Create logger");
                    logger = (string.IsNullOrWhiteSpace(loggerName))
                        ? CurveLogger.Default 
                        : CurveLogger.Get(loggerName);
                }
            }
            return logger?.IfActive;
        }
        public static CurveLogger GetLoggerIfEnableAndActive(ref CurveLogger logger, string loggerName = null)
        {
            if (logger == null)
            {
                logger = (string.IsNullOrWhiteSpace(loggerName))
                    ? CurveLogger.Default
                    : CurveLogger.Get(loggerName);
            }
            return logger?.IfActive;
        }
    }
}

