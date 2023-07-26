using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Arithmetic;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class ImpactDamageObject : IImpactBinding<ImpactDamageObjectDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactDamageObjectDef def)
        {
            var target = cast.Caster;
            if (def.Target.Target != null)
                target = await def.Target.Target.GetOuterRef(cast, repo);
            if (!target.IsValid)
                target = cast.Caster;

            using (var wrapper = await repo.Get(target.TypeId, target.Guid))
            {
                var damage = await def.Damage.Target.CalcAsync(new CalcerContext(wrapper, target, repo, cast));
                if (damage > 0)
                {
                    var mortalEntity = wrapper.Get<IHasHealthServer>(target.TypeId, target.Guid, ReplicationLevel.Server);
                    if (mortalEntity != null)
                    {
                        var damageInfo = new Damage(
                            aggressor: cast.Caster,
                            battleDamage: damage,
                            damageType: GlobalConstsHolder.GlobalConstsDef.DefaultDamageType,
                            isMiningDamage: false, 
                            miningLootMultiplier: 0f);
                        await mortalEntity.Health.ReceiveDamage(damageInfo, cast.Caster);
                    }
                }
            }
            }
    }
}