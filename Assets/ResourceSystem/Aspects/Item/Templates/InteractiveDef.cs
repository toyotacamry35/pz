using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class InteractiveDef : ComponentDef
    {
        public bool ShowMarker { get; set; } = true;
        public LocalizedString ObjectNameLs { get; set; }

        public ResourceRef<ContextualActionsDef> ContextualActions { get; set; } = new ResourceRef<ContextualActionsDef>();
        public InteractionType Interaction { get; set; }
        public InteractionType Know { get; set; }
        public ResourceRef<SpellDef> BasicInteractionSpell { get; set; }
        public ResourceRef<SpellDef> BasicAttackSpell { get; set; }
    }
}
