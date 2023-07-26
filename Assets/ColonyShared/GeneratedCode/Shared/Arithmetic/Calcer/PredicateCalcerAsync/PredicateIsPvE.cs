using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.Aspects.Sessions;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync
{
    [UsedImplicitly]
    public class PredicateIsPvE : ICalcerBinding<PredicateIsPvEDef,bool>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async ValueTask<bool> Calc(PredicateIsPvEDef def, CalcerContext ctx)
        {
            var scenicEntity = ctx.EntityContainer.Get<IScenicEntityAlways>(ctx.EntityRef, ReplicationLevel.Always);
            var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
            using (var cnt = await ctx.Repository.Get(mapEntityRef))
            {
                var mapEntity = cnt.Get<IMapEntityAlways>(mapEntityRef, ReplicationLevel.Always);
                var rules = mapEntity.RealmRules;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapEntity:{mapEntity} RealmRules:{rules.____GetDebugAddress()}").Write();
                bool res = false;
                if (rules != null)
                    foreach (var rule in rules.Rules.Select(x => x.Target))
                        if (rule is RealmPlayersInteraction rpi && rpi.PlayersInteraction == PlayersInteraction.Friendly)
                        {
                            res = true;
                            break;
                        }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"isPvE:{res}").Write();
                return res;
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateIsPvEDef def) => Enumerable.Empty<StatResource>();
    }
    [UsedImplicitly]
    public class PredicateIsRealm : ICalcerBinding<PredicateIsRealmDef, bool>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async ValueTask<bool> Calc(PredicateIsRealmDef def, CalcerContext ctx)
        {
            var scenicEntity = ctx.EntityContainer.Get<IScenicEntityAlways>(ctx.EntityRef, ReplicationLevel.Always);
            var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
            using (var cnt = await ctx.Repository.Get(mapEntityRef))
            {
                var mapEntity = cnt.Get<IMapEntityAlways>(mapEntityRef, ReplicationLevel.Always);
                var rules = mapEntity.RealmRules;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapEntity:{mapEntity} RealmRules:{rules.____GetDebugAddress()}").Write();
                bool res = false;
                if (rules != null)
                {
                    if (def.RealmRules.Any(x => x.Target == rules))
                        res = true;
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"isPvE:{res}").Write();
                return res;
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateIsRealmDef def) => Enumerable.Empty<StatResource>();
    }
    [UsedImplicitly]
    public class PredicateIsHardcore : ICalcerBinding<PredicateIsHardcoreDef, bool>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async ValueTask<bool> Calc(PredicateIsHardcoreDef def, CalcerContext ctx)
        {
            var scenicEntity = ctx.EntityContainer.Get<IScenicEntityAlways>(ctx.EntityRef, ReplicationLevel.Always);
            var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
            using (var cnt = await ctx.Repository.Get(mapEntityRef))
            {
                var mapEntity = cnt.Get<IMapEntityAlways>(mapEntityRef, ReplicationLevel.Always);
                var rules = mapEntity.RealmRules;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapEntity:{mapEntity} RealmRules:{rules.____GetDebugAddress()}").Write();
                bool res = false;
                if (rules != null)
                {
                    foreach (var rule in rules.Rules.Select(x=>x.Target))
                        if (rule is RealmDeathLimit rpi && rpi.DeathLimit == DeathLimit.Single)
                        {
                            res = true;
                            break;
                        }
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"isPvE:{res}").Write();
                return res;
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateIsHardcoreDef def) => Enumerable.Empty<StatResource>();
    }
    [UsedImplicitly]
    public class PredicateCustomRealmRule : ICalcerBinding<PredicateCustomRealmRuleDef, bool>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async ValueTask<bool> Calc(PredicateCustomRealmRuleDef def, CalcerContext ctx)
        {
            var scenicEntity = ctx.EntityContainer.Get<IScenicEntityAlways>(ctx.EntityRef, ReplicationLevel.Always);
            var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
            using (var cnt = await ctx.Repository.Get(mapEntityRef))
            {
                var mapEntity = cnt.Get<IMapEntityAlways>(mapEntityRef, ReplicationLevel.Always);
                var rules = mapEntity.RealmRules;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapEntity:{mapEntity} RealmRules:{rules.____GetDebugAddress()}").Write();
                bool res = false;
                if (rules != null)
                {
                    res = rules.Values.ContainsKey(def.CustomRealmRule);
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"isPvE:{res}").Write();
                return res;
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateCustomRealmRuleDef def) => Enumerable.Empty<StatResource>();
    }
    [UsedImplicitly]
    public class CalcerCustomRealmRule : ICalcerBinding<CalcerCustomRealmRuleDef, float>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async ValueTask<float> Calc(CalcerCustomRealmRuleDef def, CalcerContext ctx)
        {
            var scenicEntity = ctx.EntityContainer.Get<IScenicEntityAlways>(ctx.EntityRef, ReplicationLevel.Always);
            var mapEntityRef = scenicEntity.MapOwner.OwnerMap;
            using (var cnt = await ctx.Repository.Get(mapEntityRef))
            {
                var mapEntity = cnt.Get<IMapEntityAlways>(mapEntityRef, ReplicationLevel.Always);
                var rules = mapEntity.RealmRules;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MapEntity:{mapEntity} RealmRules:{rules.____GetDebugAddress()}").Write();
                float res = def.CustomRealmRule.Target.DefaultValue;
                if (rules != null)
                {
                    if (rules.Values.TryGetValue(def.CustomRealmRule, out var val))
                        res = val;
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"isPvE:{res}").Write();
                return res;
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerCustomRealmRuleDef def) => Enumerable.Empty<StatResource>();
    }
}