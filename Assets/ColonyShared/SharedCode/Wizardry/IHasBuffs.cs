using Assets.ColonyShared.SharedCode.Wizardry;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Wizardry;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using NLog;
using ResourceSystem.Aspects.Templates;
using ResourceSystem.GeneratedDefsForSpells;
using Scripting;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Modifiers;
using Core.Environment.Logging.Extension;
using Nest;
using ResourceSystem.Aspects;

namespace Assets.ColonyShared.SharedCode.Wizardry
{
    public interface IHasBuffs
    {
        IBuffs Buffs { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface IBuffs : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        SpellId Counter { get; set; }
        IDeltaDictionary<SpellId, IBuff> All { get; set; }
        Task<SpellId> TryAddBuff(ScriptingContext cast, BuffDef buffDef);
        Task<bool> RemoveBuff(SpellId buffId);
        Task<bool> RemoveBuff(BuffDef buffDef);
    }


    [GenerateDeltaObjectCode]
    public interface IBuff : IDeltaObject
    {
        BuffDef Def { get; set; }
        ScriptingContext Context { get; set; }
        SpellId Id { get; set; }
        long StartTime { get; set; }
        long EndTime { get; set; }
        long Duration { get; set; }
        bool Started { get; set; }
        bool Finished { get; set; }
        bool IsInfinite { get; set; }
    }

}

namespace GeneratedCode.DeltaObjects
{
    public partial class Buffs : IHookOnReplicationLevelChanged, IHookOnUnload, IHookOnDestroy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        Dictionary<SpellId, CancellationTokenSource> _currentlyActivatedBuffs => _currentlyActivatedBuffsBack == null ?
            _currentlyActivatedBuffsBack = new Dictionary<SpellId, CancellationTokenSource>() : _currentlyActivatedBuffsBack;

