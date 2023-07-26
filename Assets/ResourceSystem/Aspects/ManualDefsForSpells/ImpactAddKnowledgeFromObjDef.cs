using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactAddKnowledgeFromObjDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> KnowledgeHolder { get; set; }
    }
}
