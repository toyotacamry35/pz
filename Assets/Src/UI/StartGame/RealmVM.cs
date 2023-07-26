using System;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using SharedCode.EntitySystem;

namespace Uins
{
    public class RealmVM : BindingVmodel
    {
        private const float RealmTimerUpdatePeriodSeconds = 60;

        public IStream<RealmRulesDef> RealmRulesDef { get; }
        public IStream<long> RealmTimeLeftSec { get; }
        public IStream<bool> RealmActiveStream { get; }

        public IStream<bool> RealmExistsStream { get; }

        public RealmVM(IStream<IEntitiesRepository> repositoryStream, IStream<OuterRef<IEntity>> realmOuterRefStream)
        {
            var realmEntityProxy = new OuterRefProxy<IRealmEntityClientFull>(realmOuterRefStream, repositoryStream);
            D.Add(realmEntityProxy);

            RealmExistsStream = realmEntityProxy.EntityExists;

            RealmRulesDef = realmEntityProxy.ToStreamWithDefault(D, realm => realm.Def);
            var realmStartTime = realmEntityProxy.ToStreamWithDefault(D, realm => realm.StartTime, -1);
            var timer = TimeTicker.Instance.GetUtcTimer(RealmTimerUpdatePeriodSeconds);

            var realmStartWithTicker = RealmRulesDef
                .Zip(D, realmStartTime)
                .ZipSecondOrDefault(D, timer);
            RealmTimeLeftSec = realmStartWithTicker
                .Func(
                    D,
                    (def, startTime, now) =>
                        def == null || startTime == -1
                            ? 0
                            : Convert.ToInt64(SyncTime.ToSeconds(RealmEntity.GetAliveLeftMs(def, startTime)))
                );
            RealmActiveStream = RealmTimeLeftSec.Func(D, timeLeftSec => timeLeftSec > 0);
            // RealmAllowsToJoin = realmStartWithTicker.Func(
            //     D,
            //     (def, startTime, now) => def != null && startTime != -1 && RealmEntity.IsRealmAllowsToJoin(def, startTime)
            // );
        }
    }
}