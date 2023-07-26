using JetBrains.Annotations;
using UnityEngine;

namespace Uins.Tooltips
{
    public class TechnosSideContrTooltipDescr : BaseTooltipDescription
    {
        [SerializeField, UsedImplicitly]
        private TechnosSideContr _technosSideContr;

        public override bool HasDescription => _technosSideContr != null && _technosSideContr.HasBuildRecipes;

        public override object Description => HasDescription ? _technosSideContr : null;

        private void Awake()
        {
            _technosSideContr.AssertIfNull(nameof(_technosSideContr), gameObject);
        }
    }
}