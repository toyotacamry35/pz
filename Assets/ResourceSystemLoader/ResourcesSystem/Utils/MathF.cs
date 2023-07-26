using System;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public static class MathF
    {
        //
        // Сводка:
        //     The infamous 3.14159265358979... value (Read Only).
        public const float PI = 3.14159274F;
        //
        // Сводка:
        //     A representation of positive infinity (Read Only).
        public const float Infinity = float.PositiveInfinity;
        //
        // Сводка:
        //     A representation of negative infinity (Read Only).
        public const float NegativeInfinity = float.NegativeInfinity;
        //
        // Сводка:
        //     Degrees-to-radians conversion constant (Read Only).
        public const float Deg2Rad = 0.0174532924F;
        //
        // Сводка:
        //     Radians-to-degrees conversion constant (Read Only).
        public const float Rad2Deg = 57.29578F;
        //
        // Сводка:
        //     A tiny floating point value (Read Only).
        public static readonly float Epsilon;

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static int RoundToInt(float v)
        {
            return (int)Math.Round(v);
        }
    }
}
