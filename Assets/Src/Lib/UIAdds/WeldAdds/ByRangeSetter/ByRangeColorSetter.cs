using UnityEngine.UI;

namespace WeldAdds
{
    public class ByRangeColorSetter : ByRangeGraphicSetter
    {
        protected override void ApplySettingsToGraphic(Graphic graphic, Settings settings)
        {
            graphic.color = settings.Color;
        }
    }
}