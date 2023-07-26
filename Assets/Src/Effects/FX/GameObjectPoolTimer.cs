using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class GameObjectPoolTimer
    {
        private float _startedMilliseconds;
        
        private static float UpdateInterval = 0.5f;

        private float _averageFps = 0.0f;
        private int _frame;

        private float _accumulatedFpsOnInterval = 0.0f;
        private float _framesOnInterval = 0.0f;
        private float _intervalTimeLeft = 0.0f;
        public float AverageFPS => _averageFps;

        public static float ElapsedTimeFromStartTime(float startedMilliseconds)
        {
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            return realtimeSinceStartup < startedMilliseconds ? float.MaxValue - startedMilliseconds + realtimeSinceStartup : realtimeSinceStartup - startedMilliseconds;
        }

        public float ElapsedMilliseconds()
        {
            return ElapsedTimeFromStartTime(_startedMilliseconds);
        }

        public void Reset()
        {
            _startedMilliseconds = Time.realtimeSinceStartup;
        }

        public void Tick()
        {
            var frameTime = Time.unscaledDeltaTime;
            var fps = 1 / frameTime;
            _accumulatedFpsOnInterval += fps;
            _framesOnInterval += 1.0f;
            _intervalTimeLeft -= frameTime;

            if (_intervalTimeLeft <= 0)
            {
                _averageFps = _accumulatedFpsOnInterval / _framesOnInterval;
                
                _intervalTimeLeft = UpdateInterval;
                
                _accumulatedFpsOnInterval = 0.0f;
                _framesOnInterval = 0.0f;
            }
        }
    }
}