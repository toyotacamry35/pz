using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using JetBrains.Annotations;
using L10n;
using NLog;
using ResourceSystem.Aspects.Counters.Template;
using UnityEngine;
using UnityWeld.Binding;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;

namespace Uins
{
    [Binding]
    public class QuestPhaseViewModel : SomeItemViewModel<QuestPhaseData>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static int _nextFreeId = 0;
        public int Id { get; private set; }

        [SerializeField, UsedImplicitly]
        private QuestCounters _questCounters;

        private QuestPhaseData _questPhaseData;
        private QuestCounterData[] _questCounterData;


        //=== Props ===========================================================

        [Binding]
        public LocalizedString Name
        {
            get
            {
                if (_questPhaseData?.PhaseDef == null)
                    return LsExtensions.Empty;

                return _questPhaseData.PhaseDef.NameLs.IsEmpty()
                    ? LsExtensions.EmptyWarning
                    : _questPhaseData.PhaseDef.NameLs;
            }
        }

        [Binding]
        public LocalizedString ShortDescription
        {
            get
            {
                if (_questPhaseData?.PhaseDef == null)
                    return LsExtensions.Empty;

                return _questPhaseData.PhaseDef.NameLs.IsEmpty()
                    ? LsExtensions.EmptyWarning
                    : _questPhaseData.PhaseDef.ShortDescriptionLs;
            }
        }

        [Binding]
        public bool IsDone => _questPhaseData?.IsDone ?? false;

        [Binding]
        public bool IsCurrent => _questPhaseData?.IsCurrent ?? false;

        [Binding]
        public bool IsDelimiterHidden => _questPhaseData?.IsDelimiterHidden ?? false;

        [Binding]
        public bool HasQuestCounterData => _questCounterData != null && _questCounterData.Length > 0;


        //=== Unity ===========================================================

        private void Awake()
        {
            Id = System.Threading.Interlocked.Increment(ref _nextFreeId);
            _questCounters.AssertIfNull(nameof(_questCounters));
        }

        private DisposableComposite _filledDisposableAtQuestPhaseData;
        //=== Public ==========================================================

        public override void Fill(QuestPhaseData questPhaseData)
        {
            // PZ-15197 // Logger.Error($"$$$$$$$$$ QuestPhaseViewModel[{name},{Id}].Fill({questPhaseData}) // path:{TransfprmPath(this)}");

            if (questPhaseData.AssertIfNull(nameof(questPhaseData)))
                return;

            if (_filledDisposableAtQuestPhaseData == null) {
                _filledDisposableAtQuestPhaseData = new DisposableComposite();
                D.Add(_filledDisposableAtQuestPhaseData);
            } else {
                _filledDisposableAtQuestPhaseData.Clear(); // Это такой упрощённый Disconnect() или вернее DisFill()
            }

            _questPhaseData = questPhaseData;

            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(ShortDescription));
            NotifyPropertyChanged(nameof(IsDone));
            NotifyPropertyChanged(nameof(IsCurrent));
            NotifyPropertyChanged(nameof(IsDelimiterHidden));

            if (!IsCurrent)
                return;

            _questCounters.FillCollection(null);
            var counterDef = _questPhaseData?.PhaseDef?.Counter.Target;
            if (counterDef == null)
                return;


            // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ counterDef:{counterDef} of type {counterDef.GetType()}").Write();
            // всякие мои манипуляции с данными для каунтеров.

            NotifyPropertyChanged(nameof(HasQuestCounterData));

            // PZ-15197 // Logger.IfError()?.Message(_questPhaseData.QuestState.DeepLog("$$$$$$$$$ questPhaseData.QuestState:\t")).Write();

            _questPhaseData
                .QuestState
                // PZ-15197 // .LogError(_filledDisposableAtQuestPhaseData, "$$$$$$$$ QuestState $$$$$$$$$$ ", toString: qsr =>$"{qsr} PhaseSuccCounter:{qsr?.PhaseSuccCounter} => {qsr?.PhaseSuccCounter.StreamState()}", logger: Logger)
                .SubStream(_filledDisposableAtQuestPhaseData,
                    state => {
                        // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$$ QuestPhaseViewModel[{Id}]._questPhaseData.QuestState.SubStream: {state?.HavePhaseSuccCounter}; {state};").Write();
                        return state?.PhaseSuccCounter;
                    })
                // PZ-15197 // .LogError(_filledDisposableAtQuestPhaseData, "$$$$$$$$$$ SubStream.PhaseSuccCounter $$$$$$$$$$ ", logger: Logger)
                .Action(_filledDisposableAtQuestPhaseData, counter => {
                    // PZ-15197 // Logger.Error($"$$$$$$$$$$ QPVM[{Id}].PhaseSuccCounter:{counter?.GetType().NiceName()}[{counter?.Id}] // def: {counter?.questDef} // path:{TransfprmPath(this)}");
                    if (counter != null) {
                        _questCounterData = counter.flatList
                            .Where(counterState => counterState.counterDef != null && !counterState.counterDef.IsInvisible)
                            .Select(counterState => QuestCounterData.Create(counterState))
                            .ToArray();
                        // PZ-15197 // Logger.Error($"$$$$$$$$$$ QPVM[{Id}]._questCounterData.Length:{_questCounterData.Length} // path:{TransfprmPath(this)}");
                        // PZ-15197 // for (int i = 0; i < _questCounterData.Length; i++)
                        // PZ-15197 //     Logger.Error($"$$$$$$$$$$ QPVM[{Id}]._questCounterData[{i}]:{_questCounterData[i]} // path:{TransfprmPath(this)}");
                        _questCounters.FillCollection(_questCounterData);
                    } else
                        _questCounters.FillCollection(_questCounterData = null);
                    // PZ-15197 // Logger.IfError()?.Message($"$$$$$$$$$$ NotifyPropertyChanged({nameof(HasQuestCounterData)})").Write();
                    NotifyPropertyChanged(nameof(HasQuestCounterData));
                });
        }
    }
}