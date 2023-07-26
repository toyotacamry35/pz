using Assets.Src.Aspects.SpatialObjects;

namespace Assets.Src.Aspects.VisualMarkers
{
    public class VisualMarkerNoEntity : AVisualMarker, IAABBTriggered
    {
        private int _aabbCounter = 0;


        //=== Public ==========================================================

        public void OnAABBEnter()
        {
            _aabbCounter++;
            if (_aabbCounter > 0)
            {
                OnceInit();
                IsNearRp.Value = true;
            }
        }

        public void OnAABBExit()
        {
            _aabbCounter--;
            if (_aabbCounter <= 0)
                IsNearRp.Value = false;
        }


        //=== Protected =======================================================

        protected override bool GetIsOurPlayer()
        {
            return false;
        }


        //=== Private =========================================================

        private new void OnDestroy()
        {
            IsNearRp.Value = false;
        }
    }
}