using Core.Environment.Logging.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace L10n
{
    [DisallowMultipleComponent]
    public class LocalizedTextMeshPro : LocalizedText
    {
        [FormerlySerializedAs("_meshProUgui")]
        public TextMeshProUGUI Target;

        private bool _isNonLocalized;


        //=== Protected =======================================================

        protected override void OnAwake()
        {
            if (GetComponent<NonLocalized>() != null)
            {
                _isNonLocalized = true;
                return;
            }

            base.OnAwake();
            if (Target == null)
            {
                Target = GetComponent<TextMeshProUGUI>();
                if (!Target.AssertIfNull(nameof(Target)))
                    Logger.IfError()?.Message($"Unassigned {nameof(Target)} on {gameObject.transform.FullName()}", gameObject).Write();
            }

            ValueChanged += OnValueChanged;
            OnValueChanged(Value);
        }

        protected override void WithinOnDestroy()
        {
            if (_isNonLocalized)
                return;

            ValueChanged -= OnValueChanged;
        }


        //=== Private =========================================================

        private void OnValueChanged(string newValue)
        {
            if (Target != null)
                Target.text = newValue;
        }
    }
}