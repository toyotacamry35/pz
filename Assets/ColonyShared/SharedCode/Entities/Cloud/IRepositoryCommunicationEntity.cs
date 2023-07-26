using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.ActorServices;
using SharedCode.Cloud;
using SharedCode.Config;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities.Cloud
{
    public class WantsToDisconnectEventProxy
    {
        public event Func<Guid, Task> RepoWantsToDisconnect;
        public Task Fire(Guid id)
        {
            return RepoWantsToDisconnect?.Invoke(id);
        }
    }

    [ProtoContract]
    public struct EndpointAddress : IEquatable<EndpointAddress>
    {
        public EndpointAddress(string address, int port)
        {
            Address = address;
            Port = port;
        }

        [ProtoMember(1)]
        public string Address { get; set; }
        [ProtoMember(2)]
        public int Port { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EndpointAddress address && Equals(address);
        }

        public bool Equals(EndpointAddress other)
        {
            return Address == other.Address &&
                   Port == other.Port;
        }

        public override int GetHashCode()
        {
            var hashCode = 1820422833;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }

        public static bool operator ==(EndpointAddress left, EndpointAddress right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EndpointAddress left, EndpointAddress right)
        {
            return !(left == right);
        }
    }

    [GenerateDeltaObjectCode]
    [EntityService]
    public interface IRepositoryCommunicationEntity : IEntity, IHasPingDiagnostics
    {
        [ReplicationLevel(ReplicationLevel.Master)] EntitiesRepositoryConfig Config { get; set; } 
        [ReplicationLevel(ReplicationLevel.Server)] string ConfigId { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] int Num { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] [LockFreeReadonlyProperty] EndpointAddress InternalAddress { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] EndpointAddress ExternalAddress { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] bool CloudRequirementsMet { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] bool InitializationTasksCompleted { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] bool ExternalCommunicationNodeOpen { get; set; }

        [ReplicationLevel(ReplicationLevel.Always)] [LockFreeReadonlyProperty] CloudNodeType CloudNodeType { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] int ProcessId { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<bool> SetCloudRequirementsMet();
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<bool> SetInitializationTasksCompleted();
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask<bool> SetExternalCommNodeOpen();

        [RuntimeData]
        WantsToDisconnectEventProxy WantsToDisconnect { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> FireOnDisconnect();

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [RemoteMethod(60)]
        [SuppressCheckEntityIsLocked]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> NotifyOfExistingConnections(List<EndpointAddress> endpoints);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task NewNodeConnected(Guid newNodeId);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task EntityUpdate(UpdateBatch updateBatch);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task EntityUpdate(UpdateBatchContainer updateBatchContainer);

        //[AllowEmptyEntityId]
        //[AddNetworkProxyParameter]
        //[EntityMethodCallType(EntityMethodCallType.Immutable)]
        //Task EntityUpload(int typeId, Guid entityId, byte[] serializedSnapshot, long replicationMask, int version);

        [AddNetworkProxyParameter]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task EntityUpload(UploadBatchContainer uploadBatchContainer);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task EntityDestroyed(DestroyBatchContainer destroyBatchContainer);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task EntityDowngrade(DowngradeBatchContainer downgradeBatchContainer);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task Dump(string fileName);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task DumpEntity(int typeId, Guid entityId, string fileName);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task DumpEntitySerializedData(string fileName, int entityTypeId, Guid entityId, long replicationMask);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task<bool> SubscribeReplication(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLevel);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [SuppressCheckEntityIsLocked]
        Task<bool> UnsubscribeReplication(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLevel);

        [CheatRpc(AccountType.TechnicalSupport)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<string> GetEntityStatus(int typeId, Guid entityId);

        [CheatRpc(AccountType.TechnicalSupport)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<string> GetAllServiceEntityStatus();

        //TODO вынести метод в отдельную Entity, чтобы не реплицировать это api на клиент
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> ForceCloseConnection(Guid userId);

        //TODO вынести методы миграции в отдельную Entity, чтобы не реплицировать это api на клиент
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<StartMigrateEntityResult> StartMigrateEntity(int entityTypeId, Guid entityId);

        //TODO вынести методы миграции в отдельную Entity, чтобы не реплицировать это api на клиент
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<FinishMigrateEntityResult> FinishMigrateEntity(int entityTypeId, Guid entityId, Dictionary<ValueTuple<int, Guid>, Dictionary<Guid, int>> replicateRefsVersions);

        //TODO вынести методы миграции в отдельную Entity, чтобы не реплицировать это api на клиент
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task DispatchMigratedEntityDeferredRpc(int entityTypeId, Guid entityId);
    }

    public enum StartMigrateEntityResult
    {
        None,
        Success,
        ErrorUnknown,
        ErrorSourceRepositoryNotFound,
        ErrorSubscribe,
        ErrorEntityNotFound
    }
}
