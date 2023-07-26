using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.ResourcesSystem.Base;
using Scripting;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactCastSpellDef : SpellImpactDef
    {
        public ResourceRef<ScriptingContextDef> Context { get; set; } = new ScriptingContextDef();
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellVector3Def> Direction { get; set; }
        public ResourceRef<SpellDef> Spell { get; set; }
        public List<ResourceRef<SpellDef>> Spells { get; set; }
        public ResourceRef<CalcerPiecewiseResourceDef> ProcSpell { get; set; } 

    }
}