using System;

namespace ReactivePropsNs {
    public static class StreamStateExtention {
        /// <summary> Использовать строго только для логирования, потому как тратит ресурсы не считая </summary>
        public static string StreamState<T>(this IStream<T> source, Func<T, string> toString = null) {
            if (source == null)
                return $"<null IStream<{typeof(T).NiceName()}>>";
            if (source is IIsDisposed disposed && disposed.IsDisposed)
                return $"<disposed IStream<{source.GetType().NiceName()}>>";
            if (source is IReactiveProperty<T> property) {
                if (!property.HasValue)
                    return "<HasNoValue>";
                else if (property.Value == null)
                    return "<null>";
                else if (toString != null)
                    return toString(property.Value);
                else
                    return property.Value.ToString();
            } else {
                T value = default;
                bool hasValue = false, completed = false;
                var listener = new ActionBasedListener<T>(t => { hasValue = true; value = t; }, () => completed = true);
                var subscibtion = source.Subscribe(listener);
                string msg = completed ? "<completed>"
                                : !hasValue ? "<hasNoValue>"
                                : value == null ? "<null>"
                                : toString != null ? toString(value)
                                : value.ToString();
                subscibtion.Dispose();
                listener.Dispose();
                return msg;
            }
        }
    }
}
