using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Entities;

namespace Uins
{
    public class StartGameWindowVM : BindingVmodel
    {
        public GameState GameState { get; }
        public StartGameGuiNode StartGameNode { get; }
        public LobbyGuiNode LobbyGuiNode { get; }

        public IStream<StartGameWindowStateEnum> StartGameWindowState => _startGameWindowStateRp;
        public ITouchable<IAccountEntityClientFull> TouchableAccount { get; }
        public ITouchable<ICharRealmDataClientFull> CharRealmDataTouchable { get; }

        private readonly ReactiveProperty<StartGameWindowStateEnum> _startGameWindowStateRp;

        private ReactiveProperty<RealmRulesVM> _currentRealmRulesVM;
        private bool _joinFriendsValue;
        private bool _realmExistsValue;
        private RealmCharStateEnum _realmCharStateValue;
        private bool _realmCharStateReady;
        private bool _realmExistsReady;

        public RealmVM RealmVM { get; }

        public StartGameWindowVM(
            StartGameGuiNode startGameNode,
            GameState gameState,
            LobbyGuiNode lobbyGuiNode,
            ITouchable<IAccountEntityClientFull> touchableAccount)
        {
            StartGameNode = startGameNode;
            StartGameNode.AssertIfNull(nameof(StartGameNode));
            LobbyGuiNode = lobbyGuiNode;
            LobbyGuiNode.AssertIfNull(nameof(LobbyGuiNode));
            GameState = gameState;
            GameState.AssertIfNull(nameof(GameState));

            TouchableAccount = touchableAccount;
            _startGameWindowStateRp = new ReactiveProperty<StartGameWindowStateEnum> {Value = StartGameWindowStateEnum.Loading};
            _joinFriendsValue = false;

            //Init
            // var loginIdStream = _gameState.LoginEntityIdStream.WhereProp(D, accountId => !accountId.Equals(Guid.Empty));
            var repositoryStream = GameState.ClientClusterRepository.WhereProp(D, repository => repository != null);

            CharRealmDataTouchable = TouchableAccount.Child(D, account => account.CharRealmData);
            var realmOuterRefStream = CharRealmDataTouchable.ToStream(D, charRealmData => charRealmData.CurrentRealm);
            var realmCharStateStream = CharRealmDataTouchable.ToStream(D, charRealmData => charRealmData.CurrentRealmCharState);

            RealmVM = new RealmVM(repositoryStream, realmOuterRefStream);
            D.Add(RealmVM);

            ConnectStateStream(realmCharStateStream, RealmVM.RealmExistsStream);
        }

        public void SetJoinFriendsMode(bool value)
        {
            _joinFriendsValue = value;
            UpdateState();
        }

        private void ConnectStateStream(IStream<RealmCharStateEnum> realmCharStateStream, IStream<bool> realmExistsStream)
        {
            _realmCharStateValue = default;
            _realmExistsValue = false;
            _realmCharStateReady = false;
            _realmExistsReady = false;

            realmCharStateStream.Action(
                D,
                value =>
                {
                    _realmCharStateValue = value;
                    _realmCharStateReady = true;
                    UpdateState();
                });
            realmExistsStream.Action(
                D,
                value =>
                {
                     _realmExistsValue = value;
                    _realmExistsReady = true;
                    UpdateState();
                });
        }

        private void UpdateState()
        {
            if (_realmCharStateReady && _realmExistsReady)
                _startGameWindowStateRp.Value = GetState(_realmCharStateValue, _joinFriendsValue, _realmExistsValue);
        }

        // private void ConnectStateStream(IStream<RealmCharStateEnum> realmCharStateStream, IStream<bool> realmExistsStream)
        // {
        //     var stateStream = realmCharStateStream
        //         .Zip(D, realmExistsStream)
        //         .Zip(D, _joinFriendsMode)
        //         .Func(D, GetState);
        //
        //     stateStream.Bind(D, _startGameWindowStateRp);
        // }

        private static StartGameWindowStateEnum GetState(RealmCharStateEnum realmCharState, bool joinFriendsMode, bool realmExists)
        {
            switch (realmCharState)
            {
                case RealmCharStateEnum.Inactive:
                    return joinFriendsMode
                        ? StartGameWindowStateEnum.JoinFriendsMode
                        : StartGameWindowStateEnum.SelectGameMode;
                case RealmCharStateEnum.Active:
                    return realmExists
                        ? StartGameWindowStateEnum.ContinueGame
                        : StartGameWindowStateEnum.Reward;
                case RealmCharStateEnum.Surrendered:
                case RealmCharStateEnum.Success:
                    return StartGameWindowStateEnum.Reward;
                default:
                    return StartGameWindowStateEnum.Loading;
            }
        }

        public override void Dispose()
        {
            _startGameWindowStateRp?.Dispose();
            base.Dispose();
        }
    }
}