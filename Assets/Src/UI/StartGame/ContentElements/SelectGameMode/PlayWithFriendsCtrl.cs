using Assets.ReactiveProps;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PlayWithFriendsCtrl : BindingController<PlayWithFriendsVM>
    {
        // private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [Binding, UsedImplicitly]
        public bool Hovered { get; set; }

        [Binding, UsedImplicitly]
        public bool Available { get; set; }

        [Binding, UsedImplicitly]
        public int FriendsCount { get; set; }

        [Binding, UsedImplicitly]
        public int OnlineFriendsCount { get; set; }

        protected void Awake()
        {
            var friendsCountStream = Vmodel.SubStream(D, vm => vm.FriendsCount);
            Bind(friendsCountStream, () => FriendsCount);
            Bind(Vmodel.SubStream(D, vm => vm.OnlineFriendsCount), () => OnlineFriendsCount);
            Bind(Vmodel.SubStream(D, vm => vm.Hovered), () => Hovered);
            Bind(friendsCountStream.Func(D, count => count > 0), () => Available);
        }

        [UsedImplicitly]
        public void OnClick()
        {
            var startGameWindowVM = Vmodel.Value?.ScrollAreaContentVM?.SelectGameModeContentVM?.StartGameWindowVM;
            if (Available)
                startGameWindowVM?.SetJoinFriendsMode(true);
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