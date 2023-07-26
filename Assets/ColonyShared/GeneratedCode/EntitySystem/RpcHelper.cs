using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using SharedCode.Network;
using GeneratedCode.Manual.Repositories;
using System.Collections.Generic;
using SharedCode.Interfaces.Network;
using SharedCode.Refs;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using SharedCode.Serializers.Protobuf;
using SharedCode.Repositories;
using ProtoBuf;
using NLog;
using GeneratedCode.Network.Statistic;

namespace GeneratedCode.EntitySystem
{
    [ProtoContract]
    public struct RpcExceptionInfo : IEquatable<RpcExceptionInfo>
    {
        [ProtoMember(1)]
        public string Message;
        [ProtoMember(2)]
        public string Details;
        [ProtoMember(3)]
        public PropertyAddress Address;

        public override bool Equals(object obj)
        {
            if (!(obj is RpcExceptionInfo))
                return false;

            return Equals((RpcExceptionInfo)obj);
        }

        public bool Equals(RpcExceptionInfo info)
        {
            return Message == info.Message &&
                   Details == info.Details &&
                   EqualityComparer<PropertyAddress>.Default.Equals(Address, info.Address);
        }

        public override int GetHashCode()
        {
            var hashCode = 1192106692;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Details);
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyAddress>.Default.GetHashCode(Address);
            return hashCode;
        }

        public static bool operator ==(RpcExceptionInfo info1, RpcExceptionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(RpcExceptionInfo info1, RpcExceptionInfo info2)
        {
            return !(info1 == info2);
        }
    }

    public class RpcException : Exception
    {
        public PropertyAddress TargetObjectAddress { get; }
        public string Details { get; }

        public RpcException(string message, PropertyAddress address, string details) : base(message)
        {
            TargetObjectAddress = address;
            Details = details;
        }

