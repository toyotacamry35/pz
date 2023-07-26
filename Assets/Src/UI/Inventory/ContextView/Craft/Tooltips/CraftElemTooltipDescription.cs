using JetBrains.Annotations;
using Uins.Inventory;
using UnityEngine;

namespace Uins.Tooltips
{
    public class CraftElemTooltipDescription : BaseTooltipDescription
    {
        [SerializeField, UsedImplicitly]
        protected CraftRecipeElementUI _craftRecipeElement;


        //=== Props ===========================================================

        public override bool HasDescription => _craftRecipeElement != null && !_craftRecipeElement.IsEmpty;

        public override object Description => HasDescription ? _craftRecipeElement : null;


        //=== Unity ===========================================================

        private void Awake()
        {
            _craftRecipeElement.AssertIfNull(nameof(_craftRecipeElement), gameObject);
        }
    }
}