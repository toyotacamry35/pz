using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntityModel.Test;
using GeneratedCode.EntitySystem;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Repositories;
using HWT;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.Repositories;
using SharedCode.Utils.Threads;
using StringBuildersPool = SharedCode.Utils.StringBuildersPool;

namespace GeneratedCode.Manual.Repositories
{
    public class AsyncEntitiesRepositoryRequestContext
    {
        public enum ContextLockState
        {
            Locked,
            Released
        }

        public enum ContextValidState
        {
            Valid,
            Invalid
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static long LastId = 0;

        public long Id { get; }

        public Guid RepositoryId => Repository.Id;

        internal IEntitiesRepository Repository { get; }

        private readonly static AsyncLocal<EntitiesContainer> _head = new AsyncLocal<EntitiesContainer>();
        internal static EntitiesContainer Head 
        {
            get
            {
                if (_head.Value == null)
                    return null;
                if (_head.Value.State == EntitiesContainer.ContainerState.TimedOut)
                    return _head.Value.Parent;
                return _head.Value;
            }
            set => _head.Value = value; 
        }

        private ConcurrentDictionary<(int type, Guid entity), ReadWriteEntityOperationType> LockedEntities { get; set; }

        private EntitiesContainer _tail;
        internal EntitiesContainer Tail 
        { 
            get => (_tail == null || _tail.State == EntitiesContainer.ContainerState.TimedOut) ? null : _tail; 
            set => _tail = value; 
        }

        private ICancellable _timeoutReleaseCts;

        public ContextValidState State { get; set; }
        public ContextLockState LockState { get; set; }

        internal readonly object _lock = new object();

        public AsyncEntitiesRepositoryRequestContext(IEntitiesRepository repository)
        {
            Id = Interlocked.Increment(ref LastId);
            Repository = repository;
        }

        public void CheckRepositoryId(IEntitiesRepository repository)
        {
            if (repository != Repository)
            {
                Logger.IfError()?.Message("{0} used from another repository {1}, but created from repository {2}", this, repository.Id, RepositoryId).Write();
                throw new Exception($"{this} used from another repository {repository.Id}, but created from repository {RepositoryId}");
            }
        }

        public void CheckValid()
        {
            if (State == ContextValidState.Invalid)
            {
                Logger.IfError()?.Message("{0} is invalid").Write();
                throw new Exception($"{this} is invalid");
            }
        }

        public bool IsLockedAny()
        {
            if (LockedEntities == null)
                return false;

            return LockedEntities.Count > 0;
        }

        public static void AssertNoChildren()
        {
            var value = Head;
            if (value?.Child != null)
            {
                Logger.IfError()?.Message("{0} has active child {1}. stacktrace {2}", value.Context, value.Child, new StackTrace()).Write();
                throw new AsyncContextException($"{value.Context} has active child {value.Child}.");
            }
        }

        public void AddNewLockedItems(EntityBatch entityBatch)
        {
            if (entityBatch == null || entityBatch.Empty)
                return;

            if (LockedEntities == null)
                LockedEntities = new ConcurrentDictionary<(int, Guid), ReadWriteEntityOperationType>(Environment.ProcessorCount, entityBatch.Length);

            for (int i = 0; i < entityBatch.Length; i++)
            {
                ref var batchItem = ref entityBatch.Items[i];
                var key = (EntityTypeId: batchItem.EntityMasterTypeId, batchItem.EntityId);
                var exists = LockedEntities.TryGetValue(key, out var existing);
                if (batchItem.UpFromReadToExclusive)
                {
                    if (!exists)
                    {
                        Logger.IfError()?.Message("{0} invalid push of UpFromReadToExclusive. Entity {1} is not locked", this, batchItem).Write();
                        throw new AsyncContextException(string.Format("{0} invalid push UpFromReadToExclusive. Entity {1} is not locked", this, batchItem));
                    }
                    if(existing != ReadWriteEntityOperationType.Read)
                    {
                        Logger.IfError()?.Message("{0} invalid push of UpFromReadToExclusive. Entity {1} is locked for {2}", this, batchItem, existing).Write();
                        throw new AsyncContextException(string.Format("{0} invalid push UpFromReadToExclusive. Entity {1} is locked for write", this, batchItem));
                    }
                    LockedEntities[key] = ReadWriteEntityOperationType.Write;
                }
                else if (exists)
                {
                    Logger.IfError()?.Message("{0} invalid push of {1}, Entity {2} is locked for {3}", this, batchItem.RequestOperationType, batchItem, existing).Write();
                    throw new AsyncContextException(string.Format("{0} invalid push of {1}, Entity {2} is locked for {3}", this, batchItem.RequestOperationType, batchItem, existing));
                }
                else
                    LockedEntities[key] = batchItem.RequestOperationType;
            }
        }

