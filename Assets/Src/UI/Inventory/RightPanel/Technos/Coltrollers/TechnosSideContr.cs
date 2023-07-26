using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;
using Assets.Src.ContainerApis;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Building;
using SharedCode.Entities.Engine;
using Uins.Inventory;

namespace Uins
{
    [Binding]
    public class TechnosSideContr : BindingController<TechnoItemVmodel>, IContextViewTargetWithParams
    {
        [SerializeField, UsedImplicitly]
        private TechnoAtlasContr _technoAtlasContr;

        [SerializeField, UsedImplicitly]
        private TechnologiesSentencesDefRef _sentencesDefRef;

        [SerializeField, UsedImplicitly]
        private WindowId _confirmationWindowId;

        [SerializeField, UsedImplicitly]
        private RequiredScienceCountContr _requiredScienceCountContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _requiredScienceCountContrTransform;

        [SerializeField, UsedImplicitly]
        private RequiredCurrencyCountContr _requiredCurrencyCountContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _requiredCurrencyCountContrTransform;

        [SerializeField, UsedImplicitly]
        private TechnoContextCraftRecipeContr _technoContextCraftRecipeContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _technoContextCraftRecipeContrTransform;

        [SerializeField, UsedImplicitly]
        private ContextViewWithParams _contextViewWithParams;

        private IGuiWindow _confirmationWindow;
        private WindowsManager _windowsManager;
        private EntityApiWrapper<KnowledgeEngineFullApi> _knowledgeEngineFullApiWrapper;

        private readonly ContextViewParams _contextViewParams = new ContextViewParams()
        {
            Layout = ContextViewParams.LayoutType.ExtraSpaceWithPointsPanels
        };


        //=== Props ===========================================================

        private TechnologiesSentencesDef SentencesDef => _sentencesDefRef.Target;

        [Binding]
        public LocalizedString Title { get; private set; }

        [Binding]
        public LocalizedString Description { get; private set; }

        [Binding, UsedImplicitly]
        public LocalizedString MessageAboutBlocking => SentencesDef.MessageAboutBlocking;

        [Binding, UsedImplicitly]
        public LocalizedString MessageAboutLevelBlocking => SentencesDef.MessageAboutLevelBlocking;

        public TechnoItemContr.ActivationState State { get; private set; }

        [Binding]
        public int ActivationStateIndex { get; private set; }

        [Binding]
        public bool ShowRequirementsAndButton { get; private set; }

        /// <summary>
        /// С учетом блокировки
        /// </summary>
        [Binding]
        public bool IsEnableToActivate { get; private set; }

        [Binding]
        public bool IsBlocked { get; private set; }

        [Binding]
        public bool DoesMeetLevelRequirement { get; private set; }

        [Binding]
        public int LevelRequirement { get; private set; }

        [Binding]
        public bool IsEmpty { get; private set; }

        [Binding]
        public bool HasBuildRecipes { get; private set; }

        [Binding]
        public Sprite BuildRecipesSprite { get; private set; }

        public bool IsCostsRequired { get; private set; }

        public TechnologyDef TechnologyDef { get; private set; }

        public ListStream<BuildRecipeDef> BuildRecipesSubListStream { get; private set; }

        public ListStream<TechnoContextCraftRecipeVmodel> CraftRecipeVmodelsListStream { get; } = new ListStream<TechnoContextCraftRecipeVmodel>();

