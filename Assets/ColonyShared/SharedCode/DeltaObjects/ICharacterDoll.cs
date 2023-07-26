using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System.Threading.Tasks;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface ICharacterDoll : IItemsContainer, IDeltaObject
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaList<int> BlockedSlotsId { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaList<ResourceIDFull> UsedSlots { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> CanAddUsedSlot(ResourceIDFull dollSlotRes);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> AddUsedSlot(ResourceIDFull dollSlotRes);
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> RemoveUsedSlot(ResourceIDFull dollSlotRes);
        
        /// <summary>
        /// Блокирует слот для использования. Этот метод НЕ делает слот не используемым, если он уже используется!
        /// Список заблокированных слотов НЕ сохраняется в базе! 
        /// </summary>
        /// <param name="slots"></param>
        /// <returns></returns>
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> AddBlockedForUsageSlots(SlotDef[] slots);
        
        /// <summary>
        /// Разблокирует слот для использования.
        /// </summary>
        /// <param name="slots"></param>
        /// <returns></returns>
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> RemoveBlockedForUsageSlots(SlotDef[] slots);
    }
}