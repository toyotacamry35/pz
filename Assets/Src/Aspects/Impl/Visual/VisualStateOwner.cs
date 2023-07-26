using System;
using System.Collections;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using UnityEngine;
 
namespace Assets.Src.Aspects.Impl.Visual
{
    internal interface IVisualStateOwner
    {
        event Action<GameObject> VisualChanged;
    }

    [DisallowMultipleComponent]
    internal class VisualStateOwner : EntityGameObjectComponent, IVisualStateOwner
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("VisualStateOwner");
        
        //[NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("VisualStateOwner");

        public event Action<GameObject> VisualChanged;

        protected override void GotClient()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#Dbg: 0. GotClient.  ({TypeId}, {EntityId})").Write();
            this.StartInstrumentedCoroutine(StateMachineChecking());
        }

        // --- Privates: --------------------------------------------------

        private float _stateMachineCheckingStep = 15f;

        private IEnumerator StateMachineChecking()
        {
            var typeId = TypeId;
            var id = EntityId;
            while (true)
            {
                var ret = AsyncUtils.RunAsyncTask(async () => await UpdateVisualState(typeId, id), ClientRepo);

                while (!ret.IsCompleted)
                    yield return null;

                var newVisualPrefab = ret.Result;

                Logger.IfDebug()?.Message/*Error2335*/($"{SharedHelpers.NowStamp} #Dbg: X-2. UpdateVisualState. `{nameof(newVisualPrefab)}`: {newVisualPrefab}.")
                    .Write();
                if(newVisualPrefab)
                    VisualChanged?.Invoke(newVisualPrefab);

                // note: we neglect the task execution time
                yield return new WaitForSeconds(_stateMachineCheckingStep);
            }
            // ReSharper disable once IteratorNeverReturns
        }


        // Wierd, but props `TypeId` & `EntityId` are == 0 here if I don't pass their values as args to this func. (Wierd_x2: I should only pass & even shouldn't use these values)
        private async Task<GameObject> UpdateVisualState(int typeId, Guid id)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#Dbg: 1. UpdateVisualState.  ({TypeId}, {EntityId})"); // / {typeId}, {id}).").Write();

            using (var wrapper = await ClientRepo.Get(TypeId, EntityId))
            {
                var entity = wrapper.Get<IHasComputableStateMachineClientBroadcast>(TypeId/*typeId*/, EntityId/*id*/, ReplicationLevel.ClientBroadcast);
                if (entity != null)
                {
                    var currStates = await entity.ComputableStateMachine.GetCurrentStates();
                    var currPrefabState = currStates?.States?.Find(x => x.Target is PrefabStateDef);
                    if (currPrefabState != null)
                    {
                        var newVisualPrefab = ((PrefabStateDef)currPrefabState).Prefab?.Target;
                        return newVisualPrefab;
                    }
                }
                return null;
            }

        }
    }
}
