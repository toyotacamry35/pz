using Assets.ColonyShared.GeneratedCode.Shared;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.Chain;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Core.Environment.Logging.Extension;
using SharedCode.Serializers.Protobuf;

using Vector3 = SharedCode.Utils.Vector3;
using SharedCode.Entities.Engine;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class MineableEntity : IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Mining.Entity");

        private readonly bool _debugEnabled = false;
        private MineableEntityDef MineableDef => (MineableEntityDef)Def;

        private ItemResourcePack _fillerResourcePack;
        private ItemResourcePack FillerResourcePack => _fillerResourcePack; // _fillerResourcePack ?? (_fillerResourcePack = (ItemResourcePack) _mineableDef.FillerResourcePack);
        private DamageTypeDef DefaultDamageType => MineableDef.DefaultDamageType;

        public Task<ItemResourcePack> GetFillerResourcePackImpl() => Task.FromResult(FillerResourcePack);
        public Task<DamageTypeDef> GetDefaultDamageTypeImpl() => Task.FromResult(DefaultDamageType);

        // It's probably no need & should be deleted:
        public async Task<bool> IsDamageTypeEffectiveForMiningImpl(DamageTypeDef type)
        {
            var forecast = await CalcCurrForecastMineResult(Guid.Empty, 1/*test dmg*/, type);
            return forecast != null && forecast.Any();
        }

        private Task DoOnDie(Guid entityId, int entityTypeId, PositionRotation corpsePlace)
        {
            //?: Is it still needed?
            // Call via `.Chain()` to destroy entty guaranteed after subscribers got DieEvent
            this.Chain().Destroy().Run();
            return Task.CompletedTask;
        }

        private static bool _invalidFillerResourcePackSpamPreventer;
        public async Task OnInit()
        {
            _fillerResourcePack = (ItemResourcePack)MineableDef.FillerResourcePack;

            CurrProgressActualTime = 5 * 60; //5min  //#todo: may be make base jdb for all mineables, then set this prop there (now it hardcoded)
            Mortal.DieEvent += DoOnDie;

            if (!_invalidFillerResourcePackSpamPreventer && !FillerResourcePack.IsValid)
            {
                Logger.IfWarn()?.Message($"Wrong `{nameof(FillerResourcePack)}` is set. Def: {Def}, _mineableDef: {MineableDef}").Write();
                _invalidFillerResourcePackSpamPreventer = true;
            }

            ((IHasStatsEngine)this).Stats.StatsReparsedEvent += OnStatsReparsed;
            await SetMineableHealth();

            //Logger.IfError()?.Message("---OnInit {0} {1}", this.TypeId, this.TypeName).Write();
        }

        private async Task OnStatsReparsed()
        {
            await SetMineableHealth();
        }

        private async Task SetMineableHealth()
        {
            await Stats.ChangeValue(GlobalConstsHolder.StatResources.HealthMaxStat, MineableDef.Health);
            await Stats.ChangeValue(GlobalConstsHolder.StatResources.HealthCurrentStat, MineableDef.Health);
        }

        public Task<LootTableBaseDef> GetLootTableImpl()
        {
            return Task.FromResult(MineableDef.LootTable.Target);
        }

        // It's probably no need & should be deleted:
        internal async Task<List<ProbabilisticItemPack>> CalcCurrForecastMineResult(Guid minerId, float dmg, DamageTypeDef dType)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   ##3.4.0 || 4.1.0 {nameof(CalcCurrForecastMineResult)}. `DType`: {dType?.ToString() ?? "null"},  `dmg`: {dmg}").Write();

            if (!Mortal.IsAlive)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"!{nameof(Mortal.IsAlive)} already.").Write();
                return new List<ProbabilisticItemPack>();
            }

            var request = new LootListRequest(dType, dmg, minerId);
            var result = (await GetMineableLootList(request)).ToList();
            AddFillerToLootList(result);
            
            return result;
        }
        void AddFillerToLootList(List<ProbabilisticItemPack> list)
        {
            // If bunch sum-chance < 1f, fill remaining part by filler
            float fillerChance = 0;
            for (int i = list.Count - 1; i >= 0; --i)
            {
                var entry = list[i];
                if (!entry.ItemPack.IsValid || entry.ItemPack == FillerResourcePack)
                {
                    fillerChance += entry.Chance;
                    list.RemoveAt(i);
                }
            }
            if (fillerChance > 0)
                list.Add(new ProbabilisticItemPack(FillerResourcePack, fillerChance));

            // Check control sum:
            if (SharedHelpers.CompareWithTol(1f, list.Sum(x => x.Chance), SharedHelpers.EqualityType.Inequal))
                Logger.IfWarn()?.Message($"result.Sum(x => x.Chance) != 1 (=={list.Sum(x => x.Chance)})").Write();
        }

        public async Task<IEnumerable<ProbabilisticItemPack>> GetMineableLootListImpl(LootListRequest lootRequest)
        {
            var table = await GetLootTableImpl();
            if (table == null)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{SharedHelpers.NowStamp}  #DBG: table == null return empty List").Write();
                return Enumerable.Empty<ProbabilisticItemPack>();
            }

            ProbabilisticItemPack high1 = default(ProbabilisticItemPack), high2 = default(ProbabilisticItemPack), low = default(ProbabilisticItemPack);
            bool haveLow = false;
            foreach (var item in FlattenProbabLootItemDataTree(await table.CalcItemsChances(lootRequest, Id, TypeId, EntitiesRepository), 1f))
            {
                if (item.Chance > high1.Chance)
                {
                    high2 = high1;
                    high1 = item;
                }
                else if (item.Chance > high2.Chance)
                    high2 = item;
                else if (!haveLow || item.Chance <= low.Chance)
                    low = item;
            }

            var res = new[] { high1, high2, low };

            if (_debugEnabled)
            {//#Dbg:
                // ReSharper disable once PossibleMultipleEnumeration
                var str = "Normalized Result list: " + Environment.NewLine + string.Join("," + Environment.NewLine, res);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#DBG:" + str).Write();
            }

            return res.Except(new[] { default(ProbabilisticItemPack) }).Distinct().ToArray();
        }

        /// <summary>
        /// Пересчитывает составную вероятность по вложенности в конечные вероятности отдельных item'ов. Возвращает плоское перечисление.
        /// </summary>
        public IEnumerable<ProbabilisticItemPack> FlattenProbabLootItemDataTree(IEnumerable<ProbabilisticLootItemData> tree, float weight)
        {
            foreach (var branch in tree.Where(b => !b.Hidden))
            {
                var branchWeight = branch.Forced ? weight : branch.Weight * weight;
                switch (branch.IsItemPackOrSubTable)
                {
                    case ItemPackOrSubTable.ItemPack:
                        yield return new ProbabilisticItemPack(branch.ItemPack, branchWeight);
                        break;
                    case ItemPackOrSubTable.SubTable:
                        foreach (var subBranch in FlattenProbabLootItemDataTree(branch.SubTable, branchWeight))
                            yield return subBranch;
                        break;
                }
            }
        }

        public ValueTask<bool> ChangeHealthInternalImpl(float deltaValue)
        {
            return new ValueTask<bool>(false);
        }

        public async ValueTask<DamageResult> ReceiveDamageInternalImpl(Damage damageInfo, Guid minerId, int minerTypeId)
        {
            //DbgLog.Log(13334, "7.1. ReceiveDamageInternal");

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("ReceiveDamageInternal Dmg:{0} Typ:{1} Victim:{2}.{3} Miner:{4}.{5}", damageInfo.BattleDamage, damageInfo.DamageType, TypeName, Id, ReplicaTypeRegistry.GetTypeById(minerTypeId)?.GetFriendlyName() ?? minerTypeId.ToString(), minerId).Write();

            // 'Cos of specific logic of getting damage via damage pools, we should clamp incoming damage into curr.health (not sure, it's best solution)
            var currHealth = await Health.GetHealthCurrent();
            var damage = Math.Min(Consts.MiningDamage, currHealth);
            if (damage <= 0)
                return DamageResult.None;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Damage to apply: {0}", damage).Write();

            bool feedbackToMiner = await ShouldGiveFeedbackToMiner(minerId, minerTypeId, damageInfo.IsMiningDamage);
            if (!feedbackToMiner)
                Logger.Debug/*Warn*/("#Dbg: !{0}.", nameof(ShouldGiveFeedbackToMiner));
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("TC-3466   ##_3.1 ShouldGiveFeedbackToMiner == {0}", feedbackToMiner).Write();


            await FinishBars(minerId, minerTypeId, damageInfo.DamageType, damage, damageInfo.AggressionPoint, feedbackToMiner, damageInfo.MiningLootMultiplier);

            if (!Mortal.IsAlive)
                return new DamageResult(damage);

            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("TC-3466   ##_3.44 Handle tail dmg-pool.").Write();;

            return new DamageResult(damage);
        }

        /// <summary>
        ///  Answers question, incl. should grant mined resources?
        /// </summary>
        private async Task<bool> ShouldGiveFeedbackToMiner(Guid minerId, int minerTypeId, bool isMiningDamage)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   --##_3.1  `ShouldGiveFeedbackToMiner` (`minerId`: {minerId};  `minerTypeId`: {minerTypeId},  `isMining`:{isMiningDamage}).").Write();
            if (!isMiningDamage)
                return false;

            if (minerId == Guid.Empty)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   --##_3.1  a").Write();
                return false;
            }

            using (var wrapper = await EntitiesRepository.Get(minerTypeId, minerId))
            {
                var minerWorldObject = PositionedObjectHelper.GetPositioned(wrapper, minerTypeId, minerId);
                if (minerWorldObject == null)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   --##_3.1  b").Write();
                    return false;
                }

                var dist = Vector3.GetDistance(MovementSync.Transform.Position, minerWorldObject.Transform.Position);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   --##_3.1  d ou `ShouldGiveFeedbackToMiner`.Return: {dist <= GlobalConstsHolder.GlobalConstsDef.MaxMiningDistance} //{nameof(dist)}: {dist} <= `MaxMiningDistance`: `MaxMiningDistance`. Mineable.pos: {MovementSync.Transform.Position}, miner.pos: {minerWorldObject.Transform.Position}.").Write();

                return dist <= GlobalConstsHolder.GlobalConstsDef.MaxMiningDistance;
            }
        }

        internal async Task FinishBars(Guid minerId, int minerTypeId, DamageTypeDef dType, float dmg, Vector3 aggressionPoint, bool feedbackToMiner, float miningLootMultiplier)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   ##_4.1 `FinishBars`.").Write();

            if (!Mortal.IsAlive)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"!{nameof(Mortal.IsAlive)} already.").Write();
                return;
            }

            //DbgLog.Log(13334, $"7.2. FinishBars: dmg:{dmg}, fbtm:{feedbackToMiner}, mlm:{miningLootMultiplier}");

            // 1. Grant items:
            if (feedbackToMiner)
            {
                var table = await GetLootTableImpl();
                if (table == null)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{SharedHelpers.NowStamp}  #DBG: table == null return empty List").Write();
                    return;
                }

                // 1.1. Calc. result items:
                var lootRequest = new LootListRequest(dType, dmg, minerId);
                var res = await table.CalcItems(lootRequest, Id, TypeId, EntitiesRepository);
                var grantItems = res.Where(item => item.ItemResource != null).ToArray();

                // 1.2. Give resources:
                if (grantItems.Any())
                    // R# swears at `EntitiesRepository`
                    await ItemsGranter.GrantItems(grantItems, minerId, EntitiesRepository, false, miningLootMultiplier);
            }

            // 2. TakeDamage:
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   ##_4.3 call {nameof(Health.ChangeHealth)}({nameof(dmg)}: {dmg}).").Write();
            await Health.ChangeHealth(-dmg);
        }

        // -- Destroyable -----------------------------------------
        public Task<bool> DestroyImpl() => Destroyable.Destroy();
        // --- IHasLifespan -----------------------------------------

        public Task LifespanExpiredImpl() => Lifespan.LifespanExpired();

        // --- others: ----------------------------------------------
        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }
        
        public ItemSpecificStats SpecificStats => MineableDef.DefaultStats;
    }

}
