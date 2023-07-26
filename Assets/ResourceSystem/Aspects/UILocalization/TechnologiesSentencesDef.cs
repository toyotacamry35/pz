using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class TechnologiesSentencesDef : BaseResource
    {
        public LocalizedString ActivationConfirmTitle { get; set; }
        public LocalizedString ActivationConfirmQuestion { get; set; }
        public LocalizedString ActivationErrorMsg { get; set; }
        public LocalizedString ActivationErrorMsgWithResult { get; set; }
        public LocalizedString MessageAboutBlocking { get; set; }
        public LocalizedString MessageAboutLevelBlocking { get; set; }
    }
}