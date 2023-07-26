using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace ColonyShared.GeneratedCode.Predicates
{
    [UsedImplicitly]
    public class SpellPredicateFromCalcerPredicate : IPredicateBinding<SpellPredicateFromCalcerPredicateDef>
    {
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, SpellPredicateFromCalcerPredicateDef def)
        {
            if (def.Predicate != null)
            {
                using (var cnt = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
                {
                    var ctx = new CalcerContext(cnt, cast.Caster, repo, cast, null, cast.CastData.Context);
                    return await def.Predicate.Target.CalcAsync(ctx);
                }
            }
            return false;
        }
    }
}