// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountCharacter
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var CharacterName__rnd = "randomCharacterName__rnd" + __random__.Next(1000);
                CharacterName = CharacterName__rnd;
            }

            {
                var MapId__rnd = System.Guid.NewGuid();
                MapId = MapId__rnd;
            }

            {
                _CurrentSessionRewards.Clear();
                var CurrentSessionRewardsitem0__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key0__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key0__ != null)
                    CurrentSessionRewards[__key0__] = CurrentSessionRewardsitem0__rnd;
                var CurrentSessionRewardsitem1__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key1__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key1__ != null)
                    CurrentSessionRewards[__key1__] = CurrentSessionRewardsitem1__rnd;
                var CurrentSessionRewardsitem2__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key2__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key2__ != null)
                    CurrentSessionRewards[__key2__] = CurrentSessionRewardsitem2__rnd;
                var CurrentSessionRewardsitem3__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key3__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key3__ != null)
                    CurrentSessionRewards[__key3__] = CurrentSessionRewardsitem3__rnd;
                var CurrentSessionRewardsitem4__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key4__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key4__ != null)
                    CurrentSessionRewards[__key4__] = CurrentSessionRewardsitem4__rnd;
                var CurrentSessionRewardsitem5__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key5__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key5__ != null)
                    CurrentSessionRewards[__key5__] = CurrentSessionRewardsitem5__rnd;
                var CurrentSessionRewardsitem6__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key6__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key6__ != null)
                    CurrentSessionRewards[__key6__] = CurrentSessionRewardsitem6__rnd;
                var CurrentSessionRewardsitem7__rnd = (int)__random__.Next(100);
                ResourceSystem.Aspects.Rewards.RewardDef __key7__ = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<ResourceSystem.Aspects.Rewards.RewardDef>(__random__);
                if (__key7__ != null)
                    CurrentSessionRewards[__key7__] = CurrentSessionRewardsitem7__rnd;
            }

            {
                var Id__rnd = System.Guid.NewGuid();
                Id = Id__rnd;
            }
        }
    }
}