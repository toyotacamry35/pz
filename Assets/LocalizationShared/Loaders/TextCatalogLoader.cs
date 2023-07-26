using System.IO;
using System.Linq;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n.Plural;
using NLog;

namespace L10n.Loaders
{
    public abstract class TextCatalogLoader : ILoader
    {
        protected static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        protected string DomainFileName;
        protected string LocalesPath;


        //=== Props ===========================================================

        protected virtual string FileExt { get; } = "";

        /// <summary>
        /// Gets a current plural generator instance.
        /// </summary>
        public IPluralRuleGenerator PluralRuleGenerator { get; }


        //=== Ctor ============================================================

        protected TextCatalogLoader(string domainFileName, string localesPath, IPluralRuleGenerator pluralRuleGenerator = null)
        {
            DomainFileName = domainFileName;
            LocalesPath = localesPath;
            PluralRuleGenerator = pluralRuleGenerator ?? new DefaultPluralRuleGenerator();

            if (string.IsNullOrEmpty(DomainFileName))
                Logger.IfError()?.Message($"{nameof(DomainFileName)} is empty").Write();

            LocalesPath.AssertIfNull(nameof(LocalesPath));
        }


        //=== Public ==========================================================

        public void Load(TextCatalog textCatalog)
        {
            textCatalog.Translations.Clear();
            var filePath = FindTranslationFile(DomainFileName, textCatalog.CultureData.Folder);
            if (filePath == null)
            {
                Logger.IfWarn()?.Message($"Can't find file name by '{DomainFileName}' for {textCatalog} -- {this}").Write();
                return;
            }

            Logger.IfDebug()?.Message($"Loading translations from '{filePath}'...   -- {this}").Write(); //DEBUG
            var localizationDef = GetDomainLocalizationDef(filePath);
            if (localizationDef.AssertIfNull(nameof(localizationDef)))
                return;

            localizationDef.Translations.Where(kvp => kvp.Value != null).ForEach(kvp => textCatalog.Translations.Add(kvp.Key, kvp.Value));
            textCatalog.PluralRule = PluralRuleGenerator.CreateRule(textCatalog.CultureInfo);
        }

        public LocalizationDef GetLocalizationDef(string localizationFolderName)
        {
            return GetLocalizationDef(localizationFolderName, out var localizationDefRelPath);
        }

        public LocalizationDef GetLocalizationDef(string localizationFolderName, out string localizationDefRelPath)
        {
            localizationDefRelPath = FindTranslationFile(DomainFileName, localizationFolderName);
            if (localizationDefRelPath == null)
            {
                Logger.IfWarn()?.Message($"{nameof(GetLocalizationDef)}() Can't find file name by '{DomainFileName}' -- {this}").Write();
                return null;
            }

            return GetDomainLocalizationDef(localizationDefRelPath);
        }

        public override string ToString()
        {
            return
                $"[{GetType().Name}: {nameof(DomainFileName)}='{DomainFileName}', {nameof(LocalesPath)}='{LocalesPath}']";
        }


        //=== Protected =======================================================

        protected abstract LocalizationDef GetDomainLocalizationDef(string filePath);

        protected virtual bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }


        //=== Private =========================================================

        private string GetFileName(string domainFileName, string folderName)
        {
            return Path.Combine(LocalesPath, Path.Combine(folderName, domainFileName + FileExt));
        }

        private string FindTranslationFile(string domainFileName, string folderName)
        {
            var filePath = GetFileName(domainFileName, folderName);

            if (IsFileExists(filePath))
                return filePath;

            Logger.IfWarn()?.Message($"Can't find file name '{filePath}' -- {this}").Write();
            return null;
        }

//        private void AddTranslationsToTextCatalog(LocalizationDef localizationDef, TextCatalog textCatalog)
//        {
//            if (localizationDef.AssertIfNull(nameof(localizationDef)) ||
//                localizationDef.Translations.AssertIfNull(nameof(localizationDef.Translations)))
//                return;
//
//            foreach (var kvp in localizationDef.Translations)
//            {
//                if (string.IsNullOrEmpty(kvp.Key))
//                {
//                    Logger.IfError()?.Message($"<{GetType()}> Null or empty key in {nameof(localizationDef)}: {localizationDef}").Write();
//                    continue;
//                }
//
//                if (textCatalog.Translations.ContainsKey(kvp.Key))
//                    Logger.IfError()?.Message($"<{GetType()}> {nameof(textCatalog)} already contains translation [{kvp.Key}]").Write();
//
//                textCatalog.Translations[kvp.Key] = kvp.Value;
//            }
//
//
//            if (textCatalog.PluralRule == null && !string.IsNullOrWhiteSpace(localizationDef.PluralFormsRule))
//            {
//                (PluralRuleGenerator as IPluralRuleTextParser)?.SetPluralRuleText(localizationDef.PluralFormsRule);
//            }
//        }
    }
}