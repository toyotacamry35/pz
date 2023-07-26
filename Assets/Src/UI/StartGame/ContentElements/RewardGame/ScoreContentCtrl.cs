using JetBrains.Annotations;
using ReactivePropsNs;
using ResourceSystem.Aspects.Rewards;
using UnityEngine;
using UnityWeld.Binding;
using Transform = UnityEngine.Transform;
using System.Collections.Generic;

namespace Uins
{
    [Binding]
    public class ScoreContentCtrl : BindingController<ScoreContentVM>
    {
        [SerializeField, UsedImplicitly]
        private RewardElementCtrl RewardElementPrefab;

        [SerializeField, UsedImplicitly]
        private Transform RewardsContainer;

        [Binding, UsedImplicitly]
        public int Total { get; set; }

        private DictionaryStream<RewardDef, int> _rewards;

        private void Awake()
        {
            foreach (Transform child in RewardsContainer.transform)
                Destroy(child.gameObject);

            var rewardsPool = new BindingControllersPool<RewardElementVM>(RewardsContainer, RewardElementPrefab);
            _rewards = Vmodel.SubDictionaryStream(D, vm => vm?.Rewards);
            var rewardVMs = CreateRewardElementVms(_rewards);
            rewardsPool.Connect(rewardVMs);
            
            Bind(Vmodel.SubStream(D, vm => vm.RewardsTotal), () => Total);
        }

        private ListStream<RewardElementVM> CreateRewardElementVms(IDictionaryStream<RewardDef, int> rewards)
        {
            var localD = D.CreateInnerD();

            var rewardVMs = new ListStream<RewardElementVM>();
            var created = new Dictionary<RewardDef, RewardElementVM>();
            localD.Add(rewardVMs);
            rewards.AddStream.Subscribe(
                D,
                e =>
                {
                    var rewardDef = e.Key;
                    var count = e.Value;
                    
                    var rewardElementVM = new RewardElementVM(rewardDef, count);
                    localD.Add(rewardElementVM);
                    created.Add(rewardDef, rewardElementVM);
                    rewardVMs.Add(rewardElementVM);
                },
                () =>
                {
                    created.Clear();
                    D.DisposeInnerD(localD);
                }
            );
            rewards.RemoveStream.Action(
                D,
                e =>
                {
                    var rewardDef = e.Key;
                    
                    if (created.TryGetValue(rewardDef, out var value))
                    {
                        created.Remove(rewardDef);
                        localD.Remove(value);
                        rewardVMs.Remove(value);
                        value.Dispose();
                    }
                }
            );
            rewards.ChangeStream.Action(
                D,
                e =>
                {
                    var rewardDef = e.Key;
                    if (created.TryGetValue(rewardDef, out var value))
                    {
                        var newCount = e.NewValue;
                        value.SetCount(newCount);
                    }
                }
            );
            return rewardVMs;
        }
    }
}