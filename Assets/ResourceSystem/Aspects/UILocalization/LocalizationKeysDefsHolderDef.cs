using Assets.Src.ResourcesSystem.Base;
 
namespace L10n
{
    /// <summary>
    /// Holder of resources needed where is no elegant way to pass it to (f.e. at an static class)
    /// </summary>
    [Localized]
    public class LocalizationKeysDefsHolderDef : BaseResource
    {
        //Settings window:
        public ResourceRef<LocalizationKeysDef> FullscreenModeLocalizationKeys   { get; set; }
        public ResourceRef<LocalizationKeysDef> QualityLvlLocalizationKeys       { get; set; }
        public ResourceRef<LocalizationKeysDef> LanguageLocalizationKeys         { get; set; }
        public ResourceRef<LocalizationKeyPairsDef> OnOffLocalizationKeyPairsDef { get; set; }
    }
}
