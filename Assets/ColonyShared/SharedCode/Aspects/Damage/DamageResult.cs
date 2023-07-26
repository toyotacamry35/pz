using ProtoBuf;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage
{
    [ProtoContract]
    public struct DamageResult
    {
        public static readonly DamageResult None = new DamageResult();
        
        [ProtoMember(1)] public float _damage;
        [ProtoMember(2)] public bool _applied;

        [ProtoIgnore]
        public float Damage => _damage;

        [ProtoIgnore]
        public bool IsApplied => _applied;

        public DamageResult(float damage)
        {
            _damage = damage;
            _applied = true;
        }
    }
}