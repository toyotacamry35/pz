using UnityEngine;

namespace Assets.Src.Audio
{
    public class AnimationToSoundEventRelay : MonoBehaviour
    {
        public void WwiseEvent(string evText)
        {
            AkSoundEngine.PostEvent(evText, gameObject);
        }
    }
}
