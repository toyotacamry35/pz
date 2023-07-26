using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ButtonColorsSet : MonoBehaviour
    {
        public Color Normal;
        public Color Highlighted;
        public Color Pressed;
        public Color Disabled;

        public void SetColorsForButton(Button button, Image image = null)
        {
            if (button == null)
                return;

            button.colors = new ColorBlock
            {
                colorMultiplier = button.colors.colorMultiplier,
                fadeDuration = button.colors.fadeDuration,
                normalColor = Normal,
                disabledColor = Disabled,
                highlightedColor = Highlighted,
                pressedColor = Pressed,
                selectedColor = Normal //нужно ли?
            };
        }
    }
}