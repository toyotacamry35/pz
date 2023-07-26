using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using L10n.KeysExtractionNs;
using ResourcesSystem.Loader;
using L10n.Loaders;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Uins;
using UnityEditor;
using UnityEngine;

namespace L10n.XlsConverterNs
{
    public class XlsExporter
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        public enum Err
        {
            //-- Prepare-stage
            CtorWrongParams = 1001,
            Exception,
            UnableToCreateFolder,
            XlsFileNotExists,
            XlsFileSaveError,
            XlsExportConfigIsntValid,
            LocalizationDefSaveError,
            LocalizationDefSaveException,

            //-- Common
            DuplicatedColumns = 1101,
            EmptyString,
            WhitespaceString,
            DiffIsPlural,
            ArgsCountInconsistency,

            //-- Export
            ExpMissingColumns = 1201,
            ExpExistingFileDeletion,
            ExpWrongFirstRowIndex,
            ExpDiffPluralFormsCount,

            //-- Import
            ImpColumnAlreadyExists = 1301,
            ImpCultureCodeMissing,
            ImpCultureCodeMissingInString,
            ImpColumnValueIsNull,
            ImpDictionaryNotContainsKey,
            ImpIncorrectValue,
            ImpDiffArgsCount,
            ImpDiffKeySubstCount,
            ImpHasntPluralForms,
            ImpWrongPluralFormsCount,
            ImpUnexpectedPlural,
        }

        private static readonly Dictionary<Err, string> Errors = new Dictionary<Err, string>()
        {
            //-- Prepare-stage
            {Err.CtorWrongParams, "Некорректные параметры в конструкторе"},
            {Err.Exception, "Исключение: {0}"},
            {Err.UnableToCreateFolder, "Не удалось создать папку '{0}': {1}"},
            {Err.XlsFileNotExists, "Не выбран или не существует файл '{0}'"},
            {Err.XlsFileSaveError, "Невозможно сохранить файл {0} (наверное он открыт в Excel)"},
            {Err.XlsExportConfigIsntValid, "Не валиден XlsExportConfig '{0}'"},
            {Err.LocalizationDefSaveError, "Файл локализации '{0}' не был перезаписан!"},
            {Err.LocalizationDefSaveException, "Файл локализации '{0}' не был перезаписан!\n{1}"},

            //-- Common
            {Err.DuplicatedColumns, "Есть повторяющиеся столбцы: {0}"},
            {Err.EmptyString, "Пустая строка"},
            {Err.WhitespaceString, "Вместо перевода пробел"},
            {Err.DiffIsPlural, "Несоответствие {0}: должно быть {1}"},
            {Err.ArgsCountInconsistency, "Различие числа аргументов в плюральных формах или пустое поле Text"},

            //-- Export
            {Err.ExpMissingColumns, "Отсутствуют следующие обязательные столбцы: {0}"},
            {Err.ExpExistingFileDeletion, "Удаление существующего файла: '{0}'"},
            {Err.ExpWrongFirstRowIndex, "Некорректный {0}={1}. Исправлено на {2}"},
            {Err.ExpDiffPluralFormsCount, "Несоответствие числа plural-форм ({0}) языковому правилу ({1})"},

            //-- Import
            {Err.ImpColumnAlreadyExists, "Столбец типа {0} уже есть в таблице"},
            {Err.ImpCultureCodeMissing, "Не прочлось название культуры в столбце '{0}' (row={1}, col={2})"},
            {Err.ImpCultureCodeMissingInString, "Не прочлось название культуры в столбце {0} (row={1}, col={2}) из строки '{3}'"},
            {Err.ImpColumnValueIsNull, "{0} == null из столбца '{1}' -- {2} {3}"},
            {Err.ImpDictionaryNotContainsKey, "dev словарь не содержит ключа '{0}'"},
            {Err.ImpIncorrectValue, "Некорректное значение '{0}'={1}"},
            {Err.ImpDiffArgsCount, "Число ссылок на аргументы в строке ({0}) не соответствует их числу в оригинальной строке ({1})"},
            {Err.ImpDiffKeySubstCount, "Число подстановок ключей в строке ({0}) не соответствует их числу в оригинальной строке ({1})"},
            {Err.ImpHasntPluralForms, "Строка НЕ имеет множественных форм, что не соответствует флагу {0}"},
            {Err.ImpWrongPluralFormsCount, "Число множественных форм строки ({0}) не соответствует нормам языка ({1})"},
            {Err.ImpUnexpectedPlural, "Строка имеет множественные формы, что не соответствует флагу {0}"}
        };

