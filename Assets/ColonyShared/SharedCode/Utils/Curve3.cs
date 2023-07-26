#if UNITY_5_3_OR_NEWER
using UnityEngine;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Assets.ColonyShared.SharedCode.Utils
{
    [CreateAssetMenu(menuName = "Curves/Curve3")]
    public class Curve3 : ScriptableObject
    {
        [SerializeField] public AnimationCurve curveX;
        [SerializeField] public AnimationCurve curveY;
        [SerializeField] public AnimationCurve curveZ;

        [SerializeField] public AnimationCurve _lengthByTime;
        [SerializeField] public AnimationCurve _timeByLength;
        [SerializeField] public float _minTime;
        [SerializeField] public float _maxTime;

        public float FirstTime => _minTime;

        public float LastTime => _maxTime;
    }
}

#else

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class Curve3 : UnityEngine.Object
    {
    }
}
    
#endif