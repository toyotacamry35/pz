using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactSetInteractionTypeDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> AnimatorOwner { get; set; }
        public InteractionType InteractionType { get; set; }
    }
}
