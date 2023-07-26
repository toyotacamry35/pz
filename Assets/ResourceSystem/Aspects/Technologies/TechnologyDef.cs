using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using EnumerableExtensions;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace SharedCode.Aspects.Science
{
    public class TechnologyDef : SaveableBaseResource
    {
        public ResourceRef<InventoryFiltrableTypeDef> InventoryFiltrableType { get; set; }
        public LevelUpgradeDef ActivateConditions { get; set; }
        public UnityRef<Sprite> BuildRecipesImage { get; set; }
        public bool IsImportant { get; set; }

        public IEnumerable<KeyValuePair<ScienceDef, int>> GetSciences(MutationStageDef stage, bool includeBlocked = true)
        {
            if (ActivateConditions.RewardKnowledges == null)
                return Enumerable.Empty<KeyValuePair<ScienceDef, int>>();

            return ActivateConditions.RewardKnowledges
                .Where(k => k.Target != null)
                .SelectMany(k => k.Target.GetSciences(stage, includeBlocked))
                .GroupBy(kvp => kvp.Key)
                .Select(g => new KeyValuePair<ScienceDef, int>(g.Key, g.Sum(kvp => kvp.Value)));
        }

        public IEnumerable<BaseRecipeDef> GetRecipes(MutationStageDef stage, bool includeBlocked = true)
        {
            if (ActivateConditions.RewardKnowledges == null)
                return Enumerable.Empty<BaseRecipeDef>();

            return ActivateConditions.RewardKnowledges
                .Where(k => k.Target != null)
                .SelectMany(k => k.Target.GetRecipes(stage, includeBlocked));
        }

        public override string ToString()
        {
            return $"[{nameof(TechnologyDef)}: '{____GetDebugRootName()}' {ActivateConditions}/td]";
        }

        public string ToStringShort(MutationStageDef stage = null)
        {
            var info = ActivateConditions.RewardKnowledges == null
                ? ""
                : " knowledges: " +
                  ActivateConditions.RewardKnowledges.Select(e => e.Target.ToStringShort(stage)).ItemsToString();
            return $"'{____GetDebugRootName()}'{info}";
        }
    }

    public struct LevelUpgradeDef
    {
        public LevelUpgradeRequirementsDef Requirements { get; set; }
        public TechPointCountDef[] Cost { get; set; }
        public ResourceRef<KnowledgeDef>[] RewardKnowledges { get; set; }

        public bool IsRequiredCurrency => Cost != null && Cost.Any(def => def.Count > 0 && def.TechPoint.IsValid);

        public override string ToString()
        {
            string costsInfo = Cost == null ? "" : ", costs: " + Cost.ItemsToString();
            string rewardKnowledges = RewardKnowledges == null
                ? ""
                : ", rewardKnowledges: " + RewardKnowledges.Select(rk => rk.Target).ItemsToString();
            return $"[{nameof(LevelUpgradeDef)}: Requirements: {Requirements}{costsInfo}{rewardKnowledges}/lud]";
        }
    }

    public struct LevelUpgradeRequirementsDef
    {
        public ScienceCountDef[] Sciences { get; set; }
        public ResourceRef<TechnologyDef>[] Technologies { get; set; }
        public int Level { get; set; }

        public bool IsRequiredSciences => Sciences != null && Sciences.Any(def => def.Count > 0 && def.Science.IsValid);

        public bool IsRequiredTechnologies => Technologies != null && Technologies.Any(def => def.IsValid);

        public IEnumerable<KeyValuePair<ScienceDef, int>> GetRequiredSciences()
        {
            if (!IsRequiredSciences)
                return Enumerable.Empty<KeyValuePair<ScienceDef, int>>();

            return Sciences
                .Where(scd => scd.Science.Target != null)
                .Select(scd => new KeyValuePair<ScienceDef, int>(scd.Science.Target, scd.Count));
        }

        public override string ToString()
        {
            string sciencesInfo = Sciences == null ? "" : ", sciences: " + Sciences.ItemsToString();
            string technosInfo = Technologies == null
                ? ""
                : ", parents: " + Technologies.Select(t => t.Target.____GetDebugRootName()).ItemsToString();
            return $"[{nameof(LevelUpgradeRequirementsDef)}: {sciencesInfo}{technosInfo}/lurd]";
        }
    }
}