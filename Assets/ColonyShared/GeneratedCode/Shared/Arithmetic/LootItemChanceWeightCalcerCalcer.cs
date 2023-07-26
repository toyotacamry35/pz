using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers.Cluster;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;
using Core.Reflection;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Arithmetic
{
    // Is similar to `CalcerCalcer`, but about lootTable & on cluster
    public static class LootItemChanceWeightCalcerCalcer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly IReadOnlyDictionary<Type, Func<LootItemChanceWeightCalcerDef, LootItemData, LootListRequest, Guid, IEntitiesRepository, Task<float>>> Implementations;

        // --- C-tor ----------------------

        static LootItemChanceWeightCalcerCalcer()
        {
            Implementations = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(
                v => v.Name == "CalcImpl"
                && v.GetParameters().Length == 5
                && v.GetParameters()[1].ParameterType == typeof(LootItemData)
                && v.GetParameters()[2].ParameterType == typeof(LootListRequest)
                && v.GetParameters()[3].ParameterType == typeof(Guid)
                && v.GetParameters()[4].ParameterType == typeof(IEntitiesRepository)
                && v.ReturnType == typeof(Task<float>)
                && typeof(LootItemChanceWeightCalcerDef).IsAssignableFrom(v.GetParameters()[0].ParameterType)
                )
                .Select(
                method => new { func = DelegateCreator.LootItemChanceWeightCalcerMagicMethod<LootItemChanceWeightCalcerDef, Task<float>>(method), type = method.GetParameters()[0].ParameterType }
                )
                .ToDictionary(v => v.type, v => v.func);

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootItemChanceWeightCalcerCalcer)} static ctor done. Implementations.N == {Implementations.Count}").Write();
        }

        public static void TriggerStaticCtorToBeCalledHack()
        {
        }

        // --- API ----------------------------------------------------------


        [System.Diagnostics.Contracts.Pure]
        public static async Task<float> Calc([NotNull] this LootItemChanceWeightCalcerDef calcer, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, IEntitiesRepository repo)
        {
            if (calcer == null)
                return -1;

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootItemChanceWeightCalcerCalcer)}.{nameof(Calc)} Implementations.N == {Implementations.Count}. by type `{calcer.GetType()}` has table?: {Implementations.ContainsKey(calcer.GetType())}.").Write();
            return await Implementations[calcer.GetType()](calcer, lootItem, lootRequest, lootedEntityId, repo);
        }

        // --- Privates: -------------------------------------------------------------------------------------

        [UsedImplicitly]
        private static Task<float> CalcImpl([NotNull] LootItemChanceWeightCalcerConstantDef calcer, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, IEntitiesRepository repo)
        {
            return Task.FromResult(calcer.Value);
        }

        [UsedImplicitly]
        private static Task<float> CalcImpl([NotNull] LootItemChanceWeightCalcerByLootRequestDamageTypeDef calcer, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, IEntitiesRepository repo)
        {
            if (calcer.DamageType == null)
            {
                Logger.IfWarn()?.Message($"{nameof(calcer.DamageType)} == null! (Entity Id: {lootedEntityId})").Write();
                return Task.FromResult(0f);
            }

            if (lootRequest?.DamageType == null)
            {
                Logger.IfError()?.Message($"lootRequest == null ({lootRequest}) || lootRequest.DamageByTypeDic == null ({lootRequest?.DamageType})").Write();
                return Task.FromResult(0f);
            }

            return (lootRequest.DamageType == calcer.DamageType)
                ? Task.FromResult(calcer.Weight)
                : Task.FromResult(0f);
        }

        [UsedImplicitly]
        private static async Task<float> CalcImpl([NotNull] LootItemChancePerkWeightCalcerDef calcer, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, IEntitiesRepository repo)
        {
            switch (lootItem.IsItemPackOrSubTable)
            {
                case ItemPackOrSubTable.SubTable:
                    return calcer.Value;
                case ItemPackOrSubTable.ItemPack:
                    using (var wrapper = await repo.Get<IWorldCharacterClientFull>(lootRequest.Requester))
                    {
                        if (wrapper.AssertIfNull(nameof(wrapper)))
                            return 0f;

                        var worldCharacter = await wrapper.GetOrSubscribe<IWorldCharacterClientFull>(lootRequest.Requester);

                        int perkDestroyCount;
                        worldCharacter.PerksDestroyCount.TryGetValue(lootItem.ItemResRefPack.ItemResource, out perkDestroyCount);

                        float result = calcer.Value;
                        for (int i = 0; i < perkDestroyCount; i++)
                            result *= calcer.PerkDestroyMult;
                        return result;
                    }
                default:
                    Logger.IfError()?.Message($"Unexpected LootTable entry content (`{nameof(lootItem.IsItemPackOrSubTable)}`: {lootItem.IsItemPackOrSubTable}, `{nameof(lootItem)}`: {lootItem}). (entity.Id: {lootedEntityId})").Write();
                    return calcer.Value;
            }
        }

    }
}
