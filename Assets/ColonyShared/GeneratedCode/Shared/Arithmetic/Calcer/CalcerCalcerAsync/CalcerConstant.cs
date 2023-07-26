using System;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects;
using ColonyShared.SharedCode;

namespace Assets.Src.Arithmetic
{
    
    [UsedImplicitly]
    public class CalcerConstant<ReturntType> : ICalcerBinding<CalcerConstantDef<ReturntType>, ReturntType>
    {
        public ValueTask<ReturntType> Calc(CalcerConstantDef<ReturntType> def, CalcerContext ctx) => new ValueTask<ReturntType>(def.Value);

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerConstantDef<ReturntType> def) => Enumerable.Empty<StatResource>();
    }

    [UsedImplicitly]
    public class CalcerResource : ICalcerBinding<CalcerResourceDef, BaseResource>
    {
        public ValueTask<BaseResource> Calc(CalcerResourceDef def, CalcerContext ctx) => new ValueTask<BaseResource>(def.Value.Target);

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerResourceDef def) => Enumerable.Empty<StatResource>();
    }
    
    [UsedImplicitly]
    [Obsolete("For backward compatibility. Use CalcerConstant<float> instead")]
    public class CalcerConstant : CalcerConstant<float>, ICalcerBinding<CalcerConstantDef, float>
    {
        public ValueTask<float> Calc(CalcerConstantDef def, CalcerContext ctx) => base.Calc(def, ctx);
        public IEnumerable<StatResource> CollectStatNotifiers(CalcerConstantDef def) => Enumerable.Empty<StatResource>();
    }
    
    [UsedImplicitly]
    public class CalcerConstantCollector : ICalcerBindingsCollector
    {
        public IEnumerable<Type> Collect()
        {
            var genericType = typeof(CalcerConstant<>);
            return Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None && x.Item1 != Value.Type.Float).Select(x => genericType.MakeGenericType(x.Item2));
        }
    }
}
