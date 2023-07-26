using JetBrains.Annotations;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [Binding]
    public class SettingSwitcherFloatCtrl : SettingSwitcherCtrlBase_With2WayPropCtrl<SettingSwitcherFloatVM, float>
    {
        // --- Publics: ---------------------------------

        //#todo: Add Min & MaxVal to this class implementation (as IntCtrl does) & then these 2 methods could be uncommented & plugged  
        // В принципе слайдер уже имеет понимание Min и Max, т.к. без этого он не мог бы работать - можно попробовать пошарить с ним эти знания
        private const float StepPercent = 0.1f;
        [UsedImplicitly] //OnClick ->
        public void SwitchUp()
        {
            // var val = BindingProp;
            // // range * %
            // var delta = (MaxVal - MinVal) * StepPercent;
            // var targetVal = val + delta;
            // BindingProp = (targetVal > MaxVal) ? MaxVal : targetVal;
        }
        [UsedImplicitly] //OnClick <-
        public void SwitchDown()
        {
            // var val = BindingProp;
            // // range * %
            // var delta = (MaxVal - MinVal) * StepPercent;
            // var targetVal = val - delta;
            // BindingProp = (targetVal < MinVal) ? MinVal : targetVal;
        }

    }
}
