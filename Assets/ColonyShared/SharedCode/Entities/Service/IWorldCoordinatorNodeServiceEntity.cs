using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Cheats;
using GeneratedCode.Custom.Config;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.Aspects.Sessions;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Network;
using SharedCode.Utils;

namespace SharedCode.Entities.Service
{
    [ProtoContract]
    public class MapLoginMeta
    {
        [ProtoMember(1)]
        public Guid TargetRealmGuid { get; set; }
        [ProtoMember(2)]
        public MapDef TargetMap { get; set; }
        [ProtoMember(3)]
        public Guid TargetMapId { get; set; }
        [ProtoMember(4)]
        public Guid TargetTeamId { get; set; }
        [ProtoMember(5)]
        public Guid UserId { get; set; }
        [ProtoMember(6)]
        public Guid CurrentMapId { get; set; }
        [ProtoMember(7)]
        public Guid CurrentRealmId { get; set; }
        [ProtoMember(8)]
        public string CharacterName { get; set; }
        [ProtoMember(9)]
        public RealmRulesDef RealmRules { get; set; }
        public override string ToString()
        {
            return $"{nameof(MapLoginMeta)}:" +
                $"{nameof(TargetRealmGuid)}={TargetRealmGuid};" +
                $"{nameof(TargetMap)}={TargetMap?.____GetDebugShortName()};" +
                $"{nameof(TargetMapId)}={TargetMapId};" +
                $"{nameof(TargetTeamId)}={TargetTeamId};" +
                $"{nameof(UserId)}={UserId};" +
                $"{nameof(CurrentMapId)}={CurrentMapId};" +
                $"{nameof(CurrentRealmId)}={CurrentRealmId};" +
                $"{nameof(CharacterName)}={CharacterName};" + 
                $"{nameof(RealmRules)}={RealmRules?.____GetDebugAddress()}";
        }
    }
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    [GenerateDeltaObjectCode]
    public interface IWorldCoordinatorNodeServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<CreateOrConfirmMapResult> RequestLoginToMap(MapLoginMeta loginMeta);


        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<CreateOrConfirmMapResult> RequestLogoutFromMap(Guid userId, bool terminal);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> CancelMapRequest(Guid requestId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task UpdateMapQueue();

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<Guid> GetMapIdByUserId(Guid userId);

        [CheatRpc(AccountType.TechnicalSupport)]
        [Cheat]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<string> GlobalNotification(string notificationText);
    }

    [ProtoContract]
    public struct MapElement
    {
        [ProtoMember(1)]
        public Guid MapId { get; set; }
        [ProtoMember(2)]
        public MapDef MapDef { get; set; }
        [ProtoMember(3)]
        public Guid MapRepositoryId { get; set; }
    }

    [ProtoContract]
    public struct MapInstanceInfo
    {
        [ProtoMember(1)]
        public Guid MapHost { get; set; }
        [ProtoMember(2)]
        public Guid Repository { get; set; }
        [ProtoMember(3)]
        public Guid MapId { get; set; }
    }

    [ProtoContract]
    public struct CreateOrConfirmMapResult
    {
        [ProtoMember(1)]
        public CreateOrConfirmMapResultType Result { get; set; }
        [ProtoMember(2)]
        public Guid MapId { get; set; }

        public bool IsSuccess
        {
            get
            {
                return Result == CreateOrConfirmMapResultType.Exist ||
                       Result == CreateOrConfirmMapResultType.AddToQueue;
            }
        }
    }

    public enum CreateOrConfirmMapResultType
    {
        None,
        Exist,
        AddToQueue,
        Error
    }

    [ProtoContract]
    public struct CreateOrGetChunkAnswer
    {
        [ProtoMember(1)]
        public Guid ChunkEntityId { get; set; }
        [ProtoMember(2)]
        public Guid ChunkEntityRepositoryId { get; set; }
    }
    [ProtoContract]
    public class WorldNodeInfoResult
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string Host { get; set; }

        [ProtoMember(3)]
        public int Port { get; set; }

        [ProtoMember(4)]
        public int UnityPort { get; set; }

        [ProtoMember(5)]
        public string Map { get; set; }

        [ProtoMember(6)]
        public Guid MapHostId { get; set; }
        [ProtoMember(7)]
        public string WorldSpaceHost { get; set; }

        [ProtoMember(8)]
        public int WorldSpacePort { get; set; }
        [ProtoMember(9)]
        public Guid MapId { get; set; }
    }
}
