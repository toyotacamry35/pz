using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactThrowDef : SpellImpactDef
    {
        public float InitialAngleForTarget { get; set; }
        public ResourceRef<SpellTargetDef> OptionalTarget { get; set; }
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public List<ResourceRef<SpellDef>> AppliedSpells { get; set;}
        public ResourceRef<BaseResource> ItemResourceStatic { get; set; }
        public float ThrowAngle { get; set; }
    }
}