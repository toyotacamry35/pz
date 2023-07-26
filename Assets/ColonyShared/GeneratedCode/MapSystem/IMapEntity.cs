using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.Custom.Config;
using ProtoBuf;
using SharedCode.Entities.Service;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using GeneratedCode.MapSystem;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;
using SharedCode.MapSystem;

namespace SharedCode.MapSystem
{
	[ProtoContract]
	public class MapMeta
	{
		[ProtoMember(1)] public MapDef MapDef { get; set; }
        [ProtoMember(2)] public bool IsDead { get; set; }
    }

	[DatabaseSaveType(DatabaseSaveType.Explicit)]
	[GenerateDeltaObjectCode]
	public interface IMapEntity : IEntity
	{
		[ReplicationLevel(ReplicationLevel.ClientBroadcast)] event Func<string, string, Task> NewChatMessageEvent;
		[LockFreeReadonlyProperty]
		[ReplicationLevel(ReplicationLevel.Always)]
		Guid RealmId { get; set; }

		[LockFreeReadonlyProperty]
		[ReplicationLevel(ReplicationLevel.Always)]
		RealmRulesDef RealmRules { get; set; }
	
		[LockFreeReadonlyProperty]
		[ReplicationLevel(ReplicationLevel.Always)]
		MapDef Map { get; set; }

		[BsonIgnore, ReplicationLevel(ReplicationLevel.Always)]
		MapEntityState State { get; }
        [ReplicationLevel(ReplicationLevel.Always)]
		[BsonIgnore]
        bool Dead { get; }

        [BsonIgnore, ReplicationLevel(ReplicationLevel.Server)]
		IDeltaList<IWorldSpaceDescription> WorldSpaces { get; set; }

		[ReplicationLevel(ReplicationLevel.Server)]
		Task<bool> SetMapEntityState(MapEntityState state);

		IDeltaDictionary<OuterRef<IEntity>, bool> SavedScenes { get; set; }

		[ReplicationLevel(ReplicationLevel.Server)]
		Task<bool> ChangeChunkDescription(Guid descriptionId, MapChunkState newState, Guid unityRepositoryId);
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> OnLastUserLeft();

        [CheatRpc(AccountType.TechnicalSupport)]
		[ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.Immutable)]
		Task SpawnNewBots(List<Guid> botIds, string spawnPointTypePath);

		Task<bool> TryAquireSpawnRightsForPointsSet(OuterRef<IEntity> spawner, SceneChunkDef mapSceneDef);

		[EntityMethodCallType(EntityMethodCallType.Lockfree)]
		Task<OuterRef<IEntity>> GetWorldSpaceForPoint(Vector3 point);

		[EntityMethodCallType(EntityMethodCallType.Lockfree)]
		Task<bool> NotifyAllCharactersViaChat(string text);
	}

	public enum MapEntityState
	{
		None,
		Loading,
		Loaded,
		Failed
	}

	[ProtoContract]
	public struct WorldSpacesConfiguration
	{
		//пока что только 1 на 1 должен быть
		[ProtoMember(1)] public int SizeX { get; set; }
		[ProtoMember(2)] public int SizeY { get; set; }
	}

	[GenerateDeltaObjectCode]
	public interface IWorldSpaceDescription : IDeltaObject
	{
		MapDef ChunkDef { get; set; }
		Guid UnityRepositoryId { get; set; }
		Guid WorldSpaceRepositoryId { get; set; }
		Guid WorldSpaceGuid { get; set; }
		MapChunkState State { get; set; }
	}

	public enum MapChunkState
	{
		None,
		Loading,
		Loaded,
		Failed
	}

	public interface IDatabasedMapedEntity
	{
	}

	public interface IHasMapped : IScenicEntity
	{
	}
}