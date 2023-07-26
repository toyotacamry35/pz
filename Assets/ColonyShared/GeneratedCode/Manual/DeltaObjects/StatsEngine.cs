using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Repositories;
using SharedCode.Serializers;
using Src.Aspects.Impl.Stats;
using Src.Aspects.Impl.Stats.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class StatsEngine : IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy, IHookOnUnload
    {
        private const int MAX_DEPENDENCY_LEVEL = 2;
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource _cts;
        private List<StatResource> _dependencyRestrictionList = null;
        private Dictionary<CalcerDef, List<StatResource>> _nontrackableCalcers = new Dictionary<CalcerDef, List<StatResource>>();
        private Dictionary<StatResource, List<StatResource>> _dependencyTree = new Dictionary<StatResource, List<StatResource>>();

        public async Task OnInit()
        {
            await Initialization(true);
        }

        public async Task OnDatabaseLoad()
        {
            if (TimeStats == null)
                TimeStats = new DeltaDictionary<StatResource, ITimeStat>();
            if (TimeStatsBroadcast == null)
                TimeStatsBroadcast = new DeltaDictionary<StatResource, ITimeStat>();
            if (ValueStats == null)
                ValueStats = new DeltaDictionary<StatResource, IValueStat>();
            if (ValueStatsBroadcast == null)
                ValueStatsBroadcast = new DeltaDictionary<StatResource, IValueStat>();
            if (ProxyStats == null)
                ProxyStats = new DeltaDictionary<StatResource, IProxyStat>();
            if (ProceduralStats == null)
                ProceduralStats = new DeltaDictionary<StatResource, IProceduralStat>();
            if (AccumulatedStats == null)
                AccumulatedStats = new DeltaDictionary<StatResource, IAccumulatedStat>();
            if (ProceduralStatsBroadcast == null)
                ProceduralStatsBroadcast = new DeltaDictionary<StatResource, IProceduralStat>();
            if (AccumulatedStatsBroadcast == null)
                AccumulatedStatsBroadcast = new DeltaDictionary<StatResource, IAccumulatedStat>();

            await Initialization(false, true);
        }

        private async Task Initialization(bool hardReset, bool SetToDefaultForce = false)
        {
            IHasMortal mortal = parentDeltaObject as IHasMortal;
            if (mortal != null)
            {
                mortal.Mortal.DieEvent += OnDie;
                mortal.Mortal.ResurrectEvent += OnResurrect;
                if (SetToDefaultForce || await mortal.Mortal.GetIsAlive())
                    await SetToDefault(hardReset);
            }
            else
            {
                await SetToDefault(hardReset);
            }
            CheckAndStartRecalculation();

            if (parentDeltaObject is IWorldCharacter wc)
            {
                wc.OnIdleModeStarted += GoIntoIdleMode;
                wc.OnIdleModeStopped += GetBackFromIdleMode;
            }
        }

        private async Task OnResurrect(Guid entityId, int typeId, PositionRotation dummy)
        {
            InitStatsDef();
            StatsDef.AssertIfNull(nameof(StatsDef), $"eId:{entityId}, tId:{typeId}");         //Ловлю редкий nullref при resurrect'е персонажа
            StatsDef?.Stats.AssertIfNull("StatsDef?.Stats", $"eId:{entityId}, tId:{typeId}"); //Ловлю редкий nullref при resurrect'е персонажа

            foreach (var stat in StatsDef.Stats) 
            {
                if(stat.Target is TimeStatDef timeStatDef)
                    await InitializeStat<ITimeStat, TimeStat>(timeStatDef, TimeStats, TimeStatsBroadcast, true);
            }
            //await SetToDefault(true); we should not reset all stats on death, allthough we shouldn't reset all timestats either. Still better than what we have now
            CheckAndStartRecalculation();
        }

        public async ValueTask<bool> ChangeValueImpl(StatResource statResource, float delta)
        {
            if (StatsDef?.DoNotWorkAtAll ?? true)
                return true;

            ITimeStat timeStat = default;
            if ((TimeStats?.TryGetValue(statResource, out timeStat) ?? false) || (TimeStatsBroadcast?.TryGetValue(statResource, out timeStat) ?? false))
            {
                await timeStat.ChangeValue(delta); // No RecalculateDependencyStats() call, cause dependencies from Time stats is prohibited
                return true;
            }

            IValueStat valueStat = default;
            if ((ValueStats?.TryGetValue(statResource, out valueStat) ?? false) || (ValueStatsBroadcast?.TryGetValue(statResource, out valueStat) ?? false))
            {
                if (await valueStat.ChangeValue(delta))
                    await RecalculateDependencyStats(statResource, false, true);
                return true;
            }

            // Logger.IfInfo()?.Message($"Can't find TimeStats = \"{statResource.____GetDebugShortName()}\" on entity with type = {EntitiesRepositoryBase.GetTypeById(_entityHolder.TypeId).Name}").Write();
            return false;
        }

        public async ValueTask<bool> SetModifiersImpl(StatModifierData[] modifiers, ModifierCauser causer)
        {
            if (StatsDef?.DoNotWorkAtAll ?? false) // is needed, 'cos we're here 1st time  from `OnInit`, so `StatsDef` could be uninitialized
                return true;

            IAccumulatedStat accumulatedStat = default;
            foreach (var modifier in modifiers)
            {
                if ((AccumulatedStats?.TryGetValue(modifier.Stat, out accumulatedStat) ?? false) || (AccumulatedStatsBroadcast?.TryGetValue(modifier.Stat, out accumulatedStat) ?? false))
                {
                    if (await accumulatedStat.AddModifier(causer, modifier.ModifierType, modifier.Value))
                        await RecalculateDependencyStats(modifier.Stat, false, true);
                }
            }

            //Logger.IfInfo()?.Message($"Can't find AccumulatedStats = \"{statResource.____GetDebugShortName()}\" on entity with type = {EntitiesRepositoryBase.GetTypeById(_entityHolder.TypeId).Name}").Write();
            return true;
        }

        public async ValueTask<bool> UpdateModifiersImpl(StatModifierData[] modifiers, ModifierCauser causer)
        {
            if (StatsDef?.DoNotWorkAtAll ?? false) // is needed, 'cos we're here 1st time  from `OnInit`, so `StatsDef` could be uninitialized
                return true;

            HashSet<StatResource> statsForRemoveCauser = new HashSet<StatResource>();
            Dictionary<StatResource, StatModifierData> statsForUpdating = new Dictionary<StatResource, StatModifierData>(32);

            for (int i = 0; i < modifiers.Length; i++)
            {
                if ((AccumulatedStats?.ContainsKey(modifiers[i].Stat) ?? false ) || (AccumulatedStatsBroadcast?.ContainsKey(modifiers[i].Stat) ?? false))
                {
                    if (!statsForUpdating.ContainsKey(modifiers[i].Stat))
                    {
                        statsForUpdating.Add(modifiers[i].Stat, modifiers[i]);
                    }
                }
            }

            if (statsForUpdating.Count == 0)
                return await RemoveModifiersImpl(causer);

            foreach (var accumStat in AccumulatedStats)
            {
                if (await accumStat.Value.IsGetAffectedBy(causer) 
                    && !statsForUpdating.ContainsKey(accumStat.Key) 
                    && !statsForRemoveCauser.Contains(accumStat.Key))
                {
                    statsForRemoveCauser.Add(accumStat.Key);
                }
            }

            foreach (var accumBroadStat in AccumulatedStatsBroadcast)
            {
                if (await accumBroadStat.Value.IsGetAffectedBy(causer)
                    && !statsForUpdating.ContainsKey(accumBroadStat.Key) 
                    && !statsForRemoveCauser.Contains(accumBroadStat.Key))
                {
                    statsForRemoveCauser.Add(accumBroadStat.Key);
                }
            }
            await RemoveModifiersImpl(statsForRemoveCauser.ToArray(), causer);
            await SetModifiersImpl(statsForUpdating.Values.ToArray(), causer);

            return true;
        }


        public async ValueTask<bool> RemoveModifiersImpl(ModifierCauser causer)
        {
            if (StatsDef?.DoNotWorkAtAll ?? true)
                return true;

            await RemoveModifiers(AccumulatedStats, causer);
            await RemoveModifiers(AccumulatedStatsBroadcast, causer);
            return true;
        }

        private async ValueTask RemoveModifiers(IDeltaDictionary<StatResource, IAccumulatedStat> statCollection, ModifierCauser  causer)
        {
            if (statCollection != null)
            {
                foreach (var statPair in statCollection)
                {
                    if (await statPair.Value.RemoveModifiers(causer))
                    {
                        await RecalculateDependencyStats(statPair.Key, false, true);
                    }
                }
            }
        }

        public async ValueTask<bool> RemoveModifiersImpl(StatModifierInfo[] stats, ModifierCauser causer)
        {
            //Logger.IfError()?.Message($"{nameof(RemoveModifiersImpl)}({string.Join(" | ", stats.Select(v => v.____GetDebugShortName()))}, {causer})").Write();

            if (StatsDef?.DoNotWorkAtAll ?? true)
                return true;

            foreach (var statModifier in stats)
            {
                IAccumulatedStat accumulatedStat = default;
                if ((AccumulatedStats?.TryGetValue(statModifier.Stat, out accumulatedStat) ?? false) || (AccumulatedStatsBroadcast?.TryGetValue(statModifier.Stat, out accumulatedStat) ?? false))
                {
                    if (await accumulatedStat.RemoveModifier(causer, statModifier.ModifierType))
                        await RecalculateDependencyStats(statModifier.Stat, false, true);
                }
            }

            //Logger.IfInfo()?.Message($"Can't find AccumulatedStats = \"{statResource.____GetDebugShortName()}\" on entity with type = {EntitiesRepositoryBase.GetTypeById(_entityHolder.TypeId).Name}").Write();
            return true;
        }

        public async ValueTask<bool> RemoveModifiersImpl(StatResource[] stats, ModifierCauser causer)
        {
            //Logger.IfError()?.Message($"{nameof(RemoveModifiersImpl)}({string.Join(" | ", stats.Select(v => v.____GetDebugShortName()))}, {causer})").Write();

            if (StatsDef?.DoNotWorkAtAll ?? true)
                return true;

            foreach (var statDef in stats)
            {
                IAccumulatedStat accumulatedStat = default;
                if ((AccumulatedStats?.TryGetValue(statDef, out accumulatedStat) ?? false) || (AccumulatedStatsBroadcast?.TryGetValue(statDef, out accumulatedStat) ?? false))
                {
                    if (await accumulatedStat.RemoveModifiers(causer))
                        await RecalculateDependencyStats(statDef, false, true);
                }
            }

            //Logger.IfInfo()?.Message($"Can't find AccumulatedStats = \"{statResource.____GetDebugShortName()}\" on entity with type = {EntitiesRepositoryBase.GetTypeById(_entityHolder.TypeId).Name}").Write();
            return true;
        }

        public ValueTask<IStat> GetStatImpl(StatResource statResource)
        {
            return new ValueTask<IStat>(GetStatByResourceInternal(statResource, false));
        }

        public ValueTask<IStat> GetBroadcastStatImpl(StatResource statResource)
        {
            return new ValueTask<IStat>(GetStatByResourceInternal(statResource, true));
        }

        private IStat GetStatByResourceInternal(StatResource statResource, bool broadcastOnly = false)
        {
            if (statResource == null)
                return null;

            if (!broadcastOnly && AccumulatedStats != null && AccumulatedStats.TryGetValue(statResource, out IAccumulatedStat refStat))
                return refStat;

            if (AccumulatedStatsBroadcast.TryGetValue(statResource, out IAccumulatedStat refStat1))
                return refStat1;

            if (!broadcastOnly && TimeStats != null && TimeStats.TryGetValue(statResource, out ITimeStat refStat2))
                return refStat2;

            if (TimeStatsBroadcast != null && TimeStatsBroadcast.TryGetValue(statResource, out ITimeStat refStat3))
                return refStat3;

            if (!broadcastOnly && ProceduralStats != null && ProceduralStats.TryGetValue(statResource, out IProceduralStat refStat4))
                return refStat4;

            if (ProceduralStatsBroadcast != null && ProceduralStatsBroadcast.TryGetValue(statResource, out IProceduralStat refStat5))
                return refStat5;

            if (!broadcastOnly && ProxyStats != null && ProxyStats.TryGetValue(statResource, out IProxyStat refStat6))
                return refStat6;

            if (!broadcastOnly && ValueStats != null && ValueStats.TryGetValue(statResource, out IValueStat refStat7))
                return refStat7;

            if (ValueStatsBroadcast != null && ValueStatsBroadcast.TryGetValue(statResource, out IValueStat refStat9))
                return refStat9;

            return null;
        }

        public async ValueTask<(bool, float)> TryGetValueImpl(StatResource statResource)
        {
            var stat = GetStatByResourceInternal(statResource);
            return stat == null ? (false, 0) : (true, await stat.GetValue());
        }

        private async ValueTask RecalculateDependencyStats(StatResource statResource, bool calcersOnly, bool forceRecalculate)
        {
            IStat stat = (!_dependencyRestrictionList?.Contains(statResource) ?? true) ? GetStatByResourceInternal(statResource) : null;
            if (forceRecalculate ||
                (stat != null && await stat.RecalculateCaches(calcersOnly)))
            {
                if (_dependencyTree.TryGetValue(statResource, out List<StatResource> dependencies))
                {
                    foreach (var dependency in dependencies)
                    {
                        await RecalculateDependencyStats(dependency, calcersOnly, false);
                    }
                }
            }
        }

        public async ValueTask CopyImpl(IStatsEngine statsEngine)
        {
            await CopyValueStats(ValueStats, statsEngine.ValueStats);
            await CopyValueStats(ValueStatsBroadcast, statsEngine.ValueStatsBroadcast);

            await CopyAccumulatedStats(statsEngine.AccumulatedStats);
            await CopyAccumulatedStats(statsEngine.AccumulatedStatsBroadcast);

            await CopyTimeStats(statsEngine.TimeStats);
            await CopyTimeStats(statsEngine.TimeStatsBroadcast);
        }

        private async ValueTask CopyAccumulatedStats(IDeltaDictionary<StatResource, IAccumulatedStat> accumulatedStats)
        {
            foreach (var accumulatedStat in accumulatedStats)
            {
                if (await GetStat(accumulatedStat.Key) is IAccumulatedStat myStat)
                    foreach (var modifierType in accumulatedStat.Value.Modifiers)
                        foreach (var modifier in modifierType.Value.Modifiers)
                            await myStat.AddModifier(modifier.Key, modifierType.Key, modifier.Value);

                //Logger.IfError()?.Message($"myStat ({myStat.StatResource.StatName}) = {await myStat.GetValue()}").Write();
            }
        }

        private async ValueTask CopyValueStats(IDeltaDictionary<StatResource, IValueStat> myValueStats, IDeltaDictionary<StatResource, IValueStat> valueStats)
        {
            foreach (var valueStat in valueStats)
            {
                var myStat = await GetStat(valueStat.Key) as IValueStat;                
                if (myStat == null)
                {
                    myStat = new ValueStat();
                    await myStat.Copy(valueStat.Value);
                    myValueStats.Add(valueStat.Key, myStat);
                }

                var statValue = await valueStat.Value.GetValue();
                var myStatNewValue = await myStat.GetValue();
                await myStat.ChangeValue(statValue - myStatNewValue);

                //Logger.IfError()?.Message($"myStat ({myStat.StatResource.StatName}) = {await myStat.GetValue()}").Write();
            }
        }

        private async ValueTask CopyTimeStats(IDeltaDictionary<StatResource, ITimeStat> timeStats)
        {
            foreach (var timeStat in timeStats)
            {
                var myStat = await GetStat(timeStat.Key) as ITimeStat;
                if (myStat != null)
                {
                    var statValue = await timeStat.Value.GetValue();
                    var myStatNewValue = await myStat.GetValue();
                    await myStat.ChangeValue(statValue - myStatNewValue);
                    //Logger.IfError()?.Message($"myStat ({myStat.StatResource.StatName}) = {await myStat.GetValue()}, statValue = {statValue}, myStatNewValue = {myStatNewValue}").Write();
                }
            }
        }

        public async ValueTask RecalculateStatsImpl()
        {
            if (TimeWhenIdleStarted != 0 || StatsDef.DoNotWorkAtAll)
                return;

            foreach (var calcer in _nontrackableCalcers)
                foreach (var stat in calcer.Value)
                    await RecalculateDependencyStats(stat, true, false);
        }

        public async ValueTask InvokeOnStatsReparsedEventImpl()
        {
            if (parentEntity != null)
                await OnStatsReparsedEvent();
        }

        public async ValueTask AddProxyStatImpl(StatResource statResource, PropertyAddress propertyAddress)
        {
            if (ProxyStats == null)
                ProxyStats = new DeltaDictionary<StatResource, IProxyStat>();

            var proxyStat = new InventoryWeightProxyStat();
            if (!ProxyStats.ContainsKey(statResource))
                ProxyStats.Add(statResource, proxyStat);

            await proxyStat.ProxySubscribe(propertyAddress);
            await proxyStat.Initialize(null, true);
        }

        public async ValueTask SetToDefaultImpl(bool hardReset)
        {
            InitStatsDef();
            UpdateDependancyTree(StatsDef);

            if (StatsDef == null || StatsDef.Stats == null || StatsDef.DoNotWorkAtAll)
                return;

            foreach (var stat in StatsDef.Stats)
            {
                switch (stat.Target)
                {
                    case AccumulatedStatDef accumulatedStatDef:
                        {
                            await InitializeStat<IAccumulatedStat, AccumulatedStat>(accumulatedStatDef, AccumulatedStats, AccumulatedStatsBroadcast, hardReset);
                            break;
                        }
                    case TimeStatDef timeStatDef:
                        {
                            await InitializeStat<ITimeStat, TimeStat>(timeStatDef, TimeStats, TimeStatsBroadcast, hardReset);
                            break;
                        }
                    case ProceduralStatDef proceduralStatDef:
                        {
                            await InitializeStat<IProceduralStat, ProceduralStat>(proceduralStatDef, ProceduralStats, ProceduralStatsBroadcast, hardReset);
                            break;
                        }
                    case ValueStatDef valueStatDef:
                        {
                            await InitializeStat<IValueStat, ValueStat>(valueStatDef, ValueStats, ValueStatsBroadcast, hardReset);
                            break;
                        }
                    default:
                        break;
                }
            }

            await InvokeOnStatsReparsedEvent();
        }

        private void InitStatsDef()
        {
            switch (parentDeltaObject)
            {
                case IItem item:
                    {
                        StatsDef = (item.ItemResource as IHasStatsDef)?.Stats;
                        break;
                    }
                case IEntityObject entityObject:
                    {
                        StatsDef = (entityObject.Def as IHasStatsDef)?.Stats;
                        break;
                    }
                default:
                    break;
            }
        }

        private ValueTask InitializeStat<StatInterface, StatType>(StatDef statDef, IDeltaDictionary<StatResource, StatInterface> stats, IDeltaDictionary<StatResource, StatInterface> statsBroadcast, bool hardReset)
            where StatInterface : IStat
            where StatType : StatInterface, new()
        {
            var statResourceDef = statDef.StatResource.Target;
            var resetState = false;
            if (!stats.TryGetValue(statResourceDef, out StatInterface valueStat) && !statsBroadcast.TryGetValue(statResourceDef, out valueStat))
            {
                valueStat = new StatType();
                if (statDef.IsBroadcasted)
                    statsBroadcast.Add(statResourceDef, valueStat);
                else
                    stats.Add(statResourceDef, valueStat);

                resetState = true;
            }

            return valueStat.Initialize(statDef, hardReset || resetState);
        }

        private void UpdateDependancyTree(StatsDef StatsDef)
        {
            _dependencyTree.Clear();
            _nontrackableCalcers.Clear();

            if (StatsDef?.Stats == null)
                return;

            foreach (var statDef in StatsDef.Stats)
            {
                UpdateDependency(statDef.Target.LimitMinStat, statDef.Target.StatResource);
                UpdateDependency(statDef.Target.LimitMaxStat, statDef.Target.StatResource);

                switch (statDef.Target)
                {
                    case AccumulatedStatDef accumulatedStatDef:
                        {
                            break;
                        }
                    case TimeStatDef timeStatDef:
                        {
                            UpdateDependency(timeStatDef.ChangeRateStat, timeStatDef.StatResource);
                            foreach (var notifier in timeStatDef.ChangeRateCalcer.Target.CollectStatNotifiers())
                            {
                                UpdateDependency(notifier, timeStatDef.StatResource);
                                UpdateNontrackableCalcers(timeStatDef.ChangeRateCalcer.Target, notifier, timeStatDef.StatResource);
                            }

                            break;
                        }
                    case ProceduralStatDef proceduralStatDef:
                        {
                            foreach (var notifier in proceduralStatDef.ValueCalcer.Target.CollectStatNotifiers())
                            {
                                UpdateDependency(notifier, proceduralStatDef.StatResource);
                                UpdateNontrackableCalcers(proceduralStatDef.ValueCalcer.Target, notifier, proceduralStatDef.StatResource);
                            }

                            break;
                        }

                    default:
                        break;
                }
            }

            CheckStatsDependencies(StatsDef);
        }

        private void UpdateDependency(StatResource from, StatResource to)
        {
            if (from == null)
                return;

            if (_dependencyTree.TryGetValue(from, out List<StatResource> dependencies))
            {
                if (!dependencies.Contains(to))
                    dependencies.Add(to);
            }
            else
            {
                _dependencyTree.Add(from, new List<StatResource>() { to });
            }
        }

        private void UpdateNontrackableCalcers(CalcerDef calcer, StatResource notifierStat, StatResource dependentStat)
        {
            if (notifierStat == null)
            {
                if (_nontrackableCalcers.TryGetValue(calcer, out List<StatResource> dependencies))
                {
                    if (!dependencies.Contains(dependentStat))
                        dependencies.Add(dependentStat);
                }
                else
                {
                    _nontrackableCalcers.Add(calcer, new List<StatResource>() { dependentStat });
                }
            }
        }

        private void CheckStatsDependencies(StatsDef StatsDef)
        {
            bool hasErrors = false;

            var timeStatNotifiers = _dependencyTree.Where(v => StatsDef.Stats.Single(s => s.Target.StatResource == v.Key).Target is TimeStatDef);
            if (timeStatNotifiers.Any())
            {
                hasErrors = true;
                Logger.IfError()?.Message($"Error in {StatsDef.____GetDebugRootName()}: We have TimeStat as notifier stat: {StatDependencyToString(timeStatNotifiers)}").Write();
            }

            var dependencyLevels = _dependencyTree.Select(v => (v.Key, DependencyLevel(v.Key).Max())).Where(v => v.Item2 > MAX_DEPENDENCY_LEVEL);
            if (dependencyLevels.Any())
            {
                hasErrors = true;
                _dependencyRestrictionList = dependencyLevels.Select(v => v.Key).ToList();
                Logger.IfError()?.Message($"Error in {StatsDef.____GetDebugRootName()}: We have > {MAX_DEPENDENCY_LEVEL} dependency level. Root stats: \n{string.Join("\n", dependencyLevels.Select(v => v.Key.____GetDebugShortName() + ": " + v.Item2))}").Write();
            }

            if (hasErrors)
            {
                if (_dependencyTree.Any())
                    Logger.IfInfo()?.Message($"Stat Dependency Tree ({StatsDef.____GetDebugShortName()})\n-----------------------\n{StatDependencyToString(_dependencyTree)}\n-----------------------").Write();

                if (_nontrackableCalcers.Any())
                    Logger.IfInfo()?.Message($"Non Trackable Calcers ({StatsDef.____GetDebugShortName()})\n-----------------------\n{StatDependencyToString(_nontrackableCalcers)}\n-----------------------").Write();
            }
        }

        private IEnumerable<int> DependencyLevel(StatResource notifier, int counter = 0)
        {
            if (notifier == null)
                yield break;

            if (_dependencyTree.TryGetValue(notifier, out List<StatResource> dependencies))
            {
                counter++;
                foreach (var dependency in dependencies)
                {
                    yield return DependencyLevel(dependency, counter).Max();
                }
            }

            yield return counter;
        }

        private static string StatDependencyToString<T, K>(IEnumerable<KeyValuePair<T, List<K>>> dependencies)
            where T : BaseResource
            where K : BaseResource
        {
            return string.Join("\n", dependencies.Select(v => $"{v.Key.____GetDebugShortName(),40}" + " -> " + string.Join("; ", v.Value.Select(x => x.____GetDebugShortName()))));
        }

        public async ValueTask<List<StatModifierProto>> GetSnapshotImpl(StatType statType)
        {
            var statsSnapshot = new List<StatModifierProto>();
            if (TimeStats?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(TimeStats, statType));
            if (TimeStatsBroadcast?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(TimeStatsBroadcast, statType));
            if (ValueStats?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(ValueStats, statType));
            if (ValueStatsBroadcast?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(ValueStatsBroadcast, statType));
            if (ProxyStats?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(ProxyStats, statType));
            if (ProceduralStats?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(ProceduralStats, statType));
            if (AccumulatedStats?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(AccumulatedStats, statType));
            if (ProceduralStatsBroadcast?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(ProceduralStatsBroadcast, statType));
            if (AccumulatedStatsBroadcast?.Any() ?? false)
                statsSnapshot.AddRange(await GetSnapshotInternal(AccumulatedStatsBroadcast, statType));

            return statsSnapshot;
        }

        private async ValueTask<List<StatModifierProto>> GetSnapshotInternal<T>(IDeltaDictionary<StatResource, T> stats, StatType statType)
            where T : IStat
        {
            var statModifiers = new List<StatModifierProto>();
            foreach (var stat in stats)
            {
                if (statType == stat.Value.StatType)
                {
                    statModifiers.Add(
                    new StatModifierProto()
                    {
                        Stat = stat.Key,
                        Value = await stat.Value.GetValue()
                    });
                }
            }

            return statModifiers;
        }

        public ValueTask<string> DumpStatsLocalImpl(bool compactString)
        {
            return DumpStatsImpl(compactString);
        }

        public ValueTask<string> DumpStatsImpl(bool compactString)
        {
            var sb = new StringBuilder();

            LogStat(TimeStats, nameof(TimeStats), sb, compactString);
            LogStat(TimeStatsBroadcast, nameof(TimeStatsBroadcast), sb, compactString);
            LogStat(ValueStats, nameof(ValueStats), sb, compactString);
            LogStat(ValueStatsBroadcast, nameof(ValueStatsBroadcast), sb, compactString);
            LogStat(AccumulatedStats, nameof(AccumulatedStats), sb, compactString);
            LogStat(AccumulatedStatsBroadcast, nameof(AccumulatedStatsBroadcast), sb, compactString);
            LogStat(ProxyStats, nameof(ProxyStats), sb, compactString);
            LogStat(ProceduralStats, nameof(ProceduralStats), sb, compactString);
            LogStat(ProceduralStatsBroadcast, nameof(ProceduralStatsBroadcast), sb, compactString);

            return new ValueTask<string>(sb.ToString());
        }

        private static void LogStat<T>(IDeltaDictionary<StatResource, T> stats, string name, StringBuilder sb, bool compactString)
        {
            if (stats.Any())
            {
                if (!compactString)
                    sb.AppendLine("<------------ " + name + " ------------>");

                foreach (var stat in stats)
                {
                    sb.Append($"{stat.Key?.____GetDebugShortName() ?? "null",-35}: {stat.Value.ToString()}{(compactString ? "; " : "\n")}");
                }
            }
        }

        public async Task GoIntoIdleModeImpl()
        {
            TimeWhenIdleStarted = SyncTime.NowUnsynced;
            foreach (var timeStat in TimeStats)
            {
                await timeStat.Value.ChangeValue(0);
            }
            foreach (var timeStatBroadcast in TimeStatsBroadcast)
            {
                await timeStatBroadcast.Value.ChangeValue(0);
            }

            await StopStats();
        }
        public async Task GetBackFromIdleModeImpl()
        {
            foreach (var timeStat in TimeStats)
            {
                await timeStat.Value.ChangeValue(0);
            }
            foreach (var timeStatBroadcast in TimeStatsBroadcast)
            {
                await timeStatBroadcast.Value.ChangeValue(0);
            }
            TimeWhenIdleStarted = 0;

            CheckAndStartRecalculation();
        }

        private Task StopStats()
        {
            if (_cts != null)
                _cts.Cancel();
            _cts = null;

            return Task.CompletedTask;
        }

        private Task Deinitialization()
        {
            if (parentEntity is IWorldCharacter wc)
            {
                wc.OnIdleModeStarted -= GoIntoIdleMode;
                wc.OnIdleModeStopped -= GetBackFromIdleMode;
            }
            return StopStats();
        }

        public Task OnDestroy() => Deinitialization();
        public Task OnUnload() => Deinitialization();

        private async Task OnDie(Guid guid, int typeId, PositionRotation corpsePlace)
        {
            await StopStats();
        }

        private void CheckAndStartRecalculation()
        {
            if (_nontrackableCalcers.Any())
            {
                if (_cts != null)
                {
                    //Logger.IfError()?.Message("Stats recalculation cancellation token for obj {0} with id {1} is not null", EntitiesRepositoryBase.GetTypeById(parentEntity.TypeId), parentEntity.Id).Write();
                    //_cts.Cancel();
                    return;
                }
                _cts = new CancellationTokenSource();

                parentEntity.AssertIfNull(nameof(parentEntity)); //Ловлю редкий nullref при resurrect'е персонажа
                StatsDef.AssertIfNull(nameof(StatsDef));         //Ловлю редкий nullref при resurrect'е персонажа

                RunChainLight(parentEntity.TypeId, parentEntity.Id, () => RecalculateStats().AsTask(), TimeSpan.FromSeconds(StatsDef.MeanTimeToCheckCalcers), EntitiesRepository, _cts.Token);
            }
        }

        public static void RunChainLight(int typeId, Guid entityId, Func<Task> code, TimeSpan interval, IEntitiesRepository repo, CancellationToken token, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            AsyncUtils.RunAsyncTask(() => RunChainLightImpl(typeId, entityId, code, interval, repo, token, filePath, lineNumber), repo);
        }

        private static async Task RunChainLightImpl(int typeId, Guid entityId, Func<Task> code, TimeSpan interval, IEntitiesRepository repo, CancellationToken token, string filePath, int lineNumber)
        {
            try
            {
                while (true)
                {
                    await Task.Delay(interval, token);
                    using (var wrap = await repo.Get(typeId, entityId))
                    {
                        if (wrap.Get<IEntity>(typeId, entityId, ReplicationLevel.Master) == null)
                        {
                            if (!token.IsCancellationRequested)
                                Logger.IfError()?.Message("Entity {0} with id {1} was not found", ReplicaTypeRegistry.GetTypeById(typeId), entityId).Write();

                            return;
                        }
                        if (token.IsCancellationRequested)
                            return;

                        await code();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception in lightweight chaincall, restarting").Write();;
                RunChainLight(typeId, entityId, code, interval, repo, token, filePath, lineNumber);
            }
        }
    }
}
