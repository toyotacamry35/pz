using System;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using JetBrains.Annotations;
using ProtoBuf;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage
{
    [ProtoContract]
    public struct Damage
    {
        [ProtoMember(1)] public float _battleDamage;
        [ProtoMember(2)] public DamageTypeDef _damageType;
        [ProtoMember(3)] public bool _isMiningDamage;
        [ProtoMember(4)] public float _miningLootMultiplier;
        [ProtoMember(5)] public Vector3 _aggressionPoint;
        [ProtoMember(6)] public OuterRef _aggressor;
        

        [ProtoIgnore]
        public float BattleDamage => _battleDamage;

        [ProtoIgnore]
        public DamageTypeDef DamageType => _damageType;

        [ProtoIgnore]
        public bool IsMiningDamage => _isMiningDamage;

        [ProtoIgnore]
        public float MiningLootMultiplier => _miningLootMultiplier;

        [ProtoIgnore]
        public Vector3 AggressionPoint => _aggressionPoint;

        [ProtoIgnore]
        public bool HasAggressionPoint => _aggressionPoint != Vector3.Default;

        [ProtoIgnore]
        public OuterRef Aggressor => _aggressor;
 
        public Damage(
            float battleDamage,
            [NotNull] DamageTypeDef damageType,
            bool isMiningDamage,
            float miningLootMultiplier,
            OuterRef aggressor)
        {
            if (damageType == null) throw new ArgumentNullException(nameof(damageType));
            _battleDamage = battleDamage;
            _damageType = damageType;
            _isMiningDamage = isMiningDamage;
            _miningLootMultiplier = miningLootMultiplier;
            _aggressionPoint = Vector3.Default;
            _aggressor = aggressor;
        }

        public Damage(
            float battleDamage,
            [NotNull] DamageTypeDef damageType,
            bool isMiningDamage,
            float miningLootMultiplier,
            Vector3 aggressionPoint,
            OuterRef aggressor)
        {
            if (damageType == null) throw new ArgumentNullException(nameof(damageType));
            _battleDamage = battleDamage;
            _damageType = damageType;
            _isMiningDamage = isMiningDamage;
            _miningLootMultiplier = miningLootMultiplier;
            _aggressionPoint = aggressionPoint;
            _aggressor = aggressor;
        }

        public override string ToString()
        {
            var sb = StringBuildersPool.Get
                .Append("Dmg:").Append(_battleDamage)
                .Append(" Typ:").Append(_damageType)
                .Append("isMngDmd:").Append(_isMiningDamage)
                .Append("LootMul:").Append(_miningLootMultiplier);
            if (HasAggressionPoint)
                sb.Append(" Pnt:").Append(_aggressionPoint);
            if (Aggressor.IsValid)
                sb.Append(" Aggressor:").Append(_aggressor);
            return sb.ToStringAndReturn();
        }
    }
}