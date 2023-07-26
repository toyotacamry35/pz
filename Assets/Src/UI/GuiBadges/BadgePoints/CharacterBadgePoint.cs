using System;
using System.Linq;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.ResourceSystem.Account;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Uins
{
    public class CharacterBadgePoint : HasHealthBadgePoint
    {
        private TouchableEgoProxy<IWorldCharacterClientBroadcast> _touchableEntityProxy =
            new TouchableEgoProxy<IWorldCharacterClientBroadcast>(UnityEntityTouchContainerFactory<IWorldCharacterClientBroadcast>.Instance);

        [SerializeField, UsedImplicitly]
        private bool _hideNick;


        //=== Props ===========================================================

        public ReactiveProperty<string> NicknameRp { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsVisibleNicknameRp { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<Sprite> ChevronSpriteRp { get; } = new ReactiveProperty<Sprite>();

        public ReactiveProperty<bool> IsFriendRp { get; } = new ReactiveProperty<bool>() {Value = false};

        private LevelUpDataPackDef[] LevelUpData => GlobalConstsHolder.GlobalConstsDef.LevelUpDatas.Target.DataList;

        private ReactiveProperty<Guid> _userIdRp = new ReactiveProperty<Guid>() {Value = Guid.Empty};


        //=== Protected =======================================================

        protected override void Start()
        {
            base.Start();
            IsFriendRp.Func(D, GetIsVisibleNickname).Bind(D, IsVisibleNicknameRp);
        }


        protected override void OnHasClient(bool hasClient)
        {
            base.OnHasClient(hasClient);

            var ego = VisualMarker.Ego;
            if (hasClient)
            {
                _touchableEntityProxy.Connect(ego.ClientRepo, ego.TypeId, ego.EntityId, ReplicationLevel.ClientBroadcast);
                _touchableEntityProxy.ToStream(tmpD, character => character.Name).Bind(tmpD, NicknameRp);
                _touchableEntityProxy.ToStream(tmpD, character => character.AccountId).Bind(tmpD, _userIdRp);
                _userIdRp.Action(tmpD, OnUserIdChanged);

                var chevronIconStream = _touchableEntityProxy
                    .Child(tmpD, character => character.AccountStats)
                    .ToStream(tmpD, acc => acc.AccountExperience)
                    .Func(tmpD, LevelUpDatasHelpers.CalcAccLevel)
                    .Func(tmpD, GetChevronSprite);
                chevronIconStream.Bind(tmpD, ChevronSpriteRp);


                if (SurvivalGuiNode.Instance != null)
                {
                    // //2del <<<
                    // SurvivalGuiNode.Instance.FriendList.Log(tmpD, "FriendList", fi => $"id={fi.UserId}, nick={fi.Login}");
                    // NicknameRp.Action(
                    //     tmpD,
                    //     nick =>
                    //     {
                    //         if (string.IsNullOrEmpty(nick) || _cachedId != Guid.Empty)
                    //             return;
                    //
                    //         var selfFriendInfo = SurvivalGuiNode.Instance.FriendList.FirstOrDefault(el => el.UserId == ego.EntityId);
                    //         if (selfFriendInfo.UserId == Guid.Empty)
                    //         {
                    //             SurvivalGuiNode.Instance.FriendList.Add(new FriendInfo() {UserId = ego.EntityId, Login = nick});
                    //             _cachedId = ego.EntityId;
                    //         }
                    //     });
                    // //2del >>>

                    SurvivalGuiNode.Instance.FriendList.InsertStream.Action(tmpD, OnFriendAdd);
                    SurvivalGuiNode.Instance.FriendList.RemoveStream.Action(tmpD, OnFriendRemove);
                }
            }
            else
            {
                _touchableEntityProxy.Disconnect();
            }
        }

        protected override void OnDestroy()
        {
            // //2del <<<
            // if (SurvivalGuiNode.Instance != null)
            // {
            //     var selfFriendInfo = SurvivalGuiNode.Instance.FriendList.FirstOrDefault(el => el.UserId == _cachedId);
            //     if (selfFriendInfo.UserId != Guid.Empty)
            //         SurvivalGuiNode.Instance.FriendList.Remove(selfFriendInfo);
            // }
            // //2del >>>
            _touchableEntityProxy.Disconnect();
            base.OnDestroy();
        }

        private void OnUserIdChanged(Guid userId)
        {
            if (userId == Guid.Empty || SurvivalGuiNode.Instance == null)
            {
                IsFriendRp.Value = false;
                return;
            }

            var selfFriendInfo = SurvivalGuiNode.Instance.FriendList.FirstOrDefault(el => el.UserId == userId);
            if (selfFriendInfo.UserId != Guid.Empty)
                IsFriendRp.Value = true;
        }

        private void OnFriendAdd(InsertEvent<FriendInfo> insertEvt)
        {
            if (insertEvt.Item.UserId == _userIdRp.Value)
                IsFriendRp.Value = true;
        }

        private void OnFriendRemove(RemoveEvent<FriendInfo> removeEvt)
        {
            if (removeEvt.Item.UserId == _userIdRp.Value)
                IsFriendRp.Value = false;
        }

        private Sprite GetChevronSprite(int lvl)
        {
            return LevelUpData?[lvl - 1].RewardsPack.Target?.Title.Target?.StripeIcon?.Target;
        }
        private string GetDebugName()
        {
            return $"'{NicknameRp.Value}' {transform.root.name}";
        }
		
		private bool GetIsVisibleNickname(bool isFriend)
        {
            return isFriend || !_hideNick;
        }
    }
}