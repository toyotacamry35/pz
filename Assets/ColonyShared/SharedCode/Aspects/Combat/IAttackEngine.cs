using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Modifiers;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ProtoBuf;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Aspects;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Aspects.Combat
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.None)]
    public interface IAttackEngine : IDeltaObject
    {
        /// <summary>
        /// Текущие открытые атаки
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Master), BsonIgnore] IDeltaList<AttackInfo> Attacks { get; set; }
        
        /// <summary>
        /// Цели которые пришли в PushTargets, возможно, до того как началась их атака. 
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Master), BsonIgnore] IDeltaList<AttackOrphanTargetInfo> OrphanTargets { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> StartAttack(SpellPartCastId attackId, long finishTime, AttackDef attackDef, IReadOnlyList<AttackModifierDef> modifiers);

        [ReplicationLevel(ReplicationLevel.Master)] Task FinishAttack(SpellPartCastId attackId, long currentTime);
        
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task PushAttackTargets(SpellPartCastId attackId, List<AttackTargetInfo> targets);

        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull)]
        IAttackDoer AttackDoer { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task SetAttackDoer(IAttackDoer newDoer);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task UnsetAttackDoer(IAttackDoer oldDoer);
    }


    [ProtoContract]
    public struct AttackTargetInfo
    {
        [ProtoMember(1)]
        public long Timestamp;
        [ProtoMember(2)]
        public OuterRef Target;
        [ProtoMember(3)]
        public Guid SubTarget;
        [ProtoMember(4)]
        public Vector3 HitPoint;
        [ProtoMember(5)]
        public Vector3 HitLocalPoint;
        [ProtoMember(6)]
        public Quaternion HitRotation;
        [ProtoMember(7)]
        public Quaternion HitLocalRotation;
        [ProtoMember(8)]
        public string HitObject;

        public AttackTargetInfo(AttackTargetInfo oth)
        {
            this = oth;
        }

        public override string ToString() => $"[Target:{Target.ToString()}{(SubTarget!=Guid.Empty ? $" SubTarget:{SubTarget.ToString()}":string.Empty)} Timestamp:{Timestamp.ToString()} HitPoint:{HitPoint.ToString()} HitObject:{HitObject} HitLocalPoint:{HitLocalPoint.ToString()}]";
    }
    
    
    // Сделано не DeltaObject'ом так как дельта-репликация для этих данных не нужна. Но, из-за этого он должен быть immutable! 
    // Сделано не структурой, так как хранить в DeltaList'ах структуры, выходит дороже чем классы.
    [ProtoContract]
    public class AttackInfo 
    {
        [ProtoMember(1)]
        public readonly SpellPartCastId Id;
        [ProtoMember(2)]
        public readonly AttackDef Def;
        [ProtoMember(3)]
        public readonly long FinishTime;
        [ProtoMember(4)]
        public readonly int ProcessedTargetsCount;
        [ProtoMember(5)]
        public readonly AttackModifierDef[] Modifiers;

        public AttackInfo(AttackDef attackDef, SpellPartCastId id, IReadOnlyList<AttackModifierDef> modifiers, long finishTime, int processedTargetsCount = 0)
        {
            Def = attackDef;
            Id = id;
            FinishTime = finishTime;
            ProcessedTargetsCount = processedTargetsCount;
            Modifiers = (modifiers as AttackModifierDef[]) ?? modifiers?.ToArray();
        } 
        public AttackInfo(AttackInfo n, long finishTime) : this(n) { FinishTime = finishTime; }
        public AttackInfo(AttackInfo n, int processedTargetsCount) : this(n) { ProcessedTargetsCount = processedTargetsCount; }
        private AttackInfo(AttackInfo n) : this(n.Def, n.Id, n.Modifiers, n.FinishTime, n.ProcessedTargetsCount) {}
        [UsedImplicitly] public AttackInfo() {} // для protobuf
        public override string ToString() => $"[Id:{Id} Def:{Def.____GetDebugAddress()} FinishTime:{FinishTime} Processed:{ProcessedTargetsCount} Modifiers:[{string.Join(", ", Modifiers.Select(x => x.____GetDebugShortName()))}]]";
    }
    
    
    public class AttackOrphanTargetInfo
    {
        public readonly AttackTargetInfo TargetInfo;
        public readonly SpellPartCastId AttackId;

        public AttackOrphanTargetInfo(AttackTargetInfo targetInfo, SpellPartCastId attackId)
        {
            TargetInfo = targetInfo;
            AttackId = attackId;
        }
        [UsedImplicitly] public AttackOrphanTargetInfo() {} // для protobuf
        public override string ToString() => $"[AttackId:{AttackId} Target:{TargetInfo}]";        
    }
}

