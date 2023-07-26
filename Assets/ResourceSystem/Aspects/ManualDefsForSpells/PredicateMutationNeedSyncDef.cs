using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace Shared.ManualDefsForSpells
{
    public class PredicateMutationNeedSyncDef : SpellPredicateDef
    {
        public ResourceRef<SpellEntityDef> Source { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
