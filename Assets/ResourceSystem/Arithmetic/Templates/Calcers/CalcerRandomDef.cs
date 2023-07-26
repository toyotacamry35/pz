using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerRandomDef : CalcerDef<float>
    {
        public float Min { get; [UsedImplicitly] set; } = 0;
        public float Max { get; [UsedImplicitly] set; } = 1;
    }
}
