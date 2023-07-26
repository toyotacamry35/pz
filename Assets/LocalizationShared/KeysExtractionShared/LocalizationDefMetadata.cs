using Core.Environment.Logging.Extension;
using L10n.Loaders;
using NLog;

namespace L10n
{
    public class LocalizationDefMetadata
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");

        public LocalizationDef LocalizationDef;
        public bool IsChanged;
        public bool IsDevelop;
        public string RelPath;

        public LocalizationDefMetadata(LocalizationDef localizationDef, string relPath, bool isDevelop)
        {
            LocalizationDef = localizationDef;
            RelPath = relPath;
            IsDevelop = isDevelop;

            if (LocalizationDef.AssertIfNull(nameof(LocalizationDef)) ||
                string.IsNullOrEmpty(RelPath))
                Logger.IfError()?.Message($"Wrong init: {nameof(LocalizationDef)}={localizationDef}, {nameof(RelPath)}='{RelPath}'").Write();
        }

        public static LocalizationDefMetadata GetLocalizationDefMeta(string localizationFolderName, TextCatalogLoader loader, bool isDevelop = false)
        {
            var localizationDef = loader.GetLocalizationDef(localizationFolderName, out var localizationRelPath);
            if (localizationDef.AssertIfNull(nameof(localizationDef)))
                return null;

            return new LocalizationDefMetadata(localizationDef, localizationRelPath, isDevelop);
        }
    }
}