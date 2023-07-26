using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class LocalizationKeyPairsDef : BaseResource
    {
        public LocalizedString Ls1 { get; set; }
        public LocalizedString Ls2 { get; set; }

        public LocalizedString AltLs1 { get; set; }
        public LocalizedString AltLs2 { get; set; }

        public LocalizedString GetLS(bool firstOr2nd, bool alt = false) => alt ? (firstOr2nd ? AltLs1 : AltLs2) : (firstOr2nd ? Ls1 : Ls2);
    }
}