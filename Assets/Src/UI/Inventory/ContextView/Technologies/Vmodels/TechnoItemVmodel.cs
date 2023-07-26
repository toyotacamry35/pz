using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Core.Environment.Logging.Extension;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Building;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public class TechnoItemVmodel : BindingVmodel
    {
        private const int MaxDisplayedRecipesCount = 6;

        public readonly TechnologyItem TechnologyItem;

        public readonly TechnoTabVmodel TechnoTabVmodel;

        public CountRequirementsVmodel<CurrencyResource> CurrenciesRequirementsVmodel;

        public CountRequirementsVmodel<ScienceDef> SciencesRequirementsVmodel;


        //=== Props ===========================================================

        public CharacterStreamsData CharData { get; private set; }

        //Выбрана ли в табе технологий
        public ReactiveProperty<bool> IsSelectedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        /// <summary>
        /// Активирована ли (чистый признак)
        /// </summary>
        public ReactiveProperty<bool> IsActivatedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<bool> IsParentTechnologiesActivatedRp { get; } = new ReactiveProperty<bool>() {Value = true};

        /// <summary>
        /// Доступна ли активация по всем ресурсам и условиям (для неактивированных, без блокировки мутацией)
        /// </summary>
        public ReactiveProperty<bool> IsEnableToActivateByRequirementsRp { get; } = new ReactiveProperty<bool>() {Value = false};

        /// <summary>
        /// Соответствует ли по уровню аккаунта
        /// </summary>
        public ReactiveProperty<bool> DoesMeetLevelRequirementRp { get; } = new ReactiveProperty<bool>() {Value = true};

        public ReactiveProperty<LocalizedString> TechnologyNameRp { get; } = new ReactiveProperty<LocalizedString>() {Value = LsExtensions.Empty};

        public ReactiveProperty<LocalizedString> TechnologyDescriptionRp { get; } = new ReactiveProperty<LocalizedString>() {Value = LsExtensions.Empty};

        public ReactiveProperty<bool> IsBlockedByMutationRp { get; } = new ReactiveProperty<bool>() {Value = false};


        public ListStream<CraftRecipeDef> CraftRecipesListStream { get; } = new ListStream<CraftRecipeDef>();

        public ListStream<BuildRecipeDef> BuildRecipesListStream { get; } = new ListStream<BuildRecipeDef>();


        //=== Ctor ============================================================

        public TechnoItemVmodel(TechnologyItem technologyItem, TechnoTabVmodel technoTabVmodel, CharacterStreamsData charData)
        {
            CharData = charData;
            if (!technologyItem.Technology.IsValid)
            {
                UI.CallerLog($"Wrong {nameof(technologyItem)}");
                return;
            }

            TechnologyItem = technologyItem;
            TechnoTabVmodel = technoTabVmodel;
            technoTabVmodel.SelectedItemRp
                .Func(D, selected => selected == this)
                .Bind(D, IsSelectedRp);

            var requiredCosts = TechnologyItem.Technology.Target?.ActivateConditions.IsRequiredCurrency ?? false
                ? TechnologyItem.Technology.Target.ActivateConditions.Cost
                    .Where(data => data.TechPoint.IsValid)
                    .ToDictionary(data => data.TechPoint.Target, data => Mathf.Abs(data.Count))
                : new Dictionary<CurrencyResource, int>();

            CurrenciesRequirementsVmodel = new CountRequirementsVmodel<CurrencyResource>(
                requiredCosts,
                CharData.CharacterPoints?.CurrenciesStream,
                rvm => rvm.Resource?.SortingOrder ?? -1);

            var requiredSciences = TechnologyItem.Technology.Target?.ActivateConditions.Requirements.IsRequiredSciences ?? false
                ? TechnologyItem.Technology.Target.ActivateConditions.Requirements.Sciences
                    .Where(data => data.Science.IsValid)
                    .ToDictionary(data => data.Science.Target, data => data.Count)
                : new Dictionary<ScienceDef, int>();

            SciencesRequirementsVmodel = new CountRequirementsVmodel<ScienceDef>(
                requiredSciences,
                CharData.CharacterPoints?.SciencesStream,
                rvm => rvm.Resource?.SortingOrder ?? -1,
                CharData.CharacterPoints?.AvailableSciences); //словари всех имеющихся наук

            CharData.MutationStageStream.Subscribe(D,
                mutationStage => IsBlockedByMutationRp.Value = KnowledgeLogic.IsAllKnowledgesBlocked(TechnologyItem.Technology.Target, mutationStage),
                () => IsBlockedByMutationRp.Value = false);

            //--- Стримы имени и описания технологии
            var knownRecipesListStream = CharData.MutationStageStream.Func(D, mutStageDef =>
                KnowledgeLogic.GetRecipes(
                    KnowledgeLogic.GetKnownKnowledges(
                        Enumerable.Repeat(TechnologyItem.Technology.Target, 1), null, mutStageDef, false)));
            knownRecipesListStream.Action(D, recipes =>
            {
                var craftRecipes = recipes
                    .Select(recipe => recipe as CraftRecipeDef)
                    .Where(recipe => recipe != null)
                    .Take(MaxDisplayedRecipesCount)
                    .ToList();
                if (!craftRecipes.SequenceEqual(CraftRecipesListStream))
                {
                    CraftRecipesListStream.Clear();
                    craftRecipes.ForEach(recipe => CraftRecipesListStream.Add(recipe));
                }

                var buildRecipes = recipes
                    .Select(recipe => recipe as BuildRecipeDef)
                    .Where(recipe => recipe != null)
                    .ToList();
                if (!buildRecipes.SequenceEqual(BuildRecipesListStream))
                {
                    BuildRecipesListStream.Clear();
                    buildRecipes.ForEach(recipe => BuildRecipesListStream.Add(recipe));
                }
            });

            var firstKnowledgeStream = CharData.MutationStageStream.Func(D, mutStageDef => KnowledgeLogic.GetKnownKnowledges(
                Enumerable.Repeat(TechnologyItem.Technology.Target, 1), null, mutStageDef, false).FirstOrDefault());
            firstKnowledgeStream
                .Func(D, firstKnowledge => firstKnowledge?.NameLs ?? LsExtensions.Empty)
                .Bind(D, TechnologyNameRp);
            firstKnowledgeStream
                .Func(D, firstKnowledge => firstKnowledge?.DescriptionLs ?? LsExtensions.Empty)
                .Bind(D, TechnologyDescriptionRp);

            //--- И, наконец: может ли быть активирована технология по каким-либо условиям (если она еще не активирована)
            var isEnableToActivateStream = IsParentTechnologiesActivatedRp
                .Zip(D, CurrenciesRequirementsVmodel.IsEnoughRp)
                .Zip(D, SciencesRequirementsVmodel.IsEnoughRp)
                .Zip(D, DoesMeetLevelRequirementRp)
                .Func(D, (isParentTechnologiesActivated, isEnoughTechPoints, isEnoughSciences, doesMeetLevelRequirement) =>
                    isParentTechnologiesActivated && isEnoughTechPoints && isEnoughSciences && doesMeetLevelRequirement);
            isEnableToActivateStream.Bind(D, IsEnableToActivateByRequirementsRp);

            if (CharData.PawnSource != null)
            {
                CharData.PawnSource.KnownTechnologiesStream
                    .Func(D, knownTechnos => knownTechnos.Contains(TechnologyItem.Technology.Target))
                    .Bind(D, IsActivatedRp);
            }

            if (CharData.AccountLevelStream != null)
            {
                CharData.AccountLevelStream
                    .Func(D, lvl => lvl >= (TechnologyItem.Technology.Target?.ActivateConditions.Requirements.Level ?? 0))
                    .Bind(D, DoesMeetLevelRequirementRp);
            }
        }


        //=== Public ==========================================================

        public void AfterAllItemVmodelsCreated()
        {
            //--- Собираем стрим активности технологий, от которых зависит наша
            var technologyDef = TechnologyItem.Technology.Target;
            if (!technologyDef.ActivateConditions.Requirements.IsRequiredTechnologies)
                return;

            IStream<bool> isParentTechnologiesActivatedStream = null;
            foreach (var techDefRef in technologyDef.ActivateConditions.Requirements.Technologies)
            {
                if (!techDefRef.IsValid)
                {
                    UI.Logger.IfError()?.Message($"{nameof(techDefRef)} in {nameof(technologyDef.ActivateConditions)} isn't valid -- {this}").Write();
                    continue;
                }

                var technoItemVmodel = TechnoTabVmodel.ItemVmodels.FirstOrDefault(itemVmodel => itemVmodel.TechnologyItem.Technology == techDefRef);
                if (technoItemVmodel == null)
                {
                    UI.Logger.IfError()?.Message($"Not found {nameof(technoItemVmodel)} for {techDefRef.Target.____GetDebugShortName()} -- {this}").Write();
                    continue;
                }

                isParentTechnologiesActivatedStream = isParentTechnologiesActivatedStream == null
                    ? technoItemVmodel.IsActivatedRp
                    : isParentTechnologiesActivatedStream
                        .Zip(D, technoItemVmodel.IsActivatedRp)
                        .Func(D, (b1, b2) => b1 || b2);
            }

            //Если нет влияющих технологий, то rp будет содержать стартовое положительное значение
            isParentTechnologiesActivatedStream?.Bind(D, IsParentTechnologiesActivatedRp);
        }

        public override void Dispose()
        {
            base.Dispose();
            IsSelectedRp.Dispose();
            IsActivatedRp.Dispose();
            IsParentTechnologiesActivatedRp.Dispose();
            IsEnableToActivateByRequirementsRp.Dispose();
            TechnologyNameRp.Dispose();
            TechnologyDescriptionRp.Dispose();
            CharData = new CharacterStreamsData();
        }

        public string ToStringShort()
        {
            return $"[{nameof(TechnoItemVmodel)} {TechnologyItem.Technology.Target?.____GetDebugShortName()}]";
        }

        public override string ToString()
        {
            return $"[{nameof(TechnoItemVmodel)}: {TechnologyItem}, IsSelected{IsSelectedRp.Value.AsSign()}, IsActivated{IsActivatedRp.Value.AsSign()}, " +
                   $"IsEnableToActivate{IsEnableToActivateByRequirementsRp.Value.AsSign()} IsEnough[Costs{CurrenciesRequirementsVmodel.IsEnoughRp.Value.AsSign()} " +
                   $"Sciences{SciencesRequirementsVmodel.IsEnoughRp.Value.AsSign()} ParentTechs{IsParentTechnologiesActivatedRp.Value.AsSign()}]\n" +
                   $"TechnologyName={TechnologyNameRp.Value.GetText()}]";
        }

        public string ToDebug()
        {
            return $"{ToString()}\n{nameof(SciencesRequirementsVmodel)}={SciencesRequirementsVmodel},\n" +
                   $"{nameof(CurrenciesRequirementsVmodel)}={CurrenciesRequirementsVmodel}";
        }
    }
}