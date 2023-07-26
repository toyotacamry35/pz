using System.Threading.Tasks;
using SharedCode.Wizardry;
using SharedCode.EntitySystem;
using NLog;
using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using System.Diagnostics;
using System.Text;
using GeneratedCode.Manual.Repositories;
using SharedCode.Utils.DebugCollector;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.GeneratedCode;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class SpellImpacts
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [CollectTypes, UsedImplicitly] private static ImpactBindingCollector _collector;

        public static ValueTask CastImpact(SpellWordCastData castData, SpellImpactDef def, IEntitiesRepository rep)
        {
            // теперь это проверяется самим wizard'ом
            // if (castData.IsSlave)
            // {
            //     if (!(castData.SlaveMark.OnServer && def.UnityAuthorityServerImpact))
            //         return true;
            // }
            // else if (def.UnityAuthorityServerImpact)
            //     return true;

            if (((IResource)def).Address.Root == null)  Logger.IfError()?.Message($"{castData.WhereAmI} | Def without root in spell | {castData} {def.GetType()}").Write();
            if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{castData.WhereAmI} | Start impact | {def} {castData} Lateness:{SyncTime.Now - castData.WordTimeRange.Start}").Write();
            Collect.IfActive?.Event($"{castData.WhereAmI}.{def.____GetDebugRootName()}.{def.GetType().Name}", castData.Caster);
			
            if (_collector.Collection.TryGetValue(def.GetType(), out var impact))
            {
                return impact.Apply(castData, rep, def);
//                    AsyncStackHolder.AssertNoChildren(); // теперь эта проверка в TimelineRunner
            }
            
            Logger.IfError()?.Message($" {nameof(CastImpact)} is not implemented for {def.GetType()}").Write();
            return new ValueTask();
        }
        
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ImpactBindingCollector : BindingCollector<IImpactBinding, ImpactBinding, SpellImpactDef>
        {
            public ImpactBindingCollector() : base(typeof(IImpactBinding<>), typeof(ImpactBinding<>)) {}
        }
    }
}
