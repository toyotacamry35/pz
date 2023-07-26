using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Aspects.Impl.Effects
{
    public class EffectChangeAccumulatedStat : IEffectBinding<EffectChangeAccumulatedStatDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("EffectChangeAccumulatedStat");

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repository, EffectChangeAccumulatedStatDef def)
        {
            if (cast.IsSlave)
                return;
            var selfDef = (EffectChangeAccumulatedStatDef)def;
            var StatNameDef = selfDef.StatName;
            ModifierCauser causer = new ModifierCauser() { Causer = selfDef, SpellId = cast.SpellId.Counter };
            Logger.IfDebug()?.Message("EffectChangeAccumulatedStat.Attach: {5} Stat: {0} Causer: {6} Mul: {1:0.00} Sum: {2:0.00} Min: {3:0.00} Max: {4:0.00}", StatNameDef, selfDef.Multiplier, selfDef.Summand, selfDef.UseClampMin ? selfDef.ClampMin : float.NaN, selfDef.UseClampMax ? selfDef.ClampMax : float.NaN, selfDef, causer).Write();
            OuterRef<IEntity> ent = cast.Caster;
            if(selfDef.Target.Target != null)
                ent = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!ent.IsValid)
                ent = cast.Caster;

            if (ent.TypeId != 0)
                using (var wrapper = await repository.Get(ent.TypeId, ent.Guid, selfDef))
                {
                    var entity = wrapper.Get<IHasStatsEngineServer>(ent.TypeId, ent.Guid, ReplicationLevel.Server);
                    if (entity != null)
                    {
                        var modifiers = new List<StatModifierData>();
                        var summand = await selfDef.Summand.Target.CalcAsync(new CalcerContext(wrapper, cast.Caster, repository, cast, ctx: cast.Context));
                        if (summand != 0f)
                            modifiers.Add(new StatModifierData(StatNameDef, StatModifierType.Add, summand));
                        if (selfDef.Multiplier != 0f)
                            modifiers.Add(new StatModifierData(StatNameDef, StatModifierType.Mul, selfDef.Multiplier));
                        if (selfDef.UseClampMin)
                            modifiers.Add(new StatModifierData(StatNameDef, StatModifierType.ClampMin, selfDef.ClampMin));
                        if (selfDef.UseClampMax)
                            modifiers.Add(new StatModifierData(StatNameDef, StatModifierType.ClampMax, selfDef.ClampMax));
                        await entity.Stats.SetModifiers(modifiers.ToArray(), causer);
                    }
                }

            return;
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repository, EffectChangeAccumulatedStatDef def)
        {
            if (cast.IsSlave)
                return;

            var selfDef = (EffectChangeAccumulatedStatDef)def;
            var StatNameDef = selfDef.StatName;
            ModifierCauser causer = new ModifierCauser() { Causer = selfDef, SpellId = cast.SpellId.Counter };
            Logger.IfDebug()?.Message("EffectChangeAccumulatedStat.Detach: {1} Stat: {0} Causer: {2}", StatNameDef, selfDef, causer).Write();
            OuterRef<IEntity> ent = cast.Caster;
            if (selfDef.Target.Target != null)
                ent = await selfDef.Target.Target.GetOuterRef(cast, repository);
            if (!ent.IsValid)
                ent = cast.Caster;
            if (ent.TypeId != 0)
                using (var wrapper = await repository.Get(ent.TypeId, ent.Guid, selfDef))
                {
                    var entity = wrapper.Get<IHasStatsEngineServer>(ent.TypeId, ent.Guid, ReplicationLevel.Server);
                    if (entity != null)
                    {
                        var modifiers = new List<StatModifierInfo>();
                        var summand = await selfDef.Summand.Target.CalcAsync(new CalcerContext(wrapper, cast.Caster, repository, cast, ctx: cast.Context));
                        if (summand != 0f)
                            modifiers.Add(new StatModifierInfo(StatNameDef, StatModifierType.Add));
                        if (selfDef.Multiplier != 0f)
                            modifiers.Add(new StatModifierInfo(StatNameDef, StatModifierType.Mul));
                        if (selfDef.UseClampMin)
                            modifiers.Add(new StatModifierInfo(StatNameDef, StatModifierType.ClampMin));
                        if (selfDef.UseClampMax)
                            modifiers.Add(new StatModifierInfo(StatNameDef, StatModifierType.ClampMax));
                        await entity.Stats.RemoveModifiers(modifiers.ToArray(), causer);
                    }
                }
        }
    }
}
