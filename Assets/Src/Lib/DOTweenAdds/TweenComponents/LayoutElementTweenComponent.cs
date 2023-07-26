using UnityEngine.UI;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class LayoutElementTweenComponent : TweenComponentBase
    {
        public LayoutElement LayoutElement;

        public LayoutElementParam TweeningParam;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get
            {
                switch (TweeningParam)
                {
                    case LayoutElementParam.MinWidth:
                        return LayoutElement.minWidth;

                    case LayoutElementParam.MinHeight:
                        return LayoutElement.minHeight;

                    case LayoutElementParam.PrefferedWidth:
                        return LayoutElement.preferredWidth;

                    default:
                        return LayoutElement.preferredHeight;
                }
            }

            set
            {
                switch (TweeningParam)
                {
                    case LayoutElementParam.MinWidth:
                        LayoutElement.minWidth = value;
                        break;

                    case LayoutElementParam.MinHeight:
                        LayoutElement.minHeight = value;
                        break;

                    case LayoutElementParam.PrefferedWidth:
                        LayoutElement.preferredWidth = value;
                        break;

                    default:
                        LayoutElement.preferredHeight = value;
                        break;
                }
            }
        }


        //=== Enum ============================================================

        public enum LayoutElementParam
        {
            MinWidth,
            MinHeight,
            PrefferedWidth,
            PrefferedHeight,
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            LayoutElement.AssertIfNull(nameof(LayoutElement));
        }
    }
}