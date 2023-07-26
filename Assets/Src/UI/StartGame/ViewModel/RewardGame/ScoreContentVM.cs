using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using ResourceSystem.Aspects.Rewards;

namespace Uins
{
    public class ScoreContentVM : BindingVmodel
    {
        public DictionaryStream<RewardDef, int> Rewards { get; }
        public IStream<int> RewardsTotal { get; }

        public ScoreContentVM(StartGameWindowVM vm)
        {
            var currentRewards = new ReactiveProperty<RewardsVM>();
            D.Add(currentRewards);

            var touchableAccount = vm.TouchableAccount;
            var characters =
                touchableAccount.ToHashSetStream(
                    D,
                    account => account.Characters,
                    (full, func) => new RewardsVM(func())
                );
            characters.AddStream.Action(
                D,
                account => { currentRewards.Value = account; }
            );
            characters.RemoveStream.Subscribe(
                D,
                account => { currentRewards.Value = null; },
                () => { currentRewards.Value = null; }
            );
            Rewards = currentRewards.SubDictionaryStream(D, rewardsVM => rewardsVM.Rewards);

            var currentRealmRules = vm.CharRealmDataTouchable.ToStream(D, charRealmData => charRealmData.CurrentRealmRulesCached);
            RewardsTotal = currentRealmRules
                .Zip(D, Rewards.CountStream)
                .Func(
                    D,
                    (realmRulesDef, _) => realmRulesDef != null ? AccountEntity.CalcRewardsTotalExp(Rewards, realmRulesDef) : 0
                );
        }
    }

    public class RewardsVM : BindingVmodel
    {
        public IDictionaryStream<RewardDef, int> Rewards { get; }

        public RewardsVM(ITouchable<IAccountCharacterClientFull> touchableAccountCharacter)
        {
            Rewards = touchableAccountCharacter
                .ToDictionaryStream(D, clientFull => clientFull.CurrentSessionRewards);
        }
    }
}