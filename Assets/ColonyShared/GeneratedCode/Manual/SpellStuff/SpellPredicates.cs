using System.Threading.Tasks;
using SharedCode.Wizardry;
using SharedCode.EntitySystem;
using NLog;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Utils.Extensions;
using SharedCode.Utils;
using ColonyShared.GeneratedCode;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;
using JetBrains.Annotations;

namespace GeneratedCode.DeltaObjects
{
    public static class SpellPredicates
    {
        [CollectTypes, UsedImplicitly] private static PredicateBindingsCollector _predicates;
        
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public static async ValueTask<bool> CheckPredicate(SpellPredCastData cast, SpellPredicateDef def, IEntitiesRepository rep)
        {
            using (EntitySystemBlock.Lock)
            {
                if (_predicates.Collection.TryGetValue(def.GetType(), out var predicate))
                { 
                    var res = await predicate.True(cast, rep, def);
                    if (def.Inversed)
                        return !res;
                    else
                        return res;
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    Logger.IfError()?.Message(($"Predicate without implemented for {def.GetType()}")).Write();
#endif
                    return true;
                }
            }
        }

        static async Task<bool> CheckPredicate(SpellCastData impactData, IEntitiesRepository rep, TestCheckSimpleStat def)
        {
            using (EntitySystemBlock.Lock)
            {
                var tw = impactData.Caster;
                using (var target = await rep.Get(tw.RepTypeId(ReplicationLevel.Server), tw.Guid))
                {
                    var statEntity = target.Get<IStatEntityServer>(tw.RepTypeId(ReplicationLevel.Server), tw.Guid);
                    float val = 0f;
                    statEntity.SimpleStupidStats.TryGetValue(def.Stat, out val);
                    if (val > def.More)
                        return true;
                }
                return false;
            }
        }

        public static bool TryGetPredicateBinding(SpellPredicateDef def, out PredicateBinding binding)
        {
            return _predicates.Collection.TryGetValue(def.GetType(), out binding);
        }
        
        public class PredicateBindingsCollector : BindingCollector<IPredicateBinding, PredicateBinding, SpellPredicateDef>
        {
            public PredicateBindingsCollector() : base(typeof(IPredicateBinding<>), typeof(PredicateBinding<>)) {}
        }
    }
}
