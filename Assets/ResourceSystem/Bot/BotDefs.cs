using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Utils;
using ColonyShared.ManualDefsForSpells;
using JetBrains.Annotations;
using SharedCode.Wizardry;
using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.Aspects.Doings
{
    public class BotActionDef : BaseResource
    {
        public float StuckTime { get; [UsedImplicitly] set; } = -1;
    }

    public enum BotSelectTargetType
    {
        Near,
        Far,
        Random
    }
    public enum BotCompareOperationType
    {
        Equal,
        Greater,
        Lesser
    }
    public class BotSelectTargetDef : BotActionDef
    {
        public string ObjectName { get; set; }
        public float SearchRadius { get; set; } = 50;
        public bool SkipSelf { get; set; }
        public BotSelectTargetType BotSelectTargetType { get; set; } = BotSelectTargetType.Near;
        public ResourceRef<BotActionDef> Conditional { get; set; }
    }

    public class BotSelectRandomPointDef : BotActionDef
    {
        public float Radius { get; set; } = 50;
    }

    public class BotPinTargetPositionDef : BotActionDef
    {
    }

    public class BotLookToTargetDef : BotActionDef
    {
        public float TimeoutSeconds;//<=0 - infinity timeout
        public float UpdatePeriod { get; set; }
        public float SmoothTime { get; set; }
    }

    public class BotLookToPointDef : BotActionDef
    {
    }

    public class BotGoToTargetDef : BotActionDef
    {
        public float TimeoutSeconds;//<=0 - infinity timeout
        public float UpdatePeriod = 0.25f;
    }

    public class BotGoToPointDef : BotActionDef
    {
        public float TimeoutSeconds;//<=0 - infinity timeout
        public float UpdatePeriod = 0.25f;
        public bool FromKnowledge { get; set; } = true;
        public Vector3 Point { get; set; }
    }
    public class BotAddUsedSlotDef : BotActionDef
    {
        public ResourceRef<SlotDef> SlotDef { get; set; }
    }

    public class BotRemoveUsedSlotDef : BotActionDef
    {
        public ResourceRef<SlotDef> SlotDef { get; set; }
    }

    public class BotConsumeItemDef : BotActionDef
    {
        public ContainerType Container { get; set; }
        public int Slot { get; set; }
        public ResourceRef<SpellDef> Spell { get; set; }
    }

    public class BotAddOneOfItemsDef : BotActionDef
    {
        public string ItemNames { get; set; }
        public ContainerType Container { get; set; }
        public int Slot { get; set; } = -1;
        public int Count { get; set; } = 1;
    }

    public class BotMoveItemDef : BotActionDef
    {
        public ContainerType FromContainer { get; set; }
        public int FromSlot { get; set; }
        public ContainerType ToContainer { get; set; }
        public int ToSlot { get; set; }
        public int Count { get; set; } = 1;
    }
    public class BotTryEquipItemsDef : BotActionDef
    {
        public string ItemNames { get; set; }
    }

    public class BotWaitDef : BotActionDef
    {
        public float DurationSeconds { get; set; } = 0.0f;
    }
    public class BotWaitRandomDef : BotActionDef
    {
        public float DurationSecondsMin { get; set; } = 0.0f;
        public float DurationSecondsMax { get; set; } = 0.0f;
    }

    public class BotJumpDef : BotActionDef
    {
        public float AxisForward { get; set; } = 0;
        public float AxisLateral { get; set; } = 0;
    }

    public class BotDoInputActionsDef : BotActionDef
    {
        public ResourceRef<InputActionTriggerDef>[] InputActions { get; set; }
        public float DurationSeconds { get; set; } = 0.1f;
    }

    public class BotDoRepeatedDef : BotActionDef
    {
        public ResourceRef<BotActionDef> Action { get; set; }
        public float IntervalSeconds { get; set; } = 0.1f;
        public int Times { get; set; }
    }

    public class BotDoInParallelDef : BotActionDef
    {
        public bool WaitForAll { get; set; }
        public bool DoNotStopOnFail { get; set; }
        public ResourceRef<BotActionDef>[] Actions { get; set; }
    }

    public class BotDoInSequenceDef : BotActionDef
    {
        public bool Or { get; set; }
        public ResourceRef<BotActionDef>[] Actions { get; set; }
    }

    public class BotDoInLoopDef : BotActionDef
    {
        public int LoopCount { get; set; } = -1;
        public ResourceRef<BotActionDef>[] Actions { get; set; }
        public bool DoNotStopOnFail { get; set; }
    }

    public class BotDoRandomDef : BotActionDef
    {
        public ResourceRef<BotActionDef>[] Actions { get; set; }
    }

    public class BotDoRandomWeigthDef : BotActionDef
    {
        public ResourceRef<BotRandomWeigthDef>[] Actions { get; set; }
    }

    public class BotAddFlagDef : BotActionDef
    {
        public string FlagName { get; set; }
    }

    public class BotCheckFlagDef : BotActionDef
    {
        public string FlagName { get; set; }
    }

    public class BotRemoveFlagDef : BotActionDef
    {
        public string FlagName { get; set; }
    }

    public class BotSetTimestampDef : BotActionDef
    {
        public string Name { get; set; }
    }

    public class BotRemoveTimestampDef : BotActionDef
    {
        public string Name { get; set; }
    }

    public class BotIsElapsedTimestampDef : BotActionDef
    {
        public string Name { get; set; }

        public float Seconds { get; set; }
    }
    public class BotRandomWeigthDef : BaseResource
    {
        public float Weigth { get; set; }
        public ResourceRef<BotActionDef> Action { get; set; }
    }

    public class BotDoIf : BotActionDef
    {
        public ResourceRef<BotActionDef> If { get; set; }
        public ResourceRef<BotActionDef> Then { get; set; }
        public ResourceRef<BotActionDef> Else { get; set; }
    }

    public class BotDoWhileDef : BotActionDef
    {
        public ResourceRef<BotActionDef> While { get; set; }
        public ResourceRef<BotActionDef> Action { get; set; }
    }

    public class BotDropItemDef : BotActionDef
    {
        public ContainerType Container { get; set; }
        public int Slot { get; set; }
        public int Count { get; set; }
    }

    public class BotDumpStatsDef : BotActionDef { }

    public class BotCheckInteractiveDef : BotActionDef { }

    public class BotPingDef : BotActionDef
    {
        public double WarningTime { get; set; } = 500;
        public double InfoTime { get; set; } = 250;
    }

    public class BotCommitSuicideByJumpingFromHeightDef : BotActionDef
    {
        public long LiftingTime { get; set; } = 4;
        public float LiftingSpeed { get; set; } = 10;
    }

    public class BotCheckItemsInContainerDef : BotActionDef
    {
        public string ItemNames { get; set; }
        public ContainerType Container { get; set; }
        public int Count { get; set; }
    }

    public class BotCheckEmptyInventorySlotsDef : BotActionDef
    {
        public int Count { get; set; }
    }

    public class BotCheckEmptyDollSlotDef : BotActionDef
    {
        public ResourceRef<SlotDef> SlotDef { get; set; }
    }

    public class BotCheckMutationDef : BotActionDef
    {
        public bool OnTarget { get; set; }
        public ResourceRef<MutationStageDef> Mutation { get; set; }
    }

    public class BotTargetIsSameMutationDef : BotActionDef
    {
    }
    public class BotNotDef : BotActionDef
    {
        public ResourceRef<BotActionDef> Action { get; set; }
    }

    public class BotCheckTargetDistanceDef : BotActionDef
    {
        public float DistanceMeters { get; set; }
    }

    public class BotTargetIsAliveDef : BotActionDef
    {
    }

    public class BotDropTargetDef : BotActionDef
    {
    }

    public class BotCompareStatWithTargetStatDef : BotActionDef
    {
        public BotCompareOperationType Operation { get; set; } = BotCompareOperationType.Equal;

        public float DiffValue { get; set; }

        public ResourceRef<StatResource> StatResource { get; set; }
    }

    public class BotCheckStatDef : BotActionDef
    {
        public bool OnTarget { get; set; }

        public BotCompareOperationType Operation { get; set; } = BotCompareOperationType.Equal;

        public float Value { get; set; }

        public ResourceRef<StatResource> StatResource { get; set; }
    }

    public class BotCheckStatPercentDef : BotActionDef
    {
        public bool OnTarget { get; set; }

        public BotCompareOperationType Operation { get; set; } = BotCompareOperationType.Equal;

        public float Percent { get; set; }

        public ResourceRef<StatResource> StatResource { get; set; }

        public ResourceRef<StatResource> MaxStatResource { get; set; }
    }

    public class BotCastSpellDef : BotActionDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellDef>[] Spells { get; set; }
    }
}
