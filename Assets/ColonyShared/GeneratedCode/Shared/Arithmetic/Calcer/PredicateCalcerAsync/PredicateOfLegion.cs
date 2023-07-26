using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.RubiconAI;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharedCode.Utils.Extensions;

namespace Assets.ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync
{
    [UsedImplicitly]
    class PredicateOfLegion : ICalcerBinding<PredicateOfLegionDef, bool>
    {
        public ValueTask<bool> Calc(PredicateOfLegionDef def, CalcerContext ctx)
        {
            Legionary.LegionariesByRef.TryGetValue(ctx.EntityRef.To<IEntity>(), out var legionary);

            if (legionary == null)
                return new ValueTask<bool>(false);

            if (legionary.AssignedLegion != null && (legionary.AssignedLegion.Def?.IsOfMyKin(def.OfLegion.Target) ?? false))
                return new ValueTask<bool>(true);

            return new ValueTask<bool>(false);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateOfLegionDef def) => Enumerable.Empty<StatResource>();
    }
}
