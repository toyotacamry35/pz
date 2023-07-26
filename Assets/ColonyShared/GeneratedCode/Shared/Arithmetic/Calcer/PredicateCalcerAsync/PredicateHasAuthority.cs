using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace  Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class PredicateHasAuthority : ICalcerBinding<PredicateHasAuthorityDef, bool>
    {
        public async ValueTask<bool> Calc(PredicateHasAuthorityDef def, CalcerContext ctx)
        {
            return (await def.Entity.Target.CalcAsync(ctx)).HasClientAuthority(ctx.Repository);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(PredicateHasAuthorityDef def) => def.Entity.Target.CollectStatNotifiers();
    }
}