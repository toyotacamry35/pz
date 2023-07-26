using System.Collections.Immutable;
using System.Linq;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;

namespace Uins
{
    public class InGameFriendsVM : BindingVmodel
    {
        public ListStream<FriendInfo> Friends { get; }

        public InGameFriendsVM()
        {
            Friends = new ListStream<FriendInfo>();

            var accountEntityVM = new EntityVM<IAccountEntityClientFull>(
                GameState.Instance.ClientClusterRepository,
                GameState.Instance.AccountIdStream
            );
            D.Add(accountEntityVM);

            var charRealmDataTouchable = accountEntityVM.Touchable.Child(D, account => account.CharRealmData);
            var realmOuterRefStream = charRealmDataTouchable.ToStream(D, charRealmData => charRealmData.CurrentRealm);

            var friendsVm = realmOuterRefStream
                .Transform(D, outerRef => outerRef.IsValid ? new FriendsVM(outerRef.Guid.ToString()) : null);
            var friendsList = friendsVm.SubStream(D, vm => vm.Friends);
            var friendInfoList = friendsList.Func(
                D,
                list =>
                    list?
                        .Select(friend => new FriendInfo(friend))
                        .OrderBy(friend => friend.Status == FriendStatus.Online ? 0 : 1)
                        .ToImmutableList()
            );

            friendInfoList.Action(
                D,
                list =>
                {
                    if (list != null)
                    {
                        Friends.Clear();
                        foreach (var friend in list) Friends.Add(friend);
                    }
                }
            );
        }
    }
}