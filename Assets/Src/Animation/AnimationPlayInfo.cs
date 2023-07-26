using ResourceSystem.Aspects.Misc;

namespace Src.Animation
{
    public readonly struct AnimationPlayInfo
    {
        public readonly object PlayId;
        public readonly AnimationStateDef StateDef;
        public readonly long StartTime;
        public readonly float AnimationOffset;
        public readonly float SpeedFactor;

        public AnimationPlayInfo(object playId, AnimationStateDef stateDef, long startTime, float animationOffset, float speedFactor)
        {
            PlayId = playId;
            StateDef = stateDef;
            StartTime = startTime;
            AnimationOffset = animationOffset;
            SpeedFactor = speedFactor;
        }
    }
}