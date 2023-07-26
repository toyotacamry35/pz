using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.CustomData;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.GeneratedCode.Shared.Aspects;
using GeneratedCode.Custom.Containers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using GeneratedCode.Transactions;
using NLog;
using NLog.Fluent;
using SharedCode.Aspects.Science;
using SharedCode.Cloud;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Mineable;
using SharedCode.EntitySystem;
using SharedCode.OurSimpleIoC;
using SharedCode.Utils;
using Src.Aspects.Impl.Stats;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode.Utils;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;
using GeneratedCode.EntityModel.Test;
using ResourceSystem.Aspects.Misc;
using SharedCode.Serializers.Protobuf;

namespace GeneratedCode.DeltaObjects
{

    public partial class CheatServiceAgentEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<string> GetRepositoryEntitiesCountImpl()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<repository configId:{0} num:{1} type:{2} id:{3}>", ((IEntitiesRepositoryExtension)this.EntitiesRepository).RepositoryConfigId,
                ((IEntitiesRepositoryExtension)this.EntitiesRepository).RepositoryNum, this.EntitiesRepository.CloudNodeType, this.Id).AppendLine();
            sb.AppendLine(((EntitiesRepository)EntitiesRepository).GetObjectsCount());
            return Task.FromResult<string>(sb.ToString());
        }

        public async Task DumpRepositoryImpl()
        {
            var fileName = ((IEntitiesRepositoryExtension)EntitiesRepository).RepositoryConfigId + EntitiesRepository.Id.ToString().Replace("-", "");
            await RepositoryCommunicationEntity.Dump(fileName, EntitiesRepository, this.ToString());
        }

        public Task ForceGCImpl(int count)
        {
            Logger.IfInfo()?.Message("Repository {0} try forced GC.Collect {1} count", EntitiesRepository.Id, count).Write();
            for (int i = 0; i < count; i++)
                GC.Collect();
            Logger.IfInfo()?.Message("Repository {0} finished GC.Collect {1} count", EntitiesRepository.Id, count).Write();
            return Task.CompletedTask;
        }

        public Task SetGCEnabledImpl(bool enabled)
        {
            Logger.IfInfo()?.Message("Repository {0} enable GC {1}", EntitiesRepository.Id, enabled).Write();
            EntitytObjectsUnitySpawnService.SpawnService?.SetGCEnabled(enabled);
            return Task.CompletedTask;
        }
    }

    public partial class CheatServiceEntity : IHookOnReplicationLevelChanged
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask DamageAllItemsImpl(Guid character, float percent)
        {
            using (var container = await EntitiesRepository.Get<IWorldCharacterServer>(character))
            {
                var destEntity = container.Get<IWorldCharacterServer>(character);
                if (destEntity == null)
                {
                    Logger.IfError()?.Message("Destination entity not found {0}", character).Write();
                    return;
                }

                foreach(var item in destEntity.Doll.Items)
                {
                    var maxHealth = await item.Value.Item.Health.GetMaxHealth();
                    await item.Value.Item.Health.ChangeHealth(maxHealth * percent);
                }
            }
        }

        public async Task AddSomeItemsImpl(List<ItemResourcePack> prototypeNames, PropertyAddress source)
        {
            var transaction = new ItemAddBatchManagementTransaction(prototypeNames, source, true, EntitiesRepository);
            await transaction.ExecuteTransaction();
        }

        public async Task AddItemsInSlotImpl(ItemResourcePack prototypeName, PropertyAddress source, int slot)
        {
            var item = await ContainerUtils.CreateItem(prototypeName.ItemResource);
            var transaction = new ItemAddManagementTransaction(item, (int)prototypeName.Count, source, slot, true, EntitiesRepository);
            await transaction.ExecuteTransaction();
        }

        public async Task AddQuestImpl(Assets.Src.Aspects.Impl.Factions.Template.QuestDef quest, Guid characterId)
        {
            using (var wrapper = await EntitiesRepository.Get<IWorldCharacterClientFull>(characterId))
                await wrapper.Get<IWorldCharacterClientFull>(characterId).Quest.AddQuest(quest);
        }

        public async Task AddTechPointsImpl(TechPointCount[] techPointCounts, Guid characterId)
        {
            using (var worldCharacterWrapper = await EntitiesRepository.Get<IWorldCharacter>(characterId))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                    return;

                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacter>(characterId);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(worldCharacter.KnowledgeEngine.Id))
                {
                    if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                        return;

                    var knowledgeEngine = knowledgeEngineWrapper.Get<IKnowledgeEngine>(worldCharacter.KnowledgeEngine.Id);
                    if (knowledgeEngine.AssertIfNull(nameof(knowledgeEngine)))
                        return;

                    var techPoint = techPointCounts.Select(v => new TechPointCountDef() { TechPoint = v.TechPoint, Count = v.Count }).ToArray();
                    await knowledgeEngine.ChangeRPoints(techPoint, true);
                }
            }
        }

        public async Task AddKnowledgeImpl(KnowledgeDef knowledgeDef, Guid characterId)
        {
            using (var worldCharacterWrapper = await EntitiesRepository.Get<IWorldCharacter>(characterId))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                    return;

                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacter>(characterId);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(worldCharacter.KnowledgeEngine.Id))
                {
                    if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                        return;

                    var knowledgeEngine = knowledgeEngineWrapper.Get<IKnowledgeEngine>(worldCharacter.KnowledgeEngine.Id);
                    if (knowledgeEngine.AssertIfNull(nameof(knowledgeEngine)))
                        return;

                    await knowledgeEngine.AddKnowledge(knowledgeDef);
                }
            }
        }

        // Spawn InteractiveEntity
        public async Task SpawnInteractiveObjectEntityImpl(InteractiveEntityDef entityDef, Vector3 position)
        {
            Guid guid = Guid.NewGuid();
            var entityRef = await EntitiesRepository.Create<IInteractiveEntity>(guid,
                async (entity) =>
                {
                    entity.MovementSync.SetPosition = position;
                    entity.Def      = entityDef;

                     Logger.IfDebug()?.Message("        [Cheats]  1  SpawnInteractiveObjectEntityImpl").Write();;
                });
        }

        public async Task SpawnNewMineableEntityImpl(MineableEntityDef entityDef, Vector3 position)
        {
            var repo = EntitiesRepository;
            Guid guid = Guid.NewGuid();
            var entityRef = await repo.Create<IMineableEntity>(guid,
                async (entity) =>
                {
                    entity.MovementSync.SetPosition = position;
                    entity.Def      = entityDef;

                     Logger.IfDebug()?.Message("        [Cheats]  1  SpawnNewMineableEntityImpl").Write();;
                });
        }

        public async Task SpawnInteractiveEntityImpl(InteractiveEntityDef entityDef, Vector3 position)
        {
            var repo = EntitiesRepository;
            Guid guid = Guid.NewGuid();
            var entityRef = await repo.Create<IInteractiveEntity>(guid,
                async (entity) =>
                {
                    entity.MovementSync.SetPosition = position;
                    entity.Def      = entityDef;

                     Logger.IfDebug()?.Message("        [Cheats]  1  SpawnInteractiveEntityImpl").Write();;
                });
        }

        public Task<string> GetRepositoryEntitiesCountImpl()
        {
            return Task.FromResult<string>(((EntitiesRepository)EntitiesRepository).GetObjectsCount());
        }

        public async Task<string> GetRepositoryEntitiesCountOnAllRepositoriesImpl()
        {
            var result = new StringBuilder();
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(SharedCode.Entities.Service.ICheatServiceAgentEntity)))
            {
                using (var wrapper = await EntitiesRepository.Get<ICheatServiceAgentEntityServer>(pair.Value.Id))
                {
                    try
                    {
                        var entity = wrapper?.Get<ICheatServiceAgentEntityServer>(pair.Value.Id);
                        if (entity == null)
                        {
                            result.AppendLine("<null>");
                            continue;
                        }
                        result.AppendLine(await entity.GetRepositoryEntitiesCount());
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "GetRepositoryEntitiesCountOnAllRepositoriesImpl").Write();;
                        continue;
                    }
                }
            }
            return result.ToString();
        }

        public async Task DumpAllServerRepositoriesImpl()
        {
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(SharedCode.Entities.Service.ICheatServiceAgentEntity)))
            {
                using (var wrapper = await EntitiesRepository.Get<ICheatServiceAgentEntityServer>(pair.Value.Id))
                {
                    try
                    {
                        var entity = wrapper?.Get<ICheatServiceAgentEntityServer>(pair.Value.Id);
                        if (entity == null)
                        {
                            continue;
                        }
                        await entity.DumpRepository();
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "DumpAllServerRepositoriesImpl").Write();;
                        continue;
                    }
                }
            }
        }

        public Task<string> SetVisibilityRadiusImpl(float enterRadius, float leaveRadius)
        {
            Logger.IfInfo()?.Message("Try change visibility radiuses to enterVisibilityRadius {0} leaveVisibilityRadius {1}",
                enterRadius.ToString(), leaveRadius.ToString()).Write();
            return Task.FromResult("DEPRECATED");
        }

        public Task<string> GetTooLongEntityWaitQueuesImpl()
        {
            bool first = true;
            var sb = StringBuildersPool.Get;
            sb.AppendLine("Long queues:");

            void VisitQueueLength(Type type, Guid id, int length)
            {
                if (length > 10)
                {
                    if (first)
                    {
                        first = false;
                        sb.AppendFormat("Type {0}:", type.GetFriendlyName()).AppendLine();
                    }
                    sb.AppendFormat("Id {0} countOverall {1} count {2}", id, length).AppendLine();
                }
            }

            ((IEntitiesRepositoryExtension)EntitiesRepository).VisitEntityQueueLengths(VisitQueueLength);

            return Task.FromResult(sb.ToStringAndReturn());
        }

        public async Task<int> GetCCUImpl()
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IWorldSpaceServiceEntityServer>())
            {
                var entity = wrapper.GetFirstService<IWorldSpaceServiceEntityServer>();
                var ccu = await entity.GetCCU();
                return ccu;
            }
        }

        public Task SetDebugModeImpl(bool enabled)
        {
            ServerCoreRuntimeParameters.SetDebugMode(enabled);
            Logger.IfInfo()?.Message("Debug mode enabled: {0} by user {1}", enabled, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            return Task.CompletedTask;
        }

        public Task SetDebugMobsImpl(bool enabledStatus, /*float forMinutes,*/ bool hard)
        {
            GlobalConstsHolder.GlobalConstsDef.DebugMobs = enabledStatus;
            GlobalConstsHolder.GlobalConstsDef.DbgLogEnabled = enabledStatus;
            GlobalConstsHolder.GlobalConstsDef.DebugMobsHard_DANGER = enabledStatus && hard;
            Logger.IfInfo()?.Message("DebugMobs enabled set: {0} (incl. Hard: {1}) for minutes ___ by user {2}", enabledStatus, enabledStatus && hard, /*forMinutes,*/ GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            // if (enabledStatus && forMinutes > 0f)
            //     AsyncUtils.RunAsyncTask(() => 
            // {
            //     await Task.Delay(forMinutes*60*1000);
            //     ...
            // });
            return Task.CompletedTask;
        }

        public Task SetDebugSpellsImpl(bool enabledStatus)
        {
            GlobalConstsHolder.GlobalConstsDef.DebugSpells = enabledStatus;
            Logger.IfInfo()?.Message("DebugSpells enabled set: {0} by user {1}", enabledStatus, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId).Write();
            return Task.CompletedTask;
        }

        public Task PrintBrokenLocomotionsImpl()
        {
            PawnDataSharedProxy.Print();
            return Task.CompletedTask;
        }

        public async Task CastSpellImpl(OuterRef<IEntity> entityRef, SpellCast spellCast)
        {
            try
            {
                if (entityRef.Guid == Guid.Empty)
                    return;
//                entityRef.TypeId = EntitiesRepositoryBase.GetMasterTypeIdByReplicationLevelType(entityRef.TypeId);
                using (var characterWrapper = await EntitiesRepository.Get(entityRef.TypeId, entityRef.Guid))
                {
                    var hasWizard = characterWrapper.Get<IHasWizardEntity>(entityRef, ReplicationLevel.Master);
                    using (var wizardWrapper = await EntitiesRepository.Get<IWizardEntity>(hasWizard.Wizard.Id, ReplicationLevel.Master))
                    {
                        var wizard = wizardWrapper.Get<IWizardEntityServer>(hasWizard.Wizard.Id);
                        var spellId = await wizard.CastSpell(spellCast);
                        Logger.IfInfo()?.Message("Spell " + spellCast.Def.____GetDebugShortName() + "casted. Result = " + spellId).Write();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

    // --- CurveLogger ---------------------
    #region CurveLogger

        public async Task SetDebugMobPositionLoggingImpl(OuterRef<IEntity> outerRef, bool enabledStatus, bool dump)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(outerRef))
            {
                var posLogger = wrapper?.Get<IDebugPositionLoggerServer>(outerRef, ReplicationLevel.Server);
                if (posLogger != null)
                {
                    if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("2. SetDebugMobPositionLoggingImpl call posLogger.InvokeSetDebugMobPositionLoggingEvent").Write();;
                    await posLogger.InvokeSetDebugMobPositionLoggingEvent(enabledStatus, dump);
                }
                else
                    if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("2.X. SetDebugMobPositionLoggingImpl call posLogger == null").Write();;
            }
        }

        public async Task SetCurveLoggerStateImpl(OuterRef<IEntity> charRef, bool enabledStatus, bool dump, bool serverOnly, string loggerName, Guid dumpId)
        {
            try
            {
                await OnSetCurveLoggerEventHandler(charRef, enabledStatus, dump, loggerName, dumpId);

                if (!serverOnly)
                {
                    Logger.IfDebug()
                        ?.Message(charRef.Guid, $"{nameof(OnSetCurveLoggerEvent)} (enabledStatus:{enabledStatus}, dump:{dump}, loggerName:{loggerName}, dumpId:{dumpId})")
                        .Write();
                    await OnSetCurveLoggerEvent(charRef, enabledStatus, dump, loggerName, dumpId);
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedAsync(oldReplicationMask, newReplicationMask), EntitiesRepository);
        }

        private async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask)
        {
            if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) &&
                !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                // This is the way to tranfer call from master to replicas ClBroad. & upper
                 Logger.IfDebug()?.Message("Subscribe OnSetCurveLoggerEventHandler").Write();;
                using (await this.GetThis())
                    SetCurveLoggerEvent += OnSetCurveLoggerEventHandler;
            } else if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) ||
                        ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                 Logger.IfDebug()?.Message("Unsubscribe OnSetCurveLoggerEventHandler").Write();;
                using (await this.GetThis())
                    SetCurveLoggerEvent -= OnSetCurveLoggerEventHandler;
            }
        }

        private async Task OnSetCurveLoggerEventHandler(OuterRef<IEntity> charRef, bool enabledStatus, bool dump, string loggerName, Guid dumpId)
        {
            Logger.IfDebug()
                ?.Message(charRef.Guid, $"{nameof(OnSetCurveLoggerEventHandler)} (enabledStatus:{enabledStatus}, dump:{dump}, loggerName:{loggerName}, dumpId:{dumpId})")
                .Write();
            
            var repo = EntitiesRepository;

            // var loggerInstance = CurveLogger.Get(loggerName);
            // if (!enabledStatus && dump)
            // {
            //     loggerInstance.DumpDataToFile(repo.CloudNodeType == CloudNodeType.Server, null);
            // }
            // loggerInstance.Active = enabledStatus;
            SetCurveLoggerStateDo(enabledStatus, dump, loggerName, dumpId, repo);

            // Fastforward to Unity node:
            var unityId = await ClusterHelpers.GetUnityIdByChar(charRef, repo);
            if (unityId == Guid.Empty)
            {
                Logger.IfError()?.Message("Can't get valid unityId.").Write();
                return;
            }
            using (var wrapper = await repo.Get<IUnityCheatServiceEntityServer>(unityId))
            {
                var uCheatServiceEntity = wrapper.Get<IUnityCheatServiceEntityServer>(unityId);
                if (uCheatServiceEntity != null)
                    await uCheatServiceEntity.SetCurveLoggerState(enabledStatus, dump, loggerName, dumpId);
            }
        }

        public static void SetCurveLoggerStateDo(bool enabledStatus, bool dump, string loggerName, Guid dumpId, IEntitiesRepository repo)
        {
            var loggerInstance = CurveLogger.Get(loggerName);
            loggerInstance.Active = enabledStatus;
            if (!enabledStatus && dump)
                loggerInstance.DumpDataToFile(repo.CloudNodeType == CloudNodeType.Server, dumpId, null);
        }

        public async Task SetLoggableEnableImpl(OuterRef<IEntity> outerRef, bool enabledStatus)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(outerRef))
            {
                var logable = wrapper?.Get<IHasLogableEntityServer>(outerRef, ReplicationLevel.Server);
                if (logable != null)
                    await logable.LogableEntity.SetCurveLoggerEnable(enabledStatus);
                else
                    Logger.IfError()?.Message($"2.X. Can't get {nameof(IHasLogableEntityServer)} by ref {outerRef}").Write();
            }
        }

    #endregion CurveLogger

        public async Task MainUnityThreadOnServerSleepImpl(OuterRef<IEntity> charRef, bool isOn, float sleepTime, float delayBeforeSleep, float repeatTime)
        {
            Logger.IfInfo()?.Message($"2.0. ChSeEnt. MainUnityThreadOnServerSleepImpl({isOn}, {sleepTime}, {delayBeforeSleep}, {repeatTime})").Write();

            var repo = EntitiesRepository;

            var unityId = await ClusterHelpers.GetUnityIdByChar(charRef, repo);
            if (unityId == Guid.Empty)
            {
                Logger.IfError()?.Message("Can't get valid unityId.").Write();
                return;
            }

            // 4. (At least)) Get UnityCheatService by unityId
            using (var wrapper = await repo.Get<IUnityCheatServiceEntityServer>(unityId))
            {
                var uCheatServiceEntity = wrapper.Get<IUnityCheatServiceEntityServer>(unityId);
                if (uCheatServiceEntity != null)
                    await uCheatServiceEntity.MainUnityThreadOnServerSleep(isOn, sleepTime, delayBeforeSleep, repeatTime);
            }
        }

        public async Task<bool> ChangeHealthImpl(OuterRef<IEntity> victimEntityRef, int deltaValue)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(victimEntityRef))
            {
                var victim = wrapper?.Get<IHasHealthServer>(victimEntityRef, ReplicationLevel.Server);
                if (victim != null)
                    await victim.Health.ChangeHealth(deltaValue);
            }
            return true;
        }

        public async Task<bool> GodmodeImpl(OuterRef<IEntity> applicantEntityRef, bool enable)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(applicantEntityRef))
            {
                var hasStats = wrapper?.Get<IHasStatsEngineServer>(applicantEntityRef, ReplicationLevel.Server);
                if (hasStats != null)
                {
                    if (enable)
                        await hasStats.Stats.SetModifiers(new [] { new StatModifierData(GlobalConstsHolder.StatResources.HealthMinStat, StatModifierType.ClampMin, 1000) }, new ModifierCauser(GlobalConstsHolder.StatResources.HealthMinStat, 0));
                    else
                    {
                        await hasStats.Stats.ChangeValue(GlobalConstsHolder.StatResources.HealthCurrentStat, 0);
                        await hasStats.Stats.RemoveModifiers(new [] { new StatModifierInfo(GlobalConstsHolder.StatResources.HealthMinStat, StatModifierType.ClampMin) }, new ModifierCauser(GlobalConstsHolder.StatResources.HealthMinStat, 0));
                    }
                }
                else
                    Logger.IfError()?.Message($"Can't get `{nameof(IHasStatsEngineServer)}`").Write();
            }
            return true;
        }

        public Task Version01Impl()
        {
            return Task.CompletedTask;
        }

        public async Task<Vector3[]> ResolveCharacterCoordsImpl(Guid[] guids)
        {
            List<Vector3> coords = new List<Vector3>();
            foreach (var bot in guids)
            {
                using (var wrap = await EntitiesRepository.Get<IWorldCharacterServer>(bot))
                {
                    var botChar = wrap.Get<IWorldCharacterServer>(bot);
                    if(botChar == null)
                    {
                        Logger.IfError()?.Message("Cant get bot with guid {0}", bot).Write();
                        coords.Add(new Vector3());
                        continue;
                    }
                    var pos = botChar.MovementSync.Position;
                    coords.Add(pos);
                }
            }
            return coords.ToArray();

        }

        public async Task ForceGCImpl(int count, Guid repositoryId)
        {
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(SharedCode.Entities.Service.ICheatServiceAgentEntity)))
            {
                if (repositoryId != Guid.Empty && pair.Value.Id != repositoryId)
                    continue;

                using (var wrapper = await EntitiesRepository.Get<ICheatServiceAgentEntityServer>(pair.Value.Id))
                {
                    try
                    {
                        var entity = wrapper?.Get<ICheatServiceAgentEntityServer>(pair.Value.Id);
                        if (entity == null)
                        {
                            continue;
                        }
                        await entity.ForceGC(count);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "ForceGCImpl").Write();;
                        continue;
                    }
                }
            }
        }

        public Task ForceSelfCompactionGCImpl()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            return Task.CompletedTask;
        }

        public async Task SetGCEnabledImpl(bool enabled, Guid repositoryId)
        {
            foreach (var pair in ((EntitiesRepository)EntitiesRepository).GetEntitiesCollection(typeof(SharedCode.Entities.Service.ICheatServiceAgentEntity)))
            {
                if (repositoryId != Guid.Empty && pair.Value.Id != repositoryId)
                    continue;

                using (var wrapper = await EntitiesRepository.Get<ICheatServiceAgentEntityServer>(pair.Value.Id))
                {
                    try
                    {
                        var entity = wrapper?.Get<ICheatServiceAgentEntityServer>(pair.Value.Id);
                        if (entity == null)
                        {
                            continue;
                        }
                        await entity.SetGCEnabled(enabled);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "SetGCEnabledImpl").Write();;
                        continue;
                    }
                }
            }
        }

        public async Task SetServerCheatVariableImpl(BaseResource res, string value)
        {
            CheatVariables.SetCheatVariable(res, value);
        }

        #region Test

        public async Task<OuterRef<IEntity>> TestCheckPZ15200DoneImpl(float waitBeforeReplicate)
        {
            if (DbgLog.Enabled) DbgLog./*LogErr*/Log(15200, $"15200::: {DateTime.UtcNow} ##DBG:  TestCheckPZ15200DoneImpl(waitBeforeReplicate:{waitBeforeReplicate}). CRE ENTTY");

            var repo = EntitiesRepository;
            var enttyRef = await repo.Create<IToucherTestEntity>(Guid.NewGuid());

            AsyncUtils.RunAsyncTask(async () =>
            {
                if (waitBeforeReplicate > 0)
                    await Task.Delay((int) (waitBeforeReplicate * 1000));

                if (DbgLog.Enabled) DbgLog./*LogErr*/Log(15200, $"15200::: {DateTime.UtcNow} ##DBG:  TestCheckPZ15200DoneImpl(waitBeforeReplicate:{waitBeforeReplicate}). SubscribeReplication");

                await repo.SubscribeReplication(
                    enttyRef.TypeId,
                    enttyRef.Id,
                    Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId,
                    ReplicationLevel.ClientFull
                );
            });

            return new OuterRef<IEntity>(enttyRef);
        }

        #endregion Test
        public Task EnableWizardLoggerImpl(OuterRef entityRef, bool enable)
        {
            return EnableWizardLogger(EntitiesRepository, entityRef, enable);
        }
        
        public static async Task EnableWizardLogger(IEntitiesRepository repo, OuterRef entityRef, bool enable)
        {
            using(var c1 = await repo.Get(entityRef.TypeId, entityRef.Guid))
            {
                var hasWizard = c1.Get<IHasWizardEntityServer>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Server);
                if (hasWizard != null)
                {
                    var wizardRef = hasWizard.Wizard.OuterRef;
                    using (var c2 = await repo.Get(wizardRef.TypeId, wizardRef.Guid))
                    {
                        var wizard = c2.Get<IWizardEntityServer>(wizardRef, ReplicationLevel.Server);
                        if (wizard == null) throw new NullReferenceException($"HasWizardEntity {entityRef} with invalid wizard");
                        await wizard.SetIsInterestingEnoughToLog(enable);
                    }
                }
                else
                    Logger.IfError()?.Message($"{entityRef} is not a {nameof(IHasWizardEntity)}").Write();
            }
        }
		
        public async Task SetGenderImpl(OuterRef entityRef, GenderDef gender)
        {
            var repo = EntitiesRepository;
            using(var c1 = await repo.Get(entityRef.TypeId, entityRef.Guid))
            {
                var hasGender = c1.Get<IHasGender>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Master);
                if (hasGender != null)
                {
                    await hasGender.SetGender(gender);
                }
                else
                    Logger.IfError()?.Message($"{entityRef} is not a {nameof(IHasGender)}").Write();
            }
        }
        
        public async Task InvokeTraumaImpl(OuterRef entityRef, string trauma)
        {
            var repo = EntitiesRepository;
            using(var c1 = await repo.Get(entityRef.TypeId, entityRef.Guid))
            {
                var hasTraumas = c1.Get<IHasTraumas>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Master);
                if (hasTraumas != null)
                {
                    await hasTraumas.Traumas.StartTrauma(trauma);
                }
                else
                    Logger.IfError()?.Message($"{entityRef} is not a {nameof(IHasTraumas)}").Write();
            }
        }
        
        public async Task StopTraumaImpl(OuterRef entityRef, string trauma)
        {
            var repo = EntitiesRepository;
            using(var c1 = await repo.Get(entityRef.TypeId, entityRef.Guid))
            {
                var hasTraumas = c1.Get<IHasTraumas>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.Master);
                if (hasTraumas != null)
                {
                    await hasTraumas.Traumas.StopTrauma(trauma);
                }
                else
                    Logger.IfError()?.Message($"{entityRef} is not a {nameof(IHasTraumas)}").Write();
            }
        }
    }
}
