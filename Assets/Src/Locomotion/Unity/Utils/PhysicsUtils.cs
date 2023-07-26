using System;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Src.Locomotion.Unity
{
    public static class PhysicsUtils
    {
        public static int OverlapColliderNonAlloc(Collider collider, Vector3 position, Quaternion rotation, Collider[] results, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            switch (collider)
            {
                case CharacterController capsule:
                {
                    var center = rotation * capsule.center + position;
                    var offset = rotation * Vector3.up * (capsule.height / 2 - capsule.radius);
                    return Physics.OverlapCapsuleNonAlloc(center - offset, center + offset, capsule.radius, results, layerMask, queryTriggerInteraction);
                }
                case CapsuleCollider capsule:
                {
                    var center = rotation * capsule.center + position;
                    var offset = rotation * GetCapsuleAxis(capsule) * (capsule.height / 2 - capsule.radius);
                    return Physics.OverlapCapsuleNonAlloc(center - offset, center + offset, capsule.radius, results, layerMask, queryTriggerInteraction);
                }
                case SphereCollider sphere:
                {
                    var center = rotation * sphere.center + position;
                    return Physics.OverlapSphereNonAlloc(center, sphere.radius, results, layerMask, queryTriggerInteraction);
                }
                case BoxCollider box:
                {
                    var center = rotation * box.center + position;
                    return Physics.OverlapBoxNonAlloc(center, box.size * 0.5f, results, rotation, layerMask, queryTriggerInteraction);
                }
                case null:
                    throw new ArgumentNullException(nameof(collider));
                default:
                    throw new NotSupportedException($"{collider.GetType()}");
            }
        }
        
        
        public static int CastColliderNonAlloc(Collider collider, Vector3 position, Quaternion rotation, Vector3 direction, float maxDistance, RaycastHit[] results, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, float inflation = 0)
        {
            switch (collider)
            {
                case CharacterController capsule:
                {
                    var radius = Max(capsule.radius + inflation, 0);
                    var height = Max(capsule.height + inflation * 2, 0);
                    var center = rotation * capsule.center + position;
                    var offset = rotation * Vector3.up * (height / 2 - radius);
                    return Physics.CapsuleCastNonAlloc(center - offset, center + offset, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
                }
                case CapsuleCollider capsule:
                {
                    var radius = Max(capsule.radius + inflation, 0);
                    var height = Max(capsule.height + inflation * 2, 0);
                    var center = rotation * capsule.center + position;
                    var offset = rotation * GetCapsuleAxis(capsule) * (height / 2 - radius);
                    return Physics.CapsuleCastNonAlloc(center - offset, center + offset, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
                }
                case SphereCollider sphere:
                {
                    var center = rotation * sphere.center + position;
                    return Physics.SphereCastNonAlloc(center, Max(sphere.radius + inflation, 0), direction, results, maxDistance, layerMask, queryTriggerInteraction);
                }
                case BoxCollider box:
                {
                    var center = rotation * box.center + position;
                    var extends = new Vector3(Max(0.5f * box.size.x + inflation, 0), Max(0.5f * box.size.y + inflation, 0), Max(0.5f * box.size.z + inflation, 0));
                    return Physics.BoxCastNonAlloc(center, extends, direction, results, rotation, maxDistance, layerMask, queryTriggerInteraction);
                }
                case null:
                    throw new ArgumentNullException(nameof(collider));
                default:
                    throw new NotSupportedException($"{collider.GetType()}");
            }
        }
        
        public static Vector3 GetCapsuleAxis(CapsuleCollider capsule)
        {
            switch (capsule.direction)
            {
                case 0: return Vector3.right;
                case 1: return Vector3.up;
                case 2: return Vector3.forward;
            }
            return Vector3.up;
        }

        public static bool IsSameObject(Collider c1, Collider c2)
        {
            return c1 && c2 && (c1 == c2 || c1.transform.root == c2.transform.root);
        }
        
        public static bool IsSameObject(Transform c1, Transform c2)
        {
            return c1 && c2 && (c1 == c2 || c1.root == c2.root);
        }
    }
}