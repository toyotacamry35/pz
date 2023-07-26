using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Aspects.Science;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Network;
using SharedCode.Utils;
using Assets.ResourceSystem.Aspects.Banks;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IBankServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<OuterRef<IEntity>> GetBanker();
    }
}
