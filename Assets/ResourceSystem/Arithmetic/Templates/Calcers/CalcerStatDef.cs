using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerStatDef : CalcerDef<float>
    {
        public ResourceRef<StatResource> Stat { get; [UsedImplicitly] set; }
    }
}
