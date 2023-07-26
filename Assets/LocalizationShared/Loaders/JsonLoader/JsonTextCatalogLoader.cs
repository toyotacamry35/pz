using System;
using System.IO;
using Core.Environment.Logging.Extension;
using L10n.Plural;
using Newtonsoft.Json;

namespace L10n.Loaders
{
    public class JsonTextCatalogLoader : TextCatalogLoader
    {
        //=== Props ===========================================================

        protected override string FileExt => ".json";


        //=== Ctors ===========================================================

        public JsonTextCatalogLoader(string domainFileName, string localesPath, IPluralRuleGenerator pluralRuleGenerator = null)
            : base(domainFileName, localesPath, pluralRuleGenerator)
        {
        }


        //=== Protected =======================================================

        protected override LocalizationDef GetDomainLocalizationDef(string filePath)
        {
            if (!IsFileExists(filePath))
            {
                Logger.IfError()?.Message($"<{GetType()}> Unable to load from '{filePath}'").Write();
                return null;
            }

            var json = File.ReadAllText(filePath);
            LocalizationDef localizationDef = null;
            try
            {
                localizationDef = JsonConvert.DeserializeObject<LocalizationDef>(json);
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"<{GetType()}> Unable to deserialize from '{filePath}': {e.Message}\n{e.StackTrace}").Write();
                return null;
            }

            return localizationDef;
        }
    }
}