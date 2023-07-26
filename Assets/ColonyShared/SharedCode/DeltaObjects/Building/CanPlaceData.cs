using System.Text;

namespace SharedCode.DeltaObjects.Building
{
    public class CanPlaceData
    {
        public static uint REASON_OK                  = 0x00000000;
        public static uint REASON_NO_RESOURCES        = 0x00000001;
        public static uint REASON_TIMER               = 0x00000002;
        public static uint REASON_OUT_OF_PLACE        = 0x00000004;
        public static uint REASON_ALREADY_TAKEN       = 0x00000008;
        public static uint REASON_EDGES               = 0x00000010;
        public static uint REASON_GROUND              = 0x00000020;
        public static uint REASON_BLOCK_OVER          = 0x00000040;
        public static uint REASON_BLOCK_UNDER         = 0x00000080;
        public static uint REASON_WEIGHT              = 0x00000100;
        public static uint REASON_NO_RECIPE           = 0x00000200;
        public static uint REASON_STRUCTURE_ERROR     = 0x00000400;
        public static uint REASON_PROHIBITED_POSITION = 0x00000800;

        private uint reasons = REASON_OK;

        public bool Result { get { return (reasons == 0); } }
        public uint Reasons { get { return reasons; } }
        public void Set(uint reson) { reasons |= reson; }
        public void Clear(uint reson) { reasons &= ~reson; }
        public void Switch(bool set, uint reson)
        {
            if (set) { Set(reson); }
            else { Clear(reson); }
        }

        public string PrintReason()
        {
            var result = new StringBuilder();
            if ((reasons & REASON_NO_RESOURCES) > 0)
            {
                result.Append($"REASON_NO_RESOURCES\n");
            }
            if ((reasons & REASON_TIMER) > 0)
            {
                result.Append($"REASON_TIMER\n");
            }
            if ((reasons & REASON_OUT_OF_PLACE) > 0)
            {
                result.Append($"REASON_OUT_OF_PLACE\n");
            }
            if ((reasons & REASON_ALREADY_TAKEN) > 0)
            {
                result.Append($"REASON_ALREADY_TAKEN\n");
            }
            if ((reasons & REASON_EDGES) > 0)
            {
                result.Append($"REASON_EDGES\n");
            }
            if ((reasons & REASON_GROUND) > 0)
            {
                result.Append($"REASON_GROUND\n");
            }
            if ((reasons & REASON_BLOCK_OVER) > 0)
            {
                result.Append($"REASON_BLOCK_OVER\n");
            }
            if ((reasons & REASON_BLOCK_UNDER) > 0)
            {
                result.Append($"REASON_BLOCK_UNDER\n");
            }
            if ((reasons & REASON_WEIGHT) > 0)
            {
                result.Append($"REASON_WEIGHT\n");
            }
            if ((reasons & REASON_NO_RECIPE) > 0)
            {
                result.Append($"REASON_NO_RECIPE\n");
            }
            if ((reasons & REASON_STRUCTURE_ERROR) > 0)
            {
                result.Append($"REASON_STRUCTURE_ERROR\n");
            }
            if ((reasons & REASON_PROHIBITED_POSITION) > 0)
            {
                result.Append($"REASON_PROHIBITED_POSITION\n");
            }
            return result.ToString();
        }
    }
}
