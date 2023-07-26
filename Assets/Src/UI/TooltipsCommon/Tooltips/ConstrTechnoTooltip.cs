using System;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Building;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Tooltips
{
    /// <summary>
    /// Тултип показа множества рецептов строительства для технологии
    /// </summary>
    [Binding]
    public class ConstrTechnoTooltip : BaseTooltip
    {
        [SerializeField, UsedImplicitly]
        private ConstrTechnoTooltipGroup _constrTechnoTooltipGroupPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _groupsRoot;

        private List<ConstrTechnoTooltipGroup> _tooltipGroups = new List<ConstrTechnoTooltipGroup>();


        //=== Props ===========================================================

        private LocalizedString _title;

        [Binding]
        public LocalizedString Title
        {
            get => _title;
            set
            {
                if (!_title.Equals(value))
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _description;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            set
            {
                if (!_description.Equals(value))
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _bgWidth;

        [Binding]
        public float BgWidth
        {
            get => _bgWidth;
            set
            {
                if (!Mathf.Approximately(_bgWidth, value))
                {
                    _bgWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
            _constrTechnoTooltipGroupPrefab.AssertIfNull(nameof(_constrTechnoTooltipGroupPrefab));
            _groupsRoot.AssertIfNull(nameof(_groupsRoot));
        }

        protected override void OnSetup(MonoBehaviour monoBehaviour)
        {
            var tooltipDescr = monoBehaviour as TechnosSideContrTooltipDescr;
            if (tooltipDescr.AssertIfNull(nameof(tooltipDescr)))
                return;

            Setup(tooltipDescr.Description as TechnosSideContr);
        }

        protected override void AfterLayoutRecalculated()
        {
            BgWidth = Size.x;
        }


        //=== Private =========================================================

        private void Setup(TechnosSideContr technosSideContr)
        {
            if (technosSideContr.AssertIfNull(nameof(technosSideContr)))
                return;

            Description = technosSideContr.Description;
            Title = technosSideContr.Title;
            SetupGroups(technosSideContr.BuildRecipesSubListStream);
        }

        private void SetupGroups(ListStream<BuildRecipeDef> buildRecipes)
        {
            try
            {
                var groups = buildRecipes.GroupBy(r => r.BuildRecipeGroupDef).OrderBy(g => g.Key.Target?.OrderIndex ?? 0);
                int i = 0;
                foreach (var group in groups)
                {
                    var tooltipGroup = GetTooltipGroup(i++);
                    tooltipGroup.Title = group.Key.Target.NameLs.GetText();
                    tooltipGroup.IsVisble = true;
                    tooltipGroup.Setup(group);
                }

                HideUnusedTooltipGroups(i);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"Exception: {e.Message}\n{e.StackTrace}").Write();
            }
        }

        private ConstrTechnoTooltipGroup GetTooltipGroup(int index)
        {
            ConstrTechnoTooltipGroup tooltipGroup;
            if (_tooltipGroups.Count <= index)
            {
                tooltipGroup = Instantiate(_constrTechnoTooltipGroupPrefab, _groupsRoot);
                tooltipGroup.name = $"{_constrTechnoTooltipGroupPrefab.name}{_tooltipGroups.Count}";
                _tooltipGroups.Add(tooltipGroup);
            }
            else
            {
                tooltipGroup = _tooltipGroups[index];
            }

            return tooltipGroup;
        }

        private void HideUnusedTooltipGroups(int firstUnusedIndex)
        {
            if (firstUnusedIndex >= _tooltipGroups.Count)
                return;

            for (int i = firstUnusedIndex; i < _tooltipGroups.Count; i++)
            {
                _tooltipGroups[i].IsVisble = false;
            }
        }
    }
}