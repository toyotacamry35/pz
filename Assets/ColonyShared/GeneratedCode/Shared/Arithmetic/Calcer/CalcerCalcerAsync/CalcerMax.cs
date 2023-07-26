using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerMax : ICalcerBinding<CalcerMaxDef,float>
    {
        public async ValueTask<float> Calc(CalcerMaxDef def, CalcerContext ctx)
        {
            if (def.Values.Count == 0)
                throw new ArgumentOutOfRangeException();

            float result = float.MinValue;
            foreach (var value in def.Values)
            {
                var res = await value.CalcAsync(ctx);
                if (res > result)
                    result = res;
            }
            return result;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerMaxDef calcer)
        {
            foreach (var value in calcer.Values)
                foreach (var res in value.GetModifiers())
                    yield return res;
        }
    }
}
