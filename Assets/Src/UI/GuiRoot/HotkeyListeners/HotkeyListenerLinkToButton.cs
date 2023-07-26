using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class HotkeyListenerLinkToButton : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Button _button;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _hotkeyListener;


        private void Awake()
        {
            if (_button.AssertIfNull(nameof(_button)) ||
                _hotkeyListener.AssertIfNull(nameof(_hotkeyListener)))
            {
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (_hotkeyListener.IsFired())
            {
                _button.onClick?.Invoke();
            }
        }
    }
}