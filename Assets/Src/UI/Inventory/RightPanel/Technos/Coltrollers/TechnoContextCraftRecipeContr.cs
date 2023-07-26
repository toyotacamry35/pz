using Assets.ColonyShared.SharedCode.Aspects.Craft;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TechnoContextCraftRecipeContr : BindingControllerWithUsingProp<TechnoContextCraftRecipeVmodel>, ICraftRecipeSource
    {
        //=== Props ===========================================================

        [Binding]
        public Sprite BlueprintIcon { get; private set; }

        [Binding]
        public bool IsActivated { get; private set; }

        public CraftRecipeDef CraftRecipeDef => Vmodel.Value?.CraftRecipeDef;


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            var blueprintIconStream = Vmodel.Func(D, vm => vm?.CraftRecipeDef?.BlueprintIcon.Target);
            Bind(blueprintIconStream, () => BlueprintIcon);
            var isAvailableStream = Vmodel.SubStream(D, vm => vm.IsActivatedStream);
            Bind(isAvailableStream, () => IsActivated);
        }


        //=== Public ==============================================================

        [UsedImplicitly]
        public void OnClick()
        {
            if (IsActivated && CraftRecipeDef != null)
                Vmodel.Value?.OnClick();
        }
    }
}