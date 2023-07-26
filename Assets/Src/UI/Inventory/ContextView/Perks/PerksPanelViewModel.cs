using System;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Arithmetic;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem;
using Assets.Src.Aspects;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using Uins.Slots;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;
using SharedCode.Serializers;

namespace Uins
{
    [Binding]
    public class PerksPanelViewModel : BindingViewModel, ISlotAcceptanceResolver, ISortByItemTypeResolver, IPerkActionsPricesCalculator,
        IFactionStagePerksResolver
    {
        public event Action<SlotItem> TemporaryPerkAdded;

        [SerializeField, UsedImplicitly]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private TemporaryPerkSlotViewModel _temporaryPerkPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _temporaryPerksTransform;

        [SerializeField, UsedImplicitly]
        private PermanentPerkSlotViewModel _permanentPerkPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _permanentPerksTransform;

        [SerializeField, UsedImplicitly]
        private SavedPerkSlotViewModel _savedPerkPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _savedPerksTransform;

        [SerializeField, UsedImplicitly]
        private PerksContextView _perksContextView;

        [SerializeField, UsedImplicitly]
        private DraggingHandler _draggingHandler;

        [SerializeField, UsedImplicitly]
        private PerkSlotTypes _perkSlotTypes;

        [SerializeField, UsedImplicitly]
        private PerkActionsPricesDefRef _perkActionsPricesDefRef;

        [SerializeField, UsedImplicitly]
        private PerksCollectionSortViewModel _temporaryCollectionSortViewModel;

        [SerializeField, UsedImplicitly]
        private PerksCollectionSortViewModel _permanentCollectionSortViewModel;

        [SerializeField, UsedImplicitly]
        private PerksCollectionSortViewModel _savedCollectionSortViewModel;

        [SerializeField, UsedImplicitly]
        private WindowId _confirmationWindowId;

        [SerializeField, UsedImplicitly]
        private PerkSlotsLimitsRef _perkSlotsLimitsRef;

        [SerializeField, UsedImplicitly]
        private FactionsDefRef _factionsDefRef;

        [SerializeField, UsedImplicitly]
        private PerksTitlesDefRef _perksTitlesDefRef;

        private BaseItemResource _lastAddedFactionPerkItemResource;
        private WindowsManager _windowsManager;
        private IGuiWindow _confirmationWindow;
        private IPawnSource _pawnSource;
        private PerkActionsPricesDef _perkActionsPrices;
        private PerkSlotsLimits _perkSlotsLimits;

        private BaseItemResource[] _factionPerks;


        //=== Props ===========================================================

        public ReactiveProperty<int> PerkActionsMultiplierRp { get; } = new ReactiveProperty<int>();

        private PerksTitlesDef TitlesDef => _perksTitlesDefRef.Target;

        private TemporaryPerksCollection _temporaryPerksCollection = new TemporaryPerksCollection();
        private PermanentPerksCollection _permanentPerksCollection = new PermanentPerksCollection();
        private SavedPerksCollection _savedPerksCollection = new SavedPerksCollection();

        private ReactiveProperty<int> _maxSlots1CountRp = new ReactiveProperty<int>();

        [Binding]
        public int MaxSlots1Count { get; set; }

        private ReactiveProperty<int> _maxSlots2CountRp = new ReactiveProperty<int>();

        [Binding]
        public int MaxSlots2Count { get; set; }

        private ReactiveProperty<int> _maxSlots3CountRp = new ReactiveProperty<int>();

        [Binding]
        public int MaxSlots3Count { get; set; }


        //=== Unity ===============================================================

        private void Awake()
        {
            _temporaryPerkPrefab.AssertIfNull(nameof(_temporaryPerkPrefab));
            _permanentPerkPrefab.AssertIfNull(nameof(_permanentPerkPrefab));
            _savedPerkPrefab.AssertIfNull(nameof(_savedPerkPrefab));

            _temporaryPerksTransform.AssertIfNull(nameof(_temporaryPerksTransform));
            _permanentPerksTransform.AssertIfNull(nameof(_permanentPerksTransform));
            _savedPerksTransform.AssertIfNull(nameof(_savedPerksTransform));

            _perksContextView.AssertIfNull(nameof(_perksContextView));
            _draggingHandler.AssertIfNull(nameof(_draggingHandler));
            _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel));
            _perksTitlesDefRef.Target.AssertIfNull(nameof(_perksTitlesDefRef));

            if (_perkActionsPricesDefRef.AssertIfNull(nameof(_perkActionsPricesDefRef)))
                return;

            _perkActionsPrices = _perkActionsPricesDefRef.Target;
            _perkActionsPrices.AssertIfNull(nameof(_perkActionsPrices));

            if (_perkSlotTypes.AssertIfNull(nameof(_perkSlotTypes)))
                return;

