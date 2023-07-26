using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class PredicateIsKnockedDownDef : SharedCode.Wizardry.SpellPredicateDef
    {
        public Assets.Src.ResourcesSystem.Base.ResourceRef<SharedCode.Wizardry.SpellEntityDef> Target { get; set; } = new SpellTargetDef();
    }
}
