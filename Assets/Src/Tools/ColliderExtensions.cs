using UnityEngine;

namespace Src.Tools
{
    public static class ColliderExtensions
    {
        public static Vector3 GetSize(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sphereCollider:
                {
                    var sphere = sphereCollider;
                    var diam = sphere.radius * 2;
                    return new Vector3(diam, diam, diam);
                }

                case CapsuleCollider capsuleCollider:
                {
                    var capsule = capsuleCollider;
                    var diam = capsule.radius * 2;
                    switch (capsule.direction)
                    {
                        case 0:
                            return new Vector3(capsule.height, diam, diam);
                        case 1:
                            return new Vector3(diam, capsule.height, diam);
                        case 2:
                            return new Vector3(diam, diam, capsule.height);
                    }
                    break;
                }

                case CharacterController controller:
                {
                    var capsule = controller;
                    var diam = capsule.radius * 2;
                    return new Vector3(diam, capsule.height, diam);
                }

                case BoxCollider boxCollider:
                    return boxCollider.size;
            }

            return collider.bounds.size;
        }

        public static Vector3 GetCenter(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sphereCollider:
                    return sphereCollider.center;
                case CapsuleCollider capsuleCollider:
                    return capsuleCollider.center;
                case BoxCollider boxCollider:
                    return boxCollider.center;
                case CharacterController controller:
                    return controller.center;
                default:
                    return Vector3.zero;
            }
        }
        
        public static Vector3 GetCapsuleAxis(this CapsuleCollider capsule)
        {
            switch (capsule.direction)
            {
                case 0: return Vector3.right;
                case 1: return Vector3.up;
                case 2: return Vector3.forward;
            }

            return Vector3.up;
        }
        
        public static Vector3 GetCapsuleSides(this CapsuleCollider capsule)
        {
            switch (capsule.direction)
            {
                case 0: return Vector3.up + Vector3.forward;
                case 1: return Vector3.right + Vector3.forward;
                case 2: return Vector3.up + Vector3.right;
            }

            return Vector3.zero;
        }
    }
}