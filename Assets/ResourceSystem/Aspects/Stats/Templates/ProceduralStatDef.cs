using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Aspects.Impl.Stats
{
    public class ProceduralStatDef : StatDef
    {
        public ResourceRef<CalcerDef<float>> ValueCalcer { get; set; }
    }
}