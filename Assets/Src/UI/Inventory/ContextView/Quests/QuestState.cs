using System;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Entities.Engine;
using SharedCode.Utils;
using NLog;
using System.Threading;
using System.Collections.Generic;

namespace Uins
{
    public class QuestState
    {
        private static int _nextFreeId = 0;
        public int Id;

        public QuestDef QuestDef;
        public int PhaseIndex;

        public readonly IStream<QuestStateReactive> QuestReactive;


        public bool IsDone => QuestItemViewModel.GetQuestIsDone(QuestDef, PhaseIndex);

        public QuestState(QuestDef questDef, IStream<QuestStateReactive> questReactive, int phaseIndex = 0)
        {
            Id = Interlocked.Increment(ref _nextFreeId);
            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ new QuestState[{Id}]({questDef}, {questReactive}, {phaseIndex}) // questReactive => {questReactive.StreamState()}").Write();
            QuestDef = questDef;
            PhaseIndex = phaseIndex;
            QuestReactive = questReactive;
        }

        public override string ToString()
        {
            return $"[{nameof(QuestState)}[{Id}]: Def={QuestDef}, {nameof(PhaseIndex)}={PhaseIndex}, {nameof(IsDone)}{IsDone.AsSign()}]";
        }
    }

    public class QuestCounterState : IDisposable
    {
        private static int _nextFreeId = 0;
        public readonly int Id;
        public readonly QuestDef questDef;
        public readonly QuestCounterDef counterDef;

        public IStream<int> CountForClient;
        public IStream<int> DamageSumCounter;

        /// <summary>
        /// Внутри может быть какая-то сложная структура из комбинаторов. Здесь мы её разворачиваем в плоский массив, не содержайщий композиции.
        /// В простом случае список соджержит одну ссылку на самого себя. Поскольку дефы у меня на лету не меняются, просто делаю статичный массив.
        /// </summary>
        public QuestCounterState[] flatList { get; private set; }

        private DisposableComposite D = new DisposableComposite();

        public QuestCounterState(IQuestCounterClientFull counter, ITouchable<IQuestCounterClientFull> touchable)
        {
            Id = Interlocked.Increment(ref _nextFreeId);
            // PZ-15197 // Logger.IfInfo()?.Message($"$$$$$$$$$$ new QuestCounterState[{Id}]({counter}, {touchable?.GetType().NiceName() ?? "<null>"}) // count:{counter.CountForClient} def:{counter.CounterDef} quest:{counter.QuestDef}").Write();
            questDef = counter.QuestDef;
            counterDef = counter.CounterDef;
            CountForClient = touchable.ToStream(D, c => c.CountForClient);
            DamageSumCounter = touchable.OfType<IQuestCounterClientFull, IDealDamageCounterClientFull>(D)
                .ToStream(D, c => c.SumValueForClient)
                .Func(D, damage => (int) Math.Floor(damage));

            var flat = new List<QuestCounterState>();
            // Разворачиванеи композиции в плоский массив. Вызывается сугубо под локом, поэтому выполняем буквально.
            if (counter is ICombinatorCounterClientFull combinator)
            {
                for (int i = 0; i < combinator.SubCounters.Count; i++)
                {
                    var wrapper = combinator.SubCounters[i];
                    var subCounter = wrapper.Counter;
                    var subQuestCounterState = new QuestCounterState(
                        subCounter,
                        touchable
                            .OfType<IQuestCounterClientFull, ICombinatorCounterClientFull>(D)
                            .ListItemTouchable(D, combinatorInstance => combinatorInstance.SubCounters, wrapper)
                            .Child(D, wrapperInstance => wrapperInstance.Counter)
                    );
                    for (int j = 0; j < subQuestCounterState.flatList.Length; j++)
                        flat.Add(subQuestCounterState.flatList[j]);
                    flatList = flat.ToArray();
                }
            }
            else
            {
                flatList = new QuestCounterState[] {this};
            }

            // PZ-15197 // Logger.Info($"$$$$ flatList.Length = {flatList.Length}");
            // PZ-15197 // for (int i = 0; i < flatList.Length; i++)
            // PZ-15197 //     Logger.IfInfo()?.Message($"$$$ flatList[{i}] = {flatList[i].questDef}").Write();
        }

        public void Dispose()
        {
            D.Dispose();
        }

        public override string ToString()
        {
            return $"[QuestCounterState[Id={Id}] def={counterDef} ]";
        }
    }

    /// <summary>
    /// Вариант с явно лишними промежуточными хранилищами сделал исключительно ради ToString, да и вообще часть полей не потребуется, и их можно будет поотключать.
    /// </summary>
    public class QuestStateReactive : IDisposable
    {
        private static int _nextFreeId = 0;
        private int Id;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private DisposableComposite D = new DisposableComposite();

        public readonly QuestDef questDef;

        /// <summary> Дублирование отсюда поубираю если всплывёт ошибка с преждевременным диспоузом  </summary>
        public ReactiveProperty<QuestStatus> _status = new ReactiveProperty<QuestStatus>();

