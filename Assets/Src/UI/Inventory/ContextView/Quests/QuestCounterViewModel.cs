using L10n;
using NLog;
using ReactivePropsNs;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestCounterViewModel : SomeItemViewModel<QuestCounterData>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static int _nextFreeId = 0;
        private int Id;


        public void Awake() {
            Id = _nextFreeId++;
        }

        private QuestCounterData _questCounterData;

        //=== Props ===========================================================

        [Binding]
        public bool HasItemResourceSource => _questCounterData?.MutationSource == null;

        [Binding]
        public bool HasMutationSource => _questCounterData?.MutationSource != null;

        [Binding]
        public string ItemResourceName => _questCounterData?.Name?.GetText();

        [Binding]
        public Sprite ItemResourceSprite => _questCounterData?.Icon?.Target;

        [Binding]
        public bool HasItemResourceSprite => ItemResourceSprite != null;

        [Binding]
        public int ItemResourceCount => _questCounterData?.RequiredCount ?? 0;

        [Binding]
        public int ItemResourceCurrCount { get; private set; }
        private static readonly PropertyBinder<QuestCounterViewModel, int> ItemResourceCurrCountBinder = PropertyBinder<QuestCounterViewModel>.Create(_ => _.ItemResourceCurrCount);

        [Binding]
        public string MutationStageName => _questCounterData?.MutationSource?.Stage?.NameLs.GetText() ?? "";

        // Жизненный цикл "Пока подключено"
        private DisposableComposite _fillDisposables = new DisposableComposite();

        //=== Public ==========================================================

        public override void Fill(QuestCounterData questCounterData)
        {
            // PZ-15197 // Logger.Error($"$$$$$$$$$$ {GetType().NiceName()}[{name},{Id}].Fill({questCounterData}) // Count:{questCounterData?.Count.StreamState()}");
            // PZ-15197 // Logger.Error($"$$$$$$$$$$ DEEP LOG\n{questCounterData?.Count?.DeepLog("$$$$$$$$$$ Count.DeepLog:\t")}");
            (_fillDisposables).Clear(); // Это такой дисконнект

            if (questCounterData.AssertIfNull(nameof(questCounterData)))
                return;

            questCounterData.Count?.Subscribe(_fillDisposables, count => {
                // PZ-15197 // Logger.Error($"$$$$$$$$$$ {GetType().NiceName()}[{name},{Id}].questCounterData.Count.OnNext({count})");
                ItemResourceCurrCount = count;
                NotifyPropertyChanged(nameof(ItemResourceCurrCount));
            }, () => {
                // PZ-15197 // Logger.Error($"$$$$$$$$$$ {GetType().NiceName()}[{name},{Id}].questCounterData.Count.OnComplete()");
            });

            _questCounterData = questCounterData;
            if (_questCounterData.Count != null)
                _questCounterData.Count.Bind(_fillDisposables, this, ItemResourceCurrCountBinder); //NotifyPropertyChanged(nameof(ItemResourceCurrCount));

            NotifyPropertyChanged(nameof(HasItemResourceSource));
            NotifyPropertyChanged(nameof(HasMutationSource));
            NotifyPropertyChanged(nameof(ItemResourceName));
            NotifyPropertyChanged(nameof(ItemResourceSprite));
            NotifyPropertyChanged(nameof(HasItemResourceSprite));
            NotifyPropertyChanged(nameof(ItemResourceCount));
            NotifyPropertyChanged(nameof(MutationStageName));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _fillDisposables.Dispose();
        }
    }
}