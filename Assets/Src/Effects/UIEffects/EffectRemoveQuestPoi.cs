using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    public class EffectRemoveQuestPoi : IEffectBinding<EffectRemoveQuestPoiDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("UI");

        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectRemoveQuestPoiDef def)
        {
            if (!cast.IsSlave || !cast.SlaveMark.OnClient || def.AssertIfNull(nameof(def)))
                return new ValueTask();

            Logger.IfDebug()?.Message($"<{GetType()}> def={def}, targetGuid={cast.Caster.Guid}").Write();
            if (cast.Caster.Guid != (GameState.Instance?.CharacterRuntimeData?.CharacterId ?? Guid.Empty))
                return new ValueTask();

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (RemovedQuestPoiList.Instance.AssertIfNull($"{nameof(RemovedQuestPoiList)}.{nameof(RemovedQuestPoiList.Instance)}"))
                    return;

                if (def.PointOfInterest.Target != null)
                    RemovedQuestPoiList.Instance.AddPoiDef(def.PointOfInterest.Target);

                if (def.PointsOfInterest != null)
                    RemovedQuestPoiList.Instance.AddPoiDefs(def.PointsOfInterest.Where(poiRef => poiRef.IsValid).Select(poiRef => poiRef.Target).ToArray());
            });

            return new ValueTask();
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectRemoveQuestPoiDef def)
        {
            return new ValueTask();
        }
    }
}