using UnityEngine;

namespace L10n.XlsConverterNs
{
    public struct WorksheetColumnFormat
    {
        public XlsColumnType ColumnType;
        public ColumnImportOption DefaultColumnImportOption;
        public string Title;
        public float Width;
        public bool IsOutlined;
        public CellStyleFormat TitleStyle;
        public CellStyleFormat CellsStyle;
    }

    public struct CellStyleFormat
    {
        public Color32 TextColor;
        public bool HasTextColor;
        public Color32 BackgroundColor;
        public bool HasBackgroundColor;

        public int FontSizeDelta;
        public bool IsBold;
        public bool IsCentered;
        public bool IsLocked;
    }

    public class BackgroundColors
    {
        public Color32 OkColor;
        public Color32 WarningColor;
        public Color32 ErrorColor;
    }
}