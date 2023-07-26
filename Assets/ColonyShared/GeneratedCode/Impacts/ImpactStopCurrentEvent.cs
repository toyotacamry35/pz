using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.MapSystem;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Impacts
{
    public class EffectCastImpactOnEnd : IEffectBinding<EffectCastImactOnEndDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectCastImactOnEndDef def)
        {
            return new ValueTask();
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectCastImactOnEndDef def)
        {
            if(cast.FirstOrLast)
                await SpellImpacts.CastImpact(cast, def.Impact, repo);

        }
    }
    public class ImpactStopCurrentEvent : IImpactBinding<ImpactStopCurrentEventDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactStopCurrentEventDef def)
        {
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if(targetRef != default && targetRef.Type == typeof(IEventInstance))
            {
                using (var eW = await repo.Get(targetRef))
                    await eW.Get<IEventInstanceServer>(targetRef, ReplicationLevel.Server).Stop();
            }
            else
            {
                MapOwner owner;
                using (var scenicW = await repo.Get(targetRef))
                {
                    var scenic = scenicW.Get<IScenicEntityServer>(targetRef, ReplicationLevel.Server);
                    owner = scenic.MapOwner;
                }
                var sceneE = repo.TryGetLockfree<ISceneEntityServer>(owner.OwnerScene, ReplicationLevel.Server);
                var eo = sceneE.EventOwner;
                using (var eW = await repo.Get(eo))
                    await eW.Get<IEventInstanceServer>(eo, ReplicationLevel.Server).Stop();
            }
            }
    }
}
