using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    public static class PredicateHelper
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(PredicateHelper));
        
        public static ValueTask<bool> CheckPredicate(SpellPredicateDef predicate, SpellPredCastData cast, IEntitiesRepository repo)
        {
            return SpellPredicates.CheckPredicate(cast, predicate, repo);
        }
    }
}
