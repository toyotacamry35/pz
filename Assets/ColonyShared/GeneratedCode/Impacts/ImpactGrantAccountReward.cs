using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Rewards;
using ResourceSystem.Utils;
using GeneratedCode.Repositories;
using SharedCode.Entities.Cloud;

namespace Assets.Src.Impacts
{
    [UsedImplicitly]
    public class ImpactGrantAccountReward : IImpactBinding<ImpactGrantAccountRewardDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactGrantAccountRewardDef def)
        {
            var target = await def.Target.Target.GetOuterRef(cast, repo);
            if (!target.IsValid)
                return;

            var reward = def.Reward.Target;
            if (reward == null)
                return;

            await GrantReward(reward, target, repo);
        }

        public static async ValueTask GrantReward(RewardDef reward, OuterRef target, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.GetFirstService<ILoginServiceEntityServer>())
            {
                var loginServer = wrapper.GetFirstService<ILoginServiceEntityServer>();
                await loginServer.GrantAccountReward(repo.TryGetLockfree<IWorldCharacterServer>(target.Guid, ReplicationLevel.Server).AccountId, reward);
            }
        }
    }
}
