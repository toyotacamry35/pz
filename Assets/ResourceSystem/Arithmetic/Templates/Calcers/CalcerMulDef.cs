using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerMulDef : CalcerDef<float>
    {
        public ResourceArray<CalcerDef<float>> Multipliers { get; [UsedImplicitly] set; } = ResourceArray<CalcerDef<float>>.Empty;
    }
}
