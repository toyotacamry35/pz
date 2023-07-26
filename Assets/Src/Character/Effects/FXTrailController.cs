using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

namespace Assets.Src.Character.Effects
{
    public class FXTrailController : MonoBehaviour
    {
        public XWeaponTrail tailTrail;
        public float fadeoutTime;

        private void Start()
        {
            tailTrail.Deactivate();
        }

        public void OnTrailStart(string StrikeType)
        {
            switch (StrikeType)
            {                
                case "TailStrike":
                    {
                        tailTrail.Activate();
                        break;
                    }
                default:
                    {
                        Debug.LogError("No such event " + StrikeType + "registered in FXTrailController");
                        break;
                    }
            }
        }

        public void OnTrailStop(string StrikeType)
        {
            switch (StrikeType)
            {
                case "TailStrike":
                    {
                        tailTrail.StopSmoothly(fadeoutTime);
                        break;
                    }
                default:
                    {
                        Debug.LogError("No such event " + StrikeType + "registered in FXTrailController");
                        break;
                    }
            }
        }
    }
}