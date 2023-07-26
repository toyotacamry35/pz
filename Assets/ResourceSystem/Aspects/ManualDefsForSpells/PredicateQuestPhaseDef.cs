using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class PredicateQuestPhaseDef : SpellPredicateDef
    {
        public ComprasionType Type { get; set; } = ComprasionType.More;
        public int Phases { get; set; } = int.MinValue; // Необходимо указывать именно это значение, т.к. пройденный квест будет иметь номер фазы = -1
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<QuestDef> Quest { get; set; }
    }
}
