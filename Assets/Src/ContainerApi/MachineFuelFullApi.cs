using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class MachineFuelFullApi : MachineBaseFullApi
    {
        public event Action<IList<BaseItemResource>> AcceptableFuelList;

        private ThreadSafeList<BaseItemResource> _acceptableFuel = new ThreadSafeList<BaseItemResource>();

        private bool _haveAcceptableFuelInfo;


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Public ==========================================================

        /// <summary>
        /// В отличии от обычных подписок/отписок действие одноразовое:
        /// как только получает непустую инфу, раздает ее подписчикам и сразу же их отписывает, 
        /// соответственно подписывает только пока информации нет
        /// </summary>
        public void GetAcceptableFuelList(Action<IList<BaseItemResource>> callback)
        {
            if (_haveAcceptableFuelInfo)
                callback.Invoke(_acceptableFuel);
            else
                AcceptableFuelList += callback;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;
            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldMachineClientFull.FuelContainer);

            await SlotListenersCollection.SubscribeOnItems(worldMachineClientFull.FuelContainer.Items);
            SubscribeToEntityCollectionSize(worldMachineClientFull.FuelContainer.To<IMachineFuelContainerClientBroadcast>());

            var acceptableFuel = ((WorldMachineDef) worldMachineClientFull.Def)?.AcceptableFuel;

            if ((acceptableFuel?.Length ?? 0) > 0)
            {
                GetAcceptableFuelDeltaList(acceptableFuel);
            }
            else
            {
                Logger.IfError()?.Message($"{nameof(acceptableFuel)} is empty").Write();
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldMachineClientFull.FuelContainer.Items);
            UnsubscribeFromEntityCollectionSize(worldMachineClientFull.FuelContainer.To<IMachineFuelContainerClientBroadcast>());

            //--- AcceptableFuel
            AcceptableFuelList = null;
        }

        //=== Private =========================================================

        private void GetAcceptableFuelDeltaList(FuelDef[] deltaList)
        {
            _acceptableFuel = deltaList != null
                ? new ThreadSafeList<BaseItemResource>(deltaList.Select(v => v.Fuel.Target).Where(e => e != null))
                : new ThreadSafeList<BaseItemResource>();
            _haveAcceptableFuelInfo = true;
            if (_acceptableFuel.Count == 0)
            {
                Logger.IfError()?.Message($"{nameof(_acceptableFuel)} is empty").Write();
                return;
            }

            if (AcceptableFuelList != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    AcceptableFuelList.Invoke(_acceptableFuel);
                    AcceptableFuelList = null;
                });
            }
        }
    }
}