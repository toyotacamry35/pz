using ResourcesSystem.Loader;
using L10n.Plural;

namespace L10n.Loaders
{
    public class JdbLoader : TextCatalogLoader
    {
        //=== Props ===========================================================

        private GameResources _gameResources;


        //=== Ctors ===========================================================

        public JdbLoader(GameResources gameResources, string domainFileName, string localesPath, IPluralRuleGenerator pluralRuleGenerator = null)
            : base(domainFileName, localesPath, pluralRuleGenerator)
        {
            _gameResources = gameResources;
            _gameResources.AssertIfNull(nameof(_gameResources));
        }


        //=== Protected =======================================================

        protected override LocalizationDef GetDomainLocalizationDef(string filePath)
        {
            return _gameResources.LoadResource<LocalizationDef>(filePath);
        }

        protected override bool IsFileExists(string filePath)
        {
            return _gameResources.IsResourceExists(filePath);
        }
    }
}