        public const string ExportToXlsFilenameTemplate = "{0} {1}--{2}{3}.xlsx";
        public const string AvailableLanguageTemplate = "{0} ({1})";
        public const int AdditionalCellFontSizeDelta = -2;

        /// <summary>
        /// Список типов столбцов, которые должны присутствовать в XlsExportConfigDef
        /// </summary>
        public static readonly XlsColumnType[] MandatoryColumnTypes = new XlsColumnType[]
        {
            XlsColumnType.Key,
            XlsColumnType.Version,
            XlsColumnType.IsPlural,
            XlsColumnType.OrgText,
            XlsColumnType.DstNewText,
            XlsColumnType.Errors
        };

        private string _xlsFolderPath;
        private GameResources _gameResources;


        //=== Ctor ============================================================

        public XlsExporter(string repositoryPath, string xlsFolderRelPath, GameResources gameResources)
        {
            if (string.IsNullOrEmpty(repositoryPath) ||
                string.IsNullOrEmpty(xlsFolderRelPath) ||
                gameResources.AssertIfNull(nameof(gameResources)))
            {
                ErrorToLogger(Err.CtorWrongParams);
                return;
            }

            _xlsFolderPath = repositoryPath + xlsFolderRelPath;
            _gameResources = gameResources;
        }


        //=== Public ==========================================================

        public void Export(bool isDevRu, bool isAllKeys)
        {
            var localizationConfig = _gameResources.LoadResource<LocalizationConfigDef>(KeysExtractionService.LocalizationConfigRelPath);

            if (localizationConfig.AssertIfNull(nameof(localizationConfig)) ||
                !IsXlsExportConfigValid(localizationConfig.XlsExportConfig.Target) ||
                !CheckOrCreateFolder(_xlsFolderPath))
                return;

            var orgCulture = isDevRu
                ? localizationConfig.DevCulture
                : localizationConfig.LocalizationCultures[localizationConfig.LocalizationPipelineIndex1];

            var dstCulture = isDevRu
                ? localizationConfig.LocalizationCultures[localizationConfig.LocalizationPipelineIndex1]
                : localizationConfig.LocalizationCultures[localizationConfig.LocalizationPipelineIndex2];

            var jdbLoader = new JdbLoader(_gameResources, localizationConfig.DomainFile, "/" + localizationConfig.LocalesDirName);
            var orgTextCatalog = new TextCatalog(orgCulture, jdbLoader);
            var dstTextCatalog = new TextCatalog(dstCulture, jdbLoader);
            var commentsTextCatalog = isDevRu ? orgTextCatalog : new TextCatalog(localizationConfig.DevCulture, jdbLoader);

            var translationLines = GetTranslationLines(orgTextCatalog, dstTextCatalog, isAllKeys, commentsTextCatalog);
//            Logger.Info(
//                $"{nameof(Export)}(isDevRu{isDevRu.AsSign()}, isAllKeys{isAllKeys.AsSign()}) xlsFolderPath='{xlsFolderPath}', " +
//                $"org={orgCulture}, trg={targetCulture}"); //DEBUG
//            Logger.IfInfo()?.Message($"translationLines: {translationLines.ItemsToStringByLines()}").Write(); //DEBUG

            var filename = string.Format(ExportToXlsFilenameTemplate, GetCurrentDateTime(), orgCulture.Folder, dstCulture.Folder, isAllKeys ? " all" : "");

            try
            {
                var fileInfo = new FileInfo(Path.Combine(_xlsFolderPath, filename));
                if (fileInfo.Exists)
                {
                    WarnToLogger(Err.ExpExistingFileDeletion, fileInfo.FullName);
                    File.Delete(fileInfo.FullName);
                }

                using (var excelPackage = new ExcelPackage(fileInfo))
                {
                    ProcessExcelPackage(
                        excelPackage,
                        orgCulture,
                        dstCulture,
                        !isDevRu,
                        translationLines,
                        localizationConfig.XlsExportConfig.Target,
                        GetAvailableCultures(localizationConfig));
                    excelPackage.Save();
                    Logger.IfInfo()?.Message($"Saved to file '{fileInfo.FullName}'").Write();
                    EditorUtility.RevealInFinder(fileInfo.FullName);
                }
            }
            catch (Exception e)
            {
                ErrorToLogger(Err.Exception, e);
            }
        }

