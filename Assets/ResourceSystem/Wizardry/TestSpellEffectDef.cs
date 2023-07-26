using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Wizardry
{
    public class TestSpellEffectDef : SpellEffectDef
    {
        public ResourceRef<StatResource> Stat { get; set; }
        public float Delta { get; set; }
    }

}
