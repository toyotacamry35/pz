using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RedeemWidgetNode : DependencyEndNode
    {
        [SerializeField, UsedImplicitly]
        private bool _isTestMode;

        [SerializeField, UsedImplicitly]
        private WindowId _accountWindowId;

        [SerializeField, UsedImplicitly]
        private RedeemGuiWindow _redeemGuiWindow;

        [SerializeField, UsedImplicitly]
        private ServerServicesConfigDefRef _serverServicesConfigDefRef;

        private RedeemClientLogic _redeemClientLogic;


        //=== Props ===========================================================

        private ReactiveProperty<RedeemVmodel> RedeemVmodelRp { get; } = new ReactiveProperty<RedeemVmodel>();

        [Binding]
        public bool IsVisible { get; private set; }

        private static readonly PropertyBinder<RedeemWidgetNode, bool> IsVisibleBinder = PropertyBinder<RedeemWidgetNode>.Create(_ => _.IsVisible);


        //=== Public ==========================================================

        public override void AfterDependenciesInjected()
        {
            base.AfterDependenciesInjected();
            if (_serverServicesConfigDefRef.Target.AssertIfNull(nameof(_serverServicesConfigDefRef)))
                return;

            _redeemClientLogic = new RedeemClientLogic(_serverServicesConfigDefRef.Target, _isTestMode);
            RedeemVmodelRp.Value = _redeemClientLogic.RedeemVmodel;
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            if (_redeemGuiWindow.AssertIfNull(nameof(_redeemGuiWindow)) ||
                _accountWindowId.AssertIfNull(nameof(_accountWindowId)))
                return;

            var accountWindow = WindowsManager.GetWindow(_accountWindowId);
            if (accountWindow.AssertIfNull(nameof(accountWindow)))
                return;

            base.AfterDependenciesInjectedOnAllProviders();
            _redeemGuiWindow.SetRedeemVmodelStream(RedeemVmodelRp, _redeemClientLogic);

            var hasAccountDataStream = RedeemVmodelRp
                .SubStream(D, vm => vm.GetAccountDataStateRp)
                .Func(D, state => state == GetAccountDataState.HasAccountData);
            var hasAcceptedEmailAddressStream = RedeemVmodelRp
                .SubStream(D, vm => vm.HasAcceptedEmailAddressStream);
            var isVisibleByAccountIsOpenStream = accountWindow.State.Func(D, state => state == GuiWindowState.Opened);

            var isVisibleFinallyStream = hasAccountDataStream
                .Zip(D, hasAcceptedEmailAddressStream)
                .Zip(D, isVisibleByAccountIsOpenStream)
                .Func(D,
                    (hasAccountData, hasAcceptedEmailAddress, isAccountWindowOpen) =>
                    {
                        if (!hasAccountData || hasAcceptedEmailAddress) //не показываем пока не пришли данные ИЛИ пришли, но адрес уже подтвержден
                            return false;

                        return isAccountWindowOpen;
                    });

            Bind(isVisibleFinallyStream, IsVisibleBinder);
        }

        [UsedImplicitly]
        public void OnButton()
        {
            WindowsManager.Open(_redeemGuiWindow);
        }
    }
}