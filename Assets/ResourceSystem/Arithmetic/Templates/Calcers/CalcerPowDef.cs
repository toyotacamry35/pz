using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerPowDef : CalcerDef<float>
    {
        public ResourceRef<CalcerDef<float>> Value { get; [UsedImplicitly] set; } = new CalcerConstantDef<float>();
        public float Power { get; [UsedImplicitly] set; } = 1;
    }
}
