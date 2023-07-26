using System;
using System.ComponentModel;
using System.Text;
using Assets.Src.BuildingSystem;
using Assets.Src.Inventory;
using JetBrains.Annotations;
using L10n;
using SharedCode.Aspects.Building;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SelectionConstructionButton : ConstructionButton
    {
        [SerializeField, UsedImplicitly]
        private Sprite _normalBg;

        [SerializeField, UsedImplicitly]
        private Sprite _selectedBg;

        [SerializeField, UsedImplicitly]
        private Sprite _disabledBg;

        private ICharacterItemsNotifier _characterItemsNotifier;
        private ConstructionSelectionWindow _constructionSelectionWindow;


        //=== Props ===========================================================

        private ICharacterBuildInterface CharacterBuilderInterface => _constructionSelectionWindow.CharacterBuilderInterface;

        private BuildRecipeDef _buildRecipeDef;

        public BuildRecipeDef BuildRecipeDef
        {
            get => _buildRecipeDef;
            set
            {
                IsSelected = false;
                if (_buildRecipeDef != value)
                {
                    _buildRecipeDef = value;
                    HasDef = _buildRecipeDef != null;
                    Icon = _buildRecipeDef?.Icon?.Target;
                    BigIcon = _buildRecipeDef?.Image?.Target;
                    Title = _buildRecipeDef?.NameLs ?? LsExtensions.Empty;
                }
            }
        }

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSelected;

        [Binding]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnSelectionChanged();
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool IsEnoughIngreds => ProductMaxCount > 0;

        private int _productMaxCount;

        [Binding]
        public int ProductMaxCount
        {
            get => _productMaxCount;
            set
            {
                if (_productMaxCount != value)
                {
                    var oldIsEnoughIngreds = IsEnoughIngreds;
                    _productMaxCount = value;
                    NotifyPropertyChanged();
                    if (IsEnoughIngreds != oldIsEnoughIngreds)
                        NotifyPropertyChanged(nameof(IsEnoughIngreds));
                }
            }
        }

        private string _description = "";

        [Binding]
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _bgSprite;

        [Binding]
        public Sprite BgSprite
        {
            get => _bgSprite;
            set
            {
                if (_bgSprite != value)
                {
                    _bgSprite = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _bigIcon;

        [Binding]
        public Sprite BigIcon
        {
            get => _bigIcon;
            set
            {
                if (_bigIcon != value)
                {
                    var oldHasIcon = HasBigIcon;
                    _bigIcon = value;
                    NotifyPropertyChanged();
                    if (oldHasIcon != HasBigIcon)
                        NotifyPropertyChanged(nameof(HasBigIcon));
                }
            }
        }

        [Binding]
        public bool HasBigIcon => BigIcon != null;


        //=== Unity ===========================================================

        private void Awake()
        {
            _normalBg.AssertIfNull(nameof(_normalBg));
            _selectedBg.AssertIfNull(nameof(_selectedBg));
            _disabledBg.AssertIfNull(nameof(_disabledBg));
            BgSprite = GetBgSprite();
            PropertyChanged += OnSelfPropertyChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PropertyChanged -= OnSelfPropertyChanged;
        }


        //=== Public ==========================================================

        public void Init(int index, ICharacterItemsNotifier characterItemsNotifier, ConstructionSelectionWindow constructionSelectionWindow)
        {
            if (characterItemsNotifier.AssertIfNull(nameof(characterItemsNotifier)) ||
                constructionSelectionWindow.AssertIfNull(nameof(constructionSelectionWindow)))
                return;

            ButtonIndex = index;
            _characterItemsNotifier = characterItemsNotifier;
            _constructionSelectionWindow = constructionSelectionWindow;
            _constructionSelectionWindow.SelectedButtonChanged += OnSelectedButtonChanged;
        }

        public bool IsHotkeyFired()
        {
            var isFired = Hotkey.IsFired();
            if (isFired)
                _constructionSelectionWindow?.TakeSelection(this);
            return isFired;
        }

        public void UpdateDescription()
        {
            int productMaxCount;
            Description = GetDescription(out productMaxCount);
            ProductMaxCount = productMaxCount;
            if (IsSelected)
                CharacterBuilderInterface?.SetResourcesIsEnough(IsEnoughIngreds);
        }

        public override string ToString()
        {
            return $"<{nameof(SelectionConstructionButton)}> ({name}) '{Title}' {nameof(BuildRecipeDef)}={BuildRecipeDef}, " +
                   $"{nameof(IsInteractable)}{IsInteractable.AsSign()}, " +
                   $"{(IsSelected ? $"Selected, {nameof(IsEnoughIngreds)}{IsEnoughIngreds.AsSign()}" : "")}" +
                   $"{nameof(IsVisible)}{IsVisible.AsSign()}";
        }


        //=== Private =========================================================

        private void OnSelfPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "IsInteractable":
                case "IsSelectedRp":
                    BgSprite = GetBgSprite();
                    break;
            }
        }

        private Sprite GetBgSprite()
        {
            return IsInteractable ? (IsSelected ? _selectedBg : _normalBg) : _disabledBg;
        }

        private void OnSelectionChanged()
        {
            if (BuildRecipeDef.AssertIfNull(nameof(BuildRecipeDef)))
                return;

            UpdateDescription();

            if (IsSelected)
            {
                CharacterBuilderInterface?.Activate(true);
                CharacterBuilderInterface?.ActivatePlaceholder(BuildRecipeDef);
            }
        }

        private void OnSelectedButtonChanged(SelectionConstructionButton selectionConstructionButton)
        {
            IsSelected = selectionConstructionButton == this;
        }

        private string GetDescription(out int productMaxCount)
        {
            productMaxCount = Int32.MaxValue;
            if (!IsSelected)
            {
                productMaxCount = 0;
                return "";
            }

            var sb = new StringBuilder();
            if ((_buildRecipeDef?.Resource?.Resources?.Count ?? 0) == 0)
                return sb.ToString();

            foreach (var resource in _buildRecipeDef.Resource.Resources)
            {
                if (resource?.Item.Target == null)
                    continue;

                if (sb.Length > 0)
                    sb.AppendLine();

                var inventoryCount = _characterItemsNotifier.GetItemResourceCount(resource.Item.Target);
                var isEnough = inventoryCount >= resource.ClaimCount;
                productMaxCount = Mathf.Min(productMaxCount, inventoryCount / resource.ClaimCount);
                if (SharedCode.Utils.BuildUtils.BuildParamsDef.ClaimResources == false)
                    productMaxCount = Mathf.Max(productMaxCount, 1);
                var someTagBegin = isEnough
                    ? ConstructionSelectionWindow.EnoughColorTagBegin
                    : ConstructionSelectionWindow.NotEnoughColorTagBegin;
                sb.Append($"{resource.Item.Target.ItemNameLs.GetText()}: " +
                          $"{ConstructionSelectionWindow.NeutralColorTagBegin}{inventoryCount}{ConstructionSelectionWindow.ColorTagEnd}" +
                          $"/{someTagBegin}{resource.ClaimCount}{ConstructionSelectionWindow.ColorTagEnd}");
            }

            return sb.ToString();
        }
    }
}