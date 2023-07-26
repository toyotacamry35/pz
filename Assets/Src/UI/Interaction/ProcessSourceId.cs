
using System;

namespace ProcessSourceNamespace
{
    public struct ProcessSourceId
    {
        public Guid EntityId;
        public int EntityTypeId;

        public ulong Index;
        public ProcessType Type;

        public enum ProcessType
        {
            CommonInteraction,
            Mining,
            BuildingUpgrade,
        }

        public ProcessSourceId(Guid entityId, int entityTypeId, ProcessType type, ulong index)
        {
            EntityId = entityId;
            EntityTypeId = entityTypeId;
            Type = type;
            Index = index;
        }

        public override string ToString()
        {
            return $"[Index={Index}, type={Type}, entityId/TypeId={EntityId}/{EntityTypeId}]";
        }

        public string ToStringShort()
        {
            return $"[{Index}, {Type}]";
        }
    }
}