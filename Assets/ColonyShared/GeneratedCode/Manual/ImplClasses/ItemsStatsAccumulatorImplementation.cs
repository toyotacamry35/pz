using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using NLog.Fluent;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Src.Aspects.Impl.Stats;

namespace GeneratedCode.DeltaObjects
{
    public partial class ItemsStatsAccumulator: IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private ModifierCauser _key;

        // --- IHookOnInit ---------------------------------
        public Task OnInit()
        {
            return Initialization();
        }

        public Task OnDatabaseLoad()
        {
            return Initialization();
        }

        private async Task Initialization()
        {
            _key = new ModifierCauser() { Causer = GlobalConstsHolder.GlobalConstsDef.ItemsStatsAccumulatorKey };

            // shortcuts:
            var repo = EntitiesRepository;
            var id = parentEntity.Id;
            var typeId = parentEntity.TypeId;

            using (var wrapper = await repo.Get(typeId, id))
            {
                var worldChar = wrapper?.Get<IWorldCharacterClientFull>(typeId, id, ReplicationLevel.ClientFull);
                if (worldChar == null)
                {
                    Logger.IfError()?.Message("Can't get {target} by {typeId}, {id}.", nameof(IWorldCharacterClientFull), typeId, id);
                    return;
                }

                // Subscribe:
                worldChar.Doll.Items.OnChanged += OnItemsChanged;
                //Dbg: worldChar.Doll.Items.OnChanged += DbgOnDollItemsChanged;
                worldChar.PermanentPerks.Items.OnChanged += OnItemsChanged;
                //worldChar.TemporaryPerks.Items.OnChanged += OnItemsChanged;
                worldChar.Doll.UsedSlots.OnChanged += OnListItemsChanged;
                worldChar.Stats.StatsReparsedEvent += async () => await OnItemsChangedInternal(parentEntity.TypeId, parentEntity.Id);
                await OnItemsChangedInternal(typeId, id);
            }
        }

        // --- Privates: -------------------------------------------

        //#Dbg:
        // ReSharper disable once UnusedMember.Local
        async Task DbgOnDollItemsChanged(int typeId, Guid id)
        {
            var repo = EntitiesRepository;

            using (var wrapper = await repo.Get(typeId, id))
            {
                var worldChar = wrapper?.Get<IWorldCharacterClientFull>(typeId, id, ReplicationLevel.ClientFull);
                if (worldChar == null)
                {
                    Logger.IfError()?.Message($"worldChar == null").Write();
                    return;
                }
                /*
                var dollItems = worldChar.Doll?.Items?.Values ?? Enumerable.Empty<ISlotItemClientFull>();
                var dollGeneralStats = (from slotItem in dollItems
                                        let item = slotItem?.Item
                                        where item != null
                                        let appliedStats = item.GeneralStats                              // приобретенные статы
                                        let resourceStats = (item.ItemResource as IHasStatsResource)?.GeneralStats.Target?.Stats // статы по умолчанию (прописано в ресурсах)
                                        let summStats = 
                                            (appliedStats ?? Enumerable.Empty<StatModifier>())
                                            .Concat(resourceStats ?? Enumerable.Empty<StatModifier>())
                                        where summStats != null
                                        from stat in summStats
                                        select stat).ToArray();
                                        */
                //Logger.IfWarn()?.Message($"#Dbg:  z z z z z z z z DbgOnDollItemsChanged: dollItems.N: {dollItems.Count()}. Stats:\n{String.Join(";  ", dollGeneralStats)}.").Write();
            }
        }

        private async Task OnItemsChanged(DeltaDictionaryChangedEventArgs eventArgs)
        {
            var typeId = eventArgs.Sender.ParentTypeId;
            var id = eventArgs.Sender.ParentEntityId;
            await OnItemsChangedInternal(typeId, id);
        }

        private async Task OnListItemsChanged(DeltaListChangedEventArgs eventArgs)
        {
            var typeId = eventArgs.Sender.ParentTypeId;
            var id = eventArgs.Sender.ParentEntityId;
            await OnItemsChangedInternal(typeId, id);
        }

