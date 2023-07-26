using System.Linq;
using Assets.Src.Aspects;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Utils;
using ResourceSystem.Aspects;

namespace Assets.Src.Impacts
{
    [UsedImplicitly]
    public class ImpactAttackObject : IImpactBinding<ImpactAttackObjectDef>// SpellImpactBase<ImpactAttackObjectDef>, ISpellImpact
    {
        public async ValueTask Apply([NotNull] SpellWordCastData cast, [NotNull] IEntitiesRepository repository, [NotNull] ImpactAttackObjectDef def)
        {
            var attackerRef = await def.Attacker.Target.GetOuterRef(cast, repository);
            var victimRef   = await def.Victim.Target.GetOuterRef(cast, repository);
            await DamagePipelineHelper.ExecuteStrike(
                aggressorRef: attackerRef.To(), 
                victimInfo: new AttackTargetInfo { Target = victimRef.To() }, 
                repository: repository, 
                attackTimestamp: SyncTime.Now,
                attackDesc: def.Attack.Target,
                attackModifiers: def.Attack.Target.Modifiers.Combine(cast.Modifiers)
                );
            }
    }
}