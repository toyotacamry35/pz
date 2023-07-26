using ColonyDI;
using Uins.GuiWindows;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// Задача: Cre. Ctrl-r в Awake;  Cre. VM в OnOpen и засетить её в Ctrl-r, и обратно на OnClose. Ну и держать ссылки на них (Ctrl-r и VM)
    /// </summary>
    [RequireComponent(typeof(AccountGuiWindowCtrl))]
    [Binding]
    public class AccountGuiWindow : BaseGuiWindow
    {
        [Dependency]
        private StartGameGuiNode StartGameNode { get; set; }

        private AccountGuiWindowCtrl _accGuiWindowCtrl;
        private AccountGuiWindowVM _accGuiWindowVM;


        protected override void Awake()
        {
            base.Awake();
            _accGuiWindowCtrl = GetComponent<AccountGuiWindowCtrl>();
        }

        public override void OnOpen(object arg)
        {
            BlurredBackground.Instance.SwitchCameraFullBlur(this, true);
            _accGuiWindowVM = new AccountGuiWindowVM(
                GameState,
                StartGameNode,
                () => StartGameNode.SetState(StartGameGuiNode.State.StartGame));
            _accGuiWindowCtrl.SetVmodel(_accGuiWindowVM);
            base.OnOpen(arg);
        }

        public override void OnClose()
        {
            _accGuiWindowCtrl.SetVmodel(null);

            _accGuiWindowVM?.Dispose();
            _accGuiWindowVM = null;

            base.OnClose();
            BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
        }
    }
}
