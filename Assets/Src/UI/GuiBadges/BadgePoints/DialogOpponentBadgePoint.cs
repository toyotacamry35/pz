using L10n;
using ResourceSystem.Aspects.Dialog;

namespace Uins
{
    public class DialogOpponentBadgePoint : BadgePoint
    {
        //=== Props ===========================================================

        protected DialogOpponentGuiBadge DialogOpponentGuiBadge => (DialogOpponentGuiBadge) ConnectedGuiBadge;


        //=== Public ==========================================================

        public void SetPhrase(LocalizedString phrase)
        {
            DialogOpponentGuiBadge.AnswerLsRp.Value = phrase;
        }
    }
}