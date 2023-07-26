using System.Linq;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class AchievedPointsInfoViewModel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _localizationKeyPairsDefRef;

        [SerializeField, UsedImplicitly]
        private TechPointsCollection _techPointsCollection;

        [SerializeField, UsedImplicitly]
        private SciencesCollection _sciencesCollection;


        //=== Props ===========================================================

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public LocalizedString TitleLs => _localizationKeyPairsDefRef.Target?.Ls1 ?? LsExtensions.EmptyWarning;


        //=== Unity ===========================================================

        private void Awake()
        {
            _localizationKeyPairsDefRef.Target.AssertIfNull(nameof(_localizationKeyPairsDefRef));
            _techPointsCollection.AssertIfNull(nameof(_techPointsCollection));
            _sciencesCollection.AssertIfNull(nameof(_sciencesCollection));
        }

        public void Init(AchievedPointsNotificationInfo achievedPointsInfo)
        {
            IsVisible = achievedPointsInfo != null;
            if (!IsVisible)
                return;

            _techPointsCollection.FillCollection(achievedPointsInfo.HasTechPointCounts
                ? achievedPointsInfo.TechPointCounts
                    .Select(tpc => new TechPointViewModelData(tpc.TechPoint, tpc.Count, true))
                    .ToArray()
                : null);

            _sciencesCollection.FillCollection(achievedPointsInfo.HasScienceCounts
                ? achievedPointsInfo.ScienceCounts
                    .Select(sc => new ScienceViewModelData(sc.Science, sc.Count, true))
                    .ToArray()
                : null);
        }
    }
}