        /// <summary>
        /// Проверка на валидность конфига 
        /// </summary>
        public static bool IsXlsExportConfigValid(XlsExportConfigDef xlsExportConfig)
        {
            if (xlsExportConfig.AssertIfNull(nameof(xlsExportConfig)) ||
                xlsExportConfig.Columns.AssertIfNull($"{nameof(xlsExportConfig)}.{nameof(xlsExportConfig.Columns)}"))
                return false;

            var columnTypes = xlsExportConfig.Columns.Select(c => c.ColumnType).ToArray();
            return IsColumnsValid(columnTypes);
        }

        /// <summary>
        /// Проверка на валидность столбцов
        /// </summary>
        public static bool IsColumnsValid(IList<XlsColumnType> columnTypes)
        {
            var missingMandatoryColumnTypes = MandatoryColumnTypes.Where(mct => !columnTypes.Contains(mct)).ToArray();
            if (missingMandatoryColumnTypes.Length > 0)
            {
                ErrorToLogger(Err.ExpMissingColumns, missingMandatoryColumnTypes.ItemsToString(false));
                return false;
            }

            if (columnTypes.Distinct().Count() != columnTypes.Count)
            {
                ErrorToLogger(Err.DuplicatedColumns, columnTypes.ItemsToString(false));
                return false;
            }

            return true;
        }

        public static bool ShouldBeMarkedAsWarning(string messages, XlsColumnType columnType)
        {
            return ShouldBeMarked(messages, TranslationLineBase.WarningBegin, columnType);
        }

        public static bool ShouldBeMarkedAsError(string messages, XlsColumnType columnType)
        {
            return ShouldBeMarked(messages, TranslationLineBase.ErrorBegin, columnType);
        }

        public static void SetStyleTextColor(ExcelStyle excelStyle, Color32 color)
        {
            excelStyle.Font.Color.SetColor(color.a, color.r, color.g, color.b); //в Excel у цвета alpha всегда 255
        }

        public static void SetStyleBackgroundColor(ExcelStyle excelStyle, Color32 color)
        {
            excelStyle.Fill.PatternType = ExcelFillStyle.Solid;
            excelStyle.Fill.BackgroundColor.SetColor(color.a, color.r, color.g, color.b); //в Excel alpha всегда 255
        }

        public static void ClearStyleBackgroundColor(ExcelStyle excelStyle)
        {
            excelStyle.Fill.PatternType = ExcelFillStyle.None;
        }

        public static string GetErrorNumberAndText(Err error)
        {
            return Errors.TryGetValue(error, out var msg)
                ? $"{(int) error} {msg}"
                : $"{nameof(Err)}.{error} Текст ошибки не задан";
        }

        public static void WriteToLoggerAndTranslationLine(
            NLog.Logger logger,
            bool isErrorNorWarn,
            Err error,
            XlsColumnType columnType,
            TranslationLineBase translationLine,
            TextCatalog textCatalog,
            params object[] args)
        {
            var commonMsg =
                $"{(isErrorNorWarn ? TranslationLineBase.ErrorBegin : TranslationLineBase.WarningBegin)}[{columnType}] {GetErrorNumberAndText(error)}";
            UI.ErrorOrWarn(logger, isErrorNorWarn, $"[{translationLine.Key}] {commonMsg}{(textCatalog != null ? $" -- {textCatalog}" : "")}", args);
            translationLine.AddErrors(commonMsg);
        }


        //=== Private =========================================================

        private static void ErrorToLogger(Err error, params object[] args)
        {
            UI.ErrorOrWarn(Logger, true, GetErrorNumberAndText(error), args);
        }

