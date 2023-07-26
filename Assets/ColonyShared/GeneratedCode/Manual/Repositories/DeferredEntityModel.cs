using ProtoBuf;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace GeneratedCode.Manual.Repositories
{
    [ProtoContract]
    public struct DeferredEntityModel
    {
        public DeferredEntityModel(OuterRef entity, ReplicationLevel replicationLevel)
        {
            Entity = entity;
            ReplicationLevelSerialized = 0;
            ReplicationLevel = replicationLevel;
        }

        [ProtoMember(1)]
        public OuterRef Entity { get; set; }

        [ProtoMember(2)] 
        public long ReplicationLevelSerialized { get; set; }
        
        // Protobuf somehow can't serialize long enum
        public ReplicationLevel ReplicationLevel
        {
            get => (ReplicationLevel) ReplicationLevelSerialized;
            set => ReplicationLevelSerialized = (long)value;
        }
    }
}