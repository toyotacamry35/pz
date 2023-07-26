using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using System;

namespace Assets.Src.GameObjectAssembler.Res
{
    public class StatsDef : SaveableBaseResource
    {
        public float MeanTimeToCheckCalcers { get; set; } = 2f;
        public bool DoNotWork { get; set; } = false;
        public bool DoNotWorkAtAll { get; set; } = false;
        public ResourceRef<StatDef>[] Stats { get; set; } = Array.Empty<ResourceRef<StatDef>>();
    }
}
