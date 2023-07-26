using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Building;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.Transactions;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities;
using SharedCode.Entities.Building;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Player;
using SharedCode.DeltaObjects;
using SharedCode.Utils.Extensions;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Mineable;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl.Traumas.Template;
using GeneratedCode.DeltaObjects.Chain;
using NLog;
using SharedCode.CustomData;
using Assets.Src.Aspects.Impl.Factions.Template;
using SharedCode.Aspects.Science;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.GeneratedCode.Shared;
using ResourcesSystem.Loader;
using SharedCode.Serializers;
using System.Reflection;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ResourceSystem.Account;
using Telemetry;
using GeneratedCode.Custom.Config;
using GeneratedCode.Repositories;
using Assets.Src.Arithmetic;
using Assets.ResourceSystem.Aspects.Banks;
using Assets.ResourceSystem.Aspects.FX.FullScreenFx;
using ColonyShared.GeneratedCode.Shared.Aspects;
using SharedCode.Refs;
using Assets.ResourceSystem.Entities;
using ColonyShared.GeneratedCode.Manual.DeltaObjects;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.MovementSync;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldCharacter : IHookOnInit, IHookOnDatabaseLoad, IHookOnReplicationLevelChanged, IHookOnUnload
    {

        public readonly int DebugObjectWatchID = GlobalLoggers.GetNextWatchID();

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public bool QuerySpatialData => ((IHasSpatialDataHandlersDef)Def).QuerySpatialData;

        public async Task OnInit()
        {
            IsIdle = false;
            IsAFK = false;
            //Назначение параметров по ум. - в WorldNodeServiceEntity SpawnClusterEntity()
            if(string.IsNullOrWhiteSpace(Name))
                Name = "Player";

            WorldCharacterDef _playerDef = (WorldCharacterDef)Def;


            var @ref = new OuterRef<IEntity>(Id, TypeId);
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, w => { w.Owner = @ref; w.IsInterestingEnoughToLog = Constants.WorldConstants.IsCharacterInterestingEnoughtToLogWizardEvents; return Task.CompletedTask; });
            CraftEngine = await EntitiesRepository.Create<ICraftEngine>(Id, craftEngine =>
            {
                craftEngine.OwnerInformation.Owner = @ref;
                craftEngine.ResultContainerAddress = EntityPropertyResolver.GetPropertyAddress(Inventory);
                craftEngine.UseOwnOutputContainer = false;
                craftEngine.MaxQueueSize = _playerDef.MaxCraftQueueSize;
                return Task.CompletedTask;
            });
            BuildingEngine = await EntitiesRepository.Create<IBuildingEngine>(Id, buildingEngine =>
            {
                buildingEngine.OwnerInformation.Owner = @ref;
                return Task.CompletedTask;
            });

            KnowledgeEngine = await EntitiesRepository.Create<IKnowledgeEngine>(Id, knowledgeEngine =>
            {
                knowledgeEngine.OwnerInformation.Owner = @ref;
                return Task.CompletedTask;
            });

            PerkActionsPrices = GameResourcesHolder.Instance.LoadResource<PerkActionsPricesDef>("/Inventory/Perks/PerkActionPrices/PerkActionsPrices");


            await CreateProxyStats(_playerDef);

            Health.DamageReceivedEvent += StopHealthRegeneration;

            Mortal.DieEvent += DoOnDie;
            Mortal.BeforeResurrectEvent += OnBeforeResurrectEvent;
            GlobalLoggers.SubscribeLogger.IfInfo()?.Message($"Subscribe: [{((WorldCharacter)parentEntity).DebugObjectWatchID}; {parentEntity.Id} -> {GetType().Name}] -> [{nameof(Mortal.DieEvent)}; {nameof(DoOnDie)}]").Write();

            await AssignInitialQuests();

            await WorldBakenRepoSubscriber.SubscribeBakenRepository(EntitiesRepository, this);

            SessionId = Guid.NewGuid();
            await SetGender(AccountStats.Gender ?? Constants.CharacterConstants.DefaultGender);
            
            var spellsOnBirth = ((WorldCharacterDef)Def).SpellsOnBirth;
            await SpellCastHelpers.CastSpells(parentEntity, spellsOnBirth);

            var spellsOnEnterWorld = ((WorldCharacterDef)Def).SpellsOnEnterWorld;
            await SpellCastHelpers.CastSpells(parentEntity, spellsOnEnterWorld);

            int lvl = LevelUpDatasHelpers.CalcAccLevel(AccountStats.AccountExperience);
            var achievementSpellsOnBirth = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.GetAllAchievementSpellsToCastByLvl(lvl, LevelUpDatasDef.SpellsGroup.OnBirth);
            if (achievementSpellsOnBirth != null)
                await SpellCastHelpers.CastSpells(parentEntity, achievementSpellsOnBirth.ToArray());

            var achievementSpellsOnEnter = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.GetAllAchievementSpellsToCastByLvl(lvl, LevelUpDatasDef.SpellsGroup.OnEnterWorld);
            if (achievementSpellsOnEnter != null)
                await SpellCastHelpers.CastSpells(parentEntity, achievementSpellsOnEnter.ToArray());
        }

        private async Task CreateProxyStats(WorldCharacterDef playerDef)
        {
            await CreateProxyStat(playerDef.InventoryWeightProxyStat, Inventory.To<IContainerClientFull>());
            await CreateProxyStat(playerDef.DollWeightProxyStat, Doll.To<ICharacterDollClientFull>());

            using (var ceWrap = await EntitiesRepository.Get<ICraftEngine>(ParentEntityId))
            {
                var ce = ceWrap.Get<ICraftEngineClientFull>(ParentEntityId);
                if (ce == null)
                    Logger.IfError()?.Message("Somehow we don't have ICraftEngine on object {0}", ParentEntityId).Write();
                else
                {
                    await CreateProxyStat(playerDef.CraftWeightProxyStat, ce.IntermediateCraftContainer);
                }
            }
        }

        private async Task CreateProxyStat(StatResource statResource, IItemsContainerClientFull container)
        {
            EntityPropertyResolver.TryGetPropertyAddress(container, out PropertyAddress address);
            await Stats.AddProxyStat(statResource, address);
        }

        private async Task AssignInitialQuests()
        {
            using (var myCharWrapper = await EntitiesRepository.Get<IWorldCharacter>(Id))
            {
                var myChar = myCharWrapper.Get<IWorldCharacter>(Id);
                foreach (var recQuest in ((WorldCharacterDef)Def).InitialQuests)
                    if (recQuest.IsValid && !Quest.Quests.ContainsKey(recQuest.Target))
                        await Quest.AddQuest(recQuest.Target);
            }
        }

        public async Task OnDatabaseLoad()
        {

            IsIdle = false;
            IsAFK = false;
            var @ref = new OuterRef<IEntity>(Id, TypeId);
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, w => { w.Owner = @ref; w.IsInterestingEnoughToLog = Constants.WorldConstants.IsCharacterInterestingEnoughtToLogWizardEvents; return Task.CompletedTask; });

            BuildingEngine = await EntitiesRepository.Create<IBuildingEngine>(Id, buildingEngine =>
            {
                buildingEngine.OwnerInformation.Owner = @ref;
                return Task.CompletedTask;
            });

            Mortal.DieEvent += DoOnDie;
            GlobalLoggers.SubscribeLogger.IfInfo()?.Message($"Subscribe: [{((WorldCharacter)parentEntity).DebugObjectWatchID}; {parentEntity.Id} -> {GetType().Name}] -> [{nameof(Mortal.DieEvent)}; {nameof(DoOnDie)}]").Write();

            MovementSync.SetTransform = new Transform(MovementSync.__SyncMovementStateReliable.State.Position, MovementSync.__SyncMovementStateReliable.State.Rotation);
            var at = new PositionRotation(MovementSync.Position, MovementSync.Rotation);
            //await InvokeResurrectEvent(at);

            await WorldBakenRepoSubscriber.SubscribeBakenRepository(EntitiesRepository, this);

            WorldCharacterDef _playerDef = (WorldCharacterDef)Def;
            await CreateProxyStats(_playerDef);

            await Quest.OnDatabaseLoad();
            SessionId = Guid.NewGuid();

            var spellsOnEnterWorld = ((WorldCharacterDef)Def).SpellsOnEnterWorld;
            await SpellCastHelpers.CastSpells(parentEntity, spellsOnEnterWorld);

            await SetGender(AccountStats.Gender ?? Constants.CharacterConstants.DefaultGender);

            int lvl = LevelUpDatasHelpers.CalcAccLevel(AccountStats.AccountExperience);
            var spellsToCast = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.GetAllAchievementSpellsToCastByLvl(lvl, LevelUpDatasDef.SpellsGroup.OnEnterWorld);
            if (spellsToCast != null)
                await SpellCastHelpers.CastSpells(parentEntity, spellsToCast.ToArray());
        }

        public static void LogEnteredWorld(IWorldCharacterServer character)
        {
            var repo = character.EntitiesRepository;
            var mapId = character.MapOwner.OwnerMapId;
            async ValueTask<MapDef> GetMap()
            {
                using (var wrapper = await repo.Get<IMapEntityAlways>(mapId))
                {
                    var mapEnt = wrapper.Get<IMapEntityAlways>(mapId);
                    return mapEnt?.Map;
                }
            }


            var name = character.Name;
            var pos = character.MovementSync.Position;
            var id = character.Id;
            var sessionId = character.SessionId;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrap = await repo.GetFirstService<ISessionHolderEntityServer>())
                {
                    var service = wrap.GetFirstService<ISessionHolderEntityServer>();
                    await service.Register(id, sessionId);
                }

                var map = await GetMap();
                Telemetry.WorldCharacterEvents.EnteredWorld(name, id, sessionId, pos, map);
            }, repo);
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedAsync(oldReplicationMask, newReplicationMask, Id), EntitiesRepository);
        }

        private async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask, Guid id)
        {
            if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Always))
            {
                using (var wrapper = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
                {
                    var bakenCoordinator = wrapper?.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                    if (bakenCoordinator != null)
                        await bakenCoordinator.SetCharacterLoaded(id, false);
                }
            }

            if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Always))
            {
                using (var wrapper = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
                {
                    var bakenCoordinator = wrapper?.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                    if (bakenCoordinator != null)
                        await bakenCoordinator.SetCharacterLoaded(id, true);
                }
            }
        }

        public async Task SendChatMessageImpl(string message)
        {
            if (message.Length > 1024)
                message = message.Substring(0, 1024); // To avoid network overload

            await InvokeNewChatMessageEvent(Name, message);
        }

        public async Task InvokeNewChatMessageEventImpl(string name, string message)
        {
            // Logger.IfInfo()?.Message($"Chat message: {message}").Write();
            await OnNewChatMessageEvent(name, message);
        }
        public async Task<bool> RespawnImpl(bool useBaken, bool anyCommonBaken, Guid commonBakenId)
        {
            using (var wnseWrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(WorldSpaced.OwnWorldSpace.Guid))
            {
                var wnse = wnseWrapper.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid);
                return await wnse.Respawn(Id, useBaken, anyCommonBaken, commonBakenId);
            }

        }
        public async ValueTask<bool> HasBakenImpl()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[WNSE].HasBakenImpl ({Id})").Write();
            if (ActivatedCommonBakens.Count > 0)
                return true;

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityClientFull>(Id))
            {
                var bakenChar = wrapper?.Get<IBakenCharacterEntityClientFull>(Id) ?? null;
                if (bakenChar == null)
                    return false;

                return bakenChar.ActiveBaken != default;
            }
        }

        public async ValueTask<bool> IsBakenActivatedImpl(OuterRef<IEntity> bakenRef)
        {
            if (ActivatedCommonBakens.ContainsKey(bakenRef.Guid))
                return true;

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityClientFull>(Id))
            {
                var coordinatorServer = wrapper?.Get<IBakenCharacterEntityClientFull>(Id) ?? null;
                if (coordinatorServer == null)
                    return false;

                return coordinatorServer.ActiveBaken == bakenRef;
            }
        }

        private async Task StopHealthRegeneration(float prevHealthVal, float newHealthVal, float maxHealth, Damage damage)
        {
            using (var wrapper = await EntitiesRepository.Get(Wizard.TypeId, Wizard.Id))
            {
                var wizardEntity = wrapper.Get<IWizardEntity>(Wizard.Id);
                if (wizardEntity != null)
                {
                    var spellDef = GlobalConstsHolder.GlobalConstsDef.StopHealthRegenerationSpell;
                    //if (await wizardEntity.HasActiveSpell(spellDef))
                    //    await wizardEntity.StopSpellByDef(spellDef, SpellId.Invalid, SpellFinishReason.SucessOnDemand);

                    var spellId = await wizardEntity.CastSpell(new SpellCast { Def = spellDef });
                    //Logger.IfError()?.Message(spellId).Write();
                }
            }
        }

        public async Task<ContainerItemOperation> AddItemsImpl(List<ItemResourcePack> itemResourcesToAdd, PropertyAddress destination)
        {
            var itemTransaction = new ItemAddBatchManagementTransaction(itemResourcesToAdd, destination, true, EntitiesRepository);
            return await itemTransaction.ExecuteTransaction();
        }

        public async Task<ContainerItemOperation> MoveItemImpl(PropertyAddress source, int sourceSlotId,
            PropertyAddress destination, int destinationSlotId, int count, Guid clientSrcEntityId)
        {
            var itemTransaction = new ItemMoveManagementTransaction(source, sourceSlotId, destination, destinationSlotId, count, clientSrcEntityId, true, EntitiesRepository);
            return await itemTransaction.ExecuteTransaction();
        }

        public async Task<ContainerItemOperation> MoveAllItemsImpl(PropertyAddress source, PropertyAddress destination)
        {
            var itemTransaction = new ItemMoveAllManagementTransaction(source, destination, true, false, EntitiesRepository);
            return await itemTransaction.ExecuteTransaction();
        }

        public async Task<ContainerItemOperation> RemoveItemImpl(PropertyAddress source, int sourceSlotId, int count, Guid clientEntityId)
        {
            var itemTransaction = new ItemRemoveBatchManagementTransaction(
                new List<RemoveItemBatchElement>() {
                    new RemoveItemBatchElement(source, sourceSlotId, count, clientEntityId)
                }, true, EntitiesRepository);
            var result = await itemTransaction.ExecuteTransaction();

            if (result.IsSuccess)
            {
                var container = EntityPropertyResolver.Resolve<IItemsContainer>(this, source, ReplicationLevel.Master);
                if (container == TemporaryPerks || container == SavedPerks || container == PermanentPerks)
                {
                    var item = container.Items[sourceSlotId].Item;
                    if (item.Id == clientEntityId)
                    {
                        int destroyCountValue = 0;
                        if (PerksDestroyCount.TryGetValue(item.ItemResource, out destroyCountValue))
                            PerksDestroyCount[item.ItemResource] = destroyCountValue + 1;
                        else
                            PerksDestroyCount[item.ItemResource] = 1;
                    }
                    PerksDestroyCount[item.ItemResource]++;
                }
            }

            return result;
        }

        public async Task<ContainerItemOperationResult> SavePerkImpl(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId)
        {
            var destinationAddress = EntityPropertyResolver.GetPropertyAddress(SavedPerks);
            var sourceContainer = EntityPropertyResolver.Resolve<IItemsContainer>(this, source, ReplicationLevel.Master);
            var item = sourceContainer.Items[sourceSlotId].Item;

            PriceDef cost;
            var perkActionCost = PerkActionsPrices.PerkSavingCustomCosts.Where(v => v.Item == item.ItemResource);
            if (perkActionCost.Any())
                cost = perkActionCost.Single().Price;
            else
                cost = PerkActionsPrices.PerkSavingDefaultCosts.Where(v => v.PerkType == item.ItemResource.ItemType).Single().Price;

            using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(KnowledgeEngine.Id))
            {
                var knowledgeEngineClientFull = knowledgeEngineWrapper.Get<IKnowledgeEngine>(KnowledgeEngine.Id);
                if (await knowledgeEngineClientFull.CanChangeRPoints(cost.TechPointCosts, true))
                {
                    var itemTransaction = new ItemMoveManagementTransaction(source, sourceSlotId, destinationAddress, -1, 1, clientSrcEntityId, true, EntitiesRepository);
                    var result = await itemTransaction.ExecuteTransaction();
                    if (result.IsSuccess)
                        await knowledgeEngineClientFull.ChangeRPoints(cost.TechPointCosts, true);
                }
                else
                    return ContainerItemOperationResult.ErrorSrcCantAdd;
            }

            return ContainerItemOperationResult.Success;
        }

        public async Task<ContainerItemOperationResult> DisassemblyPerkImpl(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId)
        {
            var sourceContainer = EntityPropertyResolver.Resolve<IItemsContainer>(this, source, ReplicationLevel.Master);
            var item = sourceContainer.Items[sourceSlotId].Item;

            IPerkActionPrice priceDef;
            var perkActionCost = PerkActionsPrices.PerkDisassemblyCustomBenefits.Where(v => v.Item == item.ItemResource);
            if (perkActionCost.Any())
                priceDef = perkActionCost.Single();
            else
                priceDef = PerkActionsPrices.PerkDisassemblyDefaultBenefits.Where(v => v.PerkType == item.ItemResource.ItemType).Single();

            int itemsCountMultiplier = priceDef.AmountOfResourcesMultiplier == null
                ? 1
                : (int)Math.Round((await priceDef.AmountOfResourcesMultiplier.Target.CalcAsync(
                    new OuterRef<IEntity>(Id, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter))), EntitiesRepository)).Float);

            var newPriceDef = itemsCountMultiplier != 1
                ? priceDef.Price.GetPriceWithMultiplier(itemsCountMultiplier)
                : priceDef.Price;

            using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(KnowledgeEngine.Id))
            {
                var knowledgeEngineClientFull = knowledgeEngineWrapper.Get<IKnowledgeEngine>(KnowledgeEngine.Id);

                var itemTransaction = new ItemRemoveBatchManagementTransaction(
                    new List<RemoveItemBatchElement>() {new RemoveItemBatchElement(source, sourceSlotId, 1, clientSrcEntityId)}, true, EntitiesRepository);
                var result = await itemTransaction.ExecuteTransaction();
                if (result.IsSuccess)
                    await knowledgeEngineClientFull.ChangeRPoints(newPriceDef.TechPointCosts, true);
            }

            return ContainerItemOperationResult.Success;
        }

        public async Task<ContainerItemOperationResult> BreakImpl(PropertyAddress source, int sourceSlotId, Guid clientSrcEntityId)
        {
            var container = EntityPropertyResolver.Resolve<IItemsContainer>(this, source, ReplicationLevel.Master);
            ISlotItem usedItem;
            if (container.Items.TryGetValue(sourceSlotId, out usedItem))
            {
                var stack = usedItem.Stack;
                var durability = (usedItem.Item.ItemResource as ItemResource)?.Durability.Target;
                if (durability != null)
                {
                    var currentDurability = await usedItem.Item.Health.GetHealthCurrent();
                    var maxDurability = await usedItem.Item.Health.GetMaxHealth();
                    var maxAbsoluteDurability = await usedItem.Item.Health.GetMaxHealthAbsolute();
                    var isFullBreak = durability.FullBreakDurability >= maxDurability / maxAbsoluteDurability;

                    var itemPack = isFullBreak ? durability.ItemsOnFullBreak : durability.ItemsOnBreak;
                    if ((await RemoveItem(source, sourceSlotId, stack, clientSrcEntityId)).IsSuccess)
                    {
                        var itemsList = new List<ItemResourcePack>();
                        foreach (var items in itemPack)
                            itemsList.Add(new ItemResourcePack(items.Item, (uint)(items.Count * stack)));

                        var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(Inventory);
                        var result = await AddItems(itemsList, inventoryAddress);
                        return result.Result;
                    }
                    else
                    {
                        return ContainerItemOperationResult.ErrorSrcCantRemove;
                    }
                }
            }
            else
            {
                return ContainerItemOperationResult.ErrorSrcNotFound;
            }

            return ContainerItemOperationResult.Success;
        }

        public async Task<ContainerItemOperationResult> ChangeCurrenciesImpl(List<CurrencyResourcePack> currencies)
        {
            var result = ContainerItemOperationResult.Success;
            var currencyAddress = EntityPropertyResolver.GetPropertyAddress(Currency);

            var currencyToAdd = currencies
                .Where(v => v.Count > 0)
                .Select(v => new ItemResourcePack(v.ItemResource, (uint)v.Count)).ToList();

            if (currencyToAdd.Any())
            {
                var addResult = await AddItems(currencyToAdd, currencyAddress);
                if (!addResult.IsSuccess)
                    result = ContainerItemOperationResult.ErrorDstCantAdd;
            }

            var currencyToRemove = currencies.Where(v => v.Count < 0);
            if (currencyToRemove.Any())
            {
                foreach (var currency in currencyToRemove)
                {
                    var countToRemove = -currency.Count;
                    var slots = Currency.Items.Where(v => v.Value.Item.ItemResource == currency.ItemResource);
                    if (slots.Any())
                    {
                        var slotItem = slots.First();
                        var removeResult = await RemoveItem(currencyAddress, slotItem.Key, countToRemove, slotItem.Value.Item.Id);
                        if (!removeResult.IsSuccess)
                            result = ContainerItemOperationResult.ErrorDstCantRemove;
                    }
                    else
                        result = ContainerItemOperationResult.ErrorDstCantRemove;
                }
            }

            return result;
        }

        public Task<uint> GetCurrencyValueImpl(CurrencyResource currency)
        {
            return Task.FromResult(
                    (uint)Currency.Items
                        .Where(v => v.Value.Item.ItemResource == currency)
                        .Select(v => v.Value.Stack)
                        .DefaultIfEmpty(0).Sum()
                );
        }

        private Random _random = new Random((int)DateTime.UtcNow.Ticks);

        public Task ChangeStatisticImpl(StatisticType target, StatisticType statistic, float value)
        {
            float oldValue = 0;
            float newValue = value;
            if (!Statistics.Keys.Contains(target))
            {
                var objectStatistic = new DeltaDictionary<StatisticType, float>();
                objectStatistic.Add(statistic, value);
                Statistics.Add(target, objectStatistic);
            }
            else if (!Statistics[target].Keys.Contains(statistic))
            {
                Statistics[target].Add(statistic, value);
            }
            else
            {
                oldValue = Statistics[target][statistic];
                newValue = oldValue + value;
                Statistics[target][statistic] = newValue;
            }
            return Task.CompletedTask;
        }

        private static BaseItemResource _emptyPerk;
        private static bool IsEmptyPerk(BaseItemResource perk, IEntitiesRepository repo)
        {
            if (_emptyPerk == null)
                _emptyPerk = GameResourcesHolder.Instance.LoadResource<BaseItemResource>("/Inventory/Perks/Special/Empty");

            if (_emptyPerk == null)
                Logger.IfWarn()?.Message("EmptyPerk == null").Write();

            return perk == _emptyPerk;
        }

        private string LogProbability(IDeltaDictionary<BaseItemResource, float> prob)
        {
            string str = "";
            foreach (var p in prob)
                str += p.Key + ": " + p.Value.ToString("F1") + "%; ";

            return str;
        }

        public async Task AddUsedSlotImpl(ResourceIDFull dollSlotRes)
        {
            await Doll.AddUsedSlot(dollSlotRes);
        }

        public async Task RemoveUsedSlotImpl(ResourceIDFull dollSlotRes)
        {
            await Doll.RemoveUsedSlot(dollSlotRes);
        }

        public async Task<bool> AddPerkSlotImpl(PropertyAddress slotsAddress, int slotId, ItemTypeResource perkSlotType)
        {
            if (perkSlotType.AssertIfNull(nameof(perkSlotType)))
                return false;

            if (!await CanAddPerkSlotImpl(perkSlotType))
                return false;

            TechnologyOperationResult result = TechnologyOperationResult.None;
            using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(KnowledgeEngine.Id))
            {
                if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                    return false;

                var knowledgeEngineClientFull = knowledgeEngineWrapper.Get<IKnowledgeEngine>(KnowledgeEngine.Id);
                if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                    return false;

                PriceDef perkActionCost = PerkActionsPrices.ToPerkSlotUpgradingCosts.Where(v => v.PerkType == perkSlotType).Single().Price;
                result = await knowledgeEngineClientFull.ChangeRPoints(perkActionCost.TechPointCosts, true);
            }

            if (result == TechnologyOperationResult.Success)
            {
                using (var perksWrapper = await EntitiesRepository.Get(slotsAddress.EntityTypeId, slotsAddress.EntityId))
                {
                    if (perksWrapper.AssertIfNull(nameof(perksWrapper)))
                        return false;

                    var characterPerks = EntityPropertyResolver.Resolve<ICharacterPerks>(perksWrapper.Get<IEntity>(slotsAddress.EntityTypeId, slotsAddress.EntityId), slotsAddress, ReplicationLevel.Master);
                    if (characterPerks.AssertIfNull(nameof(characterPerks)))
                        return false;

                    await characterPerks.AddPerkSlot(slotId, perkSlotType);
                }
            }

            return (result == TechnologyOperationResult.Success);
        }

        public async Task<bool> CanAddPerkSlotImpl(ItemTypeResource perkSlotType)
        {
            using (var knowledgeEngineWrapper = await EntitiesRepository.Get<IKnowledgeEngine>(KnowledgeEngine.Id))
            {
                if (knowledgeEngineWrapper.AssertIfNull(nameof(knowledgeEngineWrapper)))
                    return false;

                var knowledgeEngineClientFull = knowledgeEngineWrapper.Get<IKnowledgeEngine>(KnowledgeEngine.Id);
                if (knowledgeEngineClientFull.AssertIfNull(nameof(knowledgeEngineClientFull)))
                    return false;

                PriceDef perkActionCost = PerkActionsPrices.ToPerkSlotUpgradingCosts.Where(v => v.PerkType == perkSlotType).Single().Price;
                return await knowledgeEngineClientFull.CanChangeRPoints(perkActionCost.TechPointCosts, true);
            }
        }

      

        public Task SetStatImpl(StatResource res, float deltaValue)
        {
            return Task.CompletedTask;
        }

        // Build system begin ---------------------------------------------------------------------
        public async Task<OperationResult> CreateBuildElementImpl(BuildType type, Guid placeId, BuildRecipeDef buildRecipeDef, CreateBuildElementData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = await CreateBuildElementInternal(type, placeId, buildRecipeDef, data);
            using (var statWrapper = await EntitiesRepository.Get(TypeId, Id))
            {
                var hasStatistics = statWrapper.Get<IHasStatisticsServer>(TypeId, Id, ReplicationLevel.Server);
                if (hasStatistics != null)
                {
                    await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.BuildEventArgs() { Building = buildRecipeDef });
                }
            }
            return result;
        }

        private async Task<OperationResult> CreateBuildElementInternal(BuildType type, Guid placeId, BuildRecipeDef buildRecipeDef, CreateBuildElementData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (buildRecipeDef == null)
            {
                BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, param elementDef is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_InvalidElementDef };
            }
            if (data == null)
            {
                BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, param data is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_InvalidCreateElementData };
            }
            if ((type == BuildType.BuildingElement) || (type == BuildType.BuildingAttachment))
            {
                bool cantBuildHere = !BuildingEngineHelper.CanBuildHere(data.Position, EntitiesRepository.GetSceneForEntity(new OuterRef<IEntity>(this)), BuildUtils.DefaultBuildingPlaceDef, false);
                if (cantBuildHere)
                {
                    BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, building place is not allowed to build at this position", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_PlacePositionNotAllowed };
                }
                if (placeId == Guid.Empty)
                {
                    var createBuildingPlaceAndBuildingElementData = data as CreateBuildingPlaceAndBuildingElementData;
                    if (createBuildingPlaceAndBuildingElementData == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, param data type: {data.GetType().ToString()} is not supported for empty placeId", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_InvalidCreateElementData };
                    }
                    //TODO поддержать межсерверное строительство (когда персонаж не на том сервере, на котором ставится забор)
                    using (var worldSpaceServiceEntityWrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid))
                    {
                        var worldSpaceServiceEntity = worldSpaceServiceEntityWrapper.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid);
                        //TODO building: UnityWorldSpace check geometry validation
                        //worldSpaceServiceEntity.UnityWorldSpace.CanBuildHere()

                        var newBuildingPlaceRef = await EntitiesRepository.Create<IBuildingPlace>(Guid.NewGuid(), (entity) =>
                        {
                            entity.Def = BuildUtils.DefaultBuildingPlaceDef;
                            entity.MovementSync.SetTransform = new Transform(createBuildingPlaceAndBuildingElementData.PlacePosition, createBuildingPlaceAndBuildingElementData.PlaceRotation);
                            entity.OwnerInformation.Owner = new OuterRef<IEntity>(Id, TypeId);
                            entity.MapOwner = MapOwner;
                            entity.WorldSpaced.OwnWorldSpace = WorldSpaced.OwnWorldSpace;
                            return Task.CompletedTask;
                        });

                        using (var newBuildingPlaceWrapper = await EntitiesRepository.Get<IBuildingPlace>(newBuildingPlaceRef.Id))
                        {
                            var newBuildingPlace = newBuildingPlaceWrapper.Get<IBuildingPlace>(newBuildingPlaceRef.Id);
                            await newBuildingPlace.WorldSpaced.AssignToWorldSpace(WorldSpaced.OwnWorldSpace);
                            var result = await newBuildingPlace.StartPlace(true);
                            if (!result)
                            {
                                BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {newBuildingPlaceRef.Id}, start place return error", MethodBase.GetCurrentMethod().DeclaringType.Name);
                                return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_PlaceNotStarted };
                            }
                        }
                        placeId = newBuildingPlaceRef.Id;
                    }
                    if (placeId == Guid.Empty)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, invalid building place id", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_InvalidPlaceId };
                    }
                }
                using (var buildingPlaceWrapper = await EntitiesRepository.Get<IBuildingPlace>(placeId))
                {
                    var buildingPlace = buildingPlaceWrapper.Get<IBuildingPlace>(placeId);
                    if (buildingPlace == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, building place not found", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_PlaceNotFound };
                    }

                    if (buildingPlace.OwnerInformation.Owner.Guid != Id)
                    {
                        BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, character is not owner of building place", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_PlaceNotOwner };
                    }

                    var checkResourcesResult = BuildResourceManager.CheckResources(buildRecipeDef, this);
                    if (checkResourcesResult == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Check resources returned null result", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CheckResourcesNull };
                    }
                    if (checkResourcesResult.Result != ResourceOperationResultCode.Success)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Check resources returned error:{checkResourcesResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ((checkResourcesResult.Result == ResourceOperationResultCode.ContainerItemOperationNotEnoughResources) ? ErrorCode.Error_CheckResourcesNotEnough : ErrorCode.Error_CheckResourcesError) };
                    }

                    var newBuild = BuildPlace.CreatePositionedBuild(type, buildRecipeDef, data, new OuterRef<IEntity>(this));
                    if (newBuild == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error creating positioned build data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CreatePositionedBuild };
                    }

                    var positionedBuildWrapper = new IPositionedBuildWrapper() { PositionedBuild = newBuild };

                    var checkResult = await buildingPlace.BuildPlace.Check(type, positionedBuildWrapper);
                    if (!checkResult)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error checking element", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CheckElement };
                    }

                    var claimResourcesResult = await BuildResourceManager.ClaimResources(checkResourcesResult.ClaimItems, this.EntitiesRepository);
                    if (claimResourcesResult == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Claim resources returned null result", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_ClaimResourcesNull };
                    }
                    if (claimResourcesResult.Result != ResourceOperationResultCode.Success)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Claim resources returned error:{claimResourcesResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_ClaimResourcesError };
                    }

                    var result = await buildingPlace.BuildPlace.Start(type, positionedBuildWrapper);
                    if (!result)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error starting element", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_StartElement };
                    }

                    return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Success, ElementId = newBuild.Id };
                }
            }
            else if ((type == BuildType.FenceElement) || (type == BuildType.FenceAttachment))
            {
                bool cantBuildHere = !BuildingEngineHelper.CanBuildHere(data.Position, EntitiesRepository.GetSceneForEntity(new OuterRef<IEntity>(this)), BuildUtils.DefaultFencePlaceDef, false);
                if (cantBuildHere)
                {
                    BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, fence is not allowed to build at this position", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_FencePositionNotAllowed };
                }
                if (placeId == Guid.Empty)
                {
                    //TODO поддержать межсерверное строительство (когда персонаж не на том сервере, на котором ставится забор)
                    using (var wrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid))
                    {
                        var worldSpaceEntity = wrapper.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid);
                        if (worldSpaceEntity == null)
                        {
                            BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, world space entity not found", MethodBase.GetCurrentMethod().DeclaringType.Name);
                            return new OperationResult { Type = OperationType.Create, Result = ErrorCode.Error_CantFindWorldSpaceEntity };
                        }

                        placeId = await worldSpaceEntity.GetFencePlaceId(data.Position, false);
                        if (placeId == Guid.Empty)
                        {
                            BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, fence place not created", MethodBase.GetCurrentMethod().DeclaringType.Name);
                            return new OperationResult { Type = OperationType.Create, Result = ErrorCode.Error_PlaceNotCreated };
                        }
                    }
                }
                using (var wrapper = await EntitiesRepository.Get<IFencePlace>(placeId))
                {
                    var fencePlace = wrapper.Get<IFencePlace>(placeId);
                    if (fencePlace == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, fence place not found", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, Result = ErrorCode.Error_PlaceNotFound };
                    }
                    //TODO building: IUnityWorldSpace check geometry validation and element validation
                    //worldSpaceServiceEntity.UnityWorldSpace.CanBuildFenceHere()

                    var checkResourcesResult = BuildResourceManager.CheckResources(buildRecipeDef, this);
                    if (checkResourcesResult == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Check resources returned null result", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CheckResourcesNull };
                    }
                    if (checkResourcesResult.Result != ResourceOperationResultCode.Success)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, Check resources returned error:{checkResourcesResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ((checkResourcesResult.Result == ResourceOperationResultCode.ContainerItemOperationNotEnoughResources) ? ErrorCode.Error_CheckResourcesNotEnough : ErrorCode.Error_CheckResourcesError) };
                    }

                    var newBuild = BuildPlace.CreatePositionedBuild(type, buildRecipeDef, data, new OuterRef<IEntity>(this));
                    if (newBuild == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error creating positioned build data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CreatePositionedBuild };
                    }

                    var positionedBuildWrapper = new IPositionedBuildWrapper() { PositionedBuild = newBuild };

                    var checkResult = await fencePlace.BuildPlace.Check(type, positionedBuildWrapper);
                    if (!checkResult)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error checking element", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_CheckElement };
                    }

                    var claimResourcesResult = await BuildResourceManager.ClaimResources(checkResourcesResult.ClaimItems, this.EntitiesRepository);
                    if (claimResourcesResult == null)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, claim resources returned null result", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_ClaimResourcesNull };
                    }
                    if (claimResourcesResult.Result != ResourceOperationResultCode.Success)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, claim resources returned error:{claimResourcesResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_ClaimResourcesError };
                    }

                    var result = await fencePlace.BuildPlace.Start(type, positionedBuildWrapper);
                    if (!result)
                    {
                        BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, error starting element", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_StartElement };
                    }

                    return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Success, ElementId = newBuild.Id };
                }

            }
            return new OperationResult { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_UnknownBuildType };
        }

        public async Task<OperationResultEx> OperateBuildElementImpl(BuildType type, Guid placeId, Guid elementId, OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (data == null)
            {
                BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, param operation data is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResultEx { Type = OperationType.None, BuildType = type, Result = ErrorCode.Error_InvalidOperationData, OperationData = null };
            }

            if (type == BuildType.Any)
            {
                var result = await OperateBuildingElementInternal(type, placeId, elementId, data);
                if (result.Result == ErrorCode.Error_PlaceNotFound)
                {
                    result = await OperateFenceElementInternal(type, placeId, elementId, data);
                }
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, operation: {data.Type}, error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
                return result;
            }
            else if (type == BuildType.BuildingElement || type == BuildType.BuildingAttachment)
            {
                var result = await OperateBuildingElementInternal(type, placeId, elementId, data);
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, operation: {data.Type}, error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
                return result;
            }
            else if (type == BuildType.FenceElement || type == BuildType.FenceAttachment)
            {
                var result = await OperateFenceElementInternal(type, placeId, elementId, data);
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, operation: {data.Type}, error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
                return result;
            }

            return new OperationResultEx { Type = OperationType.Create, BuildType = type, Result = ErrorCode.Error_UnknownBuildType, OperationData = null };
        }

        private async Task<OperationResultEx> OperateBuildingElementInternal(BuildType type, Guid placeId, Guid elementId, OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            using (var wrapper = await EntitiesRepository.Get<IBuildingPlaceServer>(placeId))
            {
                var buildingPlace = wrapper.Get<IBuildingPlaceServer>(placeId);
                if (buildingPlace == null)
                    return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_PlaceNotFound, ElementId = elementId, OperationData = null };

                var result = await buildingPlace.BuildPlace.Operate(type, this.Id, elementId, data);
                return result;
            }
        }

        private async Task<OperationResultEx> OperateFenceElementInternal(BuildType type, Guid placeId, Guid elementId, OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, type: {type}, placeId: {placeId}, elementId: {elementId}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            using (var wrapper = await EntitiesRepository.Get<IFencePlaceServer>(placeId))
            {
                var fencePlace = wrapper.Get<IFencePlaceServer>(placeId);
                if (fencePlace == null)
                    return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_PlaceNotFound, ElementId = elementId, OperationData = null };

                var result = await fencePlace.BuildPlace.Operate(type, this.Id, elementId, data);
                return result;
            }
        }

        public Task<OperationResult> SetBuildCheatImpl(OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (data == null)
            {
                BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, param operation data is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(new OperationResult { Type = OperationType.None, Result = ErrorCode.Error_InvalidOperationData });
            }
            if (data.Type == OperationType.CheatDamage)
            {
                var cheatDamageData = data as CheatDamageData;
                if (cheatDamageData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                BuildUtils.SetCheatDamage(cheatDamageData.Enable, cheatDamageData.Value);
                BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, enable: {cheatDamageData.Enable}, value: {cheatDamageData.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Success });
            }
            else if (data.Type == OperationType.CheatClaimResources)
            {
                var cheatClaimResourcesData = data as CheatClaimResourcesData;
                if (cheatClaimResourcesData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                BuildUtils.SetCheatClaimResources(cheatClaimResourcesData.Enable, cheatClaimResourcesData.Value);
                BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, enable: {cheatClaimResourcesData.Enable}, value: {cheatClaimResourcesData.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Success });
            }
            else if (data.Type == OperationType.CheatDebug)
            {
                var cheatDebugData = data as CheatDebugData;
                if (cheatDebugData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                BuildUtils.SetCheatDebug(cheatDebugData.Enable, cheatDebugData.Verboose);
                BuildUtils.Message?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, enable: {cheatDebugData.Enable}, verbose: {cheatDebugData.Verboose}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Success });
            }
            return Task.FromResult(new OperationResult { Type = data.Type, Result = ErrorCode.Error_UnknownOperationType });
        }

        public Task<OperationResultEx> GetBuildCheatImpl(OperationData data)
        {
            //BuildUtils.Debug?.Report(true, $"characterId: {this.Id}, characterName: {this.Name}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (data == null)
            {
                BuildUtils.Error?.Report($"character: {this.Id}, param operation data is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(new OperationResultEx { Type = OperationType.None, Result = ErrorCode.Error_InvalidOperationData });
            }
            if (data.Type == OperationType.CheatDamage)
            {
                var cheatDamageData = data as CheatDamageData;
                if (cheatDamageData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                cheatDamageData.Enable = BuildUtils.CheatDamageEnable;
                cheatDamageData.Value = BuildUtils.CheatDamageValue;
                return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Success, OperationData = cheatDamageData });
            }
            else if (data.Type == OperationType.CheatClaimResources)
            {
                var cheatClaimResourcesData = data as CheatClaimResourcesData;
                if (cheatClaimResourcesData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                cheatClaimResourcesData.Enable = BuildUtils.CheatClaimResourcesEnable;
                cheatClaimResourcesData.Value = BuildUtils.CheatClaimResourcesValue;
                return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Success, OperationData = cheatClaimResourcesData });
            }
            else if (data.Type == OperationType.CheatDebug)
            {
                var cheatDebugData = data as CheatDebugData;
                if (cheatDebugData == null)
                {
                    BuildUtils.Error?.Report($"characterId: {this.Id}, characterName: {this.Name}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Error_InvalidOperationData });
                }

                cheatDebugData.Enable = BuildUtils.CheatDebugEnable;
                cheatDebugData.Verboose = BuildUtils.CheatDebugVerboose;
                return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Success, OperationData = cheatDebugData });
            }
            return Task.FromResult(new OperationResultEx { Type = data.Type, Result = ErrorCode.Error_UnknownOperationType });
        }
        // Build system end -----------------------------------------------------------------------

        public async Task<bool> InvokeItemDroppedImpl(BaseItemResource item, int count)
        {
            await OnItemDroppedEvent(item, count);
            return true;
        }

        private async Task DoOnDie(Guid id, int typeId, PositionRotation corpsePlace)
        {
            TemporaryPerks.Items.Clear();
            await DropCorpse();
            LogDeath(EntitiesRepository, MapOwner.OwnerMapId);
        }

        private void LogDeath(IEntitiesRepository repo, Guid mapId)
        {
            async ValueTask<MapDef> GetMap()
            {
                using (var wrapper = await repo.Get<IMapEntityAlways>(mapId))
                {
                    var mapEnt = wrapper.Get<IMapEntityAlways>(mapId);
                    return mapEnt?.Map;
                }
            }

            var name = Name;
            var charId = Id;
            var pos = MovementSync.Position;
            var session = SessionId;

            AsyncUtils.RunAsyncTask(async () =>
            {
                var map = await GetMap();
                Telemetry.WorldCharacterEvents.Dead(name,charId, session, pos, map);
            }, repo);
        }

        public async Task OnBeforeResurrectEventImpl(Guid id, int typeId)
        {
            var defaultCharacterDef = Gender.DefaultCharacter.Target;
            if (defaultCharacterDef.DefaultDoll.DefaultItems != null)
            {
                var dollAddress = EntityPropertyResolver.GetPropertyAddress(Doll);
                using (var container = await this.GetThisRead())
                    await AddItems(
                        await ClusterHelpers.ResolveDefaultItems(
                            defaultCharacterDef.DefaultDoll.DefaultItems,
                            new CalcerContext(container, new OuterRef(Id, TypeId), EntitiesRepository)),
                        dollAddress);
            }

            if (defaultCharacterDef.DefaultInventory.DefaultItems != null)
            {
                var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(Inventory);
                using (var container = await this.GetThisRead())
                    await AddItems(
                        await ClusterHelpers.ResolveDefaultItems(
                            defaultCharacterDef.DefaultInventory.DefaultItems,
                            new CalcerContext(container, new OuterRef(Id, TypeId), EntitiesRepository)),
                        inventoryAddress);
            }
        }

        public ValueTask<bool> ChangeHealthInternalImpl(float deltaValue)
        {
            return new ValueTask<bool>(false);
        }

        public ValueTask<DamageResult> ReceiveDamageInternalImpl(Damage damage, Guid aggressorId, int aggressorTypeId)
        {
            return new ValueTask<DamageResult>(DamageResult.None);
        }

        public Task GatherResourcesImpl(OuterRef<IEntity> giver, List<ItemResourcePack> items)
        {
            return OnGatherResourcesEvent(giver, items);
        }

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }

        public Task<bool> AllowedSpawnPointSetImpl(SpawnPointTypeDef allowedSpawnPoint)
        {
            AllowedSpawnPoint = allowedSpawnPoint;
            return Task.FromResult(true);
        }

        // --- ResourceMiner implementation --------------------------------------
        #region ResourceMiner


        public Task OnUnload()
        {
            IEntityExt ext = this;
            if (ext.IsMaster())

                LogLeftWorld();
            return Task.CompletedTask;
        }

        private void LogLeftWorld()
        {
            var repo = EntitiesRepository;
            var mapId = MapOwner.OwnerMapId;

            async ValueTask<MapDef> GetMap()
            {
                using (var wrapper = await repo.Get<IMapEntityAlways>(mapId))
                {
                    var mapEnt = wrapper.Get<IMapEntityAlways>(mapId);
                    return mapEnt?.Map;
                }
            }

            var name = Name;
            var session = SessionId;
            var pos = MovementSync.Position;
            var id = Id;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrap = await repo.GetFirstService<ISessionHolderEntityServer>())
                {
                    var service = wrap?.GetFirstService<ISessionHolderEntityServer>();
                    if (service != null)
                    {
                        await service.Unregister(id);
                    }
                }

                var map = await GetMap();
                Telemetry.WorldCharacterEvents.LeftWorld(name, id, session, pos, map);
            }, repo);
        }

        #endregion //ResourceMiner

        private ThreadSafeList<Guid> _removedPointMarkers = new ThreadSafeList<Guid>();

        public Task AddPointMarkerImpl(Guid pointMarkerGuid, PointMarker pointMarker)
        {
            if (pointMarker.AssertIfNull(nameof(pointMarker)))
                return Task.CompletedTask;

            if (PointMarkers.ContainsKey(pointMarkerGuid) || _removedPointMarkers.Contains(pointMarkerGuid))
                return Task.CompletedTask;

            PointMarkers.Add(pointMarkerGuid, pointMarker);
            return Task.CompletedTask;
        }

        public Task RemovePointMarkerImpl(Guid pointMarkerGuid)
        {
            PointMarker pointMarker;
            if (!PointMarkers.TryGetValue(pointMarkerGuid, out pointMarker))
            {
                Logger.IfWarn()?.Message($"Unable to remove PointMarker with guid={pointMarkerGuid}").Write();
                return Task.CompletedTask;
            }

            _removedPointMarkers.Add(pointMarkerGuid);
            PointMarkers.Remove(pointMarkerGuid);
            return Task.CompletedTask;
        }

        public Task AddPointOfInterestImpl(PointOfInterestDef poi)
        {
            if (!poi.AssertIfNull(nameof(poi)) && !PointsOfInterest.Contains(poi))
                PointsOfInterest.Add(poi);
            return Task.CompletedTask;
        }

        public Task RemovePointOfInterestImpl(PointOfInterestDef poi)
        {
            if (!poi.AssertIfNull(nameof(poi)))
                PointsOfInterest.Remove(poi);
            return Task.CompletedTask;
        }

        public Task UnstuckTeleportImpl(float minTimeout)
        {
            var timeout = Math.Min(minTimeout, GlobalConstsHolder.GlobalConstsDef.UnstuckTeleportTimeout);
            Logger.IfInfo()?.Message($"2. UnstuckTeleportImpl({timeout})").Write();
            //if (DbgLog.Enabled) DbgLog.Log("2. UnstuckTeleportImpl");
            this.Chain().Delay(timeout).UnstuckTeleportDo().Run();
            return Task.CompletedTask;
        }

        public async Task UnstuckTeleportDoImpl()
        {
            Logger.IfInfo()?.Message("3. UnstuckTeleportDo").Write();
            //if (DbgLog.Enabled) DbgLog.Log("3. UnstuckTeleportDo");

            // Get UnityCheatService by unityId
            Transform closestSpawnPointTransform = default;
            using (var wsseWrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntityServer>(WorldSpaced.OwnWorldSpace.Guid))
            {
                var wsse = wsseWrapper.Get<IWorldSpaceServiceEntity>(WorldSpaced.OwnWorldSpace.Guid);
                var spawnPointPosition = await wsse.GetPositionToSpawnAt(Id, false, false, default);
                closestSpawnPointTransform = new Transform(spawnPointPosition.Position, spawnPointPosition.Rotation);
                Logger.IfDebug()?.Message($"Got SpawnPoint: {spawnPointPosition.Position}.").Write();
            }

            if (closestSpawnPointTransform.Equals(default(Transform)))
            {
                Logger.IfError()?.Message("Can't get closestSpawnPointTransform.").Write();
                return;
            }

            //if (DbgLog.Enabled) DbgLog.Log($"6. UnstuckTeleportDo before Set. Pos: {MovementSync.Position} & Rot: {MovementSync.Rotation.eulerAngles} 'll be set to {closestSpawnPointTransform.Position} & {closestSpawnPointTransform.Rotation.eulerAngles}");
            MovementSync.SetTransform = closestSpawnPointTransform;
            //if (DbgLog.Enabled) DbgLog.Log($"7. UnstuckTeleportDo after Set. Pos: {MovementSync.Position} & Rot: {MovementSync.Rotation.eulerAngles}");
        }

        public async Task<bool> AFKStateMachineChainCallImpl()
        {
            Logger.IfWarn()?.Message($"AFKStateMachineChainCallImpl for {Id}").Write();
            var deltaTime = TimeWhenUserDisconnected != 0 ? SyncTime.ToSeconds(SyncTime.NowUnsynced - TimeWhenUserDisconnected) : 0;
            if (deltaTime > ((WorldCharacterDef)Def).AFKStateMachine.TimeToBecomeIdleAndInteractive && !IsAFK)
            {
                Logger.IfWarn()?.Message($"AFKStateMachineChainCallImpl got into idle {Id}").Write();
                IsAFK = true;
                await GoIntoIdleMode();
                using (var wiz = await EntitiesRepository.Get<IWizardEntity>(Wizard.Id))
                {
                    deltaTime = TimeWhenUserDisconnected != 0 ? SyncTime.ToSeconds(SyncTime.NowUnsynced - TimeWhenUserDisconnected) : 0;
                    if (deltaTime > ((WorldCharacterDef)Def).AFKStateMachine.TimeToBecomeIdleAndInteractive)
                        await wiz.Get<IWizardEntity>(Wizard.Id).GoIntoIdleMode();
                }
            }
            if (deltaTime > ((WorldCharacterDef)Def).AFKStateMachine.TimeToDieAndUnload)
            {
                if (false)//тут нужно проверить, что мы должны перед выгрузкой умереть, например, что мы залогаутились на улице
                    await Mortal.Die();
                Logger.IfWarn()?.Message($"AFKStateMachineChainCallImpl unload {Id}").Write();
                _ = AsyncUtils.RunAsyncTask(() => EntitiesRepository.Destroy(TypeId, Id, true));
            }
            return true;
        }

        private Task GoIntoIdleMode() => OnOnIdleModeStartedInvoke();

        public async Task<bool> NotifyThatClientIsGoneImpl()
        {
            if (IsIdle)
                return true;
            IsIdle = true;
            TimeWhenUserDisconnected = SyncTime.NowUnsynced;
            Logger.IfWarn()?.Message($"Client for {Id} is gone").Write();
            this.Chain().Delay(((WorldCharacterDef)Def).AFKStateMachine.TimeToBecomeIdleAndInteractive + 1).AFKStateMachineChainCall().Run();
            this.Chain().Delay(((WorldCharacterDef)Def).AFKStateMachine.TimeToDieAndUnload + 1).AFKStateMachineChainCall().Run();
            await MovementSync.NotifyThatClientIsGone();
            return true;
        }
        
        public async Task<bool> NotifyThatClientIsBackImpl()
        {
            if (!IsIdle)
                return true;
            IsIdle = false;
            IsAFK = false;
            TimeWhenUserDisconnected = 0;
            await GetBackFromIdleMode();
            using (var wiz = await EntitiesRepository.Get<IWizardEntity>(Wizard.Id))
            {
                await wiz.Get<IWizardEntity>(Wizard.Id).GetBackFromIdleMode();
            }
            return true;
        }

        private Task GetBackFromIdleMode() => OnOnIdleModeStoppedInvoke();
        public async Task<bool> DropCorpseImpl()
        {
            WorldCorpseDef worldCorpseDef = ((WorldCharacterDef)Def).CorpseDef;
            Vector3 position = MovementSync.Transform.Position;
            Quaternion rotation = MovementSync.Transform.Rotation;
            PropertyAddress corpseInventoryAddress = null;
            PropertyAddress corpseDollAddress = null;
            bool visibleOnlyForOwner = Faction.CorpsesVisibleOnlyForOwner != null && await Faction.CorpsesVisibleOnlyForOwner.Target.CalcAsync(new OuterRef<IEntity>(ParentEntityId, ParentTypeId), EntitiesRepository);
            var newItemRef = await EntitiesRepository.Create<IWorldCorpse>(Guid.NewGuid(), (IWorldCorpse createdItem) =>
            {
                createdItem.Doll.Size = this.Doll.Size;
                createdItem.Inventory.Size = this.Inventory.Size + this.Doll.Size;//добавляем this.Doll.Size, так как пока временный костыль - с куклы предметы потом переезжают в инвентарь 
                createdItem.OwnerInformation.Owner = new OuterRef<IEntity>(this);
                createdItem.OwnerInformation.AccessPredicate = Faction.RelationshipRules.Target.CorpseAccessPredicate;
                createdItem.MovementSync.SetTransform = new Transform(position, rotation);
                createdItem.Def = worldCorpseDef;
                createdItem.WorldSpaced.OwnWorldSpace = WorldSpaced.OwnWorldSpace;
                createdItem.MapOwner = MapOwner;
                createdItem.VisibleOnlyForOwner = visibleOnlyForOwner;

                return Task.CompletedTask;
            });

            //Держим залоченным труп на все время перекладывания туда айтемов
            using (var wrapper = await EntitiesRepository.Get(newItemRef.TypeId, newItemRef.Id))
            {
                var entity = wrapper.Get<IWorldCorpseServer>(newItemRef.Id);
                if (entity == null)
                {
                    Logger.IfError()?.Message("Failed to create corpse").Write();
                    return default;
                }

                corpseInventoryAddress = EntityPropertyResolver.GetPropertyAddress(entity.Inventory);
                corpseDollAddress = EntityPropertyResolver.GetPropertyAddress(entity.Doll);

                var moveTransactions = new List<ItemMoveManagementTransaction>();
                List<RemoveItemBatchElement> removeItems = new List<RemoveItemBatchElement>();

                var sourceDollAddress = EntityPropertyResolver.GetPropertyAddress(Doll);
                foreach (var item in Doll.Items)
                {
                    var calcerOnDeath = (item.Value.Item.ItemResource as ItemResource)?.ActionOnDeathCalcer.Target;
                    ActionOnDeathDef actionOnDeath = Constants.ItemConstants.MoveToCorpse.Target;
                    if (calcerOnDeath != null)
                    {
                        actionOnDeath = await calcerOnDeath.CalcAsync(new OuterRef<IEntity>(Id, TypeId), EntitiesRepository) as ActionOnDeathDef;
                    }

                    if (actionOnDeath == Constants.ItemConstants.LeaveAtCharacter)
                        continue;

                    if (actionOnDeath == Constants.ItemConstants.Destroy)
                    {
                        removeItems.Add(new RemoveItemBatchElement(sourceDollAddress, item.Key, item.Value.Stack, item.Value.Item.Id));
                    }
                    else if (actionOnDeath == Constants.ItemConstants.MoveToCorpse)
                    {
                        var moveInventoryItemTransaction = new ItemMoveManagementTransaction(sourceDollAddress, item.Key, corpseInventoryAddress,
                            -1, item.Value.Stack, item.Value.Item.Id, false, EntitiesRepository);
                        moveTransactions.Add(moveInventoryItemTransaction);
                    }
                }

                var sourceInventoryAddress = EntityPropertyResolver.GetPropertyAddress(Inventory);
                foreach (var item in Inventory.Items)
                {
                    var calcerOnDeath = (item.Value.Item.ItemResource as ItemResource)?.ActionOnDeathCalcer.Target;
                    ActionOnDeathDef actionOnDeath = Constants.ItemConstants.MoveToCorpse.Target;
                    if (calcerOnDeath != null)
                    {
                        actionOnDeath = await calcerOnDeath.CalcAsync(new OuterRef<IEntity>(Id, TypeId), EntitiesRepository) as ActionOnDeathDef;
                    }

                    if (actionOnDeath == Constants.ItemConstants.LeaveAtCharacter)
                        continue;

                    if (actionOnDeath == Constants.ItemConstants.Destroy)
                    {
                        removeItems.Add(new RemoveItemBatchElement(sourceInventoryAddress, item.Key, item.Value.Stack, item.Value.Item.Id));
                    }
                    else if (actionOnDeath == Constants.ItemConstants.MoveToCorpse)
                    {
                        var moveInventoryItemTransaction = new ItemMoveManagementTransaction(sourceInventoryAddress, item.Key,
                            corpseInventoryAddress, -1, item.Value.Stack, item.Value.Item.Id, false, EntitiesRepository);
                        moveTransactions.Add(moveInventoryItemTransaction);
                    }
                }

                if (removeItems.Count > 50)
                    Logger.IfError()?.Message("DropCorpseImpl {0} {1} too many remove items in transaction {2}", TypeName, Id, removeItems.Count).Write();

                var removeTransaction = new ItemRemoveBatchManagementTransaction(removeItems, false, EntitiesRepository);
                var removeResult = await removeTransaction.ExecuteTransaction();
                if (!removeResult.IsSuccess)
                    Logger.IfError()?.Message($"Can't remove item on transaction = {removeTransaction.ToString()}").Write();

                if (moveTransactions.Count > 50)
                    Logger.IfError()?.Message("DropCorpseImpl {0} {1} too many move transactions {2}", TypeName, Id, moveTransactions.Count).Write();

                foreach (var transaction in moveTransactions)
                {
                    var result = await transaction.ExecuteTransaction();
                    if (!result.IsSuccess)
                    {
                        Logger.IfError()?.Message($"Can't move item on transaction = {transaction.ToString()}").Write();
                    }
                }

                return true;
            }
        }

        public Task<bool> ActivateCommonBakenImpl(Guid baken)
        {
            //Design states that in future you might have more than one activated common baken, but for now I clear and activate new
            ActivatedCommonBakens.Clear();
            if (baken == Guid.Empty)
            {
                LastActivatedWasCommonBaken = false;
                return Task.FromResult(true);
            }
            ActivatedCommonBakens[baken] = 0;
            LastActivatedWasCommonBaken = true;
            return Task.FromResult(true);
        }

        public Task<string> TestCheatRpcImpl(string argument)
        {
            Console.WriteLine("TestCheat");
            return Task.FromResult("test");
        }

        // #tmp: usings: НЕТ
        public async ValueTask<int> ResyncAccountExperienceImpl()
        {
            Guid accountId;
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                var entity = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                var userGuid = await entity.GetUserIdByCharacterId(Id);
                var accData = await entity.GetAccountDataByUserId(userGuid);
                accountId = accData.AccountId;
            }

            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
            {
                var entity = wrapper.GetFirstService<ILoginServiceEntityServer>();
                AccountStats.AccountExperience = await entity.GetAccountExperience(accountId); //was: Consumed := Exp ???
                return AccountStats.AccountExperience;
            }
        }


        public async Task SuicideImpl()
        {
            var hasMortal = (IHasMortal) this;
            await hasMortal.Mortal.Die();
        }
		
        public ValueTask SetGenderImpl(GenderDef gender)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Set Gender | GendeR:{gender?.____GetDebugAddress()}").Write();
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            return new ValueTask();
        }

        public Task SuicideCheatImpl() => Suicide();

        public ItemSpecificStats SpecificStats => (ClusterHelpers.GetActiveWeaponResource(GetReplicationLevel(ReplicationLevel.ClientBroadcast) as IHasDollClientBroadcast).Key as ItemResource)?.SpecificStats.Target ?? ((WorldCharacterDef)Def).DefaultStats;

        public ValueTask<CalcerDef<float>> GetIncomingDamageMultiplierImpl() => new ValueTask<CalcerDef<float>>(Faction?.RelationshipRules.Target?.IncomingDamageMultiplier.Target.GetCalcer());
    }
}
