namespace L10n.XlsConverterNs
{
    /// <summary>
    /// Класс, в который читается из Excel-таблица вся информация по ключу,
    /// а затем в него же дописываются результаты анализа данных и импорта,
    /// на основании чего происходит окрашивание соответствующих ячеек в таблице
    /// </summary>
    public class ImportTranslationLine : TranslationLineBase
    {
        public ColumnValue OrgText;
        public ColumnValue DstNewText;
        public ColumnValue Dst2NewText;
        public ColumnValue Comments;


        //=== Public ==========================================================

        public ColumnValue GetColumnValue(XlsColumnType columnType)
        {
            switch (columnType)
            {
                case XlsColumnType.OrgText:
                    return OrgText;

                case XlsColumnType.Comments:
                    return Comments;

                case XlsColumnType.DstNewText:
                    return DstNewText;

                case XlsColumnType.Dst2NewText:
                    return Dst2NewText;

                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return $"({nameof(ImportTranslationLine)}: [{Key}] v{Version}, {(IsPlural ? "plural, " : "")}ColumnValues:\n" +
                   $"  {nameof(OrgText)}: {OrgText}\n" +
                   $"  {nameof(DstNewText)}: {DstNewText}\n  {nameof(Dst2NewText)}: {Dst2NewText}\n" +
                   $"  {nameof(Comments)}: {Comments}\n  {nameof(Errors)}: <<{Errors}>>";
        }
    }

    public class ColumnValue
    {
        public string Text;
        public ColumnImportData ColumnImportData;

        /// <summary>
        /// Значение было импортировано в словарь
        /// </summary>
        public bool HasBeenImported;

        /// <summary>
        /// Флаг того, что значение отличается от словаря 
        /// </summary>
        public bool HasChanges;

        public ColumnValue(string text, ColumnImportData columnImportData)
        {
            Text = text;
            ColumnImportData = columnImportData;
        }

        public override string ToString()
        {
            return $"({nameof(ColumnValue)}: {ColumnImportData}, {nameof(HasBeenImported)}{HasBeenImported.AsSign()}, " +
                   $"{nameof(HasChanges)}{HasChanges.AsSign()}, '{Text}' /cv)";
        }
    }

    public class ColumnImportData
    {
        public XlsColumnType ColumnType;
        public ColumnImportOption ImportOption;
        public int ColumnIndex;

        public override string ToString()
        {
            return $"({nameof(ColumnImportData)}: {nameof(ColumnType)}={ColumnType}, idx={ColumnIndex}, {nameof(ImportOption)}={ImportOption} /cid)";
        }
    }
}