            _perkSlotTypes.Init();
            if (!_perkSlotTypes.IsInited)
                UI.Logger.IfError()?.Message($"{nameof(_perkSlotTypes)} isn't inited").Write();

            if (_perkSlotsLimitsRef.AssertIfNull(nameof(_perkSlotsLimitsRef)))
                return;

            _perkSlotsLimits = _perkSlotsLimitsRef.Target;
            if (_perkSlotsLimits.AssertIfNull(nameof(_perkSlotsLimits)))
                return;
            if (_perkSlotTypes.ItemTypes.Length > _perkSlotsLimits.Limits.Length)
            {
                UI.Logger.Error(
                    $"{nameof(_perkSlotsLimits)}.{nameof(_perkSlotsLimits.Limits)} length ({_perkSlotsLimits.Limits.Length}) " +
                    $"is less than {nameof(_perkSlotTypes)}.{nameof(_perkSlotTypes.ItemTypes)} length ({_perkSlotTypes.ItemTypes.Length})");
                return;
            }

            Bind(_maxSlots1CountRp, () => MaxSlots1Count);
            Bind(_maxSlots2CountRp, () => MaxSlots2Count);
            Bind(_maxSlots3CountRp, () => MaxSlots3Count);
            _maxSlots1CountRp.Value = _perkSlotsLimits.Limits[0]; //Свойства макс. допустимого числа типов слотов в перманентной коллекции
            _maxSlots2CountRp.Value = _perkSlotsLimits.Limits[1];
            _maxSlots3CountRp.Value = _perkSlotsLimits.Limits[2];

            _factionPerks = GetFactionPerks(_factionsDefRef);
            if (_factionPerks.AssertIfNull(nameof(_factionPerks)))
                return;

