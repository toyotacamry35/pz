using JetBrains.Annotations;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [Binding]
    public class SettingSwitcherIntCtrl : SettingSwitcherCtrlBase<SettingSwitcherIntVM, int>
    {
        [UsedImplicitly] //is set via reflection by Weld inside .Bind
        [Binding]  public int MaxVal  { get; private set; }

        //#Dbg:
        // static int DBG_count_static;
        // int DBG_count;

        protected override void AwakeInternal()
        {
            // DBG_count = DBG_count_static++;

            var maxValStream   = Vmodel.SubStream(D, vm => vm.MaxVal);
            Bind(maxValStream,   () => MaxVal);
        }


        // --- Publics: ---------------------------------

        [UsedImplicitly] //OnClick ->
        public void SwitchUp()
        {
            var val = CurrInStreamVal;
            var res = (val == MaxVal) ? 0 : val+1;
            // if (DbgLog.Enabled) DbgLog.Log($"#*** IntCtlr[{DBG_count}]:: SwitchUp: {val} --> {res}");
            SetToOutRpForCtrl(res);
        }
        [UsedImplicitly] //OnClick <-
        public void SwitchDown()
        {
            var val = CurrInStreamVal;
            var res = (val == 0) ? MaxVal : val-1;
            //if (DbgLog.Enabled) DbgLog.Log($"#*** IntCtlr[{DBG_count}]:: SwitchUp: {val} --> {res}");
            SetToOutRpForCtrl(res);
        }

    }
}
