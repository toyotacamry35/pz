#if UNITY_5_3_OR_NEWER
using UnityEngine;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using Quaternion = SharedCode.Utils.Quaternion;

namespace Assets.ColonyShared.SharedCode.Utils
{
    [CreateAssetMenu(menuName = "Curves/CurveQ")]
    public class CurveQ : ScriptableObject
    {
        [SerializeField] public AnimationCurve curveX;
        [SerializeField] public AnimationCurve curveY;
        [SerializeField] public AnimationCurve curveZ;
        [SerializeField] public AnimationCurve curveW;
        
        public float FirstTime => Min(curveX.length > 0 ? curveX[0].time : 0, Min(curveY.length > 0 ? curveY[0].time : 0, Min(curveZ.length > 0 ? curveZ[0].time : 0, curveW.length > 0 ? curveW[0].time : 0)));

        public float LastTime => Max(curveX.length > 0 ? curveX[curveX.length - 1].time : 0, Min(curveY.length > 0 ? curveY[curveY.length - 1].time : 0, Min(curveZ.length > 0 ? curveZ[curveZ.length - 1].time : 0, curveW.length > 0 ? curveW[curveW.length - 1].time : 0)));
    }
}

#else

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class CurveQ : UnityEngine.Object
    {
    }
}
    
#endif
