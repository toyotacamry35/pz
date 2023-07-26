using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using Uins.Inventory;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    public delegate void TargetChangedDelegate(IContextViewTarget target, bool isContextViewBlocked);

    [Binding]
    public class ContextViewWithParams : BindingController<ContextViewWithParamsVmodel>
    {
        [SerializeField, UsedImplicitly]
        private CanvasGroup[] _hideExtraSpaceCanvasGroups;

        [SerializeField, UsedImplicitly]
        private CanvasGroup _extraSpaceCanvasGroup;

        [SerializeField, UsedImplicitly]
        private CanvasGroup _techPointsPanelCanvasGroup;

        [SerializeField, UsedImplicitly]
        private ContextViewTitlesDefRef _contextViewTitlesDefRef;

        [SerializeField, UsedImplicitly]
        private TabsContextContr _tabsContextContr;

        [SerializeField, UsedImplicitly]
        private CraftSideViewModel _craftSideViewModel;

        [SerializeField, UsedImplicitly]
        private MachineCraftSideViewModel _machineCraftSideViewModel;

        private SelectContextLogic _selectContextLogic;


        //=== Props ===========================================================

        private Sprite SlotContextIcon => _contextViewTitlesDefRef.Target.ItemSlot.Sprite?.Target;

        private LocalizedString SlotContextTitle => _contextViewTitlesDefRef.Target.ItemSlot.Ls;

        public ReactiveProperty<bool> IsMachineTabOpenedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        [Binding]
        public bool HasContext { get; set; }

        [Binding]
        public Sprite ContextIcon { get; set; }

        [Binding]
        public LocalizedString ContextTitle { get; set; }

        [Binding]
        public bool HasBgFinally { get; set; }

        [Binding]
        public bool HasTitleFinally { get; set; }

        [Binding]
        public bool IsContextIconVisible { get; set; }

        [Binding]
        public bool IsWarningTitle { get; set; }

        [Binding]
        public bool IsMinified { get; set; }


        //=== Unity ===========================================================

        private void Start()
        {
            if (_tabsContextContr.AssertIfNull(nameof(_tabsContextContr)) ||
                SlotContextIcon.AssertIfNull(nameof(SlotContextIcon)) ||
                _hideExtraSpaceCanvasGroups.IsNullOrEmptyOrHasNullElements(nameof(_hideExtraSpaceCanvasGroups)) ||
                _extraSpaceCanvasGroup.AssertIfNull(nameof(_extraSpaceCanvasGroup)) ||
                _techPointsPanelCanvasGroup.AssertIfNull(nameof(_techPointsPanelCanvasGroup)) ||
                _contextViewTitlesDefRef.Target.AssertIfNull(nameof(_contextViewTitlesDefRef)) ||
                _craftSideViewModel.AssertIfNull(nameof(_craftSideViewModel)) ||
                _machineCraftSideViewModel.AssertIfNull(nameof(_machineCraftSideViewModel)))
                return;

            var hasContextStream = Vmodel.SubStream(D, vm => vm.CurrentContext).Func(D, contextTarget => contextTarget != null);
            Bind(hasContextStream, () => HasContext);

            var contextParamsStream = Vmodel.SubStream(D, vm => vm.ContextParamsStream, new ContextViewParams());
            var isWarningTitleStream = contextParamsStream
                .Func(D, cParams => cParams?.HasWarningFlag ?? false);
            Bind(isWarningTitleStream, () => IsWarningTitle);

            var needForExtraSpaceStream = contextParamsStream
                .Func(D, cParams => cParams?.NeedForExtraSpace ?? false);
            needForExtraSpaceStream.Action(D, OnNeedForExtraSpaceChanged);

            var showTechPanelOnExtraSpaceStream = contextParamsStream
                .Func(D, cParams => cParams != null && cParams.Layout == ContextViewParams.LayoutType.ExtraSpaceWithPointsPanels);
            showTechPanelOnExtraSpaceStream.Action(D, OnShowTechPanelOnExtraSpaceChanged);

            _tabsContextContr.CurrentTabStream.Func(D, (type) => type == InventoryTabType.Machine).Bind(D, IsMachineTabOpenedRp);
            Bind(IsMachineTabOpenedRp, () => IsMinified);

            var contextWithParamsStream = Vmodel.SubStream(D, vm => vm.CurrentContextWithParamsRp);
            var contextIconStream = contextWithParamsStream
                .Func(D, (target, cvParams) => (target is SlotViewModel) ? SlotContextIcon : cvParams?.ContextIcon);
            Bind(contextIconStream, () => ContextIcon);

            var contextTitleStream = contextWithParamsStream
                .Func(D, (target, cvParams) => (target is SlotViewModel) ? SlotContextTitle : cvParams?.ContextTitleLs ?? LsExtensions.Empty);
            Bind(contextTitleStream, () => ContextTitle);

            var hasContextTitleOrIconStream = contextIconStream
                .Zip(D, contextTitleStream)
                .Func(D, (contextIcon, contextTitle) => contextIcon != null || !contextTitle.IsEmpty());

            var hasBgFinallyStream = contextParamsStream.Func(D, cParams => cParams.Layout != ContextViewParams.LayoutType.NoTitleAndBackground);
            Bind(hasBgFinallyStream, () => HasBgFinally);

            var hasTitleFinallyStream = hasContextTitleOrIconStream
                .Zip(D, hasBgFinallyStream)
                .Func(D, (hasContextTitleOrIcon, needBgAndTitle) => hasContextTitleOrIcon && needBgAndTitle);
            Bind(hasTitleFinallyStream, () => HasTitleFinally);

            var isContextIconVisibleStream = contextIconStream.Func(D, contextIcon => contextIcon != null);
            Bind(isContextIconVisibleStream, () => IsContextIconVisible);

            CreateVmodel();

            _selectContextLogic = new SelectContextLogic(this, _craftSideViewModel, _machineCraftSideViewModel);
        }

        private void CreateVmodel()
        {
            var contextViewWithParamsVmodel = new ContextViewWithParamsVmodel(_tabsContextContr.GetTabVmodels());
            SetVmodel(contextViewWithParamsVmodel); //Единственная установка Vmodel
            _tabsContextContr.SetVmodel(contextViewWithParamsVmodel);
        }


        //=== Public ==========================================================

        public static void SwitchCanvasGroup(CanvasGroup canvasGroup, bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.blocksRaycasts = canvasGroup.interactable = isVisible;
        }

        public void OpenRecipeByDef(CraftRecipeDef craftRecipeDef)
        {
            _selectContextLogic.SelectRecipe(craftRecipeDef);
        }


        //=== Private ==========================================================

        private void OnNeedForExtraSpaceChanged(bool needForExtraSpace)
        {
            for (int i = 0; i < _hideExtraSpaceCanvasGroups.Length; i++)
                SwitchCanvasGroup(_hideExtraSpaceCanvasGroups[i], !needForExtraSpace);

            SwitchCanvasGroup(_extraSpaceCanvasGroup, needForExtraSpace);
        }

        private void OnShowTechPanelOnExtraSpaceChanged(bool showTechPanelOnExtraSpace)
        {
            SwitchCanvasGroup(_techPointsPanelCanvasGroup, showTechPanelOnExtraSpace);
        }
    }
}