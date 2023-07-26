using UnityEngine;

public static class FromUnityToSharedVector
{
    public static SharedCode.Utils.Vector2 ToShared(this Vector2 vec)
    {
        return new SharedCode.Utils.Vector2(vec.x, vec.y);
    }
    public static SharedCode.Utils.Vector3 ToShared(this Vector3 vec)
    {
        return new SharedCode.Utils.Vector3(vec.x, vec.y, vec.z);
    }
}