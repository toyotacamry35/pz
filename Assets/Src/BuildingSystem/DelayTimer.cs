using SharedCode.Utils;
using System;

namespace Assets.Src.BuildingSystem
{
    public class DelayTimer
    {
        private bool inProgress = false;
        private long timestamp = 0;
        private float timer = 0.0f;

        public bool IsInProgress()
        {
            if (!inProgress) { return false; }
            inProgress = (DateTime.UtcNow < UnixTimeHelper.DateTimeFromUnix(timestamp).AddSeconds(timer));
            return inProgress;
        }

        public void Set(float delay)
        {
            if (delay > 0)
            {
                inProgress = true;
                timestamp = DateTime.UtcNow.ToUnix();
                timer = delay;
            }
        }
    }
}