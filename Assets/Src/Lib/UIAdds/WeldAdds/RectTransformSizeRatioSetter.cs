using UnityEngine;

namespace WeldAdds
{
    public class RectTransformSizeRatioSetter : MonoBehaviour
    {
        public RectTransform RectTransform;

        public Vector2 OriginalSize;

        public bool DoWidthTracking = true;

        public bool DoHeightTracking = true;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private float _ratio;

        public float Ratio
        {
            get => _ratio;
            set
            {
                if (!Mathf.Approximately(_ratio, value))
                {
                    _ratio = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (RectTransform.AssertIfNull(nameof(RectTransform)))
            {
                enabled = false;
                return;
            }

            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =============================================================

        private void SyncIfWoken()
        {
            if (!DoHeightTracking && !DoWidthTracking)
                return;

            RectTransform.sizeDelta = new Vector2(
                DoWidthTracking ? OriginalSize.x * Ratio : RectTransform.sizeDelta.x,
                DoHeightTracking ? OriginalSize.y * Ratio : RectTransform.sizeDelta.y);
        }
    }
}