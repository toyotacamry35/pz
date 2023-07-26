using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using ProtoBuf;
using SharedCode.Aspects.Building;
using SharedCode.Aspects.Science;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.Quests
{
    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public class StatisticEventArgs
    {
    }

    [ProtoContract]
    public class DealDamageEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public float Value { get; set; }
        [ProtoMember(2)]
        public StatisticType ObjectType { get; set; }
        [ProtoMember(3)]
        public IEntityObjectDef TargetObjectDef { get; set; }

    }

    [ProtoContract]
    public class CraftEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public CraftRecipeDef Recipe { get; set; }
        [ProtoMember(2)]
        public CraftSourceType CraftSource { get; set; }
    }

    [ProtoContract]
    public class KnowledgeEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public KnowledgeDef Knowledge { get; set; }
    }

    [ProtoContract]
    public class KillMortalObjectEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public IEntityObjectDef MortalObjectDef { get; set; }
    }

    [ProtoContract]
    public class PlaceObjectEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public IEntityObjectDef PlacedObject { get; set; }
    }

    [ProtoContract]
    public class InventoryChangedEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public float TotalWeight { get; set; }
    }

    [ProtoContract]
    public class BuildEventArgs : StatisticEventArgs
    {
        [ProtoMember(1)]
        public BuildRecipeDef Building { get; set; }
    }

}
