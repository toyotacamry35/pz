using System.Linq;
using ColonyDI;
using Core.Cheats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.Rewards;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins.GuiWindows;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [RequireComponent(typeof(StartGameWindowCtrl))]
    [Binding]
    public class StartGameGuiWindow : BaseGuiWindow
    {
        private static StartGameGuiWindow _instance;
        
        private StartGameWindowVM _gameWindowVM;
        private EntityVM<IAccountEntityClientFull> _accountEntityVM;

        private StartGameWindowCtrl _startGameWindowCtrl;

        public StartGameGuiWindow()
        {
            _instance = this;
        }

        [Dependency]
        private LobbyGuiNode LobbyGuiNode { get; set; }

        [Dependency]
        private StartGameGuiNode StartGameNode { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _startGameWindowCtrl = GetComponent<StartGameWindowCtrl>();
            _startGameWindowCtrl.AssertIfNull(nameof(_startGameWindowCtrl));
        }

        public override void OnOpen(object arg)
        {
            BlurredBackground.Instance.SwitchCameraFullBlur(this, true);
            _accountEntityVM = new EntityVM<IAccountEntityClientFull>(GameState.ClientClusterRepository, GameState.AccountIdStream);
            _gameWindowVM = new StartGameWindowVM(StartGameNode, GameState, LobbyGuiNode, _accountEntityVM.Touchable);
            _startGameWindowCtrl.SetVmodel(_gameWindowVM);
            base.OnOpen(arg);
        }

        public override void OnClose()
        {
            _startGameWindowCtrl.SetVmodel(null);

            _gameWindowVM?.Dispose();
            _gameWindowVM = null;
            
            _accountEntityVM?.Dispose();
            _accountEntityVM = null;
            
            base.OnClose();
            BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
        }

        [Cheat, UsedImplicitly]
        public static void OpenStartGame()
        {
            _instance.WindowsManager.Open(_instance);
        }

        [Cheat, UsedImplicitly]
        public static void CloseStartGame()
        {
            _instance.WindowsManager.Close(_instance);
        }

        [Cheat, UsedImplicitly]
        public static void ChangeCurrentRealmActivity(bool active)
        {
            AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        var accountId = GameState.Instance.AccountId;
                        var repo = GameState.Instance.ClientClusterNode;

                        using (var wrapper = await repo.Get<IAccountEntityClientFull>(accountId))
                        {
                            var entity = wrapper.Get<IAccountEntityClientFull>(accountId);
                            await entity.CharRealmData.ChangeCurrentRealmActivity(accountId, active);
                        }
                    });
        }

        [Cheat, UsedImplicitly]
        public static void DestroyCurrentRealm()
        {
            AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        var accountId = GameState.Instance.AccountId;
                        var repo = GameState.Instance.ClientClusterNode;

                        using (var wrapper = await repo.Get<IAccountEntityClientFull>(accountId))
                        {
                            var entity = wrapper.Get<IAccountEntityClientFull>(accountId);
                            await entity.CharRealmData.DestroyCurrentRealm(accountId);
                        }
                    });
        }

        [Cheat, UsedImplicitly]
        public static void AddRewards()
        {
            // {new RewardDef {Experience = 66, Title = new LocalizedString("Keys unlocked")}, 3},
            // {new RewardDef {Experience = 20, Title = new LocalizedString("Hunting Quests Completed")}, 5},
            // {new RewardDef {ExperienceMultiplier = 3, Title = new LocalizedString("Game Difficulty")}, 0}

            AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        var accountId = GameState.Instance.AccountId;
                        var repo = GameState.Instance.ClientClusterNode;

                        using (var wrapper = await repo.Get<IAccountEntityClientFull>(accountId))
                        {
                            var entity = wrapper.Get<IAccountEntityClientFull>(accountId);
                            var rewardDef = GameResourcesHolder.Instance
                                .LoadResource<RewardDef>("/Sessions/Rewards/NormalGameDifficultyReward");
                            await entity.Characters.First().AddReward(rewardDef);

                            var reward2Def = GameResourcesHolder.Instance
                                .LoadResource<RewardDef>("/Sessions/Rewards/KeysUnlockedReward");
                            await entity.Characters.First().AddReward(reward2Def);
                            await entity.Characters.First().AddReward(reward2Def);
                        }
                    });
        }
    }
}