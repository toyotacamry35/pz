using System;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;

namespace Uins.Settings
{
    public class SettingSwitcherFloatVM : SettingSwitcherImplementationBase<float>
    {
        internal SettingSwitcherFloatVM(Definition definition, [NotNull] IApplyableCancelableProxy<float> proxy) : base(definition, proxy)
        {}

        // --- Util types: ------------------------
        internal class Definition : SettingSwitcherDefinitionBase<float>
        {}
    }

    internal interface ISettingSwitcherDefinition
    {
    }

    internal class SettingSwitcherDefinitionBase<T> : ISettingSwitcherDefinition
    {
        internal T DfltVal;
        internal T MaxVal;
        internal IStream<T> InStream;
        internal ReactiveProperty<T> OutRp;
        // Convert real data value to VM-value (convenient to be used by Controller (f.e. volume (0-100) it converts to ratio (0.0-1.0) for slider underhood expected value range)
        internal Func<T,T> StreamToValTransformer;
        internal Func<T, (bool setValue, T value)> ValToRpTransformer;
        /// <summary>
        /// Should setting change controlled value instantly when user changes it in UI, or should do it only when "Apply" button clicked.
        /// F.e. should be `true` for MusicVolume, `false` for resolution. 
        /// </summary>
        internal bool AppliedInstantly;
        // Probably you should use always only one of these two (dep.on is localization needed (no need f.e. for "1280 x 800"):
        internal Func<T, string> ValToStringConverter;
        internal Func<T, LocalizedString> ValToLocalizedStringConverter;

        public override string ToString()
        {
            return $"T:{typeof(T)}, DfltVal:{DfltVal}, MaxVal:{MaxVal}";
        }
    }
    

    internal interface ISettingSwitcherImplementationBase<T>
    {
        ReactiveProperty<T> DfltVal { get; set; }
        ReactiveProperty<T> MaxVal  { get; set; }
        ReactiveProperty<T> InStreamForCtrl { get; set; }
        ReactiveProperty<T> OutRpForCtrl    { get; set; }
        ReactiveProperty<Func<T, T>> StreamToValTransformer                    { get; set; }
        ReactiveProperty<Func<T, (bool isValueSeted, T setedValue)>> ValToRpTransformer { get; set; }
    }
    public abstract class SettingSwitcherImplementationBase<T> : BindingVmodel, ISettingSwitcherImplementationBase<T>, IApplyableCancelableDefaultableCanBeChanged
    {
        public ReactiveProperty<T> DfltVal { get; set; } = new ReactiveProperty<T>();
        public ReactiveProperty<T> MaxVal  { get; set; } = new ReactiveProperty<T>();
        protected IStream<T> TrueInStream       { get; set; }
        protected ReactiveProperty<T> TrueOutRp { get; set; }
        /// <summary>
        /// Отсюда Ctrl слушает якобы** входящий стрим.  **) - или не якобы - зависит от того, что именно в кач-ве этого Rp предоставляет ему провайдер (VM).
        /// </summary>
        public ReactiveProperty<T> InStreamForCtrl { get; set; } = new ReactiveProperty<T>();
        /// <summary>
        /// Сюда Ctrl должен вещать свои намерения
        /// </summary>
        public ReactiveProperty<T> OutRpForCtrl    { get; set; } = new ReactiveProperty<T>();

        /// <summary>
        /// Эта пара transformer'ов применяется к in- и out- стримам контроллера (`InStreamForCtrl` & `OutRpForCtrl`) на стыке уже с 2WayPropController'ом, после
        ///   кот-го уже только Weld.  Их действия ожидаются достаточно симметричны, чтобы не принимать их во внимание, если работа с Weld'ом не попадает в фокус
        ///   подзадачи. (Так напр., кухня внутри proxy VM'а никак не затронута этими tranformer'ами).
        ///   They are f.e. Val-to_ratio & backwards funcs used for volume sliders (where Re data is in [0 .. 100], but slider range is [0.0 .. 1.0]
        /// </summary>
        public ReactiveProperty<Func<T, T>> StreamToValTransformer                    { get; set; } = new ReactiveProperty<Func<T, T>>();
        /// <summary>
        /// Func returns tuple (bool isValueSeted, T setedValue): `isValueSeted` - was passed value setted inside,  `setedValue` - what value was set inside (matters only if `isValueSeted` == true)
        /// </summary>
        public ReactiveProperty<Func<T, (bool isValueSeted, T setedValue)>> ValToRpTransformer { get; set; } = new ReactiveProperty<Func<T, (bool isValueSeted, T setedValue)>>();

        // Probably you should use always only one of these two (dep.on is localization needed (no need f.e. for "1280 x 800"):
        public Func<T, string> ValToStringConverter;
        public Func<T, LocalizedString> ValToLocalizedStringConverter;

        protected readonly IApplyableCancelableProxy<T> Proxy;

        internal SettingSwitcherImplementationBase(SettingSwitcherDefinitionBase<T> definition, [NotNull] IApplyableCancelableProxy<T> proxy)
        {
            DfltVal.Value = definition.DfltVal;
            MaxVal.Value  = definition.MaxVal;
            TrueInStream = definition.InStream;
            TrueOutRp    = definition.OutRp;
            StreamToValTransformer.Value = definition.StreamToValTransformer;
            ValToRpTransformer.Value     = definition.ValToRpTransformer;
            ValToStringConverter          = definition.ValToStringConverter;
            ValToLocalizedStringConverter = definition.ValToLocalizedStringConverter;

            // Proxy:
            Proxy = proxy;
            Proxy.Init(TrueInStream, TrueOutRp, InStreamForCtrl, OutRpForCtrl, D);
        }

        // --- IApplyableCancelable: ---------------------------------

        public bool WasChanged => Proxy.WasChanged; 
        public bool Apply() => Proxy.Apply();
        public bool Cancel() => Proxy.Cancel();
        public bool SetToDefault()
        {
            //#wrong: if (!OutRpForCtrl.HasValue || !DfltVal.HasValue)
            if (!DfltVal.HasValue)
                return false;

            OutRpForCtrl.Value = DfltVal.Value;
            return true;
        }

    }

}
