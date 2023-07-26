using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly]
    public class EffectOpenUITeleportCountdown : IClientOnlyEffectBinding<EffectOpenUITeleportCountdownDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUITeleportCountdownDef def)
        {
            if (cast.OnClientWithAuthority())
            {
                var causer = cast.WordGlobalCastId(def);
                UnityQueueHelper.RunInUnityThreadNoWait(() => TeleportWindow.Instance.Open(causer, SyncTime.ToSeconds(cast.WordTimeRange.Duration)));
            }
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUITeleportCountdownDef def)
        {
            var causer = cast.WordGlobalCastId(def);
            UnityQueueHelper.RunInUnityThreadNoWait(() => TeleportWindow.Instance.Close(causer));
            return new ValueTask();
        }
    }
}