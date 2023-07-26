using System.Threading.Tasks;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using ResourceSystem.GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using Uins;

namespace Src.Effects
{
    public class EffectShowKnockdownInterface : IClientOnlyEffectBinding<EffectShowKnockdownInterfaceDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowKnockdownInterfaceDef def)
        {
            if (cast.OnClientWithAuthority())
            {
                var casterRf = cast.Caster;
                var casterGo = cast.GetCaster();
                if (casterGo)
                    UnityQueueHelper.RunInUnityThreadNoWait(() => HudGuiNode.Instance?.KnockdownInterface.Activate(casterRf, repo, def.TextLs));
            }
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowKnockdownInterfaceDef def)
        {
            if (cast.OnClientWithAuthority())
            {
                var casterGo = cast.GetCaster();
                if (casterGo)
                    UnityQueueHelper.RunInUnityThreadNoWait(() => HudGuiNode.Instance?.KnockdownInterface.Deactivate());
            }
            return new ValueTask();
        }
    }
}