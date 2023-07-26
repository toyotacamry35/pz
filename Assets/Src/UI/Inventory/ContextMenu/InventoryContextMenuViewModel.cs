using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Lib.Cheats;
using Assets.Src.ResourceSystem.L10n;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using L10n;
using Assets.Tools;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Uins.Slots;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Repositories;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using SharedCode.Serializers;

namespace Uins
{
    [Binding]
    public class InventoryContextMenuViewModel : BindingViewModel, IContextActionsSource
    {
        private const string IsDropAllAvailableKey = "IsDropAllAvailable";

        [SerializeField, UsedImplicitly]
        private ContextMenuActionsDefRef _contextMenuActionsDefRef;

        public event ShownContextMenuEvent ShowContextMenu;

        private List<Func<SlotViewModel, bool, IList<ContextMenuItemData>>> _contextActions;

        private readonly List<ContextMenuItemData> _emptyDataList = new List<ContextMenuItemData>();


        //=== Props ===============================================================

        public static InventoryContextMenuViewModel Instance { get; private set; }

        private ContextMenuActionsDef ActionsDef => _contextMenuActionsDefRef.Target;

        private IGuiWindow _inventoryGuiWindow;
        private IGuiWindow _benchMountingGuiWindow;
        private WindowsManager _windowsManager;
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        private bool _isShown;