        protected RpcException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return $"Rpc Exception at {TargetObjectAddress}, {Environment.NewLine}Call stack: {Environment.NewLine}{Details}, {Environment.NewLine}Base Exception: {Environment.NewLine}{base.ToString()}";
        }
    };

    public class RpcTimeoutException : Exception
    {
        public RpcTimeoutException()
        {
        }

        public RpcTimeoutException(string message) : base(message)
        {
        }

        public RpcTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RpcTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    };

    internal static class RpcHelper
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static Pool<byte[]> _bufferPool = new Pool<byte[]>(200, 50,
            () => new byte[1024 * 1024 * 2], null);

        public static Pool<byte[]> BufferPool => _bufferPool;

        public static Guid GetObjectIdForSerialization(this IDeltaObject obj) => ((IDeltaObjectExt)obj).GetParentEntity().Id;
        public static Guid GetObjectIdForSerialization(this IEntity obj) => obj.Id;

        private static int GetObjectTypeForSerialization(this IDeltaObject obj) => ((IDeltaObjectExt)obj).GetParentEntity().TypeId;
        private static int GetObjectTypeForSerialization(this IEntity obj) => obj.TypeId;

        public static INetworkProxy GetNetworkProxyForSerialization(this IDeltaObject obj) => ((IDeltaObjectExt)obj).GetParentEntity().GetNetworkProxyForSerialization();
        public static INetworkProxy GetNetworkProxyForSerialization(this IEntity obj) => ((IRemoteEntity)obj).GetNetworkProxy();

        public static void CheckValidateEntityInAsyncContext(this IEntity entity) => ((IEntityExt)entity).CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
        public static void CheckValidateEntityInAsyncContext(this IDeltaObject obj) => ((IEntityExt)((IDeltaObjectExt)obj).GetParentEntity())?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);

        public static bool IsMaster(this IEntity entity) => ((IEntityExt)entity).IsMaster();
        public static bool IsMaster(this IDeltaObject obj) => ((IEntityExt)((IDeltaObjectExt)obj).GetParentEntity())?.IsMaster() ?? true;

        public static bool NeedDeferredRpcOnMigrating(this IEntity entity) => ((IEntityExt) entity).NeedDeferredRpcOnMigrating();
        public static bool NeedDeferredRpcOnMigrating(this IDeltaObject obj) => ((IEntityExt)((IDeltaObjectExt)obj).GetParentEntity())?.NeedDeferredRpcOnMigrating() ?? false;

        public static Task<T> AsTask<T>(this Task<T> task) => task;
        public static Task AsTask(this Task task) => task;

        public static ValueTask<IEntitiesContainer> GetThisExclusive(this IDeltaObject deltaObject, [CallerMemberName] string callerTag = null)
        {
            var repo = (IEntitiesRepositoryExtension)((IDeltaObjectExt)deltaObject).GetParentEntity()?.EntitiesRepository;
            if (repo == null)
                return default;

            var myId = deltaObject.GetObjectIdForSerialization();
            var myType = deltaObject.GetObjectTypeForSerialization();
            return repo.GetExclusive(myType, myId, callerTag);
        }

        public static ValueTask<IEntitiesContainer> GetThisExclusive(this IEntity entity, [CallerMemberName] string callerTag = null)
        {
            var repo = (IEntitiesRepositoryExtension)entity.EntitiesRepository;
            if (repo == null)
                return default;

            var myId = entity.GetObjectIdForSerialization();
            var myType = entity.GetObjectTypeForSerialization();
            return repo.GetExclusive(myType, myId, callerTag);
        }
        
        public static ValueTask<IEntitiesContainer> GetThis(this IEntity entity, [CallerMemberName] string callerTag = null)
        {
            var repo = entity.EntitiesRepository;
            if (repo == null)
                return default;

            var myId = entity.GetObjectIdForSerialization();
            var myType = entity.GetObjectTypeForSerialization();
            return repo.Get(myType, myId, callerTag);
        }

        public static byte[] SerializeObjectId(ISerializer serializer, byte[] data, ref int offset, IDeltaObject obj)
        {
            PropertyAddress address = null;
            if (!EntityPropertyResolver.TryGetPropertyAddress(obj, out address))
            {
                Logger.IfError()?.Message("SerializeObjectId cant get property address. Delta object {0}", obj.GetType().GetFriendlyName()).Write();
                return null;
            }

            data = serializer.Serialize(data, ref offset, address);

            return data;

        }

        public static PropertyAddress DeserializeObjectId(ISerializer serializer, byte[] data, ref int offset)
        {
            var address = serializer.Deserialize<PropertyAddress>(data, ref offset);

            return address;
        }

        public static byte[] FillSendHeader(ISerializer serializer, byte[] data, out int offset, byte msg, ref Guid migrationId)
        {
            var header = new NetworkProxy.PacketHeader()
            {
                MethodId = msg,
                PacketType = PacketType.Send,
                HasMigrationId = migrationId != Guid.Empty
            };

            offset = 0;
            data = serializer.Serialize(data, ref offset, header);

            if (header.HasMigrationId)
                data = serializer.Serialize(data, ref offset, migrationId);

            return data;
        }

        public static byte[] FillRequestHeader(ISerializer serializer, byte[] data, out int offset, out Guid guid, byte msg, ref Guid migrationId)
        {
            var header = new NetworkProxy.PacketHeader()
            {
                MethodId = msg,
                PacketType = PacketType.Request,
                HasMigrationId = migrationId != Guid.Empty
            };

            offset = 0;
            data = serializer.Serialize(data, ref offset, header);

            if (header.HasMigrationId)
                data = serializer.Serialize(data, ref offset, migrationId);

            guid = Guid.NewGuid();
            data = serializer.Serialize(data, ref offset, guid);

            return data;
        }

        public static byte[] FillResponseHeader(ISerializer serializer, byte[] data, out int offset, Guid transactionId, byte msg, ref Guid migrationId)
        {
            var header = new NetworkProxy.PacketHeader()
            {
                MethodId = msg,
                PacketType = PacketType.Response,
                HasMigrationId = migrationId != Guid.Empty
            };

            offset = 0;
            data = serializer.Serialize(data, ref offset, header);

            if (header.HasMigrationId)
                data = serializer.Serialize(data, ref offset, migrationId);

            data = serializer.Serialize(data, ref offset, transactionId);

            return data;
        }

        private static byte[] FillExceptionHeader(ISerializer serializer, byte[] data, out int offset, Guid transactionId, byte msg, ref Guid migrationId)
        {
            var header = new NetworkProxy.PacketHeader()
            {
                MethodId = msg,
                PacketType = PacketType.Exception,
                HasMigrationId = migrationId != Guid.Empty
            };

            offset = 0;
            data = serializer.Serialize(data, ref offset, header);

            if (header.HasMigrationId)
                data = serializer.Serialize(data, ref offset, migrationId);

            data = serializer.Serialize(data, ref offset, transactionId);
            return data;
        }

        public static void CheckAndSendTransactionException(INetworkProxy networkProxy, Guid transactionId, Type type, byte messageId, PropertyAddress address, Exception exception, ref Guid migrationId)
        {
            try
            {
                var data = _bufferPool.Take();
                try
                {
                    data = FillExceptionHeader(networkProxy.Serializer, data, out var offset, transactionId, messageId, ref migrationId);

                    var exceptionInfo = new RpcExceptionInfo()
                    {
                        Details = exception.ToString(),
                        Message = exception.Message,
                        Address = address
                    };

                    data = networkProxy.Serializer.Serialize(data, ref offset, exceptionInfo);

                    Statistics<RpcNetworkStatistics>.Instance.SentResponseBytes(type, messageId, offset);
                    networkProxy.SendMessage(data, 0, offset, MessageSendOptions.ReliableOrdered);
                }
                finally
                {
                    _bufferPool.Return(data);
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception on send exception for message {0}", ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName).Write();
            }
        }

        private struct RpcTimeout<T> : ITimeoutPayload
        {
            private readonly INetworkProxy networkProxy;
            private readonly Guid transactionId;
            private readonly Type type;
            private readonly byte messageId;
            private readonly TaskCompletionSource<T> resultTcs;

            public RpcTimeout(INetworkProxy networkProxy, Guid transactionId, Type type, byte messageId, TaskCompletionSource<T> resultTcs)
            {
                this.resultTcs = resultTcs;
                this.networkProxy = networkProxy;
                this.transactionId = transactionId;
                this.type = type;
                this.messageId = messageId;
            }

            public bool Run()
            {
                if (!networkProxy.TransactionTimeout(transactionId))
                    return true;

                if (!resultTcs.TrySetException(new RpcTimeoutException($"Network: transaction {transactionId} of RPC {ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName} timeout")))
                    Logger.IfFatal()?.Message("Race condition in RPC handlers of {0}", ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName).Write();
                return true;
            }
        }

        public static async ValueTask<T> SendRequest<T>(byte[] buffer, int length, Type type, byte messageId, MessageSendOptions options, Guid guid, INetworkProxy networkProxy, double timeout, bool suppressCheckEntityIsLocked)
        {
            if (!suppressCheckEntityIsLocked)
                EntitySystemBlock.ThrowIfLocked();

            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            var serializer = networkProxy.Serializer;

            if (!networkProxy.SendMessage(buffer, 0, length, options, guid, CompleteFunc, FailFunc))
                throw new InvalidOperationException($"Failed to send reqeust {ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName} with guid {guid}");

            TimeoutSystem.Install(new RpcTimeout<T>(networkProxy, guid, type, messageId, taskSource), TimeSpan.FromSeconds(timeout));

            Statistics<RpcNetworkStatistics>.Instance.SentBytes(type, messageId, length);

            var context = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            context?.Release();

            try
            {
                return await taskSource.Task;
            }
            finally
            {
                if (context != null)
                    await context.Relock();
            }

            void CompleteFunc(byte[] data, int offset)
            {
                Statistics<RpcNetworkStatistics>.Instance.ReceiveResponseBytes(type, messageId, data.Length);

                var result = serializer.Deserialize<T>(data, ref offset);

                if (!taskSource.TrySetResult(result))
                    Logger.IfFatal()?.Message("Race condition in RPC handlers of {0}", ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName).Write();
            }

            void FailFunc(byte[] data, int offset)
            {
                Statistics<RpcNetworkStatistics>.Instance.ReceiveResponseBytes(type, messageId, data.Length);

                var exceptionInfo = serializer.Deserialize<RpcExceptionInfo>(data, ref offset);

                var exception = new RpcException(exceptionInfo.Message, exceptionInfo.Address, exceptionInfo.Details);
                if (!taskSource.TrySetException(exception))
                    Logger.IfFatal()?.Message("Race condition in RPC handlers of {0}", ReplicaTypeRegistry.GetDispatcher(type, messageId).messageName).Write();
            }

        }

        public static async ValueTask<T> DeferredMigratingRpc<T>(Func<Task<T>> func, ConcurrentQueue<Func<TaskCompletionSource<bool>, Task>> deferredMigratingRpcQueue, double timeout, string functionName, string entityTypeName, Guid entityId)
        {
            var taskSource = new TaskCompletionSource<TaskCompletionSource<bool>>(TaskCreationOptions.RunContinuationsAsynchronously);

            deferredMigratingRpcQueue.Enqueue((TaskCompletionSource<bool> _tcs_) =>
            {
                lock (taskSource)
                {
                    if (taskSource.Task.IsCompleted)
                        return Task.CompletedTask;

                    taskSource.TrySetResult(_tcs_);
                    return Task.CompletedTask;
                }
            });

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            ct.Token.Register(() =>
            {
                lock (taskSource)
                {
                    if (taskSource.Task.IsCompleted)
                        return;
                    taskSource.TrySetResult(null);
                }
                Logger.IfError()?.Message("Migrating entity timeout. Rpc {0} entity {1}:{2} timeout:{3}", functionName, entityTypeName, entityId, timeout).Write();

            }, useSynchronizationContext: false);

            var context = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            context?.Release();

            TaskCompletionSource<bool> tcs = null;
            try
            {
                tcs =  await taskSource.Task;
            }
            finally
            {
                if (context != null)
                {
                    var task = context.Relock();
                    tcs.SetResult(true);
                    await task;
                }
                else
                    tcs?.TrySetResult(true);
            }

            var returnTask = func();
            if (!returnTask.IsCompleted || returnTask.IsFaulted)
                return await returnTask;

            return returnTask.Result;
        }

        public static async ValueTask DeferredMigratingRpc(Func<Task> func, ConcurrentQueue<Func<TaskCompletionSource<bool>, Task>> deferredMigratingRpcQueue, double timeout, string functionName, string entityTypeName, Guid entityId)
        {
            var taskSource = new TaskCompletionSource<TaskCompletionSource<bool>>(TaskCreationOptions.RunContinuationsAsynchronously);

            deferredMigratingRpcQueue.Enqueue((TaskCompletionSource<bool> _tcs_) =>
            {
                lock (taskSource)
                {
                    if (taskSource.Task.IsCompleted)
                        return Task.CompletedTask;

                    taskSource.TrySetResult(_tcs_);
                    return Task.CompletedTask;
                }
            });

            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            ct.Token.Register(() =>
            {
                lock (taskSource)
                {
                    if (taskSource.Task.IsCompleted)
                        return;
                    taskSource.TrySetResult(null);
                }
                Logger.IfError()?.Message("Migrating entity timeout. Rpc {0} entity {1}:{2} timeout:{3}", functionName, entityTypeName, entityId, timeout).Write();

            }, useSynchronizationContext: false);

            var context = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            context?.Release();

            TaskCompletionSource<bool> tcs = null;
            try
            {
                tcs = await taskSource.Task;
            }
            finally
            {
                if (context != null)
                {
                    var task = context.Relock();
                    tcs?.TrySetResult(true);
                    await task;
                }
                else
                    tcs?.TrySetResult(true);
            }

            var returnTask = func();
            if (!returnTask.IsCompleted || returnTask.IsFaulted)
                await returnTask;
        }

        public static ValueTask SendMessage(byte[] buffer, int length, Type type, byte messageId, MessageSendOptions options, IEnumerable<INetworkProxy> networkProxies, bool suppressCheckEntityIsLocked)
        {
            if (!suppressCheckEntityIsLocked)
                EntitySystemBlock.ThrowIfLocked();


            foreach (var networkProxy in networkProxies)
            {
                Statistics<RpcNetworkStatistics>.Instance.SentBytes(type, messageId, length);
                networkProxy.SendMessage(buffer, 0, length, options);
            }
            return new ValueTask();
        }

        public static ValueTask SendMessage(byte[] buffer, int length, Type type, byte messageId, MessageSendOptions options, INetworkProxy networkProxy, bool suppressCheckEntityIsLocked)
        {
            if (!suppressCheckEntityIsLocked)
                EntitySystemBlock.ThrowIfLocked();

            Statistics<RpcNetworkStatistics>.Instance.SentBytes(type, messageId, length);
            networkProxy.SendMessage(buffer, 0, length, options);

            return new ValueTask();
        }

        public static IEnumerable<INetworkProxy> GatherMessageTargets(this IEntity entity, ReplicationLevel level, Type type, byte messageId)
        {
            var ext = (IEntityExt)entity;
            var replicateToRaw = ext.ReplicateTo();

            foreach (var pair in replicateToRaw)
            {
                if (pair.Key == entity.EntitiesRepository.Id)//Migrating process. Temporaly replicate container to self.
                    continue;

                if ((pair.Value.GetReplicationMask() & (long)level) == (long)level)
                {
                    var remoteEntityRef = (IEntityRefExt)((IEntitiesRepositoryExtension)entity.EntitiesRepository).CheckAndGetSubscriberRepositoryCommunicationRef(pair.Key, entity.TypeId, entity.Id);
                    if (remoteEntityRef == null)
                    {
                        Logger.IfError()?.Message("Replication broadcast message {0} subscribed repository {1} not found. Entity {2} {3}", type, messageId, pair.Key, entity.TypeName, entity.TypeId).Write();
                        continue;
                    }

                    yield return ((IRemoteEntity)remoteEntityRef.GetEntity()).GetNetworkProxy();
                }
            }
        }

        public static IEnumerable<INetworkProxy> GatherMessageTargets(this IDeltaObject entity, ReplicationLevel level, Type type, byte messageId)
        {
            return ((IDeltaObjectExt)entity).GetParentEntity().GatherMessageTargets(level, type, messageId);
        }
    }
}
