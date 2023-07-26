using L10n;
using Uins.Tooltips;

namespace Uins
{
    public class TechPointTooltipDescription : BaseTooltipDescription
    {
        public LocalizedString TechPointName { get; set; } //from binding

        public LocalizedString TechPointDescr { get; set; } //from binding

        public override bool HasDescription => !TechPointName.IsEmpty() || !TechPointDescr.IsEmpty();

        public override object Description => HasDescription
            ? $"{TechPointName.GetText()}\n\n{TechPointDescr.GetText()}"
            : LsExtensions.EmptyWarning.GetText();
    }
}