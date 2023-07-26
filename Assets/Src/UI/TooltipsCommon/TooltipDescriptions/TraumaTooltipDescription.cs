using L10n;

namespace Uins.Tooltips
{
    public class TraumaTooltipDescription : BaseTooltipDescription
    {
        public LocalizedString DescriptionFromViewModel { get; set; } //from binding

        public override bool HasDescription => !DescriptionFromViewModel.IsEmpty();

        public override object Description => (HasDescription ? DescriptionFromViewModel : LsExtensions.EmptyWarning).GetText();
    }
}