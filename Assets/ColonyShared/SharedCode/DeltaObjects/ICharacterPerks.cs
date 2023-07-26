using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using GeneratorAnnotations;

namespace SharedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface ITemporaryPerks : ICharacterPerks, IItemsContainer, IDeltaObject { }

    [GenerateDeltaObjectCode]
    public interface IPermanentPerks : ICharacterPerks, IItemsContainer, IDeltaObject { }

    [GenerateDeltaObjectCode]
    public interface ISavedPerks : ICharacterPerks, IItemsContainer, IDeltaObject { }

    public interface ICharacterPerks
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaDictionary<int, ItemTypeResource> PerkSlots { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task AddPerkSlot(int slotId, ItemTypeResource perkSlotType);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task RemovePerkSlot(int slotId);
    }

    public static class CharacterPerks
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Task<bool> CanRemove(IEntity parentEntity, IItem item, int index, int count, bool manual, Type type)
        {
            if (parentEntity is IHasMutationMechanics)
            {
                IHasMutationMechanics mutationMechanicsEntity = (IHasMutationMechanics)parentEntity;
                var result = !(mutationMechanicsEntity.MutationMechanics.Stage?.StagePerks ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                    .SelectMany(v => v.Target.Items)
                    .Where(v => v.Target != null)
                    .Select(v => v.Target)
                    .Contains(item.ItemResource);

                if (!result)
                    Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be removed from '{type.Name}'. " +
                        $"Reason: Item is 'Stage Item'. Faction '{mutationMechanicsEntity.Faction?.____GetDebugShortName() ?? "null"}' and Stage '{mutationMechanicsEntity.MutationMechanics.Stage?.____GetDebugShortName() ?? "null"}'.")
                        .Write();

                return Task.FromResult(result);
            }

            return Task.FromResult(true);
        }

        public static Task<bool> CanAdd(IEntity parentEntity, IItem item, int index, int count, bool manual, Type type)
        {
            if (parentEntity is IHasMutationMechanics)
            {
                IHasMutationMechanics mutationMechanicsEntity = (IHasMutationMechanics)parentEntity;
                var result = (mutationMechanicsEntity.MutationMechanics.Stage?.Perks ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                    .Concat(mutationMechanicsEntity.MutationMechanics.Stage?.StagePerks ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                    .SelectMany(v => v.Target.Items)
                    .Where(v => v.Target != null)
                    .Select(v => v.Target)
                    .Contains(item.ItemResource);

                if (!result)
                    Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be added to '{type.Name}'. " +
                                                $"Reason: Faction '{mutationMechanicsEntity.Faction?.____GetDebugShortName() ?? "null"}' and Stage '{mutationMechanicsEntity.MutationMechanics.Stage?.____GetDebugShortName() ?? "null"}' does't not allow this item.")
                        .Write();

                return Task.FromResult(result);
            }

            return Task.FromResult(true);
        }

        public static Task<PropertyAddress> OnBeforeItemRemoved(IWorldCharacter worldCharacter, IItem item, int index, int count, bool manual)
        {
            return Task.FromResult<PropertyAddress>(null);
        }
    }
}