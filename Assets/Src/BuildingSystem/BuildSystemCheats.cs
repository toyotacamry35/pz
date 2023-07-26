using Assets.ColonyShared.SharedCode.Shared;
using Core.Cheats;

namespace Assets.Src.BuildingSystem
{
    public class BuildSystemCheat
    {
        // Cheat ----------------------------------------------------------------------------------
        [Cheat]
        public static void BuildDamageCheat(float damage)
        {
            bool enable = (damage > 0.0f);
            if (damage < 0.0f) { damage = 0.0f; }
            BuildSystem.Builder?.SetDamageCheat(NodeAccessor.Repository, enable, damage);
        }

        [Cheat]
        public static void BuildClaimResourceCheat(bool enable)
        {
            bool claim = !enable;
            BuildSystem.Builder?.SetClaimResourceCheat(NodeAccessor.Repository, enable, claim);
        }

        [Cheat]
        public static void BuildVisualCheat(bool enable)
        {
            BuildSystem.Builder?.SetVisualCheat(NodeAccessor.Repository, enable);
        }

        [Cheat]
        public static void BuildVisualRaycastCheat(bool enable)
        {
            BuildSystemHelper.SetVisualRaycastCheat(enable);
        }

        [Cheat]
        public static void BuildDebugCheat(bool enable)
        {
            BuildSystem.Builder?.SetDebugCheat(NodeAccessor.Repository, enable, true);
        }
    }
}