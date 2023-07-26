using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerVectorComponent : ICalcerBinding<CalcerVectorComponentDef, float>
    {
        public async ValueTask<float> Calc(CalcerVectorComponentDef def, CalcerContext ctx)
        {
            var vec = await def.Vector.Target.CalcAsync(ctx);
            switch (def.Component)
            {
                case CalcerVectorComponentDef.VectorComponent.X:
                    return vec.x;
                case CalcerVectorComponentDef.VectorComponent.Y:
                    return vec.y;
                case CalcerVectorComponentDef.VectorComponent.Z:
                    return vec.z;
                default:
                    throw new ArgumentException(nameof(def.Component));
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerVectorComponentDef def)
        {
            return def.Vector.Target.GetModifiers();
        }
    }
    
    [UsedImplicitly]
    public class CalcerVector2Component : ICalcerBinding<CalcerVector2ComponentDef, float>
    {
        public async ValueTask<float> Calc(CalcerVector2ComponentDef def, CalcerContext ctx)
        {
            var vec = await def.Vector.Target.CalcAsync(ctx);
            switch (def.Component)
            {
                case CalcerVector2ComponentDef.VectorComponent.X:
                    return vec.x;
                case CalcerVector2ComponentDef.VectorComponent.Y:
                    return vec.y;
                default:
                    throw new ArgumentException(nameof(def.Component));
            }
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerVector2ComponentDef def)
        {
            return def.Vector.Target.GetModifiers();
        }
    }
}