        private static void WarnToLogger(Err error, params object[] args)
        {
            UI.ErrorOrWarn(Logger, false, GetErrorNumberAndText(error), args);
        }

        private void WriteToLoggerAndTranslationLine(
            bool isErrorNorWarn,
            Err error,
            XlsColumnType columnType,
            TranslationLineBase translationLine,
            TextCatalog textCatalog,
            params object[] args)
        {
            WriteToLoggerAndTranslationLine(Logger, isErrorNorWarn, error, columnType, translationLine, textCatalog, args);
        }

        private List<TranslationLine> GetTranslationLines(
            TextCatalog orgTextCatalog,
            TextCatalog dstTextCatalog,
            bool isAllKeys,
            TextCatalog commentsTextCatalog)
        {
            var lines = new List<TranslationLine>();
            var pluralFormsCountByRule = orgTextCatalog.NumPlurals;
            foreach (var kvp in orgTextCatalog.Translations)
            {
                var orgTdata = kvp.Value;
                var translationLine = new TranslationLine()
                {
                    Key = kvp.Key,
                    Version = orgTdata.Version,
                    IsPlural = orgTdata.IsPlural,
                    OrgText = orgTdata.Text,
                };

                var hasDstTdata = dstTextCatalog.Translations.TryGetValue(translationLine.Key, out var dstTdata);

                if (!isAllKeys)
                {
                    if (hasDstTdata && dstTdata.Version >= translationLine.Version)
                        continue;
                }

                if (string.IsNullOrEmpty(translationLine.OrgText))
                    WriteToLoggerAndTranslationLine(false, Err.EmptyString, XlsColumnType.OrgText, translationLine, orgTextCatalog);
                else if (string.IsNullOrEmpty(translationLine.OrgText.Trim()))
                    WriteToLoggerAndTranslationLine(false, Err.WhitespaceString, XlsColumnType.OrgText, translationLine, orgTextCatalog);

                var calculatedPluralFormsCount = KeysExtractionService.CalculatePluralFormsCount(orgTdata, orgTextCatalog.Translations);
                var calculatedIsPlural = calculatedPluralFormsCount > 1;
                if (calculatedIsPlural != translationLine.IsPlural)
                    WriteToLoggerAndTranslationLine(
                        true,
                        Err.DiffIsPlural,
                        XlsColumnType.IsPlural,
                        translationLine,
                        orgTextCatalog,
                        nameof(orgTdata.IsPlural),
                        calculatedIsPlural);

                if (calculatedIsPlural && pluralFormsCountByRule != calculatedPluralFormsCount)
                    WriteToLoggerAndTranslationLine(
                        true,
                        Err.ExpDiffPluralFormsCount,
                        XlsColumnType.IsPlural,
                        translationLine,
                        orgTextCatalog,
                        calculatedPluralFormsCount,
                        pluralFormsCountByRule);

                if (orgTdata.GetArgReferencesCountFromText() < 0)
                    WriteToLoggerAndTranslationLine(
                        true,
                        Err.ArgsCountInconsistency,
                        XlsColumnType.OrgText,
                        translationLine,
                        orgTextCatalog);

                if (hasDstTdata && dstTdata.GetArgReferencesCountFromText() < 0)
                    WriteToLoggerAndTranslationLine(
                        true,
                        Err.ArgsCountInconsistency,
                        XlsColumnType.DstOldText,
                        translationLine,
                        dstTextCatalog);

                if (commentsTextCatalog.Translations.TryGetValue(translationLine.Key, out var commentTdata))
                {
                    translationLine.Comments = commentTdata.Comments;
                }

                translationLine.DstOldText = hasDstTdata ? dstTdata.Text : "";
                translationLine.DstNewText = hasDstTdata && dstTdata.Version >= translationLine.Version ? translationLine.DstOldText : "";
                lines.Add(translationLine);
            }

            return lines;
        }

        private CultureData[] GetAvailableCultures(LocalizationConfigDef localizationConfig)
        {
            if (localizationConfig.AssertIfNull(nameof(localizationConfig)))
                return null;

            return localizationConfig.LocalizationCultures
                .Where((value, index) => index != localizationConfig.LocalizationPipelineIndex1 && index != localizationConfig.LocalizationPipelineIndex2)
                .ToArray();
        }

