using Assets.ColonyShared.SharedCode.Aspects.Craft;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Aspects.Templates;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Wizardry;
using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldMachine : IHookOnInit, IHookOnStart, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task OnInit()
        {
            var worldMachineDef = Def as WorldMachineDef;
            if (!worldMachineDef.AssertIfNull(nameof(worldMachineDef)))
            {
                Inventory.Size = worldMachineDef.InventorySize;
                FuelContainer.Size = worldMachineDef.FuelContainerSize;
                OutputContainer.Size = worldMachineDef.OutContainerSize;
            }

            CraftEngine = await EntitiesRepository.Create<ICraftEngine>(Guid.NewGuid(), async craftEngine =>
            {
                craftEngine.OwnerInformation.Owner = new OuterRef<IEntity>(Id, TypeId);
                craftEngine.FuelContainerAddress = EntityPropertyResolver.GetPropertyAddress(FuelContainer);
                craftEngine.ResultContainerAddress = EntityPropertyResolver.GetPropertyAddress(OutputContainer);
                craftEngine.CurrentWorkbenchType = worldMachineDef.WorkbenchType;
            });

            FuelContainer.Items.OnChanged += async (eventArgs) =>
            {
                await UpdateCraftProgressInfo();
            };

            IsActive = true;
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
        }

        public async Task OnDatabaseLoad()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
        }

        public async Task OnStart()
        {
            using (var wizard = await EntitiesRepository.Get<IWizardEntity>(Id))
                foreach (var spell in ((IHasInitialSpells)Def).InitialSpells)
                {
                    await wizard.Get<IWizardEntity>(Id).CastSpell(new SpellCast() { Def = spell.Target, StartAt = SyncTime.Now });
                }
        }
        private async Task OnZeroHealthEvent(Guid arg1, int arg2)
        {
            if (await Health.GetMaxHealthAbsolute() <= 0)
                return;

            await ContainerApi.ContainerOperationSetSize(EntityPropertyResolver.GetPropertyAddress(Inventory), 0);
            await ContainerApi.ContainerOperationSetSize(EntityPropertyResolver.GetPropertyAddress(FuelContainer), 0);
            await ContainerApi.ContainerOperationSetSize(EntityPropertyResolver.GetPropertyAddress(OutputContainer), 0);
            await EntitiesRepository.Destroy(TypeId, Id);
        }

        public async Task<RecipeOperationResult> SetActiveImpl(bool activate)
        {
            Logger.IfInfo()?.Message($"SetActiveImpl({activate}), IsActive = {IsActive}").Write();
            if (IsActive == activate)
                return RecipeOperationResult.Success;

            var result = RecipeOperationResult.Success;
            using (var wrapper = await EntitiesRepository.Get<ICraftEngine>(CraftEngine.Id))
            {
                var craftEngine = wrapper.Get<ICraftEngineServer>(CraftEngine.Id);
                if (activate)
                {
                    var craftResult = await craftEngine.RunCraft();
                    result = craftResult.Is(CraftOperationResult.Success) ? RecipeOperationResult.Success : RecipeOperationResult.ErrorUnknown;
                }
                else
                {
                    await craftEngine.StopCraft();
                }
            }

            return result;
        }

        public async Task<RecipeOperationResult> SetRecipeActivityImpl(CraftRecipeDef recipeDef, bool activate)
        {
            if (activate && !PriorityQueue.Where(v => v.CraftRecipe == recipeDef).Any())
            {
                //var availableRecipes = ((WorldMachineDef)Def)?.AvaliableRecipes;
                //if (availableRecipes == null || Array.IndexOf(availableRecipes, recipeDef) == -1)
               //     return RecipeOperationResult.ErrorNotExist;

                PriorityQueue.Add(new CraftingPriorityQueueItem() { CraftRecipe = recipeDef });

                var activateResult = await SetActiveImpl(false);
                activateResult = await SetActiveImpl(true);
                return RecipeOperationResult.Success;
            }

            if (!activate)
            {
                int recipeIndex = -1;
                ICraftingPriorityQueueItem itemToRemove = null;
                foreach (var item in PriorityQueue)
                {
                    recipeIndex++;
                    if (item.CraftRecipe == recipeDef)
                    {
                        itemToRemove = item;
                        break;
                    }
                }
                if (itemToRemove != null)
                {
                    PriorityQueue.Remove(itemToRemove);
                    using (var wrapper = await EntitiesRepository.Get<ICraftEngine>(CraftEngine.Id))
                    {
                        var craftEngine = wrapper.Get<ICraftEngineServer>(CraftEngine.Id);
                        await craftEngine.RemoveCraft(recipeIndex);
                    }
                    return RecipeOperationResult.Success;
                }
                else
                {
                    return RecipeOperationResult.ErrorNotExist;
                }
            }

            return RecipeOperationResult.ErrorUnknown;

        }

        public async Task<RecipeOperationResult> SetRecipePriorityImpl(CraftRecipeDef recipeDef, int priority)
        {
            var recipe = PriorityQueue.Select((v, i) => new { Recipe = v, Priority = i }).Where(v => v.Recipe.CraftRecipe == recipeDef).FirstOrDefault();
            if (recipe != null)
            {
                PriorityQueue.RemoveAt(recipe.Priority);
                PriorityQueue.Insert(priority, recipe.Recipe);
            }
            else
            {
                return RecipeOperationResult.ErrorNotExist;
            }

            var activateResult = await SetActiveImpl(false);
            activateResult = await SetActiveImpl(true);
            return RecipeOperationResult.Success;
        }

        public async Task UpdateCraftProgressInfoImpl()
        {
            using (var wrapper = await EntitiesRepository.Get<ICraftEngine>(CraftEngine.Id))
            {
                var craftEngine = wrapper.Get<ICraftEngine>(CraftEngine.Id);
                var acceptableFuel = ((WorldMachineDef)Def)?.AcceptableFuel;
                if (acceptableFuel != null)
                {
                    CraftProgressInfo = new CraftProgressInfo()
                    {
                        StartTime = IsActive ? craftEngine.StartFuelTimeUTC0InMilliseconds : SyncTime.Now,
                        Duration =
                            FuelContainer.Items.Values
                            .Concat(craftEngine.IntermediateFuelContainer.Items.Values)
                            .Where(v => v?.Item?.ItemResource != null)
                            .Select(
                                v => v.Stack * acceptableFuel.Where(t => t.Fuel == v.Item.ItemResource).FirstOrDefault().BurnTime
                            ).Sum() - craftEngine.FuelTimeAlreadyInUse * 0.001f
                    };

                   // Logger.IfInfo()?.Message($"CraftProgressInfo: StartTime = {CraftProgressInfo.StartTime}, Duration = {CraftProgressInfo.Duration}, StartFuelTimeUTC0InMilliseconds = {craftEngine.StartFuelTimeUTC0InMilliseconds}, IsActive = {IsActive}").Write();
                }
            }
        }

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }

        public Task<bool> CraftProgressInfoSetImpl(SharedCode.DeltaObjects.ICraftProgressInfo craftProgressInfo)
        {
            CraftProgressInfo = craftProgressInfo;
            return Task.FromResult(true);
        }
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public ItemSpecificStats SpecificStats => ((WorldMachineDef)Def).DefaultStats;
    }
}