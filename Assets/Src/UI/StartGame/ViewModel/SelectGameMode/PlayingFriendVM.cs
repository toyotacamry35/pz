using System;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using NLog;
using PzLauncher.Models.Dto;
using ReactivePropsNs;
using ResourcesSystem.Loader;
using SharedCode.Aspects.Sessions;

namespace Uins
{
    public class PlayingFriendVM : BindingVmodel
    {
        private const float RealmTimerUpdatePeriodSeconds = 60;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ReactiveProperty<string> _friendName = new ReactiveProperty<string>();
        private readonly ReactiveProperty<bool> _online = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _hovered = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<string> _selectedFriend;

        public PlayingFriendVM(Friend friend, ReactiveProperty<string> selectedFriend)
        {
            _selectedFriend = selectedFriend;

            _friendName.Value = friend.Login;
            _online.Value = friend.Status == "online";
            UserId = friend.UserId;

            Selected = _selectedFriend.Func(D, userId => userId == UserId);

            RealmQueryDef = LoadDef<RealmRulesQueryDef>(friend.Definition);

            RealmTimeLeftSec = InitTimeLeft(friend.SessionWhenCreated, RealmRulesDef);

            RealmActiveStream = RealmTimeLeftSec.Func(D, timeLeftSec => timeLeftSec > 0);
        }

        public IStream<string> FriendName => _friendName;
        public IStream<bool> Online => _online;
        public IStream<bool> Selected { get; }
        public IReactiveProperty<bool> Hovered => _hovered;
        public RealmRulesQueryDef RealmQueryDef { get; }
        public RealmRulesDef RealmRulesDef => RealmQueryDef?.RealmRules.Target;
        public IReactiveProperty<long> RealmTimeLeftSec { get; }
        public string UserId { get; }
        public IStream<bool> RealmActiveStream { get; }

        public void SetSelected()
        {
            _selectedFriend.Value = UserId;
        }

        public void SetHovered(bool value)
        {
            _hovered.Value = value;
        }

        private IReactiveProperty<long> InitTimeLeft(DateTime? friendSessionWhenCreated, RealmRulesDef realmRulesDef)
        {
            var timeLeft = new ReactiveProperty<long>();

            if (realmRulesDef != null && friendSessionWhenCreated != null)
            {
                var timer = TimeTicker.Instance.GetUtcTimer(RealmTimerUpdatePeriodSeconds);
                var startTime = SyncTime.DateTimeToTimeUnits(friendSessionWhenCreated.Value);
                timeLeft.Value = GetTimeLeft();
                timer.Action(
                    D,
                    now => timeLeft.Value = GetTimeLeft()
                );

                long GetTimeLeft()
                {
                    return startTime == -1
                        ? 0
                        : Convert.ToInt64(
                            SyncTime.ToSeconds(
                                RealmEntity.GetAliveLeftMs(realmRulesDef, startTime)
                            )
                        );
                }
            }
            else
                timeLeft.Value = 0;

            return timeLeft;
        }

        private static T LoadDef<T>(string root) where T : IResource
        {
            var realmRulesQueryDef = default(T);
            if (root != null)
                try
                {
                    realmRulesQueryDef = GameResourcesHolder.Instance.LoadResource<T>(new ResourceIDFull(root));
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"Unable Parse Definition By Address {root}").Exception(e).Write();
                }

            return realmRulesQueryDef;
        }
    }
}