using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class PositionTweenComponent : TweenComponentBase
    {
        public RectTransform RectTransform;

        public RectTransformParam TweeningParam;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get
            {
                switch (TweeningParam)
                {
                    case RectTransformParam.AnchoredPosX:
                        return RectTransform.anchoredPosition.x;

                    case RectTransformParam.AnchoredPosY:
                        return RectTransform.anchoredPosition.y;

                    case RectTransformParam.SizeDeltaX:
                        return RectTransform.sizeDelta.x;

                    default:
                        return RectTransform.sizeDelta.y;
                }
            }

            set
            {
                switch (TweeningParam)
                {
                    case RectTransformParam.AnchoredPosX:
                        RectTransform.anchoredPosition = new Vector2(value, RectTransform.anchoredPosition.y);
                        break;

                    case RectTransformParam.AnchoredPosY:
                        RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, value);
                        break;

                    case RectTransformParam.SizeDeltaX:
                        RectTransform.sizeDelta = new Vector2(value, RectTransform.sizeDelta.y);
                        break;

                    default:
                        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, value);
                        break;
                }
            }
        }


        //=== Enum ============================================================

        public enum RectTransformParam
        {
            AnchoredPosX,
            AnchoredPosY,
            SizeDeltaX,
            SizeDeltaY,
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            RectTransform.AssertIfNull(nameof(RectTransform));
        }
    }
}