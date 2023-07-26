using Assets.Src.ResourceSystem.L10n;
using Uins;
using UnityWeld.Binding;

namespace L10n
{
    [Binding]
    public class LocalizationKeyPairsDefHolder : BindingViewModel
    {
        public enum LocalizationKeyPair
        {
            Ls1,
            Ls2,
            AltLs1,
            AltLs2
        }

        public LocalizationKeyPairsDefRef JdbRef;

        public LocalizationKeyPair Selected;

        private LocalizationKeyPairsDef _def;


        //=== Props ===========================================================

        private LocalizedString _localizedString;

        [Binding]
        public LocalizedString LocalizedString
        {
            get => _localizedString;
            set
            {
                if (!_localizedString.Equals(value))
                {
                    _localizedString = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _def = JdbRef.Target;
            if (_def.AssertIfNull(nameof(_def), gameObject))
                return;

            LocalizedString = GetLs(_def, Selected);
        }


        //=== Private =========================================================

        private LocalizedString GetLs(LocalizationKeyPairsDef def, LocalizationKeyPair selected)
        {
            if (def == null)
                return LsExtensions.Empty;

            switch (selected)
            {
                case LocalizationKeyPair.Ls1:
                    return def.Ls1;

                case LocalizationKeyPair.Ls2:
                    return def.Ls2;
                case LocalizationKeyPair.AltLs1:
                    return def.AltLs1;

                default:
                    return def.AltLs2;
            }
        }
    }
}