using System.Threading.Tasks;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RewardGameContentCtrl : BaseStartGameContentCtrl
    {
        // private const int RewardsConsumeAnimationDelay = 5;

        [SerializeField, UsedImplicitly]
        private ScoreContentCtrl ScoreContentCtrl;

        [SerializeField, UsedImplicitly]
        private LevelContentCtrl LevelContentCtrl;
        
        [Binding, UsedImplicitly]
        public bool ConsumeButtonEnabled { get; set; }

        protected override void Start()
        {
            base.Start();

            var scoreContentVM = Vmodel.Transform(D, vm => vm != null ? new ScoreContentVM(vm) : null);
            ScoreContentCtrl.BindVM(D, scoreContentVM);

            var rewardsTotal = scoreContentVM.SubStream(D, contentVM => contentVM.RewardsTotal);
            var levelContentVM = Vmodel.Transform(D, vm => vm != null ? new LevelContentVM(vm, rewardsTotal) : null);
            LevelContentCtrl.BindVM(D, levelContentVM);
            
            Bind(LevelContentCtrl.ConsumeButtonEnabled, () => ConsumeButtonEnabled);
        }

        [UsedImplicitly]
        public void OnConsumeButton()
        {
            if (Vmodel.HasValue)
            {
                var vm = Vmodel.Value;
                var startGameNode = vm.StartGameNode;
                RunSingleCommandAsync(ConsumeRewards(startGameNode));
            }
        }

        private static async Task ConsumeRewards(StartGameGuiNode startGameNode)
        {
            await startGameNode.ConsumeCurrentRealm();
            // await Task.Delay(TimeSpan.FromSeconds(RewardsConsumeAnimationDelay));
            startGameNode.SetState(StartGameGuiNode.State.Account);
        }
    }
}