        private bool CheckOrCreateFolder(string xlsFolderPath)
        {
            try
            {
                if (!Directory.Exists(xlsFolderPath))
                    Directory.CreateDirectory(xlsFolderPath);
            }
            catch (Exception e)
            {
                ErrorToLogger(Err.UnableToCreateFolder, xlsFolderPath, e);
                return false;
            }

            return true;
        }

        private string GetCurrentDateTime()
        {
            var now = DateTime.Now;
            return $"{now.Year}-{now.Month:00}-{now.Day:00} {now.Hour:00}_{now.Minute:00}";
        }


        private void ProcessExcelPackage(
            ExcelPackage excelPackage,
            CultureData orgCultureData,
            CultureData dstCultureData,
            bool showAdditionalTranslationColumn,
            List<TranslationLine> translationLines,
            XlsExportConfigDef xlsExportConfig,
            CultureData[] availableCultures)
        {
            if (xlsExportConfig.AssertIfNull(nameof(xlsExportConfig)))
                return;

            var worksheet = excelPackage.Workbook.Worksheets.Add(xlsExportConfig.WorksheetName);
            var backgroundColors = xlsExportConfig.GetBackgroundColors();
            var columnFormats = GetWorksheetColumnFormats(xlsExportConfig.Columns, showAdditionalTranslationColumn);
            var nextBlankRowIndex = ApplyFormatAndFillTitles(
                worksheet,
                1,
                columnFormats,
                orgCultureData,
                dstCultureData,
                availableCultures,
                xlsExportConfig.ImportOptionsRowTitle);
            FillValues(worksheet, nextBlankRowIndex, translationLines, columnFormats, backgroundColors);

            //worksheet.Protection.AllowSelectLockedCells = true; //разрешить выделять заблокированные
            //worksheet.Protection.IsProtected = true; //включить защиту листа

            worksheet.View.FreezePanes(nextBlankRowIndex, 2); //Непрокручиваемые области

            //Автофильтр
            var headersRowIndex = nextBlankRowIndex - 1;
            var headersRange = worksheet.Cells[headersRowIndex, 2, headersRowIndex, columnFormats.Count];
            headersRange.AutoFilter = true;
        }

        private List<WorksheetColumnFormat> GetWorksheetColumnFormats(WorksheetColumn[] columns, bool showAdditionalTranslationColumn)
        {
            if (columns.AssertIfNull(nameof(columns)))
                return null;

            var internalColumns = new List<WorksheetColumnFormat>();
            for (int i = 0; i < columns.Length; i++)
            {
                if (!showAdditionalTranslationColumn && columns[i].ColumnType == XlsColumnType.Dst2NewText)
                    continue;

                internalColumns.Add(columns[i].GetWorksheetColumnFormat());
            }

            return internalColumns;
        }



        private int ApplyFormatAndFillTitles(
            ExcelWorksheet worksheet,
            int firstRowIndex,
            List<WorksheetColumnFormat> columnFormats,
            CultureData orgCulture,
            CultureData dstCulture,
            CultureData[] availableCultures,
            string importOptionsRowTitle)
        {
            var defaultFontSize = worksheet.Cells[1, 1].Style.Font.Size;
            if (firstRowIndex <= 0)
            {
                ErrorToLogger(Err.ExpWrongFirstRowIndex, nameof(firstRowIndex), firstRowIndex, 1);
                firstRowIndex = 1;
            }

            if (columnFormats.AssertIfNull(nameof(columnFormats)))
                return firstRowIndex;

            for (int i = 0; i < columnFormats.Count; i++)
            {
                var columnFormat = columnFormats[i];
                var columnIndex = i + 1;

                ApplyColumnFormats(worksheet.Column(columnIndex), columnFormat);
                FillColumnKeyCell(worksheet.Cells[firstRowIndex, columnIndex], columnFormat, defaultFontSize);
                FillImportOptionsCell(worksheet.Cells[firstRowIndex + 1, columnIndex], columnFormat, defaultFontSize, worksheet, importOptionsRowTitle);
                FillHeaderCell(
                    worksheet.Cells[firstRowIndex + 2, columnIndex],
                    columnFormat,
                    defaultFontSize,
                    orgCulture,
                    dstCulture,
                    availableCultures,
                    worksheet);
            }

            var row = worksheet.Row(firstRowIndex); //строка id
            row.Height = 0;
            row.Style.Locked = true;

            return firstRowIndex + 3;
        }

