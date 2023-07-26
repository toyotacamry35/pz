using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using L10n.Loaders;
using NLog;

namespace L10n
{
    /// <summary>
    /// Ресурсы (тексты, спрайты и пр.), касающиеся всех аспектов локализации
    /// </summary>
    public class CultureLocalization
    {
        private static readonly Logger Logger = LogManager.GetLogger("UI");

        private static Dictionary<string, CultureLocalization> _localizationsByFolder = new Dictionary<string, CultureLocalization>();
        private static ILoader _loader;
        private static CultureData[] _allCultureDataIncludeDev;


        //=== Props ===========================================================

        public CultureData CultureData { get; }

        public CultureData FallbackCultureData { get; }

        public TextCatalog TextCatalog { get; }

        public static bool IsInited { get; private set; }


        //=== Ctor ============================================================

        public static void Init(ILoader loader, CultureData[] allCultureDataIncludeDev)
        {
            if (IsInited)
                return;

            _loader = loader;
            _allCultureDataIncludeDev = allCultureDataIncludeDev;
            _localizationsByFolder.Clear();
            if (_loader.AssertIfNull(nameof(_loader)) ||
                _allCultureDataIncludeDev.AssertIfNull(nameof(_allCultureDataIncludeDev)))
                return;

            IsInited = true;
        }

        public CultureLocalization(CultureData cultureData)
        {
            Logger.IfInfo()?.Message($"CL_start: by {cultureData}").Write(); //2del
            CultureData = cultureData;
            TextCatalog = new TextCatalog(cultureData, _loader);

            //Добавление Fallback-культуры
            if (string.IsNullOrEmpty(cultureData.FallbackCultureFolder))
                return;

            if (cultureData.FallbackCultureFolder == cultureData.Folder)
            {
                Logger.IfError()
                    ?.Message($"Wrong {nameof(cultureData)} {nameof(cultureData.FallbackCultureFolder)}=={nameof(cultureData.Folder)}: {cultureData}")
                    .Write();
                return;
            }

            FallbackCultureData = _allCultureDataIncludeDev.FirstOrDefault(data => data.Folder == cultureData.FallbackCultureFolder);
            if (FallbackCultureData.Equals(default(CultureData)))
            {
                Logger.IfError()
                    ?.Message(
                        $"Unable to find {nameof(FallbackCultureData)} by {nameof(cultureData.FallbackCultureFolder)}={cultureData.FallbackCultureFolder}")
                    .Write();
                return;
            }

            var fallbackCultureLocalization = GetOrAddLocalization(FallbackCultureData);
            TextCatalog.SetFallbackTextCatalog(fallbackCultureLocalization?.TextCatalog);
            Logger.IfInfo()?.Message($"CL_end: {this}").Write(); //2del
        }


        //=== Public ==========================================================

        public static CultureLocalization GetLocalization(string cultureCode)
        {
            Logger.IfInfo()?.Message($"GetLocalization(cultureCode={cultureCode})").Write(); //2del
            if (!IsInited)
            {
                Logger.IfError()?.Message($"Attempt to get localization to '{cultureCode}' without initialization").Write();
                return null;
            }

            var selectedCulture = _allCultureDataIncludeDev.FirstOrDefault(data => data.Code == cultureCode && !data.IsDev);

            if (selectedCulture.Equals(default))
            {
                Logger.IfError()?.Message($"Unable to find {nameof(selectedCulture)} by {nameof(cultureCode)} '{cultureCode}'").Write();
                return null;
            }

            return GetOrAddLocalization(selectedCulture);
        }

        public override string ToString()
        {
            return $"({nameof(CultureLocalization)}: {nameof(CultureData)}={CultureData}, {nameof(FallbackCultureData)}={FallbackCultureData}, " +
                   $"{nameof(TextCatalog)}={TextCatalog} /CL)";
        }


        //=== Private ==============================================================

        private static CultureLocalization GetOrAddLocalization(CultureData cultureData)
        {
            var key = cultureData.Folder;
            if (!_localizationsByFolder.TryGetValue(key, out var cultureLocalization))
            {
                cultureLocalization = new CultureLocalization(cultureData);
                _localizationsByFolder.Add(key, cultureLocalization);
            }

            return cultureLocalization;
        }
    }
}