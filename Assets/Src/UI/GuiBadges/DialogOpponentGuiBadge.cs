using L10n;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DialogOpponentGuiBadge : GuiBadge
    {
        //=== Prop ============================================================

        public ReactiveProperty<LocalizedString> AnswerLsRp { get; } = new ReactiveProperty<LocalizedString>();

        [Binding]
        public LocalizedString AnswerLs { get; private set; }


        //=== Public ==========================================================

        protected override void Awake()
        {
            base.Awake();
            Bind(AnswerLsRp, () => AnswerLs);
        }
    }
}