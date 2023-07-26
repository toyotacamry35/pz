using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    class GraphicColorTweenComponent : GraphicColoringTweenComponent
    {
        protected override float Parameter
        {
            get { return Value; }
            set
            {
                if (!Mathf.Approximately(Value, value))
                {
                    Value = value;
                    var newColor = GetColorByValue(Value, true);
                    Graphic.color = newColor;
                }
            }
        }
    }
}