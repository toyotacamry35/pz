using System.Runtime.CompilerServices;
using UnityEngine;

namespace Src.Locomotion.Unity
{
    public static class VectorConvert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharedCode.Utils.Vector2 ToShared(this Vector2 vec) => new SharedCode.Utils.Vector2(vec.x, vec.y);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharedCode.Utils.Vector3 ToShared(this Vector3 vec) => new SharedCode.Utils.Vector3(vec.x, vec.y, vec.z);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SharedCode.Utils.Quaternion ToShared(this in Quaternion q) => new SharedCode.Utils.Quaternion(q.x, q.y, q.z, q.w);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToUnity(this SharedCode.Utils.Vector2 vec) => new Vector2(vec.x, vec.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToUnity(this SharedCode.Utils.Vector3 vec) =>  new Vector3(vec.x, vec.y, vec.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion ToUnity(this in SharedCode.Utils.Quaternion q) =>  new Quaternion(q.x, q.y, q.z, q.w);

        // #Note: high load method - should be optimized as possible. 
        // @param `inVec` - passed by ref only for optimization (avoid struct copying).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocomotionToWorldVectorAndToUnity(ref LocomotionVector inVec) => new Vector3(-inVec.Horizontal.y, inVec.Vertical, inVec.Horizontal.x);

        // #Note: high load method - should be optimized as possible. 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LocomotionToWorldOrientationAndToUnity(float angle)
        {
            angle *= -0.5f;
            return new Quaternion(0, Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LocomotionVector ToLocomotion(this in Vector3 vec) =>  LocomotionHelpers.WorldToLocomotionVector(vec);
    }
}