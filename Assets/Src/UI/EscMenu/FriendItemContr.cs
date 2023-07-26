using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class FriendItemContr : BindingControllerWithUsingProp<FriendInfo>
    {
        //=== Props ===========================================================

        [Binding]
        public string Nick { get; private set; }

        [Binding]
        public bool IsOnline { get; private set; }


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            Bind(Vmodel.Func(D, info => info.Login), () => Nick);
            Bind(Vmodel.Func(D, info => info.Status == FriendStatus.Online), () => IsOnline);
        }
    }
}