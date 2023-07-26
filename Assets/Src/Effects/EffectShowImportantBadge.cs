using System.Threading.Tasks;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects
{
    public class EffectShowImportantBadge : IClientOnlyEffectBinding<EffectShowImportantBadgeDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowImportantBadgeDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var casterGo = cast.GetCaster();
                if (casterGo.AssertIfNull(nameof(casterGo)))
                    return;

                var hasHealthBadgePoint = casterGo.GetComponentInChildren<HasHealthBadgePoint>();
                if (hasHealthBadgePoint.AssertIfNull(nameof(hasHealthBadgePoint), casterGo))
                    return;

                hasHealthBadgePoint.SetIsImportantBadgeShown(true);
            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowImportantBadgeDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var casterGo = cast.GetCaster();
                if (casterGo.AssertIfNull(nameof(casterGo)))
                    return;

                var hasHealthBadgePoint = casterGo.GetComponentInChildren<HasHealthBadgePoint>();
                if (hasHealthBadgePoint.AssertIfNull(nameof(hasHealthBadgePoint), casterGo))
                    return;

                hasHealthBadgePoint.SetIsImportantBadgeShown(false);
            });

            return new ValueTask();
        }
    }
}