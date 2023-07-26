using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactAddImpulseDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> To { get; set; }
        public float ImpulseValue { get; set; }
        public ResourceRef<SpellVector3Def> ImpulseDirection { get; set; }
        public bool _directionInLocalSpace { get; set; }
    }
}
