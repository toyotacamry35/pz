using UnityEngine.UI;

namespace WeldAdds
{
    public class ByRangeColoringSetter : ByRangeGraphicSetter
    {
        protected override void ApplySettingsToGraphic(Graphic graphic, Settings settings)
        {
            graphic.SetColoring(settings.Color);
        }
    }
}