using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using EnumerableExtensions;
using L10n;
using SharedCode.Aspects.Item.Templates;

namespace Assets.ColonyShared.SharedCode.Aspects.Craft
{
    /// <summary>
    /// Cтэк предметов
    /// </summary>
    public struct RecipeItemStack
    {
        /// <summary>
        /// Количество предметов
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Предмет
        /// </summary>
        public ResourceRef<BaseItemResource> Item { get; set; }

        public override string ToString()
        {
            return $"[{nameof(RecipeItemStack)}: {nameof(Item)}={Item.Target}, {nameof(Count)}={Count}]";
        }
    }

    /// <summary>
    /// Видообразующие предметы (те, от которых зависит вид предмета (пример: простой топорик, острый топорик, модный топорик и т.д.))
    /// </summary>
    public struct ViewVariant
    {
        /// <summary>
        /// То, что у меня получается на выходе: результат крафта
        /// </summary>
        public RecipeItemStack Product { get; set; }

        /// <summary>
        /// То, без чего мне не обойтись (логически являются взаимозаменяемыми предметами в своей группе)
        /// </summary>
        public RecipeItemStack[] MandatorySlots { get; set; }

        /// <summary>
        /// Info: если CraftingTime == 0 или не задано, то предполагается моментальный крафтинг, иначе с некой задержкой (указывается в секундах). 
        /// </summary>
        public float CraftingTime;

        public override string ToString()
        {
            return $"[{nameof(ViewVariant)}: {nameof(Product)}={Product}, {nameof(CraftingTime)}={CraftingTime}, " +
                   $"{nameof(MandatorySlots)}={CraftRecipeDef.GetAggregatedItems(MandatorySlots)}\n" +
                   "/vv]";
        }
    }

    /// <summary>
    /// Модификатор рецепта (то как рецепт влияет на что-либо: вес, урон, время сборки и т.д.)
    /// </summary>
    public struct CraftRecipeModifier
    {
        /// <summary>
        /// Предмет, используемый для сборки
        /// </summary>
        public RecipeItemStack Item { get; set; }

        /// <summary>
        /// Как меняются статы собранного предмета (вес, урон, время сборки и т.д.)
        /// </summary>
        public ValueStatDef[] StatsModifiers { get; set; }

        public override string ToString()
        {
            return $"[{nameof(CraftRecipeModifier)}: {nameof(Item)}={Item},\n" +
                   $"{nameof(StatsModifiers)}: {CraftRecipeDef.GetAggregatedItems(StatsModifiers)}\n" +
                   "/crm]";
        }
    }

    /// <summary>
    /// Компоненты, которые используются для крафтинга (весь набор предметов, которые необходимы для крафтинга)
    /// </summary>
    public struct RecipeSlot
    {
        public CraftRecipeModifier[] Items { get; set; }

        public override string ToString()
        {
            return $"[{nameof(RecipeSlot)}: {nameof(Items)}={CraftRecipeDef.GetAggregatedItems(Items)}/rs]";
        }
    }

    /// <summary>
    /// Рецепт (крафта с руки, верстачный, станочный)
    /// </summary>
    [Localized]
    public class CraftRecipeDef : RepairRecipeDef
    {
        /// <summary>
        /// Необходимые слоты, но логически взаимозаменяемые
        /// </summary>
        public RecipeSlot[] MandatorySlots { get; set; } = { };

        /// <summary>
        /// Опциональные слоты (логически влияют только на статы создаваемого предмета)
        /// </summary>
        public RecipeSlot[] OptionalSlots { get; set; } = { };

        public CraftRecipeModifier[] GetSimpleRecipeModifiers()
        {
            var len = MandatorySlots.Length;
            var modifiers = new CraftRecipeModifier[len];
            for (int i = 0; i < len; i++)
                modifiers[i] = MandatorySlots[i].Items[0];
            return modifiers;
        }

        public BaseItemResource GetProductItemResource(int productVariant)
        {
            if (productVariant < 0 ||
                Variants == null ||
                productVariant >= Variants.Length)
                return null;

            return Variants[productVariant].Product.Item.Target;
        }

        public float GetProductCraftTime(int productVariant)
        {
            if (productVariant < 0 ||
                Variants == null ||
                productVariant >= Variants.Length)
                return 0;

            return Variants[productVariant].CraftingTime;
        }

        public override string ToString()
        {
            return $"[{nameof(CraftRecipeDef)} '{____GetDebugShortName()}'" +
                   $"{(WorkbenchTypes == null ? "" : $" types: {WorkbenchTypes?.ItemsToString()}")}, Variants={Variants?.Length ?? 0}/crd]";
        }

        public string ToStringFull()
        {
            return $"[{nameof(CraftRecipeDef)}: " +
                   $"{nameof(WorkbenchTypes)}: {WorkbenchTypes?.ItemsToString()}, {nameof(Variants)}={GetAggregatedItems(Variants)}, " +
                   $"{nameof(MandatorySlots)}={GetAggregatedItems(MandatorySlots)},\n" +
                   $"{nameof(OptionalSlots)}={GetAggregatedItems(OptionalSlots)}\n" +
                   $"/{nameof(CraftRecipeDef)}]";
        }

        public static string GetAggregatedItems<T>(IEnumerable<T> items)
        {
            if (items == null)
                return "null";

            var count = items.Count();
            if (count == 0)
                return "empty";

            return $"Enumerable<{typeof(T)}> ({count}): " + items.Select(item => item.ToString())
                       .Aggregate((current, next) => current + ",\n" + next);
        }
    }

    [Localized]
    public class RepairRecipeDef : BaseRecipeDef
    {
        /// <summary>
        /// Видообразующие варианты рецепта
        /// </summary>
        public ViewVariant[] Variants { get; set; } = { };

        public ResourceRef<WorkbenchTypeDef>[] WorkbenchTypes { get; set; }

        public bool HasWorkbenchTypes => WorkbenchTypes != null && WorkbenchTypes.Any(t => t.Target != null);
    }
}