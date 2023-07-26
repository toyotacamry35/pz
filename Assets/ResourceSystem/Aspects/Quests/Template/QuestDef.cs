using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;
using System.Collections.Generic;
using System.Linq;
using L10n;
using UnityEngine;

namespace Assets.Src.Aspects.Impl.Factions.Template
{   [Localized]
    public class QuestDef : ProgressDef
    {
        public ResourceRef<PhaseDef>[] Phases { get; set; }
        public QuestGroup Group { get; set; }
        public QuestAfterCompleteAction AfterComplete { get; set; }
        public bool IsVisibleDyDefault { get; set; } = true;

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Group)}={Group}";
        }
    }

    public enum QuestGroup : byte
    {
        Main,
        Daily,
        Hidden
    }

    public enum QuestAfterCompleteAction : byte
    {
        Keep,
        Remove,
        Loop
    }

    [Localized]
    public class PhaseDef : ProgressDef
    {
        public ResourceRef<QuestCounterDef> Counter { get; set; }
        public ResourceRef<QuestCounterDef> FailCounter { get; set; }
        public string Label { get; set; }
        public string OnSuccessPhase { get; set; }
        public string OnFailPhase { get; set; }
        public bool IsFinalPhase { get; set; }
    }

    public class ProgressDef : SaveableBaseResource
    {
        public UnityRef<Sprite> Image { get; set; }

        public LocalizedString NameLs { get; set; }

        public LocalizedString ShortDescriptionLs { get; set; }

        public LocalizedString DescriptionLs { get; set; }

        public ResourceRef<SpellImpactDef>[] OnStart { get; set; }
        public ResourceRef<SpellImpactDef>[] OnEnd { get; set; }
        public ResourceRef<SpellImpactDef>[] OnSuccess { get; set; }
        public ResourceRef<SpellImpactDef>[] OnFail { get; set; }
    }

    public abstract class QuestCounterDef : SaveableBaseResource
    {
        public bool IsInvisible { get; set; }
    }

    public abstract class CounteredQuestCounterDef : QuestCounterDef
    {
        public int Count { get; set; }
        public ResourceRef<SpellImpactDef>[] OnEveryCounterChangeImpacts { get; set; }

    }

    public abstract class TargetedQuestCounterDef<T> : CounteredQuestCounterDef where T : class, IResource
    {
        public ResourceRef<T> Target { get; set; }
        public ResourceRef<T>[] Targets { get; set; }

        public bool HaveTargets => (Target != null && Target.IsValid) || (Targets != null && Targets.Length > 0);

        public IEnumerable<T> AllTargets => _allTargets ?? (_allTargets = GatherTargets().ToArray());
        
        private IEnumerable<T> GatherTargets()
        {
            if (Target != null && Target.IsValid)
                yield return Target.Target;
            else if (Targets != null && Targets.Length > 0)
                foreach (var target in Targets)
                    yield return target;
        }

        private T[] _allTargets;
    }
}