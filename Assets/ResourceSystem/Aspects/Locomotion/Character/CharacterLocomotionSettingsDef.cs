using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    /// <summary>
    /// Константные параметры перемещения субъекта 
    /// </summary>
    public class CharacterLocomotionSettingsDef : BaseResource
    {
        /// Ускорение при обычной ходьбе/беге
        public float WalkingAccel { get; set; }

        /// Ускорение в спринте
        public float SprintAccel { get; set; }

        /// Фактор замедления при перемещении
        public float Decel { get; set; }

        /// Коллайдер персонажа колизящийся с землёй
        public float SingleStepTime { get; set; }

        /// Порог скорости для перехода в состояние стояния
        public float StandingSpeedThreshold { get; set; }

        /// Максимальный угол наклона поверхности (включительно) по которой можно ходить (градусы)
        public float MaxWalkingSlope { get; set; }
        
        /// Множитель ускорения при движении в гору в зависимости от угла наклона поверхности.
        /// Угол наклона задаётся как нормированная величина, где 0 - горизонтальная поверхность, 1 - MaxWalkingSlope
        public UnityRef<Curve> AccelBySlope { get; set; }
        
        /// Минимальный угол наклона поверхности при котором возникает скольжение (градусы)
        public float SlipSlope { get; set; }
        
        /// Зависимость максимальной скорости скольжения от угла наклона поверхности.
        /// Угол наклона задаётся как нормированная величина, где 0 - SlipSlope, 1 - 90°
        public UnityRef<Curve> SlipSpeed { get; set; }

        /// Зависимость ускорения скольжения от угла наклона поверхности.
        /// Угол наклона задаётся как нормированная величина, где 0 - SlipSlope, 1 - 90°
        public UnityRef<Curve> SlipAccel { get; set; }

        /// Скорость при которой скольжение переходит в стейт неконтролируемого скольжения
        public float SlippingStartSpeedThreshold { get; set; }

        /// "Скорость при которой неконтролируемое скольжение заканчивается
        public float SlippingStopSpeedThreshold { get; set; }
        
        /// Время через которое скольжение переходит в стейт неконтролируемого скольжения
        public float SlippingTimeThreshold { get; set; }

        /// Ускорение контролируемого перемещения (подруливания) в состоянии скольжения
        public float SlippingAccel { get; set; }

        /// Фактор замедления при неконтролируемом скольжении
        public float SlippingDecel { get; set; }

        /// Расстояние до земли при котором персонаж должен спрыгивать вниз, а не просто бежать
        public float JumpingOffDistance { get; set; }

        /// Время отводимое на приземление (в большинстве случаев должно равняться анимации приземления, если таковая имеется)
        public float LandingDuration { get; set; }

        /// Зависимость ускорения контролируемого полёта в зависимости от времени полёта
        public UnityRef<Curve> AirControlAccel { get; set; }

        /// Время на которое персонаж должен останавливаться после жёсткого приземления
        public float HardLandingStunTime { get; set; }
        
        /// Задержка прыжка на месте
        public float JumpFromSpotDelay { get; set; }

        /// Скорость ходьбы при которой срабатывает прыжок с разбега вместо прыжка с места 
        public float JumpFromRunSpeedThreshold { get; set; }

        /// Время после спрыгивания, в течении которого ещё можно совершать другие действия (прыгать, дешится и т п)
        public float ActionWhileJumpOffTimeWindow { get; set; }

        /// Время до наступления возможности совершить действие в течении которого нажатие на кнопку этого действия должно срабатывать
        public float ActionTriggerInHindsight { get; set; }
        
        /// Минимальное время отсутствия земли под ногами, через которое считаем, что мы в воздухе
        public float MinAirborneTime { get; set; }

        /// Сглаживание изменения направления движения для аниматора
        public float AnimatorDirectionSmoothness { get; set; }

        /// Сглаживание изменения скорости движения для аниматора
        public float AnimatorSpeedSmoothness { get; set; }

        /// Порог скорости ниже которого считается что персонаж стоит
        public float AnimatorMotionThreshold { get; set; }
        
        /// Базовая скорость читового перемещения
        public float CheatSpeed = 10;

        // Максимальная допустимая линейная горизонтальная скорость 
        public float HorizontalSpeedLimit { get; set; } = 50;        

        // Максимальная допустимая линейная вертикальная скорость 
        public float VerticalSpeedLimit { get; set; } = 50;        

        /// Расстояние до земли во время атаки-с-воздуха при котором активируется ударная фаза атаки 
        public float AirborneAttackHitDistance { get; set; }
        
        /// Максимальная скорость управляемого движения во время атаки-с-воздуха 
        public float AirborneAttackControlSpeed { get; set; }
        
        /// Функция ускорения управляемого движения во время атаки-с-воздуха от времени с начала атаки 
        public UnityRef<Curve> AirborneAttackControlAccel { get; set; }

        /// Максимальный угол между точкой удара и направлением движения, при котором срабатывает прилипание (градусы) 
        public float StickHitAngle { get; set; } = 90;

        /// Ускорение скатывания с голов персонажей и мобов
        public float SlideDownFromActorsAccel { get; set; } = 10;
        
        /// Максимальная скорость скатывания с голов персонажей и мобов
        public float SlideDownFromActorsSpeed { get; set; } = 10;
    }
}
