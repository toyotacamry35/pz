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
    public class EffectOpenUIBank : BaseEffectOpenUi, IEffectBinding<EffectOpenUIBankDef>
    {
        protected override Task OnOpenSuccess(EffectData effectData)
        {
            //Предполагаем пока, что effectData.FinalTargetOuterRef невалиден
            bool isAuthoredContainer = true; //GetIsAuthoredContainer(targetOuterRef); //TODOM Сделать метод определения, авторизованный ли это контейнер
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                InventoryUiOpener.Instance.OnOpenContainerFromSpell(effectData, isAuthoredContainer, ContainerMode.Bank);
            });
            return Task.CompletedTask;
        }

        protected override Task OnCloseAnyway(EffectData effectData)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() => { InventoryUiOpener.Instance?.OnCloseContainerFromSpell(effectData.Cast); });
            return Task.CompletedTask;
        }

        private bool GetIsAuthoredContainer(OuterRef<IEntityObject> targetOuterRef)
        {
            return EntitiesRepository.GetMasterTypeIdByReplicationLevelType(targetOuterRef.TypeId) ==
                   EntitiesRepository.GetIdByType(typeof(ICharacterChest));
        }

        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUIBankDef def)
        {
            return base.Attach(cast, repo, def);
        }

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUIBankDef def)
        {
            return base.Detach(cast, repo, def);
        }
    }
}