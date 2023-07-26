using UnityEngine;

using SharedQuaternion = SharedCode.Utils.Quaternion;

namespace Assets.Src.Lib.Extensions
{
    public static class SharedQuaternionExtension
    {
        public static Quaternion ToUnityQuaternion(this SharedQuaternion qShared) => new Quaternion(qShared.x, qShared.y, qShared.z, qShared.w);
    }

    public static class UnityQuaternionExtension
    {
        public static SharedQuaternion ToSharedQuaternion(this Quaternion qUnity) => new SharedQuaternion(qUnity.x, qUnity.y, qUnity.z, qUnity.w);
    }
}
