using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ProgressPanelOneItem : ProgressPanelBase
    {
        public Image[] ResourceImages;

        protected int LastAchievedItemIndex;

        private Sequence _sequence;


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            if (!CanvasGroup.AssertIfNull(nameof(CanvasGroup)))
                CanvasGroup.alpha = 0;

            ResourceImages.IsNullOrEmptyOrHasNullElements(nameof(ResourceImages));
            transform.localPosition = Vector3.zero;
        }


        //=== Public ==========================================================

        public override void SetupResources(InteractionIndicator interactionIndicator)
        {
            RelatedIndicator = interactionIndicator;
            if (RelatedIndicator == null)
            {
                CanvasGroup.alpha = 0;
                return;
            }

            if (RelatedIndicator.IsExpectedItemsChanged)
            {
                RelatedIndicator.IsExpectedItemsChanged = false;
                var processIcon = RelatedIndicator.ProcessSource.ProcessIcon;
                var expectedItems = RelatedIndicator.ProcessSource.ExpectedItems;
                for (int i = 0, len = ResourceImages.Length; i < len; i++)
                {
                    var resourceImage = ResourceImages[i];
                    if (processIcon != null)
                    {
                        if (i == 0)
                        {
                            resourceImage.sprite = processIcon;
                            resourceImage.enabled = true;
                        }
                        else
                        {
                            resourceImage.enabled = false;
                        }
                    }
                    else
                    {
                        if (i < expectedItems.Count)
                        {
                            resourceImage.sprite = expectedItems[i].ItemPack.ItemResource?.Icon.Target;
                            resourceImage.enabled = resourceImage.sprite != null;
                        }
                        else
                        {
                            resourceImage.enabled = false;
                        }
                    }
                }
            }
        }

        public override void HideAndReset()
        {
            CanvasGroup.alpha = 0;
            _sequence.KillIfExistsAndActive();
            transform.localPosition = Vector3.zero;
            if (RelatedIndicator != null)
                SetupResources(null);
        }

        public override void HideAllResources()
        {
            HideResourcesButChosen(-1);
        }

        public override void HideResourcesButOne()
        {
            LastAchievedItemIndex = -1;
            var displayedItem = RelatedIndicator.AchievedItems?.FirstOrDefault().ItemResource;
            if (displayedItem != null)
            {
                LastAchievedItemIndex = RelatedIndicator.ProcessSource.ProcessIcon != null
                    ? -1
                    : RelatedIndicator.ProcessSource.ExpectedItems.FindIndex(mrd => mrd.ItemPack.ItemResource == displayedItem);
            }
            HideResourcesButChosen(LastAchievedItemIndex);
        }

        protected virtual void HideResourcesButChosen(int chosenIndex)
        {
            for (int i = 0, len = ResourceImages.Length; i < len; i++)
                if (i != chosenIndex)
                    ResourceImages[i].enabled = false;
        }

        public override void BringOneResourceToCenterAndThenHideIt(TweenCallback onEnd)
        {
            _sequence.KillIfExistsAndActive();
            //1 ресурс, двигать некуда, просто ждем и скрываем
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(RelatedIndicator.ProgressPanelResourceMoveTime);
            _sequence.AppendCallback(HideAllResources);

            if (onEnd != null)
                _sequence.AppendCallback(onEnd);
        }

        public override string ToString()
        {
            return name;
        }
    }
}