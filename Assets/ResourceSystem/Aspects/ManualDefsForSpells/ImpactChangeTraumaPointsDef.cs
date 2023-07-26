using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactChangeTraumaPointsDef : SpellImpactDef
    {
        public int Delta { get; set; }
        public string TraumaType { get; set; }
    }
}
