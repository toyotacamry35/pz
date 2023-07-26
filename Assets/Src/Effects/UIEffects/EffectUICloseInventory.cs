using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly]
    public class EffectUICloseInventory : IClientOnlyEffectBinding<EffectUICloseInventoryDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectUICloseInventoryDef def)
        {
            if (!def.OnDetach && cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThreadNoWait(() => InventoryUiOpener.Instance.CloseInventoryWindow());
            return new ValueTask();                
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectUICloseInventoryDef def)
        {
            if (!def.OnDetach && cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThreadNoWait(() => InventoryUiOpener.Instance.CloseInventoryWindow());
            return new ValueTask();
        }
    }
}