using System;
using System.Collections.Generic;

namespace ReactivePropsNs
{
    public static class LastExtension
    {
        public static IReactiveProperty<T> Last<T>(
            this IStream<T> source,
            ICollection<IDisposable> disposables,
            bool reactOnChangesOnly = true)
        {
            ReactiveProperty<T> output = null;
            output = new ReactiveProperty<T>(reactOnChangesOnly: reactOnChangesOnly,
                prefix =>
                    $"{source.GetType().NiceName()}.Last({(reactOnChangesOnly ? "" : "reactOnChangesOnly: false")}) " +
                    $"// {(output.IsDisposed ? "DISPOSED" : (!output.HasValue ? "<HasNoValue>" : output.Value == null ? "<null>" : output.Value.ToString())) + "\n" + (source.DeepLog(prefix + "\n"))}"
            );
            disposables.Add(output);
            disposables.Add(source.Action(disposables, t => output.Value = t));
            return output;
        }
    }
}