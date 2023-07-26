using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerTempChangeDef : CalcerDef<float>
    {
        public ResourceRef<StatResource> ThermalBalance { get; set; }
        public ResourceRef<CalcerDef<float>> MaxTemperature { get; set; }
        public ResourceRef<CalcerDef<float>> SecondsToReachMax{ get; set; }
    }
}
