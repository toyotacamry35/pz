using System.Collections.Generic;
using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourceSystem.Reactions;
using SharedCode.Wizardry;

namespace Assets.Src.ManualDefsForSpells
{
    public class EffectPostVisualEventOnTargetDef : SpellEffectDef
    {
        public ResourceRef<FXEventType> TriggerName { get; set; }
        public Dictionary<ResourceRef<ArgDef>, ResourceRef<SpellContextValueDef>> Params { get; [UsedImplicitly] set; }
    }
}
