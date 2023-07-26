using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Aspects.Doings;
using Assets.Src.Client;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace Assets.Src.Client
{
    public class CharacterRuntimeData
    {
        public Guid CurrentPrimaryWorldSceneRepositoryId { get; set; }

        public Guid CharacterId { get; set; }

        public Guid UserId { get; set; }
    }
}
namespace SharedCode.Entities.Cloud
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.Client)]
    public interface IClientCommunicationEntity : IEntity
    {
        [LockFreeMutableProperty, ReplicationLevel(ReplicationLevel.Server)]
        bool LevelLoaded { get; }

        Task SetLevelLoaded();

        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaList<ConnectionInfo> Connections { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        event DisconnectedEventHandler DisconnectedByAnotherConnection;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task DisconnectByAnotherConnection();


        [ReplicationLevel(ReplicationLevel.Master)]
        event DisconnectedEventHandler GracefullLogoutEvent;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task GracefullLogout();

        [ReplicationLevel(ReplicationLevel.Master)]
        event DisconnectedByErrorEventHandler DisconnectedByError;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task DisconnectByError(string reason, string details);

        [ReplicationLevel(ReplicationLevel.Master)]
        event DisconnectedEventHandler LoginConfirmed;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task ConfirmLogin();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<MapHostInitialInformation> GetMapHostInitialInformation();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> AddConection(string host, int port, Guid nodeId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> RemoveConection(Guid nodeId);
    }

    public static class ClientCommRuntimeDataProvider
    {
        public static CharacterRuntimeData CharRuntimeData;
        public static Guid CurrentPrimaryWorldSceneRepositoryId { get; set; }
    }

    public delegate Task DisconnectedEventHandler();
    public delegate Task DisconnectedByErrorEventHandler(string reason, string details);

    [ProtoContract]
    public class ConnectionInfo
    {
        [ProtoMember(1)]
        public string Host { get; set; }

        [ProtoMember(2)]
        public int Port { get; set; }

        [ProtoMember(3)]
        public Guid NodeId { get; set; }
    }

    [ProtoContract]
    public class MapHostInitialInformation
    {
        [ProtoMember(1)]
        public string SpawnPointPath { get; set; }
    }
}
