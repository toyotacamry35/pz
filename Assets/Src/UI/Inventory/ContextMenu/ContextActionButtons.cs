using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class ContextActionButtons : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Transform _buttonsRoot;

        [SerializeField, UsedImplicitly]
        private ContextActionViewModel _contextButtonPrefab;

        [SerializeField, UsedImplicitly]
        private bool _revertButtonsOrder;

        private List<ContextActionViewModel> _buttons = new List<ContextActionViewModel>();


        //=== Unity ===========================================================

        private void Awake()
        {
            _buttonsRoot.AssertIfNull(nameof(_buttonsRoot));
            _contextButtonPrefab.AssertIfNull(nameof(_contextButtonPrefab));
        }


        //=== Public ==========================================================

        public void SetContextButtons(List<ContextMenuItemData> contextMenuItems = null)
        {
            HideAllButtons();
            if (contextMenuItems == null || contextMenuItems.Count == 0)
                return;

            if (contextMenuItems.Count > _buttons.Count)
                for (int i = 0, delta = contextMenuItems.Count - _buttons.Count; i < delta; i++)
                    GetNewContextActionVm();

            int j = _revertButtonsOrder ? contextMenuItems.Count - 1 : 0;
            int jDelta = _revertButtonsOrder ? -1 : 1;
            for (int i = 0, len = contextMenuItems.Count; i < len; i++, j += jDelta)
            {
                var contextMenuItem = contextMenuItems[i];
                var contextActionViewModel = _buttons[j];
                contextActionViewModel.IsUsed = true;
                contextActionViewModel.Title = contextMenuItem.Title;
                contextActionViewModel.IsDefaultAction = contextMenuItem.IsActive;
                contextActionViewModel.IsDisabled = contextMenuItem.IsDisabled;
                contextActionViewModel.SetAction(contextMenuItem.Action, contextMenuItem.ActionParams);
            }
        }

        private ContextActionViewModel GetNewContextActionVm()
        {
            var contextActionViewModel = Instantiate(_contextButtonPrefab, _buttonsRoot);
            contextActionViewModel.name = _contextButtonPrefab.name + _buttons.Count;
            _buttons.Add(contextActionViewModel);
            return contextActionViewModel;
        }

        private void HideAllButtons()
        {
            foreach (var contextActionViewModel in _buttons)
                contextActionViewModel.IsUsed = false;
        }
    }
}