        /// <summary>
        /// Форматирование всего столбца
        /// </summary>
        private void ApplyColumnFormats(ExcelColumn column, WorksheetColumnFormat columnFormat)
        {
            //Общие для всех правила
            column.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            column.Style.WrapText = true;

            //Правила формата ячейки
            var cellsStyle = columnFormat.CellsStyle;
            column.Width = columnFormat.Width;
            if (cellsStyle.IsCentered)
                column.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (cellsStyle.IsLocked)
                column.Style.Locked = true;

            if (cellsStyle.IsBold)
                column.Style.Font.Bold = true;

            if (cellsStyle.FontSizeDelta != 0)
                column.Style.Font.Size += cellsStyle.FontSizeDelta;

            if (columnFormat.IsOutlined)
                column.OutlineLevel = 1;

            if (cellsStyle.HasTextColor)
                SetStyleTextColor(column.Style, cellsStyle.TextColor);

            if (cellsStyle.HasBackgroundColor)
                SetStyleBackgroundColor(column.Style, cellsStyle.BackgroundColor);
        }

        /// <summary>
        /// Заполнение ячейки с ключом столбца (обычно скрытой)
        /// </summary>
        private void FillColumnKeyCell(ExcelRange cell, WorksheetColumnFormat columnFormat, float defaultFontSize)
        {
            cell.Style.Font.Size = defaultFontSize + AdditionalCellFontSizeDelta;
            cell.Style.Locked = true;
            cell.Value = columnFormat.ColumnType.ToString(); //ключ столбца
        }

        /// <summary>
        /// Заполнение ячейки опций импорта
        /// </summary>
        private void FillImportOptionsCell(
            ExcelRange cell,
            WorksheetColumnFormat columnFormat,
            float defaultFontSize,
            ExcelWorksheet worksheet,
            string importOptionsRowTitle)
        {
            cell.Style.Font.Size = defaultFontSize + AdditionalCellFontSizeDelta;
            SetStyleTextColor(cell.Style, Color.black);
            var isTitle = columnFormat.ColumnType == XlsColumnType.Key;
            cell.Style.HorizontalAlignment = isTitle ? ExcelHorizontalAlignment.Left : ExcelHorizontalAlignment.Center;
            if (isTitle)
            {
                cell.Style.Font.Bold = true;
                cell.Style.WrapText = false;
            }

            switch (columnFormat.ColumnType)
            {
                case XlsColumnType.Comments:
                case XlsColumnType.Dst2NewText:
                case XlsColumnType.DstNewText:
                case XlsColumnType.OrgText:
                    cell.Value = columnFormat.DefaultColumnImportOption.ToString();
                    var options = Enum.GetNames(typeof(ColumnImportOption))
                        .Select(t => t.ToString())
                        .ToArray();
                    SetDropdownList(worksheet, cell, options);
                    break;

                case XlsColumnType.Key:
                    cell.Value = importOptionsRowTitle;
                    break;
            }
        }

