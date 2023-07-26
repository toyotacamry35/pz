using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.Arithmetic;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using Core.Reflection;
using SharedCode.Repositories;

namespace Assets.ColonyShared.SharedCode.Arithmetic
{
    public static class LootTableCalcer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly IReadOnlyDictionary<Type, Func<LootTableBaseDef, LootListRequest, Guid, int, IEntitiesRepository, Task<IEnumerable<ProbabilisticLootItemData>>>> Implementations;
        
        // --- C-tor ----------------------

        static LootTableCalcer()
        {
            Implementations = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(
                v => v.Name == "CalcItemsChancesDo"
                && v.GetParameters().Length == 5 
                && v.GetParameters()[1].ParameterType == typeof(LootListRequest)
                && v.GetParameters()[2].ParameterType == typeof(Guid)
                && v.GetParameters()[3].ParameterType == typeof(int)
                && v.GetParameters()[4].ParameterType == typeof(IEntitiesRepository)
                && v.ReturnType == typeof(Task<IEnumerable<ProbabilisticLootItemData>>)
                && typeof(LootTableBaseDef).IsAssignableFrom(v.GetParameters()[0].ParameterType)
                )
                .Select(
                method => new { func = DelegateCreator.LootTableMagicMethod<LootTableBaseDef, Task<IEnumerable<ProbabilisticLootItemData>>>(method), type = method.GetParameters()[0].ParameterType }
                )
                .ToDictionary(v => v.type, v => v.func);

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootTableCalcer)} static ctor done. Implementations.N == {Implementations.Count}").Write();
        }

        public static void TriggerStaticCtorToBeCalledHack()
        {
        }

        // --- API ----------------------------------------------------------

        [System.Diagnostics.Contracts.Pure] [NotNull]
        public static async Task<IEnumerable<ItemResourcePack>> CalcItems([NotNull] this LootTableBaseDef table, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (table == null)
                return Array.Empty<ItemResourcePack>();

            var itemsWithChances = await CalcItemsChances(table, lootRequest, lootedEntityId, lootedEntityTypeId, repo);
            return RollItems(itemsWithChances, Guid.Empty);
        }

        private static IEnumerable<ItemResourcePack> RollItems(IEnumerable<ProbabilisticLootItemData> treeItems, Guid lootedEntityId)
        {
            var itemsList = new List<ProbabilisticLootItemData>();
            var chancesList = new List<float>();
            foreach (var item in treeItems)
                if (item.Forced)
                    foreach (var deepItem in GoDeeper(item, lootedEntityId))
                        yield return deepItem;
                else
                {
                    itemsList.Add(item);
                    chancesList.Add(item.Weight);
                }
            if (itemsList.Count == 0)
                yield break;

            int chosenIndex = SharedHelpers.ChoseRandomly1From(chancesList);
            if (chosenIndex == Consts.InvalidIndex || chosenIndex >= chancesList.Count)
            {
                Logger.IfWarn()?.Message("`ChoseRandomly1From()` returned InvalidIndex.").Write();
                chosenIndex = 0;
            }
            foreach (var deepItem in GoDeeper(itemsList[chosenIndex], lootedEntityId))
                yield return deepItem;
        }

        private static IEnumerable<ItemResourcePack> GoDeeper(ProbabilisticLootItemData entry, Guid lootedEntityId)
        {
            switch (entry.IsItemPackOrSubTable)
            {
                case ItemPackOrSubTable.ItemPack:
                    yield return entry.ItemPack;
                    break;
                case ItemPackOrSubTable.SubTable:
                    foreach (var rolledItem in RollItems(entry.SubTable, lootedEntityId))
                        yield return rolledItem;
                    break;
                default:
                    Logger.IfError()?.Message($"Unexpected LootTable entry content (`{nameof(entry.IsItemPackOrSubTable)}`: {entry.IsItemPackOrSubTable}, `{nameof(entry)}`: {entry}). (entity.Id: {lootedEntityId})").Write();
                    break;
            }
        }

        // Return namely `Chances` inside list. I.e. Summ <= 1.0f. If < 1f, then (1 - Summ) == Chance of nothing
        [System.Diagnostics.Contracts.Pure] [NotNull]
        public static async Task<IEnumerable<ProbabilisticLootItemData>> CalcItemsChances([NotNull] this LootTableBaseDef table, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (table == null)
                return Array.Empty<ProbabilisticLootItemData>();

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootTableCalcer)}.{nameof(CalcItemsChances)} Implementations.N == {Implementations.Count}. by type `{table.GetType()}` has table?: {Implementations.ContainsKey(table.GetType())}.").Write();
            return await Implementations[table.GetType()](table, lootRequest, lootedEntityId, lootedEntityTypeId, repo); 
        }

        // --- Privates: -------------------------------------------------------------------------------------
        static ProbabilisticLootItemData ProcessDataWithMult(float itemsCountMult, ProbabilisticLootItemData data)
        {
            if (data.IsItemPackOrSubTable == ItemPackOrSubTable.ItemPack)
            {
                var irp = (ItemResourcePack)data.ItemPack;
                var resResource = itemsCountMult == 1 ? irp.Count : StochasticAmoutOfResources(itemsCountMult, irp.Count);
                irp.Count = resResource;
                data.ItemPack = irp;
            }
            else
                foreach (var entry in data.SubTable)
                    ProcessDataWithMult(itemsCountMult, entry);
            return data;
        }
        [UsedImplicitly]
        private static async Task<IEnumerable<ProbabilisticLootItemData>> CalcItemsChancesDo(LootTableDef table, LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (table.LootTable == null || table.LootTable.Count == 0)
                return Array.Empty<ProbabilisticLootItemData>();
        
            var res = new List<ProbabilisticLootItemData>();
            float sumWeight = 0f;
            float itemsCountMultiplier = table.AmountOfResourcesMultiplier.Target == null ? 1f : await table.AmountOfResourcesMultiplier.Target.CalcAsync(
                new OuterRef<IEntity>(lootRequest.Requester, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter))), repo);
            // Fill `preResultLists` & `preResultWeights`:
            foreach (var entry in table.LootTable)
            {
                if (!(await (entry.Predicate.Target?.Calc(entry, lootRequest, lootedEntityId, lootedEntityTypeId, repo)
                    ?? (table.DefaultPredicate?.Calc(entry, lootRequest, lootedEntityId, lootedEntityTypeId, repo)
                    ?? Task.FromResult(true)))))
                    continue;

                var weight = await (entry.WeightCalcer.Target?.Calc(entry, lootRequest, lootedEntityId, repo) ?? Task.FromResult(-1f));
                if (weight == 0f)
                    continue;
                // ItemPack || SubLootTable polymorphism handling:
                if (entry.IsItemPackOrSubTable == ItemPackOrSubTable.SubTable)
                {
                    var chances = await entry.SubTable.Target.CalcItemsChances(lootRequest, lootedEntityId, lootedEntityTypeId, repo);
                    res.Add(new ProbabilisticLootItemData(chances.Select(x => ProcessDataWithMult(itemsCountMultiplier, x)), weight, entry.Hidden));
                }
                else if (entry.IsItemPackOrSubTable == ItemPackOrSubTable.ItemPack)
                {
                    res.Add(ProcessDataWithMult(itemsCountMultiplier, new ProbabilisticLootItemData((ItemResourcePack)entry.ItemResRefPack, weight, entry.Hidden)));
                }
                else
                {
                    Logger.IfError()?.Message($"Unexpected LootTable entry content (`{nameof(entry.IsItemPackOrSubTable)}`: {entry.IsItemPackOrSubTable}, `{nameof(entry)}`: {entry}). (entity.Id: {lootedEntityId})").Write();
                    continue;
                }
                if (weight > 0f)
                    sumWeight += weight;
            }

            foreach (var entry in res)
                if (!entry.Forced)
                    entry.Weight /= sumWeight;

            return res;
        }

        [ThreadStatic]
        static System.Random _rnd;
        /// <summary>
        /// Turns 1.6 `multiplier` & 2 `count` into 3 (with a 80% probability) || 4 (with a 20% probability)
        /// </summary>
        public static uint StochasticAmoutOfResources(float multiplier, uint count) => (uint)ColonyHelpers.SharedHelpers.StochasticRound(multiplier * count);

        // All lacking chance for total sum == 1.0 is added with null item
        [UsedImplicitly]
        private static async Task<IEnumerable<ProbabilisticLootItemData>> CalcItemsChancesDo(LootTableByDamageTypesDef table, LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (lootRequest?.DamageType == null)
            {
                Logger.IfError()?.Message($"lootRequest == null ({lootRequest}) || lootRequest.DamageType == null ({lootRequest?.DamageType}).").Write();
                return Array.Empty<ProbabilisticLootItemData>();
            }

            

            if (table.TablesByDamageTypes == null || table.TablesByDamageTypes.Count == 0)
                return Array.Empty<ProbabilisticLootItemData>();

            float itemsCountMultiplier = table.AmountOfResourcesMultiplier.Target == null ? 1f : await table.AmountOfResourcesMultiplier.Target.CalcAsync(
                new OuterRef<IEntity>(lootRequest.Requester, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter))), repo);
                
            //List<LootTableAndDamageType> tablesAndDTypes = table.TablesByDamageTypes.FindAll(x => x.DamageTypes.Any(y => y == kvp.Key));
            List<LootTableAndDamageType> tablesAndDTypes = table.TablesByDamageTypes.FindAll((lootTbl) =>
            {
                foreach (var entry in lootTbl.DamageTypes)
                    if (entry == lootRequest.DamageType)
                        return true;

                return false;
            });
            if (tablesAndDTypes.Count == 0)
                return Array.Empty<ProbabilisticLootItemData>();

            var res = new List<ProbabilisticLootItemData>();
            foreach (var subTable in tablesAndDTypes)
            {
                var lootList = await subTable.LootTableTable.Target.CalcItemsChances(lootRequest, lootedEntityId, lootedEntityTypeId, repo);
                res.AddRange(lootList.Select(x => ProcessDataWithMult(itemsCountMultiplier, x)));
            }

            // Complement result list to 1.0f chances sum (by null item - means give nothing):

            //float currResDamage = res.Sum(x => x.Chance);
            float currResDamage = 0f;
            foreach (var entry in res.Where(r => !r.Forced))
                currResDamage += entry.Weight;

            if (currResDamage > 1)
            {
                Logger.IfError()?.Message($"{nameof(currResDamage)} > 1! : {res}. (EntityId: {lootedEntityId})").Write();
                return res;
            }

            if (currResDamage < 1)
            {
                float remainingInefficientDamage = 1 - currResDamage;
                res.Add(new ProbabilisticLootItemData(new ItemResourcePack(), remainingInefficientDamage));
            }

            return res;
        }

    }

}
