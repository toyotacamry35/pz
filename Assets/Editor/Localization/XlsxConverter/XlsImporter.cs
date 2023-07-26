using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.ResourcesSystem.JdbUpdater;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ResourcesSystem.Loader;
using L10n.KeysExtractionNs;
using L10n.Loaders;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using Uins;
using UnityEditor;
using UnityEngine;

namespace L10n.XlsConverterNs
{
    public class XlsImporter
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");

        private const int MaxSkippedEmptyColumns = 5;
        private const int MaxSkippedEmptyRows = 50;

        private const int ColumnIdsRowIndex = 1;
        private const int ImportOptionsRowIndex = ColumnIdsRowIndex + 1;
        private const int HeadersRowIndex = ImportOptionsRowIndex + 1;
        private const int ContentFirstRowIndex = HeadersRowIndex + 1;

        private string _repositoryPath;
        private GameResources _gameResources;


        //=== Ctor ============================================================

        public XlsImporter(string repositoryPath, GameResources gameResources)
        {
            if (string.IsNullOrEmpty(repositoryPath) ||
                gameResources.AssertIfNull(nameof(gameResources)))
            {
                ErrorToLogger(XlsExporter.Err.CtorWrongParams);
                return;
            }

            _repositoryPath = repositoryPath;
            _gameResources = gameResources;
        }


        //=== Public ==========================================================

