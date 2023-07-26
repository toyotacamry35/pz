using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.JdbUpdater;
using Core.Environment.Logging.Extension;
using ResourcesSystem.Loader;
using EnumerableExtensions;
using L10n.Loaders;
using Newtonsoft.Json;
using NLog;

namespace L10n.KeysExtractionNs
{
    public class KeysExtractionService
    {
        public const string LocalizationConfigRelPath = "/locales/LocalizationsConfig";
        private const string OldInfoBegin = "OLD ";

        private static readonly Logger Logger = LogManager.GetLogger("Localization");

        private const string KeyBody = "l10n";
        private const int KeyIndexShiftValue = 500000;

        private JdbKeysExtractor _extractor;
        private List<string> _jdbRelPaths;

        /// <summary>
        /// Dictionary(cultureName, LocalizationDefMetadata)
        /// </summary>
        private Dictionary<string, LocalizationDefMetadata> _metas = new Dictionary<string, LocalizationDefMetadata>();

        private LocalizationDefMetadata _devMetadata;

        private long _maxKeyIndex = 0;
        private bool _hasKeyIndexShift;
        private GameResources _gameResources;


        //=== Props ===========================================================

        public string RepositoryPath { get; }

        public JsonSerializer SkipRefsSerializer { get; }


        //=== Ctor ============================================================

        public KeysExtractionService(string repositoryPath, List<string> jdbRelPaths, GameResources gameResources)
        {
            if (jdbRelPaths.AssertIfNull(nameof(jdbRelPaths)) ||
                gameResources.AssertIfNull(nameof(gameResources)))
                return;

            _gameResources = gameResources;
            SkipRefsSerializer = GetSkipRefsSerializer();
            RepositoryPath = repositoryPath;
            var locCfgDef = gameResources.LoadResource<LocalizationConfigDef>(LocalizationConfigRelPath);
            _extractor = new JdbKeysExtractor(_gameResources, this, locCfgDef.FilenamesForDebug);
            _jdbRelPaths = jdbRelPaths;
            var jdbLoader = new JdbLoader(_gameResources, locCfgDef.DomainFile, "/" + locCfgDef.LocalesDirName);

            var devFolderName = locCfgDef.DevCulture.Folder;
            _devMetadata = LocalizationDefMetadata.GetLocalizationDefMeta(devFolderName, jdbLoader, true);
            _metas[devFolderName] = _devMetadata;

            for (int i = 0; i < locCfgDef.LocalizationCultures.Length; i++)
            {
                var folderName = locCfgDef.LocalizationCultures[i].Folder;
                _metas[folderName] = LocalizationDefMetadata.GetLocalizationDefMeta(folderName, jdbLoader);
            }
        }


        //=== Public ==========================================================

        public bool IsKeyExists(string key)
        {
            return _devMetadata.LocalizationDef.Translations.ContainsKey(key);
        }

        public LocalizedString TakeEmbeddedTranslationDataAndReturnDataWithNewKey(LocalizedString embeddedLocalizedString, string comment = null)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                if (embeddedLocalizedString.TranslationData == null)
                {
                    embeddedLocalizedString = new LocalizedString(
                        embeddedLocalizedString.Key,
                        new TranslationData());
                }

                if (string.IsNullOrEmpty(embeddedLocalizedString.TranslationData.Comments))
                    embeddedLocalizedString.TranslationData.Comments = comment;
                else
                    embeddedLocalizedString.TranslationData.Comments += $", {comment}";
            }

