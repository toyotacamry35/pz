using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Wizardry
{
    public class TestCheckSimpleStat : SpellImpactDef
    {
        public ResourceRef<StatResource> Stat { get; set; }
        public float More { get; set; }
    }

}
