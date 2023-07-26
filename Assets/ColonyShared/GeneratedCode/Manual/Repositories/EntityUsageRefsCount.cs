using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GeneratedCode.EntitySystem.Statistics;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using ColonyShared.SharedCode.Utils;
using System.Threading;
using SharedCode.Utils;
using SharedCode.Repositories;
using GeneratedCode.Network.Statistic;

namespace GeneratedCode.Manual.Repositories
{
    public class EntityUsageRefsCount
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private int _read;

        public int Read => _read;

        private int _write;

        public int Write => _write;

        private readonly int _entityTypeId;

        private readonly Guid _entityId;

        private DateTime _lastChangedTime = DateTime.UtcNow;

        private DateTime _lastOperationLogTime = DateTime.UtcNow;

        private int _usingCount;

        public int UsingCount => _usingCount;

        private readonly ConcurrentQueue<LogOperationInfo> _operationsLog = new ConcurrentQueue<LogOperationInfo>();

        public string TypeName => ReplicaTypeRegistry.GetTypeById(_entityTypeId)?.GetFriendlyName() ?? _entityTypeId.ToString();

        private readonly HashSet<long> _usedContexts = new HashSet<long>();

        public EntityUsageRefsCount(int entityTypeId, Guid entityId)
        {
            _entityTypeId = entityTypeId;
            _entityId = entityId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void addOperationsLog(LogOperationInfoOperationType logOperationInfoOperation, long batchId, long asyncContextId, string callerName, object tag)
        {
            var newOp = new LogOperationInfo(logOperationInfoOperation, batchId, asyncContextId, callerName, _lastChangedTime, _read, _write, _usingCount, tag);
            _operationsLog.Enqueue(newOp);
            LogOperationInfo info;
            while (_operationsLog.Count > 100)
                _operationsLog.TryDequeue(out info);
        }

        public void UpRead(long batchId, string callerName, object tag, long contextId)
        {
            if (_usedContexts.Contains(contextId))
                throw new InvalidOperationException($"UpRead already contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");
            _usedContexts.Add(contextId);

            _read++;
            _usingCount++;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.UpRead, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.UpRead(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        public void UpWrite(long batchId, string callerName, object tag, long contextId)
        {
            if (_usedContexts.Contains(contextId))
                throw new InvalidOperationException($"UpWrite already contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");
            _usedContexts.Add(contextId);

            _write++;
            _usingCount++;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.UpWrite, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.UpWrite(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        public void UpFromReadToExclusive(long batchId, string callerName, object tag, long contextId)
        {
            if (!_usedContexts.Contains(contextId))
                throw new InvalidOperationException($"UpFromReadToExclusive not contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");

            _read--;
            _write++;
            _usingCount++;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.UpFromReadToExclusive, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.UpFromReadToExclusive(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        public void DownRead(long batchId, string callerName, object tag, long contextId)
        {
            if (!_usedContexts.Remove(contextId))
                throw new InvalidOperationException($"DownRead not contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");

            _read--;
            _usingCount--;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.DownRead, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.DownRead(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        public void DownWrite(long batchId, string callerName, object tag, long contextId)
        {
            if (!_usedContexts.Remove(contextId))
                throw new InvalidOperationException($"DownWrite not contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");

            _write--;
            _usingCount--;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.DownWrite, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.DownWrite(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        public void DownFromReadToExclusive(long batchId, string callerName, object tag, long contextId)
        {
            if (!_usedContexts.Contains(contextId))
                throw new InvalidOperationException($"DownFromReadToExclusive not contains context {contextId}. typeId {TypeName} id {_entityId}. oplog {GetOperationLog()}");

            _read++;
            _write--;
            _usingCount--;
            _lastChangedTime = DateTime.UtcNow;
            if (ServerCoreRuntimeParameters.CollectOperationsLog)
                addOperationsLog(LogOperationInfoOperationType.DownFromReadToExclusive, batchId, contextId, callerName, tag);
            Statistics<EntityUsagesStatistics>.Instance.DownFromReadToExclusive(callerName, _entityTypeId, _entityId);
            validateRefsInternal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool validateRefsInternal()
        {
            var isInvalid = _read < 0 || _write < 0 || _write > 1 || (_write == 1 && _read != 0) || _usingCount < 0 ||
                            (_read == 0 && _write == 0 && _usingCount > 0);
            if (isInvalid)
            {
                var opLog = GetOperationLog();

                Logger.Error(
                    "EntityUsageRefsCount invalid read {0} write {1} usingCount {2} typeId {3} entityId {4} update {5} seconds ago. Operations: {6}",
                    _read, _write, _usingCount, TypeName, _entityId, (DateTime.UtcNow - _lastChangedTime).TotalSeconds, opLog);
            }
            return !isInvalid;
        }

        public string GetOperationLog()
        {
            var opLog = StringBuildersPool.Get;
            GetOperationLog(opLog, _usedContexts);
            return opLog.ToStringAndReturn();
        }

        private static readonly ThreadLocal<List<LogOperationInfo>> OpLogCopy = new ThreadLocal<List<LogOperationInfo>>(() => new List<LogOperationInfo>());

        public bool CanGetOperationLog()
        {
            return (DateTime.UtcNow - _lastOperationLogTime).TotalSeconds >= ServerCoreRuntimeParameters.EntityOperationLogMinDelaySeconds;
        }

        public HashSet<long> CloneUsedContexts()
        {
            return new HashSet<long>(_usedContexts); 
        }

        public void GetOperationLog(StringBuilder opLog, HashSet<long> usedContexts)
        {
            _lastOperationLogTime = DateTime.UtcNow;
            opLog.AppendLine();
            opLog.AppendFormat("-----Used contexts {0}:", usedContexts?.Count ?? 0);
            opLog.AppendLine();

            if (usedContexts != null)
                foreach (var usedContextId in usedContexts)
                    opLog.Append($"<Context Id {usedContextId}>");

            if (_operationsLog != null)
            {
                var logCopy = OpLogCopy.Value;
                logCopy.Clear();
                logCopy.AddRange(_operationsLog);

                opLog.AppendLine();
                opLog.AppendLine("-----Operations:");
                for (int i = logCopy.Count - 1; i >= 0; --i)
                {
                    var operation = logCopy[i];
                    opLog.AppendFormat(
                        "<{0}, caller \"{1}\", {2:0.##} secs ago, batch {3}, context {4}, r {5}, w {6}, u {7}, tag {8}>",
                        operation.LogOperationInfoOperation.ToString(), operation.CallerName,
                        (DateTime.UtcNow - operation.Time).TotalSeconds, operation.BatchId,
                        operation.AsyncContextId, operation.Read, operation.Write, operation.UsagesCount,
                        operation.Tag?.ToString() ?? "none").AppendLine();
                }
            }
        }

        public bool validateRefs()
        {
            var isValid = validateRefsInternal();
            return isValid;
        }

        public bool IsFree()
        {
            return _write == 0 && _read == 0;
        }

        public bool IsRead()
        {
            return _write == 0 && _read > 0;
        }

        public bool IsWrite()
        {
            return _write > 0;
        }

        public string Dump => $"R:{_read} W:{_write}";
    }
}
