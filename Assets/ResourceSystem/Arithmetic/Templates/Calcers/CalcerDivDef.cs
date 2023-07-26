using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerDivDef : CalcerDef<float>
    {
        public ResourceRef<CalcerDef<float>> Dividend { get; set; } = new CalcerConstantDef<float>();
        public ResourceRef<CalcerDef<float>> Divisor { get; set; } = new CalcerConstantDef<float> {Value = 1.0f};
    }
}
