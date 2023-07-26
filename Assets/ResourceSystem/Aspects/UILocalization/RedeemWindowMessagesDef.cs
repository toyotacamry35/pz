using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class RedeemWindowMessagesDef : BaseResource
    {
        public LocalizedString HasAcceptedMailMessage { get; set; }
        
        public LocalizedString EnterMailDescription { get; set; }
        public LocalizedString EnterCodeDescription { get; set; }

        public LocalizedString ErrNotEmailMessage { get; set; }
        public LocalizedString SendMailError { get; set; }
        public LocalizedString SendCodeError { get; set; }

        public LocalizedString CloseButtonTextNormal { get; set; }
        public LocalizedString CloseButtonTextSuccess { get; set; }
    }
}