        Dictionary<SpellId, CancellationTokenSource> _currentlyActivatedBuffsBack;
        public async Task<SpellId> TryAddBuffImpl(ScriptingContext cast, BuffDef def)
        {
            var spellId = new SpellId(Counter.Counter + 1);
            Counter = spellId;
            var currentTime = SyncTime.Now;
            var buff = new Buff() { Def = def, Id = spellId, Context = cast, StartTime = currentTime, Duration = SyncTime.FromSeconds(def.Duration), IsInfinite = def.IsInfinite};
            All.Add(spellId, buff);
            await StartBuff(buff, currentTime, true);
            return spellId;
        }
        private async Task StartBuff(IBuff buff, long currentTime, bool firstTimeStarted)
        {
            var s = new CancellationTokenSource();
            if (_currentlyActivatedBuffs.TryAdd(buff.Id, s))
            {
                var castData = new SpellWordCastData(
                    buff.Id,
                    0,
                    new SpellCast {Context = buff.Context, Def = null, StartAt = buff.StartTime},
                    currentTime,
                    buff.StartTime,
                    buff.StartTime,
                    TimeRange.FromDuration(buff.StartTime, !buff.IsInfinite ? buff.Duration : TimeUnitsHelpers.Infinite),
                    new OuterRef<IWizardEntity>(ParentEntityId, WizardEntity.StaticTypeId),
                    new OuterRef<IEntity>(ParentEntityId, ParentTypeId),
                    mark,
                    firstOrLast: firstTimeStarted,
                    canceled: false,
                    modifiers: null, /*buff.Modifiers,*/
                    context: buff.Context,
                    repo:EntitiesRepository);
                Logger.IfDebug()?.Message(ParentEntityId, $"StartBuff | Buff:{buff.Def?.____GetDebugAddress()} Id:{buff.Id} TimeRange:{castData.WordTimeRange} Time:{currentTime}").Write();
                foreach (var effect in buff.Def.Effects)
                    if (CanRunOnSite(effect))
                        await SpellEffects.StartEffect(castData, effect, EntitiesRepository);
                if (((IEntityExt)parentEntity).IsMaster() && firstTimeStarted)
                    buff.Started = true;
                if (!buff.IsInfinite && ((IEntityExt)parentEntity).IsMaster())
                {
                    var bid = buff.Id;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        var dt = castData.WordTimeRange.Finish - SyncTime.Now;
                        Logger.IfDebug()?.Message(ParentEntityId, $"Schedule StopBuff after {dt}ms", castData).Write();
                        if (dt > 0)
                            await Task.Delay(TimeSpan.FromSeconds(SyncTime.ToSeconds(dt)), s.Token);
                        using (await parentEntity.GetThisRead())
                            await RemoveBuff(bid);
                    }, EntitiesRepository);
                }
            }
            else
                s.Dispose();
        }

        public async Task<bool> RemoveBuffImpl(SpellId id)
        {
            if (!All.TryGetValue(id, out var buff) || buff == null)
                return true;
            if (!buff.Finished)
            {
                var currentTime = SyncTime.Now;
                buff.Finished = true;
                buff.EndTime = currentTime;
                if (buff.Started)
                    await EndBuff(buff, currentTime, true);
            }
            All.Remove(id);
            return true;
        }

        public async Task<bool> RemoveBuffImpl(BuffDef def)
        {
            bool any = false;
            var spells = All.Where(x => x.Value.Def == def).ToArray();
            foreach (var pair in spells)
            {
                await RemoveBuffImpl(pair.Key);
                any = true;
            }
            return any;
        }
        
        private async Task EndBuff(IBuff buff, long currentTime, bool isLast)
        {
            if (_currentlyActivatedBuffs.TryGetValue(buff.Id, out var s))
            {
                _currentlyActivatedBuffs.Remove(buff.Id);
                s.Cancel();
                var castData = new SpellWordCastData(
                    buff.Id,
                    0,
                    new SpellCast() {Context = buff.Context, Def = null, StartAt = buff.StartTime,},
                    currentTime,
                    buff.StartTime,
                    buff.StartTime,
                    TimeRange.FromDuration(buff.StartTime, !buff.IsInfinite ? buff.Duration : TimeUnitsHelpers.Infinite),
                    new OuterRef<IWizardEntity>(ParentEntityId, WizardEntity.StaticTypeId),
                    new OuterRef<IEntity>(ParentEntityId, ParentTypeId),
                    mark,
                    firstOrLast: isLast,
                    canceled: false,
                    context: buff.Context,
                    modifiers: null /*buff.Modifiers*/,
                    repo:EntitiesRepository);
                Logger.IfDebug()?.Message(ParentEntityId, $"StopBuff | Buff:{buff.Def?.____GetDebugAddress()} Id:{buff.Id} TimeRange:{castData.WordTimeRange} Time:{currentTime}").Write();
                foreach (var effect in buff.Def.Effects)
                    if (CanRunOnSite(effect))
                        await SpellEffects.EndEffect(castData, effect, EntitiesRepository);
            }
        }

        private bool CanRunOnSite(SpellEffectDef effect)
        {
            return mark.OnClient() || !SpellEffects.IsClientOnlyEffect(effect);
        }
        
        UnityEnvironmentMark mark;


        ReplicaOrNotStateMachine ronsm = default;
        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if (ronsm == null)
                ronsm = new ReplicaOrNotStateMachine(new OuterRef<IEntity>(parentEntity), FromReplica, ToReplica, FromMaster, ToMaster);
            ronsm.OnReplicationLevelChanged(EntitiesRepository, oldReplicationMask, newReplicationMask);
        }


        async Task RestartKillAllBuffs()
        {
            var now = SyncTime.Now;
            foreach (var buff in All)
                await EndBuff(buff.Value, now, false);
        }

        async Task RestartStartAllBuffs()
        {
            var now = SyncTime.Now;
            foreach (var buff in All)
                await StartBuff(buff.Value, now, false);

        }
        async Task FromReplica()
        {
            await RestartKillAllBuffs();
            mark = null;
            All.OnItemAddedOrUpdated -= OnNewBuffOnReplica;
            All.OnItemRemoved -= OnBuffRemovedOnReplica;
        }


        async Task ToReplica()
        {
            mark = new UnityEnvironmentMark(EntitiesRepository.CloudNodeType == SharedCode.Cloud.CloudNodeType.Client ? UnityEnvironmentMark.ServerOrClient.Client : UnityEnvironmentMark.ServerOrClient.Server);
            All.OnItemAddedOrUpdated += OnNewBuffOnReplica;
            All.OnItemRemoved += OnBuffRemovedOnReplica;
            await RestartStartAllBuffs();
        }

        private async Task OnBuffRemovedOnReplica(DeltaDictionaryChangedEventArgs<SpellId, IBuff> args)
        {
            using (var self = await EntitiesRepository.Get(new OuterRef<IEntity>(parentEntity)))
                await EndBuff(args.Value, args.Value.EndTime, false);

        }

        private async Task OnNewBuffOnReplica(DeltaDictionaryChangedEventArgs<SpellId, IBuff> args)
        {
            using (var self = await EntitiesRepository.Get(new OuterRef<IEntity>(parentEntity)))
                await StartBuff(args.Value, args.Value.StartTime, false);
        }

        async Task ToMaster()
        {
            await RestartStartAllBuffs();
        }

        async Task FromMaster()
        {
            await RestartKillAllBuffs();
        }

        public async Task OnUnload()
        {
            using (var w = await parentEntity.GetThisWrite())
                await RestartKillAllBuffs();
        }

        public async Task OnDestroy()
        {
            using (var w = await parentEntity.GetThisWrite())
                foreach (var buff in All.ToList())
                    await RemoveBuffImpl(buff.Key);
        }
    }

    class ReplicaOrNotStateMachine
    {
        public Func<Task> FromReplica;
        public Func<Task> ToReplica;
        public Func<Task> FromMaster;
        public Func<Task> ToMaster;
        OuterRef<IEntity> _self;
        public ReplicaOrNotStateMachine(OuterRef<IEntity> self, Func<Task> fromReplica, Func<Task> toReplica, Func<Task> fromMaster, Func<Task> toMaster)
        {
            _self = self;
            FromReplica = fromReplica;
            ToReplica = toReplica;
            FromMaster = fromMaster;
            ToMaster = toMaster;
        }
        public void OnReplicationLevelChanged(IEntitiesRepository repo, long oldReplicationMask, long newReplicationMask)
        {
            var oldRepLevel = (ReplicationLevel)oldReplicationMask;
            var newRepLevel = (ReplicationLevel)newReplicationMask; AsyncUtils.RunAsyncTask(async () =>
            {
                using (var selfW = await repo.Get(_self))
                {
                    if (newRepLevel != ReplicationLevel.Master && oldRepLevel == ReplicationLevel.Master && newRepLevel != 0) // if it's zero - there will be unload or destroy
                    {
                        await FromMaster();
                    }
                    if ((newRepLevel == ReplicationLevel.Master || newRepLevel == ReplicationLevel.None) && (oldRepLevel != ReplicationLevel.Master && oldRepLevel != 0))
                    {
                        await FromReplica();
                    }
                    if (newRepLevel != ReplicationLevel.Master && newRepLevel != ReplicationLevel.None)
                    {
                        await ToReplica();
                    }
                    if (newRepLevel == ReplicationLevel.Master)
                    {
                        await ToMaster();
                    }

                }
            }, repo);
        }
    }


    public class PredicateHasBuff : IPredicateBinding<PredicateHasBuffDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateHasBuffDef def)
        {
            var selfDef = ((PredicateHasBuffDef)def);
            var targetRef = await selfDef.Target.Target.GetOuterRef(cast, repo);
            using (var targetW = await repo.Get(targetRef))
            {
                var target = targetW.Get<IHasBuffs>(targetRef, ReplicationLevel.Server);
                return target?.Buffs.All.Any(x => x.Value.Def == selfDef.Buff.Target) ?? false;
            }
        }
    }
    public class ImpactCastBuff : IImpactBinding<ImpactCastBuffDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactCastBuffDef def)
        {
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            using (var targetW = await repo.Get(targetRef))
            {
                var target = targetW.Get<IHasBuffsServer>(targetRef, ReplicationLevel.Server);
                if (target == null)
                {
                    Logger.IfError()?.Message($"There is no target to cast buff {def.____GetDebugAddress()}").Write();
                    return;
                }
                await target.Buffs.TryAddBuff(await def.Context.Target.CalcFromDef(cast.Caster, cast.Context, repo), def.Buff);
            }
        }
    }
    public class ImpactRemoveBuff : IImpactBinding<ImpactRemoveBuffDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRemoveBuffDef def)
        {
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            using (var targetW = await repo.Get(targetRef))
            {
                var target = targetW.Get<IHasBuffsServer>(targetRef, ReplicationLevel.Server);
                foreach (var buff in target.Buffs.All.ToList())
                    if (buff.Value.Def == def.Buff)
                        await target.Buffs.RemoveBuff(buff.Key);
            }
        }
    }

    public class CalcerFloat : ICalcerBinding<FloatCalcerDef, float>
    {
        public async ValueTask<float> Calc(FloatCalcerDef def, CalcerContext ctx)
        {
            var value = await def.Calcer.Target.CalcAsync(ctx);
            if (value.ValueType == ColonyShared.SharedCode.Value.Type.Float)
                return value.AsFloat();
            else
                return 0f;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(FloatCalcerDef def)
        {
            return Enumerable.Empty<StatResource>();
        }
    }

    public class ContextArgCalcer : ICalcerBinding<ContextArgCalcerDef, Value>
    {
        public async ValueTask<Value> Calc(ContextArgCalcerDef def, CalcerContext ctx)
        {
            if (ctx.Ctx == null || ctx.Ctx.TypedArgs == null)
                return default;
            if (ctx.Ctx.TypedArgs.TryGetValue(DefToType.GetNetIdForType(def.Arg.Target.GetType()), out var val))
                return val;
            return default;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(ContextArgCalcerDef def)
        {
            return Enumerable.Empty<StatResource>();
        }
    }

    public class CustomContextArgCalcer : ICalcerBinding<CustomContextArgCalcerDef, Value>
    {
        public ValueTask<Value> Calc(CustomContextArgCalcerDef def, CalcerContext ctx)
        {
            if (ctx.Ctx == null || ctx.Ctx.CustomArgs == null)
                return new ValueTask<Value>(default(Value));
            if (ctx.Ctx.CustomArgs.TryGetValue(def.Arg.Target, out var val))
                return new ValueTask<Value>(val);
            return new ValueTask<Value>(default(Value));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CustomContextArgCalcerDef def)
        {
            return Enumerable.Empty<StatResource>();
        }
    }
}
