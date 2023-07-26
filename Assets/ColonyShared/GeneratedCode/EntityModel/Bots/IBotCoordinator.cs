using GeneratedCode.Custom.Config;
using SharedCode.AI;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneratedCode.EntityModel.Bots
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    public interface IBotCoordinator : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Always)] SpawnPointTypeDef BotSpawnPointTypeDef { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<Guid, IBotsHolder> BotsByAccount { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.Immutable)] Task Register();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast), EntityMethodCallType(EntityMethodCallType.Immutable)] Task Initialize(MapDef mapDef, List<OuterRef<IEntity>> botsRefs, LegionaryEntityDef botConfig);
        [ReplicationLevel(ReplicationLevel.Server)] Task ActivateBots(Guid account, List<Guid> botsIds);
        [ReplicationLevel(ReplicationLevel.Server)] Task DeactivateBots(Guid account);
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IBotsHolder: IDeltaObject
    {
        IDeltaDictionary<Guid, bool> Bots { get; set; }
    }
}
