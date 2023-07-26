using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Utils.BsonSerialization;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class ImpactRemoveItemFromInventory : IImpactBinding<ImpactRemoveItemDef>
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRemoveItemDef indef)
        {
            var def = (ImpactRemoveItemDef) indef;
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var ec = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                var character = ec.Get<IWorldCharacterServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);
                var dollAddress = EntityPropertyResolver.GetPropertyAddress(character.Doll);
               
                int count = def.Count;
                var removeBatch = new ItemRemoveBatchManagementTransaction(GetListToRemove(character, def.Item, count), false, repo);
                var opResult = await removeBatch.ExecuteTransaction();
                if (!opResult.IsSuccess)
                {        
                    count -= opResult.ItemsCount;
                    var removeBatch2 = new ItemRemoveBatchManagementTransaction(GetListToRemove(character, def.Item, count), false, repo);
                    var opResult2 = await removeBatch2.ExecuteTransaction();
                }
            }
        }
        private List<RemoveItemBatchElement> GetListToRemove(IWorldCharacterServer character, BaseItemResource item, int count)
        {
            var result = new List<RemoveItemBatchElement>();
            var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);
            var dollAddress = EntityPropertyResolver.GetPropertyAddress(character.Doll);
            int itemCount = 0;
            foreach (var invItem in character.Inventory.Items)
            {
                if(invItem.Value.Item.ItemResource == item)
                {
                    var _stack = Math.Min(invItem.Value.Stack, count - itemCount);
                    itemCount += _stack;
                    result.Add(new RemoveItemBatchElement(inventoryAddress, invItem.Key, _stack, invItem.Value.Item.Id));
                }

                if (itemCount >= count)
                    return result;
            }
            foreach (var dollItem in character.Doll.Items)
            {
                if (dollItem.Value.Item.ItemResource == item)
                {
                    var _stack = Math.Min(dollItem.Value.Stack, count - itemCount);
                    itemCount += _stack;
                    result.Add(new RemoveItemBatchElement(dollAddress, dollItem.Key, _stack, dollItem.Value.Item.Id));
                }

                if (itemCount >= count)
                    return result;
            }
            return result;
        }
    }
   
}
