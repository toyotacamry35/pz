using System.Threading.Tasks;
using Core.Cheats;
using SharedCode.Serializers;

namespace Assets.Src.Cluster.Cheats
{
    // Личные читы Сергея Букова.
    // За несанкционированное изменение - смерть лютая!
    public static partial class ClusterCheats
    {
        public class SergeyCheats
        {
            [Cheat]
            public static async Task AddClothes(int setIndex)
            {
                string[] lines = null;
                switch (setIndex)
                {
                    case 1:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/CapsuleBoots",
                            "/Inventory/Items/Clothes/CapsuleJacket",
                            "/Inventory/Items/Clothes/CapsulePants",
                        };
                        break;

                    case 2:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/TornBoots",
                            "/Inventory/Items/Clothes/TornJacket",
                            "/Inventory/Items/Clothes/TornPants",
                        };
                        break;

                    case 3:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/New/Fire_protection_chest_1",
                            "/Inventory/Items/Clothes/New/Fire_protection_feet_1",
                            "/Inventory/Items/Clothes/New/Fire_protection_hands_1",
                            "/Inventory/Items/Clothes/New/Fire_protection_head_1",
                            "/Inventory/Items/Clothes/New/Fire_protection_legs_1"
                        };
                        break;

                    case 4:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/ReinforcedBoots",
                            "/Inventory/Items/Clothes/ReinforcedGloves",
                            "/Inventory/Items/Clothes/ReinforcedJacket",
                            "/Inventory/Items/Clothes/ReinforcedPants",
                            "/Inventory/Items/Clothes/Headband",
                        };
                        break;

                    case 5:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/LeatherBoots",
                            "/Inventory/Items/Clothes/LeatherGloves",
                            "/Inventory/Items/Clothes/LeatherJacket",
                            "/Inventory/Items/Clothes/LeatherPants",
                        };
                        break;

                    case 6:
                        lines = new string[]
                        {
                            "/Inventory/Items/Clothes/CombatBoots",
                            "/Inventory/Items/Clothes/CombatJacket",
                            "/Inventory/Items/Clothes/CombatPants",
                            "/Inventory/Items/Clothes/CombatGloves",
                            "/Inventory/Items/Clothes/CombatHelmet",
                        };
                        break;
                }

                AddItemsList(lines);
            }

            [Cheat]
            public static async Task AddAllClothes()
            {
                await AsyncUtils.RunAsyncTask(() => InventorySetSize(17, true, InventoryAddressResolver));
                await Task.Delay(1000);
                string[] lines =
                {
                    "/Inventory/Items/Clothes/New/Fire_protection_chest_1",
                    "/Inventory/Items/Clothes/New/Fire_protection_feet_1",
                    "/Inventory/Items/Clothes/New/Fire_protection_hands_1",
                    "/Inventory/Items/Clothes/New/Fire_protection_head_1",
                    "/Inventory/Items/Clothes/New/Fire_protection_legs_1",

                    "/Inventory/Items/Clothes/New/Cold_protection_3_chest",
                    "/Inventory/Items/Clothes/New/Cold_protection_3_feet",
                    "/Inventory/Items/Clothes/New/Cold_protection_3_hands",
                    "/Inventory/Items/Clothes/New/Cold_protection_3_head",
                    "/Inventory/Items/Clothes/New/Cold_protection_3_legs",

                    "/Inventory/Items/Clothes/New/Heat_protection_3",
                    "/Inventory/Items/Clothes/New/Toxic_protection_1",
                    "/Inventory/Items/Clothes/New/Toxic_protection_2",
                    "/Inventory/Items/Clothes/New/Toxic_protection_4",

                    "/Inventory/Items/Consumables/HeatProtectionSuit",
                    "/Inventory/Items/Consumables/ChemicalProtectionSuit",
                    "/Inventory/Items/Consumables/EmpowerSuit"
                };
                AddItemsList(lines);
            }

            [Cheat]
            public static async Task AddArmor(int setIndex)
            {
                string[] lines = null;
                switch (setIndex)
                {
                    case 1:
                        lines = new string[]
                        {
                            "/Inventory/Items/Armor/New/Armor_1_arms",
                            "/Inventory/Items/Armor/New/Armor_1_chest",
                            "/Inventory/Items/Armor/New/Armor_1_head",
                            "/Inventory/Items/Armor/New/Armor_1_legs",
                        };
                        break;

                    case 2:
                        lines = new string[]
                        {
                            "/Inventory/Items/Armor/New/Armor_2_arms",
                            "/Inventory/Items/Armor/New/Armor_2_chest",
                            "/Inventory/Items/Armor/New/Armor_2_head",
                            "/Inventory/Items/Armor/New/Armor_2_legs",
                        };
                        break;

                    case 3:
                        lines = new string[]
                        {
                            "/Inventory/Items/Armor/New/Armor_3_arms",
                            "/Inventory/Items/Armor/New/Armor_3_chest",
                            "/Inventory/Items/Armor/New/Armor_3_head",
                            "/Inventory/Items/Armor/New/Armor_3_legs",
                        };
                        break;

                    case 4:
                        lines = new string[]
                        {
                            "/Inventory/Items/Armor/New/Armor_4_chest",
                            "/Inventory/Items/Armor/New/Armor_4_hands",
                            "/Inventory/Items/Armor/New/Armor_4_head",
                            "/Inventory/Items/Armor/New/Armor_4_legs"
                        };
                        break;
                }

                AddItemsList(lines);
            }

            [Cheat]
            public static async Task AddAllArmor()
            {
                await AsyncUtils.RunAsyncTask(() => InventorySetSize(16, true, InventoryAddressResolver));
                await Task.Delay(1000);
                string[] lines =
                {
                    "/Inventory/Items/Armor/New/Armor_1_arms",
                    "/Inventory/Items/Armor/New/Armor_1_chest",
                    "/Inventory/Items/Armor/New/Armor_1_head",
                    "/Inventory/Items/Armor/New/Armor_1_legs",

                    "/Inventory/Items/Armor/New/Armor_2_arms",
                    "/Inventory/Items/Armor/New/Armor_2_chest",
                    "/Inventory/Items/Armor/New/Armor_2_head",
                    "/Inventory/Items/Armor/New/Armor_2_legs",

                    "/Inventory/Items/Armor/New/Armor_3_arms",
                    "/Inventory/Items/Armor/New/Armor_3_chest",
                    "/Inventory/Items/Armor/New/Armor_3_head",
                    "/Inventory/Items/Armor/New/Armor_3_legs",

                    "/Inventory/Items/Armor/New/Armor_4_chest",
                    "/Inventory/Items/Armor/New/Armor_4_hands",
                    "/Inventory/Items/Armor/New/Armor_4_head",
                    "/Inventory/Items/Armor/New/Armor_4_legs"
                };
                AddItemsList(lines);
            }
        }
    }
}