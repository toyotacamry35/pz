using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ProtoBuf;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;

namespace SharedCode.Entities
{
    [ProtoContract]
    public struct CurrencyResourcePack
    {
        [ProtoMember(1)] public CurrencyResource ItemResource { get; [JetBrains.Annotations.UsedImplicitly] set; }
        [ProtoMember(2)] public int Count { get; [JetBrains.Annotations.UsedImplicitly] set; }

        public CurrencyResourcePack(CurrencyResource itemResource, int count)
        {
            ItemResource = itemResource;
            Count = count;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CurrencyResourcePack))
                return false;
            var irp = (CurrencyResourcePack)obj;
            return irp.ItemResource == ItemResource && irp.Count == Count;
        }

        public override int GetHashCode()
        {
            var hashCode = -1492983518;
            hashCode = hashCode * -1521134295 + EqualityComparer<CurrencyResource>.Default.GetHashCode(ItemResource);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CurrencyResourcePack a, CurrencyResourcePack b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CurrencyResourcePack a, CurrencyResourcePack b) => !(a == b);

        public bool IsValid => ItemResource != null;
    }
}
