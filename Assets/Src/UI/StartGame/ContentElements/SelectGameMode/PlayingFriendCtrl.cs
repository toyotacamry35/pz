using JetBrains.Annotations;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PlayingFriendCtrl : BindingController<PlayingFriendVM>
    {
        [Binding, UsedImplicitly]
        public string FriendName { get; set; }

        [Binding, UsedImplicitly]
        public bool Online { get; set; }
        
        [Binding, UsedImplicitly]
        public bool Selected { get; set; }
        
        [Binding, UsedImplicitly]
        public bool Hovered { get; set; }

        private void Awake()
        {
            Bind(Vmodel.SubStream(D, vm => vm.FriendName), () => FriendName);
            Bind(Vmodel.SubStream(D, vm => vm.Online), () => Online);
            Bind(Vmodel.SubStream(D, vm => vm.Selected), () => Selected);
            Bind(Vmodel.SubStream(D, vm => vm.Hovered), () => Hovered);
        }
        
        [UsedImplicitly]
        public void OnClick()
        {
            Vmodel.Value?.SetSelected();
        }

        [UsedImplicitly]
        public void OnPointerEnter()
        {
            Vmodel.Value?.SetHovered(true);
        }

        [UsedImplicitly]
        public void OnPointerExit()
        {
            Vmodel.Value?.SetHovered(false);
        }

    }
}