        [Binding]
        public bool IsShown
        {
            get => _isShown;
            set
            {
                if (_isShown != value)
                {
                    _isShown = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private static bool IsDropAllAvailable
        {
            get => UniquePlayerPrefs.GetBool(IsDropAllAvailableKey);
            set => UniquePlayerPrefs.SetBool(IsDropAllAvailableKey, value);
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            _contextMenuActionsDefRef.Target.AssertIfNull(nameof(_contextMenuActionsDefRef));
            _contextActions = new List<Func<SlotViewModel, bool, IList<ContextMenuItemData>>>()
            {
                GetContextMenuItemData_TakeFromContainer,
                GetContextMenuItemData_EquipUnequip,
                GetContextMenuItemData_Consumable,
                GetContextMenuItemData_Repair,
                GetContextMenuItemData_Break,
                GetContextMenuItemData_Mount,
                GetContextMenuItemData_Split,
                GetContextMenuItemData_Drop,
                GetContextMenuItemData_DropAll,
                // GetContextMenuItemData_Destroy
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        [Cheat]
        public static void toggle_drop_all()
        {
            IsDropAllAvailable = !IsDropAllAvailable;
        }

        public void Init(OurCharacterSlotsViewModel ourCharacterSlotsViewModel, IGuiWindow inventoryGuiWindow,
            IGuiWindow benchMountingGuiWindow, WindowsManager windowsManager)
        {
            _ourCharacterSlotsViewModel = ourCharacterSlotsViewModel;
            _inventoryGuiWindow = inventoryGuiWindow;
            _benchMountingGuiWindow = benchMountingGuiWindow;
            _windowsManager = windowsManager;
        }

        public List<ContextMenuItemData> OnContextButtonsRequest(SlotViewModel slotViewModel)
        {
            if (!slotViewModel.AssertIfNull(nameof(slotViewModel)))
            {
                try
                {
                    return GetContextMenuItemDataList(slotViewModel);
                }
                catch (Exception exception)
                {
                    UI.Logger.IfError()?.Message($"{nameof(OnContextButtonsRequest)}() {exception}").Write();
                }
            }

            return _emptyDataList;
        }

        public void OnContextMenuRequest(SlotViewModel slotViewModel)
        {
            try
            {
                if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                    return;

                var contextMenuItemDataList = GetContextMenuItemDataList(slotViewModel);
                if (contextMenuItemDataList.Count == 0)
                    return;

                ShowContextMenu?.Invoke(contextMenuItemDataList);
                IsShown = true;
            }
            catch (Exception exception)
            {
                UI.Logger.IfError()?.Message($"{nameof(OnContextMenuRequest)}() {exception}").Write();
            }
        }

        public void ExecuteDefaultAction(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            var contextMenuItemData = GetDefaultContextMenuItemData(slotViewModel, isLimitedSet);
            try
            {
                contextMenuItemData?.Action?.Invoke(contextMenuItemData.ActionParams);
            }
            catch (Exception e)
            {
                UI.Logger.Error($"{nameof(ExecuteDefaultAction)}(svm={slotViewModel}, " +
                                $"{nameof(isLimitedSet)}{isLimitedSet.AsSign()}) exception: {e}");
            }
        }

        public void CloseContextMenuRequest()
        {
            if (IsShown)
                IsShown = false;
        }

        public static void DoEquipmentSound(bool isEquip, SlotViewModel dollSlotViewModel, GameObject soundSourceGo)
        {
            if (dollSlotViewModel == null || !dollSlotViewModel.DoEquipSound)
                return;

            if (isEquip)
                SoundControl.Instance.ClothPutOnEvent?.Post(soundSourceGo);
            else
                SoundControl.Instance.ClothTakeOffEvent?.Post(soundSourceGo);
        }


        //=== Private =============================================================

        private ContextMenuItemData GetDefaultContextMenuItemData(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.IsEmpty ||
                slotViewModel.ItemIsNullOrDefault)
                return null;

            foreach (var contextAction in _contextActions)
            {
                IList<ContextMenuItemData> contextMenuItemDataList = null;
                try
                {
                    contextMenuItemDataList = contextAction.Invoke(slotViewModel, isLimitedSet);
                }
                catch (Exception e)
                {
                    UI.Logger.IfError()?.Message($"Attempt to get default is failed: {e}").Write();
                }

                if (contextMenuItemDataList != null && contextMenuItemDataList.Count > 0)
                    return contextMenuItemDataList.First();
            }

            UI.Logger.Error(
                $"Unable to get default context action for {slotViewModel} {nameof(isLimitedSet)}{isLimitedSet.AsSign()}");
            return null;
        }

        private List<ContextMenuItemData> GetContextMenuItemDataList(SlotViewModel slotViewModel)
        {
            var dataList = new List<ContextMenuItemData>();

            foreach (var contextAction in _contextActions)
            {
                IList<ContextMenuItemData> contextMenuItemDataList = null;
                try
                {
                    contextMenuItemDataList = contextAction.Invoke(slotViewModel, false);
                }
                catch (Exception e)
                {
                    UI.Logger.IfError()?.Message($"{nameof(GetContextMenuItemDataList)}() Exception for {contextAction.Method.Name}: {e}").Write();
                    return null;
                }

                if (contextMenuItemDataList == null || contextMenuItemDataList.Count == 0)
                    continue;

                if (dataList.Count == 0)
                    contextMenuItemDataList.First().IsActive = true; //отмечаем действие по ум.

                dataList.AddRange(contextMenuItemDataList);
            }

            return dataList;
        }

        
        private IList<ContextMenuItemData> GetContextMenuItemData_TakeFromContainer(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType != SlotViewModel.CollectionType.Output)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Take,
                    Action = (parameters) => TakeFromOutput((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private void TakeFromOutput(SlotViewModel fromSlotVM)
        {
            if (fromSlotVM.IsEmpty)
            {
                UI.Logger.IfError()?.Message($"{nameof(TakeFromOutput)}({fromSlotVM}) Empty slotViewModel").Write();
                return;
            }
            var suitableSlots = _ourCharacterSlotsViewModel.GetInventorySuitableSlots(fromSlotVM);
            if (suitableSlots.Count == 0)
                return;
           
            var originalStackCount = fromSlotVM.Stack;
            var restStackCount = fromSlotVM.Stack;
            var suitableSlotsIndex = 0;
            do
            {
                var toSvm = suitableSlots[suitableSlotsIndex++];
                if (toSvm.HasUnfilledStack(fromSlotVM.ItemResource))
                {
                    var movedStack = Mathf.Min(restStackCount, toSvm.UnfilledStack(fromSlotVM.ItemResource));
                    ClusterCommands.MoveItem_OnClient(
                        fromSlotVM.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                        fromSlotVM,
                        toSvm,
                        movedStack);
                    restStackCount -= movedStack;
                }

            } while (restStackCount > 0 && suitableSlotsIndex < suitableSlots.Count);

            if (restStackCount != originalStackCount)
                DoEquipmentSound(false, fromSlotVM, gameObject);
        }
        
        private IList<ContextMenuItemData> GetContextMenuItemData_EquipUnequip(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || isLimitedSet || !slotViewModel.CanBeEquippedOrUnequipped)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = slotViewModel.IsInventorySlot ? ActionsDef.Equip : ActionsDef.Unequip,
                    Action = (parameters) => EquipOrUnequip((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private void EquipOrUnequip(SlotViewModel fromSlvm)
        {
            if (fromSlvm.IsInventorySlot)
            {
                //Equip
                //0) Получаем коллекцию принимающих айтем слотов
                //1) Если айтем стекающийся, ищем ячейку, которую можно дополнить 
                //2) Если такой ячейки нет, ищем первую пустую ячейку и заполняем ее из slotViewModel
                //3) Если такой ячейки нет, берем первую принимающую ячейку и заполняем из slotViewModel

                SlotViewModel equipedSlotViewModel = null;
                var suitableSlots = _ourCharacterSlotsViewModel.GetDollSuitableSlots(fromSlvm);
                if (suitableSlots.Count == 0)
                    return;

                if (fromSlvm.ItemResource.MaxStack > 1)
                {
                    //1
                    var sameItemUnfilledSvm = suitableSlots.FirstOrDefault(svm =>
                        !svm.IsEmpty &&
                        svm.HasUnfilledStack(fromSlvm.ItemResource));
                    if (sameItemUnfilledSvm != null)
                    {
                        //докладываем недостающее
                        ClusterCommands.MoveItem_OnClient(
                            fromSlvm.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                            fromSlvm,
                            sameItemUnfilledSvm,
                            Mathf.Min(sameItemUnfilledSvm.UnfilledStack(fromSlvm.ItemResource), fromSlvm.Stack));
                        DoEquipmentSound(true, sameItemUnfilledSvm, gameObject);
                        return;
                    }
                }

                //2
                var emptySvm = suitableSlots.FirstOrDefault(svm =>
                    svm.IsEmpty &&
                    !((svm as ICanBeInaccessible)?.IsInaccessible ?? false));
                if (emptySvm != null)
                {
                    //максимально заполняем пустой
                    equipedSlotViewModel = emptySvm;
                    ClusterCommands.MoveItem_OnClient(
                        fromSlvm.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                        fromSlvm,
                        emptySvm,
                        Mathf.Min(new[]
                            {fromSlvm.ItemResource.MaxStack, emptySvm.SlotDef.MaxStack, fromSlvm.Stack}));
                }
                else
                {
                    //3
                    var firstSvm = suitableSlots.First();
                    if (firstSvm.ItemResource == fromSlvm.ItemResource) //вообще-то можно продолжать искать не такой же
                        return;

                    equipedSlotViewModel = firstSvm;
                    ClusterCommands.MoveItem_OnClient(
                        fromSlvm.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                        fromSlvm,
                        firstSvm,
                        Mathf.Min(new[]
                            {fromSlvm.ItemResource.MaxStack, firstSvm.SlotDef.MaxStack, fromSlvm.Stack}));
                }

                DoEquipmentSound(true, equipedSlotViewModel, gameObject);
            }
            else
            {
                //unequip
                if (!TryUnequip(fromSlvm))
                {
                    UI.Logger.IfWarn()?.Message($"{nameof(EquipOrUnequip)}({fromSlvm}) Unequip: Unable to unequip").Write();
                }
            }
        }

        private bool TryUnequip(SlotViewModel fromSvm)
        {
            if (fromSvm.IsInventorySlot || fromSvm.IsEmpty)
            {
                UI.Logger.IfError()?.Message($"{nameof(TryUnequip)}({fromSvm}) Wrong slotViewModel").Write();
                return false;
            }

            var suitableSlots = _ourCharacterSlotsViewModel.GetInventorySuitableSlots(fromSvm);
            //UI.CallerLog($"suitableSlots: {suitableSlots.ItemsToStringByLines()}");
            if (suitableSlots.Count == 0)
                return false;

            var orgRestOfStack = fromSvm.Stack;
            int restOfStack = fromSvm.Stack;
            int suitableSlotsIndex = 0; //идем по suitableSlots и докладываем, пока не закончятся restOfStack или suitableSlots
            do
            {
                var toSvm = suitableSlots[suitableSlotsIndex++];
                if (toSvm.HasUnfilledStack(fromSvm.ItemResource))
                {
                    var movedStack = Mathf.Min(restOfStack, toSvm.UnfilledStack(fromSvm.ItemResource));
                    ClusterCommands.MoveItem_OnClient(
                        fromSvm.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                        fromSvm,
                        toSvm,
                        movedStack);
                    restOfStack -= movedStack;
                }

            } while (restOfStack > 0 && suitableSlotsIndex < suitableSlots.Count);

            if (restOfStack < orgRestOfStack)
                DoEquipmentSound(false, fromSvm, gameObject);

            return restOfStack == 0;
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Consumable(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            var consumableData = (slotViewModel.ItemResource as ItemResource)?.ConsumableData.Target;
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || consumableData == null || !consumableData.HasSpells)
                return null;

            //TODOM отправка запроса возможности исполнения
            List<ContextMenuItemData> list = new List<ContextMenuItemData>();
            for (int i = 0, len = consumableData.SpellsGroups.Length; i < len; i++)
            {
                var spellsGroup = consumableData.SpellsGroups[i];
                int j = i;
                list.Add(new ContextMenuItemData()
                {
                    Title = !spellsGroup.ActionTitleLs.IsEmpty() ? spellsGroup.ActionTitleLs : ActionsDef.Consume,
                    IsDisabled = false, // slotViewModel.Item.GetConsumableInteractionSpell(pawnConsumer.gameObject) == null,
                    Action = (parameters) =>
                    {
                        //повторяем все проверки на момент выполнения
                        var executionTimeSvm = (SlotViewModel) parameters[0];
                        if (executionTimeSvm == null ||
                            executionTimeSvm.IsEmpty)
                            return;

                        var repo = ClusterCommands.ClientRepository;
                        var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                        var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
                        AsyncUtils.RunAsyncTask(async () =>
                        {
                            using (var wrapper = await repo.Get(typeId, characterId))
                            {
                                var worldCharacter = wrapper.Get<IHasConsumerClientFull>(typeId, characterId, ReplicationLevel.ClientFull);
                                await worldCharacter.Consumer.ConsumeItemInSlot(executionTimeSvm.SlotId, j, executionTimeSvm.IsInventorySlot);
                            }
                        });
                    },
                    ActionParams = new object[] {slotViewModel}
                });
            }

            return list;
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Repair(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || isLimitedSet)
                return null;

            var itemResource = slotViewModel.ItemResource;
            var slotItem = slotViewModel.SelfSlotItem;
            if (itemResource.AssertIfNull(nameof(itemResource)) ||
                (itemResource as ItemResource)?.Durability.Target?.RepairRecipe.Target == null)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Repair,
                    Action = (parameters) => ClusterCommands.Repair_OnClient((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Break(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || isLimitedSet)
                return null;

            var itemResource = slotViewModel.ItemResource;
            if (itemResource.AssertIfNull(nameof(itemResource)) ||
                (itemResource as ItemResource)?.Durability.Target == null ||
                (itemResource as ItemResource)?.Durability.Target.ItemsOnBreak.Length == 0)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Break,
                    Action = (parameters) => ClusterCommands.Break_OnClient((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Mount(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            var entityObjectDef = (slotViewModel.ItemResource as ItemResource)?.MountingData?.EntityObjectDef.Target;
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || entityObjectDef == null)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = (slotViewModel.ItemResource as ItemResource).MountingData.ActionTitleLs.IsEmpty()
                        ? ActionsDef.Build
                        : (slotViewModel.ItemResource as ItemResource).MountingData.ActionTitleLs,
                    Action = (parameters) => Build_Command((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Split(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || isLimitedSet || slotViewModel.Stack < 2)
                return null;

            var inventoryFirstEmptySlot = _ourCharacterSlotsViewModel.GetInventoryFirstEmptySlot();

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Split,
                    IsDisabled = inventoryFirstEmptySlot == null,
                    Action = (parameters) =>
                    {
                        var fromSvm = (SlotViewModel) parameters[0];
                        ClusterCommands.MoveItem_OnClient(
                            fromSvm.SlotsCollectionApi.CollectionPropertyAddress.EntityId,
                            fromSvm,
                            (SlotViewModel) parameters[1],
                            (int) parameters[2]);
                    },
                    ActionParams = new object[]
                    {
                        slotViewModel,
                        inventoryFirstEmptySlot,
                        slotViewModel.Stack / 2 //TODOM
                    }
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Drop(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || slotViewModel.ItemResource is DollItemResource)
                return null;

            if (isLimitedSet)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Drop,
                    Action = (parameters) =>
                    {
                        SoundControl.Instance.ItemDrop.Post(gameObject);
                        ClusterCommands.DropItemAsBox_OnClient((SlotViewModel) parameters[0]);
                    },
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_DropAll(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (slotViewModel.SlotCollectionType == SlotViewModel.CollectionType.Output || slotViewModel.ItemResource is DollItemResource)
                return null;

            if (!IsDropAllAvailable || isLimitedSet)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.DropAll,
                    Action = (parameters) => DropAllContainerItems((SlotViewModel) parameters[0]),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }

        private IList<ContextMenuItemData> GetContextMenuItemData_Destroy(SlotViewModel slotViewModel, bool isLimitedSet)
        {
            if (isLimitedSet)
                return null;

            return new[]
            {
                new ContextMenuItemData()
                {
                    Title = ActionsDef.Destroy,
                    Action = (parameters) => Destroy_Command((SlotViewModel) parameters[0]).WrapErrors(),
                    ActionParams = new object[] {slotViewModel}
                }
            };
        }


        //--- Private. Commands ---------------------------------------------------

        private void Build_Command(SlotViewModel slotViewModel)
        {
            if (slotViewModel == null ||
                slotViewModel.IsEmpty ||
                slotViewModel.ItemIsNullOrDefault)
                return;

            if (_inventoryGuiWindow.State.Value == GuiWindowState.Opened)
                _windowsManager.Close(_inventoryGuiWindow);
            _windowsManager.Open(_benchMountingGuiWindow, null, slotViewModel);
        }

        private async Task Destroy_Command(SlotViewModel target)
        {
            var repository = ClusterCommands.Repository;
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await repository.Get<IWorldCharacterClientFull>(characterId))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterId);
                var itemSlot = target.IsInventorySlot
                    ? worldCharacter.Inventory.Items[target.SlotId]
                    : worldCharacter.Doll.Items[target.SlotId];
                var kindAddress = EntityPropertyResolver.GetPropertyAddress(target.IsInventorySlot ? worldCharacter.Inventory as IDeltaObject : worldCharacter.Doll as IDeltaObject);

                var result = await worldCharacter.RemoveItem(kindAddress, target.SlotId, itemSlot.Stack, itemSlot.Item.Id);
            }
        }

        private void DropAllContainerItems(SlotViewModel sampleSlotViewModel)
        {
            this.StartInstrumentedCoroutine(WaitableDropAllItems(sampleSlotViewModel));
        }

        private WaitForSeconds _waitForNextDrop = new WaitForSeconds(0.35f);

        IEnumerator WaitableDropAllItems(SlotViewModel sampleSlotViewModel)
        {
            IEnumerable<SlotViewModel> nonEmptySlotViewModels = sampleSlotViewModel.IsInventorySlot
                ? _ourCharacterSlotsViewModel.InventoryViewModels.Where(csvm => !csvm.IsEmpty).Select(csvm => (SlotViewModel) csvm)
                : _ourCharacterSlotsViewModel.DollViewModels.Where(svm => !svm.IsEmpty).Select(csvm => (SlotViewModel) csvm);

            foreach (var slotViewModel in nonEmptySlotViewModels)
            {
                ClusterCommands.DropItemAsBox_OnClient(slotViewModel);
                yield return _waitForNextDrop;
            }
        }
    }
}