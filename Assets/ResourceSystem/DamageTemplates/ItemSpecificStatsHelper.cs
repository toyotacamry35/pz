using System;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public static class ItemSpecificStatsHelper
    {   
        [Obsolete] // но пока это необходимо для мобов у которых нет реального стата Damage 
        public static float GetDamage(ItemSpecificStats itemSpecificStats)
        {
            if (itemSpecificStats.ConstantDamage.HasValue)
                return itemSpecificStats.ConstantDamage.Value;
            if (itemSpecificStats.Stats != null)
                for (int i = 0; i < itemSpecificStats.Stats.Length; i++)
                {
                    var stat = itemSpecificStats.Stats[i];
                    if (stat.Stat.Target == DamageCalculationRoot.Instance.DamageStat.Target)
                        return stat.Value;
                }
            return 0;
        }

    }
}