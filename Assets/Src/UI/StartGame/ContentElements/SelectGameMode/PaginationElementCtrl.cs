using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PaginationElementCtrl : BindingControllerWithUsingProp<IPaginationElementVM>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField]
        private PassiveToggle Toggle;

        protected override void Awake()
        {
            base.Awake();

            Vmodel.SubStream(D, vm => vm.Available)
                .Action(D, available => Toggle.interactable = available);
            Vmodel.SubStream(D, vm => vm.Selected)
                .Action(D, selected => Toggle.SetIsOnWithoutNotify(selected));
        }

        [UsedImplicitly]
        public void OnClick()
        {
            Vmodel.Value?.SetSelected();
        }
    }
}