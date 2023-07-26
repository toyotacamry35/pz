using UnityEngine;

namespace Assets.Src.Tools
{
    public class KalmanFilter
    {
        private Matrix _Q;
        private Matrix _H;
        private Matrix _R;
        private Matrix _F;
        private Matrix _P;
        private Matrix _value;

        private bool _isInitialized = false;
        private float _noiseError;
        private float _measurementError;

        public KalmanFilter(float measurementError, float noiseError)
        {
            _noiseError = noiseError;
            _measurementError = measurementError;

            _value = new float[,] {
                { 0f },
                { 0f },
                { 0f }
            };

            _R = new float[,] {
                { _measurementError }
            };
            _F = new float[,] {
                { 1, 0 },
                { 0, 1 }
            };
            _Q = new float[,] {
                { 0, 0 },
                { 0, 1 }
            };
            _H = new float[,] {
                { 1, 0 }
            };
            _P = new float[,] {
                { 0, 0 },
                { 0, 0 }
            };
        }

        public Vector3 Filter(float value, float speed, float dt)
        {
            SetDeltaTime(dt);
            if (!_isInitialized)
            {
                _value = new float[,] {
                    { value },
                    { speed }
                };
                _isInitialized = true;
            }
            // _value[0, 1] = speed;
            //_value[0, 2] = 0;

            return SetMeasurement(value);
        }

        private void SetDeltaTime(float dt)
        {
            float dt2 = dt * dt;

            _F = new float[,] {
                { 1, dt },
                { 0, 1 }
            };

            _Q = (new float[,] {
                { dt2, dt },
                { dt, 1 }
            });
            _Q *= _noiseError;
        }

        private Vector3 SetMeasurement(float value)
        {
            Matrix z = new float[,] {
                { value }
            };

            // Prediction
            Matrix valueExpected = _F * _value;
            _P = (_F * _P) * _F.Transposed + _Q;

            // Correction
            Matrix y = z - _H * valueExpected;
            Matrix S = (_H * _P * _H.Transposed + _R).Inverse;
            Matrix K = _P * _H.Transposed * S;

            _value = valueExpected + K * y;
            _P = (Matrix.Identity(2, 2) - K * _H) * _P;

            return new Vector3(_value[0, 0], _value[0, 1], _value[0, 1]);
        }
    }
}