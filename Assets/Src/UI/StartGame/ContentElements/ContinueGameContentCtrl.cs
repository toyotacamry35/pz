using System;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ContinueGameContentCtrl : BaseStartGameContentCtrl
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField]
        private GameModeElementCtrl GameModeElementCtrl;

        [SerializeField, UsedImplicitly]
        private TimeLeftCtrl TimeLeftCtrl;

        [SerializeField]
        private LocalizationKeyProp TimeoutTitleHolder;

        [SerializeField]
        private LocalizationKeyProp LeaveConfirmationQuestion;

        [Binding, UsedImplicitly]
        public bool Available { get; set; }

        protected override void Start()
        {
            base.Start();

            var currentRealmRulesDef = Vmodel.SubStream(D, vm => vm.RealmVM.RealmRulesDef);
            var realmActiveStream = Vmodel.SubStream(D, vm => vm.RealmVM.RealmActiveStream);

            var currentRealmVM = currentRealmRulesDef.Transform(D, def => def != null ? new RealmRulesVM(def, realmActiveStream) : null);
            GameModeElementCtrl.BindVM(D, currentRealmVM);

            TimeLeftCtrl.BindVM(D, Vmodel.Transform(D, vm => vm != null ? new TimeLeftVM(vm.RealmVM.RealmTimeLeftSec) : null));

            realmActiveStream.Action(D, b => SetTitle(b ? TitleHolder : TimeoutTitleHolder, SemanticContext.Primary));
            Bind(realmActiveStream, () => Available);
        }

        [UsedImplicitly]
        public void OnPlayButton()
        {
            if (Vmodel.HasValue)
            {
                var vm = Vmodel.Value;
                var startGameNode = vm.StartGameNode;
                var lobbyGuiNode = vm.LobbyGuiNode;
                RunSingleCommandAsync(startGameNode.EnterCurrentGame(lobbyGuiNode));
            }
        }

        [UsedImplicitly]
        public void OnGiveUpButton()
        {
            if (Vmodel.HasValue)
            {
                var vm = Vmodel.Value;
                var startGameNode = vm.StartGameNode;
                if (Available)
                    startGameNode.OpenConfirmationDialog(
                        LeaveConfirmationQuestion.LocalizedString,
                        async () => { await startGameNode.LeaveCurrentRealm(); }
                    );
                else
                    RunSingleCommandAsync(startGameNode.LeaveCurrentRealm());
            }
        }
    }
}