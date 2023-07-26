using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.DeltaObjects;
using SharedCode.Entities.Building;
using SharedCode.Entities.Engine;
using SharedCode.Entities.Regions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;
using SharedCode.Wizardry;
using SharedCode.Entities;
using System;
using SharedCode.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Entities.Items;
using SharedCode.Aspects.Item.Templates;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.Aspects.Doings;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Entities.Reactions;
using SharedCode.MovementSync;
using GeneratorAnnotations;
using SharedCode.AI;
using MongoDB.Bson.Serialization.Attributes;
using Assets.ColonyShared.SharedCode.Wizardry;
using ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Modifiers;
using SharedCode.Entities.Service;
using SharedCode.FogOfWar;
using Core.Cheats;
using GeneratedCode.DeltaObjects;
using ResourceSystem.Aspects.Misc;

namespace SharedCode.Entities
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IWorldCharacter : IEntity, IPlayerPawnEntity, IHasInventory, IHasCurrency, IHasDialogEngine,
        IHasDoll, IHasPerks, IHasStatistics, IHasCraftEngine, IHasWizardEntity,
        IStatEntity, IHasContainerApi, ISpawnable,   
        IHasHealthWithCustomMechanics, IHasMortal, IHasBrute, IHasStatsEngine,
        IHasMutationMechanics, IHasGender, IHasTraumas, IHasPingDiagnostics, IHasItemsStatsAccumulator, IHasAuthorityOwner, IHasCharacterMovementSync, 
        IHasLocomotionOwner, IHasConsumer, ICanGatherResources,
        IHasQuestEngine, IHasInputActionHandlers, IHasReactionsOwner, IHasAttackEngine, IHasAnimationDoerOwner, IHasFounderPack, 
        IHasSpatialDataHandlers, IIsDummyLegionary, IHasBuffs, IHasLinksEngine, IHasFogOfWar, IHasWorldObjectInformationSets, IHasWorldPersonalMachineEngine,
		IHasAccountStats, IHasSpellModifiers, IHasIncomingDamageMultiplier, ICanGiveRewardForKillingMe
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]      event Func<string, string, Task> NewChatMessageEvent;
        [ReplicationLevel(ReplicationLevel.Master)] Task InvokeNewChatMessageEvent(string name, string message);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task SendChatMessage(string message);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> Respawn(bool onBaken, bool anyCommonBaken, Guid commonBakenId);

        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] [ReplicationLevel(ReplicationLevel.ClientFull)] ValueTask<bool> HasBaken();
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] [ReplicationLevel(ReplicationLevel.ClientFull)] ValueTask<bool> IsBakenActivated(OuterRef<IEntity> bakenRef);
        
        [ReplicationLevel(ReplicationLevel.ClientFull)] Refs.EntityRef<IBuildingEngine> BuildingEngine { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] Refs.EntityRef<IKnowledgeEngine> KnowledgeEngine { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)] bool                LastActivatedWasCommonBaken { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<Guid, long> ActivatedCommonBakens { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<Guid, PointMarker> PointMarkers { get; }

        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> ActivateCommonBaken(Guid commonBakenGuid);

        #region items management
        [ReplicationLevel(ReplicationLevel.ClientFull)] event Func<BaseItemResource, int, Task> ItemDroppedEvent;
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> InvokeItemDropped(BaseItemResource item, int count);

        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> AddItems(List<ItemResourcePack> itemResourcesToAdd, PropertyAddress destination);
        [ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperation> MoveItem(PropertyAddress source, int sourceSlotId, PropertyAddress destination, int destinationSlotId, int count, Guid clientSrcEntityId);
        [Obsolete, ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperation> MoveAllItems(PropertyAddress source, PropertyAddress destination);
        [ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperation> RemoveItem(PropertyAddress source, int sourceSlotId, int count, Guid clientEntityId);

        [ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperationResult> SavePerk(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId);
        [ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperationResult> DisassemblyPerk(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId);
        [ReplicationLevel(ReplicationLevel.ClientFullApi)] Task<ContainerItemOperationResult> Break(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId);

        #endregion

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> AddPerkSlot(PropertyAddress source, int slotId, ItemTypeResource perkSlotType);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> CanAddPerkSlot(ItemTypeResource perkSlotType);

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task AddUsedSlot(ResourceIDFull dollSlotRes);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task RemoveUsedSlot(ResourceIDFull dollSlotRes);
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> DropCorpse();
        
        #region build element management
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<OperationResult> CreateBuildElement(BuildType type, Guid placeId, BuildRecipeDef buildRecipeDef, CreateBuildElementData data);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<OperationResultEx> OperateBuildElement(BuildType type, Guid placeId, Guid elementId, OperationData data);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<OperationResult> SetBuildCheat(OperationData data);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<OperationResultEx> GetBuildCheat(OperationData data);
        #endregion

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task AddPointMarker(Guid pointMarkerGuid, PointMarker pointMarker);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task RemovePointMarker(Guid pointMarkerGuid);

        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaList<PointOfInterestDef> PointsOfInterest { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task AddPointOfInterest(PointOfInterestDef poi);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task RemovePointOfInterest(PointOfInterestDef poi);

        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> NotifyThatClientIsGone();
        [ReplicationLevel(ReplicationLevel.Server)] Task<bool> NotifyThatClientIsBack();

        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.Master)] long TimeWhenUserDisconnected { get; set; }
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] bool IsIdle { get; set; }
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] bool IsAFK { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> AFKStateMachineChainCall();

        [ReplicationLevel(ReplicationLevel.Master)] event Func<Task> OnIdleModeStarted;
        [ReplicationLevel(ReplicationLevel.Master)] event Func<Task> OnIdleModeStopped;

        [ReplicationLevel(ReplicationLevel.Master)] Task OnBeforeResurrectEvent(Guid id, int typeId);

        [BsonIgnore]
        [LockFreeReadonlyProperty]
        [ReplicationLevel(ReplicationLevel.Server)] Guid SessionId { get; set; }
        [BsonIgnore]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Guid AccountId { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task UnstuckTeleport(float minTimeout);
        [ReplicationLevel(ReplicationLevel.Master)] Task UnstuckTeleportDo();
        [ManualCheatRpc(AccountType.User)]Task<string> TestCheatRpc(string argument);
        
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task Suicide();

        [Cheat]
        [CheatRpc(AccountType.GameMaster)]
        [ReplicationLevel(ReplicationLevel.Server)] Task SuicideCheat();

        [Cheat]
        [CheatRpc(AccountType.TechnicalSupport)]
        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<int> ResyncAccountExperience();
    }
}
