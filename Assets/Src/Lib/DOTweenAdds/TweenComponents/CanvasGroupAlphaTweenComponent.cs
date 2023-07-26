using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class CanvasGroupAlphaTweenComponent : TweenComponentBase
    {
        public CanvasGroup CanvasGroup;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get => CanvasGroup.alpha;
            set => CanvasGroup.alpha = value;
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            CanvasGroup.AssertIfNull(nameof(CanvasGroup));
        }
    }
}