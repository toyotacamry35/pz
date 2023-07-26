using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerTempChange : ICalcerBinding<CalcerTempChangeDef, float>
    {
        public async ValueTask<float> Calc(CalcerTempChangeDef def, CalcerContext ctx)
        {
            var statsengine = ctx.GetEntity<IHasStatsEngineClientFull>(ReplicationLevel.ClientFull);

            float thermBalance = (await statsengine.Stats.TryGetValue(def.ThermalBalance)).Item2;
            var maxTemp = await def.MaxTemperature.Target.CalcAsync(ctx);
            var secondsMax = await def.SecondsToReachMax.Target.CalcAsync(ctx);
            float rateCoef = 3.5f;

            // Logger.IfInfo()?.Message($"MaxTemperature = {maxTemp}, SecondsToReachMax = {secondsMax}").Write();

            var rate = ((maxTemp - thermBalance) / secondsMax) * rateCoef;

            // Logger.IfInfo()?.Message($"rate = {rate}, ThermalBalance = {thermBalance}").Write();


            if (Math.Abs(maxTemp - thermBalance) < rate)
                rate = maxTemp - thermBalance;
            if (Math.Abs(maxTemp - thermBalance) < 0.1)
                rate = 0;
            return rate;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerTempChangeDef def)
        {
            if (def.ThermalBalance != null && def.ThermalBalance.IsValid)
                yield return def.ThermalBalance.Target;
            foreach (var res in def.MaxTemperature.GetModifiers())
                yield return res;
            foreach (var res in def.SecondsToReachMax.GetModifiers())
                yield return res;
        }
    }
}