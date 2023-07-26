using System;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using L10n;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins;

namespace Assets.Src.Aspects
{
    public class TmpKnockdownInterface : BindingVmodel
    {
        private const float DiasbledAfterClickPeriod = 5;

        private IEntitiesRepository _entitiesRepository;
        private OuterRef _entityRef;

        private ReactiveProperty<DateTime> _lastClickDateTimeRp = new ReactiveProperty<DateTime>() {Value = DateTime.Now};
        private ReactiveProperty<bool> _isActivatedRp = new ReactiveProperty<bool>();


        //=== Props ===========================================================

        public ReactiveProperty<LocalizedString> ButtonTextRp { get; } = new ReactiveProperty<LocalizedString>();

        public IStream<bool> IsButtonVisibleStream { get; }

        public IStream<bool> IsButtonInteractiveStream { get; }

        public TmpKnockdownInterface(IStream<bool> hasPawnStream)
        {
            if (hasPawnStream.AssertIfNull(nameof(hasPawnStream)))
                return;

            hasPawnStream.Action(D,
                hasPawn =>
                {
                    if (!hasPawn)
                        _isActivatedRp.Value = false; //дополнительный аварийный выключатель
                });

            IsButtonVisibleStream = _isActivatedRp;

            IsButtonInteractiveStream =
                TimeTicker.Instance.GetLocalTimer(0.5f)
                    .Zip(D, _lastClickDateTimeRp)
                    .Zip(D, _isActivatedRp)
                    .Where(D, (tickerDt, lastClickDt, isActivated) => isActivated) //отсекаем ненужные вычисления
                    .Func(D, (tickerDt, lastClickDt, isActivated) => (tickerDt - lastClickDt).TotalSeconds > DiasbledAfterClickPeriod);
        }


        //=== Public ==========================================================

        public void Activate(OuterRef entityRef, IEntitiesRepository entitiesRepository, LocalizedString buttonTextLs)
        {
            ButtonTextRp.Value = buttonTextLs;
            _isActivatedRp.Value = true;
            _entityRef = entityRef;
            _entitiesRepository = entitiesRepository;
        }

        public void Deactivate()
        {
            _isActivatedRp.Value = false;
        }

        public void OnButtonClick()
        {
            _lastClickDateTimeRp.Value = DateTime.Now;
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    using (var wrapper = await _entitiesRepository.Get(_entityRef.TypeId, _entityRef.Guid))
                    {
                        if (wrapper.TryGet<IHasMortalClientFull>(_entityRef.TypeId, _entityRef.Guid, ReplicationLevel.ClientFull, out var mortal))
                            if (mortal != null)
                                await mortal.Mortal.FinishOff();
                    }
                });
        }
    }
}