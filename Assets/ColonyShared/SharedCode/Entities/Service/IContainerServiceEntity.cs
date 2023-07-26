using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IContainerServiceEntity : IEntity
    {
        [RemoteMethod]
        Task<ContainerItemOperation> MoveItem(PropertyAddress source, int sourceSlotId, PropertyAddress destination, int destinationSlotId, int count, Guid clientSrcEntityId);

        [RemoteMethod]
        Task<ContainerItemOperation> RemoveItem(PropertyAddress source, int sourceSlotId, int count, Guid clientEntityId);
    }

}
