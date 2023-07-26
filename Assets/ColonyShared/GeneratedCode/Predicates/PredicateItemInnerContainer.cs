using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using ColonyShared.ManualDefsForSpells;

namespace Assets.Src.Predicates
{
    class PredicateItemInnerContainer : IPredicateBinding<PredicateItemInnerContainerDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateItemInnerContainerDef def)
        {
            var selfDef = (PredicateItemInnerContainerDef)def;
            var spellCastWithSlotItem = (SpellCastWithSlotItem)cast.CastData;
            var targetRef = cast.Caster;
            if (selfDef.Target.Target != null)
                targetRef = await selfDef.Target.Target.GetOuterRef(cast, repository);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var ec = await repository.Get(targetRef.TypeId, targetRef.Guid))
            {
                var character = ec.Get<IWorldCharacterClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);

                ISlotItemClientFull item = null;
                if (spellCastWithSlotItem.IsInventory)
                    character.Inventory.Items.TryGetValue(spellCastWithSlotItem.SlotId, out item);
                else
                    character.Doll.Items.TryGetValue(spellCastWithSlotItem.SlotId, out item);

                return item.Stack > 0 && item.Item.AmmoContainer.Size > 0 && item.Item.AmmoContainer.Items.Count > 0 && item.Item.AmmoContainer.Items[0].Item?.ItemResource == selfDef.Item;
            }
        }
    }
}