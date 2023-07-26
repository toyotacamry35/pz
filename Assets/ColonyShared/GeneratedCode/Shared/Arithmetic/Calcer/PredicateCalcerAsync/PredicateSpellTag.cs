using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Aspects;

namespace ColonyShared.GeneratedCode.Shared.Arithmetic.Calcer.PredicateCalcerAsync
{
    [UsedImplicitly]
    public class PredicateSpellTag : ICalcerBinding<PredicateSpellTagDef, bool>
    {
        public ValueTask<bool> Calc(PredicateSpellTagDef def, CalcerContext ctx)
        {
            var spellCast = ctx.SpellCastData;
            if (spellCast == null)
                throw new InvalidOperationException($"{nameof(PredicateSpellTag)} must be used in the context of the spell.");
            return new ValueTask<bool>(spellCast.CastData.Def?.Tags != null && spellCast.CastData.Def.Tags.Contains(def.Tag));
        }
        
        public IEnumerable<StatResource> CollectStatNotifiers(PredicateSpellTagDef def) => Enumerable.Empty<StatResource>();
    }
}