using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class WwiseL10ComponentTest : MonoBehaviour
    {
        public void OnLocalizedSoundButton()
        {
            UI.CallerLogDefault();
            var localizedEvent = SoundControl.Instance.LocalizedEvent;
            if (!localizedEvent.AssertIfNull(nameof(localizedEvent)))
                localizedEvent.Post(transform.root.gameObject);
        }
    }
}