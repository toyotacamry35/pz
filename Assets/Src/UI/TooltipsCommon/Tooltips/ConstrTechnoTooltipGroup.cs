using System.Collections.Generic;
using JetBrains.Annotations;
using SharedCode.Aspects.Building;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Tooltips
{
    [Binding]
    public class ConstrTechnoTooltipGroup : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private BuildRecipeSimpleViewModel _buildRecipeSimpleViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _buildRecipesRoot;

        private List<BuildRecipeSimpleViewModel> _recipeSimpleViewModels = new List<BuildRecipeSimpleViewModel>();


        //=== Props ===========================================================

        private string _title;

        [Binding]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isVisble;

        [Binding]
        public bool IsVisble //reusable
        {
            get { return _isVisble; }
            set
            {
                if (_isVisble != value)
                {
                    _isVisble = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _buildRecipeSimpleViewModelPrefab.AssertIfNull(nameof(_buildRecipeSimpleViewModelPrefab));
            _buildRecipesRoot.AssertIfNull(nameof(_buildRecipesRoot));
        }


        //=== Public ==========================================================

        public void Setup(IEnumerable<BuildRecipeDef> buildRecipeDefs)
        {
            int i = 0;
            if (!buildRecipeDefs.AssertIfNull(nameof(buildRecipeDefs)))
            {
                foreach (var buildRecipeDef in buildRecipeDefs)
                {
                    var recipeViewModel = GetRecipeViewModel(i++);
                    recipeViewModel.ProductSprite = buildRecipeDef.Icon.Target;
                    recipeViewModel.IsVisble = true;
                }
            }

            HideUnusedViewModels(i);
        }


        //=== Private =========================================================

        private BuildRecipeSimpleViewModel GetRecipeViewModel(int index)
        {
            BuildRecipeSimpleViewModel tooltipGroup;
            if (_recipeSimpleViewModels.Count <= index)
            {
                tooltipGroup = Instantiate(_buildRecipeSimpleViewModelPrefab, _buildRecipesRoot);
                tooltipGroup.name = $"{_buildRecipeSimpleViewModelPrefab.name}{_recipeSimpleViewModels.Count}";
                _recipeSimpleViewModels.Add(tooltipGroup);
            }
            else
            {
                tooltipGroup = _recipeSimpleViewModels[index];
            }

            return tooltipGroup;
        }

        private void HideUnusedViewModels(int firstUnusedIndex)
        {
            if (firstUnusedIndex >= _recipeSimpleViewModels.Count)
                return;

            for (int i = firstUnusedIndex; i < _recipeSimpleViewModels.Count; i++)
            {
                _recipeSimpleViewModels[i].IsVisble = false;
            }
        }
    }
}