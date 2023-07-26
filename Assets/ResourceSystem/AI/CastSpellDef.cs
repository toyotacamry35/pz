using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using SharedCode.Wizardry;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class CastSpellDef : BehaviourNodeDef
    {
        public ResourceRef<SpellDef> Spell { get; set; }
        public ResourceRef<SpellSelectorDef> WhatSpell { get; set; }
        public ResourceRef<TargetSelectorDef> At { get; set; }
        public bool PointTargetHasPriority { get; set; } = false;

        public bool TreatFailedSpellAsSuccess { get; set; } = false;
        public override string ToString()
        {
            return base.ToString() + $" (Spell:{Spell} At:{At})";
        }
    }
}
