using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ResourceSystem.Account;

namespace Assets.Src.Arithmetic
{
    public class CalcerPlayerLevel : ICalcerBinding<CalcerPlayerLevelDef, float>
    {
        public ValueTask<float> Calc(CalcerPlayerLevelDef def, CalcerContext ctx)
        {
            var character = ctx.TryGetEntity<IWorldCharacterClientFull>(ReplicationLevel.ClientFull);
            var exp = character.AccountStats.AccountExperience;

            if (def.LevelTable == null)
                return new ValueTask<float>(0.0f);

            return new ValueTask<float>(Calc(def, exp));
        }

        public static int Calc(CalcerPlayerLevelDef def, int exp)
        {
            return Calc(def.LevelTable.Target.DataList, exp);

            // ------------------
            int acc = 0;
            int level = 0;
            for (; ; )
            {
                if (level >= def.LevelTable.Target.DataList.Length)
                    return level + 1;
                acc += def.LevelTable.Target.DataList[level].DeltaExpNeededToGetNextLevel;
                if (exp < acc)
                    return level + 1;
                ++level;
            }
        }

        public static int Calc(LevelUpDataPackDef[] lvlUpDatas, int exp)
        {
            // #todo: may be opt.: (лениво 1ый раз собрав кэш хэшсета по expToLvalUp, по нему узнавать ответ), но пока не нужно - элементов всего 10 в листе - так что пофиг.
            int sum = 0;
            for (int i = 0;  i < lvlUpDatas.Length-1;  ++i)
            {
                sum += lvlUpDatas[i].DeltaExpNeededToGetNextLevel;
                if (exp < sum)
                    return i + 1;
            }
            //max lvl reached:
            return lvlUpDatas.Length;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerPlayerLevelDef def) => Array.Empty<StatResource>();
    }
}
