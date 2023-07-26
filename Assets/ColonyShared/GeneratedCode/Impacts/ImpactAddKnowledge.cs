using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Repositories;

namespace Assets.Src.Impacts
{
    public class ImpactAddKnowledge : IImpactBinding<ImpactAddKnowledgeDef>
    {
        private NLog.Logger Logger => GlobalLoggers.SpellSystemLogger;

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactAddKnowledgeDef def)
        {
            if (EntitiesRepository.GetMasterTypeIdByReplicationLevelType(cast.Caster.TypeId) !=
                ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)))
            {
                Logger.IfDebug()?.Message($"Caster isn't {nameof(IWorldCharacter)} (Caster.Guid: {cast.Caster.Guid}, type: {cast.Caster.TypeId}). Unexpected in this impact").Write();
                return;
            }

            var impactDef = ((ImpactAddKnowledgeDef)def);
            using (var wrapper = await repo.Get<IWorldCharacterServer>(cast.Caster.Guid))
            {
                var entity = wrapper.Get<IWorldCharacterServer>(cast.Caster.Guid);
                if (entity == null)
                {
                    Logger.IfError()?.Message($"Can't get {nameof(IWorldCharacterServer)} by Caster.Guid: {cast.Caster.Guid} (type: {cast.Caster.TypeId}).").Write();
                    return;
                }
                using (var wrapper2 = await repo.Get<IKnowledgeEngine>(entity.KnowledgeEngine.Id))
                {
                    var entity2 = wrapper2.Get<IKnowledgeEngine>(entity.KnowledgeEngine.Id);
                    var knowledgeDef = impactDef.Knowledge.Target;

                    await entity2.AddKnowledge(knowledgeDef);
                }
            }

            }
    }
}