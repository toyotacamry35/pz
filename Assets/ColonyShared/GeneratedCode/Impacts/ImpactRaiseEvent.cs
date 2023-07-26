using System;
using Assets.Src.RubiconAI;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using JetBrains.Annotations;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Utils;
using SharedCode.MovementSync;
using System.Collections.Generic;

namespace Assets.Src.Impacts
{
    [UsedImplicitly]
    public class ImpactRaiseEvent : IImpactBinding<ImpactRaiseEventDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRaiseEventDef def)
        {
            var selfDef = def;
            if (selfDef.PathToEventStatisDataType.Target != null)
            {
                Legionary.LegionariesByRef.TryGetValue( await selfDef.From.Target.GetOuterRef(cast, repo), out var initiator);
                var aiEvent = new AIEvent()
                {
                    Initiator = initiator,
                    StaticData = selfDef.PathToEventStatisDataType.Target
                };
                using (var wr = await repo.Get(cast.Caster))
                {
                    var wsEntity = wr.Get<IHasWorldSpacedServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server);
                    EventsRaiser.RaiseEvent(wsEntity.WorldSpaced.OwnWorldSpace.Guid, aiEvent, cast.Caster, selfDef.Radius, repo);

                }
            }
        }
    }

    public static class EventsRaiser
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static void RaiseEvent(Guid wsGuid, AIEvent aiEvent, OuterRef<IEntity> ent, float radius, IEntitiesRepository repo)
        {
            if (aiEvent.Initiator == null)
                return;
            var alreadyKnownEntities = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();
            VisibilityGrid.Get(wsGuid, repo).SampleDataForAllAround(ent, alreadyKnownEntities, radius, true);
            foreach(var foundEnt in alreadyKnownEntities)
                if (Legionary.LegionariesByRef.TryGetValue(foundEnt.Key, out var legionary))
                    (legionary as MobLegionary)?.HandleEvent(aiEvent);
        }
    }
}
