using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerDiv : ICalcerBinding<CalcerDivDef, float>
    {
        public async ValueTask<float> Calc(CalcerDivDef def, CalcerContext ctx)
        {
            return await def.Dividend.Target.CalcAsync(ctx) / await def.Divisor.Target.CalcAsync(ctx);
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerDivDef calcer)
        {
            //Logger.Log(NLog.LogLevel.Info, $"        CalcerDiv.Collect");
            foreach (var res in calcer.Dividend.GetModifiers())
                yield return res;
            foreach (var res in calcer.Divisor.GetModifiers())
                yield return res;
            //Logger.Log(NLog.LogLevel.Info, $"    /// CalcerDiv.Collect");
        }
    }
}

