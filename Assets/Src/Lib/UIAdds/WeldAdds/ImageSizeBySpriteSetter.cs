using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ImageSizeBySpriteSetter : MonoBehaviour
    {
        public Image Image;

        public bool DoWidthTracking = true;

        public bool DoHeightTracking = true;

        public bool SwitchOfImageWithEmptySprite = true;

        private Sprite _lastSprite;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (Image.AssertIfNull(nameof(Image)))
            {
                enabled = false;
                return;
            }

            Image.DisableSpriteOptimizations();
            SetImageSize();
        }

        private void Update()
        {
            if (_lastSprite != Image.sprite)
            {
                _lastSprite = Image.sprite;
                SetImageSize();
            }
        }


        //=== Private =============================================================

        private void SetImageSize()
        {
            if (!DoHeightTracking && !DoWidthTracking)
                return;

            if (_lastSprite == null)
                return;

            if (SwitchOfImageWithEmptySprite)
                Image.enabled = _lastSprite != null;

            if (_lastSprite == null || (!DoWidthTracking && !DoHeightTracking))
                return;

            Image.rectTransform.sizeDelta = new Vector2(
                DoWidthTracking ? _lastSprite.texture.width : Image.rectTransform.sizeDelta.x,
                DoHeightTracking ? _lastSprite.texture.height : Image.rectTransform.sizeDelta.y);
        }
    }
}