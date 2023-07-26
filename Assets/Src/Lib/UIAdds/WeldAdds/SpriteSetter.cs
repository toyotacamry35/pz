using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class SpriteSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Image _image;

        [SerializeField, UsedImplicitly]
        private bool _hideNullSprite;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private Sprite _sprite;

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (!enabled)
                    return;

                if (_sprite != value)
                {
                    _sprite = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_image.AssertIfNull(nameof(_image), gameObject))
            {
                enabled = false;
                return;
            }

            _image.DisableSpriteOptimizations();
            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =========================================================

        private void SyncIfWoken()
        {
            _image.sprite = Sprite;
            if (_hideNullSprite)
                _image.enabled = Sprite != null;
        }
    }
}