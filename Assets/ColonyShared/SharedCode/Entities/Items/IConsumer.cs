using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities.Items
{
    public interface IHasConsumer
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IConsumer Consumer { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IConsumer : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<bool> ConsumeItemInSlot(int slotId, int spellsGroupIndex, bool fromInventory);
    }
}
