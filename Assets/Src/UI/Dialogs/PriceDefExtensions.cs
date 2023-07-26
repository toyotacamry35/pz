using System.Linq;
using SharedCode.Aspects.Science;

namespace Uins
{
    public static class PriceDefExtensions
    {
        /// <summary>
        /// Отрицательное значение Count - затраты, положительное - прибыль. Считаем, есть ли затраты
        /// </summary>
        public static bool HasCosts(this PriceDef priceDef)
        {
            if (priceDef.TechPointCosts == null)
                return false;

            return priceDef.TechPointCosts.Where(cost => cost.TechPoint.Target != null).Min(cost => cost.Count) < 0;
        }

        /// <summary>
        /// Отрицательное значение Count - затраты, положительное - прибыль. Считаем, есть ли прибыль
        /// </summary>
        public static bool HasBenefits(this PriceDef priceDef)
        {
            if (priceDef.TechPointCosts == null)
                return false;

            return priceDef.TechPointCosts.Where(cost => cost.TechPoint.Target != null).Max(cost => cost.Count) > 0;
        }
    }
}