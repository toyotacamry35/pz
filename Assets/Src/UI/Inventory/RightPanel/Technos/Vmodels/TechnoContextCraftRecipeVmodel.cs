using System;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using ReactivePropsNs;

namespace Uins
{
    public class TechnoContextCraftRecipeVmodel
    {
        public CraftRecipeDef CraftRecipeDef { get; }

        public IStream<bool> IsActivatedStream { get; }

        private Action<CraftRecipeDef> _clickAction;

        public TechnoContextCraftRecipeVmodel(CraftRecipeDef craftRecipeDef, IStream<bool> isActivatedStream, Action<CraftRecipeDef> clickAction)
        {
            CraftRecipeDef = craftRecipeDef;
            IsActivatedStream = isActivatedStream;
            _clickAction = clickAction;
            CraftRecipeDef.AssertIfNull(nameof(CraftRecipeDef));
            IsActivatedStream.AssertIfNull(nameof(IsActivatedStream));
        }

        public void OnClick()
        {
            _clickAction?.Invoke(CraftRecipeDef);
        }
    }
}