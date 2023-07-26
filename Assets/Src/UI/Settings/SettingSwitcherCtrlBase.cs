using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;
// ReSharper disable InconsistentNaming

namespace Uins.Settings
{
    [Binding]
    public abstract class SettingSwitcherCtrlBase<TVM,TVal> : BindingController<TVM>, ISettingSwitcherCtrlBase where TVM : SettingSwitcherImplementationBase<TVal>
    {
        [UsedImplicitly] //is set via reflection by Weld inside .Bind
        [Binding]  public TVal DfltVal  { get; protected set; }

        private TwoWayPropController<TVal, SettingSwitcherCtrlBase_With2WayPropCtrl<TVM, TVal>> _2WayPropController;

        // protected TVal CurrInStreamVal;//BindingProp
        //[Binding] protected TVal CurrInStreamVal { get; set; }
        protected TVal CurrInStreamVal;

        [SerializeField, UsedImplicitly]  private Transform _representerRoot;
        [SerializeField, UsedImplicitly]  private ValAsStringRepresenterCtrl _asStringRepresenter;
        [SerializeField, UsedImplicitly]  private ValAsLocalizedStringRepresenterCtrl _asLocalizedStringRepresenter;
        private ValRepresenterCtrlBase _instantiatedRepresenter;


        //=== Unity ===========================================================

        [UsedImplicitly] //U
        private void Awake()
        {
            var dfltValStream = Vmodel.SubStream(D, vm => vm.DfltVal);
            
            var inSubStream = Vmodel.SubStream(D, vm => vm.InStreamForCtrl);
            var transformedInStream = inSubStream.Func(D, val =>
            {
                if (!Vmodel.HasValue)
                        return default;
            
                if ( Vmodel.Value.StreamToValTransformer.HasValue 
                  && Vmodel.Value.StreamToValTransformer.Value != null )
                    return Vmodel.Value.StreamToValTransformer.Value(val);
                else
                    return val;
            });

            //Bind(transformedInStream, () => CurrInStreamVal);
            transformedInStream.Action(D, val => CurrInStreamVal = val);
            Bind(dfltValStream, () => DfltVal);
            
            // Action, where Instantiates whether: A) LS-representer or B) not localized string-representer:
            inSubStream.Zip(D, Vmodel)
                .Action(D, (val, vm) =>
                {
                        //if (DbgLog.Enabled) DbgLog.Log($"SeSwCtBa: inSubStream.Zip: ({val}, vm:{vm})");

                        // All is done already:
                        if (_instantiatedRepresenter != null)
                            return;

                        // No all needed for representation data - so do nothing:
                        if (vm == null || !Vmodel.HasValue 
                                       || (Vmodel.Value.ValToLocalizedStringConverter == null && Vmodel.Value.ValToStringConverter == null))
                        {
                            //if (_instantiatedRepresenter != null)
                            //    UnityEngine.Object.Destroy(_instantiatedRepresenter);
                            return;
                        }

                        //if (DbgLog.Enabled) DbgLog.Log("---------------------------------- Vmodel.Action( ... vm != null && we 've not-null at least 1 converter-to-representer ");

                        if (Vmodel.Value.ValToLocalizedStringConverter != null)
                        {
                            // A. Instantiate (most A-priority) LS-representer:

                            var representer = Instantiate(_asLocalizedStringRepresenter, _representerRoot);
                            var inSubStream_convertedToLocalizedString = inSubStream.Func(D, x =>
                            {
                                if (!Vmodel.HasValue || Vmodel.Value.ValToLocalizedStringConverter == null)
                                    return LsExtensions.Empty;

                                return Vmodel.Value.ValToLocalizedStringConverter.Invoke(x);
                            });
                            representer.Init(inSubStream_convertedToLocalizedString);
                            _instantiatedRepresenter = representer;
                            //if (DbgLog.Enabled) DbgLog.Log("---------------------------------- toLS A-representer inst-ed");
                        }
                        else
                        {
                            // B. Instantiate (less B-priority) not localized string-representer:
                            
                            System.Diagnostics.Debug.Assert(Vmodel.Value.ValToStringConverter != null);
                            var representer = Instantiate(_asStringRepresenter, _representerRoot);
                            var inSubStream_convertedToString = inSubStream.Func(D, x =>
                            {
                                if (!Vmodel.HasValue || Vmodel.Value.ValToStringConverter == null)
                                    return string.Empty; //return $"!Vmodel.HasValue(1) : {{{!Vmodel.HasValue}}}";

                                return Vmodel.Value.ValToStringConverter.Invoke(x);
                            });
                            representer.Init(inSubStream_convertedToString);
                            _instantiatedRepresenter = representer;
                            // if (DbgLog.Enabled) DbgLog.Log("---------------------------------- to-string B-representer inst-ed");
                        }
                        // else
                        // {
                        //      #TODO: Если понадобится дефолтный representer по типу TVal, то сюда дописать:
                        //       - доастать из dic-ря какого-то префаб по мапине TVal - префаб.
                        //       - Контроллеры (т.к. vm отдельно не нужно, то наследники BindingViewModel) должды быть с generic i-face'ом от TVal и i-face'ным методом Init(IStream<TVal> )
                        //       - зовём ему Init, и кормим стримом значений. Всё вроде.
                        // }

                    });

            AwakeInternal();
        }

        protected virtual void AwakeInternal()
        {
        }


        // --- IApplyableCancelable: ---------------------------------

        public bool WasChanged
        {
            get
            {
                if (!Vmodel.HasValue || Vmodel.Value == null)
                    return false;
                return Vmodel.Value.WasChanged;
            }
        }

        public virtual bool Apply()
        {
            if (!Vmodel.HasValue || Vmodel.Value == null)
                return false;
            return Vmodel.Value.Apply();
        }
        
        public virtual bool Cancel()
        {
            if (!Vmodel.HasValue || Vmodel.Value == null)
                return false;
            return Vmodel.Value.Cancel();
        }
        
        public virtual bool SetToDefault()
        {
            if (!Vmodel.HasValue || Vmodel.Value == null)
                return false;
        
            return Vmodel.Value.SetToDefault();
        }

        // --- Protected: -------------- -------------------
        protected void SetToOutRpForCtrl(TVal newValue)
        {
            if (!Vmodel.HasValue || Vmodel.Value == null)
            {
                //if (DbgLog.Enabled) DbgLog.Log($"SeSwCtBa: break callback, 'cos (!Vmodel.HasValue || Vmodel.Value == null): {Vmodel.HasValue}");
                return;
            }
            var vm = Vmodel.Value;
            if (vm.ValToRpTransformer.HasValue && vm.ValToRpTransformer.Value != null)
            {
                var transformedOut = vm.ValToRpTransformer.Value(newValue);
                if (transformedOut.isValueSeted)
                    vm.OutRpForCtrl.Value = transformedOut.setedValue;
            }
            else
            {
                //if (DbgLog.Enabled) DbgLog.Log($"SeSwCtBa: _2WPropCtrl _outCalbck: outRp:{vm.OutRpForCtrl.Value} :== newVal:{newValue}");
                vm.OutRpForCtrl.Value = newValue;
            }
        }

    }

}
