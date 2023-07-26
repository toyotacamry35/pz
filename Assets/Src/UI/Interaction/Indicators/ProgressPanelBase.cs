using DG.Tweening;
using UnityEngine;

namespace Uins
{
    public abstract class ProgressPanelBase : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        protected InteractionIndicator RelatedIndicator;

        public abstract void HideAndReset();

        public abstract void HideAllResources();

        public abstract void SetupResources(InteractionIndicator interactionIndicator);

        public abstract void HideResourcesButOne();

        public abstract void BringOneResourceToCenterAndThenHideIt(TweenCallback onEnd);
    }
}