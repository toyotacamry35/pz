using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class HudVisibilityBase : BindingViewModel
    {
        public ReactiveProperty<bool> BottomSlotsIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool BottomSlotsIsVisible { get; set; }

        public ReactiveProperty<bool> HpIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool HpIsVisible { get; set; }

        public ReactiveProperty<bool> NavigationIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool NavigationIsVisible { get; set; }

        public ReactiveProperty<bool> EnvironmentIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool EnvironmentIsVisible { get; set; }

        public ReactiveProperty<bool> ChatIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool ChatSlotsIsVisible { get; set; }

        public ReactiveProperty<bool> HelpBlockIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool HelpBlockIsVisible { get; set; }

        public ReactiveProperty<bool> FactionBlockIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool FactionBlockIsVisible { get; set; }

        public ReactiveProperty<bool> OtherIsVisibleRp { get; set; } = new ReactiveProperty<bool>();

        [Binding]
        public bool OtherIsVisible { get; set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            BottomSlotsIsVisibleRp.Value = true;
            Bind(BottomSlotsIsVisibleRp, () => BottomSlotsIsVisible);

            HpIsVisibleRp.Value = true;
            Bind(HpIsVisibleRp, () => HpIsVisible);

            NavigationIsVisibleRp.Value = true;
            Bind(NavigationIsVisibleRp, () => NavigationIsVisible);

            EnvironmentIsVisibleRp.Value = true;
            Bind(EnvironmentIsVisibleRp, () => EnvironmentIsVisible);

            ChatIsVisibleRp.Value = true;
            Bind(ChatIsVisibleRp, () => ChatSlotsIsVisible);

            HelpBlockIsVisibleRp.Value = true;
            Bind(HelpBlockIsVisibleRp, () => HelpBlockIsVisible);

            FactionBlockIsVisibleRp.Value = true;
            Bind(FactionBlockIsVisibleRp, () => FactionBlockIsVisible);

            OtherIsVisibleRp.Value = true;
            Bind(OtherIsVisibleRp, () => OtherIsVisible);
        }
    }
}