using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Aspects.ManualDefsForSpells
{
    public class EffectInputSpellBreakerDef : SpellEffectDef
    {
        public ResourceRef<InputActionHandlersLayerDef> Layer = new ResourceRef<InputActionHandlersLayerDef>(EffectInputLayerDef.DefaultLayer);
        public ResourceArray<InputActionTriggerDef> Actions { get; [UsedImplicitly] set; }
        public ResourceRef<InputActionsListDef> List { get; [UsedImplicitly] set; }
        public When When { get; [UsedImplicitly] set; } = When.Active;
        public FinishReasonType FinishReason { get; [UsedImplicitly] set; } = FinishReasonType.Success;
    }
}