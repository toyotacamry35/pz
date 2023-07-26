using Src.Locomotion.Delegates;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public interface ICommonStatsProvider
    {
        /// <summary>
        /// Скорость при которой переходим из состояния ходьбы в айдл
        /// </summary>
        float StandingSpeedThreshold { get; }

        /// <summary>
        /// Скорость поворота тела в сторону направления камеры в айдле (рад/сек)
        /// </summary>
        float StandingYawSpeed { get; }
        
        /// <summary>
        /// Скорость ходьбы/бега в заданном направлении относительно тела персонажа 
        /// </summary>
        SpeedByDirFn WalkingSpeed { get; }

        /// <summary>
        /// Скорость поворота тела в сторону направления камеры при ходьбе (рад/сек)
        /// </summary>
        float WalkingYawSpeed { get; }

        /// <summary>
        /// Ускорение при обычной ходьбе/беге 
        /// </summary>
        float WalkingAccel { get; }

        /// <summary>
        /// Фактор замедления движения
        /// </summary>
        float Decel { get; }

        /// <summary>
        /// Вертикальная начальная скорость прыжка 
        /// </summary>
        ImpulseByDirFn JumpVerticalImpulse { get; }

        /// <summary>
        /// Минимальная горизонтальная начальная скорость прыжка
        /// </summary>
        ImpulseByDirFn JumpHorizontalImpulse { get; }

        /// <summary>
        /// Время между нажатием на кнопку прыжка и самим прыжком на месте или с места.
        /// </summary>
        float JumpFromSpotDelay { get; }
        
        /// <summary>
        /// Скорость ходьбы при которой срабатывает прыжок с разбега вместо прыжка с места 
        /// </summary>
        float JumpFromRunSpeedThreshold { get; }

        /// <summary>
        /// Расстояние до земли при котором персонаж должен спрыгивать вниз, а не просто бежать    
        /// </summary>
        float JumpOffDistance { get; }

        /// <summary>
        /// Минимальная длительность прыжка. (To prevent exit jumping state before jump actually is happen.)
        /// </summary>
        float JumpMinDuration { get; }
        
        /// <summary>
        /// Скорость поворота тела в сторону прыжка (рад/сек)
        /// </summary>
        float JumpYawSpeed { get; }
        
        /// <summary>
        /// Время отводимое на приземление (в большинстве случаев должна равнятся анимации приземления, если таковая имеется)
        /// </summary>
        float LandingDuration { get; }

        /// <summary>
        /// Максимальная скорость управляемого движения в воздухе 
        /// </summary>
        SpeedByDirAndTimeFn AirControlSpeed { get; }

        /// <summary>
        /// Ускорение управляемого движения в воздухе 
        /// </summary>
        AccelByTimeFn AirControlAccel { get; }
        
        /// <summary>
        /// Время после спрыгивания, в течении которого можно совершать действия  
        /// </summary>
        float ActionWhileJumpOffTimeWindow { get; }
        
        /// <summary>
        /// Время до наступления возможности совершить действие в течении которого нажатие на кнопку этого действия должно срабатывать
        /// </summary>
        float ActionTriggerInHindsight { get; }

        /// <summary>
        /// 
        /// </summary>
        AccelBySlopeFn AccelBySlope { get; }
    }
}