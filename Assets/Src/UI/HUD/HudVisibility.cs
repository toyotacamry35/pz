using Assets.Src.Lib.Cheats;
using Core.Cheats;
using Uins.Sound;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HudVisibility : HudVisibilityBase
    {
        //=== Props ===========================================================

        public static HudVisibility Instance { get; private set; }



        public HudBlocksVisibility BlocksVisibility { get; private set; }


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            BlocksVisibility = HudBlocksVisibility.AllVisible;
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        [Cheat]
        public static void toggle_hud_visibility()
        {
            Instance.ToggleHudVisibility();
        }

        public void ToggleHudVisibility()
        {
            if (HudBlocksVisibility.AllVisible != BlocksVisibility)
                VisibleOn(HudBlocksVisibility.AllVisible);
            else
                VisibleOff(HudBlocksVisibility.AllVisible);
        }

        /// <summary>
        /// Выставляет маску видимости блоков
        /// </summary>
        public void SetVisibility(HudBlocksVisibility mask)
        {
            if (mask == BlocksVisibility)
                return;

            BlocksVisibility = mask;
            BottomSlotsIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.BottomSlots);
            HpIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.HpBlock);
            NavigationIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.NavigationBlock);
            EnvironmentIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.EnvironmentBlock);
            ChatIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.ChatBlock);
            HelpBlockIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.HelpBlock);
            FactionBlockIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.FactionBlock);
            OtherIsVisibleRp.Value = GetIsVisible(HudBlocksVisibility.Other);
        }

        /// <summary>
        /// Включает видимость блоков
        /// </summary>
        public void VisibleOn(HudBlocksVisibility onBlocks)
        {
            SetVisibility(BlocksVisibility | onBlocks);
        }

        /// <summary>
        /// Выключает видимость блоков
        /// </summary>
        public void VisibleOff(HudBlocksVisibility offBlocks)
        {
            SetVisibility(BlocksVisibility & ~offBlocks);
        }


        //=== Private =========================================================

        private bool GetIsVisible(HudBlocksVisibility hudBlocksVisibility)
        {
            return (BlocksVisibility & hudBlocksVisibility) > 0;
        }
    }
}