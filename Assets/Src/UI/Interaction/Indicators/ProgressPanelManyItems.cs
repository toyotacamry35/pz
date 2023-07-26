using Assets.Src.Lib.DOTweenAdds;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ProgressPanelManyItems : ProgressPanelOneItem
    {
        /// <summary>
        /// For 0...n resources
        /// </summary>
        //public ChanceImagesSet[] ChanceImagesSets;

        public ChanceImage[] ChanceImages;

        public Image[] Dividers;

        public TweenSettingsVector2 ResourcePositionTweenSettings;
        public TweenSettingsVector2 ResourceScaleTweenSettings;

        private Tweener _resourceMoveTweener;
        private Tweener _resourceScaleTweener;


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            ChanceImages.IsNullOrEmptyOrHasNullElements(nameof(ChanceImages));
            Dividers.IsNullOrEmptyOrHasNullElements(nameof(Dividers));
            ResourcePositionTweenSettings.AssertIfNull(nameof(ResourcePositionTweenSettings));
            ResourceScaleTweenSettings.AssertIfNull(nameof(ResourceScaleTweenSettings));
            if (ChanceImages.Length != ResourceImages.Length)
                UI.Logger.Error($"{nameof(Awake)}() Don't corresponds lengths: {nameof(ChanceImages)}={ChanceImages.Length}, " +
                               $"{nameof(ResourceImages)}={ResourceImages.Length}");
        }


        //=== Public ==========================================================

        public override void SetupResources(InteractionIndicator interactionIndicator)
        {
            bool orgIsExpectedItemsChanged = interactionIndicator?.IsExpectedItemsChanged ?? false; //в базовом будет сброшен
            base.SetupResources(interactionIndicator);
            if (RelatedIndicator == null)
                return;

            if (orgIsExpectedItemsChanged)
            {
                for (int i = 0, len = ResourceImages.Length; i < len; i++)
                {
                    var resourceImage = ResourceImages[i];
                    resourceImage.rectTransform.anchoredPosition = ResourcePositionTweenSettings.From;
                    resourceImage.rectTransform.sizeDelta = ResourceScaleTweenSettings.From;
                }

                for (int i = 0, len = ChanceImages.Length; i < len; i++)
                    ChanceImages[i].SwitchImagesByChance(RelatedIndicator.ProcessSource.ExpectedItems[i].Chance);

                SwitchDividersVisibility(true);
            }
        }

        public override void HideAndReset()
        {
            base.HideAndReset();
            _resourceMoveTweener.KillIfExistsAndActive();
            _resourceScaleTweener.KillIfExistsAndActive();
            SwitchDividersVisibility(true);
        }

        protected override void HideResourcesButChosen(int chosenIndex)
        {
            base.HideResourcesButChosen(chosenIndex);
            HideAllChanceImagesSets();
            SwitchDividersVisibility(false);
        }

        public override void BringOneResourceToCenterAndThenHideIt(TweenCallback onEnd)
        {
            _resourceMoveTweener.KillIfExistsAndActive();
            if (LastAchievedItemIndex >= 0 && LastAchievedItemIndex < ResourceImages.Length)
            {
                _resourceMoveTweener = ResourceImages[LastAchievedItemIndex].transform.DOLocalMove(
                        ResourcePositionTweenSettings.To,
                        RelatedIndicator.ProgressPanelResourceMoveTime)
                    .SetEase(ResourcePositionTweenSettings.Ease);

                _resourceMoveTweener.OnComplete(() =>
                {
                    HideAllResources();
                    onEnd?.Invoke();
                });

                _resourceScaleTweener.KillIfExistsAndActive();
                var targetRectTransform = (RectTransform) ResourceImages[LastAchievedItemIndex].transform;
                _resourceScaleTweener = DOTween.To(
                        () => targetRectTransform.sizeDelta,
                        v2 => targetRectTransform.sizeDelta = v2,
                        ResourceScaleTweenSettings.To,
                        RelatedIndicator.ProgressPanelResourceMoveTime)
                    .SetEase(ResourceScaleTweenSettings.Ease);
            }
            else
            {
                base.BringOneResourceToCenterAndThenHideIt(onEnd);
            }
        }


        //=== Private =============================================================



        private void SwitchDividersVisibility(bool isOn)
        {
            for (int i = 0, len = Dividers.Length; i < len; i++)
                Dividers[i].enabled = isOn;
        }

        private void HideAllChanceImagesSets()
        {
            for (int i = 0, len = ChanceImages.Length; i < len; i++)
                ChanceImages[i].Hide();
        }
    }
}