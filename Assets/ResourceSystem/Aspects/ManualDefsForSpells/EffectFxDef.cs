using System.Collections.Generic;
using Assets.Src.Character.Events;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using ResourceSystem.Reactions;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectFxDef : SpellEffectDef
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<VisualEffectHandlerCasterTargetDef> Fx;
        [JsonProperty(Required = Required.DisallowNull)] public ResourceRef<SpellEntityDef> Owner = new ResourceRef<SpellEntityDef>(new SpellCasterDef()); // тот на ком должен проигрываться FX
        public Dictionary<ResourceRef<ArgDef>, ResourceRef<SpellContextValueDef>> Params;
        public ResourceRef<SpellEntityDef> Target; // необязательная цель для FX'ов типа PlaceUnparentedFXWithTarget
    }
}