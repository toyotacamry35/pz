using System;
using System.Collections.Generic;
using ReactivePropsNs;

namespace Uins.Settings
{
    //Should work this way:
    // `TrueInStream` --> this_Proxy --> `InStreamForCtrl`
    // `TrueOutRp`    <-- this_Proxy <-- `OutRpForCtrl`

    // It's like MusicVolume f.e.
    internal class AppliedInstantlyCancelByCommandProxy<T> : IApplyableCancelableProxy<T>
    {
        private ReactiveProperty<T> TrueOutRp { get; set; }

        private bool _isInitValueFromInStreamReceived;
        private T _cachedTrueValue;
        private T _currValueToApply;


        // --- IApplyableCancelableProxy<T>: ----------------------------

        public bool WasChanged => _isInitValueFromInStreamReceived && !_currValueToApply.Equals(_cachedTrueValue);

        public void Init(
            IStream<T> trueInStream,
            ReactiveProperty<T> trueOutRp,
            ReactiveProperty<T> inStreamForCtrl,
            ReactiveProperty<T> outRpForCtrl,
            ICollection<IDisposable> d)
        {
            TrueOutRp = trueOutRp;

            trueInStream.Action(d,
                val =>
                {
                    /**/if (!_isInitValueFromInStreamReceived) //wrong: obstructs needed connection from trueInStream after apply (or change from somewhere aoutside)
                    //#wrong:  Т.е. тут нужно **безусловно** просто пропустить всё насквозь, обновив свои кэши связанных значений (_cached & _currToApply)
                    //Нельзя пускать безусловно, т.к. затираем своим тек.значением `_cachedTrueValue` (а оно нужно для Cancel'а). Оба остальные поля уже имеют эти значения.
                    //  `_currValueToApply` - т.к. тут мы оказались, т.к. завернулись из себя же из `outRpForCtrl.Action` (см. парой строк ниже)
                    {
                        _cachedTrueValue = val;
                        _currValueToApply = val;
                        _isInitValueFromInStreamReceived = true;
                        //inStreamForCtrl.Value = val;
                    }
                    inStreamForCtrl.Value = val;
                    // Нам нужно обновить Out4Ctrl и In4Ctrl.
                    //  Out4- чтобы не остаться с отставшим от реальности состоянием - приводит к тому, что когда Apply-им знач-е, которое новое, но в Out4- это значение осталось (т.к. не отреагировали на изменение TrueIn- извне), то Out4- не реагирует, т.к. то же знач-е, что и было. (проявлялось на `AppliedByCommandProxy`, но и тут похоже не помешает? если только не infinite-cycle)
                    outRpForCtrl.Value = val;
                });

            outRpForCtrl.Action(d,
                val =>
                {
                    _currValueToApply = val;
                    TrueOutRp.Value = val;
                    //inStreamForCtrl.Value = val;
                });
        }

        public bool Apply()
        {
            if (!WasChanged)
                return false;

            //TrueOutRp.Value = _currValueToApply;
            _cachedTrueValue = _currValueToApply;
            return true;
        }

        public bool Cancel()
        {
            if (!WasChanged)
                return false;

            TrueOutRp.Value = _cachedTrueValue;
            _currValueToApply = _cachedTrueValue;
            return true;
        }

    }

}
