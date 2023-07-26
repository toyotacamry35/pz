using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerSpellParameter<T> : ICalcerBinding<CalcerSpellParameterDef<T>, T>
    {
        public async ValueTask<T> Calc(CalcerSpellParameterDef<T> def, CalcerContext ctx)
        {
            var spellData = ctx.SpellCastData;
            if (spellData == null)
                throw new Exception($"Wrong calcer context: {nameof(CalcerSpellParameter<T>)} must be called within spell");
            return ValueConverter<T>.Convert(await CastDataExtension.GetValue(def.Parameter, spellData, ctx.Repository));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerSpellParameterDef<T> def) => Enumerable.Empty<StatResource>();
    }
    
    [UsedImplicitly]
    public class CalcerSpellParameterCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect() => Value.MakeGenericTypes(typeof(CalcerSpellParameter<>));
    }
}