using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using static Assets.ColonyShared.GeneratedCode.Regions.RegionProcessors.WeatherRegionProcessor;
using static Assets.ColonyShared.GeneratedCode.Regions.RegionProcessors.WeatherRegionProcessor.WeatherType;
using static Assets.Src.Arithmetic.CalcerRegionsHelper;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerOxygen : ICalcerBinding<CalcerOxygenDef, float>
    {
        public async ValueTask<float> Calc(CalcerOxygenDef def, CalcerContext ctx) => ProcessWeatherRegions(await GetCurrentRegions(ctx), Oxygen);
        public IEnumerable<StatResource> CollectStatNotifiers(CalcerOxygenDef calcer) { yield return null; }
    }
}
