using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectStaminaFlash : IClientOnlyEffectBinding<EffectStaminaFlashDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectStaminaFlashDef def)
        {
            if (SurvivalGuiNode.Instance.PlayerGuid == cast.Caster.Guid)
                UnityQueueHelper.RunInUnityThreadNoWait(HudGuiNode.StaminaFlash);
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectStaminaFlashDef def)
        {
            return new ValueTask();
        }
    }
}