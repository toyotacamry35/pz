using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Tools
{
    public static class CoroutineAwaiters
    {
        const float _waitTimeQuarter = 0.8f;
        const float _waitTimeDeltaStepQuarter = 0.05f;
        const float _waitTime = 1.5f;
        const float _wait5Time = 5f;
        const float _waitTimeDeltaStep = 0.1f;

    #region Are used to avoid allocation, but have randomizing
        public static WaitForSeconds[] QuarterAroundSecond = new WaitForSeconds[]
        {
            new WaitForSeconds(_waitTimeQuarter),
            new WaitForSeconds(_waitTimeQuarter + _waitTimeDeltaStepQuarter),
            new WaitForSeconds(_waitTimeQuarter + _waitTimeDeltaStepQuarter * 2),
            new WaitForSeconds(_waitTimeQuarter + _waitTimeDeltaStepQuarter * 3),
            new WaitForSeconds(_waitTimeQuarter + _waitTimeDeltaStepQuarter * 4)
        };
        public static WaitForSeconds[] SecondAndAHalf = new WaitForSeconds[]
        {
            new WaitForSeconds(_waitTime),
            new WaitForSeconds(_waitTime + _waitTimeDeltaStep),
            new WaitForSeconds(_waitTime + _waitTimeDeltaStep * 2),
            new WaitForSeconds(_waitTime + _waitTimeDeltaStep * 3),
            new WaitForSeconds(_waitTime + _waitTimeDeltaStep * 4)
        };
        public static WaitForSeconds[] FiveSeconds = new WaitForSeconds[]
        {
            new WaitForSeconds(_wait5Time),
            new WaitForSeconds(_wait5Time + _waitTimeDeltaStep),
            new WaitForSeconds(_wait5Time + _waitTimeDeltaStep * 2),
            new WaitForSeconds(_wait5Time + _waitTimeDeltaStep * 3),
            new WaitForSeconds(_wait5Time + _waitTimeDeltaStep * 4)
        };
    #endregion Are used to avoid allocation, but have randomizing

        static List<WaitForSeconds[]> _ticks = new List<WaitForSeconds[]>();
        public static System.Random Random = new System.Random();
        public static WaitForSeconds GetTick(int tick)
        {
            while (_ticks.Count <= tick)
                _ticks.Add(null);

            var ticks = _ticks[tick];
            if (ticks == null)
            {
                int steps = 10;
                ticks = _ticks[tick] = new WaitForSeconds[steps];
                for (int i = 0; i < steps; i++)
                    ticks[i] = new WaitForSeconds((float)tick + i * _waitTimeDeltaStep);
            }
            return ticks[Random.Next(0, ticks.Length)];

        }
    }
}
