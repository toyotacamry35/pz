using System.Threading.Tasks;
using Assets.Src.RubiconAI;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Impacts
{
    public class ImpactSendEvent : IImpactBinding<ImpactSendEventDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSendEventDef def)
        {
            var selfDef = def;
            OuterRef<IEntity> fromLegRef = default;
            if (selfDef.From.Target != null)
                fromLegRef = await selfDef.From.Target.GetOuterRef(cast, repo);
            if (!fromLegRef.IsValid)
                fromLegRef = await selfDef.Caster.Target.GetOuterRef(cast, repo);

            Legionary.LegionariesByRef.TryGetValue(fromLegRef, out var fromLeg);
            var toLegRef = await selfDef.EventTarget.Target.GetOuterRef(cast,repo);
            Legionary.LegionariesByRef.TryGetValue(toLegRef, out var toLeg);
            var aiEvent = new AIEvent()
            {
                Initiator = fromLeg,
                StaticData = selfDef.PathToEventStatisDataType.Target
            };
            (toLeg as MobLegionary)?.HandleEvent(aiEvent);
            }

    }
}
