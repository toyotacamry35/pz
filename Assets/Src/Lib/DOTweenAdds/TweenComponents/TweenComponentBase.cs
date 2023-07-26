using System;
using DG.Tweening;
using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    public abstract class TweenComponentBase : MonoBehaviour, ITweenComponent
    {
        [Range(0, 100)]
        public float Delay;

        public float From;

        public float To;

        public Ease Ease;

        [Range(0.1f, 100)]
        public float Duration;

        public bool AlwaysDoReset;

        public TweenComponentBase[] NextTweenComponents;

        private Tweener _tweener;


        //=== Props ===========================================================

        protected abstract float Parameter { get; set; }


        //=== Public ==========================================================

        public void Play(bool isForward, float duration = 0, TweenCallback onEnd = null)
        {
            if (_tweener == null || AlwaysDoReset)
            {
                _tweener.KillIfExistsAndActive();
                _tweener = GetParamTweener(isForward, duration, onEnd);
            }

            if (isForward)
            {
                _tweener.Restart();
            }
            else
            {
                _tweener.Goto((duration > 0 ? duration : Duration) + Delay); //переходим в конец по времени, при этом не завершаем
                _tweener.PlayBackwards();
            }
        }

        public void SetParamValue(bool isFromNorTo)
        {
            Parameter = isFromNorTo ? From : To;

            if (_tweener != null && _tweener.IsPlaying())
                _tweener.Pause();
        }


        //=== Protected =======================================================

        protected Tweener GetParamTweener(bool isForward, float duration, TweenCallback onEnd)
        {
            Parameter = From;
            var tweener = DOTween.To(
                    () => Parameter,
                    f => Parameter = f,
                    To,
                    duration > 0 ? duration : Duration)
                .SetEase(Ease);

            if (!Mathf.Approximately(0, Delay))
                tweener.SetDelay(Delay);

            if (onEnd != null)
                tweener.OnComplete(onEnd);
            else
            {
                if (NextTweenComponents != null && NextTweenComponents.Length > 0)
                    tweener.OnComplete(() =>
                        {
                            foreach (var tweenComponent in NextTweenComponents)
                                tweenComponent?.Play(isForward, duration);
                        }
                    );
            }

            if (!AlwaysDoReset)
                tweener.SetAutoKill(false);

            return tweener;
        }
    }
}