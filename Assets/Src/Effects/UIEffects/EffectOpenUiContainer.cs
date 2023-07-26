using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratedCode.Repositories;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly]
    public class EffectOpenUiContainer : BaseEffectOpenUi, IEffectBinding<EffectOpenUiContainerDef>
    {
        protected override async Task OnOpenSuccess(EffectData effectData)
        {
            if (!effectData.FinalTargetOuterRef.IsValid)
            {
                LogError(effectData, $"OnOpenSuccess() {effectData.FinalTargetOuterRef} isn't valid");
                await StopSpellAsync(effectData.Cast, effectData.Repo);
                return;
            }

            bool isAuthoredContainer = true; // GetIsAuthoredContainer(targetOuterRef); //TODOM Сделать метод определения, авторизованный ли это контейнер
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                InventoryUiOpener.Instance.OnOpenContainerFromSpell(effectData, isAuthoredContainer, ContainerMode.Inventory);
            });
        }

        protected override Task OnCloseAnyway(EffectData effectData)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                InventoryUiOpener.Instance?.OnCloseContainerFromSpell(effectData.Cast);
            });
            return Task.CompletedTask;
        }

        private bool GetIsAuthoredContainer(OuterRef<IEntityObject> targetOuterRef)
        {
            return EntitiesRepository.GetMasterTypeIdByReplicationLevelType(targetOuterRef.TypeId) ==
                   EntitiesRepository.GetIdByType(typeof(ICharacterChest));
        }

        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUiContainerDef def)
        {
            return base.Attach(cast, repo, def);
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUiContainerDef def)
        {
            return base.Detach(cast, repo, def);
        }
    }
}