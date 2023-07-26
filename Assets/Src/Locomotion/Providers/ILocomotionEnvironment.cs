using System.Collections.Generic;
using SharedCode.Utils;
using Src.Locomotion.Delegates;

namespace Src.Locomotion
{
    public interface ILocomotionEnvironment
    {
        /// <summary>
        /// Оторвался от земли
        /// </summary>
        bool Airborne { get; }
         
        /// <summary>
        /// Расстояние до земли от нижней точки коллайдера. Отрицательное, если земля вдруг оказалась выше точки отсчёта.
        /// </summary>
        float DistanceToGround { get; }

        /// <summary>
        /// Есть непосредственный контакт с землёй (если его нет, то это не значит, что не стоим на земле!)
        /// </summary>
        bool HasGroundContact { get; }

        /// <summary>
        /// Направление наклона земли под ногами (направление спуска) 
        /// </summary>
        Vector2 SlopeDirection { get; }

        /// <summary>
        /// Синус угла наклона нормали поверхности "под ногами" относительно мировой вертикали в направлении SlopeDirection.
        /// 0 - поверхность горизонтальна, 1 - поверхность вертикальна.
        /// </summary>
        float SlopeFactor();

        /// <summary>
        /// Синус угла наклона поверхности "под ногами" относительно мировой вертикали в заданном направлении.
        /// Положительное значение - спуск, отрицательное - подъём. 
        /// </summary>
        SlopeByDirFn SlopeFactorAlongDirection { get; }

        /// <summary>
        /// Ускорение свободного падения
        /// </summary>
        float Gravity { get; }
        
        /// <summary>
        /// Окружение находится в валидном состоянии 
        /// </summary>
        bool Valid { get; }
        
        /// <summary>
        /// Контакты с другими объектами/землёй
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ContactPoint> Contacts { get; } 
        
        /// <summary>
        /// Смещение нижней точки коллайдера относительно пивота GO персонажа. (+ вверх, - вниз)
        /// </summary>
        float ColliderOffset { get; }
    }
}
