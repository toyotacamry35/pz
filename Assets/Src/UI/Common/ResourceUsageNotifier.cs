using System.Linq;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ReactivePropsNs;

namespace Uins
{
    /// <summary>
    /// Следилка за использованием какого-либо UI ресурса, например, за использованием какой-то зоны показа сообщений
    /// </summary>
    public class ResourceUsageNotifier : IIsDisposed
    {
        private DictionaryStream<string, object> _dictionaryStream = new DictionaryStream<string, object>();

        private DisposableComposite _d = new DisposableComposite();


        //=== Props ===========================================================

        public IStream<bool> InUsage { get; }

        public bool IsDisposed { get; private set; }


        //=== Ctor ============================================================

        public ResourceUsageNotifier()
        {
            _d.Add(_dictionaryStream);
            InUsage = _dictionaryStream.CountStream.Func(_d, count => count > 0);
        }


        //=== Public ==========================================================

        public void Dispose()
        {
            IsDisposed = true;
            foreach (var disposable in _d)
                disposable.Dispose();
        }

        public void UsageRequest(string uniqueTag, object owner = null)
        {
            if (IsDisposed)
                return;

            if (_dictionaryStream.TryGetValue(uniqueTag, out var oldOwner))
            {
                UI.Logger.IfWarn()?.Message($"Key '{uniqueTag}' already exists. (Owners: old={oldOwner}, new={owner}").Write();
                return;
            }

            _dictionaryStream.Add(uniqueTag, owner);
        }

        public void UsageRevoke(string uniqueTag, object owner = null)
        {
            if (IsDisposed)
                return;

            if (!_dictionaryStream.ContainsKey(uniqueTag))
            {
                UI.Logger.IfWarn()?.Message($"Key '{uniqueTag}' isn't exists (owner={owner})").Write();
                return;
            }

            _dictionaryStream.Remove(uniqueTag);
        }

        public string GetUsageDebugInfo()
        {
            if (IsDisposed)
                return "";

            return _dictionaryStream.Select(kvp => $"[{kvp.Key}]={kvp.Value}").ItemsToString();
        }
    }
}