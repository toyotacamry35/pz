using System;

namespace Uins.Extensions
{
    public static class BuiltinTypesExtensions
    {
        //-- long, ulong
        public static long SubstractSigned(this ulong a, ulong b)
        {
            if (a > b)
                return (long) (a - b);
            return -(long) (b - a);
        }

        /// <summary>
        /// Возвращает результат деления dividend на divisor с учетом того, что divisor м.б. равен 0
        /// </summary>
        public static float SafeDivide(this float dividend, float divisor, float divisionByZeroResult = float.MaxValue)
        {
            if (Math.Abs(divisor) < float.Epsilon)
            {
                if (Math.Abs(dividend) < float.Epsilon)
                    return 0;

                return (dividend < 0 ^ divisor < 0) ? -divisionByZeroResult : divisionByZeroResult; //знак
            }

            return dividend / divisor;
        }

        /// <summary>
        /// Возвращает лимитированное (clampMin...clampMax) значение безопасного деления dividend на divisor
        /// </summary>
        public static float SafeRatio(this float dividend, float divisor, float clampMin = 0, float clampMax = 1)
        {
            return Math.Max(Math.Min(clampMax, dividend.SafeDivide(divisor)), clampMin);
        }
    }
}