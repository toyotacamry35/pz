using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public delegate void OnPremiumChangedDelegate(DateTime expiration);

    public class HasPremiumApi : EntityApi
    {
        private DateTime _expiration;

        private event OnPremiumChangedDelegate OnPremiumExpirationChangedEvent;


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Public ==========================================================

        public void SubscribeToPremium(OnPremiumChangedDelegate onPremiumExpirationChanged)
        {
            if (onPremiumExpirationChanged.AssertIfNull(nameof(onPremiumExpirationChanged)))
                return;

            OnPremiumExpirationChangedEvent += onPremiumExpirationChanged;

            if (IsSubscribedSuccessfully)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { onPremiumExpirationChanged?.Invoke(_expiration); });
        }

        public void UnsubscribeFromNewFaction(OnPremiumChangedDelegate onPremiumExpirationChanged)
        {
            if (onPremiumExpirationChanged.AssertIfNull(nameof(onPremiumExpirationChanged)))
                return;

            OnPremiumExpirationChangedEvent -= onPremiumExpirationChanged;
        }


        //=== Protected =======================================================

        protected async override Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            /*var hasPremiumClientFull = (IHasPremiumClientFull) wrapper;
            if (hasPremiumClientFull.AssertIfNull(nameof(hasPremiumClientFull)))
                return;

            var premiumStatusClientFull = hasPremiumClientFull.PremiumStatus;
            _expiration = premiumStatusClientFull.Expiration;
            premiumStatusClientFull.SubscribePropertyChanged(nameof(premiumStatusClientFull.Expiration), OnExpirationChanged);

            UnityQueueHelper.RunInUnityThreadNoWait(() => { OnPremiumExpirationChangedEvent?.Invoke(_expiration); });*/
        }

        protected async override Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            /*var hasPremiumClientFull = (IHasPremiumClientFull) wrapper;
            if (hasPremiumClientFull.AssertIfNull(nameof(hasPremiumClientFull)))
                return;

            var premiumStatus = hasPremiumClientFull.PremiumStatus;
            premiumStatus.UnsubscribePropertyChanged(nameof(premiumStatus.Expiration), OnExpirationChanged);*/
        }


        //=== Private =========================================================

        private Task OnExpirationChanged(EntityEventArgs args)
        {
            _expiration = (DateTime) args.NewValue;
            Logger.IfDebug()?.Message($"new prem expire: {_expiration:R}").Write();
            UnityQueueHelper.RunInUnityThreadNoWait(() => { OnPremiumExpirationChangedEvent?.Invoke(_expiration); });
            return Task.CompletedTask;
        }
    }
}