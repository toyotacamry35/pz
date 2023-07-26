using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace Assets.ResourceSystem.Account
{
    [Localized]
    public class AccountLevelRewardsPackDef : BaseResource
    {
        // Главная уровневая ачивка (Джон прелагает называть её "анлок"):
        public readonly ResourceRef<AccountLevelAchievementDef> Achievement;
        // Титул:
        public readonly ResourceRef<AccountTitleDef> Title;

        public override bool Equals(object obj)
        {
            if (!(obj is AccountLevelRewardsPackDef))
                return false;
            var other = (AccountLevelRewardsPackDef)obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Achievement.GetHashCode() * 397) ^ Title.GetHashCode();
            }
        }

        public bool Equals(AccountLevelRewardsPackDef other)
        {
            return Achievement.Equals(other.Achievement)
                   && Title.Equals(other.Title);
        }
    }
}