        private async Task OnItemsChangedInternal(int typeId, Guid id)
        {
            // shortcuts:
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(typeId, id))
            {
                var worldChar = wrapper?.Get<IWorldCharacterClientFull>(typeId, id, ReplicationLevel.ClientFull);
                var hasStats  = wrapper?.Get<IHasStatsEngineServer>(typeId, id, ReplicationLevel.Server);
                if (worldChar == null || hasStats == null)
                {
                    Logger.IfWarn()?.Message($"Can't get {nameof(IWorldCharacterClientFull)} &&/|| {nameof(IHasStatsEngineServer)} by {typeId}, {id}.").Write();
                    return;
                }

                // 1. Collect all active items (weapon) SpecificStats. GeneralStats we'll take into account a little later

                var activeItems = new List<ISlotItemClientFull>();
                var usedSlots = worldChar.Doll?.UsedSlots ?? Enumerable.Empty<ResourceIDFull>();
                foreach (var slotResId in usedSlots)
                {
                    var usedIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(slotResId);
                    // ReSharper disable once PossibleNullReferenceException //Its safe here
                    if (worldChar.Doll.Items.ContainsKey(usedIndex))
                        activeItems.Add(worldChar.Doll.Items[usedIndex]);
                }

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"ActiveItems:[{string.Join(", ", activeItems.Select(x => x?.Item?.ItemResource?.____GetDebugAddress()))}]").Write();

                var selectedActiveItems = activeItems.Select(slotItem => slotItem?.Item).Where(t => t != null);
                List<StatModifierProto> sm = new List<StatModifierProto>();
                foreach (var item in selectedActiveItems)
                    sm.AddRange(await item.Stats.GetSnapshot(StatType.Specific));

                var activeSpecificStats = selectedActiveItems.SelectMany(t => (t.ItemResource as IHasStatsResource)?.SpecificStats.Target?.Stats ?? Enumerable.Empty<StatModifier>() // статы предмета по умолчанию (прописано в ресурсах)
                    ).Concat(sm.Select(v => new StatModifier(v.Stat, v.Value))).ToArray();

