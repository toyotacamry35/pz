using UnityEngine;

namespace WeldAdds
{
    class CanvasEnableSetter : MonoBehaviour
    {
        public Canvas Canvas;
        private bool _isAfterAwake;


        //=== Props ===============================================================

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (!enabled)
                    return;

                if (_isVisible != value)
                {
                    _isVisible = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            if (Canvas.AssertIfNull(nameof(Canvas)))
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
            Canvas.enabled = IsVisible;
        }
    }
}