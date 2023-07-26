using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using static Assets.ColonyShared.GeneratedCode.Regions.RegionProcessors.WeatherRegionProcessor;
using static Assets.ColonyShared.GeneratedCode.Regions.RegionProcessors.WeatherRegionProcessor.WeatherType;
using static Assets.Src.Arithmetic.CalcerRegionsHelper;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerHumidity : ICalcerBinding<CalcerHumidityDef, float>
    {
        public async ValueTask<float> Calc(CalcerHumidityDef def, CalcerContext ctx) => ProcessWeatherRegions(await GetCurrentRegions(ctx), Humidity);
        public IEnumerable<StatResource> CollectStatNotifiers(CalcerHumidityDef calcer) { yield return null; }
    }
}
