using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using L10n;
using ReactivePropsNs;
using Uins.Settings;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class BindingController<T> : BindingViewModel
    {
        //=== Props ===========================================================

        public ReactiveProperty<T> Vmodel { get; } = new ReactiveProperty<T>();


        //=== Public ==============================================================

        public void SetVmodel(T viewModel)
        {
            try
            {
                Vmodel.Value = viewModel;
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"{nameof(SetVmodel)}() Exception: {e.Message}\n{e.StackTrace}").Write();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Vmodel.Dispose();
        }
    }
}