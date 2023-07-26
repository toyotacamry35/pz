using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Arithmetic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class ComputableStateMachine : IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("LootableEntity");

        public ValueTask<ComputableStateMachineDef> GetStateMachineDefImpl()
        {
            var entityObj = (IEntityObject)parentEntity;
            if (entityObj == null)
            {
                Logger.IfError()?.Message($"Entity {parentEntity} (guid: {parentEntity.Id}) unexpected is not {nameof(IEntityObject)}.").Write();
                return new ValueTask<ComputableStateMachineDef>(null as ComputableStateMachineDef);
            }
            return new ValueTask<ComputableStateMachineDef>(((IComputableStateMachineOwnerDef)entityObj.Def)?.ComputableStateMachine.Target);
        }

        public async Task<ComputableStatesDef> GetCurrentStatesImpl()
        {
            if (!IsPristineInternal)
                return FixedStates;

            var stateMachine = await GetStateMachineDef();
            if (stateMachine == null)
            {
                Logger.IfDebug()?.Message($"{SharedHelpers.NowStamp}  #DBG: *********************** `{nameof(stateMachine)}` == null return empty List. ({parentEntity?.TypeId}, {parentEntity?.Id}).").Write();
                return null;
            }

            return await stateMachine.CalcCurrStates(parentEntity.TypeId, parentEntity.Id, EntitiesRepository);
        }

        public ValueTask<bool> GetIsPristineImpl()
        {
            return new ValueTask<bool>(IsPristineInternal);
        }

        public async Task MarkNotPristineImpl()
        {
            if (!IsPristineInternal)
                return;

            FixedStates = await GetCurrentStates();
            IsPristineInternal = false;
        }

        public Task OnInit()
        {
            IsPristineInternal = true;
            return Task.CompletedTask;
        }
    }
}
