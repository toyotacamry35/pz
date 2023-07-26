using Src.Animation;

namespace Src.Aspects.Doings
{
    public delegate void AnimationPlayStartedDelegate(in AnimationPlayInfo nfo);

    public interface IAnimationPlayProvider
    {
        event AnimationPlayStartedDelegate AnimationPlayStarted;
    }
}