using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.Transactions;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using ColonyShared.SharedCode.Entities;
using Core.Environment.Logging.Extension;
using static Assets.ColonyShared.GeneratedCode.Shared.GlobalLoggers;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem.Delta;
using SharedCode.DeltaObjects;
using Assets.Src.Arithmetic;
using SharedCode.Wizardry;
using SharedCode.Refs;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace GeneratedCode.DeltaObjects
{
    public partial class MutationMechanics : IHookOnInit, IHookOnStart
    {
        private IEntity _entityHolder => parentEntity;
        private IHasPerks _entityPerks => (IHasPerks)parentEntity;
        private IHasDoll _entityDoll => (IHasDoll)parentEntity;
        private IHasInventory _entityInventory => (IHasInventory)parentEntity;
        private IMortal _entityMortal => ((IHasMortal)parentEntity).Mortal;
        private IQuestEngine _entityQuest => ((IHasQuestEngine)parentEntity).Quest;
        private EntityRef<IWizardEntity> _entityWizardRef => ((IHasWizardEntity)parentEntity).Wizard; 

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private void SetFaction(MutatingFactionDef faction)
        {
            Faction = faction;
            ((IHasFaction) parentEntity).Faction = faction;
        }
        
        public Task<bool> CanChangeMutationImpl(float deltaValue, MutatingFactionDef toFaction)
        {
            return MakeMutationChanges(deltaValue, toFaction, false, 0, false);
        }

        public Task<bool> ChangeMutationImpl(float deltaValue, MutatingFactionDef toFaction, float coolDownTime, bool forceChange)
        {
            return MakeMutationChanges(deltaValue, toFaction, true, coolDownTime, forceChange);
        }

        private async Task<bool> MakeMutationChanges(float deltaValue, [CanBeNull]MutatingFactionDef toFaction, bool makeChange, float coolDownTime, bool forceChange)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"MakeMutationChanges | deltaValue:{deltaValue} toFaction:{toFaction} coolDownTime:{coolDownTime} forceChange:{forceChange} makeChange:{makeChange}").Write();
            
            if(toFaction != null && !FactionsDef.Factions.Any(v => v.Target == toFaction))
                return false;

            var now = SyncTime.Now;
            if (now >= AllowedTimeMutationChange)
            {
                MutationState oldMutationState = new MutationState(Faction, Stage, NewFaction, NewStage, Mutation);
                MutationState newMutationState = ChangeFactionState(oldMutationState, deltaValue, toFaction);
                var changed = newMutationState != oldMutationState;

                if (changed && makeChange)
                {
                    Mutation = newMutationState.Mutation;
                    if (forceChange /*|| newMutationState.Faction != newMutationState.NewFaction*/)
                    {
                        await ApplyMutationChange(newMutationState.NewStage, newMutationState.NewFaction);
                    }
                    else
                    {
                        NewStage = newMutationState.NewStage;
                        NewFaction = newMutationState.NewFaction;
                        AllowedTimeMutationChange = now + (long)(coolDownTime * 1000f);
                    }

                    //Logger.IfInfo()?.Message($"{oldMutationState} --> \n{newMutationState}").Write();
                }

                return changed;
            }

            return false;
        }

        public async Task<bool> ApplyMutationChangeForcedImpl(MutationStageDef newStage, MutatingFactionDef newFaction)
        {
            await ApplyMutationChange(newStage, newFaction);
            //await _entity.InvokeStateForceChanged();
            return true;
        }

        private async Task ApplyMutationChange(MutationStageDef newStage, MutatingFactionDef newFaction, bool applyEntityChanges = true)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"ApplyMutationChange | NewStage:{newStage} NewFaction:{newFaction}").Write();

            if (!FactionsDef.Factions.Any(v => v.Target == newFaction))
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Faction {newFaction.____GetDebugAddress()} is not exists in {FactionsDef.____GetDebugAddress()}").Write();
                return;
            }

            var oldStage = Stage;
            if (newStage != null)
                Stage = newStage;
            if (newFaction != null)
                SetFaction(newFaction);

            await StageProcessing();
        }
        private async Task StageProcessing()
        {
            await ChangePerksItem(FactionsDef, Stage);

            if (Faction?.RemoveAllItems ?? false)
            {
                await ClearInventory();
                await LockAllSlot();
            }
            else
                await ChangeStageItems(FactionsDef, Stage);

            if (Faction?.RemoveAllQuests ?? false)
                await ClearQuests();

            if (Stage?.StageQuests?.Length > 0)
                await AddStageQuests();

            if (Stage?.StageSpells?.Length > 0)
                await CastStageSpells();

        }
        private async Task ChangePerksItem(MutatingFactionsDef factions, MutationStageDef newStage)
        {
            if (_entityPerks != null)
            {
                if (_entityPerks.TemporaryPerks.Items.Any() 
                    || (Faction.RemoveAllPerks && (
                        _entityPerks.PermanentPerks.Items.Any() 
                        || _entityPerks.SavedPerks.Items.Any())))
                {
                    var notNewStages = factions.Factions.SelectMany(v => v.Target.Stages).Where(v => v.Target != newStage);

                    IEnumerable<BaseItemResource> notNewStagePerks = (notNewStages.Where(v=> v.Target != null && v.Target.StagePerks != null).SelectMany(v => v.Target.StagePerks) ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                        .SelectMany(v => v.Target.Items)
                        .Where(v => v.Target != null)
                        .Select(v => v.Target);

                    var TempPerkAddress = EntityPropertyResolver.GetPropertyAddress(_entityPerks.TemporaryPerks);
                   
                    var perksToRemove = new List<RemoveItemBatchElement>();
                    foreach (var perkSlot in _entityPerks.TemporaryPerks.Items)
                    {
                        if (Faction.RemoveAllPerks || notNewStagePerks.Contains(perkSlot.Value.Item.ItemResource))
                        {
                            perksToRemove.Add(new RemoveItemBatchElement(TempPerkAddress, perkSlot.Key, 1, Guid.Empty));
                        }
                    }
                    if (Faction.RemoveAllPerks)
                    {
                        var PermPerkAddress = EntityPropertyResolver.GetPropertyAddress(_entityPerks.PermanentPerks);
                        var SavedPerkAddress = EntityPropertyResolver.GetPropertyAddress(_entityPerks.SavedPerks);

                        foreach (var perkSlot in _entityPerks.PermanentPerks.Items)
                            perksToRemove.Add(new RemoveItemBatchElement(PermPerkAddress, perkSlot.Key, 1, Guid.Empty));

                        foreach (var perkSlot in _entityPerks.SavedPerks.Items)
                            perksToRemove.Add(new RemoveItemBatchElement(SavedPerkAddress, perkSlot.Key, 1, Guid.Empty));
                    }

                    ItemRemoveBatchManagementTransaction removeTransaction = new ItemRemoveBatchManagementTransaction(perksToRemove, false, _entityHolder.EntitiesRepository);
                    await removeTransaction.ExecuteTransaction();

                    var permPerkIds = _entityPerks.PermanentPerks.PerkSlots.Keys.ToList();
                    foreach (var item in permPerkIds)
                        await _entityPerks.PermanentPerks.RemovePerkSlot(item);
                }

                // Добавление новых перков
                var newPerks = (newStage?.StagePerks ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                    .SelectMany(v => v.Target.Items)
                    .Where(v => v.Target != null)
                    .Select(v => v.Target).ToList();

                if(Faction.RemoveAllPerks)
                    _entityPerks.PermanentPerks.Size = newPerks.Count;

                if (newPerks.Count == 0)
                    return;

                IDeltaObject perkContainer;
                IDeltaDictionary<int, ISlotItem> perksOnCharacter;
                if (newStage.AddStagePerksToPermanent)
                {
                    perkContainer = _entityPerks.PermanentPerks;
                    perksOnCharacter = _entityPerks.PermanentPerks.Items;
                    int count = 0;
                    foreach (var item in newPerks)
                    {
                        if (item is PerkResource perk)
                        {
                            await _entityPerks.PermanentPerks.AddPerkSlot(count, perk.ItemType);
                            count++;
                        }
                    }
                }
                else
                {
                    perkContainer = _entityPerks.TemporaryPerks;
                    perksOnCharacter = _entityPerks.TemporaryPerks.Items;
                }

                var PerksAddress = EntityPropertyResolver.GetPropertyAddress(perkContainer);
                foreach (var perk in newPerks)
                {
                    if (!perksOnCharacter.Any(v => v.Value.Item.ItemResource == perk))
                    {
                        var itemTransaction = new ItemAddBatchManagementTransaction(new List<ItemResourcePack>() { new ItemResourcePack(perk, 1) }, PerksAddress, false, _entityHolder.EntitiesRepository);
                        var result = await itemTransaction.ExecuteTransaction();
                    }
                }
            }
        }
        private async Task ChangeStageItems(MutatingFactionsDef factions, MutationStageDef newStage)
        {
            // Перемещение из Doll в инвентарь предметов, которые стали недоступны при смене Стадии
            if (_entityDoll != null && _entityInventory != null)
            {
                if (_entityDoll.Doll.Items.Any())
                {
                    IEnumerable<BaseItemResource> allowedItems = (newStage?.Items ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                        .SelectMany(v => v.Target.Items)
                        .Where(v => v.Target != null)
                        .Select(v => v.Target);

                    var dollAddress = EntityPropertyResolver.GetPropertyAddress(_entityDoll.Doll);
                    var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(_entityInventory.Inventory);
                    List<ItemMoveManagementTransaction> moveTransactionList = new List<ItemMoveManagementTransaction>();
                    foreach (var itemSlot in _entityDoll.Doll.Items)
                    {
                        if (!allowedItems.Contains(itemSlot.Value.Item.ItemResource))
                        {
                            var sourceSlotId = itemSlot.Key;
                            var itemGuid = _entityDoll.Doll.Items[sourceSlotId].Item.Id;
                            moveTransactionList.Add(new ItemMoveManagementTransaction(dollAddress, sourceSlotId, inventoryAddress, -1, itemSlot.Value.Stack, itemGuid, false, _entityHolder.EntitiesRepository));
                        }
                    }

                    var unsuccessTransactions = new List<ItemMoveManagementTransaction>();
                    foreach (var moveTransaction in moveTransactionList)
                    {
                        var result = await moveTransaction.ExecuteTransaction();
                        if (!result.IsSuccess)
                        {
                            unsuccessTransactions.Add(moveTransaction);
                        }
                    }

                    // Костыль если вдруг что-то не перенеслось, пробуем еще раз (рюкзак, к примеру, если там что-то было)
                    var unsuccessTransactions2 = new List<ItemMoveManagementTransaction>();
                    foreach (var moveTransaction in unsuccessTransactions)
                    {
                        var result = await moveTransaction.ExecuteTransaction();
                        if (!result.IsSuccess) 
                        {
                            unsuccessTransactions2.Add(moveTransaction);
                        }
                    }
                    // если уж это не помогло - удаляем к х..м
                    var removeBatch = new ItemRemoveBatchManagementTransaction(unsuccessTransactions2, _entityHolder.EntitiesRepository);
                    await removeBatch.ExecuteTransaction();
                }
            }
        }
        private async ValueTask ClearInventory()
        {
            if (_entityDoll != null && _entityInventory != null)
            {
                if (_entityDoll.Doll.Items.Any() || _entityInventory.Inventory.Items.Any())
                {
                    var dollAddress = EntityPropertyResolver.GetPropertyAddress(_entityDoll.Doll);
                    PropertyAddress inventoryAddress = EntityPropertyResolver.GetPropertyAddress(_entityInventory.Inventory);
                    List<RemoveItemBatchElement> moveTransactionList = new List<RemoveItemBatchElement>();
                    foreach (var itemDollSlot in _entityDoll.Doll.Items)
                    {
                        var sourceSlotId = itemDollSlot.Key;
                        var itemGuid = _entityDoll.Doll.Items[sourceSlotId].Item.Id;
                        moveTransactionList.Add(new RemoveItemBatchElement(dollAddress, sourceSlotId, itemDollSlot.Value.Stack, itemGuid));
                    }
                    foreach (var itemInvSlot in _entityInventory.Inventory.Items)
                    {
                        var sourceSlotId = itemInvSlot.Key;
                        var itemGuid = _entityInventory.Inventory.Items[sourceSlotId].Item.Id;
                        moveTransactionList.Add(new RemoveItemBatchElement(inventoryAddress, sourceSlotId, itemInvSlot.Value.Stack, itemGuid));
                    }
                    var removeBatch = new ItemRemoveBatchManagementTransaction(moveTransactionList, false, _entityHolder.EntitiesRepository);
                    var opResult = await removeBatch.ExecuteTransaction();

                    //костыль
                    //if(!opResult.IsSuccess)
                    //{
                    moveTransactionList.Clear();
                    foreach (var itemDollSlot in _entityDoll.Doll.Items)
                    {
                        var sourceSlotId = itemDollSlot.Key;
                        var itemGuid = _entityDoll.Doll.Items[sourceSlotId].Item.Id;
                        moveTransactionList.Add(new RemoveItemBatchElement(dollAddress, sourceSlotId, itemDollSlot.Value.Stack, itemGuid));
                    }
                    foreach (var itemInvSlot in _entityInventory.Inventory.Items)
                    {
                        var sourceSlotId = itemInvSlot.Key;
                        var itemGuid = _entityInventory.Inventory.Items[sourceSlotId].Item.Id;
                        moveTransactionList.Add(new RemoveItemBatchElement(inventoryAddress, sourceSlotId, itemInvSlot.Value.Stack, itemGuid));
                    }
                    if (moveTransactionList.Count != 0)
                    {
                        var removeBatch2 = new ItemRemoveBatchManagementTransaction(moveTransactionList, false, _entityHolder.EntitiesRepository);
                        var opResult2 = await removeBatch2.ExecuteTransaction();
                    }
                    //}
                }

            }
        }
        private async Task LockAllSlot()
        {
            if (_entityDoll != null && _entityInventory != null)
            {
                var hasContainerApi = parentEntity as IHasContainerApi;
                PropertyAddress inventoryAddress = EntityPropertyResolver.GetPropertyAddress(_entityInventory.Inventory);
                await hasContainerApi.ContainerApi.ContainerOperationSetSize(inventoryAddress, 0);

                var character = (parentEntity as IWorldCharacter);
                if (character != null)
                {
                    var defaultCharacterDef = character.Gender.DefaultCharacter.Target;
                    _entityDoll.Doll.BlockedSlotsId.Clear();
                    for (int i = 0; i <= defaultCharacterDef.DefaultDoll.Size; i++)
                        _entityDoll.Doll.BlockedSlotsId.Add(i);
                }
            }
            
        }

        private async ValueTask ClearQuests()
        {
            if (_entityQuest != null)
                await _entityQuest.RemoveAllQuests();

        }
        private async ValueTask AddStageQuests()
        {
            if (_entityQuest != null)
            {
                foreach (var item in Stage.StageQuests)
                    if(item.Target != null)
                        await _entityQuest.AddQuest(item.Target);
            }

        }
        private async ValueTask CastStageSpells()
        {
            if (_entityWizardRef != null)
            {
                using(var wrapper = await EntitiesRepository.Get(_entityWizardRef.TypeId,_entityWizardRef.Id))
                {
                    if (wrapper == null)
                        return;

                    var wizard = wrapper.Get<IWizardEntityServer>(_entityWizardRef.TypeId, _entityWizardRef.Id, ReplicationLevel.Server);

                    if (wizard == null)
                        return;

                    foreach (var item in Stage.StageSpells)
                        if (item.Target != null)
                            await wizard.CastSpell(new SpellCast { Def = item.Target, StartAt = SyncTime.Now });
                }
                
            }

        }
        private static MutationState ChangeFactionState(MutationState currentState, float deltaValue, [CanBeNull]MutatingFactionDef toFaction)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"ChangeFactionState | currentState:{currentState} deltaValue:{deltaValue} toFaction:{toFaction}").Write();
            
            //Logger.IfInfo()?.Message($"currentState = {currentState}").Write();
            MutationState newState = new MutationState(currentState);
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"a) {newState}").Write();
 
            if (newState.NewFaction == null)
                newState.NewFaction = currentState.Faction;

            if (toFaction == null || currentState.NewFaction == toFaction)
            {
                newState.Mutation = currentState.Mutation + deltaValue;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"b1) newState.Mutation:{newState.Mutation}").Write();
            }
            else
            {
                deltaValue *= -1;

                if (deltaValue > 0)
                {
                    newState.Mutation = currentState.Mutation + deltaValue;
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"b2) newState.Mutation:{newState.Mutation}").Write();
                }
                else
                {
                    var newValue = currentState.Mutation + deltaValue;
                    if (newValue < 0)
                        newState.NewFaction = toFaction;

                    newState.Mutation = Math.Abs(newValue);
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"b3) newState.Mutation:{newState.Mutation} newState.NewFaction:{newState.NewFaction} newValue:{newValue}").Write();
                }
            }
            newState.Mutation = Math.Max(0, Math.Min(newState.Mutation, newState.NewFaction.MaxMutation));
            newState.NewStage = newState.NewFaction.Stages.Where(v => newState.Mutation <= v.Target.Boundary).OrderBy(v => v.Target.Boundary).First();

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"c) {newState}").Write();

            if (currentState.NewStage != null &&
                currentState.Stage != currentState.NewStage &&
                currentState.NewStage.IsHostStage &&
                !currentState.Stage.IsHostStage &&
                currentState.Faction != newState.NewFaction)
            {
                newState.NewStage = currentState.NewStage;
                if (newState.Mutation >= newState.NewStage.Boundary)
                    newState.Mutation = newState.NewStage.Boundary;
                
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"d) {newState.Mutation}").Write();
            }

            if (newState.NewStage is TransitionMutationStageDef tms)
            {
                newState = ChangeFactionState(newState, newState.Mutation + 1, tms.ToFaction);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"e) {newState.Mutation}").Write();
            }

            return newState;
        }
      
        public async Task OnInit()
        {
            FactionsDef = GameResourcesHolder.Instance.LoadResource<MutatingFactionsDef>("/Inventory/Factions/Factions");
            await ChangeMutation(FactionsDef.DefaultMutation, FactionsDef.DefaultFaction, 0, true);
            NewFaction = Faction;
            NewStage = Stage;
        }

        public Task OnStart()
        {
            if (SubscribeLogger.IsDebugEnabled) SubscribeLogger.IfDebug()?.Message($"Subscribe: [{((WorldCharacter)_entityHolder).DebugObjectWatchID}; {_entityHolder.Id} -> {GetType().Name}] -> [OnStart()]").Write();

            if (_entityMortal != null) 
            {
                if (SubscribeLogger.IsDebugEnabled) SubscribeLogger.IfDebug()?.Message($"Subscribe: [{((WorldCharacter)_entityHolder).DebugObjectWatchID}; {_entityHolder.Id} -> {GetType().Name}] -> [_entityMortal != null]").Write();

                _entityMortal.DieEvent += (guid, typeId, corpsePlace) =>
                {
                    if (NewStage != null && NewFaction != null)
                    {
                        if (NewStage != null)
                        {
                            Stage = NewStage;
                            NewStage = null;
                        }
                        if (NewFaction != null)
                        {
                            SetFaction(NewFaction);
                            NewFaction = null;
                        }
                    }
                    return Task.CompletedTask;
                };
                if (SubscribeLogger.IsDebugEnabled) SubscribeLogger.IfDebug()?.Message($"Subscribe: [{((WorldCharacter)_entityHolder).DebugObjectWatchID}; {_entityHolder.Id} -> {GetType().Name}] -> [{nameof(_entityMortal.DieEvent)}]").Write();

                _entityMortal.BeforeResurrectEvent += (guid, typeId) =>
                {
                    return StageProcessing();
                };
                if (SubscribeLogger.IsDebugEnabled) SubscribeLogger.IfDebug()?.Message($"Subscribe: [{((WorldCharacter)_entityHolder).DebugObjectWatchID}; {_entityHolder.Id} -> {GetType().Name}] -> [{nameof(_entityMortal.BeforeResurrectEvent)}]").Write();

                _entityMortal.ResurrectEvent += (guid, typeId, dummy) =>
                {
                    return ChangeMutation(Faction?.MutationOnDeath ?? 0, null, 0, false);
                };
                if (SubscribeLogger.IsDebugEnabled) SubscribeLogger.IfDebug()?.Message($"Subscribe: [{((WorldCharacter)_entityHolder).DebugObjectWatchID}; {_entityHolder.Id} -> {GetType().Name}] -> [{nameof(_entityMortal.ResurrectEvent)}]").Write();
            }
            return Task.CompletedTask;
        }
    }
}
