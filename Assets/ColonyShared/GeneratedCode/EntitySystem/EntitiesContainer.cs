using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Utils;

namespace SharedCode.EntitySystem
{
    public sealed class EntitiesContainer : IEntitiesContainer, IEntitiesContainerExtension
    {
        private const int _warnBatchSize = 10;

        public enum ContainerState
        {
            Alive,
            Disposed,
            TimedOut
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public EntityBatch Batch { get; }

        internal EntitiesContainer Parent { get; set; }

        private EntitiesContainer _child;
        internal EntitiesContainer Child { get => (_child == null || _child.State == ContainerState.TimedOut) ? null : _child; set => _child = value; }

        public ContainerState State { get; set; }

        public object Tag { get; }

        public StackTrace CreatedStackTrace { get; }

        public StackTrace CurrentStackTrace { get; private set; }

        public AsyncEntitiesRepositoryRequestContext Context { get; }

        public EntitiesContainer(AsyncEntitiesRepositoryRequestContext context, EntityBatch batch, EntitiesContainer parent, object tag)
        {
            CreatedStackTrace = StackTraceUtils.GetStackTrace();
            CurrentStackTrace = CreatedStackTrace;
            Batch = batch;
            Parent = parent;
            Tag = tag;
            Context = context;
        }

        ~EntitiesContainer()
        {
            Dispose(false);
        }

        public int AllBatchItemsCount
        {
            get
            {
                var sum = 0;
                foreach (var stack in AsyncStackEnumerable.ToParents(this))
                {
                    sum += stack.Batch.Length;
                }

                return sum;
            }
        }

        public IEntityRef GetEntityRef(int typeId, Guid entityId)
        {
            var masterType = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            ReadWriteEntityOperationType operationType = Context.CheckIsEntityAlreadyLocked(masterType, entityId);
            if (operationType == ReadWriteEntityOperationType.None)
                return default;

            return ((IEntitiesRepositoryExtension)Context.Repository).GetRef(masterType, entityId);
        }

        public bool IsEntityExist(int typeId, Guid entityId)
        {
            return ((IEntitiesRepositoryExtension)Context.Repository).GetRepositoryEntityContainsStatus(typeId, entityId) != RepositoryEntityContainsStatus.NotContains;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && State != ContainerState.Alive)
            {
                Logger.IfError()?.Message("{0} is in invalid state", this).Write();
                return;
            }

            if (!disposing && State == ContainerState.Disposed)
            {
                Logger.IfError()?.Message("{0}: Finalizer was called on disposed object", this).Write();
                return;
            }

            if (!disposing && State == ContainerState.TimedOut)
            {
                return;
            }

            if (!disposing && State == ContainerState.Alive)
            {
                Logger.IfError()?.Message("{0} was not disposed correctly", this).Write();
            }

            ReleaseCurrent();
        }

        public T Get<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel)
        {
            var entityRef = GetEntityRef(typeId, entityId);
            if(entityRef == null)
            {
                if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message("Entity {0}:{1} not found", ReplicaTypeRegistry.GetTypeById(typeId).Name, entityId).Write();
                return default;
            }

            if (!entityRef.ContainsReplicationLevel(replicationLevel))
            {
                var masterType = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
                if (Logger.IsErrorEnabled) Logger.IfError()?.Message("Entity {0}:{1} not contains replication level {2} in repository {3}({4}) (CurrentReplicationMask {5})", ReplicaTypeRegistry.GetTypeById(masterType).Name, entityId, replicationLevel, Context.Repository.Id, Context.Repository.CloudNodeType, entityRef.CurrentReplicationMask).Write();
                return default;
            }

            if (((IEntityRefExt)entityRef).State == EntityState.Destroyed)
            {
                var masterType = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
                if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message("Entity {0}:{1} is already destroyed in repository {2}({3})", ReplicaTypeRegistry.GetTypeById(masterType).Name, entityId, Context.Repository.Id, Context.Repository.CloudNodeType).Write();
                return default;
            }

            var entityReplicationLevel = ((IEntityRefExt)entityRef).GetEntity().GetReplicationLevel(replicationLevel);
            return entityReplicationLevel is T ? (T)entityReplicationLevel : default(T);
        }

