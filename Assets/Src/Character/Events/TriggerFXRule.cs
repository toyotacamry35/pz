using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Entities.Reactions;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;

namespace Assets.Src.Character.Events
{
    public class TriggerFXRule
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly TriggerFXRuleDef _def;

        public TriggerFXRule(TriggerFXRuleDef def)
        {
            _def = def ?? throw new ArgumentNullException(nameof(def));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool CheckEventType(FXEventType evt) => _def.EventType == evt;

        
        public async Task<bool> CheckPredicate(VisualEvent evt)
        {
            Logger.IfDebug()?.Message($"Check event predicate | Rule:{_def} Event:{evt}").Write();
            bool predicateResult = true;
            if (_def.Predicate != null && !(_def.Predicate.Target is PredicateTrueDef))
                using (var wrapper = await evt.casterRepository.Get(evt.casterEntityRef.TypeId, evt.casterEntityRef.Guid))
                    predicateResult = await _def.Predicate.Target.CalcAsync(new CalcerContext(wrapper, evt.casterEntityRef, evt.casterRepository, null, ToCalcerArgs(evt.parameters)));
            if (!predicateResult)
                Logger.IfDebug()?.Message($"Predicate failed | Rule:{_def} Event:{evt} Predicate:{_def.Predicate}").Write();
            return predicateResult;
        }
        
        
        public void UpdateEvent(VisualEvent evt, Animator animator)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Run event | Rule:{_def} Event:{evt}").Write();

            if (_def.AnimatedProp != null && animator)
                animator.SetTrigger(_def.AnimatedProp);

            if (_def.OnEvent.Target != null)
            {
                var swaped = new VisualEventSwapWrapper(evt);
                foreach (var t in _def.OnEvent.Target)
                    VisualEffectHandlerCollection.Get(t).OnEventUpdate(t, swaped);
            }
            if (_def.OnEvent.Caster != null)
                foreach (var c in _def.OnEvent.Caster)
                    VisualEffectHandlerCollection.Get(c).OnEventUpdate(c, evt);
        }

        private CalcerContext.Arg[] ToCalcerArgs(ArgTuple[] args)
        {
            var res = new CalcerContext.Arg[args.Length];
            for (int i = 0; i < args.Length; ++i)
                res[i] = new CalcerContext.Arg(args[i].Def, args[i].Value.Value);
            return res;
        }
    }
}
