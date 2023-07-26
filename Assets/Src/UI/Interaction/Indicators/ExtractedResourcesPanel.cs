using System.Collections.Generic;
using Assets.Src.Inventory;
using Assets.Src.Lib.DOTweenAdds;
using Core.Environment.Logging.Extension;
using DG.Tweening;
using SharedCode.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ExtractedResourcesPanel : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        public Image MainResourceImage;
        public TextMeshProUGUI MainResourceText;

        public Image ExtraResourceImage;
        public TextMeshProUGUI ExtraResourceText;

        public TextMeshProUGUI AdditionalResourcesText;

        public TweenSettingsVector3 ScaleSettings;
        public TweenSettingsFloat YPositionSettings;
        public float ExtractedResultsStayingTime = 1;

        [SerializeField]
        private List<TweenComponentBase> _tweenComponents;

        private Tweener _yPositionTweener;
        private Tweener _scaleTweener;
        private Sequence _sequence;
        private RectTransform _rectTransform;

        private IList<ItemResourcePack> _lastExtractedItems;
        private TweenCallback _lastCallback;


        //=== Unity ===============================================================

        private void Awake()
        {
            CanvasGroup.AssertIfNull(nameof(CanvasGroup));
            MainResourceImage.AssertIfNull(nameof(MainResourceImage));
            MainResourceText.AssertIfNull(nameof(MainResourceText));
            ExtraResourceImage.AssertIfNull(nameof(ExtraResourceImage));
            ExtraResourceText.AssertIfNull(nameof(ExtraResourceText));
            AdditionalResourcesText.AssertIfNull(nameof(AdditionalResourcesText));
            ScaleSettings.AssertIfNull(nameof(ScaleSettings));
            YPositionSettings.AssertIfNull(nameof(YPositionSettings));

            CanvasGroup.alpha = 0;
            _rectTransform = transform as RectTransform;
        }


        //=== Public ==============================================================

        public void HideAndReset()
        {
            CanvasGroup.alpha = 0;
            _yPositionTweener.KillIfExistsAndActive();
            _scaleTweener.KillIfExistsAndActive();
            _sequence.KillIfExistsAndActive();
            transform.localScale = Vector3.one;
        }

        public void Appearing(float mandatoryDelay, IList<ItemResourcePack> extractedItems, TweenCallback onEnd = null)
        {
            if (extractedItems == null || extractedItems.Count == 0)
            {
                UI.Logger.IfError()?.Message($"<{GetType()}> {nameof(Appearing)}() items is empty").Write();
                onEnd?.Invoke();
                return;
            }

            _lastExtractedItems = extractedItems;
            _lastCallback = onEnd;
            CanvasGroup.alpha = 0;
            _rectTransform.anchoredPosition = new Vector2(0, YPositionSettings.From);
            _yPositionTweener.KillIfExistsAndActive();
            _scaleTweener.KillIfExistsAndActive();
            _sequence.KillIfExistsAndActive();

            _sequence = DOTween.Sequence().AppendCallback(PrepareAdditionalTweenComponents)
                .AppendInterval(mandatoryDelay)
                .AppendCallback(SetResources)
                .AppendCallback(PlayAdditionalTweenComponents)
                .Append(_yPositionTweener = DOTween.To(
                        () => _rectTransform.anchoredPosition.y,
                        y => _rectTransform.anchoredPosition = new Vector2(0, y),
                        YPositionSettings.To,
                        YPositionSettings.Duration)
                    .SetEase(YPositionSettings.Ease))
                .AppendInterval(ExtractedResultsStayingTime)
                .AppendCallback(DoCallbackAndScaleDown);
        }

        private void SetResources()
        {
            SetExtractedItem(_lastExtractedItems[0], MainResourceImage, MainResourceText);

            var isShow = _lastExtractedItems.Count > 1;
            ExtraResourceImage.gameObject.SetActive(isShow);
            if (isShow)
            {
                SetExtractedItem(_lastExtractedItems[1], ExtraResourceImage, ExtraResourceText);

                isShow = _lastExtractedItems.Count > 2;
                AdditionalResourcesText.gameObject.SetActive(isShow);
                if (isShow)
                    AdditionalResourcesText.text = $"and {_lastExtractedItems.Count - 2} more"; //TODO Localization
            }
            CanvasGroup.alpha = 1;
        }

        private void SetExtractedItem(ItemResourcePack itemStack, Image image, TextMeshProUGUI textMeshPro)
        {
            image.sprite = itemStack.ItemResource?.Icon.Target;
            textMeshPro.text = $"[{itemStack.Count}]";
        }

        private void PrepareAdditionalTweenComponents()
        {
            if (_tweenComponents == null || _tweenComponents.Count == 0)
                return;

            for (int i = 0, len = _tweenComponents.Count; i < len; i++)
            {
                _tweenComponents[i].SetParamValue(true);
            }
        }

        private void PlayAdditionalTweenComponents()
        {
            if (_tweenComponents == null || _tweenComponents.Count == 0)
                return;

            for (int i = 0, len = _tweenComponents.Count; i < len; i++)
            {
                _tweenComponents[i].Play(true);
            }
        }

        private void DoCallbackAndScaleDown()
        {
            _lastCallback?.Invoke();
            _scaleTweener = DOTween.To(
                    () => transform.localScale,
                    v2 => transform.localScale = v2,
                    ScaleSettings.To,
                    ScaleSettings.Duration)
                .SetEase(ScaleSettings.Ease)
                .OnComplete(HideAndReset);
        }
    }
}