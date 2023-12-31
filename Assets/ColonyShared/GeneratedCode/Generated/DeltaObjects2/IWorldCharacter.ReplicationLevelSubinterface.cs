// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 564444504, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterAlways : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityAlways, IEntityObjectAlways, IHasMappedAlways, IScenicEntityAlways, IWorldObjectAlways, IHasAutoAddToWorldSpaceAlways, IHasWorldSpacedAlways, IHasInventoryAlways, IHasCurrencyAlways, IHasDialogEngineAlways, IHasDollAlways, IHasPerksAlways, IHasStatisticsAlways, IHasCraftEngineAlways, IHasWizardEntityAlways, IStatEntityAlways, IHasContainerApiAlways, ISpawnableAlways, IHasHealthWithCustomMechanicsAlways, IHasHealthAlways, IHasMortalAlways, IHasBruteAlways, IHasSpecificStatsAlways, IHasStatsEngineAlways, IHasMutationMechanicsAlways, IHasFactionAlways, IHasGenderAlways, IHasTraumasAlways, IHasPingDiagnosticsAlways, IHasItemsStatsAccumulatorAlways, IHasAuthorityOwnerAlways, IHasCharacterMovementSyncAlways, IHasLogableEntityAlways, IHasLocomotionOwnerAlways, IHasConsumerAlways, ICanGatherResourcesAlways, IHasQuestEngineAlways, IHasInputActionHandlersAlways, IHasReactionsOwnerAlways, IHasAttackEngineAlways, IHasAnimationDoerOwnerAlways, IHasFounderPackAlways, IHasSpatialDataHandlersAlways, IIsDummyLegionaryAlways, IHasBuffsAlways, IHasLinksEngineAlways, IHasFogOfWarAlways, IHasWorldObjectInformationSetsAlways, IHasWorldPersonalMachineEngineAlways, IHasAccountStatsAlways, IHasSpellModifiersAlways, IHasIncomingDamageMultiplierAlways, ICanGiveRewardForKillingMeAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<string> TestCheatRpc(string argument);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1015134027, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterClientBroadcast : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityClientBroadcast, IEntityObjectClientBroadcast, IHasMappedClientBroadcast, IScenicEntityClientBroadcast, IWorldObjectClientBroadcast, IHasAutoAddToWorldSpaceClientBroadcast, IHasWorldSpacedClientBroadcast, IHasInventoryClientBroadcast, IHasCurrencyClientBroadcast, IHasDialogEngineClientBroadcast, IHasDollClientBroadcast, IHasPerksClientBroadcast, IHasStatisticsClientBroadcast, IHasCraftEngineClientBroadcast, IHasWizardEntityClientBroadcast, IStatEntityClientBroadcast, IHasContainerApiClientBroadcast, ISpawnableClientBroadcast, IHasHealthWithCustomMechanicsClientBroadcast, IHasHealthClientBroadcast, IHasMortalClientBroadcast, IHasBruteClientBroadcast, IHasSpecificStatsClientBroadcast, IHasStatsEngineClientBroadcast, IHasMutationMechanicsClientBroadcast, IHasFactionClientBroadcast, IHasGenderClientBroadcast, IHasTraumasClientBroadcast, IHasPingDiagnosticsClientBroadcast, IHasItemsStatsAccumulatorClientBroadcast, IHasAuthorityOwnerClientBroadcast, IHasCharacterMovementSyncClientBroadcast, IHasLogableEntityClientBroadcast, IHasLocomotionOwnerClientBroadcast, IHasConsumerClientBroadcast, ICanGatherResourcesClientBroadcast, IHasQuestEngineClientBroadcast, IHasInputActionHandlersClientBroadcast, IHasReactionsOwnerClientBroadcast, IHasAttackEngineClientBroadcast, IHasAnimationDoerOwnerClientBroadcast, IHasFounderPackClientBroadcast, IHasSpatialDataHandlersClientBroadcast, IIsDummyLegionaryClientBroadcast, IHasBuffsClientBroadcast, IHasLinksEngineClientBroadcast, IHasFogOfWarClientBroadcast, IHasWorldObjectInformationSetsClientBroadcast, IHasWorldPersonalMachineEngineClientBroadcast, IHasAccountStatsClientBroadcast, IHasSpellModifiersClientBroadcast, IHasIncomingDamageMultiplierClientBroadcast, ICanGiveRewardForKillingMeClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool IsIdle
        {
            get;
        }

        bool IsAFK
        {
            get;
        }

        System.Guid AccountId
        {
            get;
        }

        System.Threading.Tasks.Task<string> TestCheatRpc(string argument);
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -912319459, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterClientFullApi : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityClientFullApi, IEntityObjectClientFullApi, IHasMappedClientFullApi, IScenicEntityClientFullApi, IWorldObjectClientFullApi, IHasAutoAddToWorldSpaceClientFullApi, IHasWorldSpacedClientFullApi, IHasInventoryClientFullApi, IHasCurrencyClientFullApi, IHasDialogEngineClientFullApi, IHasDollClientFullApi, IHasPerksClientFullApi, IHasStatisticsClientFullApi, IHasCraftEngineClientFullApi, IHasWizardEntityClientFullApi, IStatEntityClientFullApi, IHasContainerApiClientFullApi, ISpawnableClientFullApi, IHasHealthWithCustomMechanicsClientFullApi, IHasHealthClientFullApi, IHasMortalClientFullApi, IHasBruteClientFullApi, IHasSpecificStatsClientFullApi, IHasStatsEngineClientFullApi, IHasMutationMechanicsClientFullApi, IHasFactionClientFullApi, IHasGenderClientFullApi, IHasTraumasClientFullApi, IHasPingDiagnosticsClientFullApi, IHasItemsStatsAccumulatorClientFullApi, IHasAuthorityOwnerClientFullApi, IHasCharacterMovementSyncClientFullApi, IHasLogableEntityClientFullApi, IHasLocomotionOwnerClientFullApi, IHasConsumerClientFullApi, ICanGatherResourcesClientFullApi, IHasQuestEngineClientFullApi, IHasInputActionHandlersClientFullApi, IHasReactionsOwnerClientFullApi, IHasAttackEngineClientFullApi, IHasAnimationDoerOwnerClientFullApi, IHasFounderPackClientFullApi, IHasSpatialDataHandlersClientFullApi, IIsDummyLegionaryClientFullApi, IHasBuffsClientFullApi, IHasLinksEngineClientFullApi, IHasFogOfWarClientFullApi, IHasWorldObjectInformationSetsClientFullApi, IHasWorldPersonalMachineEngineClientFullApi, IHasAccountStatsClientFullApi, IHasSpellModifiersClientFullApi, IHasIncomingDamageMultiplierClientFullApi, ICanGiveRewardForKillingMeClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveAllItems(SharedCode.EntitySystem.PropertyAddress source, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> SavePerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> DisassemblyPerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> Break(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -161116020, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterClientFull : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityClientFull, IEntityObjectClientFull, IHasMappedClientFull, IScenicEntityClientFull, IWorldObjectClientFull, IHasAutoAddToWorldSpaceClientFull, IHasWorldSpacedClientFull, IHasInventoryClientFull, IHasCurrencyClientFull, IHasDialogEngineClientFull, IHasDollClientFull, IHasPerksClientFull, IHasStatisticsClientFull, IHasCraftEngineClientFull, IHasWizardEntityClientFull, IStatEntityClientFull, IHasContainerApiClientFull, ISpawnableClientFull, IHasHealthWithCustomMechanicsClientFull, IHasHealthClientFull, IHasMortalClientFull, IHasBruteClientFull, IHasSpecificStatsClientFull, IHasStatsEngineClientFull, IHasMutationMechanicsClientFull, IHasFactionClientFull, IHasGenderClientFull, IHasTraumasClientFull, IHasPingDiagnosticsClientFull, IHasItemsStatsAccumulatorClientFull, IHasAuthorityOwnerClientFull, IHasCharacterMovementSyncClientFull, IHasLogableEntityClientFull, IHasLocomotionOwnerClientFull, IHasConsumerClientFull, ICanGatherResourcesClientFull, IHasQuestEngineClientFull, IHasInputActionHandlersClientFull, IHasReactionsOwnerClientFull, IHasAttackEngineClientFull, IHasAnimationDoerOwnerClientFull, IHasFounderPackClientFull, IHasSpatialDataHandlersClientFull, IIsDummyLegionaryClientFull, IHasBuffsClientFull, IHasLinksEngineClientFull, IHasFogOfWarClientFull, IHasWorldObjectInformationSetsClientFull, IHasWorldPersonalMachineEngineClientFull, IHasAccountStatsClientFull, IHasSpellModifiersClientFull, IHasIncomingDamageMultiplierClientFull, ICanGiveRewardForKillingMeClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Refs.EntityRef<SharedCode.Entities.Engine.IBuildingEngine> BuildingEngine
        {
            get;
        }

        SharedCode.Refs.EntityRef<SharedCode.Entities.Engine.IKnowledgeEngine> KnowledgeEngine
        {
            get;
        }

        bool LastActivatedWasCommonBaken
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, long> ActivatedCommonBakens
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointMarker> PointMarkers
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef> PointsOfInterest
        {
            get;
        }

        bool IsIdle
        {
            get;
        }

        bool IsAFK
        {
            get;
        }

        System.Guid AccountId
        {
            get;
        }

        System.Threading.Tasks.Task SendChatMessage(string message);
        System.Threading.Tasks.Task<bool> Respawn(bool onBaken, bool anyCommonBaken, System.Guid commonBakenId);
        System.Threading.Tasks.ValueTask<bool> HasBaken();
        System.Threading.Tasks.ValueTask<bool> IsBakenActivated(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveAllItems(SharedCode.EntitySystem.PropertyAddress source, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> SavePerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> DisassemblyPerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> Break(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<bool> AddPerkSlot(SharedCode.EntitySystem.PropertyAddress source, int slotId, SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType);
        System.Threading.Tasks.Task<bool> CanAddPerkSlot(SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType);
        System.Threading.Tasks.Task AddUsedSlot(Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes);
        System.Threading.Tasks.Task RemoveUsedSlot(Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResult> CreateBuildElement(SharedCode.DeltaObjects.Building.BuildType type, System.Guid placeId, SharedCode.Aspects.Building.BuildRecipeDef buildRecipeDef, SharedCode.Aspects.Building.CreateBuildElementData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResultEx> OperateBuildElement(SharedCode.DeltaObjects.Building.BuildType type, System.Guid placeId, System.Guid elementId, SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResult> SetBuildCheat(SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResultEx> GetBuildCheat(SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task AddPointMarker(System.Guid pointMarkerGuid, Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointMarker pointMarker);
        System.Threading.Tasks.Task RemovePointMarker(System.Guid pointMarkerGuid);
        System.Threading.Tasks.Task AddPointOfInterest(Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef poi);
        System.Threading.Tasks.Task RemovePointOfInterest(Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef poi);
        System.Threading.Tasks.Task UnstuckTeleport(float minTimeout);
        System.Threading.Tasks.Task<string> TestCheatRpc(string argument);
        System.Threading.Tasks.Task Suicide();
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
        event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, System.Threading.Tasks.Task> ItemDroppedEvent;
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -487404166, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterServerApi : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityServerApi, IEntityObjectServerApi, IHasMappedServerApi, IScenicEntityServerApi, IWorldObjectServerApi, IHasAutoAddToWorldSpaceServerApi, IHasWorldSpacedServerApi, IHasInventoryServerApi, IHasCurrencyServerApi, IHasDialogEngineServerApi, IHasDollServerApi, IHasPerksServerApi, IHasStatisticsServerApi, IHasCraftEngineServerApi, IHasWizardEntityServerApi, IStatEntityServerApi, IHasContainerApiServerApi, ISpawnableServerApi, IHasHealthWithCustomMechanicsServerApi, IHasHealthServerApi, IHasMortalServerApi, IHasBruteServerApi, IHasSpecificStatsServerApi, IHasStatsEngineServerApi, IHasMutationMechanicsServerApi, IHasFactionServerApi, IHasGenderServerApi, IHasTraumasServerApi, IHasPingDiagnosticsServerApi, IHasItemsStatsAccumulatorServerApi, IHasAuthorityOwnerServerApi, IHasCharacterMovementSyncServerApi, IHasLogableEntityServerApi, IHasLocomotionOwnerServerApi, IHasConsumerServerApi, ICanGatherResourcesServerApi, IHasQuestEngineServerApi, IHasInputActionHandlersServerApi, IHasReactionsOwnerServerApi, IHasAttackEngineServerApi, IHasAnimationDoerOwnerServerApi, IHasFounderPackServerApi, IHasSpatialDataHandlersServerApi, IIsDummyLegionaryServerApi, IHasBuffsServerApi, IHasLinksEngineServerApi, IHasFogOfWarServerApi, IHasWorldObjectInformationSetsServerApi, IHasWorldPersonalMachineEngineServerApi, IHasAccountStatsServerApi, IHasSpellModifiersServerApi, IHasIncomingDamageMultiplierServerApi, ICanGiveRewardForKillingMeServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<bool> InvokeItemDropped(SharedCode.Aspects.Item.Templates.BaseItemResource item, int count);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> AddItems(System.Collections.Generic.List<SharedCode.Entities.ItemResourcePack> itemResourcesToAdd, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveAllItems(SharedCode.EntitySystem.PropertyAddress source, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> SavePerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> DisassemblyPerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> Break(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -2001424413, typeof(SharedCode.Entities.IWorldCharacter))]
    public interface IWorldCharacterServer : SharedCode.EntitySystem.IEntity, IPlayerPawnEntityServer, IEntityObjectServer, IHasMappedServer, IScenicEntityServer, IWorldObjectServer, IHasAutoAddToWorldSpaceServer, IHasWorldSpacedServer, IHasInventoryServer, IHasCurrencyServer, IHasDialogEngineServer, IHasDollServer, IHasPerksServer, IHasStatisticsServer, IHasCraftEngineServer, IHasWizardEntityServer, IStatEntityServer, IHasContainerApiServer, ISpawnableServer, IHasHealthWithCustomMechanicsServer, IHasHealthServer, IHasMortalServer, IHasBruteServer, IHasSpecificStatsServer, IHasStatsEngineServer, IHasMutationMechanicsServer, IHasFactionServer, IHasGenderServer, IHasTraumasServer, IHasPingDiagnosticsServer, IHasItemsStatsAccumulatorServer, IHasAuthorityOwnerServer, IHasCharacterMovementSyncServer, IHasLogableEntityServer, IHasLocomotionOwnerServer, IHasConsumerServer, ICanGatherResourcesServer, IHasQuestEngineServer, IHasInputActionHandlersServer, IHasReactionsOwnerServer, IHasAttackEngineServer, IHasAnimationDoerOwnerServer, IHasFounderPackServer, IHasSpatialDataHandlersServer, IIsDummyLegionaryServer, IHasBuffsServer, IHasLinksEngineServer, IHasFogOfWarServer, IHasWorldObjectInformationSetsServer, IHasWorldPersonalMachineEngineServer, IHasAccountStatsServer, IHasSpellModifiersServer, IHasIncomingDamageMultiplierServer, ICanGiveRewardForKillingMeServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Refs.EntityRef<SharedCode.Entities.Engine.IBuildingEngine> BuildingEngine
        {
            get;
        }

        SharedCode.Refs.EntityRef<SharedCode.Entities.Engine.IKnowledgeEngine> KnowledgeEngine
        {
            get;
        }

        bool LastActivatedWasCommonBaken
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, long> ActivatedCommonBakens
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointMarker> PointMarkers
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef> PointsOfInterest
        {
            get;
        }

        bool IsIdle
        {
            get;
        }

        bool IsAFK
        {
            get;
        }

        System.Guid SessionId
        {
            get;
        }

        System.Guid AccountId
        {
            get;
        }

        System.Threading.Tasks.Task SendChatMessage(string message);
        System.Threading.Tasks.Task<bool> Respawn(bool onBaken, bool anyCommonBaken, System.Guid commonBakenId);
        System.Threading.Tasks.ValueTask<bool> HasBaken();
        System.Threading.Tasks.ValueTask<bool> IsBakenActivated(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> bakenRef);
        System.Threading.Tasks.Task<bool> ActivateCommonBaken(System.Guid commonBakenGuid);
        System.Threading.Tasks.Task<bool> InvokeItemDropped(SharedCode.Aspects.Item.Templates.BaseItemResource item, int count);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> AddItems(System.Collections.Generic.List<SharedCode.Entities.ItemResourcePack> itemResourcesToAdd, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, int count, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> MoveAllItems(SharedCode.EntitySystem.PropertyAddress source, SharedCode.EntitySystem.PropertyAddress destination);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> RemoveItem(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, int count, System.Guid clientEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> SavePerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> DisassemblyPerk(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperationResult> Break(SharedCode.EntitySystem.PropertyAddress source, int sourceSlotId, System.Guid clientSrcEntityId);
        System.Threading.Tasks.Task<bool> AddPerkSlot(SharedCode.EntitySystem.PropertyAddress source, int slotId, SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType);
        System.Threading.Tasks.Task<bool> CanAddPerkSlot(SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType);
        System.Threading.Tasks.Task AddUsedSlot(Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes);
        System.Threading.Tasks.Task RemoveUsedSlot(Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes);
        System.Threading.Tasks.Task<bool> DropCorpse();
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResult> CreateBuildElement(SharedCode.DeltaObjects.Building.BuildType type, System.Guid placeId, SharedCode.Aspects.Building.BuildRecipeDef buildRecipeDef, SharedCode.Aspects.Building.CreateBuildElementData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResultEx> OperateBuildElement(SharedCode.DeltaObjects.Building.BuildType type, System.Guid placeId, System.Guid elementId, SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResult> SetBuildCheat(SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task<SharedCode.Entities.Building.OperationResultEx> GetBuildCheat(SharedCode.Entities.Building.OperationData data);
        System.Threading.Tasks.Task AddPointMarker(System.Guid pointMarkerGuid, Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointMarker pointMarker);
        System.Threading.Tasks.Task RemovePointMarker(System.Guid pointMarkerGuid);
        System.Threading.Tasks.Task AddPointOfInterest(Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef poi);
        System.Threading.Tasks.Task RemovePointOfInterest(Assets.ColonyShared.SharedCode.Aspects.WorldObjects.PointOfInterestDef poi);
        System.Threading.Tasks.Task<bool> NotifyThatClientIsGone();
        System.Threading.Tasks.Task<bool> NotifyThatClientIsBack();
        System.Threading.Tasks.Task UnstuckTeleport(float minTimeout);
        System.Threading.Tasks.Task<string> TestCheatRpc(string argument);
        System.Threading.Tasks.Task Suicide();
        System.Threading.Tasks.Task SuicideCheat();
        event System.Func<string, string, System.Threading.Tasks.Task> NewChatMessageEvent;
        event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, System.Threading.Tasks.Task> ItemDroppedEvent;
    }
}