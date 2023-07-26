using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Src.Predicates
{
    public class PredicateCheckItem : IPredicateBinding<PredicateCheckItemDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateCheckItemDef indef)
        {
            var def = (PredicateCheckItemDef) indef;
            var targetRef = cast.Caster;

            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            int itemsCount = 0;
            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var dollEntity = wrapper?.Get<IHasDollClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);
                if (dollEntity != null)
                    itemsCount += GetItemsCount(dollEntity.Doll, def.Item);

                var inventoryEntity = wrapper?.Get<IHasInventoryClientFull>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientFull);
                if (inventoryEntity != null)
                    itemsCount += GetItemsCount(inventoryEntity.Inventory, def.Item);
            }

            return itemsCount >= def.Count;
        }

        private static int GetItemsCount(IItemsContainerClientFull itemsCountainer, BaseItemResource resource)
        {
            if (resource != default)
                return itemsCountainer.Items.Where(x => x.Value.Item?.ItemResource == resource).Sum(x => x.Value.Stack);
            else
            {
                return itemsCountainer.Size - itemsCountainer.Items.Count;
            }
        }
    }
}