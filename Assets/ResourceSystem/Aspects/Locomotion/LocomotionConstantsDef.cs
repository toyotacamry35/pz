
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class LocomotionConstantsDef : BaseResource
    {
        /// Ускорение свободного падения
        public float Gravity { get; set; }
        
        /// Угол наклона поверхности начиная с которого она считается вертикальной и на ней нельзя стоять (градусы)
        public float VerticalSlopeAngle { get; set; }
        
        
        /// Значение ввода при котором ходьба должна переходить в standing
        public float InputStandingThreshold { get; set; }

        /// Значение ввода при котором должно начинаться движение
        public float InputWalkThreshold { get; set; }

        /// Значение ввода при котором скорость ходьбы максимальна и далее она должна переходить в бег
        public float InputRunThreshold { get; set; }

        /// Кол-во итераций при расчёте столкновений между акторами (персонажами или мобами)
        public int ActorWithActorCollisionMaxIterations { get; set; } = 5;

        /// Смещение вдоль нормали к препятствию при расчёте столкновений между акторами (персонажами или мобами)
        public float ActorWithActorCollisionDepenetrationOffset { get; set; } = 0.01f;
    }
}