using UnityEngine;
using UnityEngine.UI;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class GraphicColoringTweenComponent : TweenComponentBase
    {
        public Graphic Graphic;
        public Color FromColor;
        public Color ToColor;

        protected float Value;
        private Vector4 _rColoringData, _gColoringData, _bColoringData, _aColoringData;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get { return Value; }
            set
            {
                if (!Mathf.Approximately(Value, value))
                {
                    Value = value;
                    var newColoring = GetColorByValue(Value, false);
                    Graphic.SetColoring(newColoring);
                }
            }
        }

        protected Color GetColorByValue(float x, bool withAlpha)
        {
            return new Color(
                LinearRelation.GetClampedY(_rColoringData, x),
                LinearRelation.GetClampedY(_gColoringData, x),
                LinearRelation.GetClampedY(_bColoringData, x),
                withAlpha ? LinearRelation.GetClampedY(_aColoringData, x) : 0
            );
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Graphic.AssertIfNull(nameof(Graphic));
            if (Mathf.Approximately(From, To))
                Debug.LogError($"<{GetType()}> '{name}' From == To ({From})");

            _rColoringData = new Vector4(From, FromColor.r, To, ToColor.r);
            _gColoringData = new Vector4(From, FromColor.g, To, ToColor.g);
            _bColoringData = new Vector4(From, FromColor.b, To, ToColor.b);
            _aColoringData = new Vector4(From, FromColor.a, To, ToColor.a);
        }
    }
}