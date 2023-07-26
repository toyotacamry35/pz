using Core.Environment.Logging.Extension;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class WarningItem : BindingViewModel
    {
        private const int AdditionalTransformSiblingIndex = 1;

        private static readonly Color DefaultColor = new Color();

        [SerializeField, UsedImplicitly]
        private CanvasGroup _canvasGroup;

        [SerializeField, UsedImplicitly]
        private AchievedPointsInfoViewModel _achievedPointsInfoPrefab;

        private AchievedPointsInfoViewModel _achievedPointsInfoVm;
        private MessagesUnit _messagesUnit;
        private Sequence _alphaSequence;
        private RectTransform _rectTransform;


        //=== Props ===============================================================

        public float StartShowTime { get; private set; }

        private bool _isShown;

        [Binding]
        public bool IsShown
        {
            get => _isShown;
            set
            {
                if (value != _isShown)
                {
                    _isShown = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _sprite1;

        [Binding]
        public Sprite Sprite1
        {
            get => _sprite1;
            set
            {
                if (value != _sprite1)
                {
                    _sprite1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _sprite2;

        [Binding]
        public Sprite Sprite2
        {
            get => _sprite2;
            set
            {
                if (value != _sprite2)
                {
                    _sprite2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _text;

        [Binding]
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Color _textColor;

        [Binding]
        public Color TextColor
        {
            get => _textColor;
            set
            {
                if (!value.Equals(_textColor))
                {
                    _textColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Color _spritesColor;

        [Binding]
        public Color SpritesColor
        {
            get => _spritesColor;
            set
            {
                if (!value.Equals(_spritesColor))
                {
                    _spritesColor = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _canvasGroup.AssertIfNull(nameof(_canvasGroup));
            _achievedPointsInfoPrefab.AssertIfNull(nameof(_achievedPointsInfoPrefab));

            var images = transform.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
                image.DisableSpriteOptimizations();

            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup.alpha = 0;
        }


        //=== Public ==============================================================

        public void Setup(MessagesUnit messagesUnit)
        {
            _messagesUnit = messagesUnit;
            _messagesUnit.AssertIfNull(nameof(_messagesUnit));
        }

        public void SetPosition(Vector2 anchoredPosition)
        {
            _rectTransform.anchoredPosition = anchoredPosition;
        }

        public void Show(string message, Color textColor, float lifeDuration, float fadeDuration, Sprite sprite1, Sprite sprite2,
            Color spritesColor, object additionalData)
        {
            if (IsShown && _alphaSequence != null && _alphaSequence.IsPlaying())
                _alphaSequence.KillIfExistsAndActive();

            StartShowTime = Time.time;
            TextColor = textColor.Equals(DefaultColor) ? Color.white : textColor;
            SpritesColor = spritesColor.Equals(DefaultColor) ? Color.white : spritesColor;
            Text = message;
            Sprite1 = sprite1;
            Sprite2 = sprite2;
            _canvasGroup.alpha = 0;
            IsShown = true;
            ProcessAdditionalData(additionalData);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

            var delayBetweenFadings = lifeDuration - 2 * fadeDuration;
            _alphaSequence = DOTween.Sequence()
                .Append(DOTween.To(alpha => _canvasGroup.alpha = alpha, 0, 1, fadeDuration))
                .AppendInterval(delayBetweenFadings)
                .Append(DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha, 0, fadeDuration))
                .AppendCallback(OnLifeEnd)
                .SetEase(Ease.Linear);
        }

        public float GetMessageHeight()
        {
            return _rectTransform.sizeDelta.y;
        }


        //=== Private ==============================================================

        private void ProcessAdditionalData(object additionalData)
        {
            if (additionalData != null && !(additionalData is AchievedPointsNotificationInfo))
            {
                UI.Logger.IfError()?.Message($"Unhandled {nameof(additionalData)} type: {additionalData.GetType()}").Write();
            }

            var achievedPointsInfo = additionalData as AchievedPointsNotificationInfo;

            if (achievedPointsInfo != null)
            {
                if (_achievedPointsInfoVm == null)
                    _achievedPointsInfoVm = GetAchievedPointsInfoVm();

                _achievedPointsInfoVm.Init(achievedPointsInfo);
            }
            else
            {
                if (_achievedPointsInfoVm != null)
                    _achievedPointsInfoVm.IsVisible = false;
            }
        }

        public void OnLifeEnd()
        {
            IsShown = false;
            _messagesUnit.ReleaseItem(this);
        }


        //=== Private =========================================================

        private AchievedPointsInfoViewModel GetAchievedPointsInfoVm()
        {
            var achievedPointsInfoViewModel = Instantiate(_achievedPointsInfoPrefab, transform);
            if (achievedPointsInfoViewModel.AssertIfNull(nameof(achievedPointsInfoViewModel)))
                return null;

            achievedPointsInfoViewModel.transform.SetSiblingIndex(AdditionalTransformSiblingIndex);
            return achievedPointsInfoViewModel;
        }
    }
}