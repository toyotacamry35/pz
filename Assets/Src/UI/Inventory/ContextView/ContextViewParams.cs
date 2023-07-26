using L10n;
using UnityEngine;

namespace Uins.Inventory
{
    public class ContextViewParams
    {
        public enum LayoutType
        {
            NoTitleAndBackground,
            TitleAndNormalBackground,
            ExtraSpace,
            ExtraSpaceWithPointsPanels,
        }

        public bool HasWarningFlag = false;
        public LayoutType Layout = LayoutType.TitleAndNormalBackground;
        public LocalizedString ContextTitleLs;
        public Sprite ContextIcon;

        public bool NeedForExtraSpace => Layout == LayoutType.ExtraSpace || Layout == LayoutType.ExtraSpaceWithPointsPanels;

        public override string ToString()
        {
            return $"{nameof(Layout)}{Layout}, {nameof(HasWarningFlag)}{HasWarningFlag.AsSign()}, {nameof(ContextTitleLs)}={ContextTitleLs}";
        }
    }
}