using System.Threading.Tasks;
using Assets.Src.RubiconAI;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Impacts
{
    public class ImpactSendTargetedEvent : IImpactBinding<ImpactSendTargetedEventDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactSendTargetedEventDef def)
        {
           
            var selfDef = def;
            var initiatorRef = await selfDef.Caster.Target.GetOuterRef(cast, repo);
            Legionary.LegionariesByRef.TryGetValue(initiatorRef, out var initLeg);
            var fromLegRef = await selfDef.Caster.Target.GetOuterRef(cast, repo);
            Legionary.LegionariesByRef.TryGetValue(fromLegRef, out var fromLeg);
            var toLegRef = await selfDef.EventTarget.Target.GetOuterRef(cast, repo);
            Legionary.LegionariesByRef.TryGetValue(toLegRef, out var toLeg);
            var aiEvent = new SpellCastEvent()
            {
                Initiator = initLeg,
                Target = fromLeg,
                StaticData = selfDef.PathToEventStatisDataType.Target
            };
            (toLeg as MobLegionary)?.HandleEvent(aiEvent);
            }
    }
}
