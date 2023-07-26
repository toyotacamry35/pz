using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Wizardry;
using SharedCode.EntitySystem;
using NLog;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Utils.Extensions;
using ColonyShared.GeneratedCode;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;
using JetBrains.Annotations;
using SharedCode.Utils;
using SharedCode.Utils.DebugCollector;

namespace GeneratedCode.DeltaObjects
{
    public static class SpellEffects
    {
        [CollectTypes, UsedImplicitly] private static EffectBindingsCollector _effects;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static ValueTask StartEffect(SpellWordCastData castData, SpellEffectDef def, IEntitiesRepository rep)
        {
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{castData.WhereAmI} | Start effect | {def} {castData} Lateness:{SyncTime.Now - castData.WordTimeRange.Start}").Write();
            Collect.IfActive?.EventBgn($"{castData.WhereAmI}.{def.____GetDebugRootName()}.{def.GetType().Name}", castData.Caster, castData.WordGlobalCastId(def));
            if (_effects.Collection.TryGetValue(def.GetType(), out var effect))
            {
                return effect.Attach(castData, rep, def);
//                AsyncStackHolder.AssertNoChildren();
            }
            if (def is TestSpellEffectDef effectDef)
                return StartEffect(castData, rep, effectDef);
#if UNITY_5_3_OR_NEWER
            Logger.IfError()?.Message($" {nameof(StartEffect)} is not implemented for {def.GetType()}").Write();
#endif
            return new ValueTask();
        }
        
        public static ValueTask EndEffect(SpellWordCastData castData, SpellEffectDef def, IEntitiesRepository rep)
        {
            if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{castData.WhereAmI} | Finish effect | {def} {castData} Lateness:{SyncTime.Now - castData.WordTimeRange.Finish}").Write();
            Collect.IfActive?.EventEnd(castData.WordGlobalCastId(def));
            if (_effects.Collection.TryGetValue(def.GetType(), out var effect))
            {
                return effect.Detach(castData, rep, def);
//                AsyncStackHolder.AssertNoChildren();
            }
            if (def is TestSpellEffectDef effectDef)
                return EndEffect(castData, rep, effectDef);
#if UNITY_5_3_OR_NEWER
            Logger.IfError()?.Message(($" {nameof(EndEffect)} is not implemented for {def.GetType()}")).Write();
#endif
            return new ValueTask();
        }
        
        public static bool IsPredictableEffect(SpellEffectDef def)
        {
            return _effects.PredictableEffects.Contains(def.GetType());
        }

        public static bool IsClientOnlyEffect(SpellEffectDef def)
        {
            return _effects.ClientOnlyEffects.Contains(def.GetType());
        }
        
        static async ValueTask StartEffect(SpellWordCastData impactData, IEntitiesRepository rep, TestSpellEffectDef def)
        {
            if (impactData.IsSlave)
                return;
            var tw = impactData.Caster;
            using (var target = await rep.Get(tw.RepTypeId(ReplicationLevel.Server), tw.Guid))
            {
                var statEntity = target.Get<IStatEntityServer>(tw.RepTypeId(ReplicationLevel.Server), tw.Guid);
                await statEntity.SetStat(def.Stat, def.Delta);
            }
        }
        
        static async ValueTask EndEffect(SpellWordCastData impactData, IEntitiesRepository rep, TestSpellEffectDef def)
        {
            if (impactData.IsSlave)
                return;
            var tw = impactData.Caster;
            using (var target = await rep.Get(tw.RepTypeId(ReplicationLevel.Server), tw.Guid))
            {
                var statEntity = target.Get<IStatEntityServer>(tw.RepTypeId(ReplicationLevel.Server), tw.Guid);
                await statEntity.SetStat(def.Stat, -def.Delta);
            }
        }

        
        // ReSharper disable once ClassNeverInstantiated.Local
        public class EffectBindingsCollector : BindingCollectorBase<IEffectBinding, EffectBinding, SpellEffectDef>
        {
            public static readonly Type PredictableAttributeType = typeof(PredictableEffectAttribute);
            public static readonly Type ClientOnlyInterfaceType = typeof(IClientOnlyEffectBinding);

            private Type ClientOnlyBindingHolderGenericType => typeof(ClientOnlyEffectBinding<>); 
            
            public HashSet<Type> ClientOnlyEffects { get; private set; }

            public HashSet<Type> PredictableEffects { get; private set; }
            
            public EffectBindingsCollector() : base(typeof(IEffectBinding<>), typeof(EffectBinding<>)) {}

            protected override void AdditionalInit()
            {
                ClientOnlyEffects = new HashSet<Type>();
                PredictableEffects = new HashSet<Type>();
            }
            
            protected override EffectBinding CreateBindingHolder(Type implType, Type defType, Type holderGenericType)
            {
                if (ClientOnlyInterfaceType.IsAssignableFrom(implType))
                    holderGenericType = ClientOnlyBindingHolderGenericType;
                return CreateBindingHolderStatic(implType, defType, holderGenericType);
            }
            
            protected override void TypeAdded(Type defType, Type implType)
            {
                if (implType.IsDefined(PredictableAttributeType, true))
                    PredictableEffects.Add(defType);
                if (ClientOnlyInterfaceType.IsAssignableFrom(implType))
                    ClientOnlyEffects.Add(defType);
            }
            
            protected override void DumpType(Type defType, EffectBinding holder, StringBuilder sb)
            {
                DumpTypeStatic(defType, holder, sb);
                if (PredictableEffects.Contains(defType))
                    sb.Append(" [Predictable]");
                if (ClientOnlyEffects.Contains(defType))
                    sb.Append(" [ClientOnly]");
            }
        }
    }

    public class PredictableEffectAttribute : Attribute
    { }
}
