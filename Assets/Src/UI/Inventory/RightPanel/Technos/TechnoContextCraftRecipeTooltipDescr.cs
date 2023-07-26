using JetBrains.Annotations;
using UnityEngine;

namespace Uins.Tooltips
{
    public class TechnoContextCraftRecipeTooltipDescr : BaseTooltipDescription
    {
        [SerializeField, UsedImplicitly]
        private TechnoContextCraftRecipeContr _technoContextCraftRecipeContr;

        public override bool HasDescription => _technoContextCraftRecipeContr != null;

        public override object Description => HasDescription ? _technoContextCraftRecipeContr : null;

        private void Awake()
        {
            _technoContextCraftRecipeContr.AssertIfNull(nameof(_technoContextCraftRecipeContr), gameObject);
        }
    }
}