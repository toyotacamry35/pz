using SharedCode.Aspects.Science;

namespace Uins
{
    public class TechPointViewModelData
    {
        public CurrencyResource TechPointDef;
        public int Count;
        public bool IsBenefit;
        public int AvailCount;

        public TechPointViewModelData(CurrencyResource techPointDef = null, int count = 0, bool isBenefit = false,
            int availCount = int.MaxValue)
        {
            TechPointDef = techPointDef;
            Count = count;
            IsBenefit = isBenefit;
            AvailCount = availCount;
        }
    }
}