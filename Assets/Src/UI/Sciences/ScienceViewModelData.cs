using SharedCode.Aspects.Science;

namespace Uins
{
    public class ScienceViewModelData
    {
        public ScienceDef ScienceDef;
        public int Count;
        public bool IsBenefit;
        public int AvailCount;

        public ScienceViewModelData(ScienceDef scienceDef = null, int count = 0, bool isBenefit = false,
            int availCount = int.MaxValue)
        {
            ScienceDef = scienceDef;
            Count = count;
            IsBenefit = isBenefit;
            AvailCount = availCount;
        }
    }
}