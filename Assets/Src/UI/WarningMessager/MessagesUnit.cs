using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Uins.Sound;
using UnityEngine;

namespace Uins
{
    public class MessagesUnit : MonoBehaviour
    {
        private const int ItemsMaxCount = 4;
        private const float ItemLifeDuration = 8;
        private const float ItemFadeDuration = 0.5f;
        private const float ItemMoveDuration = 1;

        [SerializeField, UsedImplicitly]
        private bool _isFromUpToDown;

        [SerializeField, UsedImplicitly]
        private float _verticalGapBetweenItems = 4;

        [SerializeField, UsedImplicitly]
        private RectTransform _rootRectTransform;

        [SerializeField, UsedImplicitly]
        private WarningItem _warningItemPrefab;

        private WarningItem[] _warningItems;
        private Tweener _moveTweener;
        private float _rootLastEndY;


        //=== Unity ===============================================================

        private void Awake()
        {
            _rootRectTransform.AssertIfNull(nameof(_rootRectTransform));
            _warningItemPrefab.AssertIfNull(nameof(_warningItemPrefab));
        }

        private void Start()
        {
            if (_warningItemPrefab == null)
                return;

            _warningItems = new WarningItem[ItemsMaxCount];

            for (int i = 0; i < ItemsMaxCount; i++)
            {
                var warningItem = Instantiate(_warningItemPrefab, _rootRectTransform);
                warningItem.name = _warningItemPrefab.name + i;
                warningItem.Setup(this);
                _warningItems[i] = warningItem;
            }
        }


        //=== Public ==============================================================

        public void ShowWarningMessage(string message, Color textColor, Sprite sprite1, Sprite sprite2, Color spritesColor,
            object additionalInfo)
        {
            SoundControl.Instance.KnowledgeBaseNewNote?.Post(gameObject.transform.root.gameObject);

//            UI.Logger.Debug(
//                $"{nameof(ShowWarningMessage)} '{message}', {nameof(textColor)}={textColor}, {nameof(spritesColor)}={spritesColor}"); //DEBUG
            var warningItem = GetWarningItem();
            if (warningItem.AssertIfNull(nameof(warningItem)))
                return;

            warningItem.Show(
                message,
                textColor,
                ItemLifeDuration,
                ItemFadeDuration,
                sprite1,
                sprite2,
                spritesColor,
                additionalInfo);

            var verticalOffset = warningItem.GetMessageHeight() + _verticalGapBetweenItems; //сколько займет места
            warningItem.SetPosition(new Vector2(0, (verticalOffset * (_isFromUpToDown ? 1 : -1) - _rootLastEndY)));

            MoveRoot(verticalOffset * (_isFromUpToDown ? -1 : 1), ItemMoveDuration);
        }

        public void ReleaseItem(WarningItem item)
        {
            if (_warningItems.Any(i => i.IsShown))
                return;

            _moveTweener.KillIfExistsAndActive();
            ResetRootPosition();
        }


        //=== Private =============================================================

        private WarningItem GetWarningItem()
        {
            var unusedItem = _warningItems.FirstOrDefault(i => !i.IsShown);
            if (unusedItem != null)
                return unusedItem;

            return _warningItems.OrderBy(i => i.StartShowTime).FirstOrDefault();
        }

        private void MoveRoot(float distance, float duration)
        {
            _moveTweener.KillIfExistsAndActive();

            _rootLastEndY += distance;
            _moveTweener =
                DOTween.To(
                    () => _rootRectTransform.anchoredPosition,
                    v2 => _rootRectTransform.anchoredPosition = v2,
                    new Vector2(0, _rootLastEndY),
                    duration);
        }

        private void ResetRootPosition()
        {
            _rootLastEndY = 0;
            _rootRectTransform.anchoredPosition = new Vector2(_rootRectTransform.anchoredPosition.x, _rootLastEndY);
        }
    }
}