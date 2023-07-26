using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ButtonSpritesSet : MonoBehaviour
    {
        public string Description;
        public Sprite Normal;
        public Sprite Highlighted;
        public Sprite Pressed;
        public Sprite Disabled;


        //=== Unity ===============================================================

        private void Awake()
        {
            Normal.AssertIfNull(nameof(Normal));
            Highlighted.AssertIfNull(nameof(Highlighted));
            Pressed.AssertIfNull(nameof(Pressed));
            Disabled.AssertIfNull(nameof(Disabled));
        }

        public void SetSpritesForButton(Button button, Image image = null)
        {
            Image buttonTargetImage = image != null ? image : (button.targetGraphic as Image);
            if (buttonTargetImage != null)
                buttonTargetImage.sprite = Normal;

            if (button != null)
                button.spriteState = new SpriteState()
                {
                    highlightedSprite = Highlighted,
                    pressedSprite = Pressed,
                    disabledSprite = Disabled, //где Selected?
                };
        }
    }
}