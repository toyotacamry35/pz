using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ResourceSystem.Account;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using SharedCode.Wizardry;

namespace Assets.ColonyShared.SharedCode.Utils
{
    [Localized] //for `LocalizationKeysDefsHolderDef`
    public class GlobalConstsDef : BaseResource
    {
        public ResourceRef<DamageTypeDef> DefaultDamageType       { get; set; }
        public ResourceRef<WeaponSizeDef> DefaultWeaponSize       { get; set; }
        public ResourceRef<AttackTypeDef> DefaultAttackType       { get; set; }
        public ResourceRef<HitMaterialDef> DefaultHitMaterial       { get; set; }
        public ResourceRef<SpellDef> StopHealthRegenerationSpell  { get; set; }
        public ResourceRef<BaseResource> ItemsStatsAccumulatorKey { get; set; }
        public ResourceRef<BaseResource> PreDeathHandlerKey       { get; set; }
        public ResourceRef<AnimationParametersDef> AnimationParameters { get; set; }
        public ResourceRef<LocalizationKeysDefsHolderDef> LocalizationKeysDefsHolder { get; set; }
        public float[] DurabilityLevels     { get; set; }
        public float VisibilityDistance     { get; set; }
        public float UnstuckTeleportTimeout { get; set; }
        public float MaxMiningDistance      { get; set; }
        public ResourceRef<LevelUpDatasDef> LevelUpDatas { get; set; }

        // --- Dbg: ------------------------------------------------------

        // leave `false` to use unsynced
        public bool UseSyncedTimestamperAtDebugCollector { private get; set; }
        public bool DbgLogEnabled                 { private get; set; }
        // Mobs:
        public bool DebugMobs                     { private get; set; }
        public bool DebugMobsHard_DANGER          { private get; set; }
        public bool DebugMobs_OffClLocoInHostMode { get; set; }
        // Locomotion:
        public bool DisableLocoClPosDamper        { private get; set; }
        // Фиксированный dt для отображения скорости на графике (в clip asset'е) в виде дельтапозиции отн-но позиции в тек.кадре.
        public float LocoPosLogAgentFixedDtToCalcVeloAsDltPos { get; set; }
        public int   CurveTangentMode             { get; set; } // values see at `AnimationUtility.TangentMode` enum. (dflt val. is 0)

        // Spells:
        public bool DebugSpells                   { private get; set; }

        // --- Profiler: -------------------------------------------------
        public bool EnableLocomotionProfiler  { private get; set; }
        public bool EnableLocomotionProfiler2 { private get; set; }
        public bool EnableLocomotionProfiler3 { private get; set; }
        public bool EnableLocomotionProfiler4 { private get; set; }

        // --- Tmp: ------------------------------------------------------
        public float Tmp_Dbg_RelevancyBordersMid { get; set; } // 0f == 14.99f //#todo: Place this setting in right mob-dependent place (when it'll become needed to be different)


        #region PZ-13568:

        public bool PZ_13761_EnableLogZoneBorders { private get; set; }
        //public Vector2 PZ_13761_LogZoneBorderMin { get; set; }
        public float PZ_13761_LogZoneBorderMin_X { get; set; }
        public float PZ_13761_LogZoneBorderMin_Y { get; set; }
        //public Vector2 PZ_13761_LogZoneBorderMax { get; set; }
        public float PZ_13761_LogZoneBorderMax_X { get; set; }
        public float PZ_13761_LogZoneBorderMax_Y { get; set; }

        #endregion PZ-13568:



        // #Tmp Hotfix. Good solution 'll be done at PZ-17523
        public static class DebugFlagsGetter
        {
            private static bool IsDebug => Consts.IsDebug;
            public static bool IsUseSyncedTimestamperAtDebugCollector(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.UseSyncedTimestamperAtDebugCollector;
            }
            public static bool IsDbgLogEnabled(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.DbgLogEnabled;
            }
            public static bool IsDebugMobs(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.DebugMobs;
            }
            public static bool IsDebugMobsHard_DANGER(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.DebugMobsHard_DANGER;
            }
            public static bool IsDisableLocoClPosDamper(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.DisableLocoClPosDamper;
            }
            public static bool IsDebugSpells(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.DebugSpells;
            }
            public static bool IsEnableLocomotionProfiler(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.EnableLocomotionProfiler;
            }
            public static bool IsEnableLocomotionProfiler2(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.EnableLocomotionProfiler2;
            }
            public static bool IsEnableLocomotionProfiler3(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.EnableLocomotionProfiler3;
            }
            public static bool IsEnableLocomotionProfiler4(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.EnableLocomotionProfiler4;
            }
            public static bool IsPZ_13761_EnableLogZoneBorders(GlobalConstsDef globConstsDef)
            {
                return (!IsDebug) ? false : globConstsDef.PZ_13761_EnableLogZoneBorders;
            }
        }
    }

}
