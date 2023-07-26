using L10n;
using SharedCode.Aspects.Item.Templates;

namespace SharedCode.Aspects.Science
{
    [Localized]
    public class CurrencyResource : BaseItemResource
    {
        public CurrencyResource()
        {
            MaxStack = int.MaxValue;
        }

        public LocalizedString ShortName { get; set; }
        public int SortingOrder { get; set; }
    }
}
