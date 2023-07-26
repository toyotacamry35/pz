using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Assets.Src.ManualDefsForSpells
{
    public class EffectPostVisualEventVisibleOnlyToPlayerDef : SpellEffectDef
    {
        public ResourceRef<FXEventType> TriggerName { get; set; }
    }
}