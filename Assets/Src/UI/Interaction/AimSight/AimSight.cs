using Core.Environment.Logging.Extension;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class AimSight : MonoBehaviour
    {
        private const int TwoLoopCount = 2;

        public CanvasGroup CanvasGroup;
        public Image SolidCenter;
        public Image Sectors;
        public Sprite[] SectorsVariants;
        public Image ProgressRing;

        [Range(0.1f, 50)]
        public float RotationTime = 10;

        [Range(-360, 360)]
        public float RotationAngle = 180;

        public Vector2 FirstElemSmallScale = new Vector2(20, 20);

        [Range(0.1f, 50)]
        public float FirstElemScaleTime = 5;

        [Range(0.1f, 50)]
        public float SectorVariantShowTime = 5;

        [Range(0.1f, 50)]
        public float LastSectorVariantAlpha = 0.3f;

        [Range(0.1f, 50)]
        public float LastSectorVariantAlphaTime = 5;

        [Range(0.01f, 20)]
        public float ProgressTime = 1;

        [Range(0.1f, 50)]
        public float FinalFadeOutTime = 5;

        [Range(0.001f, 5)]
        public float AnimationRatio = 1 / 30f;

        public Color CancelColor = Color.red;

        private Sequence _sequence;
        private Vector2 _sectorsOriginalSize;
        private float _sequenceStartTime;


        //=== Unity ===============================================================

        private void Awake()
        {
            SolidCenter.AssertIfNull(nameof(SolidCenter));
            CanvasGroup.AssertIfNull(nameof(CanvasGroup));
            Sectors.AssertIfNull(nameof(Sectors));
            SectorsVariants.IsNullOrEmptyOrHasNullElements(nameof(SectorsVariants));
            ProgressRing.AssertIfNull(nameof(ProgressRing));
            if (SectorsVariants.Length < 2)
                UI.Logger.IfError()?.Message($"Too small {nameof(SectorsVariants)} length: {SectorsVariants.Length}").Write();
            CanvasGroup.alpha = 0;
        }


        //=== Public ==============================================================

        public void Play()
        {
            SetElementsToOriginalState();
            _sequenceStartTime = Time.time;
            if (_sequence == null)
                _sequence = GetSequence();
            else
                _sequence.Restart();
        }

        public void StopAndHide()
        {
            if (_sequence != null && _sequence.IsPlaying())
                _sequence.Pause();

            Sectors.color = SolidCenter.color = ProgressRing.color = CancelColor;
            CanvasGroup.DOFade(0, GetDuration(FinalFadeOutTime));
        }


        //=== Private =============================================================

        private void SetElementsToOriginalState()
        {
            Sectors.SetAlpha(1);
            Sectors.sprite = SectorsVariants[0];
            Sectors.rectTransform.localRotation = Quaternion.identity;
            Sectors.color = SolidCenter.color = ProgressRing.color = Color.white;
            Sectors.SetNativeSize();
            _sectorsOriginalSize = Sectors.rectTransform.sizeDelta;
            ProgressRing.fillAmount = 0;
            CanvasGroup.alpha = 1;
        }

        private Sequence GetSequence()
        {
            var sequence = DOTween.Sequence();
            sequence.SetAutoKill(false);
            sequence.Append(
                Sectors.rectTransform.DORotate(
                        new Vector3(0, 0, RotationAngle),
                        GetDuration(RotationTime / TwoLoopCount))
                    .SetLoops(TwoLoopCount, LoopType.Incremental)
                    .SetEase(Ease.Linear)
            );

            sequence.Append(Sectors.rectTransform.DOSizeDelta(
                    FirstElemSmallScale,
                    GetDuration(FirstElemScaleTime / TwoLoopCount))
                .SetLoops(TwoLoopCount, LoopType.Yoyo)
                .SetEase(Ease.Linear)
            );

            for (int i = 1, len = SectorsVariants.Length; i < len; i++)
            {
                var sectorsVariant = SectorsVariants[i];
                sequence.AppendCallback(() => Sectors.sprite = sectorsVariant);
                sequence.AppendInterval(GetDuration(SectorVariantShowTime));
            }

            sequence.Append(Sectors.DOFade(
                LastSectorVariantAlpha,
                GetDuration(LastSectorVariantAlphaTime))
            );

            sequence.Append(ProgressRing.DOFillAmount(
                    1,
                    ProgressTime)
                .SetEase(Ease.Linear)
            );

            sequence.AppendCallback(() => ProgressRing.fillAmount = 0);

            for (int i = SectorsVariants.Length - 2; i >= 0; i--)
            {
                sequence.AppendInterval(GetDuration(SectorVariantShowTime));
                var sectorsVariant = SectorsVariants[i];
                sequence.AppendCallback(() => Sectors.sprite = sectorsVariant);
            }

            Sectors.rectTransform.sizeDelta = _sectorsOriginalSize;
            sequence.Append(Sectors.rectTransform.DOSizeDelta(
                    FirstElemSmallScale,
                    GetDuration(FirstElemScaleTime))
                .SetEase(Ease.Linear)
            );

            sequence.AppendCallback(() => CanvasGroup.alpha = 0);

            sequence.AppendCallback(
                () => UI.Logger.IfDebug()?.Message("<{1}> Sequence time={2:f3})", GetType(), Time.time - _sequenceStartTime).Write());
            return sequence;
        }

        private float GetDuration(float time)
        {
            return AnimationRatio * time;
        }
    }
}