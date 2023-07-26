using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using ProtoBuf;
using SharedCode.Aspects.Science;

namespace SharedCode.Aspects.Item.Templates
{
    public struct ScienceCountDef : IScienceRewardSource
    {
        public int Count { get; set; }
        public ResourceRef<ScienceDef> Science { get; set; }

        ScienceDef IScienceRewardSource.Science => Science.Target;

        public override string ToString()
        {
            return $"{Science.Target.____GetDebugRootName()}={Count}";
        }
    }

    [ProtoContract]
    public class ScienceCount
    {
        [ProtoMember(1)]
        public ScienceDef Science { get; set; }

        [ProtoMember(2)]
        public int Count { get; set; }
    }
}