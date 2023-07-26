using System;
using System.Threading.Tasks;
using Assets.Src.RubiconAI;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Impacts
{
    [UsedImplicitly]
    public class ImpactRaiseTargetedEvent : IImpactBinding<ImpactRaiseTargetedEventDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRaiseTargetedEventDef def)
        {
            var selfDef = def;
            if (selfDef.PathToEventStatisDataType.Target != null)
            {
                if (!Legionary.LegionariesByRef.TryGetValue(await selfDef.Target.Target.GetOuterRef(cast,repo), out var target))
                    return;
                Legionary.LegionariesByRef.TryGetValue(await selfDef.From.Target.GetOuterRef(cast, repo), out var initiator);
                var aiEvent = new SpellCastEvent()
                {
                    Initiator = initiator,
                    Target = target,
                    StaticData = selfDef.PathToEventStatisDataType.Target
                };
                using (var wr = await repo.Get(cast.Caster))
                {
                    var wsEntity = wr.Get<IHasWorldSpacedServer>(cast.Caster.TypeId, cast.Caster.Guid, ReplicationLevel.Server);
                    if (wsEntity != null)
                        EventsRaiser.RaiseEvent(wsEntity.WorldSpaced.OwnWorldSpace.Guid, aiEvent, cast.Caster, selfDef.Radius, repo);

                }

            }
            }
    }
}
