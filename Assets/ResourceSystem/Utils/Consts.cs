using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class Consts
    {
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        // Numerics:-------------------------
        public const int InvalidIndex = -1;

        // Animator -------------------------
        public const string AnimatorReactionDirZ = "ReactionDirZ";
        public const string AnimatorReactionDirX = "ReactionDirX";

        // Resources ------------------------
        public static ItemTypeResource BaseWeaponItemType => BaseWeaponItemTypeRef.Target;
        private static readonly ResourceRef<ItemTypeResource> BaseWeaponItemTypeRef = new ResourceRef<ItemTypeResource>("/UtilPrefabs/ItemGroups/Group_Weapons");

        // Logic: ---------------------------
        public static float MiningDamage = 1f;

        // Debug ----------------------------
        public const string DbgTagIn = "IN";
        public const string DbgTagIn1 = "IN-1";
        public const string DbgTagIn2 = "IN-2";
        public const string DbgTagOut = "OUT";
        public const string DbgTagOut1 = "OUT-1";
        public const string DbgTagOut2 = "OUT-2";
    }
}
