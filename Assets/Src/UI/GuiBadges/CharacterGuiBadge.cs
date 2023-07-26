using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CharacterGuiBadge : HasHealthGuiBadge
    {
        //=== Props ===========================================================

        [Binding]
        public bool IsVisibleNickname { get; private set; }

        private static readonly PropertyBinder<CharacterGuiBadge, bool> IsVisibleNicknamePb = PropertyBinder<CharacterGuiBadge>.Create(_ => _.IsVisibleNickname);

        [Binding]
        public string Nickname { get; private set; }

        private static readonly PropertyBinder<CharacterGuiBadge, string> NicknamePb = PropertyBinder<CharacterGuiBadge>.Create(_ => _.Nickname);

        [Binding]
        public Sprite ChevronSprite { get; private set; }

        private static readonly PropertyBinder<CharacterGuiBadge, Sprite> ChevronPb = PropertyBinder<CharacterGuiBadge>.Create(_ => _.ChevronSprite);

        [Binding]
        public bool IsFriend { get; private set; }

        private static readonly PropertyBinder<CharacterGuiBadge, bool> IsFriendPb = PropertyBinder<CharacterGuiBadge>.Create(_ => _.IsFriend);


        //=== Public ==========================================================

        public override void Connect(IBadgePoint badgePoint)
        {
            base.Connect(badgePoint);

            var characterBadgePoint = badgePoint as CharacterBadgePoint;
            if (characterBadgePoint.AssertIfNull(nameof(characterBadgePoint), gameObject))
                return;

            characterBadgePoint.NicknameRp.Bind(ConnectionD, this, NicknamePb, "");
            characterBadgePoint.ChevronSpriteRp.Bind(ConnectionD, this, ChevronPb);
            characterBadgePoint.IsFriendRp.Bind(ConnectionD, this, IsFriendPb);
            characterBadgePoint.IsVisibleNicknameRp.Bind(ConnectionD, this, IsVisibleNicknamePb);
        }
    }
}