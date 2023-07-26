using Assets.Src.ContainerApis;
using Assets.Src.Effects.UIEffects;
using Assets.Src.SpawnSystem;
using SharedCode.Aspects.Item.Templates;
using Uins.Inventory;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.Serializers;

namespace Uins
{
    public class CraftWindowLogic : BindingVmodel
    {
        private int _ourPlayerQueueSize;

        private CraftQueueSlots _selfCraftQueueSlots;
        private CraftQueueSlots _machineCraftQueueSlots;
        private CraftRecipeContextView _craftRecipeContextView;
        private CraftSideViewModel _craftSideViewModel;


        public CraftWindowLogic(CraftQueueSlots selfCraftQueueSlots, CraftQueueSlots machineCraftQueueSlots, CraftRecipeContextView craftRecipeContextView,
            int ourPlayerQueueSize, CraftSideViewModel craftSideViewModel, MachineCraftSideViewModel machineCraftSideViewModel,
            IStream<CraftSourceVmodel> craftSourceVmodelStream)
        {
            _craftRecipeContextView = craftRecipeContextView;
            _selfCraftQueueSlots = selfCraftQueueSlots;
            _machineCraftQueueSlots = machineCraftQueueSlots;
            _ourPlayerQueueSize = ourPlayerQueueSize;
            _craftSideViewModel = craftSideViewModel;
            _craftRecipeContextView.AssertIfNull(nameof(_craftRecipeContextView));
            _selfCraftQueueSlots.AssertIfNull(nameof(_selfCraftQueueSlots));
            _craftSideViewModel.AssertIfNull(nameof(_craftSideViewModel));
            machineCraftSideViewModel.AssertIfNull(nameof(machineCraftSideViewModel));
            craftSourceVmodelStream.AssertIfNull(nameof(craftSideViewModel));

            craftSourceVmodelStream.PrevAndCurrent(D)
                .Action(D, (prev, curr) =>
                {
                    if (prev != null)
                    {
                        machineCraftSideViewModel.CraftEngineOuterRef = OuterRef.Invalid;
                        UnsubscribeFromMachineQueue(prev);
                    }

                    if (curr != null)
                    {
                        machineCraftSideViewModel.CraftEngineOuterRef = curr.TargetOuterRef;
                        SubscribeToMachineQueue(curr);
                    }
                });
        }

        public void Init(DisposableComposite d, IPawnSource pawnSource)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(d, OnOurPawnChanged);
        }


        //=== Private =========================================================

        private void SubscribeToMachineQueue(CraftSourceVmodel craftSourceVmodel)
        {
            var worldPersonalMachineDef = craftSourceVmodel.WorldPersonalMachineDef;
            WorkbenchTypeDef workbenchType = null;
            if (worldPersonalMachineDef != null)
            {
                workbenchType = worldPersonalMachineDef.WorkbenchType.Target;
            }

            _craftRecipeContextView.SetAvailableWorkbenchType(workbenchType);
            _machineCraftQueueSlots.Subscribe(craftSourceVmodel.TargetOuterRef, true, worldPersonalMachineDef?.MaxQueueSize ?? 0);
        }

        private void UnsubscribeFromMachineQueue(CraftSourceVmodel craftSourceVmodel)
        {
            _craftRecipeContextView.SetAvailableWorkbenchType(null);
            _machineCraftQueueSlots.Unsubscribe();
            AsyncUtils.RunAsyncTask(() => BaseEffectOpenUi.StopSpellAsync(craftSourceVmodel.Cast, craftSourceVmodel.Repository));
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _craftSideViewModel.CraftEngineOuterRef = OuterRef.Invalid;
                _selfCraftQueueSlots.Unsubscribe();
                _craftRecipeContextView.SetCharacterContainersAddresses();
            }

            if (newEgo != null)
            {
                AsyncUtils.RunAsyncTask(() =>
                    CraftEngineCommands.GetCharacterContainersPropertyAddressesAsync(
                        newEgo.OuterRef,
                        (invAddr, dollAddr, craftEngineOuterRef) =>
                        {
                            _selfCraftQueueSlots.Subscribe(craftEngineOuterRef, false, _ourPlayerQueueSize);
                            _craftRecipeContextView.SetCharacterContainersAddresses(invAddr, dollAddr);
                            _craftSideViewModel.CraftEngineOuterRef = craftEngineOuterRef;
                        }));
            }
        }
    }
}