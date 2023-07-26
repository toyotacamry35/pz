using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Uins;

namespace Assets.Src.ContainerApis
{
    public delegate void InaccessibleSlotDelegate(int slotId, bool isAdded);

    public class HasDollFullApi : SlotsCollectionApi
    {
        private List<int> _inaccessibleSlotsIndices = new List<int>();

        private event InaccessibleSlotDelegate InaccessibleSlotChanged;


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        protected override bool WatchForSubitems => true;


        //=== Public ==========================================================

        public void SubscribeToInaccessibleSlots(InaccessibleSlotDelegate onInaccessibleSlotChanged)
        {
            if (onInaccessibleSlotChanged.AssertIfNull(nameof(onInaccessibleSlotChanged)))
                return;

            InaccessibleSlotChanged += onInaccessibleSlotChanged;
            if (_inaccessibleSlotsIndices.Count > 0)
            {
                for (int i = 0, len = _inaccessibleSlotsIndices.Count; i < len; i++)
                    onInaccessibleSlotChanged.Invoke(_inaccessibleSlotsIndices[i], true);
            }
        }

        public void UnsubscribeFromInaccessibleSlots(InaccessibleSlotDelegate onInaccessibleSlotChanged)
        {
            if (onInaccessibleSlotChanged.AssertIfNull(nameof(onInaccessibleSlotChanged)))
                return;

            InaccessibleSlotChanged -= onInaccessibleSlotChanged;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var characterDollClientFull = ((IHasDollClientFull) wrapper)?.Doll;
            if (characterDollClientFull.AssertIfNull(nameof(characterDollClientFull)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(characterDollClientFull);

            await SlotListenersCollection.SubscribeOnItems(characterDollClientFull.Items);

            characterDollClientFull.BlockedSlotsId.OnItemAdded += OnInaccessibleSlotIndexAdded;
            characterDollClientFull.BlockedSlotsId.OnItemRemoved += OnInaccessibleSlotIndexRemoved;
            if (characterDollClientFull.BlockedSlotsId.Count > 0)
            {
                foreach (var id in characterDollClientFull.BlockedSlotsId)
                    AddInaccessibleSlotsIndex(id);

                if (_inaccessibleSlotsIndices.Count > 0 && InaccessibleSlotChanged != null)
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        for (int i = 0, len = _inaccessibleSlotsIndices.Count; i < len; i++)
                            InaccessibleSlotChanged.Invoke(_inaccessibleSlotsIndices[i], true);
                    });
                }
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var characterDollClientFull = ((IHasDollClientFull) wrapper)?.Doll;
            if (characterDollClientFull.AssertIfNull(nameof(characterDollClientFull)))
                return;

            characterDollClientFull.BlockedSlotsId.OnItemAdded -= OnInaccessibleSlotIndexAdded;
            characterDollClientFull.BlockedSlotsId.OnItemRemoved -= OnInaccessibleSlotIndexRemoved;
            while (_inaccessibleSlotsIndices.Count > 0)
                _inaccessibleSlotsIndices.RemoveAt(_inaccessibleSlotsIndices.Count - 1);
            InaccessibleSlotChanged = null;
        }


        //=== Private =========================================================

        private void AddInaccessibleSlotsIndex(int id, bool doEventInUnityThread = false)
        {
            if (_inaccessibleSlotsIndices.Contains(id))
            {
                UI.Logger.IfError()?.Message($"{nameof(_inaccessibleSlotsIndices)} already contains id={id}").Write();
            }

            _inaccessibleSlotsIndices.Add(id);

            if (doEventInUnityThread && InaccessibleSlotChanged != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => { InaccessibleSlotChanged.Invoke(id, true); });
            }
        }

        private void RemoveInaccessibleSlotsIndex(int id, bool doEventInUnityThread = false)
        {
            if (!_inaccessibleSlotsIndices.Contains(id))
            {
                UI.Logger.IfError()?.Message($"{nameof(_inaccessibleSlotsIndices)} don't contains id={id}").Write();
            }

            _inaccessibleSlotsIndices.Remove(id);

            if (doEventInUnityThread && InaccessibleSlotChanged != null)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => { InaccessibleSlotChanged.Invoke(id, false); });
            }
        }

        private Task OnInaccessibleSlotIndexAdded(DeltaListChangedEventArgs<int> args)
        {
            AddInaccessibleSlotsIndex(args.Value, true);
            return Task.CompletedTask;
        }

        private Task OnInaccessibleSlotIndexRemoved(DeltaListChangedEventArgs<int> args)
        {
            RemoveInaccessibleSlotsIndex(args.Value, true);
            return Task.CompletedTask;
        }
    }
}