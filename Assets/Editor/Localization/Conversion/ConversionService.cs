//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Assets.Src.ResourcesSystem.Base;
//using Assets.Src.ResourcesSystem.JdbUpdater;
//using ResourcesSystem.Loader;
//using EnumerableExtensions;
//using L10n.KeysExtractionNs;
//using L10n.Loaders;
//using Newtonsoft.Json;
//using NLog;
//
//namespace L10n.ConversionNs
//{
//    public class ConversionService
//    {
//        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");
//
//        private LocalizationConfigDef _locCfgDef;
//        private List<string> _allJdbRelPaths;
//        private GameResources _gameResources;
//        private JdbPropsConverter _converter;
//        private LocalizationDef _devLocalization;
//        private string _devLocalizationRelPath;
//        private bool _hasChangesForWrite;
//        private JdbLoader _jdbLoader;
//
//        private Dictionary<string, ConversionResourceData> _allLoadedResources = new Dictionary<string, ConversionResourceData>();
//        private Dictionary<string, ConversionResourceData> _allBrokenResources = new Dictionary<string, ConversionResourceData>();
//
//        private DateTime _startDateTime;
//
//
//        //=== Props ===========================================================
//
//        public JsonSerializer SkipRefsSerializer { get; }
//
//        public string RepositoryPath { get; }
//
//        public Type CurrentDefType { get; private set; }
//
//        public Type[] UsedBaseResourceTypes { get; private set; }
//
//        public int LoadedResourcesCount { get; private set; }
//
//        public int LoadedResourcesMaxCount { get; private set; } = 1;
//
//        public bool IsBrokenResourcesShown { get; private set; }
//
//        public Dictionary<string, ConversionResourceData> ShownResources { get; private set; } =
//            new Dictionary<string, ConversionResourceData>();
//
//
//        //=== Ctor ============================================================
//
//        public ConversionService(string repositoryPath, LocalizationConfigDef locCfgDef, List<string> allJdbRelPaths,
//            GameResources gameResources)
//        {
//            if (locCfgDef.AssertIfNull(nameof(locCfgDef)) ||
//                allJdbRelPaths.AssertIfNull(nameof(allJdbRelPaths)) ||
//                gameResources.AssertIfNull(nameof(gameResources)))
//                return;
//
//            SkipRefsSerializer = KeysExtractionService.GetSkipRefsSerializer();
//            _gameResources = gameResources;
//            RepositoryPath = repositoryPath;
//            _converter = new JdbPropsConverter(gameResources, this);
//            _allJdbRelPaths = allJdbRelPaths;
//            _locCfgDef = locCfgDef;
//            _jdbLoader = new JdbLoader(_gameResources, _locCfgDef.DomainFile, "/" + _locCfgDef.LocalesDirName);
//            _devLocalization = _jdbLoader.GetLocalizationDef(_locCfgDef.DevLocalizationName, out _devLocalizationRelPath);
//            var assembly = typeof(BaseResource).Assembly;
//            UsedBaseResourceTypes = assembly.GetTypes().Where(t => typeof(BaseResource).IsAssignableFrom(t)).ToArray();
//        }
//
//
//        //=== Public ==========================================================
//
//        public void SetCurrentDefType(string defTypeAsString)
//        {
//            CurrentDefType = GetDefType(defTypeAsString);
//        }
//
//        public void ExcludeUnusedTypes(bool isAllTypes)
//        {
//            List<Type> usedTypes = new List<Type>();
//            foreach (var type in UsedBaseResourceTypes) //исключаем типы, ресурсов которых нет в проекте
//            {
//                var customAttributes = type.GetCustomAttributes(typeof(LocalizedAttribute), false);
//                if (customAttributes.Length == 0)
//                    continue;
//
//                UpdateResourcesByType(type, false, true);
//                if (ShownResources.Count > 0)
//                    usedTypes.Add(type);
//            }
//
//            usedTypes.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));
//            UsedBaseResourceTypes = usedTypes.ToArray();
//            //UI.Logger.IfInfo()?.Message($"============ _allLoadedResources {UsedBaseResourceTypes.Select(t => t.Name).ItemsToString()}").Write(); //DEBUG
//            UpdateResourcesByType(CurrentDefType, isAllTypes);
//        }
//
//        public IEnumerator GetAllResourcesEnumerator()
//        {
//            _allLoadedResources.Clear();
//
//            if (_allJdbRelPaths != null)
//            {
//                LoadedResourcesMaxCount = _allJdbRelPaths.Count;
//                LoadedResourcesCount = 0;
//                foreach (var relPath in _allJdbRelPaths)
//                {
//                    var res = _gameResources.LoadResource<IResource>(relPath);
//                    LoadedResourcesCount++;
//                    if (res != null)
//                        _allLoadedResources.Add(relPath, new ConversionResourceData(relPath, _gameResources));
//                    else
//                        _allBrokenResources.Add(relPath, new ConversionResourceData(relPath, _gameResources));
//
//                    yield return null;
//                }
//            }
//        }
//
//        public void UpdateResourcesByType(Type type, bool isAllTypes, bool speedMode = false)
//        {
//            ShownResources.Clear();
//            IsBrokenResourcesShown = type == null;
//            if (type != null)
//            {
//                foreach (var kvp in _allLoadedResources)
//                {
//                    var resourceData = kvp.Value;
//                    if (resourceData?.BaseResource == null)
//                        continue;
//
//                    if (isAllTypes || resourceData.BaseResource.GetType() == type)
//                    {
//                        ShownResources.Add(kvp.Key, kvp.Value);
//                        if (speedMode)
//                            break;
//
//                        kvp.Value.UpdateUnconvertedProps(_converter.ScanOrConvertData(kvp.Key, false));
//                    }
//                }
//            }
//            else
//            {
//                ShownResources = _allBrokenResources.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
//            }
//        }
//
//        public void RunScanOrConversion(string[] relPaths, bool doConversion = false)
//        {
//            if (_converter.AssertIfNull(nameof(_converter)) ||
//                relPaths.AssertIfNull(nameof(relPaths)))
//                return;
//
//            _startDateTime = DateTime.Now;
//
//            int jdbCount = 0;
//            for (int i = 0; i < relPaths.Length; i++)
//            {
//                var relPath = relPaths[i];
//                if (string.IsNullOrEmpty(relPath))
//                {
//                    Logger.IfError()?.Message($"{nameof(relPath)} [{i}] is null or empty").Write();
//                    continue;
//                }
//
//                var conversionMetadataList = _converter.ScanOrConvertData(relPath, doConversion);
//                if (!_allLoadedResources.TryGetValue(relPath, out var conversionResourceData))
//                {
//                    Logger.IfError()?.Message($"Unable to get resource by {nameof(relPath)} '{relPath}'").Write();
//                    return;
//                }
//
//                conversionResourceData.UpdateUnconvertedProps(conversionMetadataList);
//                jdbCount++;
//            }
//
//            if (doConversion && _hasChangesForWrite)
//            {
//                if (!JdbRewriteTool.JdbUpdate(
//                    SkipRefsSerializer,
//                    RepositoryPath,
//                    _devLocalizationRelPath,
//                    _devLocalization,
//                    false,
//                    nameof(LocalizationDef.Translations)))
//                    Logger.IfError()?.Message($"DevTranslations hasn't been overwritten! --- '{_devLocalizationRelPath}'").Write();
//            }
//
//            DoReport(relPaths, doConversion, jdbCount);
//        }
//
//        public void SetUnconvertedTextByKey(string localizedStringKey, string unconvertedText)
//        {
//            if (!_devLocalization.Translations.TryGetValue(localizedStringKey, out var translationDataExt))
//            {
//                Logger.Error($"{nameof(SetUnconvertedTextByKey)}({nameof(localizedStringKey)}='{localizedStringKey}', " +
//                             $"{nameof(unconvertedText)}='{unconvertedText}') Key not found in {nameof(_devLocalization.Translations)}");
//                return;
//            }
//
//            if ((translationDataExt.Forms?.Length ?? 0) < 1 ||
//                translationDataExt.Forms[0] != unconvertedText)
//            {
//                translationDataExt.ChangeText(unconvertedText, true);
//                _hasChangesForWrite = true;
//            }
//        }
//
//
//        //=== Private =========================================================
//
//        private void DoReport(string[] relPaths, bool doConversion, int jdbCount)
//        {
//            var timeSpan = DateTime.Now - _startDateTime;
//            var spentTimeInfo = "";
//            if (timeSpan.Hours > 0)
//                spentTimeInfo = $"{timeSpan.Hours}h ";
//            spentTimeInfo += $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
//
//            Logger.Info($"Unconverted text {(doConversion ? "conversion" : "scanning")} " +
//                        $"finished. Passed jdb-files: {relPaths.ItemsToString()}, converted: {jdbCount}, {spentTimeInfo}");
//        }
//
//        private Type GetDefType(string typeStr)
//        {
//            if (typeStr == L10nConversionWindow.AllTypesName)
//                return typeof(BaseResource); //важно, что не null
//
//            return UsedBaseResourceTypes.FirstOrDefault(t => t.Name == typeStr);
//        }
//    }
//}