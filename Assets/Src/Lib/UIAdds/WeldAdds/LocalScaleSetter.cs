using UnityEngine;

namespace WeldAdds
{
    public class LocalScaleSetter : MonoBehaviour
    {
        public Transform Transform;
        public Vector3 OriginalScale = Vector3.one;

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
            if (Transform.AssertIfNull(nameof(Transform)))
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
            Transform.localScale = new Vector3(OriginalScale.x * Ratio, OriginalScale.y * Ratio, OriginalScale.z * Ratio);
        }
    }
}