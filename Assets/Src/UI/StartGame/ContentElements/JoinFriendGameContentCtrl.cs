using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Assets.ReactiveProps;
using Assets.Src.App;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using PzLauncher.Models.Dto;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class JoinFriendGameContentCtrl : BaseStartGameContentCtrl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField]
        private LocalizationKeyProp JoinFriendDoom;

        [SerializeField]
        private GameModeElementCtrl GameModeElementCtrl;

        [SerializeField, UsedImplicitly]
        private TimeLeftCtrl TimeLeftCtrl;

        [SerializeField, UsedImplicitly]
        private Transform PlayingFriendsContainer;

        [SerializeField, UsedImplicitly]
        private PlayingFriendCtrl PlayingFriendPrefab;

        [Binding, UsedImplicitly]
        public bool IsPlayAvailable { get; set; }

        private readonly ReactiveProperty<string> _selectedFriendId = new ReactiveProperty<string>();

        private ListStream<PlayingFriendVM> _playingFriendVms = new ListStream<PlayingFriendVM>();

        protected override void Start()
        {
            base.Start();

            foreach (Transform child in PlayingFriendsContainer.transform)
                Destroy(child.gameObject);

            var friendsVM = Vmodel.Transform(D, vm => vm != null ? new FriendsVM() : null);
            var friendsList = friendsVM.SubStream(D, vm => vm.Friends);
            var sortedFriendsList = friendsList.Func(
                D,
                list => list?.OrderBy(friend => friend.Status == "online" ? 0 : 1).ToImmutableList()
            );
            var friends = ToListStream(sortedFriendsList);
            var friendsVMs = friends.Transform(D, friend => friend != null ? new PlayingFriendVM(friend, _selectedFriendId) : null);
            _playingFriendVms = friendsVMs.Where(D, vm => vm?.RealmQueryDef != null);

            friendsList.Action(
                D,
                list =>
                {
                    if (list != null && list.Count > 0)
                    {
                        var currentId = _selectedFriendId.Value;
                        if (string.IsNullOrEmpty(currentId) || list.Any(friend => friend.UserId == currentId))
                            _selectedFriendId.Value = list.First().UserId;
                    }
                });

            var selectedFriend = CreateSelectedStream(D, _selectedFriendId, _playingFriendVms);

            var timeLeftVm = selectedFriend.Transform(D, vm => vm != null ? new TimeLeftVM(vm.RealmTimeLeftSec) : null);
            TimeLeftCtrl.BindVM(D, timeLeftVm);

            var currentRealmVM = selectedFriend.Transform(
                D,
                vm => vm != null ? new RealmRulesVM(vm.RealmRulesDef, vm.RealmActiveStream) : null
            );
            GameModeElementCtrl.BindVM(D, currentRealmVM);

            var playingFriendsPool = new BindingControllersPool<PlayingFriendVM>(PlayingFriendsContainer, PlayingFriendPrefab);
            playingFriendsPool.Connect(_playingFriendVms);
            D.Add(new DisposeAgent(playingFriendsPool.Disconnect));

            Bind(selectedFriend.SubStream(D, vm => vm.RealmActiveStream), () => IsPlayAvailable);
        }

        private static IStream<PlayingFriendVM> CreateSelectedStream(
            DisposableComposite disposable,
            IReactiveProperty<string> selectedFriendId,
            ListStream<PlayingFriendVM> playingFriendVms)
        {
            var localD = disposable.CreateInnerD();
            var selectedFriend = new ReactiveProperty<PlayingFriendVM>();
            localD.Add(selectedFriend);

            selectedFriendId.Subscribe(localD, e => UpdateSelected(), Dispose);
            playingFriendVms.InsertStream.Action(localD, e => UpdateSelected());
            playingFriendVms.RemoveStream.Action(localD, e => UpdateSelected());
            playingFriendVms.ChangeStream.Subscribe(localD, e => UpdateSelected(), Dispose);

            void UpdateSelected()
            {
                selectedFriend.Value = playingFriendVms.FirstOrDefault(vm => vm.UserId == selectedFriendId.Value);
            }

            void Dispose()
            {
                selectedFriend.Value = null;
                disposable.DisposeInnerD(localD);
            }

            return selectedFriend;
        }

        private ListStream<Friend> ToListStream(IStream<ImmutableList<Friend>> friendsList)
        {
            var friends = new ListStream<Friend>();
            friendsList.Action(
                D,
                list =>
                {
                    _playingFriendVms.Clear();
                    if (list != null)
                        foreach (var friend in list)
                            friends.Add(friend);
                }
            );
            return friends;
        }

        [UsedImplicitly]
        public void OnPlayButton()
        {
            var friendId = _selectedFriendId.Value;
            var startGameWindowVM = Vmodel.Value;
            if (startGameWindowVM != null && friendId != null && IsPlayAvailable)
                RunSingleCommandAsync(PlayWithFriends(friendId, startGameWindowVM.StartGameNode, JoinFriendDoom.LocalizedString));
        }

        private static async Task PlayWithFriends(string friendId, StartGameGuiNode startGameGuiNode, LocalizedString doom)
        {
            await UnityQueueHelper.RunInUnityThread(
                () => { startGameGuiNode.OpenDoomDialog(doom); }
            );
            if (PzApiHolder.Connected)
                try
                {
                    var response = await PzApiHolder.Communicator.JoinFriend(new JoinFriendRequestData {FriendId = friendId});
                    if (response.Success)
                    {
                        AppStart.Restart();
                        return;
                    }

                    Logger.IfError()?.Message($"Error Join Friend {friendId} {response.Error}").Write();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"Error Join Friend {friendId}").Exception(e).Write();
                }
            else
                Logger.IfWarn()?.Message("PzApi Not Connected Can't Play With Friends").Write();

            await UnityQueueHelper.RunInUnityThread(
                startGameGuiNode.CloseDoomDialog
            );
        }

        [UsedImplicitly]
        public void OnBackButton()
        {
            Vmodel.Value?.SetJoinFriendsMode(false);
        }
    }
}