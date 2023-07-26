using UnityEngine;

namespace Uins
{
    public interface IPositionSetter
    {
        void SetPosition(Vector2 vector2, RectTransform positionedRectTransform);
        float GridPitch { get; }
    }
}