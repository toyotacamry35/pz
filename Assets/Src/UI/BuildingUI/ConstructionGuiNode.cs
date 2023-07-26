using System;
using ColonyDI;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class ConstructionGuiNode : DependencyNodeWithChildren
    {
        public event Action<bool> ReadyForWorkingChanged;

        [SerializeField, UsedImplicitly]
        private GameObject[] _mustBeActive;

        public ConstructionMainWindow MainWindow;

        [SerializeField, UsedImplicitly]
        private ConstructionSelectionWindow _selectionWindow;

        [Dependency]
        private InventoryNode InventoryNode { get; set; }

        private void Awake()
        {
            _selectionWindow.AssertIfNull(nameof(_selectionWindow));

            if (!_mustBeActive.IsNullOrEmptyOrHasNullElements(nameof(_mustBeActive)))
                for (int i = 0; i < _mustBeActive.Length; i++)
                    _mustBeActive[i]?.SetActive(true);
        }

        public override void AfterDependenciesInjected()
        {
            _selectionWindow.CharacterItemsNotifier = InventoryNode.CharacterItemsNotifier;
        }
    }
}