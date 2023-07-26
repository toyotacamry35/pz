using JetBrains.Annotations;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [Binding]
    internal class SettingSwitcherBoolCtrl : SettingSwitcherCtrlBase<SettingSwitcherBoolVM, bool>
    { 
        // --- Publics: ---------------------------------

        [UsedImplicitly] //OnClick
        public void SwitchValue()
        {
            SetToOutRpForCtrl(!CurrInStreamVal);
        }
    }
}