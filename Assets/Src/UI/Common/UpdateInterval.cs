using System;
using UnityEngine;

namespace Uins
{
    [Serializable]
    public class UpdateInterval
    {
        public float Interval;

        [NonSerialized]
        private float _lastTime;

        public bool IsItTime()
        {
            bool isItTime = false;
            if (Time.time - _lastTime > Interval)
            {
                _lastTime = Time.time;
                isItTime = true;
            }

            return isItTime;
        }
    }
}