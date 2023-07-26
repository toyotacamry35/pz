using UnityEngine.UI;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class GraphicAlphaTweenComponent : TweenComponentBase
    {
        public Graphic Graphic;


        //=== Props ===========================================================

        protected override float Parameter
        {
            get => Graphic.color.a;
            set => Graphic.SetAlpha(value);
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Graphic.AssertIfNull(nameof(Graphic));
        }
    }
}