using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Modifiers;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerSpellModifier : ICalcerBinding<CalcerSpellModifierDef, float> 
    {
        public ValueTask<float> Calc(CalcerSpellModifierDef def, CalcerContext ctx)
        {
            var spellData = ctx.SpellPredCastData;
            if (spellData == null)
                throw new Exception($"Wrong calcer context: {nameof(CalcerSpellModifierDef)} must be called within spell. SpellCast:{ctx.SpellCastData}");
            float value = spellData.Modifiers.GetSpellCalcerModifier(def.Variable, def.StackingPolicy, def.DefaultValue);
            return new ValueTask<float>(value);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerSpellModifierDef _) => Enumerable.Empty<StatResource>();
    }
}