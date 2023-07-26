using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    /// <summary>
    /// Константные параметры перемещения субъекта 
    /// </summary>
    public class MobLocomotionSettingsDef : BaseResource
    {
        /// Допуск угла между текущим направлением моба и направлением guide, при котором считаем, что эти направления совпадают
        public float TowardsDirectionTolerance { get; protected set; }

        /// Угол между forward и guide, при превышении которого включается стейт поворота на месте  
        public float TurnOnSpotThreshold { get; protected set; }

        /// Угол между forward и guide, при превышении которого включается стейт разворота на бегу  
        public float TurnOnRunThreshold { get; protected set; }
        
        /// Порог скорости для перехода в состояние стояния
        public float StandingSpeedThreshold { get; set; }
        
        /// Кривая скорости уворота
        public UnityRef<Curve> DodgeMotion { get; set; }
        
        /// Расстояние до земли при котором персонаж должен спрыгивать вниз, а не просто бежать
        public float JumpingOffDistance { get; set; }

        /// Время отводимое на приземление (в большинстве случаев должна равнятся анимации приземления, если таковая имеется)
        public float LandingDuration { get; set; }

        /// Зависимость ускорения контролируемого полёта в зависимости от времени полёта
        public UnityRef<Curve> AirControlAccel { get; set; }

        /// Задержка прыжка на месте
        public float JumpFromSpotDelay { get; set; }

        /// Скорость ходьбы при которой срабатывает прыжок с разбега вместо прыжка с места 
        public float JumpFromRunSpeedThreshold { get; set; }

        /// Время после спрыгивания, в течении которого можно совершать действия
        public float ActionWhileJumpOffTimeWindow { get; set; }

        /// Время до наступления возможности совершить действие в течении которого нажатие на кнопку этого действия должно срабатывать
        public float ActionTriggerInHindsight { get; set; }
        
        /// Минимальное время отсутствия земли под ногами, через которое считаем, что мы в воздухе
        public float MinAirborneTime { get; set; }

        /// Минимальное время длительности прыжка
        public float JumpMinDuration { get; set; }
        
        /// Сглаживание изменения направления движения для аниматора
        public float AnimatorDirectionSmoothness { get; set; }

        /// Сглаживание изменения скорости движения для аниматора
        public float AnimatorSpeedSmoothness { get; set; }

        /// Сглаживание изменения угловой скорости для аниматора
        public float AnimatorAngularVelocitySmoothness = 1;

        /// Скорость вращения при которой изгиб тела должен быть максимальным
        public float AnimatorAngularVelocityForMaxTwist = 100;
        
        /// Порог скорости ниже которого считается что персонаж стоит
        public float AnimatorMotionThreshold { get; set; }
    }
}
