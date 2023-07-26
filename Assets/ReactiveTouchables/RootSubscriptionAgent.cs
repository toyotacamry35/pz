using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class RootSubscriptionAgent<T> : ToucherSubscriptionAgent<T> where T : IDeltaObject
    {
        public RootSubscriptionAgent(object chainLock, Action<T> onAdd = null, Action<T> onRemove = null, Action onCompleted = null)
            : base(chainLock, onAdd, onRemove, onCompleted)
        {
            NextInChain = PreviousInChain = this;
        }

        public ToucherSubscriptionAgent<T> Next => NextInChain;
        public ToucherSubscriptionAgent<T> Prev => PreviousInChain;

        public override void ReceiveAddContainerChain(ITouchContainer<T> container) { }

        public override void ReceiveRemoveContainerChain(ITouchContainer<T> container) { }

        public override void DisposeChain() { }
    }
}