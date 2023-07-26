using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Utils;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class Lootable : IHookOnInit
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("LootableEntity");

        private readonly bool _debugEnabled = false;

        public async ValueTask<LootTableBaseDef> GetLootTableImpl()
        {
            if (parentEntity is IHasComputableStateMachine stateMachineOwner)
            {
                var states = await stateMachineOwner.ComputableStateMachine.GetCurrentStates();
                var lootTable = ((LootTableStateDef)(states?.States.Find(x => x.Target is LootTableStateDef).Target))?.LootTable.Target;
                if (lootTable != null)
                    return lootTable;
            }

            return await GetDefaultLootTable();
        }

        private ValueTask<LootTableBaseDef> GetDefaultLootTable()
        {
            var entityObj = (IEntityObject)parentEntity;
            if (entityObj == null)
            {
                Logger.IfError()?.Message($"Entity {parentEntity} (guid: {parentEntity.Id}) unexpected is not {nameof(IEntityObject)}.").Write();
                return new ValueTask<LootTableBaseDef>(null as LootTableBaseDef);
            }
            if (entityObj.Def == null)
            {
                Logger.IfError()?.Message($"Entity {parentEntity} (guid: {parentEntity.Id}) `{nameof(entityObj.Def)}`==null.").Write();
                return new ValueTask<LootTableBaseDef>(null as LootTableBaseDef);
            }
            var lootableDef = entityObj.Def as ILootableDef;
            if (lootableDef == null)
            {
                Logger.IfError()?.Message($"Entity {parentEntity} (guid: {parentEntity.Id}) `{nameof(entityObj.Def)}` unexpected is not `{nameof(ILootableDef)}`.").Write();
                return new ValueTask<LootTableBaseDef>(null as LootTableBaseDef);
            }

            return new ValueTask<LootTableBaseDef>(lootableDef.DefaultLootTable.Target);
        }

        public async Task<IEnumerable<ItemResourcePack>> GetLootListImpl(Guid looterId)
        {
            var mortal = parentEntity as IHasMortal;
            if (mortal != null && !mortal.Mortal.IsAlive)
                return Enumerable.Empty<ItemResourcePack>();

            var table = await GetLootTable();
            if (table == null)
            {
                Logger.IfDebug()?.Message/*Error2069*/($"{SharedHelpers.NowStamp}  #DBG: *********************** `{nameof(table)}` == null return empty List")
                    .Write();
                return Enumerable.Empty<ItemResourcePack>();
            }

            var res = await table.CalcItems(new LootListRequest(looterId), parentEntity.Id, parentEntity.TypeId, EntitiesRepository);

            if (_debugEnabled)
            {//#Dbg:

                var str = "Normalized Result list: " + Environment.NewLine + string.Join("," + Environment.NewLine, res);
                Logger.IfDebug()?.Message/*Error2069*/($"#DBG:" + str)
                    .Write();
            }

            return res;
        }

        public Task OnInit()
        {
            if (!(parentEntity is IHasComputableStateMachine))
                Logger.IfError()?.Message($"Every entity, implementing `{nameof(ILootable)}` interface, should implement `{nameof(IHasComputableStateMachine)}` also (even if you don't use its functionality explicitly).").Write();
            return Task.CompletedTask;
        }
    }
}
