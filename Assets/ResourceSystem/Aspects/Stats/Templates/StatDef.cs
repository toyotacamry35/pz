using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using System;

namespace Assets.Src.Aspects.Impl.Stats
{
    public class StatDef : BaseResource
    {
        [JsonProperty(Required = Required.Always)]
        public ResourceRef<StatResource> StatResource { get; set; }

        public float LimitMaxDefault { get; set; } = float.MaxValue;

        [JsonProperty(Required = Required.DisallowNull)]
        public ResourceRef<StatResource> LimitMinStat { get; set; }

        public float LimitMinDefault { get; set; } = float.MinValue;
        
        [JsonProperty(Required = Required.DisallowNull)]
        public ResourceRef<StatResource> LimitMaxStat { get; set; }

        public bool IsBroadcasted { get; set; } = false;

        public StatType StatType { get; set; } = StatType.Internal;

        public float InitialValue { get; set; } = 0;
    }

    [Flags]
    public enum StatType
    {
        Internal    = 1,
        General     = 2,
        Specific    = 4,
        All         = Internal | General | Specific
    }
}
