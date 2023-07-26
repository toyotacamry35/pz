using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ColorAndSpriteSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Graphic _target;

        [SerializeField, UsedImplicitly]
        private ColorAndSprite[] _colorsAndSprites;

        [SerializeField, UsedImplicitly]
        private ColorAndSprite _defaultColorAndSprite;

        [SerializeField, UsedImplicitly]
        private bool _watchForSprite;

        [SerializeField, UsedImplicitly]
        private bool _watchForColor;

        [SerializeField, UsedImplicitly]
        private bool _hideNullSpriteGraphic;

        [SerializeField, UsedImplicitly]
        private bool _useFlagNorIndex;

        private bool _isAfterAwake;
        private Image _graphicAsImage;


        //=== Props ===========================================================

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                if (!enabled)
                    return;

                if (_index != value)
                {
                    _index = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _target.AssertIfNull(nameof(_target), gameObject);
            _defaultColorAndSprite.AssertIfNull(nameof(_defaultColorAndSprite), gameObject);
            _colorsAndSprites.IsNullOrEmptyOrHasNullElements(nameof(_colorsAndSprites));
            if (_watchForSprite)
            {
                _graphicAsImage = _target as Image;
                _graphicAsImage?.DisableSpriteOptimizations();
            }

            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =========================================================

        private void SyncIfWoken()
        {
            if (!_watchForColor && !_watchForSprite)
                return;

            Sprite sprite = _defaultColorAndSprite.Sprite;
            Color color = _defaultColorAndSprite.Color;

            int index = _useFlagNorIndex ? (Flag ? 0 : -1) : Index;

            if (index >= 0 && index < _colorsAndSprites.Length)
            {
                sprite = _colorsAndSprites[index].Sprite;
                color = _colorsAndSprites[index].Color;
            }

            if (_watchForColor)
                _target.color = color;

            if (_watchForSprite && _graphicAsImage != null)
            {
                _graphicAsImage.sprite = sprite;
                if (_hideNullSpriteGraphic)
                    _target.enabled = sprite != null;
            }
        }
    }
}