using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ToggleGroupWithIndex : MonoBehaviour
    {
        public delegate void ChangedEventHandler(int newIndex);

        public event ChangedEventHandler OnIndexChanged;

        public Toggle[] Toggles;
        private bool _ignoreChangesTime;


        //=== Props ===============================================================

        private int _selectedIndex;

        public int SelectedIndex
        {
            get => _selectedIndex;
            private set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    SwitchAllToggles(_selectedIndex);
                    OnIndexChanged?.Invoke(_selectedIndex);
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            if (Toggles == null || Toggles.Length == 0)
            {
                Debug.LogError($"{nameof(Toggles)} is empty", gameObject);
                return;
            }

            _selectedIndex = -1;
            for (int i = 0; i < Toggles.Length; i++)
            {
                var toggle = Toggles[i];
                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener(OnSelectedToggleChanged);
                }
                else
                {
                    Debug.LogError($"{nameof(Toggles)}[{i}] is null!", gameObject);
                }
            }

            var selectedToggleIndex = GetSelectedToggleIndex();
            if (selectedToggleIndex < 0)
                selectedToggleIndex = 0;

            SelectedIndex = selectedToggleIndex;
        }


        //=== Private =============================================================

        private void OnSelectedToggleChanged(bool isOn)
        {
            if (_ignoreChangesTime)
                return;

            if (isOn)
            {
                var anotherActiveToggleIndex = GetSelectedToggleIndex(SelectedIndex);
                if (anotherActiveToggleIndex > -1)
                    SelectedIndex = anotherActiveToggleIndex;
            }
            else
            {
                var selectedToggleIndex = GetSelectedToggleIndex();
                if (selectedToggleIndex < 0)
                {
                    selectedToggleIndex = 0;
                    _selectedIndex = -1;
                }

                SelectedIndex = selectedToggleIndex;
            }
        }

        private void SwitchAllToggles(int isOnToggleIndex)
        {
            _ignoreChangesTime = true;
            for (int i = 0; i < Toggles.Length; i++)
                Toggles[i].isOn = i == isOnToggleIndex;
            _ignoreChangesTime = false;
        }

        private int GetSelectedToggleIndex(int ignoredIndex = -1)
        {
            for (int i = 0; i < Toggles.Length; i++)
            {
                if (i != ignoredIndex && Toggles[i].isOn)
                    return i;
            }

            return -1;
        }
    }
}