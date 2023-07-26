using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactSetQuestVisibleDef : SpellImpactDef
    {
        public ResourceRef<QuestDef> Quest { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public bool Visible { get; set; } = true;
    }
}