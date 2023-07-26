using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CostConfirmation : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private PlayerPointsSource _playerPointsSource;

        [SerializeField, UsedImplicitly]
        private TechPointViewModel _techPointViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _coinsTransform;

        Action _onOkAction;
        Action _onCancelAction;
        Action _currentAction;
        private List<TechPointViewModel> _techPointViewModels = new List<TechPointViewModel>();


        //=== Props ===============================================================

        private ICharacterPoints CharacterPoints => _playerPointsSource;

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }


        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;

        private static readonly PropertyBinder<CostConfirmation, bool> IsOpenBinder
            = PropertyBinder<CostConfirmation>.Create(_ => _.IsOpen);

        [Binding]
        public bool IsOpen { get; private set; }

        private ReactiveProperty<LocalizedString> _titleRp = new ReactiveProperty<LocalizedString>();

        private static readonly PropertyBinder<CostConfirmation, LocalizedString> TitleBinder
            = PropertyBinder<CostConfirmation>.Create(_ => _.Title);

        [Binding]
        public LocalizedString Title { get; private set; }

        private ReactiveProperty<LocalizedString> _title2Rp = new ReactiveProperty<LocalizedString>();

        private static readonly PropertyBinder<CostConfirmation, LocalizedString> Title2Binder
            = PropertyBinder<CostConfirmation>.Create(_ => _.Title2);

        [Binding]
        public LocalizedString Title2 { get; private set; }

        private ReactiveProperty<LocalizedString> _questionRp = new ReactiveProperty<LocalizedString>();

        private static readonly PropertyBinder<CostConfirmation, LocalizedString> QuestionBinder
            = PropertyBinder<CostConfirmation>.Create(_ => _.Question);

        [Binding]
        public LocalizedString Question { get; private set; }

        private ReactiveProperty<bool> _isSufficientRp = new ReactiveProperty<bool>();

        private static readonly PropertyBinder<CostConfirmation, bool> IsSufficientBinder
            = PropertyBinder<CostConfirmation>.Create(_ => _.IsSufficient);

        [Binding]
        public bool IsSufficient { get; private set; }

        private ReactiveProperty<bool> _hasBenefitsRp = new ReactiveProperty<bool>();

        private static readonly PropertyBinder<CostConfirmation, bool> HasBenefitsBinder
            = PropertyBinder<CostConfirmation>.Create(_ => _.HasBenefits);

        [Binding]
        public bool HasBenefits { get; private set; }



        //=== Unity ===========================================================

        private void Awake()
        {
            if (_windowId.AssertIfNull(nameof(_windowId)) ||
                _techPointViewModelPrefab.AssertIfNull(nameof(_techPointViewModelPrefab)) ||
                _coinsTransform.AssertIfNull(nameof(_coinsTransform)))
                return;

            State.Value = GuiWindowState.Closed;

            var availableTechPoints = _playerPointsSource.GetAvailableTechPoints();
            foreach (var techPoint in availableTechPoints)
            {
                var techPointViewModel = Instantiate(_techPointViewModelPrefab, _coinsTransform);
                if (techPointViewModel.AssertIfNull(nameof(techPointViewModel)))
                    break;

                techPointViewModel.Set(techPoint);
                _techPointViewModels.Add(techPointViewModel);
            }

            Bind(State.Func(D, state => state == GuiWindowState.Opened), IsOpenBinder);
            Bind(_titleRp, TitleBinder);
            Bind(_title2Rp, Title2Binder);
            Bind(_questionRp, QuestionBinder);
            Bind(_isSufficientRp, IsSufficientBinder);
            Bind(_hasBenefitsRp, HasBenefitsBinder);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==============================================================

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }

        public void OnOpen(object arg)
        {
            if (arg == null || !(arg is CostConfirmationSettings settings))
            {
                UI.Logger.Error($"<{GetType()}> OnOpen: no params");
                return;
            }

            _onOkAction = settings.OnOkAction;
            _onCancelAction = settings.OnCancelAction;
            _titleRp.Value = settings.Title;
            _title2Rp.Value = settings.Title2;
            _questionRp.Value = settings.Question;
            var isCostNorBenefit = settings.IsCostNorBenefit;
            _isSufficientRp.Value = GetIsSufficientAndTechPointViewModelsUpdate(settings.CostsOrBenefits, isCostNorBenefit);
            _hasBenefitsRp.Value = !isCostNorBenefit && settings.CostsOrBenefits.HasBenefits();
        }

        public void OnClose()
        {
            _currentAction?.Invoke();
            _currentAction = _onOkAction = _onCancelAction = null;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
        }

        [UsedImplicitly]
        public void ClickOk()
        {
            _currentAction = _onOkAction;
            WindowsManager.Close(this);
        }

        [UsedImplicitly]
        public void ClickCancel()
        {
            _currentAction = _onCancelAction;
            SoundControl.Instance.ButtonSmall.Post(transform.root.gameObject);
            WindowsManager.Close(this);
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }


        //=== Private ==============================================================

        private bool GetIsSufficientAndTechPointViewModelsUpdate(PriceDef costsOrBenefits, bool isCostNorBenefit)
        {
            var isSufficient = true;
            bool reqsIsEmpty = costsOrBenefits.Equals(default(PriceDef)) || costsOrBenefits.TechPointCosts == null;
            for (int i = 0; i < _techPointViewModels.Count; i++)
            {
                var techPointViewModel = _techPointViewModels[i];
                if (reqsIsEmpty)
                {
                    techPointViewModel.Set();
                    continue;
                }

                var techPointCostCount = Math.Abs(
                    costsOrBenefits.TechPointCosts
                        .FirstOrDefault(tpcd => tpcd.TechPoint.Target == techPointViewModel.TechPointDef)
                        .Count);

                var availCount = isCostNorBenefit
                    ? CharacterPoints.GetTechPointsCount(techPointViewModel.TechPointDef)
                    : Int32.MaxValue;
                techPointViewModel.Set(null, techPointCostCount, !isCostNorBenefit, availCount);

                isSufficient &= techPointViewModel.IsSufficient;
            }

            return isSufficient;
        }
    }
}