using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Time;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;
using Core.Reflection;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
using GeneratedCode.DeltaObjects;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Arithmetic
{
    // Is similar to `CalcerCalcer`, but about lootTable & on cluster
    public static class LootTablePredicateCalcer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly IReadOnlyDictionary<Type, Func<LootTablePredicateDef, LootItemData, LootListRequest, Guid, int, IEntitiesRepository, Task<bool>>> Implementations;

        // --- C-tor ----------------------

        static LootTablePredicateCalcer()
        {
            Implementations = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(
                v => v.Name == "CalcImpl"
                && v.GetParameters().Length == 6
                && v.GetParameters()[1].ParameterType == typeof(LootItemData)
                && v.GetParameters()[2].ParameterType == typeof(LootListRequest)
                && v.GetParameters()[3].ParameterType == typeof(Guid)
                && v.GetParameters()[4].ParameterType == typeof(int)
                && v.GetParameters()[5].ParameterType == typeof(IEntitiesRepository)
                && v.ReturnType == typeof(Task<bool>)
                && typeof(LootTablePredicateDef).IsAssignableFrom(v.GetParameters()[0].ParameterType)
                )
                .Select(
                method => new { func = DelegateCreator.LootTablePredicateMagicMethod<LootTablePredicateDef, Task<bool>>(method), type = method.GetParameters()[0].ParameterType }
                )
                .ToDictionary(v => v.type, v => v.func);

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootTablePredicateCalcer)} static ctor done. Implementations.N == {Implementations.Count}").Write();
        }

        public static void TriggerStaticCtorToBeCalledHack()
        {
        }

        // --- API ----------------------------------------------------------

        [System.Diagnostics.Contracts.Pure]
        public static async Task<bool> Calc([NotNull] this LootTablePredicateDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (pred == null)
                return false;

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(LootTablePredicateCalcer)}.{nameof(Calc)} Implementations.N == {Implementations.Count}. by type `{pred.GetType()}` has table?: {Implementations.ContainsKey(pred.GetType())}.").Write();
            return await Implementations[pred.GetType()](pred, lootItem, lootRequest, lootedEntityId, lootedEntityTypeId, repo);
        }

        // --- Privates: -------------------------------------------------------------------------------------

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicatePlayerHaveNotPerkDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            BaseItemResource itemResource = null;
            if (pred.Perk.IsValid)
                itemResource = pred.Perk.Target;
            else if (lootItem.IsItemPackOrSubTable == ItemPackOrSubTable.ItemPack)
                itemResource = lootItem.ItemResRefPack.ItemResource.Target;
            else
                return true;
            using (var wrapper = await repo.Get<IWorldCharacterClientFull>(lootRequest.Requester))
            {
                if (wrapper.AssertIfNull(nameof(wrapper)))
                    return false;

                var worldCharacter = await wrapper.GetOrSubscribe<IWorldCharacterClientFull>(lootRequest.Requester);

                return !worldCharacter.SavedPerks.Items
                    .Concat(worldCharacter.TemporaryPerks.Items)
                    .Concat(worldCharacter.PermanentPerks.Items)
                    .Any(p => p.Value.Item.ItemResource.Equals(itemResource));
            }
        }

        [UsedImplicitly] [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicateInGameTimeDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return InGameTime.IsNowWithinInterval(pred.TimeInterval, repo);
        }

        [UsedImplicitly] [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicateExpiredLifespanPercentDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.Get(lootedEntityTypeId, lootedEntityId))
            {
                var entty = wrapper?.Get<IHasLifespanClientBroadcast>(lootedEntityTypeId, lootedEntityId, ReplicationLevel.ClientBroadcast);
                if (entty == null)
                {
                    Logger.IfWarn()?.Message($"Can't get {nameof(IHasLifespanClientBroadcast)} by {lootedEntityTypeId}, {lootedEntityId}.").Write();
                    return false;
                }

                return await entty.Lifespan.IsExpiredLifespanPercentInRange(pred.FromIncluding, pred.TillExcluding);
            }
        }

        [UsedImplicitly] [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicateTrueDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return Task.FromResult(true);
        }

        [UsedImplicitly] [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicateFalseDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return Task.FromResult(false);
        }

        [UsedImplicitly]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicateInverseDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return !await pred.Predicate.Target.Calc(lootItem, lootRequest, lootedEntityId, lootedEntityTypeId, repo);
        }

        [UsedImplicitly]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicateAndDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            //return pred.Predicates.All(x=>x.Target.Calc(obj));
            for (int i = 0; i < pred.Predicates.Count; ++i)
            {
                if (!await pred.Predicates[i].Target.Calc(lootItem, lootRequest, lootedEntityId, lootedEntityTypeId, repo))
                    return false;
            }
        
            return true;
        }

        [UsedImplicitly]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicateOrDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            //return pred.Predicates.Any(x=>x.Target.Calc(obj));
            for (int i = 0;  i < pred.Predicates.Count;  ++i)
            {
                if (await pred.Predicates[i].Target.Calc(lootItem, lootRequest, lootedEntityId, lootedEntityTypeId, repo))
                    return true;
            }

            return false;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [UsedImplicitly]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicateIsAnyOfDamageTypesInLootRequestDef pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            if (lootRequest?.DamageType == null)
            {
                Logger.IfError()?.Message($"lootRequest == null ({lootRequest}) || lootRequest.DamageByTypeDic == null ({lootRequest?.DamageType}).").Write();
                return Task.FromResult(false);
            }

            for (int i = 0;  i < pred.DamageTypes.Count;  ++i)
            {
                //if (lootRequest.DamageByTypeDic.Any(x => x.Key == dType))
                //    return true;
                    if (lootRequest.DamageType == pred.DamageTypes[i])
                        return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [UsedImplicitly]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicatePremium pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return Task.FromResult(false);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [UsedImplicitly]
        private static Task<bool> CalcImpl([NotNull] LootTablePredicateContentKey pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {
            return Task.FromResult(ContentKeyServiceEntity.ContainsKey(pred.Key));
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [UsedImplicitly]
        private static async Task<bool> CalcImpl([NotNull] LootTablePredicateSpellPredicate pred, [NotNull] LootItemData lootItem, [CanBeNull] LootListRequest lootRequest, Guid lootedEntityId, int lootedEntityTypeId, IEntitiesRepository repo)
        {

            var spellTargetParam = new SpellCastParameterTarget() { Target = new OuterRef<IEntity>(lootedEntityId, lootedEntityTypeId) };
            var castData = new SpellCastWithTarget(new SpellCastParameterTarget[] { spellTargetParam });

            var spellPred = new SpellPredCastData(
                      castData: castData,
                      currentTime: 0,
                      wizard: new OuterRef<IWizardEntity>(lootRequest.Requester, WizardEntity.StaticTypeId),
                      caster: new OuterRef<IEntity>(lootRequest.Requester, WorldCharacter.StaticTypeId),
                      slaveMark: null,
                      modifiers: null,
                      canceled: false,
                      repo:repo);

            return await SpellPredicates.CheckPredicate(spellPred, pred.SpellPredicate, repo);
        }
    }
}