        private readonly struct PostSetInvalidTimeout : ITimeoutPayload
        {
            private readonly AsyncEntitiesRepositoryRequestContext _ctx;

            public PostSetInvalidTimeout(AsyncEntitiesRepositoryRequestContext ctx)
            {
                _ctx = ctx;
            }

            public bool Run()
            {
                _ctx.Release();
                return true;
            }
        }

        public void SetAsyncContextInvalid()
        {
            if (State == ContextValidState.Invalid)
                return;

            State = ContextValidState.Invalid;

            Logger.IfError()?.Message("SetAsyncContextInvalid {0}\n{1}", this, new StackTrace()).Write();

            //_timeoutReleaseCts = TPLTimeouts.It.Install(new PostSetInvalidTimeout(this), TimeSpan.FromSeconds(ServerCoreRuntimeParameters.ReleaseInvalidContainersDelaySeconds));
            _timeoutReleaseCts = TimeoutSystem.Install(new PostSetInvalidTimeout(this), TimeSpan.FromSeconds(ServerCoreRuntimeParameters.ReleaseInvalidContainersDelaySeconds));
        }

        internal void RemoveLockedEntities(EntityBatch batch)
        {
            if (batch == null)
                return;

            for (int i = 0; i < batch.Length; i++)
            {
                ref var batchItem = ref batch.Items[i];
                var key = (EntityTypeId: batchItem.EntityMasterTypeId, batchItem.EntityId);
                bool exists = LockedEntities.TryGetValue(key, out var existing);
                if (batchItem.UpFromReadToExclusive)
                {
                    if (!exists)
                    {
                        Logger.IfError()?.Message("{0} invalid pop of UpFromReadToExclusive. Entity {1} is not locked", this, batchItem).Write();
                    }
                    else if(existing != ReadWriteEntityOperationType.Write)
                    {
                        Logger.IfError()?.Message("{0} invalid pop of UpFromReadToExclusive. Entity {1} is locked for {2}", this, batchItem, existing).Write();
                    }
                    else
                        LockedEntities[key] = ReadWriteEntityOperationType.Read;
                }
                else if (!LockedEntities.TryRemove(key, out var _))
                    Logger.IfError()?.Message("{0} invalid pop of {1}. Entity {1} is not locked", this, batchItem.RequestOperationType, key).Write();
            }
        }

        public override string ToString()
        {
            return $"[{typeof(AsyncEntitiesRepositoryRequestContext).Name} Id: {Id}, State: {State}, LockState: {LockState}]";
        }

        public ReadWriteEntityOperationType CheckIsEntityAlreadyLocked(int typeId, Guid id)
        {
            if (LockedEntities == null)
                return ReadWriteEntityOperationType.None;

            LockedEntities.TryGetValue((typeId, id), out var operationType);
            return operationType;
        }

