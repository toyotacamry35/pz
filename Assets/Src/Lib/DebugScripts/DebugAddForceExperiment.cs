using UnityEngine;

namespace Assets.Src.Lib.DebugScripts
{
    // Debug script to try myself different `ForceMode` modes
    public class DebugAddForceExperiment : MonoBehaviour
    {
        public ForceMode FMode;
        public float Value;
        public float Freq;
        public bool ContinuousImpact;
        public bool AddTorque;
        public Vector3 TorqueVec;
        public ForceMode TorqueMode;
        public bool ResetTrigger;

        private bool _continuousImpactApplied;
        private float _nextDoTime;
        private Rigidbody _rb;
        private Vector3 _startPos;
        private Vector3 _dir = Vector3.one;

        private void Awake()
        {
            _startPos = transform.position;

            _rb = GetComponent<Rigidbody>();
            _nextDoTime = Time.time + Freq;
        }

        private void FixedUpdate()
        {
            if (ResetTrigger)
            {
                transform.position = _startPos;
                transform.rotation = Quaternion.identity;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;

                _nextDoTime = Time.time + Freq;
                _continuousImpactApplied = false;

                ResetTrigger = false;
            }

            if (!ContinuousImpact)
            {
                if (Time.time < _nextDoTime)
                    return;

                _rb.AddForce(_dir * Value, FMode);
                if (AddTorque)
                    _rb.AddTorque(TorqueVec, TorqueMode);
                _nextDoTime = Time.time + Freq;
            }
            else
            {
                if (!_continuousImpactApplied && Time.time < _nextDoTime)
                    return;

                _continuousImpactApplied = true;
                _rb.AddForce(_dir * Value, FMode);
            }
        }


    }
}
