using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using ProtoBuf;
using SharedCode.Aspects.Item.Templates;

namespace SharedCode.Entities
{
    [ProtoContract]
    public struct ItemResourcePack
    {
        [ProtoMember(1)] public BaseItemResource ItemResource { get; [UsedImplicitly] set; }
        [ProtoMember(2)] public uint Count { get; set; }
        [ProtoMember(3)] public int Index { get; [UsedImplicitly] set; }
        [ProtoMember(4)] public List<ValueStatDef> StatsModifiers { get; set; }

        public ItemResourcePack(BaseItemResource itemResource, uint count, int index = -1, List<ValueStatDef> statsModifiers = null)
        {
            ItemResource = itemResource;
            Count = count;
            Index = index;
            StatsModifiers = statsModifiers;
        }

        public override string ToString()
        {
            return $"{nameof(ItemResource)}: {ItemResource?.ToString() ?? "null"}, {nameof(Count)}: {Count}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemResourcePack))
                return false;
            var irp = (ItemResourcePack)obj;
            return irp.ItemResource == ItemResource && irp.Count == Count;
        }

        public override int GetHashCode()
        {
            var hashCode = -1492983518;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseItemResource>.Default.GetHashCode(ItemResource);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ItemResourcePack a, ItemResourcePack b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ItemResourcePack a, ItemResourcePack b) => !(a == b);

        public bool IsValid => ItemResource != null; //&& Count > 0;
    }
}
