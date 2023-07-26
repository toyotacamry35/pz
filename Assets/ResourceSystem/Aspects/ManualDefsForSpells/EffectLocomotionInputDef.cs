using Newtonsoft.Json;
using SharedCode.Wizardry;

namespace Src.ManualDefsForSpells
{
    public class EffectLocomotionInputDef : SpellEffectDef
    {
        [JsonProperty(Required = Required.Always)] public string Input { get; set; }
        public float Value { get; set; } = 1;
    }
}