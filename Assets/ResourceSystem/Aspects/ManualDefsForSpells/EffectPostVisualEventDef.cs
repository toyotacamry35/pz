using System.Collections.Generic;
using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using UnityEngine;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourceSystem.Reactions;

namespace Assets.Src.ManualDefsForSpells
{
    public class EffectDebugFxDef : SpellEffectDef
    {
        public UnityRef<GameObject> FxObj { get; set; }
        public string AttachmentObj { get; set; }
    }
    public class EffectDebugTintDef : SpellEffectDef
    {
        public bool IgnoreTintDisablment { get; set; } = false;
        public SharedCode.Utils.Color Color { get; set; }
    }
    public class EffectPostVisualEventDef : SpellEffectDef
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<FXEventType> TriggerName { get; [UsedImplicitly] set; }
        public ResourceRef<SpellEntityDef> Target { get; [UsedImplicitly] set; }
        public Dictionary<ResourceRef<ArgDef>, ResourceRef<SpellContextValueDef>> Params { get; [UsedImplicitly] set; }
    }
}
