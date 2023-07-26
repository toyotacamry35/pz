using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ResourceSystem.Reactions
{
    public class ValueDef<T> : VarDef<T>, IValue<T>
    {
        [JsonProperty(Required = Required.Always)] public T Value { get; [UsedImplicitly] set; }
        ColonyShared.SharedCode.Value IValue.Value => ColonyShared.SharedCode.ValueConverter<T>.Convert(Value);
    }
}