            _devMetadata.IsChanged = true;
            var newKey = GetNewKey();
            var translationDataWithText = embeddedLocalizedString.CreateTranslationDataWithText();
            var isPlural = CalculatePluralFormsCount(translationDataWithText, _devMetadata.LocalizationDef.Translations) > 1;
            _devMetadata.LocalizationDef.Translations.Add(newKey, translationDataWithText.ToTranslationDataExt(isPlural));
            return new LocalizedString(newKey);
        }

        public Results RunOperation(JdbKeysExtractor.Operation operation)
        {
            var results = new Results()
            {
                AllMessages = new List<string>(),
                UsingLocalizedStrings = new List<LocalizedStringMetadata>(), //внутри LS - ключи
                NewLocalizedStrings = new List<LocalizedStringMetadata>(), //при извлечении внутри LS - ключи, при скане - тексты
                OrphanLocalizedStrings = new List<LocalizedStringMetadata>(), //внутри LS ключи
                DevTranslations = _devMetadata.LocalizationDef.Translations,
                GameResources = _gameResources
            };

            if (_extractor.AssertIfNull(nameof(_extractor)))
                return results;

            var startDateTime = DateTime.Now;
            List<string> processedWithErrorsPaths = new List<string>();

            string translationsHashcodeChangesInfos = "";

            //Проверка измененений текста
            foreach (var kvp in _metas)
            {
                if (IsTranslationsChangedByHashcode(
                    kvp.Value.LocalizationDef,
                    kvp.Value.IsDevelop,
                    IsOperationAffectsToExistingRecords(operation),
                    ref translationsHashcodeChangesInfos))
                    kvp.Value.IsChanged = true;
            }

            if (operation == JdbKeysExtractor.Operation.HashesRecalculation)
            {
                foreach (var kvp in _metas)
                {
                    HashesRecalculation(kvp.Value.LocalizationDef, ref translationsHashcodeChangesInfos);
                    kvp.Value.IsChanged = true;
                }
            }

            //--- Проверка словарей на соответствие версий
            string translationsVersionErrorInfos = "";
            if (operation == JdbKeysExtractor.Operation.ScanOnly ||
                operation == JdbKeysExtractor.Operation.KeysExtraction ||
                operation == JdbKeysExtractor.Operation.RecordVersionsAutofix)
            {
                //ScanOnly - сообщаем об ошибках
                //KeysExtraction если есть ошибки - запрещаем далее идти
                //RecordVersionsAutofix - фиксим ошибки
                var hadVersionErrors = false;
                foreach (var kvp in _metas)
                {
                    if (kvp.Value.IsDevelop)
                        continue;

                    if (IsTranslationsHadVersionErrors(
                        _devMetadata,
                        kvp.Value,
                        operation == JdbKeysExtractor.Operation.RecordVersionsAutofix,
                        ref translationsVersionErrorInfos))
                    {
                        kvp.Value.IsChanged = true;
                        hadVersionErrors = true;
                    }
                }

                if (operation == JdbKeysExtractor.Operation.KeysExtraction && hadVersionErrors)
                {
                    results.AllMessages.Add($"Следующие ошибки должны быть исправлены перед извлечением ключей!\n{translationsVersionErrorInfos}");
                    foreach (var message in results.AllMessages)
                        Logger.IfError()?.Message(message).Write();
                    return results;
                }
            }

            int jdbCount = 0;
            if (IsOperationAffectsToResources(operation))
            {
                foreach (var jdbRelPath in _jdbRelPaths)
                {
                    List<LocalizedStringMetadata> localizedStringsMetadata = null;
                    try
                    {
                        localizedStringsMetadata = _extractor.GetAllLocalizedStrings(jdbRelPath, operation, ref processedWithErrorsPaths);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()
                            ?.Message($"{nameof(RunOperation)}({operation}) {nameof(jdbRelPath)}='{jdbRelPath}' Exception: {e.Message}\n{e.StackTrace}")
                            .Write();
                    }

                    if ((localizedStringsMetadata?.Count ?? 0) > 0)
                    {
                        foreach (var localizedStringMetadata in localizedStringsMetadata)
                        {
                            var key = localizedStringMetadata.LocalizedString.Key;
                            if (key.AssertIfNull(nameof(key)))
                                continue;

                            if (localizedStringMetadata.IsNewKey)
                            {
                                results.NewLocalizedStrings.Add(localizedStringMetadata);
                            }
                            else
                            {
                                (IsKeyExists(key) ? results.UsingLocalizedStrings : results.OrphanLocalizedStrings).Add(localizedStringMetadata);
                            }
                        }
                    }

                    jdbCount++;
                }
            }

            string someOperationInfo = "";
            if (operation == JdbKeysExtractor.Operation.DeleteUnusedKeys)
            {
                var unusedKeys = GetUnusedKeys(_devMetadata.LocalizationDef.Translations, results.UsingLocalizedStrings, results.NewLocalizedStrings);
                if (unusedKeys.Count > 0)
                {
                    foreach (var meta in _metas.Values)
                    {
                        if (RemoveTranslationKeys(meta.LocalizationDef, unusedKeys))
                            meta.IsChanged = true;
                    }

                    someOperationInfo = $"Удалены неиспользуемые ключи: {unusedKeys.ItemsToString()}";
                }
                else
                {
                    someOperationInfo = "Не найдено неиспользуемых ключей";
                }
            }

            if (operation == JdbKeysExtractor.Operation.UpdateExtractionComments && results.UsingLocalizedStrings.Count > 0)
            {
                foreach (var kvp in _devMetadata.LocalizationDef.Translations)
                {
                    if (IsCommentUpdated(kvp.Value, results.UsingLocalizedStrings.Where(metadata => metadata.LocalizedString.Key == kvp.Key).ToArray()))
                    {
                         _devMetadata.IsChanged = true;
                         someOperationInfo = "В dev-словаре обновлены Comments-поля (использования ключей)";
                    }
                }
            }

            foreach (var meta in _metas.Values)
            {
                if (!IsOperationReadOnly(operation) && meta.IsChanged)
                {
                    if (!JdbRewriteTool.JdbUpdate(
                        SkipRefsSerializer,
                        RepositoryPath,
                        meta.RelPath,
                        meta.LocalizationDef,
                        false,
                        nameof(LocalizationDef.Translations)))
                        Logger.IfError()?.Message($"DevTranslations hasn't been overwritten! --- '{meta.RelPath}'").Write();
                }
            }

            //=== Reports time
            results.AllMessages.Add(GetSpentTimeInfo(operation, jdbCount, startDateTime));

            if (processedWithErrorsPaths.Count > 0)
                results.AllMessages.Add($"Нулевые записи или обработанные с ошибками: {processedWithErrorsPaths.ItemsToStringByLines()}");

            if (!string.IsNullOrEmpty(translationsHashcodeChangesInfos))
                results.AllMessages.Add(translationsHashcodeChangesInfos);

            if (!string.IsNullOrEmpty(translationsVersionErrorInfos))
                results.AllMessages.Add(translationsVersionErrorInfos);

            switch (operation)
            {
                case JdbKeysExtractor.Operation.ScanOnly:
                    results.AllMessages.AddRange(GetScanInfo(results.NewLocalizedStrings, results.UsingLocalizedStrings, results.OrphanLocalizedStrings));
                    break;

                case JdbKeysExtractor.Operation.KeysExtraction:
                    results.AllMessages.Add(GetExtractNewKeysInfo(results.NewLocalizedStrings));
                    break;

                case JdbKeysExtractor.Operation.DeleteUnusedKeys:
                case JdbKeysExtractor.Operation.UpdateExtractionComments:
                    if (!string.IsNullOrEmpty(someOperationInfo))
                        results.AllMessages.Add(someOperationInfo);
                    break;
            }

            foreach (var message in results.AllMessages)
                Logger.IfInfo()?.Message(message).Write();

            return results;
        }

        /// <summary>
        /// Вычисление наличия plural форм, основываясь на содержимом TranslationData.Text и ссылках в нем. НЕ учитывает свойства IsPlural
        /// </summary>
        public static int CalculatePluralFormsCount(TranslationData translationDataWithText, Dictionary<string, TranslationDataExt> existingTranslations)
        {
            var formsCount = translationDataWithText.GetForms_SelfOnly().Length;
            if (formsCount > 1) //либо сразу имеет несколько форм
                return formsCount;

            //либо имеет реф-ключи на TranslationDataExt.IsPlural+
            var referencedKeys = translationDataWithText.GetKeyReferencesFromText();
            if (referencedKeys.Length == 0)
                return 1;

            foreach (var referencedKey in referencedKeys)
            {
                if (string.IsNullOrEmpty(referencedKey))
                {
                    Logger.IfError()?.Message($"{nameof(referencedKey)} is empty -- {translationDataWithText}").Write();
                    continue;
                }

                if (!existingTranslations.TryGetValue(referencedKey, out var referencedTranslationDataExt))
                {
                    Logger.IfError()?.Message($"Not found {nameof(referencedTranslationDataExt)} for [{referencedKey}] -- {translationDataWithText}").Write();
                    continue;
                }

                formsCount = CalculatePluralFormsCount(referencedTranslationDataExt, existingTranslations);
                if (formsCount > 1)
                    return formsCount;
            }

            return 1;
        }

        public static bool IsOperationForExistingKeysOnly(JdbKeysExtractor.Operation operation)
        {
            switch (operation)
            {
                case JdbKeysExtractor.Operation.DeleteUnusedKeys:
                case JdbKeysExtractor.Operation.UpdateExtractionComments:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Нужно ли анализировать использование ключей в ресурсах проекта
        /// </summary>
        public static bool IsOperationAffectsToResources(JdbKeysExtractor.Operation operation)
        {
            switch (operation)
            {
                case JdbKeysExtractor.Operation.ScanOnly:
                case JdbKeysExtractor.Operation.KeysExtraction:
                case JdbKeysExtractor.Operation.DeleteUnusedKeys:
                case JdbKeysExtractor.Operation.UpdateExtractionComments:
                    return true;
            }

            return false;
        }

        public static JsonSerializer GetSkipRefsSerializer()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(UnityRefSkipConverter.Instance);
            serializer.Converters.Add(new EmbeddedOnlyDefRefConverter());
            return serializer;
        }


        //=== Private =========================================================

        private List<string> GetUnusedKeys(
            Dictionary<string, TranslationDataExt> allTranslations,
            List<LocalizedStringMetadata> usingTranslations,
            List<LocalizedStringMetadata> newTranslations)
        {
            var usingKeys = usingTranslations
                .Select(lsm => lsm.LocalizedString.Key)
                .Distinct()
                .ToList();

            var newKeys = newTranslations
                .Where(lsm => LocalizedString.IsKey(lsm.LocalizedString))
                .Select(lsm => lsm.LocalizedString.Key)
                .ToList();

            var keyReferences = allTranslations
                .Where(kvp => usingKeys.Contains(kvp.Key) || newKeys.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value.GetKeyReferencesFromText())
                .Union(
                    newTranslations
                        .Where(lsm => !LocalizedString.IsKey(lsm.LocalizedString))
                        .SelectMany(lsm => lsm.LocalizedString.CreateTranslationDataWithText().GetKeyReferencesFromText()))
                .Distinct()
                .ToList();

            return allTranslations
                .Where(
                    kvp => !kvp.Value.IsProtected &&
                           !usingKeys.Contains(kvp.Key) &&
                           !newKeys.Contains(kvp.Key) &&
                           !keyReferences.Contains(kvp.Key))
                .Select(e => e.Key)
                .ToList();
        }

        private bool IsOperationAffectsToExistingRecords(JdbKeysExtractor.Operation operation)
        {
            return operation == JdbKeysExtractor.Operation.KeysExtraction || operation == JdbKeysExtractor.Operation.RecordVersionsAutofix;
        }

        private bool IsOperationReadOnly(JdbKeysExtractor.Operation operation)
        {
            return operation == JdbKeysExtractor.Operation.ScanOnly;
        }

        private bool RemoveTranslationKeys(LocalizationDef localizationDef, List<string> removeKeys)
        {
            var isChanged = false;
            foreach (var removeKey in removeKeys)
            {
                if (localizationDef.Translations.ContainsKey(removeKey))
                {
                    localizationDef.Translations.Remove(removeKey);
                    isChanged = true;
                }
            }

            return isChanged;
        }

        private bool IsCommentUpdated(TranslationDataExt translationData, LocalizedStringMetadata[] suitableMetadatas)
        {
            bool isChanged;
            if (suitableMetadatas.Length == 0)
            {
                if (string.IsNullOrEmpty(translationData.Comments) || translationData.Comments.StartsWith(OldInfoBegin))
                    return false;

                translationData.Comments = OldInfoBegin + translationData.Comments;
                isChanged = true;
            }
            else
            {
                var newComments = suitableMetadatas
                    .Select(meta => LocalizedStringMetadata.GetComment(meta.JdbRelPath, meta.HierPath))
                    .ItemsToString(false, false, "; ", "", "", "", "");

                isChanged = newComments != translationData.Comments;
                if (isChanged)
                    translationData.Comments = newComments;
            }

            return isChanged;
        }

        private void HashesRecalculation(LocalizationDef localizationDef, ref string info)
        {
            if (localizationDef?.Translations == null)
            {
                Logger.IfError()?.Message($"{nameof(LocalizationDef.Translations)} is empty").Write();
                return;
            }

            var translations = localizationDef.Translations;
            foreach (var key in translations.Keys)
                translations[key].HashRecalc();
        }

        private bool IsTranslationsChangedByHashcode(LocalizationDef localizationDef, bool isDevLocalization, bool doChanges, ref string info)
        {
            if (localizationDef?.Translations == null)
            {
                Logger.IfError()?.Message($"{nameof(LocalizationDef.Translations)} is empty").Write();
                return true;
            }

            var translations = localizationDef.Translations;
            bool isChanged = false;
            var messages = new StringBuilder();
            foreach (var key in translations.Keys)
            {
                if (translations[key] == null)
                {
                    Logger.IfError()?.Message($"{localizationDef}: [{key}] data is null! Fixed").Write();
                    translations[key] = new TranslationDataExt();
                    isChanged = true;
                }

                var translationDataExt = translations[key];
                var textHashCode = translationDataExt.GetTextHashCode();
                if (translationDataExt.TextHashcode != textHashCode)
                {
                    translationDataExt.ChangeText(translationDataExt.Text, isDevLocalization);
                    messages.AppendLine(
                        $"{localizationDef}: Рехеширование {(isDevLocalization ? " и поднятие версий" : "")} для [{key}]={translationDataExt}");
                    isChanged = doChanges;
                }
            }

            if (messages.Length > 0)
                info += $"{(info.Length == 0 ? "" : "\n")}{localizationDef} {(doChanges ? "изменения" : "проверка")}:\n{messages}";
            return isChanged;
        }

        private bool IsTranslationsHadVersionErrors(
            LocalizationDefMetadata devMetadata,
            LocalizationDefMetadata translMetadata,
            bool doFixes,
            ref string versionErrorInfos)
        {
            bool hadErrors = false;
            var messages = new StringBuilder();
            foreach (var kvp in translMetadata.LocalizationDef.Translations)
            {
                var translVer = kvp.Value.Version;
                var devVersion = devMetadata.LocalizationDef.Translations.TryGetValue(kvp.Key, out var devTde) ? devTde.Version : 0;
                if (translVer > devVersion)
                {
                    hadErrors = true;
                    messages.AppendLine($"{kvp.Key}: version={translVer} > devVersion={devVersion}");
                    if (doFixes)
                        kvp.Value.Version = devVersion;
                }
            }

            if (messages.Length > 0)
                versionErrorInfos += $"{(versionErrorInfos.Length == 0 ? "" : "\n")}{translMetadata.LocalizationDef}: " +
                                     $"Ошибок версионности {(doFixes ? "исправлено" : "найдено")}:\n{messages}";

            return hadErrors;
        }

        private string GetNewKey()
        {
            if (_maxKeyIndex == 0)
                _maxKeyIndex = _devMetadata.LocalizationDef.Translations.Count;
            string key = "";
            while (true)
            {
                key = $"{LocalizedString.KeyBegin}{KeyBody}{_maxKeyIndex++}";
                if (!IsKeyExists(key))
                    break;

                if (!_hasKeyIndexShift)
                {
                    _maxKeyIndex += KeyIndexShiftValue; //сброс разницы между числом элементов и новым индексом при дальнейших запусках
                    _hasKeyIndexShift = true;
                }
            }

            return key;
        }

        private string GetSpentTimeInfo(JdbKeysExtractor.Operation operation, int jdbCount, DateTime operationStartDateTime)
        {
            var timeSpan = DateTime.Now - operationStartDateTime;
            var spentTimeInfo = timeSpan.Hours > 0 ? $"{timeSpan.Hours}h " : "";
            spentTimeInfo += $"{timeSpan.Minutes}m {timeSpan.Seconds}s";

            return $"{new string('-', 20)} Локализация. Операция {operation} завершена, обработано {jdbCount} jdb файлов, {spentTimeInfo}";
        }

        private List<string> GetScanInfo(
            List<LocalizedStringMetadata> newLocalizedStrings,
            List<LocalizedStringMetadata> usingLocalizedStrings,
            List<LocalizedStringMetadata> orphanLocalizedStrings)
        {
            var infos = new List<string>();
            var isSomethingFound = false;
            if (newLocalizedStrings.Count > 0)
            {
                isSomethingFound = true;
                infos.Add($"Найдены новые ключи: {newLocalizedStrings.ItemsToStringByLines()}");
            }

            var unusedTranslationsKeys = GetUnusedKeys(_devMetadata.LocalizationDef.Translations, usingLocalizedStrings, newLocalizedStrings);
            if (unusedTranslationsKeys.Count > 0)
            {
                isSomethingFound = true;
                infos.Add(
                    "Неиспользуемые ключи: " +
                    $"{unusedTranslationsKeys.Select(k => $"[{k}]={_devMetadata.LocalizationDef.Translations[k]}").ItemsToStringByLines()}");
            }

            if (orphanLocalizedStrings.Count > 0)
            {
                isSomethingFound = true;
                infos.Add($"Найдены потерянные ключи (ссылки есть, но в базе нет): {orphanLocalizedStrings.ItemsToStringByLines()}");
            }

            if (!isSomethingFound)
                infos.Add("Все в порядке");

            return infos;
        }

        private string GetExtractNewKeysInfo(List<LocalizedStringMetadata> newLocalizedStrings)
        {
            return newLocalizedStrings.Count > 0
                ? $"Найдены новые LocalizedStrings: {newLocalizedStrings.ItemsToStringByLines()}"
                : "Не найдено новых LocalizedStrings";
        }


        //=== Struct ==================================================================================================

        public struct Results
        {
            public List<string> AllMessages;
            public List<LocalizedStringMetadata> NewLocalizedStrings;
            public List<LocalizedStringMetadata> UsingLocalizedStrings;
            public List<LocalizedStringMetadata> OrphanLocalizedStrings;
            public Dictionary<string, TranslationDataExt> DevTranslations;
            public GameResources GameResources;
        }
    }
}