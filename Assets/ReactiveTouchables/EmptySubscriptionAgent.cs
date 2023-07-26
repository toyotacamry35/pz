using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class EmptySubscriptionAgent<T> : ToucherSubscriptionAgent<T> where T : IDeltaObject
    {
        private static EmptySubscriptionAgent<T> _instance;
        public static EmptySubscriptionAgent<T> Instance => _instance ?? (_instance = new EmptySubscriptionAgent<T>());

        private EmptySubscriptionAgent() : base(new object())
        {
            NextInChain = PreviousInChain = this;
            _disposeWrapper.RequestDispose();
        }

        public override void ReceiveAddContainerChain(ITouchContainer<T> container) { }
        public override void ReceiveRemoveContainerChain(ITouchContainer<T> container) { }
        public override void DisposeChain() { }

        public override void Dispose() { }
    }
}