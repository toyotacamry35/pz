using System;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class InteractiveGuiBadge : GuiBadge
    {
        [SerializeField, UsedImplicitly]
        private ActionImage _actionImage;


        //=== Props ==============================================================

        [Binding]
        public bool IsVisibleLogicallyAndSelected { get; protected set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, bool> IsVisibleLogicallyAndSelectedBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.IsVisibleLogicallyAndSelected);

        [Binding]
        public bool IsVisibleLogicallyAndUnselected { get; protected set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, bool> IsVisibleLogicallyAndUnselectedBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.IsVisibleLogicallyAndUnselected);

        [Binding]
        public bool IsUnknown { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, bool> IsUnknownBinder = PropertyBinder<InteractiveGuiBadge>.Create(_ => _.IsUnknown);

        public LocalizedString ResourceName { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, LocalizedString> ResourceNameBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.ResourceName);

        public InputActionDef InteractionKey { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, InputActionDef> InteractionKeyBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.InteractionKey);

        [Binding]
        public LocalizedString DisplayedName { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, LocalizedString> DisplayedNameBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.DisplayedName);

        [Binding]
        public LocalizedString InteractionName { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, LocalizedString> InteractionNameBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.InteractionName);

        [Binding]
        public LocalizedString InfoName { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, LocalizedString> InfoNameBinder =
            PropertyBinder<InteractiveGuiBadge>.Create(_ => _.InfoName);

        [Binding]
        public bool HasInfoName { get; set; }

        private static readonly PropertyBinder<InteractiveGuiBadge, bool> HasInfoNameBinder = PropertyBinder<InteractiveGuiBadge>.Create(_ => _.HasInfoName);


        //=== Public ==========================================================

        public override void Connect(IBadgePoint badgePoint)
        {
            base.Connect(badgePoint);
            var interactiveBadgePoint = badgePoint as InteractiveBadgePoint;
            if (interactiveBadgePoint.AssertIfNull(nameof(interactiveBadgePoint)))
                return;

            var isVisibleAndSelectedStream = badgePoint.IsVisibleLogicallyRp
                .Zip(ConnectionD, badgePoint.IsSelectedRp)
                .Func(ConnectionD, Uins.BadgePoint.AndFunc);
            isVisibleAndSelectedStream.Bind(ConnectionD, this, IsVisibleLogicallyAndSelectedBinder);

            var isVisibleAndUnselectedStream = badgePoint.IsVisibleLogicallyRp
                .Zip(ConnectionD, badgePoint.IsSelectedRp)
                .Func(ConnectionD, Uins.BadgePoint.AndNotFunc);
            isVisibleAndUnselectedStream.Bind(ConnectionD, this, IsVisibleLogicallyAndUnselectedBinder);

            interactiveBadgePoint.InteractionNameRp.Bind(ConnectionD, this, InteractionNameBinder);
            interactiveBadgePoint.ResourceNameRp.Bind(ConnectionD, this, ResourceNameBinder);
            interactiveBadgePoint.InteractionKeyRp.Bind(ConnectionD, this, InteractionKeyBinder);
            interactiveBadgePoint.IsUnknownRp.Bind(ConnectionD, this, IsUnknownBinder, true);
            interactiveBadgePoint.HasInfoNameRp.Bind(ConnectionD, this, HasInfoNameBinder);
            interactiveBadgePoint.InfoNameRp.Bind(ConnectionD, this, InfoNameBinder);
            if (_actionImage != null)
                interactiveBadgePoint.InteractionKeyRp.Action(ConnectionD, def => _actionImage.SetKey(def));

            var finalResourceNameStream = interactiveBadgePoint.ResourceNameRp
                .Zip(ConnectionD, interactiveBadgePoint.IsUnknownRp)
                .Func(ConnectionD, (nameLs, isUnknown) => isUnknown ? (BadgesGui.Instance?.UnknownObjectLs ?? LsExtensions.EmptyWarning) : nameLs);

            finalResourceNameStream.Bind(ConnectionD, this, DisplayedNameBinder);

            if (badgePoint.IsDebug) //DEBUG
            {
                finalResourceNameStream.Log(ConnectionD, $"{name} {nameof(finalResourceNameStream)}");
                interactiveBadgePoint.IsUnknownRp.Log(ConnectionD, $"{name} {nameof(IsUnknown)}");
                interactiveBadgePoint.InteractionKeyRp.Log(ConnectionD, $"{name} {nameof(InteractionKey)}");
                interactiveBadgePoint.InfoNameRp.Log(ConnectionD, $"{name} {nameof(InfoName)}");
            }
        }
    }
}