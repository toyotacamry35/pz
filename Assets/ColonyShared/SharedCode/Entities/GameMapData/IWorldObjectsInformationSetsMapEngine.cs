using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.GameMapData;
using GeneratorAnnotations;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    public interface IWorldObjectsInformationSetsMapEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<WorldObjectInformationSetDef, OuterRef> WorldObjectInformationSets { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<Dictionary<WorldObjectInformationSetDef, OuterRef>> Subscribe(List<WorldObjectInformationSetDef> worldObjectSetsDef, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> Unsubscribe(List<WorldObjectInformationSetDef> worldObjectSetsDef, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> AddWorldObject(OuterRef worldObjectRef);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> RemoveWorldObject(OuterRef worldObjectRef);
    }
}
