using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using Entities.GameMapData;
using GeneratedCode.Repositories;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.EntitySystem.Delta;
using SharedCode.Wizardry;

namespace SharedCode.Entities
{
    public interface IHasWorldObjectsInformationDataSetEngine
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)]
        IWorldObjectsInformationDataSetEngine WorldObjectsInformationDataSetEngine { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task RegisterWorldObjectsInNewInformationSet(EntityId worldObjectId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task RegisterWorldObjectsInNewInformationSetBatch(List<EntityId> worldObjectsIds);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task UnregisterWorldObjectsInNewInformationSet(EntityId worldObjectId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task UnregisterWorldObjectsInNewInformationSetBatch(List<EntityId> worldObjectsIds);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> ContainsWorldObjectInformation(EntityId worldObjectId);
        [ReplicationLevel(ReplicationLevel.Master)]

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<List<EntityId>> ContainsWorldObjectInformationList(List<EntityId> worldObjectsId);
    }

    [GenerateDeltaObjectCode]
    public interface IWorldObjectsInformationDataSetEngine: IDeltaObject
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)]
        WorldObjectInformationSetDef WorldObjectInformationSetDef { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task RegisterWorldObjectsInNewInformationSet(EntityId worldObjectId);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task RegisterWorldObjectsInNewInformationSetBatch(List<EntityId> worldObjectsIds);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task UnregisterWorldObjectsInNewInformationSet(EntityId worldObjectId);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task UnregisterWorldObjectsInNewInformationSetBatch(List<EntityId> worldObjectsIds);
    }
}
