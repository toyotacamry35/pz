using System.Collections.Generic;
using Assets.Src.ResourceSystem.L10n;
using Core.Environment.Logging.Extension;
using Uins;
using UnityWeld.Binding;

namespace L10n
{
    [Binding]
    public class LocalizationKeysHolder : BindingViewModel
    {
        public LocalizationKeysDefRef JdbRef;

        private LocalizationKeysDef _def;

        public string JdbKey1;
        public string JdbKey2;
        public string JdbKey3;
        public string JdbKey4;

        public bool UseKey2;
        public bool UseKey3;
        public bool UseKey4;


        //=== Props ===========================================================

        private LocalizedString _ls1;

        [Binding]
        public LocalizedString Ls1
        {
            get => _ls1;
            set
            {
                if (!_ls1.Equals(value))
                {
                    _ls1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _ls2;

        [Binding]
        public LocalizedString Ls2
        {
            get => _ls2;
            set
            {
                if (!_ls2.Equals(value))
                {
                    _ls2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _ls3;

        [Binding]
        public LocalizedString Ls3
        {
            get => _ls3;
            set
            {
                if (!_ls3.Equals(value))
                {
                    _ls3 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _ls4;

        [Binding]
        public LocalizedString Ls4
        {
            get => _ls4;
            set
            {
                if (!_ls4.Equals(value))
                {
                    _ls4 = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (string.IsNullOrEmpty(JdbKey1))
            {
                UI.Logger.IfError()?.Message($"[{transform.FullName()}] <{nameof(LocalizationKeysHolder)}> Empty {nameof(JdbKey1)}").Write();
                return;
            }

            _def = JdbRef.Target;
            if (_def.AssertIfNull(nameof(_def), gameObject))
                return;

            if (_def.LocalizedStrings.AssertIfNull("_def.LocalizedStrings"))
                return;

            Ls1 = GetLsByKey(_def.LocalizedStrings, JdbKey1);

            if (UseKey2)
                Ls2 = GetLsByKey(_def.LocalizedStrings, JdbKey2);

            if (UseKey3)
                Ls3 = GetLsByKey(_def.LocalizedStrings, JdbKey3);

            if (UseKey4)
                Ls4 = GetLsByKey(_def.LocalizedStrings, JdbKey4);
        }

        private LocalizedString GetLsByKey(Dictionary<string, LocalizedString> localizedStrings, string jdbKey)
        {
            LocalizedString ls = LsExtensions.Empty;
            if (!localizedStrings.TryGetValue(jdbKey, out ls))
            {
                UI.Logger.IfError()?.Message($"[{transform.FullName()}] <{nameof(LocalizationKeysHolder)}> Not found LS by key={jdbKey}").Write();
                ls.Key = "!" + jdbKey;
            }

            return ls;
        }
    }
}