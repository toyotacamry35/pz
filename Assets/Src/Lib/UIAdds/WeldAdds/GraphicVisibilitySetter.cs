using UnityEngine.UI;

namespace WeldAdds
{
    public class GraphicVisibilitySetter : SomethingVisibilitySetter
    {
        public Graphic Graphic;
        public bool IsRaycastTargetAffects;


        //=== Protected =======================================================

        protected override bool OnAwakeSelfCheck()
        {
            return !Graphic.AssertIfNull(nameof(Graphic));
        }

        protected override void OnVisibilityChanged(bool isVisible)
        {
            Graphic.SetAlpha(isVisible ? OnAlpha : OffAlpha);
            if (IsRaycastTargetAffects)
                Graphic.raycastTarget = isVisible;
        }
    }
}