        /// <summary>
        /// Заполнение ячейки заголовка
        /// </summary>
        private void FillHeaderCell(
            ExcelRange cell,
            WorksheetColumnFormat columnFormat,
            float defaultFontSize,
            CultureData orgCulture,
            CultureData dstCulture,
            CultureData[] availableCultures,
            ExcelWorksheet worksheet)
        {
            //Общие для всех заголовков правила
            cell.Style.Border.Bottom.Style = cell.Style.Border.Top.Style = cell.Style.Border.Left.Style = cell.Style.Border.Right.Style =
                ExcelBorderStyle.Medium;

            //Правила формата ячейки заголовка
            var titleStyle = columnFormat.TitleStyle;
            cell.Style.Font.Bold = titleStyle.IsBold;
            cell.Style.Font.Size = defaultFontSize + titleStyle.FontSizeDelta;
            cell.Style.Locked = titleStyle.IsLocked;

            if (titleStyle.IsCentered)
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (titleStyle.HasTextColor)
                SetStyleTextColor(cell.Style, titleStyle.TextColor);

            if (titleStyle.HasBackgroundColor)
                SetStyleBackgroundColor(cell.Style, titleStyle.BackgroundColor);

            //Значение заголовка
            string title;
            switch (columnFormat.ColumnType)
            {
                case XlsColumnType.DstNewText:
                    title = string.Format(columnFormat.Title, dstCulture.Description, dstCulture.Code);
                    break;

                case XlsColumnType.DstOldText:
                    title = string.Format(columnFormat.Title, dstCulture.Description, dstCulture.Code);
                    break;

                case XlsColumnType.OrgText:
                    title = string.Format(columnFormat.Title, orgCulture.Description, orgCulture.Code);
                    break;

                case XlsColumnType.Dst2NewText:
                    title = columnFormat.Title;
                    if (!availableCultures.AssertIfNull(nameof(availableCultures)))
                    {
                        var options = availableCultures
                            .Select(clt => string.Format(AvailableLanguageTemplate, clt.Description, clt.Code))
                            .ToArray();
                        SetDropdownList(worksheet, cell, options); //выбор языка перевода
                    }

                    break;

                default:
                    title = columnFormat.Title;
                    break;
            }

            cell.Value = title;
        }

        /// <summary>
        /// Сделать выпадающий список опций (не выставляя значения ячейки)
        /// </summary>
        private void SetDropdownList(ExcelWorksheet worksheet, ExcelRange cell, string[] options)
        {
            if (options.AssertIfNull(nameof(options)) || options.Length == 0)
                return;

            var listValidation = worksheet.DataValidations.AddListValidation(cell.Address);
            for (int i = 0; i < options.Length; i++)
                listValidation.Formula.Values.Add(options[i]);
        }

        private void FillValues(
            ExcelWorksheet worksheet,
            int firstRowIndex,
            List<TranslationLine> translationLines,
            List<WorksheetColumnFormat> columnFormats,
            BackgroundColors backgroundColors)
        {
            if (columnFormats.AssertIfNull(nameof(columnFormats)))
                return;

            for (int i = 0; i < translationLines.Count; i++)
            {
                var rowIndex = firstRowIndex + i;
                var translationLine = translationLines[i];
                for (int j = 0; j < columnFormats.Count; j++)
                {
                    var columnIndex = j + 1;
                    var columnFormat = columnFormats[j];
                    var cell = worksheet.Cells[rowIndex, columnIndex];
                    string val = "";
                    switch (columnFormat.ColumnType)
                    {
                        case XlsColumnType.Key:
                            val = translationLine.Key;
                            break;

                        case XlsColumnType.IsPlural:
                            val = translationLine.IsPlural ? "+" : "";
                            break;
                        case XlsColumnType.Version:
                            val = translationLine.Version.ToString();
                            break;
                        case XlsColumnType.OrgText:
                            val = translationLine.OrgText;
                            break;
                        case XlsColumnType.DstOldText:
                            val = translationLine.DstOldText;
                            break;
                        case XlsColumnType.DstNewText:
                            val = translationLine.DstNewText;
                            break;
                        case XlsColumnType.Comments:
                            val = translationLine.Comments;
                            break;
                        case XlsColumnType.Errors:
                            val = translationLine.Errors;
                            break;
                    }

                    cell.Value = val;

                    if (ShouldBeMarkedAsWarning(translationLine.Errors, columnFormat.ColumnType))
                        SetStyleBackgroundColor(cell.Style, backgroundColors.WarningColor);

                    if (ShouldBeMarkedAsError(translationLine.Errors, columnFormat.ColumnType))
                        SetStyleBackgroundColor(cell.Style, backgroundColors.ErrorColor);
                }
            }
        }

        private static bool ShouldBeMarked(string messages, string messageBegin, XlsColumnType columnType)
        {
            return messages != null && messages.Contains($"{messageBegin}[{columnType}]");
        }
    }
}