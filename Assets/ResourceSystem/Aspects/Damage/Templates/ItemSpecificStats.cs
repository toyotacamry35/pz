using System;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class ItemSpecificStats : BaseResource
    {

        public float DestructionPower { get; set; } = -1;
        [Obsolete] /* но пока это необходимо для мобов у которых нет реального стата Damage */
        public float Damage
        {
            set
            {
                ConstantDamage = value;
            }
        }

        public float? ConstantDamage = null;

        /// <summary>
        /// Угловой размер (в градусах) сектора спереди персонажа в котором действует блок
        /// </summary>
        public float BlockSector { get; set; } = 90;   
 
        public ResourceRef<DamageTypeDef> DamageType { get; set; }

        public ResourceRef<WeaponSizeDef> WeaponSize { get; set; }

        public ResourceRef<HitMaterialDef> HitMaterial { get; set; }

        public StatModifier[] Stats { get; set; } = { };

        public bool TryGetStat(StatResource statRes, out float val)
        {
            val = 0f;
            if (statRes == null)
                return false;
            if(statRes == DamageCalculationRoot.Instance.DamageStat.Target)
                if (ConstantDamage.HasValue)
                {
                    val = ConstantDamage.Value;
                    return true;
                }
            if (Stats == null || Stats.Length == 0)
                return false;
            var idx = Array.FindIndex(Stats, sm => sm.Stat.Target == statRes);
            if (idx == -1)
                return false;
            val = Stats[idx].Value;
            return true;
        }
    }
}