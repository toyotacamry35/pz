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
    [GenerateDeltaObjectCode]
    public interface IWorldObjectsInformationDataSetEntity : IEntity, IHasWorldObjectsInformationDataSetEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<OuterRef, IWorldObjectPositionInformation> Positions { get; }
    }

    [GenerateDeltaObjectCode]
    public interface IWorldObjectPositionInformation: IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Vector3 Position { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> SetPosition(Vector3 position);
    }

    [GenerateDeltaObjectCode]
    public interface ICharacterPositionInformation: IWorldObjectPositionInformation
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        MutationStageDef Mutation { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> SetMutation(MutationStageDef mutation);
    }

}
