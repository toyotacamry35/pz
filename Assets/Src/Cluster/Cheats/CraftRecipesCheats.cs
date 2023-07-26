using System.Collections;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using Uins;
using Uins.Inventory;
using UnityEngine;

namespace Assets.Src.Cluster.Cheats
{
    public static class CraftRecipesCheats
    {
        /// <summary>
        /// Получить необходимые компоненты для первого варианта текущего рецепта
        /// </summary>
        [Cheat]
        public static void AddRecipeFirstItem(int count = 1)
        {
            AddRecipeItems(true, count);
        }

        /// <summary>
        /// Получить необходимые компоненты для всех вариантов текущего рецепта
        /// </summary>
        [Cheat]
        public static void AddRecipeAllItems(int count = 1)
        {
            AddRecipeItems(false, count);
        }


//        /// <summary>
//        /// Проверка корректности известных рецептов: на каждый ли 
//        /// </summary>
//        [Cheat]
//        public static void CheckRecipesVariantsRecursive()
//        {
//            var craftSideViewModel = Object.FindObjectOfType<CraftSideViewModel>();
//            if (craftSideViewModel.AssertIfNull(nameof(craftSideViewModel)))
//                return;
//
//            foreach (var item in craftSideViewModel.KnownRecipeViewModels)
//            {
//                CraftRecipeModifier[] pathItem = new CraftRecipeModifier[item.Recipe.MandatorySlots.Length];
//                IterateSlot(item.Recipe, pathItem, 0);
//            }
//        }

//        private static void IterateSlot(CraftRecipeDef def, CraftRecipeModifier[] pathItem, int index)
//        {
//            if (index > def.MandatorySlots.Length)
//            {
//                CraftRecipeContextView.GetVariantIndex(def, pathItem);
//                return;
//            }
//
//            foreach (var slot in def.MandatorySlots[index].Items)
//            {
//                pathItem[index] = slot;
//                IterateSlot(def, pathItem, index + 1);
//            }
//        }

//        /// <summary>
//        /// Проверка корректности известных рецептов: у любого ли MandatorySlot есть Variant?
//        /// </summary>
//        [Cheat]
//        public static void CheckRecipesVariants()
//        {
//            var craftSideViewModel = Object.FindObjectOfType<CraftSideViewModel>();
//            if (craftSideViewModel.AssertIfNull(nameof(craftSideViewModel)))
//                return;
//
//            foreach (var item in craftSideViewModel.KnownRecipeViewModels)
//            {
//                var recipe = item.Recipe;
//                CraftRecipeModifier[] pathItem = new CraftRecipeModifier[recipe.MandatorySlots.Length];
//                for (int s0 = 0; s0 < recipe.MandatorySlots[0].Items.Length; s0++) //варианты нулевого обязательного слота
//                {
//                    pathItem[0] = recipe.MandatorySlots[0].Items[s0];
//                    if (pathItem.Length > 1)
//                    {
//                        for (int s1 = 0; s1 < recipe.MandatorySlots[1].Items.Length; s1++) //варианты первого слота при его наличии
//                        {
//                            pathItem[1] = recipe.MandatorySlots[1].Items[s1];
//                            if (pathItem.Length > 2)
//                            {
//                                for (int s2 = 0; s2 < recipe.MandatorySlots[2].Items.Length; s2++) //
//                                {
//                                    pathItem[2] = recipe.MandatorySlots[2].Items[s2];
//                                    if (pathItem.Length > 3)
//                                    {
//                                        for (int s3 = 0; s3 < recipe.MandatorySlots[3].Items.Length; s3++) //
//                                        {
//                                            pathItem[3] = recipe.MandatorySlots[3].Items[s3];
//                                            if (pathItem.Length > 4)
//                                            {
//                                                for (int s4 = 0; s4 < recipe.MandatorySlots[4].Items.Length; s4++) //
//                                                {
//                                                    pathItem[4] = recipe.MandatorySlots[4].Items[s4];
//                                                    CraftRecipeContextView.GetVariantIndex(recipe, pathItem);
//                                                }
//                                            }
//                                            else
//                                            {
//                                                CraftRecipeContextView.GetVariantIndex(recipe, pathItem);
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        CraftRecipeContextView.GetVariantIndex(recipe, pathItem);
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                CraftRecipeContextView.GetVariantIndex(recipe, pathItem);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        CraftRecipeContextView.GetVariantIndex(recipe, pathItem);
//                    }
//                }
//            }
//
//            UI.CallerLog("END");
//        }

//        /// <summary>
//        /// Проверка корректности известных рецептов: все ли инридиенты перечислены в mandatory slot
//        /// </summary>
//        [Cheat]
//        public static void CheckRecipesSlots()
//        {
//            var craftSideViewModel = Object.FindObjectOfType<CraftSideViewModel>();
//            if (craftSideViewModel.AssertIfNull(nameof(craftSideViewModel)))
//                return;
//
//            foreach (var item in craftSideViewModel.KnownRecipeViewModels)
//            {
//                var recipe = item.Recipe;
//                for (int j = 0; j < recipe.Variants.Length; j++) //по вариантам
//                {
//                    for (int k = 0; k < recipe.Variants[j].MandatorySlots.Length; k++) //слоты варианта
//                    {
//                        if (recipe.Variants[j].MandatorySlots[k].Item.Target == ClusterCraftCommands.AnyItem)
//                        {
//                            continue;
//                        }
//
//                        for (int l = 0; l < recipe.MandatorySlots[k].Items.Length; l++)
//                        {
//                            if (recipe.MandatorySlots[k].Items[l].Item.Item.Target ==
//                                recipe.Variants[j].MandatorySlots[k].Item.Target)
//                            {
//                                break;
//                            }
//                            else if (l == recipe.MandatorySlots[k].Items.Length - 1)
//                            {
//                                UI.Logger.Error(
//                                    $"CRAFT JSON ERROR. recipe={recipe}  Not found: variantIndex={j}, itemIndex={k}");
//                            }
//                        }
//                    }
//                }
//            }
//
//            UI.CallerLog("END");
//        }


