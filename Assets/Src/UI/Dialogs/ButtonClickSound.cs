using Uins.Sound;
using UnityEngine;

namespace Uins
{
    public class ButtonClickSound : MonoBehaviour
    {
        public void PlaySound()
        {
            SoundControl.Instance.ButtonSmall.Post(gameObject);
        }
    }
}