using System;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using Assets.Src.ContainerApis;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils.Extensions;
using Uins.Sound;

namespace Uins.Inventory
{
    public class CraftQueueSlots : HasDisposablesMonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        CraftSlotViewModel[] _elements;


        //=== Props ===========================================================

        public ReactiveProperty<EntityApiWrapper<CraftEngineQueueFullApi>> CraftEngineQueueFullApiWrapperRp { get; }
            = new ReactiveProperty<EntityApiWrapper<CraftEngineQueueFullApi>>() {Value = null};

        public OuterRef CraftEngineOuterRef { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_elements.IsNullOrEmptyOrHasNullElements(nameof(_elements)))
                return;

            SwitchAndProcessElements(-1, csvm => csvm.Init(this));

            if (_elements.GroupBy(el => el.QuequeElementIndex).Any(g => g.Count() > 1))
                UI. Logger.IfError()?.Message("Craft queque elements has double indices").Write();;
        }


        //=== Public ==========================================================

        public void Subscribe(OuterRef craftEngineOuterRef, bool removeWrapperOnDispose, int queueSize)
        {
            if (!craftEngineOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(craftEngineOuterRef)} is invalid").Write();
                return;
            }

            CraftEngineOuterRef = craftEngineOuterRef; //Для отмены из очереди крафта
            CraftEngineQueueFullApiWrapperRp.Value = EntityApi.GetWrapper<CraftEngineQueueFullApi>(craftEngineOuterRef.To<IEntityObject>(),
                removeWrapperOnDispose);

            SwitchAndProcessElements(queueSize);
            CraftEngineQueueFullApiWrapperRp.Value?.EntityApi?.SubscribeToCraftQueueEvents(OnCraftQueueEvent);
        }

        public void Unsubscribe()
        {
            CraftEngineOuterRef = OuterRef.Invalid;
            CraftEngineQueueFullApiWrapperRp.Value?.EntityApi?.UnsubscribeFromCraftQueueEvents(OnCraftQueueEvent);
            var lastWrapper = CraftEngineQueueFullApiWrapperRp.Value;
            CraftEngineQueueFullApiWrapperRp.Value = null;
            lastWrapper?.Dispose();
            SwitchAndProcessElements(-1);
        }


        //=== Private =========================================================

        private void SwitchAndProcessElements(int queueSize, Action<CraftSlotViewModel> elementAction = null)
        {
            for (int i = 0, len = _elements.Length; i < len; i++)
            {
                var elem = _elements[i];
                elementAction?.Invoke(elem);
                elem.IsVisibleRp.Value = elem.QuequeElementIndex < queueSize;
            }
        }

        private void OnCraftQueueEvent(CraftQueueEvent craftQueueEvent)
        {
            //UI.CallerLogInfo($"craftQueueEvent={craftQueueEvent}"); //DEBUG
            AK.Wwise.Event soundEvent = null;
            switch (craftQueueEvent)
            {
                case CraftQueueEvent.HandcraftQueueStarted:
                    soundEvent = SoundControl.Instance.HandcraftTaskStartEvent;
                    break;

                case CraftQueueEvent.TaskAdded:
                    soundEvent = SoundControl.Instance.CraftTaskAddEvent;
                    break;

                case CraftQueueEvent.TaskCompleted:
                    soundEvent = SoundControl.Instance.CraftTaskSuccessEndEvent;
                    break;

                case CraftQueueEvent.QueueCompleted:
                    soundEvent = SoundControl.Instance.CraftQueueCompleteEvent;
                    break;

                case CraftQueueEvent.BenchSequenceStarted:
                    soundEvent = SoundControl.Instance.BenchQueueStartEvent;
                    break;

                case CraftQueueEvent.BenchSequenceEnded:
                    soundEvent = SoundControl.Instance.BenchQueueCompleteEvent;
                    break;

                case CraftQueueEvent.QueuePaused:
                    soundEvent = SoundControl.Instance.CraftQueuePauseEvent;
                    break;
            }

            if (!soundEvent.AssertIfNull(nameof(soundEvent)))
                soundEvent.Post(transform.root.gameObject);
        }
    }
}