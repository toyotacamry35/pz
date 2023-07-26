using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode.Combat;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Aspects.ManualDefsForSpells;
using ColonyShared.SharedCode.Modifiers;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Aspects;
using SharedCode.EntitySystem;

namespace Assets.Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectAttack : IEffectBinding<EffectAttackDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            var selfDef = def;
            if (cast.OnServerMaster())
                return AttachOnMaster(cast, repo, selfDef);
            if (cast.OnClientWithAuthority())
                return AttachOnClientWithAuthority(cast, repo, selfDef);
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            var selfDef = def;
            if (cast.OnServerMaster())
                return DetachOnMaster(cast, repo, selfDef);
            if (cast.OnClientWithAuthority())
                return DetachOnClientWithAuthority(cast, repo, selfDef);
            return new ValueTask();
        }

        public async ValueTask AttachOnMaster(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            using (var cont = await repo.Get(cast.Caster))
            {
                var hasAttackEngine = cont.Get<IHasAttackEngine>(cast.Caster);
                await hasAttackEngine.AttackEngine.StartAttack(cast.WordCastId(def), cast.WordTimeRange.Finish, def.Attack, def.Attack.Target.Modifiers.Combine(cast.Modifiers));
                var spellDuration = cast.CastData.Def.IsInfinite ? 0 : SyncTime.ToSeconds(cast.CastData.Duration);
                hasAttackEngine.AttackEngine.AttackDoer?.StartServerSideAttack(cast.WordCastId(def), def.Attack, def.ColliderMarker, def.TrajectoryMarkers, def.Animation.Target.State, cast.WordTimeRange, cast.CurrentTime, cast.SpellStartTime, spellDuration, targetRef);
            }
        }

        public async ValueTask DetachOnMaster(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            using (var cont = await repo.Get(cast.Caster))
            {
                var attacker = cont.Get<IHasAttackEngine>(cast.Caster);
                await attacker.AttackEngine.FinishAttack(cast.WordCastId(def), cast.CurrentTime);
                attacker.AttackEngine.AttackDoer?.FinishAttack(cast.WordCastId(def), cast.WordTimeRange, cast.CurrentTime);
            }
        }

        public async ValueTask AttachOnClientWithAuthority(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            using (var cont = await repo.Get(cast.Caster))
            {
                var attacker = cont.Get<IHasAttackEngineClientFull>(cast.Caster, ReplicationLevel.ClientFull);
                var playId = def.AnimationSpellId != null ?
                        AnimationPlayId.CreatePlayId(def.AnimationSpellId.Target.GetSpellId(cast), 0 /*FIMXE*/, def.Animation) :
                        AnimationPlayId.CreatePlayId(cast, def.Animation);
                var spellDuration = cast.CastData.Def.IsInfinite ? 0 : SyncTime.ToSeconds(cast.CastData.Duration);
                attacker.AttackEngine.AttackDoer?.StartClientSideAttack(cast.WordCastId(def), def.Attack, def.ColliderMarker, playId, cast.WordTimeRange, cast.CurrentTime, cast.SpellStartTime, spellDuration, targetRef);
            }
        }

        public async ValueTask DetachOnClientWithAuthority(SpellWordCastData cast, IEntitiesRepository repo, EffectAttackDef def)
        {
            using (var cont = await repo.Get(cast.Caster))
            {
                var attacker = cont.Get<IHasAttackEngineClientFull>(cast.Caster, ReplicationLevel.ClientFull);
                attacker.AttackEngine.AttackDoer?.FinishAttack(cast.WordCastId(def), cast.WordTimeRange, cast.CurrentTime);
            }
        }
    }
}
