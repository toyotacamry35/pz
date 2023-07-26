using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.GameMapData;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace SharedCode.Entities
{
    public interface IHasWorldObjectsInformationSetsMapEngine
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.Server)]
        IWorldObjectsInformationSetsMapEngine WorldObjectsInformationSetsMapEngine { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> RegisterWorldObjectsInNewInformationSet(OuterRef worldObjectSetRef);
    }
}
