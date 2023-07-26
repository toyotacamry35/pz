using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Arithmetic;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.Custom.Containers;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactAddItems : IImpactBinding<ImpactAddItemsDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddItemsDef def)
        {
            var targetRef = cast.Caster;

            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            IEnumerable<BaseItemResource> itemsToAdd;
            switch (def.ItemsBatchType)
            {
                case ItemsBatchType.All:
                    {
                        itemsToAdd = def.Items.Select(v => v.Target);
                        break;
                    }
                case ItemsBatchType.First:
                    {
                        itemsToAdd = def.Items.Take(1).Select(v => v.Target);
                        break;
                    }
                case ItemsBatchType.OneOfItem:
                    {
                        var random = new Random((int)DateTime.Now.ToBinary());
                        itemsToAdd = def.Items.Skip(random.Next(def.Items.Count)).Take(1).Select(v => v.Target);
                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            var result = true;
            var itemsGroups = itemsToAdd.GroupBy(v => v.GetType());
            foreach (var itemGroup in itemsGroups)
            {
                PropertyAddress address = default;
                float itemsCountMultiplier;
                using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    if (typeof(ItemResource).IsAssignableFrom(itemGroup.Key))
                    {
                        address = ContainerUtils.GetPropertyAddress(wrapper, targetRef, def.Container, repo);
                    }
                    else if (typeof(PerkResource).IsAssignableFrom(itemGroup.Key))
                    {
                        var entity = wrapper?.Get<IHasPerksServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                        address = EntityPropertyResolver.GetPropertyAddress(entity.TemporaryPerks);
                    }
                    else if (typeof(CurrencyResource).IsAssignableFrom(itemGroup.Key))
                    {
                        var entity = wrapper?.Get<IHasCurrencyServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server);
                        address = EntityPropertyResolver.GetPropertyAddress(entity.Currency);
                    }
                    var calcerContext = new CalcerContext(wrapper, targetRef, repo, cast, ctx: cast.Context);
                    itemsCountMultiplier = def.AmountOfResourcesMultiplier == null ? 1 : (await def.AmountOfResourcesMultiplier.Target.CalcAsync(calcerContext)).Float;
                }

                if (address?.IsValid() ?? false)
                {
                    var itemsPack = itemGroup.ToArray().Select(v => new ItemResourcePack(v, (uint)(def.Count * itemsCountMultiplier), def.Slot)).ToList();
                    var itemTransaction = new ItemAddBatchManagementTransaction(itemsPack, address, false, repo);
                    ContainerItemOperation res = await itemTransaction.ExecuteTransaction();
                    result &= res.IsSuccess;
                }
                else
                {
                    result = false;
                }
            }

//            return result;
        }
    }
}