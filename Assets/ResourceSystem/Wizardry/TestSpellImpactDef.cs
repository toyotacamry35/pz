using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Wizardry
{
    public class TestSpellImpactDef : SpellImpactDef
    {
        public ResourceRef<StatResource> Stat { get; set; }
        public float Delta { get; set; }
        public float? Set { get; set; } = null;
    }

}
