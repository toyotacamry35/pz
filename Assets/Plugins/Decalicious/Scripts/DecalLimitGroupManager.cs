using System;
using UnityEngine;

namespace ThreeEyedGames
{
    [ExecuteInEditMode]
    [AddComponentMenu("Decalicious/Decal Limit Group Manager")]
    public class DecalLimitGroupManager : MonoBehaviour
    {
        public static DecalLimitGroupManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("There cannot be more than one instance of DecalLimitGroupManager. Destroying.");
                DestroyImmediate(this);
            }
        }

        private void OnEnable()
        {
            if (LimitGroups.Length != NumLimitGroups)
                Array.Resize(ref LimitGroups, NumLimitGroups);
        }

#if DECALICIOUS_MORE_LIMIT_GROUPS
        public const int NumLimitGroups = 128;
#else
        public const int NumLimitGroups = 32;
#endif

        public DecalLimitGroup[] LimitGroups = new DecalLimitGroup[NumLimitGroups];
    }
}