using System;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SelectGameModeContentCtrl : BaseStartGameContentCtrl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private ScrollAreaContentCtrl ScrollAreaContentCtrl;

        [Binding, UsedImplicitly]
        public bool IsPlayAvailable { get; set; }

        public RealmRulesQueryDef CurrentQueryDef { get; set; }

        private ReactiveProperty<SelectGameModeContentVM> _selectGameModeContentVM;

        protected override void Start()
        {
            base.Start();

            _selectGameModeContentVM = Vmodel.Transform(D, vm => vm != null ? new SelectGameModeContentVM(vm) : null);

            var selectedQuery = _selectGameModeContentVM.SubStream(D, vm => vm.SelectedRealmRulesQuery);
            var playAvailable = selectedQuery.Func(D, vm => vm?.State.Value.Available ?? false);
            Bind(playAvailable, () => IsPlayAvailable);

            var currentQuery = selectedQuery.Func(D, vm => vm?.Def);
            Bind(currentQuery, () => CurrentQueryDef);

            var scrollAreaContentVM = _selectGameModeContentVM.Transform(D, vm => vm != null ? new ScrollAreaContentVM(vm) : null);
            ScrollAreaContentCtrl.BindVM(D, scrollAreaContentVM);
        }

        [UsedImplicitly]
        public void OnPlayButton()
        {
            if (Vmodel.HasValue)
            {
                var vm = Vmodel.Value;
                var startGameNode = vm.StartGameNode;
                var lobbyGuiNode = vm.LobbyGuiNode;
                RunSingleCommandAsync(startGameNode.EnterNewGame(lobbyGuiNode, CurrentQueryDef));
            }
        }

        [UsedImplicitly]
        public void OnBackButton()
        {
            try
            {
                if (Vmodel.HasValue)
                {
                    var vm = Vmodel.Value;
                    var startGameNode = vm.StartGameNode;
                    startGameNode.SetState(StartGameGuiNode.State.Account);
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message("Error Back Button").Exception(e).Write();
            }
        }
    }
}