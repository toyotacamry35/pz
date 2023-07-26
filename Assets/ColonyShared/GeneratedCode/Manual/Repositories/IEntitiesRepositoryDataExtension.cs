using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using GeneratedCode.Manual.AsyncStack;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace GeneratedCode.Manual.Repositories
{
    public readonly struct RepoCommEntityStatus
    {
        public string InternalAddress { get; }
        public int InternalPort { get; }
        public string ConfigId { get; }
        public bool ExternalCommunicationNodeOpen { get; }

        public RepoCommEntityStatus(string internalAddress, int internalPort, string configId, bool externalCommunicationNodeOpen)
        {
            InternalAddress = internalAddress;
            InternalPort = internalPort;
            ConfigId = configId;
            ExternalCommunicationNodeOpen = externalCommunicationNodeOpen;
        }
    }

    public class ClusterStatusReport
    {
        public string Label { get; set; }
        public bool IsReady { get; set; }
        public List<(string, int)> ConnectedTo { get; set; } = new List<(string, int)>();
        public List<(string,int)> FailedToConnectTo { get; set; } = new List<(string, int)>();
        public List<RepoCommEntityStatus> HasRepoComEntities { get; set; } = new List<RepoCommEntityStatus>();
    }

    public interface IEntitiesRepositoryDataExtension
    {
        void Release(ref AsyncStackEnumerable containers);
        Task EntityUpdate(UpdateBatch updateBatch);
        Task EntityUpdate(UpdateBatchContainer updateBatchContainer);
        //Task EntityUpload(int typeId, Guid entityId, byte[] serializedSnapshot, long replicationMask, int version, INetworkProxy networkProxy);
        Task EntityUpload(UploadBatchContainer uploadBatchContainer, INetworkProxy networkProxy);
        Task EntityDowngrade(DowngradeBatchContainer downgradeBatchContainer);
        Task EntityDestroyed(DestroyBatchContainer destroyBatchContainer);
        IRepositoryCommunicationEntity GetRepositoryCommunicationEntityByNetworkProxy(INetworkProxy networkProxy);
        //void InitializeAsyncEntitiesRepositoryRequestContext();
        Task<ClusterStatusReport> IsClusterReady();
        void InvokeRemoteCallResult(Action action);
        ValueTask LockAgain(EntitiesContainer entitiesContainer);
        bool NeedWaitProcessDeltaSnapshots(EntitiesContainer wrapper);

        Task<Guid> GetDataBaseServiceEntityid(int typeId, Guid entityId);
    }

    public interface IEntitiesRepositoryDebugExtension
    {
        void Dump(Stream stream);

        Task DumpEntity(int typeId, Guid entityId, Stream stream);

        Task DumpEntitySerializedData(Stream stream, int entityTypeId, Guid entityId,
            ReplicationLevel replicationMask);
    }
}
