using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class LocalizationButton : BindingViewModel
    {
        [SerializeField, UsedImplicitly, FormerlySerializedAs("_selfCultureName")]
        private string _selfCultureCode = "en-US";


        //=== Props ===========================================================

        private bool _isActive;

        [Binding]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder)))
                return;

            L10nHolder.Instance.LocalizationChanged += OnLocalizationChanged;
            IsActive = GetIsActive(L10nHolder.Instance.CurrentLocalization?.CultureData.Code ?? "");
        }

        private bool GetIsActive(string cultureCode)
        {
            return cultureCode == _selfCultureCode;
        }

        private void OnLocalizationChanged(string localizationName)
        {
            IsActive = GetIsActive(localizationName);
        }


        //=== Public ==========================================================

        public void OnClick()
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder)))
                return;

            if (!L10nHolder.Instance.IsSuitableCultureCode(_selfCultureCode))
            {
                UI.Logger.IfError()?.Message($"{nameof(L10nHolder.Instance.AvailableCultures)} don't contains '{_selfCultureCode}'").Write();
                return;
            }

            L10nHolder.Instance.ChangeLocalization(_selfCultureCode);
        }
    }
}