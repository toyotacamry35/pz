using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ColonyShared.SharedCode.Aspects.Statistic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using SharedCode.DeltaObjects;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;
using SharedCode.Utils;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Aspects.Impl.Factions.Template;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.MovementSync;
using SharedCode.Entities.Service;
using ResourceSystem.Aspects.AccessRights;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Science;
using MongoDB.Bson.Serialization.Attributes;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using GeneratedCode.Repositories;

namespace SharedCode.Entities
{
    public static class PositionedObjectHelper
    {
        public static Vector3? GetPositionLockfree(IEntitiesRepository repo, OuterRef<IEntity> ent)
        {
            IPositionedObjectAlways positionedEntity = null;
            positionedEntity = positionedEntity ?? repo.TryGetLockfree<IHasMobMovementSyncAlways>(ent, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? repo.TryGetLockfree<IHasCharacterMovementSyncAlways>(ent, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? repo.TryGetLockfree<IHasSimpleMovementSyncAlways>(ent, ReplicationLevel.Always)?.MovementSync;
            return positionedEntity?.Position;
        }
        public static Vector3? GetPosition(IEntitiesContainer wrapper, int typeId, Guid id)
        {
            IPositionedObjectAlways positionedEntity = null;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasMobMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasCharacterMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasSimpleMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            return positionedEntity?.Position;
        }
        public static IPositionedObjectAlways GetPositioned(IEntitiesContainer wrapper, int typeId, Guid id)
        {
            if (typeId == 0 || id == Guid.Empty)
                return null;
            IPositionedObjectAlways positionedEntity = null;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasMobMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasCharacterMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasSimpleMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            return positionedEntity;
        }
        public static IPositionedObject GetPositionedMaster(IEntitiesContainer wrapper, int typeId, Guid id)
        {
            IPositionedObject positionedEntity = null;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasMobMovementSync>(typeId, id, ReplicationLevel.Master)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasCharacterMovementSync>(typeId, id, ReplicationLevel.Master)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasSimpleMovementSync>(typeId, id, ReplicationLevel.Master)?.MovementSync;
            return positionedEntity;
        }
        public static IPositionedObject GetPositioned(IEntity entity)
        {
            IPositionedObject positionedEntity = null;
            positionedEntity = positionedEntity ?? (entity as IHasMobMovementSync)?.MovementSync;
            positionedEntity = positionedEntity ?? (entity as IHasCharacterMovementSync)?.MovementSync;
            positionedEntity = positionedEntity ?? (entity as IHasSimpleMovementSync)?.MovementSync;
            return positionedEntity;
        }

        public static IPositionableObjectAlways GetPositionable(IEntitiesContainer wrapper, int typeId, Guid id)
        {
            IPositionableObjectAlways positionedEntity = null;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasMobMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasCharacterMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            positionedEntity = positionedEntity ?? wrapper.Get<IHasSimpleMovementSyncAlways>(typeId, id, ReplicationLevel.Always)?.MovementSync;
            return positionedEntity;
        }
        public static IPositionableObject GetPositionable(IEntity entity)
        {
            IPositionableObject positionedEntity = null;
            positionedEntity = positionedEntity ?? (entity as IHasMobMovementSync)?.MovementSync;
            positionedEntity = positionedEntity ?? (entity as IHasCharacterMovementSync)?.MovementSync;
            positionedEntity = positionedEntity ?? (entity as IHasSimpleMovementSync)?.MovementSync;
            return positionedEntity;
        }
    }

    public interface IColliderObject
    {
        [RuntimeData(SkipField = true)]
        Vector3 Collider { get; }
    }

    public interface IPositionedObject
    {
        [RuntimeData(SkipField = true)]
        Transform Transform { get; }

        [RuntimeData(SkipField = true)]
        Vector3 Position { get; }
        
        [RuntimeData(SkipField = true)]
        Quaternion Rotation { get; }
        
        [RuntimeData(SkipField = true)]
        Vector3 Scale { get; }

        [ReplicationLevel(ReplicationLevel.Master), RuntimeData(SkipField = true)]
        IPositionHistory History { get; }
    }

    public interface IPositionableObject
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Master)]
        Transform SetTransform { set; }
        
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Master)]
        Vector3 SetPosition { set; }

        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Master)]
        Quaternion SetRotation { set; }

        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Master)]
        Vector3 SetScale { set; }
    }

    public interface IPositionHistory
    {
        Transform GetTransformAt(long timestamp);
    }
    
    public interface IWorldObject : IHasAutoAddToWorldSpace, IHasWorldSpaced
    {
        string Name { get; set; }
        string Prefab { get; set; }
        ISaveableResource SomeUnknownResourceThatMayBeUseful { get; set; }

        OnSceneObjectNetId OnSceneObjectNetId { get; set; }

        Task<bool> NameSet(string value);
        Task<bool> PrefabSet(string value);
    }

    public interface IHasAutoAddToWorldSpace
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IAutoAddToWorldSpace AutoAddToWorldSpace { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IAutoAddToWorldSpace : IDeltaObject
    {
    }

    [ProtoContract]
    public struct OnSceneObjectNetId : IEquatable<OnSceneObjectNetId>
    {
        [ProtoMember(1)] public bool IsValid;
        [ProtoMember(2)] public int SceneHash;
        [ProtoMember(3)] public int IndexInList;

        public static readonly OnSceneObjectNetId Invalid = new OnSceneObjectNetId(false, 0, 0);

        public OnSceneObjectNetId(bool isValid, int sceneHash, int indexInList)
        {
            IsValid = isValid;
            SceneHash = sceneHash;
            IndexInList = indexInList;
        }

        public static bool operator ==(OnSceneObjectNetId a, OnSceneObjectNetId b)
        {
            if (!a.IsValid && !b.IsValid)
                return true;
            return a == b;
        }

        public static bool operator !=(OnSceneObjectNetId a, OnSceneObjectNetId b)
        {
            return !(a == b);
        }

        public bool Equals(OnSceneObjectNetId other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is OnSceneObjectNetId && Equals((OnSceneObjectNetId)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 1179047484;
            hashCode = hashCode * -1521134295 + IsValid.GetHashCode();
            hashCode = hashCode * -1521134295 + SceneHash.GetHashCode();
            hashCode = hashCode * -1521134295 + IndexInList.GetHashCode();
            return hashCode;
        }
    }

    public interface IHasInventory
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IContainer Inventory { get; }
    }

    public interface ICanGatherResources
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        event Func<OuterRef<IEntity>, List<ItemResourcePack>, Task> GatherResourcesEvent;

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task GatherResources(OuterRef<IEntity> giver, List<ItemResourcePack> items);
    }

    public interface IHasLimitedLifetime : IHasDestroyable
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ILimitedLifetime LimitedLifetime { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILimitedLifetime : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ChainCancellationToken CountdownToken { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        long CountdownStartTimestamp { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        long LifetimeLimit { get; set; }

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<LimitedLifetimeDef> GetLimitedLifetimeDef();

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<long> GetLifetimeLimit(); //����� ������� (�� ������� ��� ���)

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<ChainCancellationToken> StartCountdown(); //����� ��������
    }

    [ProtoContract]
    public class RemoveItemBatchElement
    {
        [ProtoMember(1)]
        public PropertyAddress Source { get; set;}

        [ProtoMember(2)]
        public int SourceSlotId { get; set;}

        [ProtoMember(3)]
        public int Count { get; set;}

        [ProtoMember(4)]
        public Guid ClientEntityId { get; set;}

        public RemoveItemBatchElement()
        {
        }

        public RemoveItemBatchElement(PropertyAddress source, int sourceSlotId, int count, Guid clientEntityId)
        {
            Source = source;
            SourceSlotId = sourceSlotId;
            Count = count;
            ClientEntityId = clientEntityId;
        }
    }

    [ProtoContract]
    public class IItemWrapper
    {
        [ProtoMember(1, AsReference = true, DynamicType = true)]
        public IItem Item { get; set; }

        [ProtoMember(2)]
        public int Count { get; set; }
    }

    [ProtoContract]
    public class ContainerOperationRemovePrepareResult
    {
        [ProtoMember(1)]
        public ContainerItemOperationResult ContainerItemOperationResult { get; set; }

        [ProtoMember(2, AsReference = true, DynamicType = true)]
        public IItem Item { get; set; }
    }

    [ProtoContract]
    public class ContainerOperationAddPrepareResult
    {
        [ProtoMember(1)]
        public ContainerItemOperationResult ContainerItemOperationResult { get; set; }

        [ProtoMember(2)]
        public int Count { get; set; }

        [ProtoMember(3, AsReference = true, DynamicType = true)]
        public IItem Item { get; set; }
    }

    [ProtoContract]
    public class ContainerOperationMoveAllRemovePrepareResult
    {
        [ProtoMember(1)]
        public ContainerItemOperationResult ContainerItemOperationResult { get; set; }

        [ProtoMember(2, AsReference = true, DynamicType = true)]
        public Dictionary<int, IItemWrapper> Items { get; set; }
    }

    [ProtoContract]
    public class ContainerOperationMoveAllAddPrepareResult
    {
        [ProtoMember(1)]
        public ContainerItemOperationResult ContainerItemOperationResult { get; set; }

        [ProtoMember(2, AsReference = true, DynamicType = true)]
        public Dictionary<int, IItemWrapper> Items { get; set; }
    }

    public interface IHasDoll
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ICharacterDoll Doll { get; }
    }

    public interface IHasCurrency
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] ICurrencyContainer Currency { get; }

        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperationResult> ChangeCurrencies(List<CurrencyResourcePack> currencies);

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal), ReplicationLevel(ReplicationLevel.ClientFull)] Task<uint> GetCurrencyValue(CurrencyResource currency);
    }

    public interface IHasPerks
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ITemporaryPerks TemporaryPerks { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IPermanentPerks PermanentPerks { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ISavedPerks SavedPerks { get; }
    }

    public interface IHasStatistics
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<StatisticType, IDeltaDictionary<StatisticType, float>> Statistics { get; } // ObjectType, StatisticType, Values

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaDictionary<BaseItemResource, int> PerksDestroyCount { get; }

        PerkActionsPricesDef PerkActionsPrices { get; } // TODOA

        [ReplicationLevel(ReplicationLevel.Server)]
        Task ChangeStatistic(StatisticType statistic, StatisticType target, float value);

        [ReplicationLevel(ReplicationLevel.Server)]
        IStatisticEngine StatisticEngine { get; }
    }

    public interface IHasOpenMechanics
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IOpenMechanics OpenMechanics { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IOpenMechanics : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)] event Func<bool, Task> FirstOpenedOrLastClosed;
        [ReplicationLevel(ReplicationLevel.Master)] Task FirstOpenedOrLastClosedExternalCall(bool isOpened);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<OuterRef> TryOpen(OuterRef outerRef);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<bool> TryClose(OuterRef outerRef);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] bool IsOpen { get; set; }
        [ReplicationLevel(ReplicationLevel.Master), EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] ValueTask<bool> IsEmpty();
    }

    public interface IHasTraumas
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ITraumas Traumas { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ITraumas : IDeltaObject
    {
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaDictionary<string, ITraumaGiver> ActiveTraumas { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<string, ITraumaGiver> SaveableActiveTraumas { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        Task ChangeTraumaPoints(string traumaKey, int delta);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task StartTrauma(string traumaKey);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task StopTrauma(string traumaKey);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task StartTrauma(string traumaKey, ITraumaGiver traumaGiver);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task StopTrauma(string traumaKey, ITraumaGiver traumaGiver);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task SuspendTrauma(string traumaKey, ITraumaGiver traumaGiver);

        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<bool> RemoveTrauma(string traumaKey, ITraumaGiver traumaGiver);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<bool> HasActiveTraumas(string[] traumas);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RecalculateTraumas();
    }


 

    public interface IMountable : IEntityObject, IWorldObject, IHasSimpleMovementSync, IHasSpecificStats
    {
    }

    public interface ICanBeActive
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        bool IsActive { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<RecipeOperationResult> SetActive(bool activate);
    }

    public interface IHasCraftEngine
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        EntityRef<ICraftEngine> CraftEngine { get; set; }
    }

    public interface ISpawnable
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        SpawnPointTypeDef AllowedSpawnPoint { get; set; }

        Task<bool> AllowedSpawnPointSet(SpawnPointTypeDef value);
    }

    public interface IHasOwner
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        IOwnerInformation OwnerInformation { get;set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IOwnerInformation : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        OuterRef<IEntity> Owner { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        AccessPredicateDef AccessPredicate { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        AccessPredicateDef LockPredicate { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetOwner(OuterRef<IEntity> owner);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetLockPredicate(AccessPredicateDef accessPredicate);
    }

    public interface IHasCraftProgressInfo
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ICraftProgressInfo CraftProgressInfo { get; set; }

        Task<bool> CraftProgressInfoSet(ICraftProgressInfo value);
    }

    [ProtoContract]
    public struct StatModifierInfo
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [ProtoMember(1)] public StatResource Stat { get; set; }
        [ProtoMember(2)] public StatModifierType ModifierType { get; set; }

        public StatModifierInfo(StatResource stat, StatModifierType modifierType)
        {
            if (stat == null)
            {
                Logger.IfError()?.Message("StatModifierData Stat is null").Write();
                throw new Exception("StatModifierData Stat is null");
            }

            Stat = stat;
            ModifierType = modifierType;
        }

        public override string ToString()
        {
            return $"{Stat.DebugName} (mod:{ModifierType})";
        }

        public static string ToString(IEnumerable<StatModifierInfo> modifiers)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var modifier in modifiers)
                sb.Append(modifier + "\n");
            return sb.ToString();
        }
    }
    
    [ProtoContract]
    public struct StatModifierData
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [ProtoMember(1)] public StatResource Stat { get; set; }
        [ProtoMember(2)] public StatModifierType ModifierType { get; set; }
        [ProtoMember(3)] public float Value { get; set; }

        public StatModifierData(StatResource stat, StatModifierType modifierType, float value)
        {
            if (stat == null)
            {
                Logger.IfError()?.Message("StatModifierData Stat is null").Write();
                throw new Exception("StatModifierData Stat is null");
            }

            Stat = stat;
            ModifierType = modifierType;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Stat.DebugName} (mod:{ModifierType}) = {Value}";
        }

        public static string ToString(IEnumerable<StatModifierData> modifiers)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var modifier in modifiers)
                sb.Append(modifier + "\n");

            return sb.ToString();
        }

    }

    public interface IHasLocomotionOwner
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ILocomotionOwner LocomotionOwner { get; set; }
    }
    
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ILocomotionOwner : IDeltaObject
    {
        /// Присутствует (не null) только на client с authority (для персонажа) или master (для мобов)
        [RuntimeData,ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        ILocomotionEngineAgent Locomotion { get; set; }
        
        /// Присутствует (не null) только на client с authority (для персонажа) или master (для мобов)
        [RuntimeData,ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDirectMotionProducer DirectMotionProducer { get; set; }

        /// Присутствует (не null) только на client с authority (для персонажа) или master (для мобов)
        [RuntimeData,ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IGuideProvider GuideProvider { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<bool> IsValid();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask SetLocomotion(ILocomotionEngineAgent locomotion, IDirectMotionProducer directMotionProducer, IGuideProvider guideProvider);
    }

    public interface IHasGender
    {
        [ReplicationLevel(ReplicationLevel.Always)] GenderDef Gender { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask SetGender(GenderDef gender);
    }
}