        public void Import(string importFilePath)
        {
            var localizationConfig = _gameResources.LoadResource<LocalizationConfigDef>(KeysExtractionService.LocalizationConfigRelPath);
            var backgroundColors = localizationConfig.XlsExportConfig.Target.GetBackgroundColors();
            if (backgroundColors.AssertIfNull(nameof(backgroundColors)))
                return;

            if (string.IsNullOrEmpty(importFilePath) || !File.Exists(importFilePath))
            {
                ErrorToLogger(XlsExporter.Err.XlsFileNotExists, importFilePath);
                return;
            }

            var importFileInfo = new FileInfo(importFilePath);
            var xlsExportConfigDef = localizationConfig.XlsExportConfig.Target;
            if (!XlsExporter.IsXlsExportConfigValid(xlsExportConfigDef))
            {
                ErrorToLogger(XlsExporter.Err.XlsExportConfigIsntValid, KeysExtractionService.LocalizationConfigRelPath);
                return;
            }

            using (var excelPackage = new ExcelPackage(importFileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[xlsExportConfigDef.WorksheetName];
                if (worksheet == null)
                    worksheet = excelPackage.Workbook.Worksheets[1];
                if (!worksheet.AssertIfNull(nameof(worksheet)))
                {
                    try
                    {
                        excelPackage.Save();
                    }
                    catch (Exception)
                    {
                        ErrorToLogger(XlsExporter.Err.XlsFileSaveError, importFileInfo.FullName);
                        return;
                    }

                    if (!ReadColumnTypes(worksheet, out var columnsData))
                        return;

                    if (!XlsExporter.IsColumnsValid(columnsData.Keys.ToList())) //повторяемость уже проверили, проверяем наличие обязательных столбцов
                        return;

                    if (!TryReadCulture(
                            worksheet,
                            HeadersRowIndex,
                            columnsData,
                            localizationConfig,
                            XlsColumnType.OrgText,
                            false,
                            out var orgCulture) ||
                        !TryReadCulture(
                            worksheet,
                            HeadersRowIndex,
                            columnsData,
                            localizationConfig,
                            XlsColumnType.DstNewText,
                            false,
                            out var dstCulture))
                        return;

                    var hasDst2Culture =
                        TryReadCulture(
                            worksheet,
                            HeadersRowIndex,
                            columnsData,
                            localizationConfig,
                            XlsColumnType.Dst2NewText,
                            false,
                            out var dst2Culture);

                    InfoToLogger("Импорт из '{0}' с листа '{1}'", importFilePath, worksheet.Name);

                    var jdbLoader = new JdbLoader(_gameResources, localizationConfig.DomainFile, "/" + localizationConfig.LocalesDirName);

                    var devTextCatalog = new TextCatalog(localizationConfig.DevCulture, jdbLoader);
                    var orgTextCatalog = orgCulture.Equals(devTextCatalog.CultureData)
                        ? devTextCatalog
                        : new TextCatalog(orgCulture, jdbLoader);
                    var dstTextCatalog = new TextCatalog(dstCulture, jdbLoader);
                    var dst2TextCatalog = hasDst2Culture ? new TextCatalog(dst2Culture, jdbLoader) : null;

                    if (orgTextCatalog.AssertIfNull(nameof(orgTextCatalog)) ||
                        dstTextCatalog.AssertIfNull(nameof(dstTextCatalog)) ||
                        hasDst2Culture && dst2TextCatalog.AssertIfNull(nameof(dst2TextCatalog)))
                        return;

                    //Чтобы не гонять в for по словарю
                    var keyData = columnsData[XlsColumnType.Key];
                    var versionData = columnsData[XlsColumnType.Version];
                    var pluralData = columnsData[XlsColumnType.IsPlural];
                    var orgTextData = columnsData[XlsColumnType.OrgText];
                    var dstNewTextData = columnsData[XlsColumnType.DstNewText];
                    var dst2NewTextData = hasDst2Culture ? columnsData[XlsColumnType.Dst2NewText] : null;
                    var commentsData = columnsData[XlsColumnType.Comments];
                    var errorsData = columnsData[XlsColumnType.Errors];

                    for (int rowIdx = ContentFirstRowIndex, emptyCellsInRow = 0; emptyCellsInRow <= MaxSkippedEmptyRows; rowIdx++)
                    {
                        var keyStr = GetString(worksheet, rowIdx, keyData.ColumnIndex);
                        if (string.IsNullOrEmpty(keyStr)) //если нет ключа, считаем строку пустой и пропускаем
                        {
                            emptyCellsInRow++;
                            continue;
                        }

                        emptyCellsInRow = 0;
                        ClearCellText(worksheet, rowIdx, errorsData.ColumnIndex);
                        foreach (var kvp in columnsData)
                            XlsExporter.ClearStyleBackgroundColor(worksheet.Cells[rowIdx, kvp.Value.ColumnIndex].Style);

                        var importTranslationLine = new ImportTranslationLine()
                        {
                            Key = keyStr,
                            Version = GetInt(worksheet, rowIdx, versionData.ColumnIndex),
                            IsPlural = GetBool(worksheet, rowIdx, pluralData.ColumnIndex),
                            OrgText = new ColumnValue(GetString(worksheet, rowIdx, orgTextData.ColumnIndex), orgTextData),
                            DstNewText = new ColumnValue(GetString(worksheet, rowIdx, dstNewTextData.ColumnIndex), dstNewTextData),
                            Dst2NewText = hasDst2Culture
                                ? new ColumnValue(GetString(worksheet, rowIdx, dst2NewTextData.ColumnIndex), dst2NewTextData)
                                : null,
                            Comments = new ColumnValue(GetString(worksheet, rowIdx, commentsData.ColumnIndex), commentsData)
                        };

                        TranslationLineValidateAndImport(importTranslationLine, devTextCatalog, orgTextCatalog, dstTextCatalog, dst2TextCatalog);

                        TableLineHighlighting(importTranslationLine, worksheet, rowIdx, columnsData, backgroundColors);
                    }

                    var skipRefsSerializer = KeysExtractionService.GetSkipRefsSerializer();
                    SaveTextCatalogChanges(skipRefsSerializer, _repositoryPath, localizationConfig, devTextCatalog);
                    if (devTextCatalog != orgTextCatalog)
                        SaveTextCatalogChanges(skipRefsSerializer, _repositoryPath, localizationConfig, orgTextCatalog);
                    SaveTextCatalogChanges(skipRefsSerializer, _repositoryPath, localizationConfig, dstTextCatalog);
                    if (hasDst2Culture)
                        SaveTextCatalogChanges(skipRefsSerializer, _repositoryPath, localizationConfig, dst2TextCatalog);

                    excelPackage.Save();
                    InfoToLogger("Файл '{0}' сохранен", importFileInfo.FullName);
                    if (!devTextCatalog.IsChanged &&
                        !orgTextCatalog.IsChanged &&
                        !dstTextCatalog.IsChanged &&
                        !(dst2TextCatalog?.IsChanged ?? false))
                    {
                        InfoToLogger("Ничего не импортировано из файла '{0}'", importFileInfo.FullName);
                    }

                    EditorUtility.RevealInFinder(importFileInfo.FullName);
                }
            }
        }


        //=== Private =========================================================

        private static void InfoToLogger(string format, params object[] args)
        {
            UI.InfoOrDebug(Logger, true, format, args);
        }

        private static void ErrorToLogger(XlsExporter.Err error, params object[] args)
        {
            UI.ErrorOrWarn(Logger, true, XlsExporter.GetErrorNumberAndText(error), args);
        }

        private static void WarnToLogger(XlsExporter.Err error, params object[] args)
        {
            UI.ErrorOrWarn(Logger, false, XlsExporter.GetErrorNumberAndText(error), args);
        }

        private void SaveTextCatalogChanges(
            JsonSerializer skipRefsSerializer,
            string repositoryPath,
            LocalizationConfigDef localizationConfig,
            TextCatalog textCatalog)
        {
            if (!textCatalog.IsChanged)
                return;

            var localizationDefRelPath = $"/{localizationConfig.LocalesDirName}/{textCatalog.CultureData.Folder}/{localizationConfig.DomainFile}";
            try
            {
                var localizationDef = _gameResources.LoadResource<LocalizationDef>(localizationDefRelPath);
                localizationDef.Translations = textCatalog.Translations;

                if (JdbRewriteTool.JdbUpdate(
                    skipRefsSerializer,
                    repositoryPath,
                    localizationDefRelPath,
                    localizationDef,
                    false,
                    nameof(LocalizationDef.Translations)))
                    InfoToLogger("Файл локализации '{0}' перезаписан", localizationDefRelPath);
                else
                    ErrorToLogger(XlsExporter.Err.LocalizationDefSaveError, localizationDefRelPath);
            }
            catch (Exception e)
            {
                ErrorToLogger(XlsExporter.Err.LocalizationDefSaveException, localizationDefRelPath, e);
            }
        }

        private bool ReadColumnTypes(ExcelWorksheet worksheet, out IDictionary<XlsColumnType, ColumnImportData> columnsImportData)
        {
            columnsImportData = new Dictionary<XlsColumnType, ColumnImportData>();
            for (int columnIndex = 1, emptyCellsInRow = 0; emptyCellsInRow <= MaxSkippedEmptyColumns; columnIndex++)
            {
                var columnTypeString = GetString(worksheet, ColumnIdsRowIndex, columnIndex);
                if (string.IsNullOrEmpty(columnTypeString) || !Enum.TryParse<XlsColumnType>(columnTypeString, false, out var columnType))
                {
                    emptyCellsInRow++;
                    continue;
                }

                if (columnsImportData.ContainsKey(columnType))
                {
                    ErrorToLogger(XlsExporter.Err.ImpColumnAlreadyExists, columnType);
                    return false;
                }

                emptyCellsInRow = 0;
                var importOptionString = GetString(worksheet, ImportOptionsRowIndex, columnIndex);
                if (!Enum.TryParse<ColumnImportOption>(importOptionString, false, out var columnImportOption))
                    columnImportOption = ColumnImportOption.Ignore;

                var ciData = new ColumnImportData()
                {
                    ColumnType = columnType,
                    ColumnIndex = columnIndex,
                    ImportOption = columnImportOption
                };
                columnsImportData.Add(columnType, ciData);
            }

            return true;
        }

        /// <summary>
        /// Возвращает удалось ли без ошибок прочесть сultureData, которая будет:
        /// - либо LocalizationConfigDef.DevCulture,
        /// - либо из списка LocalizationConfigDef.LocalizationCultures.
        /// При isOptional допускается отсутствие столбца в листе: в этом случае результат положительный, но сultureData будет default
        /// </summary>
        private bool TryReadCulture(
            ExcelWorksheet worksheet,
            int rowIndex,
            IDictionary<XlsColumnType, ColumnImportData> columnsImportData,
            LocalizationConfigDef localizationConfig,
            XlsColumnType columnType,
            bool isOptional,
            out CultureData cultureData)
        {
            cultureData = default;
            if (!columnsImportData.TryGetValue(columnType, out var ciData))
                return isOptional; //столбца может не быть на листе, если он опциональный

            var columnIndex = ciData.ColumnIndex;
            string languageHeaderString = GetString(worksheet, rowIndex, columnIndex);
            if (string.IsNullOrEmpty(languageHeaderString)) //ячейка пуста, а ожидался заголовок
            {
                ErrorToLogger(XlsExporter.Err.ImpCultureCodeMissing, columnType, rowIndex, columnIndex);
                return false;
            }

            if (languageHeaderString.IndexOf(localizationConfig.DevCulture.Description, StringComparison.Ordinal) == 0)
            {
                cultureData = localizationConfig.DevCulture;
                return true;
            }

            for (int i = 0; i < localizationConfig.LocalizationCultures.Length; i++)
            {
                var cd = localizationConfig.LocalizationCultures[i];
                if (languageHeaderString.IndexOf(cd.Description, StringComparison.Ordinal) != 0)
                    continue;

                cultureData = cd;
                return true;
            }

            ErrorToLogger(XlsExporter.Err.ImpCultureCodeMissingInString, columnType, rowIndex, columnIndex, languageHeaderString);
            return false;
        }

        private void TranslationLineValidateAndImport(
            ImportTranslationLine translationLine,
            TextCatalog devTextCatalog,
            TextCatalog orgTextCatalog,
            TextCatalog dstTextCatalog,
            TextCatalog dst2TextCatalog)
        {
            //--- Key
            if (!devTextCatalog.Translations.TryGetValue(translationLine.Key, out var devTranslationData))
            {
                WriteToLoggerAndTranslationLine(
                    false,
                    XlsExporter.Err.ImpDictionaryNotContainsKey,
                    XlsColumnType.Key,
                    translationLine,
                    devTextCatalog,
                    translationLine.Key);
                return; //далее обработка бессмысленна
            }

            //--- Version
            if (translationLine.Version < 0)
            {
                WriteToLoggerAndTranslationLine(
                    true,
                    XlsExporter.Err.ImpIncorrectValue,
                    XlsColumnType.Version,
                    translationLine,
                    null,
                    nameof(translationLine.Version),
                    translationLine.Version);
                return;
            }

            //--- Plural
            if (devTranslationData.IsPlural != translationLine.IsPlural)
            {
                WriteToLoggerAndTranslationLine(
                    true,
                    XlsExporter.Err.DiffIsPlural,
                    XlsColumnType.IsPlural,
                    translationLine,
                    devTextCatalog,
                    nameof(translationLine.IsPlural),
                    devTranslationData.IsPlural);
                return;
            }

            //-- Обработка OrgText
            ColumnValueValidateAndImport(XlsColumnType.Comments, translationLine, devTextCatalog, devTextCatalog);
            //Важно! Комментарии импортируем первыми, не поднимая версии в devTextCatalog
            ColumnValueValidateAndImport(XlsColumnType.OrgText, translationLine, orgTextCatalog, devTextCatalog);
            ColumnValueValidateAndImport(XlsColumnType.DstNewText, translationLine, dstTextCatalog, devTextCatalog);
            if (dst2TextCatalog != null)
                ColumnValueValidateAndImport(XlsColumnType.Dst2NewText, translationLine, dst2TextCatalog, devTextCatalog);
        }


        /// <summary>
        /// Импорт текстового значения, извлеченного из Excel-файла: оригинального текста, перевода или комментария
        /// </summary>
        /// <param name="columnType">Id столбца</param>
        /// <param name="translationLine">Извлеченная </param>
        /// <param name="textCatalog">textCatalog, в который предполагается делать импорт</param>
        /// <param name="devTextCatalog">textCatalog с которым делаются сравнения по числу аргументов и подстановок</param>
        private void ColumnValueValidateAndImport(
            XlsColumnType columnType,
            ImportTranslationLine translationLine,
            TextCatalog textCatalog,
            TextCatalog devTextCatalog)
        {
            var value = translationLine.GetColumnValue(columnType);
            if (value == null)
            {
                ErrorToLogger(XlsExporter.Err.ImpColumnValueIsNull, nameof(value), columnType, translationLine, textCatalog);
                return;
            }

            if (value.ColumnImportData.ImportOption == ColumnImportOption.Ignore)
                return;

            var isComments = columnType == XlsColumnType.Comments;
            if (!isComments)
            {
                if (string.IsNullOrEmpty(value.Text)) //пустую строку рассматриваем как ошибку
                {
                    WriteToLoggerAndTranslationLine(false, XlsExporter.Err.EmptyString, value.ColumnImportData.ColumnType, translationLine, textCatalog);
                    return;
                }

                if (value.Text.Trim() == "")
                    WriteToLoggerAndTranslationLine(false, XlsExporter.Err.WhitespaceString, value.ColumnImportData.ColumnType, translationLine, textCatalog);

                var testTranslationData = new TranslationData()
                {
                    IsPlural = translationLine.IsPlural,
                    Text = value.Text
                };

                var devTranslationData = devTextCatalog.Translations[translationLine.Key];

                //Не напутали ли с числом аргументов
                var newArgsLength = testTranslationData.GetArgReferencesCountFromText();
                if (newArgsLength < 0)
                {
                    WriteToLoggerAndTranslationLine(
                        true,
                        XlsExporter.Err.ArgsCountInconsistency,
                        columnType,
                        translationLine,
                        devTextCatalog);
                    return;
                }

                var devArgsLength = devTranslationData.GetArgReferencesCountFromText();
                if (devArgsLength >= 0 && newArgsLength != devArgsLength)
                {
                    WriteToLoggerAndTranslationLine(
                        true,
                        XlsExporter.Err.ImpDiffArgsCount,
                        columnType,
                        translationLine,
                        devTextCatalog,
                        newArgsLength,
                        devArgsLength);
                    return;
                }

                //Не потеряли ли подстановки ключей при переводе
                var newKeyRefsLength = testTranslationData.GetKeyReferencesFromText().Length;
                var devKeyRefsLength = devTranslationData.GetKeyReferencesFromText().Length;
                if (newKeyRefsLength != devKeyRefsLength)
                {
                    WriteToLoggerAndTranslationLine(
                        true,
                        XlsExporter.Err.ImpDiffKeySubstCount,
                        columnType,
                        translationLine,
                        devTextCatalog,
                        newKeyRefsLength,
                        devKeyRefsLength);
                    return;
                }

                //Проверки плюральности
                var selfFormsLength = testTranslationData.GetForms_SelfOnly().Length;
                if (translationLine.IsPlural)
                {
                    if (selfFormsLength != textCatalog.NumPlurals)
                    {
                        if (selfFormsLength == 1)
                        {
                            if (newKeyRefsLength == 0)
                            {
                                WriteToLoggerAndTranslationLine(
                                    true,
                                    XlsExporter.Err.ImpHasntPluralForms,
                                    columnType,
                                    translationLine,
                                    textCatalog,
                                    nameof(translationLine.IsPlural));
                                return;
                            }

                            //Видим, что строке есть подстановки и они теоретически могут давать корректное число форм.
                            //Но на данном этапе мы это не можем определить, потому что таблица в процессе импорта
                            //и ключ, на который сылается подстановка, м.б. еще не импортирован.
                            //Поэтому здесь считаем приемлимым наличие 1 собственной формы для плюральной строки имеющей подстановку
                        }
                        else
                        {
                            WriteToLoggerAndTranslationLine(
                                true,
                                XlsExporter.Err.ImpWrongPluralFormsCount,
                                columnType,
                                translationLine,
                                textCatalog,
                                selfFormsLength,
                                textCatalog.NumPlurals);
                            return;
                        }
                    }
                }
                else
                {
                    if (selfFormsLength > 1)
                    {
                        WriteToLoggerAndTranslationLine(
                            true,
                            XlsExporter.Err.ImpUnexpectedPlural,
                            columnType,
                            translationLine,
                            textCatalog,
                            nameof(translationLine.IsPlural));
                        return;
                    }
                }
            }

            if (!textCatalog.Translations.TryGetValue(translationLine.Key, out var translationData))
            {
                //это не может произойти dev словаре
                translationData = new TranslationDataExt()
                {
                    IsPlural = translationLine.IsPlural,
                    Version = translationLine.Version,
                    Text = value.Text
                };
                translationData.HashRecalc();

                textCatalog.Translations.Add(translationLine.Key, translationData);
                textCatalog.IsChanged = true;
                value.HasBeenImported = true;
                return;
            }

            var originalText = isComments ? translationData.Comments : translationData.Text;
            if (originalText != value.Text)
            {
                value.HasChanges = true;

                if ((value.ColumnImportData.ImportOption == ColumnImportOption.ImportIfVersionGreater &&
                     translationLine.Version > translationData.Version) ||
                    (value.ColumnImportData.ImportOption == ColumnImportOption.ImportIfVersionEqualsOrGreater &&
                     translationLine.Version >= translationData.Version))
                {
                    if (isComments)
                    {
                        translationData.Comments = value.Text;
                    }
                    else
                    {
                        translationData.Text = value.Text;
                        translationData.Version = translationLine.Version;
                        translationData.IsPlural = translationLine.IsPlural;
                        translationData.HashRecalc();
                    }

                    textCatalog.IsChanged = true;
                    value.HasBeenImported = true;
                }
            }
        }

        private void WriteToLoggerAndTranslationLine(
            bool isErrorNorWarn,
            XlsExporter.Err error,
            XlsColumnType columnType,
            TranslationLineBase translationLine,
            TextCatalog textCatalog,
            params object[] args)
        {
            XlsExporter.WriteToLoggerAndTranslationLine(Logger, isErrorNorWarn, error, columnType, translationLine, textCatalog, args);
        }

        // private void AddNLogWarning(ImportTranslationLine translationLine, XlsColumnType columnType, string msg, TextCatalog textCatalog = null)
        // {
        //     AddNLogMessage(false, translationLine, columnType, msg, textCatalog);
        // }
        //
        // private void AddNLogError(ImportTranslationLine translationLine, XlsColumnType columnType, string msg, TextCatalog textCatalog = null)
        // {
        //     AddNLogMessage(true, translationLine, columnType, msg, textCatalog);
        // }

        // private void AddNLogMessage(
        //     bool isErrorNorWarning,
        //     ImportTranslationLine translationLine,
        //     XlsColumnType columnType,
        //     string msg,
        //     TextCatalog textCatalog)
        // {
        //     msg = $"{(isErrorNorWarning ? TranslationLine.ErrorBegin : TranslationLine.WarningBegin)}[{columnType}] {msg}";
        //     translationLine.AddErrors(msg);
        //     var logMsg = $"{translationLine.Key} {msg}{(textCatalog != null ? $" -- {textCatalog}" : "")}";
        //     Logger.Log(isErrorNorWarning ? LogLevel.Error : LogLevel.Warn, logMsg);
        // }

        private void TableLineHighlighting(
            ImportTranslationLine translationLine,
            ExcelWorksheet worksheet,
            int rowIdx,
            IDictionary<XlsColumnType, ColumnImportData> columnsData,
            BackgroundColors backgroundColors)
        {
            worksheet.Cells[rowIdx, columnsData[XlsColumnType.Errors].ColumnIndex].Value = translationLine.Errors;

            foreach (var columnData in columnsData.Values)
            {
                var needForHighlightning = false;
                var backgroundColor = Color.white;
                var columnType = columnData.ColumnType;
                if (XlsExporter.ShouldBeMarkedAsError(translationLine.Errors, columnType))
                {
                    needForHighlightning = true;
                    backgroundColor = backgroundColors.ErrorColor;
                }
                else
                {
                    var columnValue = translationLine.GetColumnValue(columnType);
                    if (columnValue != null && (columnValue.HasBeenImported || columnValue.HasChanges))
                    {
                        needForHighlightning = true;
                        backgroundColor = columnValue.HasBeenImported ? backgroundColors.OkColor : backgroundColors.WarningColor;
                    }
                    else
                    {
                        if (XlsExporter.ShouldBeMarkedAsWarning(translationLine.Errors, columnType))
                        {
                            needForHighlightning = true;
                            backgroundColor = backgroundColors.WarningColor;
                        }
                    }
                }

                if (needForHighlightning)
                    XlsExporter.SetStyleBackgroundColor(worksheet.Cells[rowIdx, columnData.ColumnIndex].Style, backgroundColor);
            }
        }

        private string GetString(ExcelWorksheet worksheet, int rowIdx, int columnIdx)
        {
            return worksheet.Cells[rowIdx, columnIdx].Value?.ToString() ?? null;
        }

        private int GetInt(ExcelWorksheet worksheet, int rowIdx, int columnIdx)
        {
            var val = worksheet.Cells[rowIdx, columnIdx].Value;
            return Convert.ToInt32(val);
        }

        private bool GetBool(ExcelWorksheet worksheet, int rowIdx, int columnIdx)
        {
            var val = worksheet.Cells[rowIdx, columnIdx].Value;
            return !string.IsNullOrEmpty(val as string);
        }

        private void ClearCellText(ExcelWorksheet worksheet, int rowIndex, int columnIndex)
        {
            worksheet.Cells[rowIndex, columnIndex].Value = null;
        }
    }
}