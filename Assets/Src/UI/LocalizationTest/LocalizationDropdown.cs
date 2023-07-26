using System.Linq;
using Assets.Src.ResourceSystem.L10n;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using TMPro;
using UnityEngine;
using Utilities;

namespace Uins
{
    public class LocalizationDropdown : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private LocalizationConfigDefRef _localizationConfigDefRef;

        [SerializeField, UsedImplicitly]
        private TMP_Dropdown _tmpDropdown;

        private CultureData[] _cultures;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder)) ||
                _localizationConfigDefRef.Target.AssertIfNull(nameof(_localizationConfigDefRef)) ||
                _tmpDropdown.AssertIfNull(nameof(_tmpDropdown)))
                return;

            _tmpDropdown.ClearOptions();
            _cultures = _localizationConfigDefRef.Target.LocalizationCultures;
            _tmpDropdown.AddOptions(_cultures.Select(cd => cd.Description).ToList());

            L10nHolder.Instance.LocalizationChanged += OnLocalizationChanged;
            OnLocalizationChanged(L10nHolder.Instance.CurrentLocalization?.CultureData.Code ?? "");
            _tmpDropdown.onValueChanged.AddListener(DropdownValueChanged);
        }


        //=== Private =========================================================

        private void OnLocalizationChanged(string localizationName)
        {
            if (localizationName == "")
                return;

            var index = _cultures.IndexOf(cd => cd.Code == localizationName);
            if (index < 1)
                index = 0;

            _tmpDropdown.value = index;
        }

        private void DropdownValueChanged(int index)
        {
            if (index < 0 || _cultures == null || index >= _cultures.Length)
            {
                UI.Logger.IfError()?.Message($"Wrong index {index} or {nameof(_cultures)} is empty").Write();
                return;
            }

            var cultureCode = _cultures[index].Code;
            if (cultureCode != (L10nHolder.Instance.CurrentLocalization?.CultureData.Code ?? ""))
                L10nHolder.Instance.ChangeLocalization(cultureCode);
        }
    }
}