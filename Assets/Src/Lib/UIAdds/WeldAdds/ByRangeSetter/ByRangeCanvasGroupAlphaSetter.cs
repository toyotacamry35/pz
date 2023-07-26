using UnityEngine;

namespace WeldAdds
{
    class ByRangeCanvasGroupAlphaSetter : ByRangeSetter
    {
        public CanvasGroup CanvasGroup;

        protected override bool SetupOnAwake()
        {
            return !CanvasGroup.AssertIfNull(nameof(CanvasGroup));
        }

        protected override void ApplySettingsToTarget(Settings settings)
        {
            CanvasGroup.alpha = settings.Color.a;
        }
    }
}