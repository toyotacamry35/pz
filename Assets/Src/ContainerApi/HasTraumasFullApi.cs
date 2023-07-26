using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Traumas.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace Assets.Src.ContainerApis
{
    public class HasTraumasFullApi : EntityApi
    {
        private static readonly IReadOnlyDictionary<string, TraumaDef> EmptyTraumas = Enumerable.Empty<bool>()
            .ToDictionary(v => string.Empty, v => (TraumaDef) null);

        public delegate void TraumasChangedDelegate(IReadOnlyDictionary<string, TraumaDef> newTraumas);

        private event TraumasChangedDelegate TraumasChanged;

        private IReadOnlyDictionary<string, TraumaDef> _currentTraumas = EmptyTraumas;


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Public ==========================================================

        public void SubscribeToTraumasChanged(TraumasChangedDelegate onTraumasChanged)
        {
            if (onTraumasChanged.AssertIfNull(nameof(onTraumasChanged)))
                return;

            TraumasChanged += onTraumasChanged;

            if (_currentTraumas != EmptyTraumas)
                onTraumasChanged.Invoke(_currentTraumas);
        }

        public void UnsubscribeFromActivityChanged(TraumasChangedDelegate onTraumasChanged)
        {
            if (onTraumasChanged.AssertIfNull(nameof(onTraumasChanged)))
                return;

            TraumasChanged -= onTraumasChanged;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var hasTraumasClientFull = (IHasTraumasClientFull) wrapper;
            if (hasTraumasClientFull.AssertIfNull(nameof(hasTraumasClientFull)) ||
                hasTraumasClientFull.Traumas.ActiveTraumas.AssertIfNull(nameof(hasTraumasClientFull.Traumas.ActiveTraumas)))
                return;

            //пока примитивно подписываемся просто на факт изменения и при каждом просто перечитываем все содержимое
            hasTraumasClientFull.Traumas.ActiveTraumas.OnChanged += OnActiveTraumasChanged;

            _currentTraumas = GetCurrentTraumas(hasTraumasClientFull);

            if (TraumasChanged != null && _currentTraumas != EmptyTraumas)
                UnityQueueHelper.RunInUnityThreadNoWait(() => TraumasChanged?.Invoke(_currentTraumas));
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var hasTraumasClientFull = (IHasTraumasClientFull) wrapper;
            if (hasTraumasClientFull.AssertIfNull(nameof(hasTraumasClientFull)) ||
                hasTraumasClientFull.Traumas.ActiveTraumas.AssertIfNull(nameof(hasTraumasClientFull.Traumas.ActiveTraumas)))
                return;

            hasTraumasClientFull.Traumas.ActiveTraumas.OnChanged -= OnActiveTraumasChanged;

            _currentTraumas = EmptyTraumas;

            if (TraumasChanged != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => TraumasChanged?.Invoke(_currentTraumas));
        }

        private IReadOnlyDictionary<string, TraumaDef> GetCurrentTraumas(IHasTraumasClientFull hasTraumasClientFull)
        {
            if (hasTraumasClientFull.AssertIfNull(nameof(hasTraumasClientFull)) ||
                hasTraumasClientFull.Traumas.ActiveTraumas == null ||
                hasTraumasClientFull.Traumas.ActiveTraumas.Count == 0)
                return EmptyTraumas;

            return new ConcurrentDictionary<string, TraumaDef>(hasTraumasClientFull.Traumas.ActiveTraumas.ToDictionary(v => v.Key, v => v.Value.Def));
        }

        private async Task OnActiveTraumasChanged(DeltaDictionaryChangedEventArgs eventArgs)
        {
            var entityTypeId = eventArgs.Sender.ParentTypeId;
            var entityId = eventArgs.Sender.ParentEntityId;
            var repository = ClusterCommands.ClientRepository;
            using (var wrapper = await repository.Get(entityTypeId, entityId))
            {
                var hasTraumasClientFull = wrapper.Get<IHasTraumasClientFull>(entityTypeId, entityId, ReplicationLevel.ClientFull);
                _currentTraumas = GetCurrentTraumas(hasTraumasClientFull);
            }

            if (TraumasChanged != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => TraumasChanged?.Invoke(_currentTraumas));
        }
    }
}