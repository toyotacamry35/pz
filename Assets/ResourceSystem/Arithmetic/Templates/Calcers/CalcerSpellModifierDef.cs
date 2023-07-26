using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using ResourceSystem.Reactions;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerSpellModifierDef : CalcerDef<float>
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<ArgDef<float>> Variable;
        [JsonProperty(Required = Required.Always)] public CalcerSpellModifierStackingPolicy StackingPolicy;
        [JsonProperty(Required = Required.Always)] public float DefaultValue;
    }
    
    public enum CalcerSpellModifierStackingPolicy
    {
        Multiply,
        Add,
        Min,
        Max
    }
}