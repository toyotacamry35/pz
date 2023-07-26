using System.Linq;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class StartGameWindowCtrl : BindingController<StartGameWindowVM>
    {
        [SerializeField]
        private BaseStartGameContentCtrl[] ContentCtrls;

        [SerializeField]
        private SemanticColorsHolder TitleSemanticColors;

        [Binding]
        public LocalizedString WindowTitle { get; set; }

        [Binding]
        public Color WindowTitleColor { get; set; }

        private void Start()
        {
            if (ContentCtrls.IsNullOrEmptyOrHasNullElements(nameof(ContentCtrls), gameObject))
                return;

            var windowStateStream = Vmodel.SubStream(D, vm => vm.StartGameWindowState);
            var contentCtrlStream = windowStateStream.Func(
                D,
                state => ContentCtrls.FirstOrDefault(ctrl => ctrl.RepresentedStateProp == state)
            );
            contentCtrlStream.Transform(
                D,
                (ctrl, localD) => ctrl != null ? ctrl.BindVM(localD, Vmodel) : null
            );
            Bind(contentCtrlStream.SubStream(D, ctrl => ctrl.Title), () => WindowTitle);
            Bind(
                contentCtrlStream.SubStream(D, ctrl => ctrl.TitleSemanticContext)
                    .Func(D, u => TitleSemanticColors.GetColor(u)),
                () => WindowTitleColor
            );
        }

        [UsedImplicitly]
        public void OnLobbyButton()
        {
            Vmodel.Value?.StartGameNode.ExitToLobby();
        }

        [UsedImplicitly]
        public void OnReconnectButton()
        {
            Vmodel.Value?.GameState.ExitGame();
        }
    }
}