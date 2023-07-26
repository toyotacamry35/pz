using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.ResourceSystem.Account;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using SharedCode.Wizardry;

namespace Assets.ResourceSystem.Account
{
    [Localized]
    public class LevelUpDatasDef : BaseResource
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetLogger("LevelUpDatasDef");

        public LevelUpDataPackDef[] DataList { get; set; } = Array.Empty<LevelUpDataPackDef>();


        // --- static getters: ------------------------------
        public IEnumerable<ResourceRef<SpellDef>> GetAllAchievementSpellsToCastByLvl(int lvl, SpellsGroup spellsGroup)
        {
            return DataList
                .Take(lvl)
                .Select(x =>
                    {
                        var achievementDef = x.RewardsPack.Target?.Achievement.Target;
                        if (achievementDef == null)
                            return Enumerable.Empty<ResourceRef<SpellDef>>();
                        
                        switch(spellsGroup)
                        {
                            case SpellsGroup.OnEnterWorld: return achievementDef.SpellsOnEnterWorld ?? Enumerable.Empty<ResourceRef<SpellDef>>();
                            case SpellsGroup.OnBirth:      return achievementDef.SpellsOnBirth      ?? Enumerable.Empty<ResourceRef<SpellDef>>();
                            case SpellsGroup.OnResurrect:  return achievementDef.SpellsOnResurrect  ?? Enumerable.Empty<ResourceRef<SpellDef>>();
                            default:
                                throw new ArgumentOutOfRangeException(nameof(spellsGroup), spellsGroup, null);
                        }
                    })
                .SelectMany(x => x);
        }

        public enum SpellsGroup
        {
            OnEnterWorld,
            OnBirth,
            OnResurrect
        }

        // --- Check: --------------------------------------------------
        public static bool Verify(LevelUpDatasDef def)
        {
            var list = def.DataList;
            bool wrongElementMet = false;
            for (int i = 1;  i < list.Length;  ++i)
            {
                // if (list[i].DeltaExpNeededToGetNextLevel <= list[i - 1].DeltaExpNeededToGetNextLevel)
                // {
                //     Logger.Error($"Wrong-filled {nameof(LevelUpDatasDef)} detected: " +
                //                  $"ExpNeededToGetNextLevel of [{i}] <= [{i - 1}] : {list[i].DeltaExpNeededToGetNextLevel} <= {list[i - 1].DeltaExpNeededToGetNextLevel}." +
                //                  $"Def: {def}.");
                //     return false;
                // }
                if (list[i].DeltaExpNeededToGetNextLevel == 0)
                    Logger.IfWarn()?.Message($"ExpNeededToGetNextLevel of [{i}] == 0 : {list[i].DeltaExpNeededToGetNextLevel}. Def: {def}. Is it intended?").Write();
                if (list[i].DeltaExpNeededToGetNextLevel < 0)
                {
                    Logger.Error($"Wrong-filled {nameof(LevelUpDatasDef)} detected: " +
                                 $"ExpNeededToGetNextLevel of [{i}] < 0 : {list[i].DeltaExpNeededToGetNextLevel}. Def: {def}. [Clamped to 0]");
                    list[i].DeltaExpNeededToGetNextLevel = 0;
                    wrongElementMet = true;
                }
            }
            if (!wrongElementMet)
                Logger.IfDebug()?.Message/*Error*/("LevelUpDatasDef.Verify succeed! +++++++++++++++++++++++++++++++++++++++++++++++")
                    .Write();
            else
                Logger.IfError()?.Message("LevelUpDatasDef.Verify failed! +++++++++++++++++++++++++++++++++++++++++++++++").Write();
            return !wrongElementMet;
        }

    }

    [Localized] ///#PZ-16219 Is attribute needed here ??
    public struct LevelUpDataPackDef
    {
        public ResourceRef<AccountLevelRewardsPackDef> RewardsPack;
        ///#TODO: плохая стр-ра д-х, т.к. возможна ошибка. - нужно заменить delta-до-след.ур-ня - тогда остануться только <0 знач-я, кот-е можно просто склэмпить об 0
        public int DeltaExpNeededToGetNextLevel;
    }

    //#Note: Аналогичный файл Миши назывался Assets/ResourceSystem/Player/LevelTableDef.cs
    // и его содержание было:
    /*
        using Assets.Src.ResourcesSystem.Base;
        using System;
        
        namespace ResourceSystem.Player
        {
            public struct PlayerLevel
            {
                public int Experience { get; set; }
        
                // TODO: Reward UI data goes here
            }
            public class LevelTableDef : BaseResource
            {
                public PlayerLevel[] PlayerLevels { get; set; } = Array.Empty<PlayerLevel>();
            }
        }
     */
}
