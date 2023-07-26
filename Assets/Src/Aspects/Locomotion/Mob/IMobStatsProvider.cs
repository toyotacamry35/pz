using Src.Locomotion.Delegates;

namespace Src.Locomotion
{
    public interface IMobStatsProvider : ICommonStatsProvider
    {
        SpeedByDirFn RunningSpeed { get; }

        float RunningYawSpeed { get; }

        float RunningAccel { get; }
        
        float TowardsDirectionTolerance { get; }

        float TurnOnSpotThreshold { get; }

        float TurnOnRunThreshold { get; }
        
        /// <summary>
        /// Кривая движения уворота
        /// </summary>
        SpeedByTimeFn DodgeVelocity { get; }

        /// <summary>
        /// Длительность уворота
        /// </summary>
        float DodgeDuration { get; }

        /// <summary>
        /// Прыжок к целев.точке. Ограничение дальности min
        /// Min jump distance under conditions: dh==0 & a==45deg (i.e. conditions, providing max possible jump distance with fixed |V0| value)
        /// </summary>
        float JumpToTargetMinDistance { get; }
        /// <summary>
        /// Прыжок к целев.точке. Ограничение дальности max
        /// Max jump distance under conditions: dh==0 & a==45deg (i.e. conditionsб providing max possible jump distance with fixed |V0| value)
        /// </summary>
        float JumpToTargetMaxDistance { get; }
        /// <summary>
        /// Прыжок к целев.точке. Ограничение макс.высоты прыжка (от точки старта)
        /// Is taken into account _only_ if obstacle is taken into account (is used to avoid very high jump in attempt to overcome the obstacle)
        ///.. In other cases max-H-restriction is actually indirectly(косвенно) defined by 2 other restrictions: `jumpDistanceMin/-Max`, 
        ///.. 'cos in this case (without really obstructing obstacles (i.e. crossing trajectory)) we use angle == 45deg.
        /// </summary>
        float JumpToTargetMaxHeight { get; }

    }
}