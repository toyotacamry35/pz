using DG.Tweening;

namespace Assets.Src.Lib.DOTweenAdds
{
    public interface ITweenComponent
    {
        void Play(bool isForward, float duration = 0, TweenCallback onEnd = null);
        void SetParamValue(bool isFromNorTo);
    }
}