using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;

namespace GeneratedCode.Predicates
{
    public class PredicateHasPremium : IPredicateBinding<SpellPredicateHasPremium>
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateHasPremium def)
        {
            return new ValueTask<bool>(false);
        }

    }
}
