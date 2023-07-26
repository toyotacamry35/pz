using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.Aspects.Science;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.Src.Predicates
{
    public class PredicateKnowledge : IPredicateBinding<PredicateKnowledgeDef>
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateKnowledgeDef def)
        {
            var predKnowledgeDef = (PredicateKnowledgeDef)def;
            var knowledgeDef = predKnowledgeDef.Knowledge.Target;
            using (var wizardOwner = await repo.Get(cast.Caster.TypeId, cast.Caster.Guid))
            {
                var knowledgeOwner = wizardOwner.Get<IWorldCharacterClientFull>(cast.Caster.Guid);
                if (knowledgeOwner == null)
                {
                    Logger.IfError()?.Message("knowledgeOwner == null {0}", cast.ToString()).Write();
                    return false;
                }
                if (knowledgeOwner.KnowledgeEngine == null)
                {
                    Logger.IfError()?.Message("knowledgeOwner.KnowledgeEngine == null {0}", cast.ToString()).Write();
                    return false;
                }
                using (var knowledgeEngine = await repo.Get<IKnowledgeEngine>(knowledgeOwner.KnowledgeEngine.Id))
                {
                    var knowledgeEngineCF = knowledgeEngine.Get<IKnowledgeEngineClientFull>(knowledgeOwner.KnowledgeEngine.Id);

                    return KnowledgeLogic.GetKnownKnowledges(knowledgeEngineCF.KnownTechnologies, knowledgeEngineCF.KnownKnowledges, knowledgeOwner.MutationMechanics.Stage, predKnowledgeDef.IncludeBlocked)?
                        .Any(k => k == knowledgeDef) ?? false;
                }
            }
        }
    }
}
