using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class LayoutGroupPaddingSetter : MonoBehaviour
    {
        public HorizontalOrVerticalLayoutGroup Target;

        /// <summary>
        /// Добавка/вычет относительно имеющегося размера, а не собственно значение
        /// </summary>
        public bool IsRelative;

        private bool _isAfterAwake;
        private RectOffset _basePadding;
        private float _baseSpacing;


        //=== Props ===========================================================

        private float _spacing;

        public float Spacing
        {
            get => _spacing;
            set
            {
                if (!Mathf.Approximately(_spacing, value))
                {
                    _spacing = value;
                    if (_isAfterAwake)
                        SyncIfWokenSpacing();
                }
            }
        }

        private int _topPadding;

        public int TopPadding
        {
            get => _topPadding;
            set
            {
                if (_topPadding != value)
                {
                    _topPadding = value;
                    if (_isAfterAwake)
                        SyncIfWokenPadding();
                }
            }
        }

        private int _bottomPadding;

        public int BottomPadding
        {
            get => _bottomPadding;
            set
            {
                if (_bottomPadding != value)
                {
                    _bottomPadding = value;
                    if (_isAfterAwake)
                        SyncIfWokenPadding();
                }
            }
        }

        private int _leftPadding;

        public int LeftPadding
        {
            get => _leftPadding;
            set
            {
                if (_leftPadding != value)
                {
                    _leftPadding = value;
                    if (_isAfterAwake)
                        SyncIfWokenPadding();
                }
            }
        }

        private int _rightPadding;

        public int RightPadding
        {
            get => _rightPadding;
            set
            {
                if (_rightPadding != value)
                {
                    _rightPadding = value;
                    if (_isAfterAwake)
                        SyncIfWokenPadding();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (Target.AssertIfNull(nameof(Target)))
            {
                enabled = false;
                return;
            }

            _basePadding = IsRelative ? Target.padding : new RectOffset();
            _baseSpacing = IsRelative ? Target.spacing : 0;
            _isAfterAwake = true;
            SyncIfWokenSpacing();
            SyncIfWokenPadding();
        }


        //=== Private =============================================================

        private void SyncIfWokenPadding()
        {
            Target.padding = new RectOffset(
                LeftPadding + _basePadding.left,
                RightPadding + _basePadding.right,
                TopPadding + _basePadding.top,
                BottomPadding + _basePadding.bottom);
        }

        private void SyncIfWokenSpacing()
        {
            Target.spacing = _baseSpacing + Spacing;
        }
    }
}