namespace Src.Locomotion
{
    public interface ILocomotionGroundSensor
    {
        /// <summary>
        /// True если касаемся земли
        /// </summary>
        bool OnGround { get; }
        
        /// <summary>
        /// Нормаль в точке контакта если стоим на земле или в точке под персонажем 
        /// </summary>
        LocomotionVector GroundNormal { get; }

        /// <summary>
        /// Расстояние до земли. Может быть отрицательным, если точка контакта выше нижней точки персонажа.
        /// </summary>
        float DistanceToGround { get; }
        
        /// <summary>
        /// Есть непосредственный контакт с землёй
        /// </summary>
        bool HasGroundContact { get; }
    }
}
