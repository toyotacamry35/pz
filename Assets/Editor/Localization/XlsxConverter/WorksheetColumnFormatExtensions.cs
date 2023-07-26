using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;

namespace L10n.XlsConverterNs
{
    public static class WorksheetColumnFormatExtensions
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("UI");

        public static WorksheetColumnFormat GetWorksheetColumnFormat(this WorksheetColumn worksheetColumn)
        {
            return new WorksheetColumnFormat()
            {
                Title = worksheetColumn.Title,
                Width = worksheetColumn.Width,
                IsOutlined = worksheetColumn.IsOutlined,
                ColumnType = worksheetColumn.ColumnType,
                DefaultColumnImportOption = worksheetColumn.DefaultColumnImportOption,
                TitleStyle = GetCellStyleFormat(worksheetColumn.TitleStyle),
                CellsStyle = GetCellStyleFormat(worksheetColumn.CellsStyle)
            };
        }

        public static CellStyleFormat GetCellStyleFormat(this CellStyle cellStyle)
        {
            var textColorTuple = GetColorFromHexString(cellStyle.TextHexColor);
            var bgColorTuple = GetColorFromHexString(cellStyle.BackgroundHexColor);
            return new CellStyleFormat()
            {
                IsBold = cellStyle.IsBold,
                IsCentered = cellStyle.IsCentered,
                IsLocked = cellStyle.IsLocked,
                FontSizeDelta = cellStyle.FontSizeDelta,
                HasTextColor = textColorTuple.Item1,
                TextColor = textColorTuple.Item2,
                HasBackgroundColor = bgColorTuple.Item1,
                BackgroundColor = bgColorTuple.Item2,
            };
        }

        public static (bool, Color) GetColorFromHexString(string hexColor)
        {
            var hasColor = !string.IsNullOrEmpty(hexColor);
            var color = Color.black;
            if (hasColor)
            {
                if (ColorUtility.TryParseHtmlString(hexColor, out var parsedColor))
                    color = parsedColor;
                else
                    Logger.IfError()?.Message($"Unable to parse color from '{hexColor}'").Write();
            }

            return (hasColor, color);
        }

        public static BackgroundColors GetBackgroundColors(this XlsExportConfigDef xlsExportConfig)
        {
            var okTuple = GetColorFromHexString(xlsExportConfig.OkBackgroundHexColor);
            var warnTuple = GetColorFromHexString(xlsExportConfig.WarningBackgroundHexColor);
            var errTuple = GetColorFromHexString(xlsExportConfig.ErrorBackgroundHexColor);
            return new BackgroundColors()
            {
                OkColor = okTuple.Item1 ? okTuple.Item2 : Color.white, //���� ���� ��� �� ������, �� �����
                WarningColor = warnTuple.Item1 ? warnTuple.Item2 : Color.white,
                ErrorColor = errTuple.Item1 ? errTuple.Item2 : Color.white,
            };
        }
    }
}