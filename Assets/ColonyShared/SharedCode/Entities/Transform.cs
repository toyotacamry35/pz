using System;
using ProtoBuf;
using ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;

namespace SharedCode.Entities
{
    [ProtoContract]
    public struct Transform : IHasRandomFill
    {
        [ProtoMember(1)]
        public readonly Vector3 Position;
        [ProtoMember(2)]
        public readonly Vector3 Scale;
        [ProtoMember(3)]
        public readonly Quaternion Rotation;

        [ProtoIgnore] public Vector3 Forward => Rotation * Vector3.forward;

        [ProtoIgnore] public Vector3 Right => Rotation * Vector3.right;

        [ProtoIgnore] public Vector3 Up => Rotation * Vector3.up;
        
        public Transform(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }
        
        public Transform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = Vector3.one;
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Transform(Vector3 position, float rotationY, Vector3 scale)
        {
            Position = position;
            Rotation = Quaternion.EulerRad(0, rotationY, 0);
            Scale = scale;
        }

        public Vector3 TransformPoint(Vector3 vector)
        {
            var scaledVector = vector * Scale;
            var rotatedScaledVector = Rotation * scaledVector;
            var positionedRotatedScaledVector = Position + rotatedScaledVector;
            return positionedRotatedScaledVector;
		}
		
        public Vector3 InverseScale => new Vector3(1 / Scale.x, 1 / Scale.y, 1 / Scale.z);

        public Vector3 InverseTransformPoint(Vector3 vector)
        {
            var rotatedScaledVector = vector - Position;
            var scaledVector = Quaternion.Inverse(Rotation) * rotatedScaledVector;
            var resultingVector = scaledVector * InverseScale;
            return resultingVector;
        }

        public static explicit operator PositionRotation(Transform trans) => new PositionRotation(trans.Position, trans.Rotation);

        public Transform WithPosition(Vector3 position) => new Transform(position, Rotation, Scale); 
        
        public Transform WithRotation(Quaternion rotation) => new Transform(Position, rotation, Scale); 

        public Transform WithScale(Vector3 scale) => new Transform(Position, Rotation, scale);

#if UNITY_EDITOR || UNITY_STANDALONE
        public static explicit operator Transform(UnityEngine.Transform transform)
        {
            return new Transform((Vector3)transform.position, (Quaternion)transform.rotation, (Vector3)transform.localScale);
        }
#endif

        public override bool Equals(object obj)
        {
            if (!(obj is Transform))
                return false;
            var tr = (Transform)obj;
            return Position == tr.Position && Rotation == tr.Rotation && Scale == tr.Scale;
        }

        public override int GetHashCode()
        {
            var hashCode = -1743314642;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
            hashCode = hashCode * -1521134295 + Scale.GetHashCode();
            return  hashCode;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            Position.Fill(depthCount, random, withReadonly);
            Rotation.Fill(depthCount, random, withReadonly);
            Scale.Fill(depthCount, random, withReadonly);
        }

        public override string ToString()
        {
            var angles = Rotation.eulerAngles;
            return $"({Position.x}, {Position.y}, {Position.z}) [{angles.x}, {angles.y}, {angles.z}] <{Scale.x}, {Scale.y}, {Scale.z}>";
        }

        public static readonly Transform Identity = new Transform(Vector3.zero, Quaternion.identity, Vector3.one);
    }
}