        public IStream<QuestStatus> Status => _status;
        public ReactiveProperty<int> _phaseIndex = new ReactiveProperty<int>();
        public IStream<int> PhaseIndex => _phaseIndex;
        public ReactiveProperty<QuestCounterState> _phaseSuccCounter = new ReactiveProperty<QuestCounterState>();
        public IStream<QuestCounterState> PhaseSuccCounter => _phaseSuccCounter;
        public ReactiveProperty<QuestCounterState> _phaseFailCounter = new ReactiveProperty<QuestCounterState>();
        public IStream<QuestCounterState> PhaseFailCounter => _phaseFailCounter;
        public ReactiveProperty<bool> _havePhaseSuccCounter = new ReactiveProperty<bool>();
        public IStream<bool> HavePhaseSuccCounter => _havePhaseSuccCounter;
        public ReactiveProperty<bool> _havePhaseFailCounter = new ReactiveProperty<bool>();
        public IStream<bool> HavePhaseFailCounter => _havePhaseFailCounter;

        // Вместо отдельной модели для стейта можно все нужные стримы создать в этом уровне
        public ReactiveProperty<PhaseDef> _currentPhaseDef = new ReactiveProperty<PhaseDef>();
        public IStream<PhaseDef> CurrentPhaseDef => _currentPhaseDef;

        // Вызывается строго в тредпуле, поэтому quest доступен
        public QuestStateReactive(QuestDef def, IQuestObjectClientFull quest, ITouchable<IQuestObjectClientFull> questTouchable)
        {
            Id = Interlocked.Increment(ref _nextFreeId);
            // PZ-15197 // Logger.IfInfo()?.Message($"$$$$$$$$$$ new QuestStateReactive[{Id}]({def}, {quest}, {questTouchable}) // ").Write();
            // PZ-15197 // if (quest != null)
            // PZ-15197 //     Logger.IfInfo()?.Message($"$$$$$$$$$$ PhaseIndex:{quest.PhaseIndex}; HavePhaseSuccCounter:{quest.HavePhaseSuccCounter}; PhaseSuccCounter:{quest.PhaseSuccCounter}; CountForClient:{quest.PhaseSuccCounter?.CountForClient};// ").Write();
            questDef = def;
            questTouchable.ToStream(D, q => q.Status).Action(D, value => _status.Value = value);
            questTouchable.ToStream(D, q => q.PhaseIndex).Action(D, value => _phaseIndex.Value = value);
            questTouchable.ToModelStream(
                    D,
                    q => q.PhaseSuccCounter,
                    (counter, touchable) => counter != null ? new QuestCounterState(counter, touchable()) : null)
                .Action(D, counterStateModel => _phaseSuccCounter.Value = counterStateModel);
            questTouchable.ToModelStream(
                    D,
                    q => q.PhaseFailCounter,
                    (counter, touchable) => counter != null ? new QuestCounterState(counter, touchable()) : null)
                .Action(D, counterStateModel => _phaseFailCounter.Value = counterStateModel);
            questTouchable.ToStream(D, q => q.HavePhaseSuccCounter).Action(D, value => _havePhaseSuccCounter.Value = value);
            questTouchable.ToStream(D, q => q.HavePhaseFailCounter).Action(D, value => _havePhaseFailCounter.Value = value);
            // PZ-15197 // questTouchable.Log(D, prefix: $"$$$$$$$$$$ QuestStateReactive[{Id}].questTouchable: ", questObj => questObj != null ? $"{questObj.GetType().NiceName()} PhaseIndex:{questObj.PhaseIndex} HavePhaseSuccCounter:{questObj.HavePhaseSuccCounter}; PhaseSuccCounter:{questObj.PhaseSuccCounter}" : "<null>", Logger);

            _phaseIndex.Zip(D, _status)
                .Func(
                    D,
                    (index, status) => status == QuestStatus.Sucess ? questDef.Phases[questDef.Phases.Length - 1].Target :
                        index > 0 && index < questDef.Phases.Length ? questDef.Phases[index].Target : null)
                .Action(D, phaseDef => _currentPhaseDef.Value = phaseDef);
        }

        public void Dispose()
        {
            // PZ-15197 // Logger.Info($"$$$$$$$$$$ QuestStateReactive[{Id}].Dispose() // {questDef}");
            D.Dispose();
        }

        public override string ToString()
        {
            var sb = StringBuildersPool.Get.Clear();
            sb.Append("[QuestStateReactive[").Append(Id).Append("] def:").Append(questDef);
            if (_status.HasValue) sb.Append(" status:").Append(_status.Value);
            if (_phaseIndex.HasValue) sb.Append(" phaseIndex:").Append(_phaseIndex.Value);
            if (_phaseSuccCounter.HasValue) sb.Append(" phaseSuccCounter:").Append(_phaseSuccCounter.Value);
            if (_phaseFailCounter.HasValue) sb.Append(" phaseFailCounter:").Append(_phaseFailCounter.Value);
            if (_havePhaseSuccCounter.HasValue) sb.Append(" havePhaseSuccCounter:").Append(_havePhaseSuccCounter.Value);
            if (_havePhaseFailCounter.HasValue) sb.Append(" havePhaseFailCounter:").Append(_havePhaseFailCounter.Value);
            sb.Append("]");
            return sb.ToStringAndReturn();
        }
    }
}