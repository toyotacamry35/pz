using System.ComponentModel;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using L10n;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ScienceViewModel : SomeItemViewModel<ScienceViewModelData>
    {
        [SerializeField, UsedImplicitly]
        private JdbMetadata _scienceJdb;

        private ScienceViewModel _masterScienceViewModel;


        //=== Props ===========================================================

        public ScienceDef ScienceDef { get; private set; }

        public bool HasMaster => _masterScienceViewModel != null;

        private int _count;

        [Binding]
        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSufficient;

        [Binding]
        public bool IsSufficient
        {
            get => _isSufficient;
            private set
            {
                if (_isSufficient != value)
                {
                    _isSufficient = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isBenefit;

        [Binding]
        public bool IsBenefit
        {
            get => _isBenefit;
            private set
            {
                if (_isBenefit != value)
                {
                    _isBenefit = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _scienceName;

        [Binding]
        public LocalizedString ScienceName
        {
            get => _scienceName;
            private set
            {
                if (!_scienceName.Equals(value))
                {
                    _scienceName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _scienceIcon;

        [Binding]
        public Sprite ScienceIcon
        {
            get => _scienceIcon;
            private set
            {
                if (_scienceIcon != value)
                {
                    _scienceIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _scienceMiniIcon;

        [Binding]
        public Sprite ScienceMiniIcon
        {
            get => _scienceMiniIcon;
            private set
            {
                if (_scienceMiniIcon != value)
                {
                    _scienceMiniIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _scienceInactiveMiniIcon;

        [Binding]
        public Sprite ScienceInactiveMiniIcon
        {
            get => _scienceInactiveMiniIcon;
            private set
            {
                if (_scienceInactiveMiniIcon != value)
                {
                    _scienceInactiveMiniIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_scienceJdb == null)
                return;

            var scienceDef = _scienceJdb.Get<ScienceDef>();
            if (scienceDef.AssertIfNull(nameof(scienceDef)))
                return;

            Set(scienceDef);
        }


        //=== Public ==========================================================

        /// <summary>
        /// Может быть инициирована через этот метод, а может через _scienceJdb в Awake
        /// </summary>
        public void Set(ScienceDef scienceDef = null, int count = 0, bool isBenefit = false, int availCount = int.MaxValue)
        {
            if (scienceDef != null)
            {
                ScienceDef = scienceDef;
                ScienceIcon = ScienceDef.Sprite?.Target;
                ScienceMiniIcon = ScienceDef.MiniSprite?.Target;
                ScienceInactiveMiniIcon = ScienceDef.InactiveMiniSprite?.Target;
                ScienceName = ScienceDef.NameLs;
            }

            Count = count;
            IsBenefit = isBenefit;
            IsSufficient = IsBenefit || Count <= availCount;
            IsEmpty = Count == 0;
        }

        public override void Fill(ScienceViewModelData data) //позже добавленный интерфейс SomeItemViewModel<>
        {
            Set(data?.ScienceDef, data?.Count ?? 0, data?.IsBenefit ?? true, data?.AvailCount ?? int.MaxValue);
        }

        /// <summary>
        /// Альтернатива управлению видом через Set() - репликация состояния another 
        /// (только в части подсчитываемых параметров: Count, IsBenefit, IsSufficient)
        /// </summary>
        public void LinkToMaster(ScienceViewModel master)
        {
            _masterScienceViewModel = master;
            if (_masterScienceViewModel.AssertIfNull(nameof(_masterScienceViewModel)))
                return;

            Count = _masterScienceViewModel.Count;
            IsBenefit = _masterScienceViewModel.IsBenefit;
            IsSufficient = _masterScienceViewModel.IsSufficient;
            IsEmpty = _masterScienceViewModel.IsEmpty;
            _masterScienceViewModel.PropertyChanged += OnAnotherPropertyChanged;
        }

        public void UnlinkFromMaster()
        {
            if (_masterScienceViewModel.AssertIfNull(nameof(_masterScienceViewModel)))
                return;

            _masterScienceViewModel.PropertyChanged -= OnAnotherPropertyChanged;
            _masterScienceViewModel = null;
        }


        //=== Protected =======================================================

        protected override bool GetIsEmpty()
        {
            return Count == 0;
        }


        //=== Private =========================================================

        private void OnAnotherPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_masterScienceViewModel.AssertIfNull(nameof(_masterScienceViewModel)))
                return;

            switch (args.PropertyName)
            {
                case nameof(Count):
                    Count = _masterScienceViewModel.Count;
                    return;

                case nameof(IsSufficient):
                    IsSufficient = _masterScienceViewModel.IsSufficient;
                    return;

                case nameof(IsBenefit):
                    IsBenefit = _masterScienceViewModel.IsBenefit;
                    return;

                case nameof(IsEmpty):
                    IsEmpty = _masterScienceViewModel.IsEmpty;
                    return;
            }
        }
    }
}