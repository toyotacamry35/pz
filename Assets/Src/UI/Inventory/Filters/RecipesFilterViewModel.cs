using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    public enum PlaFilter
    {
        All,
        Avaliable
    }

    public delegate void PlaFilterEventHandler(PlaFilter plaFilter);

    [Binding]
    public class RecipesFilterViewModel : FiltrableTypeFilterViewModel
    {
        public event PlaFilterEventHandler PlaFilterChanged;

        [SerializeField, UsedImplicitly]
        private bool _useFiltrableTypeToggles = true;

        [SerializeField, UsedImplicitly]
        protected ToggleGroupWithIndex PlaToggleGroupWithIndex;

        [SerializeField, UsedImplicitly]
        private PlaFilter[] _plaFilterValues;


        //=== Props ===========================================================

        public PlaFilter CurrentPlaFilter
        {
            get
            {
                if (_plaFilterValues == null || _plaFilterValues.Length == 0)
                    return 0;

                var index = Mathf.Min(PlaToggleGroupWithIndex.SelectedIndex, _plaFilterValues.Length - 1);
                return _plaFilterValues[index];
            }
        }


        //=== Protected =======================================================

        protected override bool TryInit()
        {
            if ((_useFiltrableTypeToggles && !base.TryInit()) ||
                PlaToggleGroupWithIndex.AssertIfNull(nameof(PlaToggleGroupWithIndex)))
                return false;

            PlaToggleGroupWithIndex.OnIndexChanged += OnPlaToggleIndexChanged;
            return true;
        }


        //=== Private =========================================================

        private void OnPlaToggleIndexChanged(int newIndex)
        {
            PlaFilterChanged?.Invoke(CurrentPlaFilter);
        }
    }
}