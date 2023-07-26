using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerRandom : ICalcerBinding<CalcerRandomDef, float>
    {
        static System.Random _rand = new System.Random(42);

        public ValueTask<float> Calc(CalcerRandomDef def, CalcerContext ctx)
        {
            return new ValueTask<float>(def.Min + (float) _rand.NextDouble() * (def.Max - def.Min));
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerRandomDef def) { yield return null; }
    }
}