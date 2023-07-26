using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Entities.Engine;
using NLog;
using SharedCode.Entities;

namespace Assets.Src.Impacts
{
    public class ImpactAddTechnology : IImpactBinding<ImpactAddTechnologyDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddTechnologyDef def)
        {
            if (def.Technology.Target.AssertIfNull(nameof(def.Technology)))
                return;

            var casterOref = cast.Caster;
            using (var wrapper = await repo.Get(casterOref.TypeId,casterOref.Guid))
            {
                if (wrapper.AssertIfNull(nameof(wrapper)))
                    return;
                var character = wrapper.Get<IWorldCharacter>(casterOref.Guid);
                if (character.AssertIfNull(nameof(character)))
                    return;
                using (var knowledgeEnginewrapper = await repo.Get<IKnowledgeEngine>(character.KnowledgeEngine.Id))
                {
                    var knowledgeEngine = knowledgeEnginewrapper.Get<IKnowledgeEngine>(character.KnowledgeEngine.Id);
                    if (knowledgeEngine.AssertIfNull(nameof(knowledgeEngine)))
                        return;
                    await knowledgeEngine.AddTechnology(def.Technology.Target);
                }
            }

        }
    }
}
