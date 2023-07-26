using System;
using System.Collections.Generic;
using ReactivePropsNs;

namespace Uins
{
    public static class BindingControllerExtensions
    {
        public static IDisposable BindVM<T>(this BindingController<T> ctrl, ICollection<IDisposable> disposables, IStream<T> vmStream)
        {
            return vmStream.Subscribe(
                disposables,
                ctrl.SetVmodel,
                () => { ctrl.SetVmodel(default); }
            );
        }
    }
}