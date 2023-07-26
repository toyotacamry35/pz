using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public class CalcerExpDef : CalcerDef<float>
    {
        public ResourceRef<CalcerDef<float>> Pow { get; [UsedImplicitly] set; }
    }
}