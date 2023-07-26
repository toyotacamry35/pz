using System.Collections.Generic;

namespace Src.Locomotion
{
    public interface ILocomotionCollider
    {
        /// <summary>
        /// Мировые координаты нижней точки коллайдера
        /// </summary>
        LocomotionVector OriginPoint { get; }

        /// <summary>
        /// Смещение нижней точки коллайдера относительно пивота GO
        /// </summary>
        float OriginOffset { get; }
        
        /// <summary>
        /// Радиус коллайдера
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Слои которые считаются землёй
        /// </summary>
        int GroundLayerMask { get; }

        /// <summary>
        /// Все контакты с землёй или другими акторами
        /// </summary>
        List<ContactPoint> Contacts { get; }
        
        /// <summary>
        /// 
        /// </summary>
        bool IsSame(object collider);
    }
}
