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
    public class CalcerToxicity : ICalcerBinding<CalcerToxicityDef, float>
    {
        public async ValueTask<float> Calc(CalcerToxicityDef def, CalcerContext ctx) => ProcessWeatherRegions(await GetCurrentRegions(ctx), Toxicity);
        public IEnumerable<StatResource> CollectStatNotifiers(CalcerToxicityDef calcer) { yield return null; }
    }
}
