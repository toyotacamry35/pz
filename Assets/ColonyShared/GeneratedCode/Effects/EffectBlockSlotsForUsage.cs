using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Aspects.ManualDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Repositories;

namespace ColonyShared.GeneratedCode.Effects
{
    public class EffectBlockSlotsForUsage : IEffectBinding<EffectBlockSlotsForUsageDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectBlockSlotsForUsageDef def)
        {
            if (cast.OnServerMaster())
                return Do(cast, repo, def, true);
            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectBlockSlotsForUsageDef def)
        {
            if (cast.OnServerMaster())
                return Do(cast, repo, def, false);
            return new ValueTask();
        }

        private async ValueTask Do(SpellCastData cast, IEntitiesRepository repo, EffectBlockSlotsForUsageDef def, bool add)
        {
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            using (var wrapper = await repo.Get(targetRef.TypeId, targetRef.Guid))
            {
                if (wrapper.TryGet<IHasDollServer>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Server, out var entity))
                {
                    var slots = (def.Slots != null ? def.Slots : Enumerable.Empty<SlotDef>())
                        .Concat(def.SlotsList.Target?.Slots != null ? def.SlotsList.Target.Slots.Select(x => x.Target) : Enumerable.Empty<SlotDef>())
                        .ToArray();
                    if (slots.Length > 0)
                        if (add)
                            await entity.Doll.AddBlockedForUsageSlots(slots);
                        else
                            await entity.Doll.RemoveBlockedForUsageSlots(slots);
                }
                else if (add)
                    if (Logger.IsErrorEnabled) Logger.IfError()?.Message($"Can't get entity {targetRef} as {nameof(IHasDollServer)} in repo {repo}").Write();
            }
        }
    }
}