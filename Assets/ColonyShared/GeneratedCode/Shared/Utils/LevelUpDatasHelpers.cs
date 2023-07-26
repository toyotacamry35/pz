using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.ResourceSystem.Account;
using Assets.Src.Arithmetic;
using EnumerableExtensions;
using JetBrains.Annotations;
using MongoDB.Bson;
using NLog;

namespace Assets.ColonyShared.GeneratedCode.Shared.Utils
{
    public static class LevelUpDatasHelpers
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static List<AccountLevelRewardsPackDef> _levelAchievementsList;
        //private static List<LevelUpDataPackDef> _lvlUpDatas = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.DataList;
        private static LevelUpDataPackDef[] _lvlUpDatas = GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.DataList;
        public static List<AccountLevelRewardsPackDef> LevelAchievementsList
        {
            get
            {
                return _levelAchievementsList 
                       ?? (_levelAchievementsList = _lvlUpDatas
                           .Select(x => x.RewardsPack.Target)
                           .ToList());
            }
        }

        private static int? _maxAllowedExperience;
        public static int MaxAllowedExperience
        {
            get
            {
                if (!_maxAllowedExperience.HasValue)
                    _maxAllowedExperience = _lvlUpDatas.Sum(x => x.DeltaExpNeededToGetNextLevel);
                return _maxAllowedExperience.Value;
            }
        }

        public static int MaxAllowedUnconsumedExp(int exp)
        {
            return MaxAllowedExperience - exp;
        }

        public static int MaxLevel => _lvlUpDatas.Length;


        //#!!Important!!:  Should be synced with `CalcerPlayerLevel.Calc` !
        public static int CalcAccLevel(int totalExp)
        // {
        //     // #todo: may be opt.: (лениво 1ый раз собрав кэш хэшсета по expToLvalUp, по нему узнавать ответ), но пока не нужно - элементов всего 10 в листе - так что пофиг.
        //     for (int i = 0;  i < _lvlUpDatas.Length;  ++i)
        //         if (totalExp < _lvlUpDatas[i].DeltaExpNeededToGetNextLevel)
        //             return i;
        //     //max lvl reached:
        //     return _lvlUpDatas.Length-1;
        // }
        => CalcerPlayerLevel.Calc(_lvlUpDatas, totalExp);

        public static AccountLevelRewardsPackDef GetCurrLvlRewardsPack(int consumedExp)
        {
            var lvl = CalcAccLevel(consumedExp);
            return _lvlUpDatas[lvl-1].RewardsPack;
        }

        public static int NextLvlUpExpThreshold(int totalExp)
        {
            var lvl = CalcAccLevel(totalExp);
            if (lvl > _lvlUpDatas.Length-1+1)
            {
                Logger.Warn("lvl > _lvlUpDatas.Length. Unexpected request!" +
                            $"\n// lvl:{lvl}, Length:{_lvlUpDatas.Length}.");
                return 0;
            }

            return CalcTotalExpNeededToReachLvl(lvl+1);
        }

        public static int GetDeltaExpNeededToGetNextLevel(int currLvl)
        {
            if (currLvl > _lvlUpDatas.Length)
            {
                Logger.Warn("lvl > _lvlUpDatas.Length. Unexpected request!" +
                            $"\n// lvl:{currLvl}, Length:{_lvlUpDatas.Length}.");
                return 0;
            }

            return _lvlUpDatas[currLvl-1].DeltaExpNeededToGetNextLevel;
        }

        //public static int GetDeltaExpToConsumeByChosenDef(AccountLevelRewardsPackDef def, int currConsumedExp)
        public static int CalcTotalExpNeededToReachLvl(int lvl)
        {
            if (lvl > _lvlUpDatas.Length + 1)
                Logger.Warn("Asked lvl > than ((max described in LevelUpDatasDef.jdb) + 1). Unexpected request!" +
                            $"\n// lvl:{lvl}, Length+1:{_lvlUpDatas.Length + 1}.");
            return _lvlUpDatas
                .Take(lvl-1)
                .Sum(x => x.DeltaExpNeededToGetNextLevel);
        }
        /// <summary>
        /// Сколько опыта нужно добрать к заданному `currExp`, чтобы достичь уровня `lvlTo`
        /// </summary>
        public static int CalcDeltaExpFromCurrExpToLvl(int currExp, int lvlTo)
        {
            if (currExp < 0 || lvlTo < 0)
            {
                Logger.Error($"Broken input: currExp:{currExp}, lvlTo:{lvlTo}.");
                return 0;
            }
            if (lvlTo > _lvlUpDatas.Length + 1)
                Logger.Warn("Asked lvlTo > than ((max described in LevelUpDatasDef.jdb) + 1). Unexpected request!" +
                            $"\n// lvlTo:{lvlTo}, Length+1:{_lvlUpDatas.Length + 1}.");
            return Math.Max(0, CalcTotalExpNeededToReachLvl(lvlTo) - currExp);
        }

        public static int LevelByDef(AccountLevelRewardsPackDef def)
        {
            var indx = LevelAchievementsList.IndexOf(def);
            if (indx >= 0)
                return indx + 1;

            Logger.Error($"LevelAchievementsList doesn't contain elem: {def}." +
                         $"\nLevelAchievementsList:\n{LevelAchievementsList.ItemsToStringByLines()}.");
            return -1;
        }


    }
}
