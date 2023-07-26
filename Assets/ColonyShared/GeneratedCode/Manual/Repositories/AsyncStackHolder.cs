using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System.Threading;
using System;
using SharedCode.Repositories;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Manual.Repositories
{
    public static class AsyncStackHolder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("AsyncStack");

        public static int UnityThreadId { get; set; }

        public static void CheckValidateEntityInAsyncContext(int entityTypeId, Guid entityId, ReadWriteEntityOperationType operationType)
        {
            if (_SerializerContext.Pool.Current.IsDump)
                return;

            var value = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            if (value == null)
            {
                Logger.IfError()?.Message("Entity context is null. TypeId {0} entityId {1} operationType {2}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, operationType).Write();
                throw new AsyncContextException(string.Format("Entity context is null. TypeId {0} entityId {1} operationType {2}", ReplicaTypeRegistry.GetTypeById(entityTypeId)?.Name ?? "none", entityId, operationType));
            }

            value.CheckValidateEntityInAsyncContext(entityTypeId, entityId, operationType);
        }

        public static void AssertNoChildren()
        {
            AsyncEntitiesRepositoryRequestContext.AssertNoChildren();
        }

        public static void ThrowIfInUnityContext()
        {
            if (IsInUnity)
            {
                Logger.IfError()?.Message("Cant use from unity thread").Write();
                throw new Exception("Cant use from unity thread");
            }
        }
        public static void ThrowIfNotInUnityContext()
        {
            if (!IsInUnity)
            {
                Logger.IfError()?.Message("Cant use not from unity thread").Write();
                throw new Exception("Cant use not from unity thread");
            }
        }

        
        public static bool IsInUnity => UnityThreadId != 0 && UnityThreadId == Thread.CurrentThread.ManagedThreadId;

        public readonly struct SuspendedContextHolder
        {
            private readonly EntitiesContainer _head;

            public SuspendedContextHolder(EntitiesContainer head)
            {
                _head = head;
            }

            public ValueTask Resume()
            {
                AsyncEntitiesRepositoryRequestContext.Head = _head;

                AsyncEntitiesRepositoryRequestContext.AssertNoChildren();
                if (_head != null)
                    return _head.Context.Relock();
                else
                    return new ValueTask();
            }
        }

        public static SuspendedContextHolder Suspend()
        {
            AsyncEntitiesRepositoryRequestContext.AssertNoChildren();

            var head = AsyncEntitiesRepositoryRequestContext.Head;
            if (head != null)
                head.Context.Release();

            AsyncEntitiesRepositoryRequestContext.Head = null;
            return new SuspendedContextHolder(head);
        }
    }
}