                // добавляем дефолтные статы из brute которых нет в активном оружии (или все, если активного оружия нет)
                activeSpecificStats = activeSpecificStats.Concat( (worldChar.SpecificStats).Stats
                                                                                .Where(x => activeSpecificStats.All(y => x.Stat != y.Stat)) 
                                                                                ).ToArray();

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"ActiveSpecificStats:[{string.Join(", ", activeSpecificStats)}]").Write();

                // 2. Set `HasActiveWeapon` & `ActiveWeaponDamageType`:
                if (activeItems.Any())
                {
                    // _itemsStatsAccumulator.HasActiveWeapon = true;
                    // _itemsStatsAccumulator.ActiveWeaponDamageType = activeItems[0].Item.ItemResource.SpecificStats.Target.DamageType;
                    var specStats = (activeItems[0].Item.ItemResource as IHasStatsResource)?.SpecificStats.Target;
                    await SetHasActiveWeaponAndDamageType(true, specStats?.DamageType, specStats?.WeaponSize);
                }
                else
                {
                    // _itemsStatsAccumulator.HasActiveWeapon = false;
                    // _itemsStatsAccumulator.ActiveWeaponDamageType = worldChar.DefaultStats.DamageType.Target;
                    var stats = worldChar.SpecificStats;
                    await SetHasActiveWeaponAndDamageType(false, stats.DamageType, stats.WeaponSize);
                }

                // 3. Collect all GeneralStats of worn items, incl. active weapon:
                var dollItems = (worldChar.Doll?.Items?.Values ?? Enumerable.Empty<ISlotItemClientFull>())
                    .Select(slotItem => slotItem?.Item).Where(t => t != null);

                List<StatModifierProto> smDoll = new List<StatModifierProto>();
                foreach (var item in dollItems)
                    smDoll.AddRange(await item.Stats.GetSnapshot(StatType.General));

                var dollGeneralStats = (from item in dollItems
                                        let appliedStats = smDoll.Select(v => new StatModifier(v.Stat, v.Value))                 // приобретенные статы
                                        let resourceStats = (item.ItemResource as IHasStatsResource)?.GeneralStats.Target?.Stats // статы по умолчанию (прописано в ресурсах)
                                        let summStats =
                                                 (appliedStats  ?? Enumerable.Empty<StatModifier>())
                                          .Concat(resourceStats ?? Enumerable.Empty<StatModifier>())
                                        where summStats != null
                                        from stat in summStats
                                        select stat).ToArray();

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"DollGeneralStats:[{string.Join(", ", dollGeneralStats)}]").Write();
                
                // 4. Collect all GeneralStats & SpecificStats of perks:
                var perksItems = /*(worldChar.TemporaryPerks?.Items?.Values ?? Enumerable.Empty<ISlotItemClientFull>())
                          .Concat*/(worldChar.PermanentPerks?.Items?.Values ?? Enumerable.Empty<ISlotItemClientFull>())
                          .Select(slotItem => slotItem?.Item).Where(t => t != null);
                
                List<StatModifierProto> smPerk = new List<StatModifierProto>();
                foreach (var item in perksItems)
                    smPerk.AddRange(await item.Stats.GetSnapshot(StatType.General | StatType.Specific));

                var perksAllStats = (from item in perksItems
                                     let appliedStats = smPerk.Select(v => new StatModifier(v.Stat, v.Value))
                                     let resourceGeneralStats = (item.ItemResource as IHasStatsResource)?.GeneralStats.Target?.Stats
                                     let resourceSpecificStats = (item.ItemResource as IHasStatsResource)?.SpecificStats.Target?.Stats
                                     let summStats =
                                                appliedStats
                                         .Concat(resourceGeneralStats  ?? Enumerable.Empty<StatModifier>())
                                         .Concat(resourceSpecificStats ?? Enumerable.Empty<StatModifier>())
                                     where summStats != null
                                     from stat in summStats
                                     select stat).ToArray();

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"PerksAllStats:[{string.Join(", ", perksAllStats)}]").Write();
                
                // 5. Put all 3 summands together:
                //Dictionary<StatResource, float> itemsStats = 
                StatModifierData[] itemsStats =
                                                    dollGeneralStats
                                                    .Concat(perksAllStats)
                                                    .Concat(activeSpecificStats)
                                                    .GroupBy(v => v.Stat)
                                                    .Select(x => new StatModifierData (x.Key, StatModifierType.Add, x.Sum(s => s.Value)))
                                                    //.ToDictionary(k => k.StatRes.Target, i => i.Val);
                                                    .ToArray();

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"ResultStats:[{string.Join(", ", itemsStats)}]").Write();
                
                if (false)
                {
                    //Logger.IfInfo()?.Message($"activeSpecificStats : \n{StatModifier.ToString(activeSpecificStats)}, dollItems :\n{StatModifier.ToString(dollGeneralStats)}, perksAllStats :\n{StatModifier.ToString(perksAllStats)}, itemsStats : \n{StatModifier.ToString(itemsStats)}").Write();
                    Logger.IfDebug()?.Message(
                        $"activeSpecificStats : \n{StatModifier.ToString(activeSpecificStats)}, dollItems :\n{StatModifier.ToString(dollGeneralStats)}, perksAllStats :\n{StatModifier.ToString(perksAllStats)}, itemsStats : \n{StatModifierData.ToString(itemsStats)}")
                        .Write();
                }

                await hasStats.Stats.UpdateModifiers(itemsStats, _key);
            }
        }

        public Task<bool> SetHasActiveWeaponAndDamageTypeImpl(bool has, DamageTypeDef dType, WeaponSizeDef size)
        {
            HasActiveWeapon = has;
            ActiveWeaponDamageType = dType;
            ActiveWeaponSize = size;
            return Task.FromResult(true);
        }
    }
}
