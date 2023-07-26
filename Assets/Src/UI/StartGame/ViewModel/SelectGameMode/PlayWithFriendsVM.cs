using System.Linq;
using ReactivePropsNs;

namespace Uins
{
    public class PlayWithFriendsVM : BindingVmodel
    {
        private readonly ReactiveProperty<int> _friendsCount = new ReactiveProperty<int>();
        private readonly ReactiveProperty<bool> _hovered = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<int> _onlineFriendsCount = new ReactiveProperty<int>();

        private readonly SelectGameModeContentVM _parentVM;
        public ScrollAreaContentVM ScrollAreaContentVM { get; }

        public IStream<int> FriendsCount => _friendsCount;
        public IStream<int> OnlineFriendsCount => _onlineFriendsCount;
        public IReactiveProperty<bool> Hovered => _hovered;

        public PlayWithFriendsVM(ScrollAreaContentVM scrollAreaContentVM)
        {
            ScrollAreaContentVM = scrollAreaContentVM;
            _friendsCount.Value = 0;
            _onlineFriendsCount.Value = 0;

            var friendsVM = new FriendsVM();
            D.Add(friendsVM);

            friendsVM.Friends.Action(
                D,
                friends =>
                {
                    var friendsWithSessions = friends.Where(friend => !string.IsNullOrEmpty(friend.RealmId)).ToArray();
                    _friendsCount.Value = friendsWithSessions.Length;
                    _onlineFriendsCount.Value = friendsWithSessions.Count(friend => friend.Status == "online");
                });
        }

        public void SetHovered(bool value)
        {
            _hovered.Value = value;
        }
    }
}