        public bool TryGet<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel, out T entity)
        {
            entity = default;

            var entityRef = GetEntityRef(typeId, entityId);
            if (entityRef == null || !entityRef.ContainsReplicationLevel(replicationLevel))
            {
                return false;
            }

            if (((IEntityRefExt)entityRef).State == EntityState.Destroyed)
            {
                return false;
            }


            var entityReplicationLevel = ((IEntityRefExt)entityRef).GetEntity().GetReplicationLevel(replicationLevel);
            if (!(entityReplicationLevel is T))
            {
                return false;
            }

            entity = (T)entityReplicationLevel;
            return true;
        }

        public async Task<T> GetOrSubscribe<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel)
        {
            var entity = Get<T>(typeId, entityId, replicationLevel);
            if (entity == null)
            {
                using (var addressResolverContainerWrapper = await Context.Repository.GetFirstService<IClusterAddressResolverServiceEntityServer>())
                {
                    var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
                    var addressResolverContainer = addressResolverContainerWrapper.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                    var entityRepositoryId = await addressResolverContainer.GetEntityAddressRepositoryId(masterTypeId, entityId);

                    if (entityRepositoryId == Guid.Empty)
                    {
                        Logger.IfError()?.Message("Entity repositoryId is empty. typeId {0} id {1}", masterTypeId, entityId).Write();
                        return default(T);
                    }

                    if (entityRepositoryId == Context.Repository.Id)
                    {
                        Logger.IfError()?.Message("Entity repositoryId == current, but not found. typeId {0} id {1}", masterTypeId, entityId).Write();
                        return default(T);
                    }

                    using (var repositoryCommunicationEntityWrapper = await Context.Repository.Get<IRepositoryCommunicationEntityServer>(entityRepositoryId))
                    {
                        var repositoryCommunicationEntity = repositoryCommunicationEntityWrapper.Get<IRepositoryCommunicationEntityServer>(entityRepositoryId);
                        var result = await repositoryCommunicationEntity.SubscribeReplication(masterTypeId, entityId, Context.Repository.Id, ReplicationLevel.Server);
                        if (result == false)
                        {
                            Logger.IfError()?.Message("subscribe replication fail. typeId {0} id {1}", masterTypeId, entityId).Write();
                            return default(T);
                        }
                    }
                }

                entity = Get<T>(typeId, entityId, replicationLevel);
                if (entity == null)
                {
                    Logger.IfError()?.Message("Entity not found. typeId {0} id {1}", typeId, entityId).Write();
                    return default(T);
                }
            }
            return entity;
        }

        void ReleaseCurrent()
        {
            var currentHead = AsyncEntitiesRepositoryRequestContext.Head;

            try
            {
                lock (Context._lock)
                {
                    if (currentHead != this)
                    {
                        Logger.Fatal("THIS SHOULD NEVER HAPPEN. Container {1} is not set as head. Head is {3}. Invalidating context {0}. Created stacktace: {4} ----- Current stacktrace: {5} -----",
                            Context, this, Child,
                            currentHead?.ToString() ?? "null",
                            CreatedStackTrace?.ToString() ?? "null",
                            CurrentStackTrace?.ToString() ?? "null");

                        Context.SetAsyncContextInvalid();
                    }

                    if (Child != null)
                    {
                        Logger.Error("Container {1} has child {2} Head is {3}. Invalidating context {0}. Created stacktace: {4} ----- Current stacktrace: {5} -----",
                            Context, this, Child,
                            currentHead?.ToString() ?? "null",
                            CreatedStackTrace?.ToString() ?? "null",
                            CurrentStackTrace?.ToString() ?? "null");

                        Context.SetAsyncContextInvalid();
                    }

                    State = ContainerState.Disposed;

                    Pop();
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
                throw;
            }

            AsyncEntitiesRepositoryRequestContext.Head = Parent;
        }

        private void Pop()
        {
            var context = Context;

            var container = this;
            while (container != null && container.State == ContainerState.Disposed && container.Child == null)
            {
                container.ReleaseEntities();
                container.Context.RemoveLockedEntities(container.Batch);

                var parent = container.Parent;
                if (parent != null)
                    parent.Child = null;

                container = parent;
            }

            if (container == null)
            {
                if(context.LockState == AsyncEntitiesRepositoryRequestContext.ContextLockState.Locked)
                    context.StopTimeout();

                context.Tail = null;
            }
        }

        internal void Push()
        {
            var context = Context;
            var currentLocalHead = AsyncEntitiesRepositoryRequestContext.Head;

            if (currentLocalHead != null && currentLocalHead.Context != context)
                throw new AsyncContextException($"Contexts {currentLocalHead.Context} and {context} do not match, race condition in async stack");

            if (context.State == AsyncEntitiesRepositoryRequestContext.ContextValidState.Invalid)
                throw new AsyncContextException($"{context} is invalidated. Head: {currentLocalHead}, new container: {this}");

            lock (context._lock)
            {
                if (currentLocalHead?.Child != null)
                    throw new AsyncContextException($"Current stack head {currentLocalHead} has child: {currentLocalHead.Child}");

                context.AddNewLockedItems(Batch);

                if (currentLocalHead != null)
                {
                    Parent = currentLocalHead;
                    currentLocalHead.Child = this;
                }

                if (context.Tail == null)
                    context.Tail = this;
            }

            AsyncEntitiesRepositoryRequestContext.Head = this;

            var allbatchItemCount = AllBatchItemsCount;
            if (allbatchItemCount >= _warnBatchSize)
            {
                var batchesStr = getBatchItemsInfo();
                Logger.IfError()?.Message("Batch size is too big. Batch items count {0} items {1} Stack {2}", allbatchItemCount, batchesStr, StackTraceUtils.GetStackTrace()?.ToString() ?? "<not set>").Write();
            }
        }

        private string getBatchItemsInfo()
        {
            var sb = StringBuildersPool.Get;

            foreach (var entitiesContainer in AsyncStackEnumerable.ToParents(this))
            {
                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    sb.AppendFormat("<entity {0}, {1}, {2}, {3}",
                        EntitiesRepository.GetTypeById(batchItem.EntityMasterTypeId).Name, batchItem.EntityId, batchItem.RequestOperationType.ToString(),
                        batchItem.UpFromReadToExclusive.ToString());
                }

                sb.AppendLine();
                sb.AppendLine("-------");
                sb.AppendLine();
            }

            return sb.ToStringAndReturn();
        }


        private void ReleaseEntities()
        {
            if (Batch == null || Batch.Empty || Context.LockState == AsyncEntitiesRepositoryRequestContext.ContextLockState.Released)
                return;

            var containers = AsyncStackEnumerable.Single(this);
            ((IEntitiesRepositoryDataExtension)Context.Repository).Release(ref containers);
        }

        public void DumpToStringBuilder(StringBuilder sb)
        {
            sb.AppendFormat("<EntitiesContainer Tag:{0} State:{1} Batch:", Tag?.ToString() ?? "null", State.ToString()).AppendLine();
            Batch?.DumpToStringBuilder(sb);
            sb.AppendFormat("--- createdStack:{0} --- currentStack:{1}", CreatedStackTrace?.ToString() ?? "none", CurrentStackTrace?.ToString() ?? "none").AppendLine();
        }

        public override string ToString()
        {
            return $"[{typeof(EntitiesContainer).Name} {State}, {Context}, {Batch?.ToString() ?? "null"}, Tag:{Tag?.ToString() ?? "null"}";
        }
    }

    public interface IEntitiesContainerExtension
    {
        IEntityRef GetEntityRef(int typeId, Guid entityId);

        EntityBatch Batch { get; }

        object Tag { get; }

        StackTrace CreatedStackTrace { get; }

        StackTrace CurrentStackTrace { get; }
    }
}
