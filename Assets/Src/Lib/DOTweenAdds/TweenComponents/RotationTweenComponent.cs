using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class RotationTweenComponent : TweenComponentBase
    {
        public RectTransform RectTransform;
        
        //=== Props ===========================================================

        protected override float Parameter
        {
            get { return RectTransform.eulerAngles.z; }
            set { RectTransform.eulerAngles = RectTransform.eulerAngles.SetZ(value); }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            RectTransform.AssertIfNull(nameof(RectTransform));
        }
    }
}