            _temporaryCollectionSortViewModel.AssertIfNull(nameof(_temporaryCollectionSortViewModel));
            _permanentCollectionSortViewModel.AssertIfNull(nameof(_permanentCollectionSortViewModel));
            _savedCollectionSortViewModel.AssertIfNull(nameof(_savedCollectionSortViewModel));
            _confirmationWindowId.AssertIfNull(nameof(_confirmationWindowId));
        }


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource, WindowsManager windowsManager)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)))
                return;

            _pawnSource = pawnSource;
            _temporaryPerksCollection.Init(
                this,
                pawnSource,
                _temporaryPerkPrefab,
                _temporaryPerksTransform,
                _perksContextView,
                _draggingHandler,
                transform,
                this,
                _temporaryCollectionSortViewModel,
                _perkSlotTypes,
                SoundControl.Instance.PerkObtain,
                SoundControl.Instance.PerkSlotHover);
            _temporaryPerksCollection.PerkAdded += OnTemporaryPerkAdded;
            _temporaryPerksCollection.SetFactionStagePerksResolver(this);

            _permanentPerksCollection.Init(
                this,
                pawnSource,
                _permanentPerkPrefab,
                _permanentPerksTransform,
                _perksContextView,
                _draggingHandler,
                transform,
                this,
                _permanentCollectionSortViewModel,
                _perkSlotTypes,
                SoundControl.Instance.PerkToPermanent,
                SoundControl.Instance.PerkSlotHover);
            _permanentPerksCollection.SetFactionStagePerksResolver(this);

            _savedPerksCollection.Init(
                this,
                pawnSource,
                _savedPerkPrefab,
                _savedPerksTransform,
                _perksContextView,
                _draggingHandler,
                transform,
                this,
                _savedCollectionSortViewModel,
                _perkSlotTypes,
                SoundControl.Instance.PerkToSaved,
                SoundControl.Instance.PerkSlotHover);

            _windowsManager = windowsManager;
            _confirmationWindow = windowsManager.GetWindow(_confirmationWindowId);
            _confirmationWindow.AssertIfNull(nameof(_confirmationWindow));
            PerkActionsMultiplierRp.Value = 1;

            _perksContextView.Init(windowsManager, this, _permanentPerksCollection, _perkSlotsLimits);
            /*var expirationStream = pawnSource.TouchableEntityProxy.Child(D, character => character.PremiumStatus).ToStream(D, prem => prem.Expiration);
            expirationStream.Action(D, dt => RecalcPricesMultiplier());*/
        }

        public bool GetIsFactionStagePerk(BaseItemResource perkItemResource)
        {
            if (perkItemResource == null || _factionPerks == null)
                return false;

            return _factionPerks.Contains(perkItemResource);
        }

        public int TryMoveTo(SlotViewModel fromSvm, SlotViewModel toSvm, bool doMove = false, bool isCounterSwapCheck = false)
        {
            if (doMove)
                UI.CallerLog($"{nameof(fromSvm)}={fromSvm}, {nameof(toSvm)}={toSvm}");
            var fromPerkSvm = fromSvm as PerkBaseSlotViewModel;
            var toPerkSvm = toSvm as PerkBaseSlotViewModel;
            var toPerkSvmIsSaved = toPerkSvm is SavedPerkSlotViewModel;
            if (fromPerkSvm == null || toPerkSvm == null || fromPerkSvm.IsEmpty)
                return 0;

            if (toPerkSvm is TemporaryPerkSlotViewModel ||
                (toPerkSvmIsSaved && (fromPerkSvm.GetType() == toPerkSvm.GetType() || !toPerkSvm.IsEmpty)))
                return 0;

            var fromPerkItemType = fromPerkSvm.ItemResource?.ItemType.Target;
            var toPerkSlotItemGroup = toPerkSvmIsSaved ? _perkSlotTypes.BestType : toPerkSvm.PerkSlotType;
            if (fromPerkItemType == null || toPerkSlotItemGroup == null)
                return 0;

            var acceptedStack = toPerkSvm is SavedPerkSlotViewModel || toPerkSlotItemGroup.IsAParentOf(fromPerkItemType) ? 1 : 0;

            //проверка возможности переместить содержимое toSvm во fromSvm
            if (acceptedStack > 0 &&
                !toSvm.IsEmpty &&
                !isCounterSwapCheck && //против зацикливания
                fromSvm.SlotAcceptanceResolver.TryMoveTo(toSvm, fromSvm, false, true) <= 0)
                acceptedStack = 0;

            if (acceptedStack > 0 && doMove && !isCounterSwapCheck)
            {
                if (toPerkSvm is SavedPerkSlotViewModel)
                    ShowSavingPerkDialog(fromPerkSvm, toPerkSvm);
                else
                    Assets.Src.Aspects.ClusterPerksCommands.MovePerk(_ourCharacterSlotsViewModel.PlayerGuid, fromPerkSvm, toPerkSvm);
            }

            return acceptedStack;
        }

        public bool TryToDropFrom(SlotViewModel fromSvm)
        {
            return false;
        }

        //Первые 9 разрядов (0...511) под baseOrder
        private const int BaseOrderDigitCount = 9;

        //далее 3 разряда под тип перка
        private const int ItemTypeOrderDigitCount = BaseOrderDigitCount + 3;

        //далее 3 разряда под тип слота перка
        private const int SlotTypeOrderDigitCount = ItemTypeOrderDigitCount + 3;

        /// <summary>
        /// "Сортировка цепочкой": ABCD, BCDA, CDAB
        /// </summary>
        /// <param name="sortingOrder">Индекс типа, который должен быть первым</param>
        /// <param name="itemTypes">Для перков: тип перка (null если нет), тип слота</param>
        public int GetAdditionalSortingIndex(bool hasSortingPriority, int sortingOrder, ItemTypeResource[] itemTypes)
        {
            if (itemTypes == null || itemTypes.Length != 2)
            {
                UI.Logger.IfError()?.Message($"{nameof(GetAdditionalSortingIndex)}() null or wrong length of {nameof(itemTypes)}").Write();
                return 0;
            }

            int additionalIndex = GetSortingIndexPart(sortingOrder, itemTypes[0], BaseOrderDigitCount);
            additionalIndex += GetSortingIndexPart(sortingOrder, itemTypes[1], ItemTypeOrderDigitCount);
            if (hasSortingPriority)
                additionalIndex += 1 << SlotTypeOrderDigitCount;
            return -additionalIndex;
        }

        public PriceDef GetPerkSavingCosts(BaseItemResource perk)
        {
            if (perk.AssertIfNull(nameof(perk)))
                return default;

            var perkSavingCustomCosts = _perkActionsPrices.PerkSavingCustomCosts.FirstOrDefault(perkActionPrice => perkActionPrice.Item == perk);
            if (!perkSavingCustomCosts.Equals(default(PerkActionPriceDef)))
                return perkSavingCustomCosts.Price;

            return _perkActionsPrices.PerkSavingDefaultCosts
                .FirstOrDefault(perkTypeActionPrice => perkTypeActionPrice.PerkType == perk.ItemType)
                .Price;
        }

        public PriceDef GetPerkDisassemblyBenefits(BaseItemResource perk)
        {
            if (perk.AssertIfNull(nameof(perk)))
                return default;

            var perkDisassemblyCustomBenefits = _perkActionsPrices.PerkDisassemblyCustomBenefits
                .FirstOrDefault(perkActionPrice => perkActionPrice.Item == perk);

            var priceDef = !perkDisassemblyCustomBenefits.Equals(default(PerkActionPriceDef))
                ? perkDisassemblyCustomBenefits.Price
                : _perkActionsPrices.PerkDisassemblyDefaultBenefits
                    .FirstOrDefault(perkTypeActionPrice => perkTypeActionPrice.PerkType == perk.ItemType)
                    .Price;

            var multiplier = PerkActionsMultiplierRp.Value;
            return multiplier == 1 ? priceDef : priceDef.GetPriceWithMultiplier(multiplier);
        }

        public PriceDef GetToPerkSlotUpgradingCosts(ItemTypeResource toPerkSlotType)
        {
            if (toPerkSlotType == null)
                return default;

            return _perkActionsPrices.ToPerkSlotUpgradingCosts
                .FirstOrDefault(perkTypeActionPrice => perkTypeActionPrice.PerkType == toPerkSlotType)
                .Price;
        }


        //=== Private =========================================================

        private PerkSlotsLimits GetPerkSlotsLimits(DefaultCharacterDef defaultCharacterDef)
        {
            return new PerkSlotsLimits()
            {
                Limits = new[]
                {
                    defaultCharacterDef?.DefaultTemporaryPerks?.Size ?? 0,
                    defaultCharacterDef?.DefaultSavedPerks?.Size ?? 0,
                    defaultCharacterDef?.DefaultPermanentPerks?.Size ?? 0
                }
            };
        }

        private void RecalcPricesMultiplier()
        {
            var ego = _pawnSource.OurUserPawn.GetComponent<EntityGameObject>();
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    PerkActionsMultiplierRp.Value = Mathf.RoundToInt(
                        (await _perkActionsPrices.PerkDisassemblyDefaultBenefits[0]
                            .AmountOfResourcesMultiplier.Target
                            .CalcAsync(ego.OuterRefEntity, ego.ClientRepo)).Float);
                });
        }

        private BaseItemResource[] GetFactionPerks(FactionsDefRef factionsDefRef)
        {
            var factionsDef = factionsDefRef?.Target;
            if (factionsDef.AssertIfNull(nameof(factionsDef)) || factionsDef.Factions.AssertIfNull(nameof(factionsDef.Factions)))
                return null;

            return factionsDef.Factions
                .Where(f => f.Target != null && f.Target.Stages != null)
                .SelectMany(f => f.Target.Stages)
                .Where(stg => stg.Target != null && stg.Target.StagePerks != null)
                .SelectMany(stg => stg.Target.StagePerks)
                .Where(l => l.Target != null && l.Target.Items != null)
                .SelectMany(l => l.Target.Items)
                .Where(i => i.Target != null)
                .Select(i => i.Target)
                .Distinct()
                .ToArray();
        }

        private int GetSortingIndexPart(int sortingOrder, ItemTypeResource itemType, int digitsShift)
        {
            var itemTypeIndex = _perkSlotTypes.GetTypeIndex(itemType);
            if (itemTypeIndex < 0)
                return 0;

            return GetMultiplier(sortingOrder, itemTypeIndex, _perkSlotTypes.ItemTypes.Length) << digitsShift;
        }

        /// <summary>
        /// Кольцо возрастающих [1...elemsCount] по index, минимальное - по индексу minValueIndex
        /// </summary>
        private int GetMultiplier(int minValueIndex, int index, int elemsCount)
        {
            return elemsCount - ((index - minValueIndex + elemsCount) % elemsCount) + 1;
        }

        private void OnTemporaryPerkAdded(int slotIndex, SlotItem slotItem)
        {
            var perkItemResource = slotItem.ItemResource;
            if (GetIsFactionStagePerk(perkItemResource))
            {
                if (perkItemResource == _lastAddedFactionPerkItemResource)
                    return;

                _lastAddedFactionPerkItemResource = perkItemResource;
            }

            TemporaryPerkAdded?.Invoke(slotItem);
        }

        private void ShowSavingPerkDialog(PerkBaseSlotViewModel fromPerkSvm, PerkBaseSlotViewModel toPerkSvm)
        {
            void SavePerk()
            {
                ClusterPerksCommands.SavePerk(_ourCharacterSlotsViewModel.PlayerGuid, fromPerkSvm, toPerkSvm);
                SoundControl.Instance.PerkUpgradeSlot.Post(gameObject);
            }

            if (_windowsManager.AssertIfNull(nameof(_windowsManager)))
                return;

            var costs = GetPerkSavingCosts(fromPerkSvm.ItemResource);

            if (costs.HasCosts())
            {
                var settings = new CostConfirmationSettings()
                {
                    Title = TitlesDef.SavePerkTitle,
                    Question = TitlesDef.SavePerkQuestion,
                    CostsOrBenefits = costs,
                    IsCostNorBenefit = true,
                    OnOkAction = SavePerk
                };

                _windowsManager.Open(_confirmationWindow, _confirmationWindowId.PrefferedStackId, settings);
            }
            else
            {
                SavePerk();
            }
        }
    }
}