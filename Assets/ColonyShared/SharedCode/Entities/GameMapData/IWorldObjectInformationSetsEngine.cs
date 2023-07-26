using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.GameMapData;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Utils;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IWorldObjectInformationSetsEngine: IDeltaObject
    {
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<WorldObjectInformationClientSubSetDef, int> WorldObjectInformationRefsCounter { get; }

        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaDictionary<WorldObjectInformationClientSubSetDef, OuterRef> CurrentWorldObjectInformationRefs { get; }//TODO переделать на ReplicationRef если он появится

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSet(WorldObjectInformationClientSubSetDef subSetDef);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSet(WorldObjectInformationClientSubSetDef subSetDef);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task<AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheat(WorldObjectInformationClientSubSetDef subSetDef);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task<RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheat(WorldObjectInformationClientSubSetDef subSetDef);
    }

    public enum AddWorldObjectInformationSubSetResult
    {
        None,
        Success,
        ErrorNotFound
    }
    public enum RemoveWorldObjectInformationSubSetResult
    {
        None,
        Success,
        ErrorNotFound
    }
}
