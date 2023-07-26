using UnityEngine;

namespace WeldAdds
{
    public class RotationZSetter : MonoBehaviour
    {
        public Transform Transform;

        public LimitedUsageType UsageType;

        public float TrueAngle = 180;
        public float FalseAngle;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private float _angle;

        public float Angle
        {
            get => _angle;
            set
            {
                if (!Mathf.Approximately(_angle, value))
                {
                    _angle = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
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
            var angle = UsageType == LimitedUsageType.Amount ? Angle : (Flag ? TrueAngle : FalseAngle);
            Transform.localEulerAngles = new Vector3(Transform.localEulerAngles.x, Transform.localEulerAngles.y, angle);
        }
    }
}