        //=== Private =========================================================

        private static void AddRecipeItems(bool forFirstVariantOnly, int count)
        {
            var craftRecipeContextView = Object.FindObjectOfType<CraftRecipeContextView>();
            if (craftRecipeContextView.AssertIfNull(nameof(craftRecipeContextView)))
                return;

            if (craftRecipeContextView.CurrentRecipeVm == null)
            {
                UI. Logger.IfInfo()?.Message("Don't selected any recipe").Write();;
                return;
            }

            craftRecipeContextView.StartInstrumentedCoroutine(
                AddRecipesCoroutine(craftRecipeContextView.CurrentRecipeVm.CraftRecipeDef, count, forFirstVariantOnly));
        }

        private static IEnumerator AddRecipesCoroutine(CraftRecipeDef craftRecipeDef, int count, bool forFirstVariantOnly)
        {
            for (int i = 0; i < craftRecipeDef.MandatorySlots.Length; i++)
            {
                var len = forFirstVariantOnly ? 1 : craftRecipeDef.MandatorySlots[i].Items.Length;
                for (int j = 0; j < len; j++)
                {
                    var resource = craftRecipeDef.MandatorySlots[i].Items[j].Item.Item.Target;
                    if (resource != null)
                    {
                        ClusterCheats.AddItemResource(resource, craftRecipeDef.MandatorySlots[i].Items[j].Item.Count * count);
                        yield return new WaitForSeconds(.5f);
                    }
                }
            }

            for (int i = 0; i < craftRecipeDef.OptionalSlots.Length; i++)
            {
                var len = forFirstVariantOnly ? 1 : craftRecipeDef.OptionalSlots[i].Items.Length;
                for (int j = 0; j < len; j++)
                {
                    var resource = craftRecipeDef.OptionalSlots[i].Items[j].Item.Item.Target;
                    if (resource != null)
                    {
                        ClusterCheats.AddItemResource(resource, craftRecipeDef.OptionalSlots[i].Items[j].Item.Count * count);
                        yield return new WaitForSeconds(.5f);
                    }
                }
            }
        }
    }
}