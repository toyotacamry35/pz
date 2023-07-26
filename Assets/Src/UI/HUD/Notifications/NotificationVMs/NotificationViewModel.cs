using Assets.Src.Lib.DOTweenAdds;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public abstract class NotificationViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private TweenComponentBase _fadeInTweenComponent;

        [SerializeField, UsedImplicitly]
        private TweenComponentBase _fadeOutTweenComponent;


        //=== Unity ===========================================================

        private void Awake()
        {
            _fadeInTweenComponent.AssertIfNull(nameof(_fadeInTweenComponent));
            _fadeOutTweenComponent.AssertIfNull(nameof(_fadeOutTweenComponent));
        }


        //=== Public ==========================================================

        public void Init(float beforeShowDelay, float showPeriod, float fadeIn, float fadeOut)
        {
            _fadeInTweenComponent.SetParamValue(true);

            _fadeInTweenComponent.Delay = beforeShowDelay;
            _fadeInTweenComponent.Duration = fadeIn;

            _fadeOutTweenComponent.Delay = showPeriod;
            _fadeOutTweenComponent.Duration = fadeOut;
        }

        public virtual void Show(HudNotificationInfo info)
        {
            _fadeInTweenComponent.Play(true);
        }
    }
}