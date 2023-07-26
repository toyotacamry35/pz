using System;
using L10n;
using SharedCode.Aspects.Science;

namespace Uins
{
    public class CostConfirmationSettings
    {
        public Action OnOkAction;
        public Action OnCancelAction;
        public LocalizedString Title;
        public LocalizedString Title2;
        public LocalizedString Question;
        public PriceDef CostsOrBenefits;
        public bool IsCostNorBenefit;
    }
}