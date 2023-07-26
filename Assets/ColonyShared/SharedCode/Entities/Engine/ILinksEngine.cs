using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.Aspects.Impl.Definitions;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Entities.Engine
{
    /// <summary>
    /// Is used f.e. for quests - conceptually is adds to an entity a dict-ry of outer refs to entities by string keys (filled by spell words)
    /// </summary>
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILinksEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)] Task SetLinksFromScene(Dictionary<LinkTypeDef, List<OuterRef<IEntity>>> links);

        // Just run-time dic-ry:
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<LinkTypeDef, ILinksHolder> Links { get; set; }
        // Saved to DB dic-ry:
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<LinkTypeDef, ILinksHolder> SavedLinks { get; set; }
       
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] ValueTask<OuterRef<IEntity>> GetLinked(LinkTypeDef link);
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] ValueTask<IEnumerable<OuterRef<IEntity>>> GetLinkeds(LinkTypeDef link);
        [ReplicationLevel(ReplicationLevel.Server)] Task AddLinkRef(LinkTypeDef key, OuterRef<IEntity> outerRef, bool watched, bool saved);
        [ReplicationLevel(ReplicationLevel.Server)] Task RemoveLinkKey(LinkTypeDef key);
        [ReplicationLevel(ReplicationLevel.Server)] Task RemoveLinkRef(OuterRef<IEntity> outerRef);
        [ReplicationLevel(ReplicationLevel.Server)] Task RemoveLinkRefByKey(LinkTypeDef key, OuterRef<IEntity> outerRef);

    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILinksHolder : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<OuterRef, bool> Links { get; set; }
    }
}
