using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities.Items;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    //#TODO: Unbound implementation from `WorldCharacter`
    public partial class Consumer : IConsumerImplementRemoteMethods
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        private /*SpellDoerCastPipeline*/ SpellId _spellId;

        private bool _subscribedToWizard;

        // --- API: -----------------------------------------------------------------------------

        public async Task<bool> ConsumeItemInSlotImpl(int slotId, int spellsGroupIndex, bool fromInventory = true)
        {
            try
            {
                if (_spellId.IsValid)
                {
                    Logger.IfWarn()?.Message($"Attempt to consume spell while another consume-spell ({_spellId.Counter} ({_spellId.IsValid}) executed").Write();
                    return false;
                }

                var repo = EntitiesRepository;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(parentEntity.Id))
                {
                    if (wrapper.AssertIfNull(nameof(wrapper)))
                        return false;

                    var worldCharacter = await wrapper.GetOrSubscribe<IWorldCharacterClientFull>(parentEntity.Id);
                    if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                        return false;

                    var items = fromInventory ? worldCharacter.Inventory : (IItemsContainerClientFull) worldCharacter.Doll;
                    var itemResource = items.Items[slotId]?.Item?.ItemResource as ItemResource;
                    if (itemResource.AssertIfNull(nameof(itemResource)))
                        return false;

                    if (itemResource.ConsumableData.Target.AssertIfNull(nameof(itemResource.ConsumableData)) ||
                        !itemResource.ConsumableData.Target.HasSpells)
                    {
                        Logger.IfError()?.Message("ConsumableData is null or empty").Write();
                        return false;
                    }

                    var consumableData = itemResource.ConsumableData.Target;
                    if (spellsGroupIndex < 0 || spellsGroupIndex >= consumableData.SpellsGroups.Length)
                    {
                        Logger.IfError()?.Message($"spellsGroupIndex={spellsGroupIndex} is out of order").Write();
                        return false;
                    }

                    var targetSpellsGroup = consumableData.SpellsGroups[spellsGroupIndex];
                    if (targetSpellsGroup.Spells == null || targetSpellsGroup.Spells.Length == 0)
                    {
                        Logger.IfError()?.Message($"targetSpellsGroup[{spellsGroupIndex}] hasn't spells").Write();
                        return false;
                    }

                    PropertyAddress itemsAddress = fromInventory
                        ? EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory)
                        : EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);

                    var wizardRef = worldCharacter.Wizard;
                    using (var wizardWrapper = await repo.Get(wizardRef.TypeId, wizardRef.Id))
                    {
                        var wizard = wizardWrapper.Get<IWizardEntityServer>(wizardRef.TypeId, wizardRef.Id, ReplicationLevel.Server);
                        if (wizard == null)
                        {
                            Logger.IfError()?.Message($"Can't get `{nameof(IWizardEntityServer)}` by {wizardRef.TypeId}, {wizardRef.Id}.").Write();
                            return false;
                        }

                        SubscribeWizardIfDidNot(wizard);

                        SpellDef consumeSpellDef = null;
                        for (int i = 0, len = targetSpellsGroup.Spells.Length; i < len; i++)
                        {
                            var checkedSpellDef = targetSpellsGroup.Spells[i].Target;
                            if (checkedSpellDef == null)
                                continue;

                            var cast = new SpellCastWithSlotItem(null)
                            {
                                Def = checkedSpellDef,
                                IsInventory = fromInventory,
                                SlotId = slotId,
                                StartAt = SyncTime.Now
                            };

                            var canStartCast = await wizard.CheckSpellCastPredicates(SyncTime.Now, cast, null, null);
                            Logger.IfDebug()?.Message($"'{checkedSpellDef.Name}' canStartCast{canStartCast.AsSign()}").Write(); //DEBUG
                            if (canStartCast)
                            {
                                consumeSpellDef = checkedSpellDef;
                                break;
                            }
                        }

                        if (consumeSpellDef.AssertIfNull(nameof(consumeSpellDef)))
                            return false;

                        Logger.IfDebug()?.Message($"consumeSpellDef={consumeSpellDef.Name}").Write(); //DEBUG

                        bool result = true;
                        if (!targetSpellsGroup.DontRemoveWhenUse)
                        {
                            ISlotItemClientFull item;
                            var containerUsageType = GetContainerUsageType(worldCharacter, fromInventory, slotId, out item);
                            if (containerUsageType.HasValue && containerUsageType.Value == ContainerUsageType.InnerContainer)
                                result = item.Stack > 0 && item.Item.AmmoContainer.Size > 0 && item.Item.AmmoContainer.Items.Count > 0 &&
                                         item.Item.AmmoContainer.Items[0].Stack > 0;
                            else
                                result = item.Stack > 0;
                        }

                        if (result)
                        {
                            SpellId id = await wizard.CastSpell(
                                new SpellCastWithSlotItem(null)
                                {
                                    Def = consumeSpellDef,
                                    IsInventory = fromInventory,
                                    SlotId = slotId
                                });

                            if (id.IsValid)
                            {
                                _spellId = id;
                                //await id.Task; // TODOA если у нас под конец спела не останется предметов, то спелл не выполнется. Необходимо исправить после рефакторинга Витали.

                                if (!targetSpellsGroup.DontRemoveWhenUse)
                                {
                                    ISlotItemClientFull item;
                                    var containerUsageType = GetContainerUsageType(worldCharacter, fromInventory, slotId, out item);
                                    if (containerUsageType.HasValue && containerUsageType.Value == ContainerUsageType.InnerContainer)
                                    {
                                        PropertyAddress innerContainerAddress = EntityPropertyResolver.GetPropertyAddress(item.Item.AmmoContainer);

                                        var itemTransaction = new ItemRemoveBatchManagementTransaction(
                                            new List<RemoveItemBatchElement>()
                                            {
                                                new RemoveItemBatchElement(innerContainerAddress, 0, 1, Guid.Empty)
                                            }, false, repo);
                                        await itemTransaction.ExecuteTransaction();
                                    }
                                    else
                                    {
                                        var itemTransaction = new ItemRemoveBatchManagementTransaction(
                                            new List<RemoveItemBatchElement>()
                                            {
                                                new RemoveItemBatchElement(itemsAddress, slotId, 1, Guid.Empty)
                                            }, false, repo);
                                        await itemTransaction.ExecuteTransaction();
                                    }
                                }
                            }
                            else
                            {
                                Logger.IfError()?.Message("Failed to cast consume spell").Write();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"{nameof(ConsumeItemInSlot)}() Exception: {e}. #TODO: move Consumer to cluster (should be executed on Server).");
                return false;
            }

            return true;
        }


        // --- Privates: ----------------------------------------------------------------------------------------

        private async Task OnOrderFinished(DeltaDictionaryChangedEventArgs<SpellId, ISpellServer> args) //SpellId id)
        {
            var repo = EntitiesRepository;
            using (var wrapper = await repo.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
            {
                if (_spellId == args.Value.Id)
                    _spellId = SpellId.Invalid;
            }
        }

        private void SubscribeWizardIfDidNot(IWizardEntityServer wizard)
        {
            if (_subscribedToWizard)
                return;

            wizard.Spells.OnItemRemoved += OnOrderFinished;
            _subscribedToWizard = true;
        }

        private ContainerUsageType? GetContainerUsageType(IWorldCharacterClientFull worldCharacter, bool isInventory, int itemId,
            out ISlotItemClientFull item)
        {
            if (isInventory)
                worldCharacter.Inventory.Items.TryGetValue(itemId, out item);
            else
                worldCharacter.Doll.Items.TryGetValue(itemId, out item);

            return (item?.Item?.ItemResource as ItemResource)?.WeaponDef.Target?.ContainerUsage;
        }
    }
}