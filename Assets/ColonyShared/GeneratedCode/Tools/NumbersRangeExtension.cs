using SharedCode.Utils;

namespace Assets.Src.Tools
{
    public static class NumbersRangeExtension
    {
        public static bool InRange(this float val, float min, float max)
        {
            return min <= val && max > val;
        }
        public static bool InRange(this int val, int min, int max)
        {
            return min <= val && max > val;
        }
        public static bool AllInRange(this Vector3 val, float min, float max)
        {
            return min <= val.x && max > val.x && min <= val.y && max > val.y && min <= val.z && max > val.z;
        }
        public static bool AllInRange(this Vector2 val, float min, float max)
        {
            return min <= val.x && max > val.x && min <= val.y && max > val.y;
        }
    }
}