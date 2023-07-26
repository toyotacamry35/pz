using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;

namespace Src.Impacts
{
    public class ImpactSetLock : IImpactBinding<ImpactSetLockDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSetLockDef def)
        {
            if (def.Target.Target == null)
                return;

            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            using (var wrapper = await repo.Get(targetRef))
            {
                var hasOwnerEntity = wrapper.Get<IHasOwnerServer>(targetRef, ReplicationLevel.Server);
                if (hasOwnerEntity != null)
                {
                    await hasOwnerEntity.OwnerInformation.SetLockPredicate(def.Predicate);
                }
            }
        }
    }
}