using UnityEngine;

namespace TOD
{
    class TimeOfDayCheatResetter : MonoBehaviour
    {
        private void Awake()
        {
            ASkyLighting.TimeOfDayResetCheat();
        }
    }
}
