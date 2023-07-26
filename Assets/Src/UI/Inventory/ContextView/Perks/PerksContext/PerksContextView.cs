using System.Collections.Generic;
using Assets.Src.Aspects;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using Uins.Inventory;
using Uins.Slots;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;
using System.Linq;
using System;
using Assets.Src.ResourceSystem.L10n;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n;
using ReactivePropsNs;

namespace Uins
{
    [Binding]
    public class PerksContextView : BindingViewModel, IContextView
    {
        [SerializeField, UsedImplicitly]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private TechPointViewModel _techPointViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _techPointsTransform;

        [SerializeField, UsedImplicitly]
        private WindowId _confirmationWindowId;

        [SerializeField, UsedImplicitly]
        private PerkSlotTypes _perkSlotTypes;

        [SerializeField, UsedImplicitly]
        private PlayerPointsSource _playerPointsSource;

        [SerializeField, UsedImplicitly]
        private PerksTitlesDefRef _perksTitlesDefRef;

        private List<TechPointViewModel> _techPointViewModels = new List<TechPointViewModel>();

        private WindowsManager _windowsManager;
        private IGuiWindow _confirmationWindow;

        private IPerkActionsPricesCalculator _perkActionsPricesCalculator;
        private PermanentPerksCollection _permanentPerksCollection;

        private PerkSlotsLimits _perkSlotsLimits;


        //=== Props ===========================================================

        private PerksTitlesDef TitlesDef => _perksTitlesDefRef.Target;

        private ICharacterPoints CharacterPoints => _playerPointsSource;

        public ReactiveProperty<PerkBaseSlotViewModel> SelectedPerkSlotRp { get; } = new ReactiveProperty<PerkBaseSlotViewModel>();

        public PerkBaseSlotViewModel SelectedPerk => SelectedPerkSlotRp.HasValue ? SelectedPerkSlotRp.Value : null;

        public IStream<IContextViewTarget> CurrentContext => SelectedPerkSlotRp;

        public IContextViewTarget ContextValue => SelectedPerk;

        public PriceDef UnlockOrUpgradeCosts { get; set; }

        [Binding]
        public bool HasTarget { get; set; }

        [Binding]
        public bool HasPerk { get; set; }

        [Binding]
        public bool HasDestroyablePerk { get; set; }

        [Binding]
        public bool IsExtendedDescriptionArea { get; set; }

        public PriceDef PerkDestroyBenefits { get; set; }

        [Binding]
        public bool HasSlot { get; set; }

        /// <summary>
        /// Можно ли апгрейдить слот. Без учета доп. условий: м.б. нехватка по деньгам, ограничения по макс. числу слотов
        /// </summary>
        [Binding]
        public bool IsSlotUpgradable { get; set; }

        /// <summary>
        /// Можно ли апгрейдить слот с учетом всех условий
        /// </summary>
        [Binding]
        public bool IsUpgradeEnabled { get; set; }

        /// <summary>
        /// Можно ли разлочить слот (сделать зеленым). БЕЗ учета доп. условий, таких как нехватка по деньгам, ограничения по макс. числу слотов
        /// </summary>
        [Binding]
        public bool IsSlotUnlockable { get; set; }

        /// <summary>
        /// Можно ли разлочить слот с учетом всех условий
        /// </summary>
        [Binding]
        public bool IsUnlockEnabled { get; set; }

        [Binding]
        public LocalizedString Title { get; set; }

        [Binding]
        public LocalizedString Title2 { get; set; }

        [Binding]
        public LocalizedString ContextDescr { get; set; }

        [Binding]
        public LocalizedString ContextDescr2 { get; set; }

        [Binding]
        public LocalizedString SlotTypeDescr { get; set; }

        [Binding]
        public LocalizedString SlotTypeDescr2 { get; set; }

        [Binding]
        public LocalizedString PerkTypeDescr { get; set; }

        [Binding]
        public LocalizedString PerkTypeDescr2 { get; set; }

        [Binding]
        public int TitleBgColorIndex { get; set; }

        [Binding]
        public int PerkDescrColorIndex { get; set; }

        [Binding]
        public int PerkSlotDescrColorIndex { get; set; }

