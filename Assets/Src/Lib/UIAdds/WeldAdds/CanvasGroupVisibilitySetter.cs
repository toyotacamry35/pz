using UnityEngine;

namespace WeldAdds
{
    public class CanvasGroupVisibilitySetter : SomethingVisibilitySetter
    {
        public CanvasGroup CanvasGroup;

        public bool IsAlphaAffects = true;

        public bool IsInteractableAffects;

        public bool IsBlocksRaycastsAffects;


        //=== Protected =======================================================

        protected override bool OnAwakeSelfCheck()
        {
            return !CanvasGroup.AssertIfNull(nameof(CanvasGroup));
        }

        protected override void OnVisibilityChanged(bool isVisible)
        {
            if (IsAlphaAffects)
                CanvasGroup.alpha = isVisible ? OnAlpha : OffAlpha;
            if (IsInteractableAffects)
                CanvasGroup.interactable = isVisible;
            if (IsBlocksRaycastsAffects)
                CanvasGroup.blocksRaycasts = isVisible;
        }
    }
}