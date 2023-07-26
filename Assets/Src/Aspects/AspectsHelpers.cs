using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using UnityEngine;


namespace Assets.Src.Aspects
{
    public static class AspectsHelpers
    {
        public static T Kind<T>([CanBeNull] this GameObject go) where T : MonoBehaviour
        {
            return go ? go.GetComponent<T>() : null;
        }
        public static bool Is<T>([CanBeNull] this GameObject go) where T : MonoBehaviour
        {
            if (go != null)
                return go.GetComponent<T>() != null;
            return false;
        }

        public static bool InRangeOf([NotNull] this GameObject go, [NotNull] GameObject otherGo, float range)
        {
            return Vector3.Distance(go.transform.position, otherGo.transform.position) <= range;
        }

        public static bool Expired(this long startTime, long duration)
        {
            return SyncTime.Now - startTime > duration;
        }
    }
}
