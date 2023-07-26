using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Core.Cheats;
using GeneratorAnnotations;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.Aspects.Science;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Network;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.Server)]
    public interface ICheatServiceAgentEntity : IEntity
    {
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        [RemoteMethod(60)]
        Task<string> GetRepositoryEntitiesCount();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task DumpRepository();

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task ForceGC(int count);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task SetGCEnabled(bool enabled);
    }

    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Client)]
    public interface ICheatServiceEntity : IEntity
    {
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task AddSomeItems(List<ItemResourcePack> prototypeNames, PropertyAddress source);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task AddItemsInSlot(ItemResourcePack prototypeName, PropertyAddress source, int slot);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task AddQuest(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, Guid characterId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task AddTechPoints(TechPointCount[] techPointCounts, Guid characterId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task AddKnowledge(KnowledgeDef knowledgeDef, Guid characterId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task SpawnInteractiveObjectEntity(InteractiveEntityDef entityDef, Vector3 position);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task SpawnNewMineableEntity(MineableEntityDef entityDef, Vector3 position);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task SpawnInteractiveEntity(InteractiveEntityDef entityDef, Vector3 position);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task<string> GetRepositoryEntitiesCount();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task<string> GetRepositoryEntitiesCountOnAllRepositories();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task DumpAllServerRepositories();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        Task<string> SetVisibilityRadius(float enterRadius, float leaveRadius);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [RemoteMethod(60)]
        [CheatRpc(AccountType.GameMaster)]
        Task<string> GetTooLongEntityWaitQueues();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task<int> GetCCU();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetDebugMode(bool enabled);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetDebugMobs(bool enabledStatus, /*int minutes,*/ bool hard);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetDebugSpells(bool enabledStatus);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task PrintBrokenLocomotions();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        ValueTask DamageAllItems(Guid character, float percent);

        // --- CurveLogger -------------------------------------
        #region CurveLogger

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetDebugMobPositionLogging(OuterRef<IEntity> outerRef, bool enabledStatus, bool dump);


        /// <param name="enabledStatus">Activete|Inactivete</param>
        /// <param name="dump">SaveToFile if Inactivete</param>
        /// @param `OuterRef<IEntity> charRef` is needed to get unityId to fastforward call to the UnityCheatService.
        /// <param name="serverOnly">Don't trigger event on the clients replicas</param>
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetCurveLoggerState(OuterRef<IEntity> charRef, bool enabledStatus, bool dump, bool serverOnly, string loggerName, Guid dumpId);

        /// <summary>
        /// @param `OuterRef<IEntity> charRef` is needed to get unityId to fastforward call to the UnityCheatService.
        /// </summary>
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        event Func<OuterRef<IEntity> /*charRef*/, bool /*enabledStatus*/, bool /*dump*/, string /* loggerName */, Guid/* dumpId */, Task> SetCurveLoggerEvent;

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetLoggableEnable(OuterRef<IEntity> outerRef, bool enabledStatus);

    #endregion CurveLogger

        // Emulation GC on server
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task MainUnityThreadOnServerSleep(OuterRef<IEntity> charRef, bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task<bool> ChangeHealth(OuterRef<IEntity> victimEntity, int deltaValue);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task<bool> Godmode(OuterRef<IEntity> applicantEntityRef, bool enable);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.TechnicalSupport)]
        Task Version01();

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [CheatRpc(AccountType.GameMaster)]
        Task<Vector3[]> ResolveCharacterCoords(Guid[] guids);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task ForceGC(int count, Guid repositoryId);
        
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [Cheat]
        Task ForceSelfCompactionGC();

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.Server)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetGCEnabled(bool enabled, Guid repositoryId);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task CastSpell(OuterRef<IEntity> entityRef, SpellCast spellCast);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetServerCheatVariable(BaseResource resource, string value);

    #region Test
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [CheatRpc(AccountType.GameMaster)]
        Task<OuterRef<IEntity>> TestCheckPZ15200Done(float waitBeforeReplicate);
    #endregion Test

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task EnableWizardLogger(OuterRef entity, bool enable);
        
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task SetGender(OuterRef entity, GenderDef gender);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task InvokeTrauma(OuterRef entity, string trauma);

        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [CheatRpc(AccountType.GameMaster)]
        Task StopTrauma(OuterRef entity, string trauma);
    }
}
