using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class FriendIndicatorNotifier : NavigationIndicatorNotifier
    {
        [SerializeField, UsedImplicitly]
        private CharacterBadgePoint _characterBadgePoint;

        private bool _isFriend;

        protected override void Start()
        {
            if (_characterBadgePoint.AssertIfNull(nameof(_characterBadgePoint)))
                return;

            base.Start();
            _characterBadgePoint.IsFriendRp
                .Zip(D, _characterBadgePoint.NicknameRp)
                .Action(D, OnIsFriendRpChanged);
        }

        private void OnIsFriendRpChanged(bool isFriend, string nick)
        {
            //UI.CallerLog($"OnIsFriendRpChanged(isFriend{isFriend.AsSign()}, nick='{nick}') "); //2del
            _isFriend = isFriend && !string.IsNullOrEmpty(nick);
            NavIndicator.Description = nick;
            IsDisplayable = GetIsDisplayable();
        }

        protected override bool GetIsDisplayable()
        {
            //UI.CallerLog($"GetIsDisplayable() _isFriend{_isFriend.AsSign()}"); //2del
            return _isFriend;
        }
    }
}