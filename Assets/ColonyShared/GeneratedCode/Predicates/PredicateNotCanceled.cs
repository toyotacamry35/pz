using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace ColonyShared.GeneratedCode.Predicates
{
    public class PredicateNotCanceled : IPredicateBinding<PredicateNotCanceledDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateNotCanceledDef def)
        {
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[{cast.WhereAmI}] Spell {def} is {(cast.Canceled?"CANCELED":"not canceled")}").Write();
            return new ValueTask<bool>(Task.FromResult(!cast.Canceled));
        }
    }
}