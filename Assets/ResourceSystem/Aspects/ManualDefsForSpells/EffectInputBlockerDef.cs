using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Aspects.ManualDefsForSpells
{
    public class EffectInputBlockerDef : SpellEffectDef
    {
        public ResourceRef<InputActionHandlersLayerDef> Layer = new ResourceRef<InputActionHandlersLayerDef>(EffectInputLayerDef.DefaultLayer);
        public ResourceRef<InputActionDef>[] Block;
        public ResourceRef<InputActionsListDef> BlockList;
        public ResourceRef<InputActionDef>[] Except;
        public ResourceRef<InputActionsListDef> ExceptList;
    }
}