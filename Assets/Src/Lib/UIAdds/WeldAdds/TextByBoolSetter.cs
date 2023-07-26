using JetBrains.Annotations;
using L10n;
using UnityEngine;

namespace WeldAdds
{
    public class TextByBoolSetter : BaseByBoolSetter
    {
        [SerializeField, UsedImplicitly]
        private LocalizedTextMeshPro Target;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyProp UnsetText;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyProp SetText;

        protected override void Init()
        {
            Target.AssertIfNull(nameof(Target), gameObject);
        }

        protected override void SyncIfWoken()
        {
            Target.LocalizedString = Flag ? SetText.LocalizedString : UnsetText.LocalizedString;
        }
    }
}