        public InventoryTabType? TabType => InventoryTabType.Technologies;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_requiredScienceCountContrTransform.AssertIfNull(nameof(_requiredScienceCountContrTransform)) ||
                _requiredScienceCountContrPrefab.AssertIfNull(nameof(_requiredScienceCountContrPrefab)) ||
                _requiredCurrencyCountContrTransform.AssertIfNull(nameof(_requiredCurrencyCountContrTransform)) ||
                _requiredCurrencyCountContrPrefab.AssertIfNull(nameof(_requiredCurrencyCountContrPrefab)) ||
                _technoAtlasContr.AssertIfNull(nameof(_technoAtlasContr)) ||
                _sentencesDefRef.AssertIfNull(nameof(_sentencesDefRef)) ||
                _confirmationWindowId.AssertIfNull(nameof(_confirmationWindow)) ||
                _technoContextCraftRecipeContrPrefab.AssertIfNull(nameof(_technoContextCraftRecipeContrPrefab)) ||
                _technoContextCraftRecipeContrTransform.AssertIfNull(nameof(_technoContextCraftRecipeContrTransform)) ||
                _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams)))
                return;

            var scienceContrsPool = new BindingControllersPool<RequirementVmodel<ScienceDef>>(
                _requiredScienceCountContrTransform, _requiredScienceCountContrPrefab);
            var currencyContrsPool = new BindingControllersPool<RequirementVmodel<CurrencyResource>>(
                _requiredCurrencyCountContrTransform, _requiredCurrencyCountContrPrefab);

            var requiredScienceVmodelStream = Vmodel.SubListStream(D, vm => vm.SciencesRequirementsVmodel.RequirementVmodels);
            scienceContrsPool.Connect(requiredScienceVmodelStream);

            var requiredCurrencyVmodelStream = Vmodel.SubListStream(D, vm => vm.CurrenciesRequirementsVmodel.RequirementVmodels);
            currencyContrsPool.Connect(requiredCurrencyVmodelStream);

            _technoAtlasContr.Vmodel
                .SubStream(D, vm => vm.SelectedTabVmodelStream)
                .SubStream(D, vm => vm.SelectedItemRp)
                .Action(D, SetVmodel);

            Bind(Vmodel.SubStream(D, vm => vm.TechnologyNameRp, LsExtensions.Empty), () => Title);
            Bind(Vmodel.SubStream(D, vm => vm.TechnologyDescriptionRp, LsExtensions.Empty), () => Description);

            var isActivatedStream = Vmodel.SubStream(D, vm => vm.IsActivatedRp); //чистый признак
            var isBlockedStream = Vmodel.SubStream(D, vm => vm.IsBlockedByMutationRp); //чистый признак
            Bind(isBlockedStream, () => IsBlocked);
            var doesMeetLevelRequirementStream = Vmodel.SubStream(D, vm => vm.DoesMeetLevelRequirementRp, true);
            Bind(doesMeetLevelRequirementStream, () => DoesMeetLevelRequirement);
            var levelRequirementStream = Vmodel.Func(D, vm => vm?.TechnologyItem.Technology.Target?.ActivateConditions.Requirements.Level ?? 0);
            Bind(levelRequirementStream, () => LevelRequirement);

            var isEnableToActivateStream = Vmodel.SubStream(D, vm => vm.IsEnableToActivateByRequirementsRp); //суммарные требования, без учета блокировки
            var activationStateStream = isActivatedStream
                .Zip(D, isEnableToActivateStream)
                .Zip(D, isBlockedStream)
                .Func(D, TechnoItemContr.GetActivationState);

            var isEnableToActivateFinallyStream = activationStateStream.Func(D, state => state == TechnoItemContr.ActivationState.EnableToActivate);
            Bind(isEnableToActivateFinallyStream, () => IsEnableToActivate);

            Bind(activationStateStream, () => State);
            Bind(activationStateStream.Func(D, state => (int) state), () => ActivationStateIndex);

            var isEmptyVmStream = Vmodel.Func(D, vm => vm == null);

            var showRequirementsAndButtonStream = isEmptyVmStream
                .Zip(D, activationStateStream)
                .Zip(D, isBlockedStream)
                .Func(D, (isEmpty, state, isBlocked) => !isEmpty && (isBlocked || state != TechnoItemContr.ActivationState.Activated));
            Bind(showRequirementsAndButtonStream, () => ShowRequirementsAndButton);

            Bind(Vmodel.Func(D, vm => vm?.TechnologyItem.Technology.Target?.ActivateConditions.IsRequiredCurrency ?? false), () => IsCostsRequired);

            Bind(isEmptyVmStream, () => IsEmpty);

            var technologyDefStream = Vmodel.Func(D, vm => vm?.TechnologyItem.Technology.Target);
            Bind(technologyDefStream, () => TechnologyDef);

            var isActivatedFinallyStream = activationStateStream.Func(D, state => state == TechnoItemContr.ActivationState.Activated);
            var craftRecipesControllersPool = new BindingControllersPoolWithUsingProp<TechnoContextCraftRecipeVmodel>(
                _technoContextCraftRecipeContrTransform,
                _technoContextCraftRecipeContrPrefab);

            var technoContextCraftRecipeVmodelsStream = Vmodel
                .SubListStream(D, vm => vm.CraftRecipesListStream)
                .Func(D, craftRecipeDef => new TechnoContextCraftRecipeVmodel(craftRecipeDef, isActivatedFinallyStream, OnRecipeClick));
            craftRecipesControllersPool.Connect(technoContextCraftRecipeVmodelsStream);

            BuildRecipesSubListStream = Vmodel.SubListStream(D, vm => vm.BuildRecipesListStream);
            var hasBuildRecipesStream = BuildRecipesSubListStream.CountStream.Func(D, count => count > 0);
            Bind(hasBuildRecipesStream, () => HasBuildRecipes);

            var buildRecipesSpriteStream = technologyDefStream.Func(D, technologyDef => technologyDef?.BuildRecipesImage?.Target);
            Bind(buildRecipesSpriteStream, () => BuildRecipesSprite);
        }


        //=== Public ==========================================================

        public ContextViewParams GetContextViewParamsForOpening()
        {
            return _contextViewParams;
        }

        public void Init(IPawnSource pawnSource, ICharacterPoints characterPoints, WindowsManager windowsManager)
        {
            _windowsManager = windowsManager;
            if (_windowsManager.AssertIfNull(nameof(_windowsManager)) ||
                pawnSource.AssertIfNull(nameof(pawnSource)) ||
                characterPoints.AssertIfNull(nameof(characterPoints)))
                return;

            _confirmationWindow = _windowsManager.GetWindow(_confirmationWindowId);
            if (_confirmationWindow.AssertIfNull(nameof(_confirmationWindow)))
                return;

            pawnSource.PawnChangesStream.Action(D, (prevEgo, currEgo) =>
            {
                if (prevEgo != null)
                {
                    _knowledgeEngineFullApiWrapper.Dispose();
                    _knowledgeEngineFullApiWrapper = null;
                }

                if (currEgo != null)
                {
                    _knowledgeEngineFullApiWrapper = EntityApi.GetWrapper<KnowledgeEngineFullApi>(currEgo.OuterRef);
                }
            });
        }

        [UsedImplicitly]
        public void OnActivateButton()
        {
            if (State == TechnoItemContr.ActivationState.EnableToActivate)
            {
                if (IsCostsRequired)
                {
                    SoundControl.Instance.ButtonSmall.Post(transform.root.gameObject);
                    var costConfirmationSettings = new CostConfirmationSettings()
                    {
                        OnOkAction = TechnologyActivation,
                        IsCostNorBenefit = true,
                        CostsOrBenefits = new PriceDef() {TechPointCosts = TechnologyDef.ActivateConditions.Cost},
                        Title = SentencesDef.ActivationConfirmTitle,
                        Title2 = Title,
                        Question = SentencesDef.ActivationConfirmQuestion
                    };
                    _windowsManager.Open(_confirmationWindow, null, costConfirmationSettings);
                }
                else
                {
                    TechnologyActivation();
                }
            }
            else
                UI.Logger.IfError()?.Message($"Attempt to activate technology {TechnologyDef} with state={State}").Write();
        }


        //=== Private =========================================================

        private void OnRecipeClick(CraftRecipeDef craftRecipeDef)
        {
            _contextViewWithParams.OpenRecipeByDef(craftRecipeDef);
        }

        private void TechnologyActivation()
        {
            _knowledgeEngineFullApiWrapper?.EntityApi.TechnologyActivation(TechnologyDef, OnUpgradeTechnologyResult);
        }

        private void OnUpgradeTechnologyResult(TechnologyOperationResult technologyOperationResult)
        {
            if (technologyOperationResult != TechnologyOperationResult.Success)
                UI.Logger.IfWarn()?.Message($"{TechnologyDef}: TechnologyOperationResult={technologyOperationResult}").Write(); //DEBUG
            if (technologyOperationResult != TechnologyOperationResult.Success)
                WarningMessager.Instance.ShowErrorMessage(SentencesDef.ActivationErrorMsgWithResult.GetText(0, technologyOperationResult));
        }
    }
}