        public void CheckValidateEntityInAsyncContext(int entityTypeId, Guid entityId, ReadWriteEntityOperationType operationType)
        {
            if (LockedEntities == null || !LockedEntities.TryGetValue((entityTypeId, entityId), out var currentOperationType))
            {
                Logger.IfError()?.Message("Entity {0}:{1} not found in {2}. opType {3}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType).Write();
                throw new AsyncContextException(string.Format("Entity {0}:{1} not found in {2}. opType {3}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType));
            }

            if (currentOperationType == ReadWriteEntityOperationType.Read && operationType == ReadWriteEntityOperationType.Write)
            {
                Logger.IfError()?.Message("Entity {0}:{1} was in Read in {2}. opType {3}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType).Write();
                throw new AsyncContextException(string.Format("Entity {0}:{1} was in Read in {2}. opType {3}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType));
            }

            if (LockState == ContextLockState.Released)
            {
                Logger.IfError()?.Message("Entity {0}:{1} found in {2} with opType {3}, but context is unlocked", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType).Write();
                throw new AsyncContextException(string.Format("Entity {0}:{1} found in {2} with opType {3}, but context is unlocked", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType));
            }

            if (State == ContextValidState.Invalid)
            {
                Logger.IfError()?.Message("Entity {0}:{1} found in {2} with opType {3}, but context is invalid", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType).Write();
                throw new AsyncContextException(string.Format("Entity {0}:{1} found in {2} with opType {3}, but context is invalid", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, this, operationType));
            }
        }

        public void Release()
        {
            lock(_lock)
            {
                if (LockState == ContextLockState.Released)
                {
                    Logger.IfError()?.Message("{0} is already released", this).Write();
                    return;
                }

                LockState = ContextLockState.Released;

                if (Tail == null)
                    return;

                var container = Tail;
                while (container.Child != null)
                    container = container.Child;

                var containers = AsyncStackEnumerable.ToParents(container);
                ((IEntitiesRepositoryDataExtension)Repository).Release(ref containers);

                StopTimeout();
            }
        }

        public ValueTask Relock()
        {
            lock(_lock)
            {
                if (LockState != ContextLockState.Released)
                    throw new Exception($"LockAgain: {this} is not released");

                if (State == ContextValidState.Invalid)
                    throw new Exception($"LockAgain: {this} is invalid");

                if (Tail == null)
                    return new ValueTask();

                var container = Tail;
                while (container.Child != null)
                    container = container.Child;

                return ((IEntitiesRepositoryDataExtension)Repository).LockAgain(container);
            }
        }


        private DateTime _startTimeoutTime;

        private ICancellable _mainTimeout;

        private bool _showTotalTime = false;

        private readonly struct MainTimeout : ITimeoutPayload
        {
            private readonly AsyncEntitiesRepositoryRequestContext _ctx;

            public MainTimeout(AsyncEntitiesRepositoryRequestContext ctx)
            {
                _ctx = ctx;
            }

            public bool Run()
            {
                if (ServerCoreRuntimeParameters.CanDisableRepositoryGetEntitiesTimeout && ((IEntitiesRepositoryExtension)_ctx.Repository).IsTimeoutBlocked())
                    return false;

                _ctx._showTotalTime = true;

                var sb = StringBuildersPool.Get;
                _ctx.DumpToStringBuilder(sb);
                Logger.IfError()?.Message("{0}: Entity usage timeout {1}. Containers: {2}", _ctx, TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntitiesContainerLockTimeoutSeconds), StringBuildersPool.ToStringAndReturn(sb)).Write();

                lock(_ctx._lock)
                {
                    _ctx.SetAsyncContextInvalid();
                }

                return true;
            }
        }

        public void StartTimeout()
        {
            if (!ServerCoreRuntimeParameters.EnableEntityUsagesAndEventsTimeouts)
                return;

            if(Tail == null)
            {
                Logger.IfError()?.Message("Starting timeout without containers").Write();
            }

            if (_mainTimeout != null)
            {
                Logger.IfError()?.Message("{0}: Timeout is already started", this).Write();
                return;
            }

            _startTimeoutTime = DateTime.UtcNow;
            _showTotalTime = false;

            //_mainTimeout = TPLTimeouts.It.Install(new MainTimeout(this), TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntitiesContainerLockTimeoutSeconds));
            _mainTimeout = TimeoutSystem.Install(new MainTimeout(this), TimeSpan.FromSeconds(ServerCoreRuntimeParameters.EntitiesContainerLockTimeoutSeconds));
        }



        public void DumpToStringBuilder(StringBuilder sb)
        {
            var head = Tail;
            if (head == null)
                return;

            while (head.Child != null)
                head = head.Child;

            while (head != null)
            {
                head.DumpToStringBuilder(sb);
                head = head.Parent;
            }
        }

        public void StopTimeout()
        {
            if (!ServerCoreRuntimeParameters.EnableEntityUsagesAndEventsTimeouts)
                return;

            if (_showTotalTime)
            {
                var deltaTime = DateTime.UtcNow - _startTimeoutTime;
                var sb = StringBuildersPool.Get;
                DumpToStringBuilder(sb);
                Logger.IfError()?.Message("{0}: Entity usage timeout finished {1}. Containers: {2}, Stack: {3}", this, deltaTime, StringBuildersPool.ToStringAndReturn(sb), new StackTrace()).Write();
            }

            if (_mainTimeout == null)
            {
                Logger.IfError()?.Message("{0}: Timeout was not started", this).Write();
                return;
            }

            _showTotalTime = false;
            _mainTimeout.Cancel();
            _mainTimeout = null;
        }
    }
}
