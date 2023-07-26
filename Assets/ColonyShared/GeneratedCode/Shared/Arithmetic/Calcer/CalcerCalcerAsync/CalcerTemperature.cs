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
using static SharedCode.Aspects.Item.Templates.Constants;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerTemperature : ICalcerBinding<CalcerTemperatureDef, float>
    {
        public async ValueTask<float> Calc(CalcerTemperatureDef def, CalcerContext ctx) => ProcessWeatherRegions(await GetCurrentRegions(ctx), Temperature, WorldConstants.DefaultTemperature);
        public IEnumerable<StatResource> CollectStatNotifiers(CalcerTemperatureDef calcer) { yield return null; } 
    }
}
