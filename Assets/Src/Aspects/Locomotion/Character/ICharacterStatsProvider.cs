using Src.Locomotion.Delegates;

namespace Src.Locomotion
{
    public interface ICharacterStatsProvider : ICommonStatsProvider
    {
        /// <summary>
        /// Скорость спринта в заданном направлении относительно тела персонажа
        /// </summary>
        SpeedByDirFn SprintSpeed { get; }

        /// <summary>
        /// Скорость поворота тела в сторону направления камеры при спринте (рад/сек)
        /// </summary>
        float SprintYawSpeed { get; }
        
        /// <summary>
        /// Ускорение в спринте 
        /// </summary>
        float SprintAccel { get; }
        
        /// <summary>
        /// Скорость ходьбы в блоке в заданном направлении относительно тела персонажа 
        /// </summary>
        SpeedByDirFn BlockingSpeed { get; }

        /// <summary>
        /// Время на которое персонаж должен останавливаться после жёсткого приземления
        /// </summary>
        float HardLandingStunTime { get; }

        /// <summary>
        /// Зависимость максимальной скорости скольжения от угла наклона поверхности 
        /// </summary>
        SlipSpeedBySlopFn SlipSpeed { get; }
        
        /// <summary>
        /// Скорость поворота тела в сторону движения при скольжении (рад/сек)
        /// </summary>
        float SlipYawSpeed { get; }

        /// <summary>
        /// Зависимость ускорения скольжения от угла наклона поверхности 
        /// </summary>
        SlipAccelBySlopeFn SlipAccel { get; }

        /// <summary>
        /// Скорость при которой переходим в состояние скольжения
        /// </summary>
        float SlippingStartSpeedThreshold { get; }

        /// <summary>
        /// Скорость при которой выходим из состояния скольжения
        /// </summary>
        float SlippingStopSpeedThreshold { get; }

        /// <summary>
        /// Синус угла наклона поверхности при котором возможен переход в состояние скольжения  
        /// </summary>
        float SlippingSlopeFactorThreshold { get; }

        /// <summary>
        /// Минимальное время скольжения со скоростью SlippingStartSpeedThreshold после которого происходит переход в состояние скольжения
        /// </summary>
        float SlippingTimeThreshold { get; }
        
        /// <summary>
        /// Скорость контролируемого перемещения в состоянии скольжения в заданном направлении относительно тела персонажа
        /// </summary>
        SpeedByDirFn SlippingSpeed { get; }
       
        /// <summary>
        /// Ускорение контролируемого перемещения в состоянии скольжения  
        /// </summary>
        float SlippingAccel { get; }

        /// <summary>
        /// Фактор замедления в состоянии скольжения 
        /// </summary>
        float SlippingDecel { get; }
        
        /// <summary>
        /// Минимальное время ходьбы.    
        /// </summary>
        float SingleStepTime { get; }
        
        /// Базовая скорость читового перемещения 
        /// </summary>
        float CheatSpeed { get; }
        
        /// <summary>
        /// Максимальный угол наклона поверхности (включительно) по которой можно ходить (радианы)
        /// </summary>
        float MaxWalkingSlope { get; }

        /// <summary>
        /// Максимальная скорость управляемого движения в воздухе во время атаки-с-воздуха 
        /// </summary>
        SpeedByDirAndTimeFn AirborneAttackControlSpeed { get; }

        /// <summary>
        /// Ускорение управляемого движения в воздухе во время атаки-с-воздуха
        /// </summary>
        AccelByTimeFn AirborneAttackControlAccel { get; }
		
        /// <summary>
        /// Расстояние до земли когда должна срабатывать ударная фаза атаки с воздуха 
        /// </summary>
        float AirborneAttackHitDistance { get; }
        /// <summary>
        /// Максимальный угол между точкой удара и направлением движения, при котором срабатывает прилипание (радианы) 
        /// </summary>
        float StickHitAngle { get; }
    }
}