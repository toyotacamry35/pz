using UnityEngine.UI;

namespace WeldAdds
{
    public class ByRangeAlphaSetter : ByRangeGraphicSetter
    {
        protected override void ApplySettingsToGraphic(Graphic graphic, Settings settings)
        {
            graphic.SetAlpha(settings.Color.a);
        }
    }
}