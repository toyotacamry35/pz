using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using EnumerableExtensions;
using L10n;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace SharedCode.Aspects.Science
{
    [Localized]
    public class KnowledgeDef : SaveableBaseResource, IManyRewardsSource
    {
        public LocalizedString NameLs { get; set; }

        public LocalizedString DescriptionLs { get; set; }

        public UnityRef<Sprite> BlueprintIcon { get; set; }

        public ScienceCountDef[] Sciences { get; set; }
        public ResourceRef<BaseRecipeDef>[] Recipes { get; set; }
        public ResourceRef<KnowledgeRecordDef>[] KnowledgeRecords { get; set; }

        public ResourceRef<MutationStageDef>[] BlockedByMutationStages { get; set; }

        public IEnumerable<KeyValuePair<ScienceDef, int>> GetSciences(MutationStageDef stage, bool includeBlocked = true)
        {
            if (Sciences == null || !includeBlocked && KnowledgeLogic.IsKnowledgeBlocked(this, stage))
                return Enumerable.Empty<KeyValuePair<ScienceDef, int>>();

            return Sciences
                .Where(s => s.Science.Target != null)
                .Select(s => new KeyValuePair<ScienceDef, int>(s.Science.Target, s.Count));
        }

        public IEnumerable<BaseRecipeDef> GetRecipes(MutationStageDef stage, bool includeBlocked = true)
        {
            if (Recipes == null || !includeBlocked && KnowledgeLogic.IsKnowledgeBlocked(this, stage))
                return Enumerable.Empty<BaseRecipeDef>();

            return Recipes
                .Where(r => r.Target != null)
                .Select(r => r.Target);
        }

        public override string ToString()
        {
            string sciencesInfo = Sciences == null
                ? ""
                : ", sciences: " + Sciences.ItemsToString();
            
            string recipesInfo = Recipes == null ? "" : ", recipes: " + Recipes.Select(r => r.Target.____GetDebugRootName()).ItemsToString();
            string blockStagesInfo = BlockedByMutationStages == null
                ? ""
                : ", blocked by: " + BlockedByMutationStages.Select(m => m.Target.____GetDebugRootName()).ItemsToString();
            string kRecordsInfo = KnowledgeRecords == null
                ? ""
                : ", k.records: " + KnowledgeRecords.Select(r => r.Target).ItemsToString();
            return $"[{nameof(KnowledgeDef)} '{____GetDebugRootName()}' {blockStagesInfo}{sciencesInfo}{recipesInfo}{kRecordsInfo}/kd]";
        }

        public string ToStringShort(MutationStageDef stage)
        {
            return ____GetDebugRootName() + (KnowledgeLogic.IsKnowledgeBlocked(this, stage) ? " BLOCKED" : "");
        }


        public IRewardSource[] Rewards => GetRewards();

        private IRewardSource[] GetRewards()
        {
            var rewards = new List<IRewardSource>();
            if (Sciences != null)
                foreach (var scienceCountDef in Sciences)
                    if (scienceCountDef.Science.Target != null)
                        rewards.Add(scienceCountDef);

            if (Recipes != null)
                foreach (var baseRecipeDef in Recipes)
                    if (baseRecipeDef.Target != null)
                        rewards.Add(baseRecipeDef.Target);

            return rewards.ToArray();
        }
    }
}