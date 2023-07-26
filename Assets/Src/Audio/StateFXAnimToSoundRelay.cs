using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Audio
{
    public class StateFXAnimToSoundRelay : MonoBehaviour {

        [SerializeField] public AnimationToSoundEventRelay AnimationObject;

        public void WwiseEvent(string evText)
        {
            AkSoundEngine.PostEvent(evText, AnimationObject.gameObject);
        }

    }
}
