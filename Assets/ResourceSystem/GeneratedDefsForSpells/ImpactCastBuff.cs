using Assets.Src.ResourcesSystem.Base;
using Scripting;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceSystem.GeneratedDefsForSpells
{
    public class ImpactCastBuffDef : SpellImpactDef
    {
        public ResourceRef<BuffDef> Buff { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<BuffScriptingContextDef> Context { get; set; }
    }
    public class ImpactRemoveBuffDef : SpellImpactDef
    {
        public ResourceRef<BuffDef> Buff { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }

    public class PredicateHasBuffDef : SpellPredicateDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<BuffDef> Buff { get; set; } 
    }
}
