using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PerkNotificationViewModel : NotificationViewModel
    {
        [SerializeField, UsedImplicitly]
        private PerkSlotTypes _perkSlotTypes;

        ReactiveProperty<Sprite> _perkSpriteRp = new ReactiveProperty<Sprite>();
        ReactiveProperty<int> _perkTypeIndexRp = new ReactiveProperty<int>();
        ReactiveProperty<string> _perkTypeDescrRp = new ReactiveProperty<string>();

        
        //=== Props ===========================================================

        [Binding]
        public Sprite PerkSprite { get; private set; }

        [Binding]
        public int PerkTypeIndex { get; private set; }

        [Binding]
        public string PerkTypeDescr { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _perkSlotTypes.AssertIfNull(nameof(_perkSlotTypes));
            Bind(_perkSpriteRp, () => PerkSprite);
            Bind(_perkTypeIndexRp, () => PerkTypeIndex);
            Bind(_perkTypeDescrRp, () => PerkTypeDescr);
        }


        //=== Public ==========================================================

        public override void Show(HudNotificationInfo info)
        {
            base.Show(info);
            var perkNotificationInfo = info as PerkNotificationInfo;
            if (perkNotificationInfo.AssertIfNull(nameof(perkNotificationInfo)))
                return;

            _perkSpriteRp.Value = perkNotificationInfo.ItemResource?.Icon.Target;
            var itemType = perkNotificationInfo.ItemResource?.ItemType.Target;
            _perkTypeIndexRp.Value = _perkSlotTypes.GetTypeIndex(itemType);
            _perkTypeDescrRp.Value = (itemType?.DescriptionLs ?? LsExtensions.EmptyWarning).GetText();
        }
    }
}