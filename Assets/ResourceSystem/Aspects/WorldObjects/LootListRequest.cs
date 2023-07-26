using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ProtoBuf;

namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{
    [ProtoContract]
    public class LootListRequest
    {
        [ProtoMember(1)]
        public DamageTypeDef DamageType;
        [ProtoMember(2)]
        public float Damage;
        [ProtoMember(3)]
        public System.Guid Requester;

        public LootListRequest(DamageTypeDef dmgType, float dmg, System.Guid requester)
        {
            DamageType = dmgType;
            Damage = dmg;
            Requester = requester;
        }

        public LootListRequest(System.Guid requester)
        {
            Requester = requester;
        }
    }
}
