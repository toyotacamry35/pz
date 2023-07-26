using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Aspects.Impl.Factions.Template;
using ResourcesSystem.Loader;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Science;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class KnowledgeEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("UI");


        //=== Public ==========================================================

        public async Task<TechnologyOperationResult> ExploreImpl(KnowledgeDef knowledgeDef, TechPointCountDef[] rewardPoints)
        {
            if (rewardPoints == null || rewardPoints.Length == 0)
            {
                var exploringBonusesDef =
                    GameResourcesHolder.Instance.LoadResource<ExploringBonusesDef>("/Inventory/TechPoints/ExploringBonuses");
                rewardPoints = exploringBonusesDef?.Bonuses;
            }

            if (rewardPoints != null)
                await ChangeRPointsImpl(rewardPoints, true);

            return await AddKnowledgeWork(knowledgeDef);
        }


        public async Task<TechnologyOperationResult> AddKnowledgeImpl(KnowledgeDef knowledgeDef)
        {
            return await AddKnowledgeWork(knowledgeDef);
        }

        public async Task AddTechnologyImpl(TechnologyDef technologyDef)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return;

            if (KnownTechnologies.Any(def => def == technologyDef))
                return;

            KnownTechnologies.Add(technologyDef);

            if (OwnerInformation.Owner.IsValid)
                using (var wrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                {
                    var hasStatistics = wrapper.Get<IHasStatisticsServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                    if (hasStatistics != null)
                        foreach (var knowledgeDef in technologyDef.ActivateConditions.RewardKnowledges)
                            await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.KnowledgeEventArgs() {Knowledge = knowledgeDef});
                }
        }

        public async Task<TechnologyOperationResult> TryToActivateImpl(TechnologyDef technologyDef, bool doActivate)
        {
            MutationStageDef stage;
            int level;
            using (var worldCharWrapper = await EntitiesRepository.Get<IWorldCharacterClientFull>(Id))
            {
                if (worldCharWrapper.AssertIfNull(nameof(worldCharWrapper)))
                    return TechnologyOperationResult.ErrorNullValue;

                var worldCharacterCf = worldCharWrapper.Get<IWorldCharacterClientFull>(Id);
                if (worldCharacterCf.AssertIfNull(nameof(worldCharacterCf)))
                    return TechnologyOperationResult.ErrorNullValue;

                stage = worldCharacterCf.MutationMechanics.Stage;
                level = LevelUpDatasHelpers.CalcAccLevel(worldCharacterCf.AccountStats.AccountExperience);
            }

            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return TechnologyOperationResult.ErrorNullValue;

            if (KnownTechnologies.Any(def => def == technologyDef))
                return TechnologyOperationResult.ErrorAlreadyKnown;

            if (KnowledgeLogic.IsAllKnowledgesBlocked(technologyDef, stage))
                return TechnologyOperationResult.ErrorNotMatchRequirements;

            var meetRequirementsResult = DoesMeetRequirements_Sciences(stage, technologyDef);
            if (meetRequirementsResult != TechnologyOperationResult.Success)
                return meetRequirementsResult;

            meetRequirementsResult = DoesMeetRequirements_AccountLevel(level, technologyDef);
            if (meetRequirementsResult != TechnologyOperationResult.Success)
                return meetRequirementsResult;

            meetRequirementsResult = DoesMeetRequirements_Technologies(technologyDef);
            if (meetRequirementsResult != TechnologyOperationResult.Success)
                return meetRequirementsResult;

            meetRequirementsResult = await DoesMeetRequirements_Costs(technologyDef);
            if (meetRequirementsResult != TechnologyOperationResult.Success)
                return meetRequirementsResult;

            if (!doActivate)
                return TechnologyOperationResult.Success;

            //Отвечает требованиям, активируем
            if (technologyDef.ActivateConditions.Cost != null)
                await ChangeRPointsImpl(technologyDef.ActivateConditions.Cost, false);

            KnownTechnologies.Add(technologyDef);

            if (OwnerInformation.Owner.IsValid)
                using (var wrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                {
                    var hasStatistics = wrapper.Get<IHasStatisticsServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                    if (hasStatistics != null)
                        foreach (var knowledgeDef in technologyDef.ActivateConditions.RewardKnowledges)
                            await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.KnowledgeEventArgs() {Knowledge = knowledgeDef});
                }

            return TechnologyOperationResult.Success;
        }

        /// <summary>
        /// Отрицательный Count в techPointCounts - отнимает, положительный - добавляет
        /// </summary>
        public async Task<TechnologyOperationResult> ChangeRPointsImpl(TechPointCountDef[] techPointCounts, bool isIncrement)
        {
            if (techPointCounts.AssertIfNull(nameof(techPointCounts)))
                return TechnologyOperationResult.ErrorNullValue;

            if (!await CanChangeRPointsImpl(techPointCounts, isIncrement))
                return TechnologyOperationResult.ErrorNotEnoughtResources;

            ContainerItemOperationResult result;
            using (var worldCharWrapper = await EntitiesRepository.Get<IWorldCharacterServer>(Id))
            {
                if (worldCharWrapper.AssertIfNull(nameof(worldCharWrapper)))
                    return TechnologyOperationResult.ErrorNullValue;

                var worldCharacter = worldCharWrapper.Get<IWorldCharacterServer>(Id);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return TechnologyOperationResult.ErrorNullValue;

                var currencyToChange = techPointCounts
                    .Select(v => new CurrencyResourcePack(v.TechPoint.Target, v.Count * (isIncrement ? 1 : -1))).ToList();

                result = await worldCharacter.ChangeCurrencies(currencyToChange);
            }

            return result.IsSuccess() ? TechnologyOperationResult.Success : TechnologyOperationResult.Error;
        }

        /// <summary>
        /// Отрицательный Count в techPointCounts - отнимает, положительный - добавляет
        /// </summary>
        public async Task<bool> CanChangeRPointsImpl(TechPointCountDef[] techPointCounts, bool isIncrement)
        {
            if (techPointCounts == null)
                return true;

            using (var worldCharWrapper = await EntitiesRepository.Get<IWorldCharacterClientFull>(Id))
            {
                if (worldCharWrapper.AssertIfNull(nameof(worldCharWrapper)))
                    return false;

                var worldCharacter = worldCharWrapper.Get<IWorldCharacterClientFull>(Id);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return false;


                foreach (var techPointCountDef in techPointCounts)
                {
                    if (techPointCountDef.TechPoint.Target.AssertIfNull(nameof(techPointCountDef.TechPoint)))
                        continue;

                    if (!isIncrement)
                    {
                        var hasCurrencies = await worldCharacter.GetCurrencyValue(techPointCountDef.TechPoint);
                        var requiredCurrencies = techPointCountDef.Count * (isIncrement ? 1 : -1);
                        var afterOperationCurrencies = hasCurrencies + requiredCurrencies;
                        if (afterOperationCurrencies < 0)
                            return false;
                    }
                }
            }

            return true;
        }

        public async Task<TechnologyOperationResult> AddShownKnowledgeRecordImpl(KnowledgeRecordDef knowledgeRecordDef)
        {
            if (knowledgeRecordDef.AssertIfNull(nameof(knowledgeRecordDef)))
                return TechnologyOperationResult.ErrorNullValue;

            if (ShownKnowledgeRecords.Contains(knowledgeRecordDef))
                return TechnologyOperationResult.ErrorAlreadyKnown;

            ShownKnowledgeRecords.Add(knowledgeRecordDef);
            return TechnologyOperationResult.Success;
        }

        public int GetScienceCount(MutationStageDef stage, ScienceDef scienceDef)
        {
            return KnowledgeLogic.GetScienceCount(scienceDef, GetKnownKnowledges(stage, false));
        }

        public List<KnowledgeDef> GetKnownKnowledges(MutationStageDef stage, bool includeBlocked = true)
        {
            return KnowledgeLogic.GetKnownKnowledges(KnownTechnologies.ToList(), KnownKnowledges.ToList(), stage, includeBlocked);
        }


        //=== Private =========================================================

        private TechnologyOperationResult DoesMeetRequirements_AccountLevel(int accountLevel, TechnologyDef technologyDef)
        {
            return accountLevel >= technologyDef.ActivateConditions.Requirements.Level
                ? TechnologyOperationResult.Success
                : TechnologyOperationResult.ErrorNotMatchRequirements;
        }

        private TechnologyOperationResult DoesMeetRequirements_Sciences(MutationStageDef stage, TechnologyDef technologyDef)
        {
            var requirements = technologyDef.ActivateConditions.Requirements;
            if (requirements.IsRequiredSciences)
            {
                foreach (var reqScienceCountDef in requirements.Sciences)
                {
                    var reqScienceDef = reqScienceCountDef.Science.Target;
                    if (reqScienceDef == null)
                        continue;

                    if (GetScienceCount(stage, reqScienceDef) < reqScienceCountDef.Count)
                        return TechnologyOperationResult.ErrorNotMatchRequirements;
                }
            }

            return TechnologyOperationResult.Success;
        }

        private TechnologyOperationResult DoesMeetRequirements_Technologies(TechnologyDef technologyDef)
        {
            //Возвращает успех, если нет требований по технологиям или известна хотя бы одна требуемая
            var requirements = technologyDef.ActivateConditions.Requirements;
            if (requirements.IsRequiredTechnologies &&
                !requirements.Technologies.Any(reqTechRef => KnownTechnologies.Any(knownTech => knownTech == reqTechRef.Target)))
                return TechnologyOperationResult.ErrorNotMatchRequirements;

            return TechnologyOperationResult.Success;
        }

        private async Task<TechnologyOperationResult> DoesMeetRequirements_Costs(TechnologyDef technologyDef)
        {
            return await CanChangeRPointsImpl(technologyDef.ActivateConditions.Cost, false)
                ? TechnologyOperationResult.Success
                : TechnologyOperationResult.ErrorNotEnoughtResources;
        }


        private async Task<TechnologyOperationResult> AddKnowledgeWork(KnowledgeDef knowledgeDef)
        {
            if (knowledgeDef.AssertIfNull(nameof(knowledgeDef)))
                return TechnologyOperationResult.ErrorNullValue;

            if (KnownKnowledges.Any(def => def == knowledgeDef))
                return TechnologyOperationResult.ErrorAlreadyKnown;

            KnownKnowledges.Add(knowledgeDef);

            if (OwnerInformation.Owner.IsValid)
                using (var wrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                {
                    var hasStatistics = wrapper.Get<IHasStatisticsServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                    if (hasStatistics != null)
                        await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.KnowledgeEventArgs() {Knowledge = knowledgeDef});
                }

            return TechnologyOperationResult.Success;
        }
    }
}