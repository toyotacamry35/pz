#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.ColonyShared.SharedCode.Utils
{
    [CreateAssetMenu(menuName = "Curves/Curve")]
    public class Curve : ScriptableObject
    {
        [FormerlySerializedAs("_curve")] [SerializeField] public AnimationCurve curve;

        public float Evaluate(float time) => curve.Evaluate(time);

        public float FirstTime => curve.length > 0 ? curve[0].time : 0;

        public float LastTime => curve.length > 0 ? curve[curve.length - 1].time : 0;
    }
}

#else

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class Curve : UnityEngine.Object
    {
    }
}
    
#endif