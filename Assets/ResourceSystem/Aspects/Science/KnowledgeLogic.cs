using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using EnumerableExtensions;

namespace SharedCode.Aspects.Science
{
    public static class KnowledgeLogic
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("UI");

        public static int GetScienceCount(ScienceDef scienceDef, List<KnowledgeDef> knownKnowledges)
        {
            return knownKnowledges
                .Where(k => k.Sciences != null)
                .SelectMany(k => k.Sciences)
                .Where(sc => sc.Science.Target == scienceDef)
                .Sum(sc => sc.Count);
        }

        public static bool IsAllKnowledgesBlocked(TechnologyDef technologyDef, MutationStageDef currentStage)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)) ||
                technologyDef.ActivateConditions.RewardKnowledges == null)
                return false;

            return technologyDef.ActivateConditions.RewardKnowledges
                .All(k => k.Target != null && IsKnowledgeBlocked(k.Target, currentStage));
        }

        public static bool IsKnowledgeBlocked(KnowledgeDef knowledgeDef, MutationStageDef currentStage)
        {
            if (knowledgeDef.AssertIfNull(nameof(knowledgeDef)) || currentStage == null)
                return false;

            if (knowledgeDef.BlockedByMutationStages == null)
                return false;

            return knowledgeDef.BlockedByMutationStages.Select(sr => sr.Target).Any(s => s == currentStage);
        }

        public static List<KnowledgeDef> GetKnownKnowledges(IEnumerable<TechnologyDef> knownTechnologies,
            IEnumerable<KnowledgeDef> knownKnowledges, MutationStageDef currentStage, bool includeBlocked = true)
        {
            var resDefs =
                knownTechnologies?
                    .Where(t => t.ActivateConditions.RewardKnowledges != null)
                    .SelectMany(t => t.ActivateConditions.RewardKnowledges)
                    .Where(kr => kr.Target != null && (includeBlocked || !IsKnowledgeBlocked(kr.Target, currentStage)))
                    .Select(kr => kr.Target).ToList()
                ?? new List<KnowledgeDef>();

            knownKnowledges?.Where(k => k != null && (includeBlocked || !IsKnowledgeBlocked(k, currentStage)))
                .ForEach(k => resDefs.Add(k));

            return resDefs;
        }

        public static List<BaseRecipeDef> GetRecipes(ICollection<KnowledgeDef> knowledges)
        {
            return
                knowledges != null
                    ? knowledges
                        .Where(k => k?.Recipes != null)
                        .SelectMany(k => k.Recipes)
                        .Where(rr => rr.IsValid)
                        .Select(recipeRef => recipeRef.Target)
                        .Distinct()
                        .ToList()
                    : new List<BaseRecipeDef>();
        }
    }
}