        [Binding]
        public bool IsPricesVisible { get; set; }

        [Binding]
        public bool IsPerkDisassemblyEnabled { get; set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel));
            _confirmationWindowId.AssertIfNull(nameof(_confirmationWindowId));
            _perkSlotTypes.AssertIfNull(nameof(_perkSlotTypes));
            _techPointViewModelPrefab.AssertIfNull(nameof(_techPointViewModelPrefab));
            _techPointsTransform.AssertIfNull(nameof(_techPointsTransform));
            _playerPointsSource.AssertIfNull(nameof(_playerPointsSource));
            _perksTitlesDefRef.Target.AssertIfNull(nameof(_perksTitlesDefRef));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SelectedPerkSlotRp.Dispose();
        }


        //=== Public ==========================================================

        public void Init(
            WindowsManager windowsManager,
            IPerkActionsPricesCalculator perkActionsPricesCalculator,
            PermanentPerksCollection permanentPerksCollection,
            PerkSlotsLimits perkSlotsLimits)
        {
            if (!windowsManager.AssertIfNull(nameof(windowsManager)))
                _windowsManager = windowsManager;

            _confirmationWindow = _windowsManager.GetWindow(_confirmationWindowId);
            _confirmationWindow.AssertIfNull(nameof(_confirmationWindow));

            if (!perkActionsPricesCalculator.AssertIfNull(nameof(perkActionsPricesCalculator)))
                _perkActionsPricesCalculator = perkActionsPricesCalculator;

            if (!permanentPerksCollection.AssertIfNull(nameof(permanentPerksCollection)))
                _permanentPerksCollection = permanentPerksCollection;

            if (!perkSlotsLimits.AssertIfNull(nameof(perkSlotsLimits)))
                _perkSlotsLimits = perkSlotsLimits;

            var availableTechPoints = CharacterPoints.GetAvailableTechPoints();
            foreach (var techPointDef in availableTechPoints)
            {
                var techPointViewModel = Instantiate(_techPointViewModelPrefab, _techPointsTransform);
                if (techPointViewModel.AssertIfNull(nameof(techPointViewModel)))
                    break;
                techPointViewModel.Set(techPointDef);
                _techPointViewModels.Add(techPointViewModel);
            }

            var hasTargetStream = SelectedPerkSlotRp.Func(D, target => target != null);
            Bind(hasTargetStream, () => HasTarget);

            var isFactionStagePerkStream = SelectedPerkSlotRp.Func(
                D,
                target =>
                {
                    if (target is TemporaryPerkSlotViewModel temporaryPerkSvm)
                        return temporaryPerkSvm.IsFactionStagePerk;

                    if (target is PermanentPerkSlotViewModel permanentPerkSvm)
                        return permanentPerkSvm.IsFactionStagePerk;

                    return false;
                });
            Bind(isFactionStagePerkStream, () => IsExtendedDescriptionArea);

            var currentPerkStream = SelectedPerkSlotRp.Func(D, perkSlot => perkSlot?.ItemResource);
            var currentPerkTypeStream = SelectedPerkSlotRp.Func(D, perkSlot => perkSlot?.ItemType);
            var hasPerkStream = currentPerkTypeStream.Func(D, perkType => perkType != null);
            Bind(hasPerkStream, () => HasPerk);
            var perkTypeDescrStream = currentPerkTypeStream.Func(D, perkType => GetTypeDescr(perkType, true));
            Bind(perkTypeDescrStream.Func(D, (ls, ls2) => ls), () => PerkTypeDescr);
            Bind(perkTypeDescrStream.Func(D, (ls, ls2) => ls2), () => PerkTypeDescr2);
            var perkDescrColorIndexStream = currentPerkTypeStream.Func(D, perkType => _perkSlotTypes.GetTypeIndex(perkType));
            Bind(perkDescrColorIndexStream, () => PerkDescrColorIndex);

            var slotTypeStream = SelectedPerkSlotRp.Func(D, perkSlotViewModel => (perkSlotViewModel as PermanentPerkSlotViewModel)?.PerkSlotType);
            var hasSlotTypeStream = slotTypeStream.Func(D, slotType => slotType != null);
            Bind(hasSlotTypeStream, () => HasSlot);
            var slotTypeDescrStream = slotTypeStream.Func(D, slotType => GetTypeDescr(slotType, false));
            Bind(slotTypeDescrStream.Func(D, (ls, ls2) => ls), () => SlotTypeDescr);
            Bind(slotTypeDescrStream.Func(D, (ls, ls2) => ls2), () => SlotTypeDescr2);

            Bind(slotTypeStream.Func(D, slotType => _perkSlotTypes.GetTypeIndex(slotType)), () => PerkSlotDescrColorIndex);

            var hasDestroyablePerkStream = hasPerkStream
                .Zip(D, isFactionStagePerkStream)
                .Func(D, (hasPerk, isFactionStagePerk) => hasPerk && !isFactionStagePerk);
            Bind(hasDestroyablePerkStream, () => HasDestroyablePerk);

            var isPerkDisassemblyEnabledInfoStream = currentPerkStream
                .Zip(D, isFactionStagePerkStream)
                .Zip(D, _perkActionsPricesCalculator.PerkActionsMultiplierRp)
                .Where(D, (perk, isFactionStagePerk, multiplier) => !isFactionStagePerk)
                .Func(D, (perk, isFactionStagePerk, multiplier) => GetIsPerkDisassemblyEnabledAndBenefits(perk));
            Bind(isPerkDisassemblyEnabledInfoStream.Func(D, (isDisassemblyEnabled, benefitsPrice) => isDisassemblyEnabled), () => IsPerkDisassemblyEnabled);
            Bind(isPerkDisassemblyEnabledInfoStream.Func(D, (isDisassemblyEnabled, benefitsPrice) => benefitsPrice), () => PerkDestroyBenefits);

            //(isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts)
            var unlockUpgradePropsStream = SelectedPerkSlotRp.Func(D, GetIsSlotUnlockableUpgradableAndCosts);
            var isSlotUnlockableStream = unlockUpgradePropsStream.Func(D, (isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts) => isSlotUnlockable);
            Bind(isSlotUnlockableStream, () => IsSlotUnlockable);
            var isSlotUpgradableStream = unlockUpgradePropsStream.Func(D, (isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts) => isSlotUpgradable);
            Bind(isSlotUpgradableStream, () => IsSlotUpgradable);
            var unlockOrUpgradeCostsStream = unlockUpgradePropsStream
                .Func(D, (isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts) => unlockOrUpgradeCosts);
            Bind(unlockOrUpgradeCostsStream, () => UnlockOrUpgradeCosts);

            var isSlotsOfNextTypeMaxCountReachedStream = slotTypeStream
                .Zip(D, isSlotUnlockableStream)
                .Zip(D, isSlotUpgradableStream)
                .Func(D, (slotType, isUnlockable, isUpgradable) => (isUnlockable || isUpgradable) && GetIsSlotsOfNextTypeMaxCountReached(slotType));

            var contextDescrStream = SelectedPerkSlotRp
                .Zip(D, isSlotsOfNextTypeMaxCountReachedStream)
                .Func(D, GetContextDescr);
            Bind(contextDescrStream.Func(D, (ls, ls2) => ls), () => ContextDescr);
            Bind(contextDescrStream.Func(D, (ls, ls2) => ls2), () => ContextDescr2);

            var contextTitleInfoStream = SelectedPerkSlotRp
                .Zip(D, isSlotUnlockableStream)
                .Zip(D, isSlotUpgradableStream)
                .Zip(D, isSlotsOfNextTypeMaxCountReachedStream)
                .Func(D, GetContextTitleAndBgColorIndex);

            Bind(contextTitleInfoStream.Func(D, (ls, ls2, idx) => ls), () => Title);
            Bind(contextTitleInfoStream.Func(D, (ls, ls2, idx) => ls2), () => Title2);
            Bind(contextTitleInfoStream.Func(D, (ls, ls2, idx) => idx), () => TitleBgColorIndex);

            var isPricesVisibleStream = contextDescrStream
                .Zip(D, isSlotUnlockableStream)
                .Zip(D, isSlotUpgradableStream)
                .Zip(D, isSlotsOfNextTypeMaxCountReachedStream)
                .Func(
                    D,
                    (contextDescr, isSlotUnlockable, isSlotUpgradable, isSlotsOfNextTypeMaxCountReached) =>
                        contextDescr.Item1.IsEmpty() && !isSlotsOfNextTypeMaxCountReached && (isSlotUnlockable || isSlotUpgradable));
            Bind(isPricesVisibleStream, () => IsPricesVisible);

            var isSufficientTechPointsStream = unlockUpgradePropsStream
                .Zip(D, CharacterPoints.TechPointsChangesStream)
                .Func(D, IsSufficientTechPointsForUnlockUpgrade);

            var isUnlockEnabledStream = isSlotUnlockableStream
                .Zip(D, isSufficientTechPointsStream)
                .Func(D, (isSlotUnlockable, isSufficient) => isSlotUnlockable && isSufficient);
            Bind(isUnlockEnabledStream, () => IsUnlockEnabled);

            var isUpgradeEnabledStream = isSlotUpgradableStream
                .Zip(D, isSufficientTechPointsStream)
                .Func(D, (isSlotUpgradable, isSufficient) => isSlotUpgradable && isSufficient);
            Bind(isUpgradeEnabledStream, () => IsUpgradeEnabled);

            SelectedPerkSlotRp.Value = null;
        }

        public void TakeContext(IContextViewTarget contextViewTarget)
        {
            SelectedPerkSlotRp.Value = contextViewTarget as PerkBaseSlotViewModel;
        }

        [UsedImplicitly]
        public void OnSlotUnlock()
        {
            void UnlockPerkSlot()
            {
                ClusterPerksCommands.UnlockPerkSlot(_ourCharacterSlotsViewModel.PlayerGuid, SelectedPerk, _perkSlotTypes);
                SoundControl.Instance.PerkUpgradeSlot.Post(gameObject);
            }

            if (SelectedPerk == null || SelectedPerk.Collection == PerkBaseSlotViewModel.PerksCollection.Temporary)
            {
                UI.Logger.IfError()?.Message($"{nameof(OnSlotUnlock)}() wrong {nameof(SelectedPerk)}: {SelectedPerk}").Write();
                return;
            }

            if (_windowsManager.AssertIfNull(nameof(_windowsManager)))
                return;

            //Это точно затраты
            if (UnlockOrUpgradeCosts.HasCosts())
            {
                var settings = new CostConfirmationSettings()
                {
                    Title = TitlesDef.UnlockSlotTitle,
                    Question = TitlesDef.SlotUnlockDialogQuestion,
                    CostsOrBenefits = UnlockOrUpgradeCosts,
                    IsCostNorBenefit = true,
                    OnOkAction = UnlockPerkSlot,
                };

                _windowsManager.Open(_confirmationWindow, _confirmationWindowId.PrefferedStackId, settings);
            }
            else
            {
                UnlockPerkSlot();
            }
        }

        [UsedImplicitly]
        public void OnSlotUpgrade()
        {
            void SlotUpgrade()
            {
                ClusterPerksCommands.UpgradePerkSlot(_ourCharacterSlotsViewModel.PlayerGuid, SelectedPerk, _perkSlotTypes);
                SoundControl.Instance.PerkUpgradeSlot.Post(gameObject);
            }

            if (_windowsManager.AssertIfNull(nameof(_windowsManager)))
                return;

            if (SelectedPerk == null || !IsSlotUpgradable)
            {
                UI.Logger.IfError()?.Message($"{nameof(SelectedPerk)} is null or isn't upgradable").Write();
                return;
            }

            if (UnlockOrUpgradeCosts.HasCosts())
            {
                var titles = GetUpgradeSlotTitle(SelectedPerk.PerkSlotType);
                var settings = new CostConfirmationSettings()
                {
                    Title = titles.Item1,
                    Title2 = titles.Item2,
                    Question = TitlesDef.SlotUpgradeDialogQuestion,
                    CostsOrBenefits = UnlockOrUpgradeCosts,
                    IsCostNorBenefit = true,
                    OnOkAction = SlotUpgrade,
                };

                _windowsManager.Open(_confirmationWindow, _confirmationWindowId.PrefferedStackId, settings);
            }
            else
            {
                SlotUpgrade();
            }
        }

        [UsedImplicitly]
        public void OnPerkDisassembly()
        {
            if (_windowsManager.AssertIfNull(nameof(_windowsManager)))
                return;

            var settings = new CostConfirmationSettings()
            {
                Title = TitlesDef.DestroyPerkDialogTitle,
                Question = TitlesDef.DestroyPerkDialogQuestion,
                CostsOrBenefits = PerkDestroyBenefits,
                IsCostNorBenefit = false,
                OnOkAction = () =>
                {
                    ClusterPerksCommands.DisassemblyPerk(_ourCharacterSlotsViewModel.PlayerGuid, SelectedPerk);
                    SoundControl.Instance.PerkParse.Post(gameObject);
                },
            };

            _windowsManager.Open(_confirmationWindow, _confirmationWindowId.PrefferedStackId, settings);
        }


        //=== Private =========================================================

        /// <summary>
        /// Возвращает, хватает ли средств на операцию анлока/апгрейда
        /// </summary>
        /// <param name="unlockUpgradeProps">(isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts)</param>
        /// <param name="techPoints">текущие средства</param>
        /// <returns></returns>
        private bool IsSufficientTechPointsForUnlockUpgrade((bool, bool, PriceDef) unlockUpgradeProps, IDictionary<CurrencyResource, int> techPoints)
        {
            var isSlotUnlockable = unlockUpgradeProps.Item1;
            var isSlotUpgradable = unlockUpgradeProps.Item2;
            var unlockOrUpgradeCosts = unlockUpgradeProps.Item3;

            if (!isSlotUnlockable && !isSlotUpgradable)
                return true;

            bool isSufficient = true;

            for (int i = 0; i < _techPointViewModels.Count; i++)
            {
                var techPointViewModel = _techPointViewModels[i];

                //предполагается показывать только расходы
                var availCount = techPoints.TryGetValue(techPointViewModel.TechPointDef, out var count) ? count : 0;
                var techPointCostCount = Math.Abs(
                    unlockOrUpgradeCosts.TechPointCosts
                        .FirstOrDefault(tpcd => tpcd.TechPoint.Target == techPointViewModel.TechPointDef)
                        .Count);
                techPointViewModel.Set(null, techPointCostCount, false, availCount);
                isSufficient &= techPointViewModel.IsSufficient;
            }

            return isSufficient;
        }

        private (LocalizedString, LocalizedString, int) GetContextTitleAndBgColorIndex(
            PerkBaseSlotViewModel perkBaseSvm,
            bool isSlotUnlockable,
            bool isSlotUpgradable,
            bool isSlotsOfNextTypeMaxCountReached)
        {
            LocalizedString title;
            LocalizedString title2;
            var titleBgColorIndex = -1;
            if (perkBaseSvm != null)
            {
                if (perkBaseSvm.IsLocked && isSlotUnlockable)
                {
                    title = TitlesDef.UnlockSlotTitle;
                    titleBgColorIndex = 0; //зел.
                }
                else
                {
                    if (perkBaseSvm.IsEmpty)
                    {
                        if (isSlotUpgradable && !isSlotsOfNextTypeMaxCountReached)
                        {
                            var titles = GetUpgradeSlotTitle(perkBaseSvm.PerkSlotType);
                            title = titles.Item1;
                            title2 = titles.Item2;
                            titleBgColorIndex = _perkSlotTypes.GetTypeIndex(perkBaseSvm.PerkSlotType, true);
                        }
                        else
                        {
                            title = TitlesDef.EmptySlotTitle;
                            titleBgColorIndex = _perkSlotTypes.GetTypeIndex(perkBaseSvm.PerkSlotType);
                        }
                    }
                    else
                    {
                        title = perkBaseSvm.ItemName;
                        titleBgColorIndex = _perkSlotTypes.GetTypeIndex(perkBaseSvm.ItemType);
                    }
                }
            }

            return (title, title2, titleBgColorIndex);
        }

        /// <summary>
        /// (Улучшить слот до, тип_слота)
        /// </summary>
        /// <param name="slotType"></param>
        /// <returns></returns>
        private (LocalizedString, LocalizedString) GetUpgradeSlotTitle(ItemTypeResource slotType)
        {
            return (TitlesDef.UpgradeSlotTitle, _perkSlotTypes.GetNextItemType(slotType)?.DescriptionLs ?? LsExtensions.Empty);
        }

        private (LocalizedString, LocalizedString) GetContextDescr(PerkBaseSlotViewModel perkBaseSvm, bool isSlotsOfNextTypeMaxCountReached)
        {
            LocalizedString descr;
            LocalizedString descr2;
            if (perkBaseSvm != null)
            {
                if (!perkBaseSvm.IsEmpty)
                {
                    descr = perkBaseSvm.ItemResource?.DescriptionLs ?? LsExtensions.Empty;
                }
                else
                {
                    if (isSlotsOfNextTypeMaxCountReached)
                        (descr, descr2) = GetSlotsOfTypeLimitReachedMessage(perkBaseSvm.PerkSlotType);
                    else if (perkBaseSvm.PerkSlotType == _perkSlotTypes.BestType)
                        descr = TitlesDef.BestSlotsTypeMessage;
                }
            }

            return (descr, descr2);
        }

        private (bool, PriceDef) GetIsPerkDisassemblyEnabledAndBenefits(BaseItemResource perk)
        {
            if (perk == null)
                return (false, default);

            var perksDisassemblyBenefits = _perkActionsPricesCalculator.GetPerkDisassemblyBenefits(perk);
            return (!perksDisassemblyBenefits.Equals(default(PriceDef)), perksDisassemblyBenefits);
        }

        /// <summary>
        /// Определяет, можно ли анлочить/апгрейдить слот и по какой цене
        /// </summary>
        /// <returns>(isSlotUnlockable, isSlotUpgradable, unlockOrUpgradeCosts)</returns>
        private (bool, bool, PriceDef) GetIsSlotUnlockableUpgradableAndCosts(PerkBaseSlotViewModel perkBaseSlotViewModel)
        {
            var isSlotUpgradable = false;
            var isSlotUnlockable = false;
            PriceDef costs;
            var permanentPerkSvm = perkBaseSlotViewModel as PermanentPerkSlotViewModel;
            if (permanentPerkSvm == null || permanentPerkSvm.IsFactionStagePerk)
                return (false, false, default);

            if (permanentPerkSvm.PerkSlotType == null)
            {
                isSlotUnlockable = true;
                costs = _perkActionsPricesCalculator.GetToPerkSlotUpgradingCosts(_perkSlotTypes.FirstType);
            }
            else
            {
                costs = _perkActionsPricesCalculator.GetToPerkSlotUpgradingCosts(
                    _perkSlotTypes.GetNextItemType(permanentPerkSvm.PerkSlotType));
                isSlotUpgradable = !costs.Equals(default(PriceDef));
            }

            return (isSlotUnlockable, isSlotUpgradable, costs);
        }

        private (LocalizedString, LocalizedString) GetTypeDescr(ItemTypeResource itemType, bool forPerk)
        {
            var localizedString = itemType?.DescriptionLs ?? LsExtensions.Empty;
            return localizedString.IsEmpty()
                ? (localizedString, LsExtensions.Empty)
                : (forPerk ? TitlesDef.Perk : TitlesDef.Slot, localizedString);
        }

        private bool GetIsSlotsOfNextTypeMaxCountReached(ItemTypeResource slotType)
        {
            var slotTypeIndex = _perkSlotTypes.GetTypeIndex(slotType, true);
            if (slotTypeIndex < 0 || slotTypeIndex >= _perkSlotsLimits.Limits.Length)
                return true;

            return _permanentPerksCollection.TotalSlotsCounts[slotTypeIndex] >= _perkSlotsLimits.Limits[slotTypeIndex];
        }

        private (LocalizedString, LocalizedString) GetSlotsOfTypeLimitReachedMessage(ItemTypeResource perkSlotType)
        {
            var nextTypeDescrLs = _perkSlotTypes.GetNextItemType(perkSlotType)?.DescriptionLs ?? LsExtensions.Empty;
            return (TitlesDef.SlotsOfTypeLimitReachedMessage, nextTypeDescrLs);
        }
    }
}