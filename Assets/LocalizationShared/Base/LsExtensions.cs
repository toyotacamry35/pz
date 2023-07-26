using Assets.ColonyShared.SharedCode.Aspects.Craft;

namespace L10n
{
    public static class LsExtensions
    {
        public static readonly LocalizedString Empty = new LocalizedString();

        public static readonly LocalizedString EmptyWarning = new LocalizedString("(empty)");

        public static readonly LocalizedString FirstArgumentOnly = new LocalizedString("{0}");

        public static bool IsEmpty(this LocalizedString ls)
        {
            return ls.Equals(Empty);
        }

        public static string GetText(this LocalizedString ls, int pluralN = 0, params object[] args)
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder)) ||
                ls.IsEmpty())
                return "";

            return ls.GetString(L10nHolder.Instance.TextCatalog, pluralN, args);
        }

        public static LocalizedString GetRecipeOrProductNameLs(this RepairRecipeDef def)
        {
            return
                def.NameLs.IsEmpty() &&
                !(def.Variants?[0].Product.Item.Target?.ItemNameLs ?? Empty).IsEmpty()
                    ? def.Variants[0].Product.Item.Target.ItemNameLs
                    : def.NameLs;
        }

        public static LocalizedString GetRecipeOrProductDescriptionLs(this RepairRecipeDef def)
        {
            return
                def.DescriptionLs.IsEmpty() &&
                !(def.Variants?[0].Product.Item.Target?.DescriptionLs ?? Empty).IsEmpty()
                    ? def.Variants[0].Product.Item.Target.DescriptionLs
                    : def.DescriptionLs;
        }
    }
}