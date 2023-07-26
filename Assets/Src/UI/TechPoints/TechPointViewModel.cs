using System;
using System.ComponentModel;
using L10n;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding, Obsolete("2del")]
    public class TechPointViewModel : SomeItemViewModel<TechPointViewModelData>
    {
        private TechPointViewModel _masterTechPointViewModel;


        //=== Props ===========================================================

        public CurrencyResource TechPointDef { get; private set; }

        public bool HasMaster => _masterTechPointViewModel != null;

        private int _availCount;
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

        private LocalizedString _name;

        [Binding]
        public LocalizedString Name
        {
            get => _name;
            private set
            {
                if (!_name.Equals(value))
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _shortName;

        [Binding]
        public LocalizedString ShortName
        {
            get => _shortName;
            private set
            {
                if (!_shortName.Equals(value))
                {
                    _shortName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _description;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            private set
            {
                if (!_description.Equals(value))
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _techPointIcon;

        [Binding]
        public Sprite TechPointIcon
        {
            get => _techPointIcon;
            private set
            {
                if (_techPointIcon != value)
                {
                    _techPointIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _techPointMiniIcon;

        [Binding]
        public Sprite TechPointMiniIcon
        {
            get => _techPointMiniIcon;
            private set
            {
                if (_techPointMiniIcon != value)
                {
                    _techPointMiniIcon = value;
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


        //=== Unity ===========================================================

        private void Awake()
        {
            IsSufficient = IsBenefit || Count <= _availCount;
            IsEmpty = GetIsEmpty();
        }


        //=== Public ==========================================================

        public void Set(CurrencyResource techPointDef = null, int count = 0, bool isBenefit = false, int availCount = int.MaxValue)
        {
            if (techPointDef != null) //если null, это не значит что его нет - просто ранее назначили и не трогаем
            {
                TechPointDef = techPointDef;
                TechPointIcon = TechPointDef.BigIcon?.Target;
                TechPointMiniIcon = TechPointDef.Icon?.Target;
                Name = TechPointDef.ItemNameLs;
                ShortName = TechPointDef.ShortName;
                Description = TechPointDef.DescriptionLs;
            }

            _availCount = availCount;
            Count = count;
            IsBenefit = isBenefit;
            IsSufficient = IsBenefit || Count <= _availCount;
            IsEmpty = GetIsEmpty();
        }

        public override void Fill(TechPointViewModelData data) //позже добавленный интерфейс SomeItemViewModel<>
        {
            Set(data?.TechPointDef, data?.Count ?? 0, data?.IsBenefit ?? true, data?.AvailCount ?? int.MaxValue);
        }

        public override string ToString()
        {
            return $"{GetType()}: {nameof(TechPointDef)}={TechPointDef?.____GetDebugShortName()}, {nameof(Count)}={Count}, " +
                   $"{nameof(IsBenefit)}{IsBenefit.AsSign()}, {nameof(IsEmpty)}{IsEmpty.AsSign()}";
        }

        /// <summary>
        /// Альтернатива управлению видом через Set() - репликация состояния master 
        /// (только в части подсчитываемых параметров: Count, IsBenefit, IsSufficient)
        /// </summary>
        public void LinkToMaster(TechPointViewModel master)
        {
            _masterTechPointViewModel = master;
            if (_masterTechPointViewModel.AssertIfNull(nameof(_masterTechPointViewModel)))
                return;

            Count = _masterTechPointViewModel.Count;
            IsBenefit = _masterTechPointViewModel.IsBenefit;
            IsSufficient = _masterTechPointViewModel.IsSufficient;
            IsEmpty = _masterTechPointViewModel.IsEmpty;
            _masterTechPointViewModel.PropertyChanged += OnMasterPropertyChanged;
        }

        public void UnlinkFromMaster()
        {
            if (_masterTechPointViewModel.AssertIfNull(nameof(_masterTechPointViewModel)))
                return;

            _masterTechPointViewModel.PropertyChanged -= OnMasterPropertyChanged;
            _masterTechPointViewModel = null;
        }

        protected override bool GetIsEmpty()
        {
            return Count == 0;
        }


        //=== Private =========================================================

        private void OnMasterPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_masterTechPointViewModel.AssertIfNull(nameof(_masterTechPointViewModel)))
                return;

            switch (args.PropertyName)
            {
                case nameof(Count):
                    Count = _masterTechPointViewModel.Count;
                    return;

                case nameof(IsSufficient):
                    IsSufficient = _masterTechPointViewModel.IsSufficient;
                    return;

                case nameof(IsBenefit):
                    IsBenefit = _masterTechPointViewModel.IsBenefit;
                    return;

                case nameof(IsEmpty):
                    IsEmpty = _masterTechPointViewModel.IsEmpty;
                    return;
            }
        }
    }
}