using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Item.Templates
{
    public class ContextualActionDef : BaseResource
    {
        public ResourceRef<SpellDef> Spell { get; set; }
        public ResourceRef<SpellDef> CheckSpell { get; set; }
    }
}