using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Aspects.Impl.Stats
{
    public class TimeStatDef : StatDef
    {
        public ResourceRef<CalcerDef<float>> ChangeRateCalcer { get; set; }
        public ResourceRef<StatResource> ChangeRateStat { get; set; }
        public float ChangeRateDefault { get; set; }
    }
}
