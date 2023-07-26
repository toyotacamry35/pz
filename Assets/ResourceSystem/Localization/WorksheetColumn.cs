namespace L10n
{
    public struct WorksheetColumn
    {
        public XlsColumnType ColumnType { get; set; }
        public ColumnImportOption DefaultColumnImportOption { get; set; }
        public string Title { get; set; }
        public float Width { get; set; }
        public bool IsOutlined { get; set; }
        public CellStyle TitleStyle { get; set; }
        public CellStyle CellsStyle { get; set; }
    }

    public struct CellStyle
    {
        public string TextHexColor { get; set; } //НазваниеЦвета или #RGB или #RGBA
        public string BackgroundHexColor { get; set; }
        public int FontSizeDelta { get; set; }
        public bool IsBold { get; set; }
        public bool IsCentered { get; set; }
        public bool IsLocked { get; set; }
    }

    public enum XlsColumnType
    {
        Blank,
        Key,
        Version,
        IsPlural,
        OrgText,
        DstOldText,
        DstNewText,
        Dst2NewText,
        Comments,
        Errors
    }

    public enum ColumnImportOption
    {
        Ignore, //игнорировать столбец при импорте
        ImportIfVersionGreater, //импортировать ячейки, версия которых выше, чем в базе
        ImportIfVersionEqualsOrGreater //импортировать ячейки, версия которых выше или равна версии в базе
    }
}