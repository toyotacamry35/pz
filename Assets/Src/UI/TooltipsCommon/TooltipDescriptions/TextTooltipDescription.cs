using Assets.Src.ResourceSystem.L10n;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using UnityEngine;

namespace Uins.Tooltips
{
    public class TextTooltipDescription : BaseTooltipDescription
    {
        [SerializeField, UsedImplicitly]
        private string _descriptionJdbKey;

        [SerializeField, UsedImplicitly]
        private LocalizationKeysDefRef _defRef;

        private LocalizedString _descriptionLs;

        public override bool HasDescription => !_descriptionLs.IsEmpty();

        public override object Description => (HasDescription ? _descriptionLs : LsExtensions.EmptyWarning).GetText();

        private void Awake()
        {
            if (_defRef.Target.AssertIfNull(nameof(_defRef)))
                return;

            var localizedStrings = _defRef.Target.LocalizedStrings;
            if (localizedStrings.AssertIfNull(nameof(localizedStrings)))
                return;

            if (string.IsNullOrEmpty(_descriptionJdbKey))
            {
                UI.Logger.IfError()?.Message($"{nameof(_descriptionJdbKey)} is empty", gameObject).Write();
                return;
            }

            if (!localizedStrings.TryGetValue(_descriptionJdbKey, out _descriptionLs))
                UI.Logger.IfError()?.Message($"Not found {nameof(LocalizedString)} for {nameof(_descriptionJdbKey)}='{_descriptionJdbKey}'", gameObject).Write();
        }
    }
}