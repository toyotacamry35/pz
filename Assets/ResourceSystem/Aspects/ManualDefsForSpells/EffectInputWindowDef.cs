using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Aspects.ManualDefsForSpells
{
    public class EffectInputWindowDef : SpellEffectDef
    {
        [JsonProperty(Required = Required.Always)] public Dictionary<ResourceRef<InputActionTriggerDef>,ResourceRef<InputActionTriggerHandlerDef>> Handlers { get; [UsedImplicitly] set; }
        [JsonProperty(Required = Required.Always)] public float Delay { get; [UsedImplicitly] set; }
        public ResourceRef<InputActionHandlersLayerDef> Layer { get; [UsedImplicitly] set; } = new ResourceRef<InputActionHandlersLayerDef>(EffectInputLayerDef.DefaultLayer);
        public bool DelayIsBeforeEnd { get; [UsedImplicitly] set; }
        public bool BreakCurrentSpell { get; [UsedImplicitly] set; } = true;
    }
}