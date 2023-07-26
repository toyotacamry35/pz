using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Entities.Engine;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Aspects.Templates;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldPersonalMachine : IHookOnInit, IHookOnStart, IHookOnDatabaseLoad
    {
        public ItemSpecificStats SpecificStats => ((WorldPersonalMachineDef)Def).DefaultStats;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            OuterRef result = default;
            using (var wrapper = await EntitiesRepository.Get(oref.TypeId, oref.Guid))
            {
                if (wrapper.TryGet<IHasWorldPersonalMachineEngineServer>(oref.TypeId, oref.Guid, ReplicationLevel.Server, out var wpmEngine))
                {
                    result = await wpmEngine.worldPersonalMachineEngine.GetOrAddMachine((WorldPersonalMachineDef)Def, new OuterRef(ParentEntityId, ParentTypeId));
                    return result;
                }
            }
            return result;
        }

        public async Task OnInit()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
            IsActive = true;
        }

        public async Task OnStart()
        {
            using (var wizard = await EntitiesRepository.Get<IWizardEntity>(Id))
                foreach (var spell in ((IHasInitialSpells)Def).InitialSpells)
                {
                    await wizard.Get<IWizardEntity>(Id).CastSpell(new SpellCast() { Def = spell.Target, StartAt = SyncTime.Now });
                }
        }

        public async Task OnDatabaseLoad()
        {
            Wizard = await EntitiesRepository.Create<IWizardEntity>(Id, async w => { w.Owner = new OuterRef<IEntity>(Id, TypeId); w.IsInterestingEnoughToLog = false; });
            IsActive = true;
        }
        public async Task<RecipeOperationResult> SetActiveImpl(bool activate)
        {
            return RecipeOperationResult.Success;
        }
    }
}
