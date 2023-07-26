using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using Uins;
using UnityEngine;

namespace Assets.Src.Impacts
{
    [UsedImplicitly, PredictableEffect]
    public class EffectGetInfo : IClientOnlyEffectBinding<EffectGetInfoDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repository, EffectGetInfoDef def)
        {
            // if (!cast.OnClientWithAuthority())
            //     return;
            //
            // var selfDef = def;
            // var targetRef = await selfDef.Target.Target.GetOuterRef(cast, repository);
            //
            // using (var wrapper = await repository.Get(targetRef.TypeId, targetRef.Guid))
            // {
            //     var entity = wrapper.Get<IEntityObjectClientBroadcast>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.ClientBroadcast);
            //     if (entity == null)
            //     {
            //          Logger.IfError()?.Message("No IEntityObjectClientBroadcast {0}",  targetRef.TypeId).Write();
            //         return;
            //     }
            //     
            //     var entityDef = entity.Def;
            //     LootTableBaseDef baseLootTable = null;
            //     if (entityDef is MineableEntityDef)
            //     {
            //         baseLootTable = ((MineableEntityDef)entity.Def)?.LootTable.Target;
            //     }
            //     else if (entityDef is InteractiveEntityDef)
            //     {
            //         baseLootTable = ((InteractiveEntityDef)entity.Def)?.DefaultLootTable.Target;
            //     }
            //     
            //     if (baseLootTable != null)
            //     {
            //         Logger.IfInfo()?.Message($"EffectGetInfo: baseLootTable = {baseLootTable}").Write();
            //     
            //         UnityQueueHelper.RunInUnityThread(() =>
            //         {
            //             var aimMenu = Object.FindObjectOfType<AimMenu>();
            //             if (aimMenu != null)
            //             {
            //                 aimMenu.SwitchVisibility(true, baseLootTable);
            //             }
            //         });
            //     }
            // }
            //
            // return;
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectGetInfoDef def)
        {
            return new ValueTask();
        }
    }
}
