using UnityEngine;

namespace WeldAdds
{
    public class RectTransformRotationSetter : MonoBehaviour
    {
        public RectTransform RectTransform;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private float _rotationZ;

        public float RotationZ
        {
            get => _rotationZ;
            set
            {
                if (!Mathf.Approximately(_rotationZ, value))
                {
                    _rotationZ = value;
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
            transform.localEulerAngles = new Vector3(0, 0, RotationZ);
        }
    }
}