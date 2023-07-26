using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public interface IMoveActionPosition
    {
        bool Valid { get; }
        Vector3 Position { get; }
        bool IsSameObject(IMoveActionPosition other);
        bool IsSameObject(GameObject other);
    }

    public static class MoveActionPositionExtension
    {
        public static void DebugDrawTargetPosition(this IMoveActionPosition target, Color color, float duration = 1f)
        {
            DebugExtension.DebugPoint(target.Position, duration: duration, color: color);
        }
    }
}