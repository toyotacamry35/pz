using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerSelect<T> : ICalcerBinding<CalcerSelectDef<T>, T>
    {
        public async ValueTask<T> Calc(CalcerSelectDef<T> def, CalcerContext ctx)
        {
            foreach (var r in def.Ranges)
                if (await r.Condition.Target.CalcAsync(ctx))
                    return await r.Value.Target.CalcAsync(ctx);
            return def.Default.Target != null ? await def.Default.Target.CalcAsync(ctx) : default(T);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerSelectDef<T> def)
        {
            foreach (var range in def.Ranges)
            {
                if (range.Condition.Target != null)
                    foreach (var res in range.Condition.GetModifiers())
                        yield return res;
                if (range.Value.Target != null)
                    foreach (var res in range.Value.GetModifiers())
                        yield return res;
            }

            if (def.Default.Target != null)
                foreach (var res in def.Default.Target.GetModifiers())
                    yield return res;
        }
    }
    
    [UsedImplicitly]
    public class CalcerSelectCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect() => Value.MakeGenericTypes(typeof(CalcerSelect<>));
    }
}