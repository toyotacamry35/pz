using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using TMPro;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RedeemGuiWindow : BaseGuiWindow
    {
        public enum TextColor
        {
            Normal,
            Error,
            Success
        }

        private RedeemClientLogic _redeemClientLogic;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _emailInputField;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _codeInputField;

        [SerializeField, UsedImplicitly]
        private RedeemWindowMessagesDefRef _redeemWindowMessagesDefRef;

        private string _codeText = "";

        private string _emailText = "";

        private ReactiveProperty<bool> _isValidMailRp = new ReactiveProperty<bool>() {Value = true};


        //=== Props ===========================================================

        private RedeemWindowMessagesDef MessagesDef => _redeemWindowMessagesDefRef.Target;

        [Binding]
        public bool IsEmailButtonActive { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, bool> IsEmailButtonActiveBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.IsEmailButtonActive);

        [Binding]
        public bool IsCodeButtonActive { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, bool> IsCodeButtonActiveBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.IsCodeButtonActive);

        [Binding]
        public bool IsCodeFieldActive { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, bool> IsCodeFieldActiveBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.IsCodeFieldActive);

        public ReactiveProperty<bool> IsEmailWrongRp { get; } = new ReactiveProperty<bool>() {Value = true};

        public ReactiveProperty<bool> IsCodeWrongRp { get; } = new ReactiveProperty<bool>() {Value = true};

        [Binding]
        public LocalizedString EnterMailText { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, LocalizedString> EnterMailTextBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.EnterMailText);

        [Binding]
        public int EnterMailTextColorIndex { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, int> EnterMailTextColorIndexBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.EnterMailTextColorIndex);

        [Binding]
        public LocalizedString EnterCodeText { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, LocalizedString> EnterCodeTextBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.EnterCodeText);


        [Binding]
        public int EnterCodeTextColorIndex { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, int> EnterCodeTextColorIndexBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.EnterCodeTextColorIndex);

        [Binding]
        public LocalizedString CloseButtonText { get; private set; }

        private static readonly PropertyBinder<RedeemGuiWindow, LocalizedString> CloseButtonTextBinder
            = PropertyBinder<RedeemGuiWindow>.Create(_ => _.CloseButtonText);



        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            _emailInputField.AssertIfNull(nameof(_emailInputField));
            _codeInputField.AssertIfNull(nameof(_codeInputField));
            _redeemWindowMessagesDefRef.Target.AssertIfNull(nameof(_redeemWindowMessagesDefRef));
        }


        //=== Public ==========================================================

        public void SetRedeemVmodelStream(IStream<RedeemVmodel> redeemVmodelStream, RedeemClientLogic redeemClientLogic)
        {
            if (redeemVmodelStream.AssertIfNull(nameof(redeemVmodelStream)) ||
                redeemClientLogic.AssertIfNull(nameof(redeemClientLogic)))
                return;

            _redeemClientLogic = redeemClientLogic;

            var hasSessionIdStream = redeemVmodelStream.SubStream(D, vm => vm.HasSessionIdStream);
            Bind(hasSessionIdStream, IsCodeFieldActiveBinder);

            var hasAcceptedEmailAddressStream = redeemVmodelStream.SubStream(D, vm => vm.HasAcceptedEmailAddressStream);
            Bind(
                hasAcceptedEmailAddressStream.Func(
                    D,
                    hasAccepted => hasAccepted ? MessagesDef.CloseButtonTextSuccess : MessagesDef.CloseButtonTextNormal),
                CloseButtonTextBinder);

            var emailMessageStateStream = hasAcceptedEmailAddressStream
                .Zip(D, _isValidMailRp)
                .Zip(D, _redeemClientLogic.SendMailErrorRp)
                .Func(D, OnEmailMessageState);

            Bind(emailMessageStateStream.Func(D, (ls, color) => ls), EnterMailTextBinder);
            Bind(emailMessageStateStream.Func(D, (ls, color) => (int) color), EnterMailTextColorIndexBinder);

            var codeMessageStateStream = redeemVmodelStream.SubStream(D, vm => vm.HasAcceptedEmailAddressStream)
                .Zip(D, _redeemClientLogic.SendCodeErrorRp)
                .Func(D, OnCodeMessageState);

            Bind(codeMessageStateStream.Func(D, (ls, color) => ls), EnterCodeTextBinder);
            Bind(codeMessageStateStream.Func(D, (ls, color) => (int) color), EnterCodeTextColorIndexBinder);

            var isEmailButtonActiveStream = redeemClientLogic.IsEmailSendingAllowedStream
                .Zip(D, IsEmailWrongRp)
                .Func(D, (isSendingAlowed, isEmailWrong) => isSendingAlowed && !isEmailWrong);
            Bind(isEmailButtonActiveStream, IsEmailButtonActiveBinder);

            var isCodeButtonActiveStream = redeemClientLogic.IsCodeSendingAllowedStream
                .Zip(D, IsCodeWrongRp)
                .Func(D, (isSendingAlowed, isCodeWrong) => isSendingAlowed && !isCodeWrong);
            Bind(isCodeButtonActiveStream, IsCodeButtonActiveBinder);
        }

        [UsedImplicitly]
        public void OnMailButton()
        {
            _redeemClientLogic?.SendEmail(_emailText);
        }

        [UsedImplicitly]
        public void OnCodeButton()
        {
            _redeemClientLogic?.SendCode(_codeText);
        }

        [UsedImplicitly]
        public void OnCloseButton()
        {
            WindowsManager.Close(this);
        }

        [UsedImplicitly]
        public void OnCodeTextChanged()
        {
            _codeText = _codeInputField.text;
            IsCodeWrongRp.Value = string.IsNullOrEmpty(_codeText);
        }

        [UsedImplicitly]
        public void OnEmailTextChanged()
        {
            _emailText = _emailInputField.text;
            var isValidMail = _redeemClientLogic.IsValidEmail(_emailText);
            IsEmailWrongRp.Value = !isValidMail;
            _isValidMailRp.Value = isValidMail;
        }


        //=== Private =========================================================

        /// <summary>
        /// Стейт-машина текста над полем ввода почты
        /// </summary>
        private (LocalizedString, TextColor) OnEmailMessageState(bool hasAcceptedEmail, bool isValidMail, bool sendMailError)
        {
            if (hasAcceptedEmail)
                return (LsExtensions.Empty, TextColor.Normal);

            if (!isValidMail)
                return (MessagesDef.ErrNotEmailMessage, TextColor.Error);

            if (sendMailError)
                return (MessagesDef.SendMailError, TextColor.Error);

            return (MessagesDef.EnterMailDescription, TextColor.Normal);
        }

        /// <summary>
        /// Стейт-машина текста над полем ввода почты
        /// </summary>
        private (LocalizedString, TextColor) OnCodeMessageState(bool hasAcceptedEmail, bool sendCodeError)
        {
            if (hasAcceptedEmail)
                return (MessagesDef.HasAcceptedMailMessage, TextColor.Success);

            if (sendCodeError)
                return (MessagesDef.SendCodeError, TextColor.Error);

            return (MessagesDef.EnterCodeDescription, TextColor.Normal);
        }
    }
}