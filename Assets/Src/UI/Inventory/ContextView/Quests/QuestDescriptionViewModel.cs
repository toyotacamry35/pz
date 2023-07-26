using L10n;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestDescriptionViewModel : SomeItemViewModel<LocalizedStringContainer>
    {
        //=== Props ===========================================================

        private LocalizedString _description;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            set
            {
                if (!_description.Equals(value))
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isVisibleLine;

        [Binding]
        public bool IsVisibleLine
        {
            get => _isVisibleLine;
            set
            {
                if (_isVisibleLine != value)
                {
                    _isVisibleLine = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public override void Fill(LocalizedStringContainer data)
        {
            IsVisibleLine = Index > 0;
            Description = data?.LocalizedString ?? LsExtensions.Empty;
        }
    }
}