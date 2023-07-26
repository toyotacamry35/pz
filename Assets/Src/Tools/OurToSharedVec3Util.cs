namespace Assets.Src.Tools
{
    public static class UnityToShared
    {
        public static SharedCode.Utils.Vector3 ToSharedVec3(this UnityEngine.Vector3 vec)
        {
            return new SharedCode.Utils.Vector3() { x = vec.x, y = vec.y, z = vec.z };
        }
        public static SharedCode.Utils.Quaternion ToSharedQ(this UnityEngine.Quaternion q)
        {
            return new SharedCode.Utils.Quaternion() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

    }
}