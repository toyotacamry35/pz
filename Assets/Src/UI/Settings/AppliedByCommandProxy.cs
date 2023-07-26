using System;
using System.Collections.Generic;
using ReactivePropsNs;

namespace Uins.Settings
{
    public interface IApplyable
    {
        bool Apply();
    }
    public interface ICancelable
    {
        bool Cancel();
    }
    public interface IDefaultable
    {
        bool SetToDefault();
    }
    public interface ICanBeChanged
    {
        bool WasChanged { get; }
    }
    public interface IApplyableCancelable : IApplyable, ICancelable
    {
    }
    //public interface IApplyableCancelableDefaultable : IApplyable, ICancelable, IDefaultable
    //{
    //} 
    public interface IApplyableCancelableDefaultableCanBeChanged : IApplyable, ICancelable, IDefaultable, ICanBeChanged
    {
    }

    public interface IApplyableCancelableProxy<T> : IApplyableCancelable
    {
        // Shows, is there what to do for Apply & Cancel methods:
        bool WasChanged { get; }
        void Init(
            IStream<T> trueInStream,
            ReactiveProperty<T> trueOutRp,
            ReactiveProperty<T> inStreamForCtrl,
            ReactiveProperty<T> outRpForCtrl,
            ICollection<IDisposable> d);
    }

    //Should work this way:
    // `TrueInStream` --> this_Proxy --> `InStreamForCtrl`
    // `TrueOutRp`    <-- this_Proxy <-- `OutRpForCtrl`

    // It's like Resolution f.e.
    internal class AppliedByCommandProxy<T> : IApplyableCancelableProxy<T>
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
                    //if (!_isInitValueFromInStreamReceived) //wrong: obstructs needed connection from trueInStream after apply (or change from somewhere aoutside)
                    // Т.е. тут нужно просто пропустить всё насквозь, обновив свои кэши связанных значений (_cached & _currToApply)
                    {
                        _cachedTrueValue = val;
                        _currValueToApply = val;
                        _isInitValueFromInStreamReceived = true;
                        // Нам нужно обновить Out4Ctrl и In4Ctrl.
                        //  Out4- чтобы не остаться с отставшим от реальности состоянием - приводит к тому, что когда Apply-им знач-е, которое новое, но в Out4- это значение осталось (т.к. не отреагировали на изменение TrueIn- извне), то Out4- не реагирует, т.к. то же знач-е, что и было.
                        //  In4- тут закомменчено, т.к. Out4- форвардит в In4- 
                        //inStreamForCtrl.Value = val;
                        outRpForCtrl.Value = val;
                    }
                    //inStreamForCtrl.Value = val;
                });

            outRpForCtrl.Action(d,
                val =>
                {
                    // Тут нужно запомнить _currToApply и завернуть в себя же (сделав вид д/ Ctrl'а, что указание принято)

                    //if (DbgLog.Enabled) DbgLog.Log($"aAA. abcProxy: outRpForCtrl({val}) --> inStreamForCtrl({(inStreamForCtrl.HasValue ? inStreamForCtrl.Value.ToString() : "no_val")})");

                    _currValueToApply = val;
                    //TrueOutRp.Value = val;
                    inStreamForCtrl.Value = val;

                    //if (DbgLog.Enabled) DbgLog.Log($"Zzz. abcProxy: outRpForCtrl({val}) --> inStreamForCtrl({(inStreamForCtrl.HasValue ? inStreamForCtrl.Value.ToString() : "no_val")})");
                });
        }

        public bool Apply()
        {
            if (!WasChanged)
                return false;

            TrueOutRp.Value  = _currValueToApply;
            _cachedTrueValue = _currValueToApply;
            return true;
        }

        public bool Cancel()
        {
            if (!WasChanged)
                return false;

            //TrueOutRp.Value = _initValue;
            _currValueToApply = _cachedTrueValue;
            return true;
        }

    }

}
