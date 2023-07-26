using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    public class XlsExportConfigDef : BaseResource
    {
        public string WorksheetName { get; set; }
        public string ImportOptionsRowTitle { get; set; }
        public WorksheetColumn[] Columns { get; set; }

        //НазваниеЦвета или #RGB или #RGBA
        public string OkBackgroundHexColor { get; set; }
        public string WarningBackgroundHexColor { get; set; }
        public string ErrorBackgroundHexColor { get; set; }
    }
}