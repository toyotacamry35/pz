using Assets.Src.Lib.Extensions;
using UnityEngine;

namespace Assets.Src.Aspects.Impl
{
    public struct PositionRotation
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsValid;

        public static PositionRotation InvalidInstatnce = new PositionRotation(Vector3.zero, Quaternion.identity, true);

        public PositionRotation(Vector3 pos, Quaternion rot, bool isValid = true)
        {
            Position = pos;
            Rotation = rot;
            IsValid = isValid;
        }

        // --- Convertets: ---------------------------------------------------------

        public static PositionRotation FromShared(SharedCode.Entities.GameObjectEntities.PositionRotation shared)
        {
            return new PositionRotation(shared.Position.ToUnityVector3(), shared.Rotation.ToUnityQuaternion());
        }

        public SharedCode.Entities.GameObjectEntities.PositionRotation ToShared(PositionRotation shared)
        {
            return new SharedCode.Entities.GameObjectEntities.PositionRotation(Position.ToShared(), Rotation.ToSharedQuaternion());
        }

        public static PositionRotation Lerp(PositionRotation p1, PositionRotation p2, float t)
        {
            return new PositionRotation(
                Vector3.Lerp(p1.Position, p2.Position, t),
                Quaternion.Lerp(p1.Rotation, p2.Rotation, t)
            );
        }

        public static PositionRotation Slerp(PositionRotation p1, PositionRotation p2, float t)
        {
            return new PositionRotation(
                Vector3.Lerp(p1.Position, p2.Position, t),
                Quaternion.Slerp(p1.Rotation, p2.Rotation, t)
            );
        }

        public Vector3 TransformPoint(Vector3 pt)
        {
            return Position + Rotation * pt;
        }
        
        public Vector3 TransformDirection(Vector3 pt)
        {
            return Rotation * pt;
        }
    }
}