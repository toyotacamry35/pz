using System.Globalization;
using System.Linq;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n.Loaders;
using SharedCode.Logging;
using UnityEngine;

namespace L10n
{
    public class LocalizationInit : MonoBehaviour
    {
        private const string CurrentLocalizationNameKey = "CurrentLocalizationName";

        [SerializeField, UsedImplicitly]
        private LocalizationConfigDefRef _localizationConfigDefRef;


        //=== Props ===========================================================

        public static string CurrentLocalizationCode
        {
            get => UniquePlayerPrefs.GetString(CurrentLocalizationNameKey, CultureInfo.CurrentCulture.Name);
            set
            {
                if (CurrentLocalizationCode != value)
                {
                    UniquePlayerPrefs.SetString(CurrentLocalizationNameKey, value);
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Log.StartupStopwatch.Milestone("Localization Awake start");
            var configDef = _localizationConfigDefRef?.Target;
            if (configDef.AssertIfNull(nameof(configDef)))
                return;

            //var loader = new JsonTextCatalogLoader(configDef.DomainFiles, Path.Combine(Application.dataPath, configDef.LocalesDirName));
            var loader = new JdbLoader(GameResourcesHolder.Instance, configDef.DomainFile, "/" + configDef.LocalesDirName);
            L10nHolder.Instance = new LocalizationSource(
                configDef.DevCulture, 
                configDef.LocalizationCultures,
                configDef.LocalizationCultures[configDef.DefaultLocalizationIndex].Code,
                loader,
                CurrentLocalizationCode);
            L10nHolder.Instance.LocalizationChanged += OnLocalizationChanged;
            Log.StartupStopwatch.Milestone("Localization Awake DONE!");
        }


        //=== Private =========================================================

        private void OnLocalizationChanged(string localizationName)
        {
            CurrentLocalizationCode = localizationName;
        }
    }
}