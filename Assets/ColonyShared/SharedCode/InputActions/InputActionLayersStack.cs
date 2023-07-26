using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;
using static ColonyShared.SharedCode.Utils.EqualityComparerFactory;

namespace ColonyShared.SharedCode.InputActions
{
    public partial class InputActionLayersStack : IInputActionLayersStack, IInputActionBindingsSource, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(InputActionLayersStack));

        private readonly List<(InputActionHandlersLayerDef LayerDef, List<Layer> Layers)> _layers = new List<(InputActionHandlersLayerDef, List<Layer>)>(); // слои в порядке повышения приоритета
        private readonly Dictionary<InputActionDef, HandlersStackHolder> _stacks = new Dictionary<InputActionDef, HandlersStackHolder>();
        private readonly ReactiveProperty<InputActionBindingsStream[]> _bindings;
        private readonly ReactiveProperty<bool> _bindingsWait = new ReactiveProperty<bool>(false);
        private readonly DisposableComposite _disposables = new DisposableComposite();
        private readonly Func<int> _bindingIdGenerator;
        private readonly object _queueLock = new object();
        private Task _queueTail = Task.CompletedTask;
        private CancellationTokenSource _gatherCancellation;
        private Guid _entityId;
        private bool _isMaster;
        private int _bindingCounter;
        private int _jobCounter;
        private int _disposed;
        private readonly GatherDelegate _gatherDelegate;

        public InputActionLayersStack()
        {
            _bindingIdGenerator = () => Interlocked.Increment(ref _bindingCounter);
            _gatherDelegate = Gather;
            _bindings = new ReactiveProperty<InputActionBindingsStream[]>(CreateForEnumerable<InputActionBindingsStream>());
            _disposables.Add(_bindings);
            _disposables.Add(_bindingsWait);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
                return;

            lock (_queueLock)
            {
                _gatherCancellation?.Cancel();
                _queueTail.ContinueWith(x =>
                {
                    _bindingsWait.Value = false;
                    _disposables.Dispose();
                    _gatherCancellation?.Dispose();
                });
            }
        }

        public Guid EntityId { set => _entityId = value; }

        public bool IsMaster { set => _isMaster = value; }

        public IStream<IEnumerable<InputActionBindingsStream>> Bindings()
        {
            if (IsDisposed)
                return Stream<IEnumerable<InputActionBindingsStream>>.Empty;
            return _bindings;
        }

        public IStream<bool> BindingsWait()
        {
            if (IsDisposed)
                return Stream<bool>.Empty;
            return _bindingsWait;
        }
        
        public void PushLayer(object layerOwner, InputActionHandlersLayerDef def, Action<IInputActionLayer> initializer = null)
        {
            if (layerOwner == null) throw new ArgumentNullException(nameof(layerOwner));
            if (def == null) throw new ArgumentNullException(nameof(def));

            EnqueueJob(Logger.IsTraceEnabled ? $"Push layer | Owner:{layerOwner} Def:{def}" : null, () =>
                {
                    var layer = new Layer(layerOwner, _isMaster, _entityId, _bindingIdGenerator);
                    int insertIdx = 0;
                    for (; insertIdx <= _layers.Count; ++insertIdx)
                    {
                        if (insertIdx == _layers.Count || _layers[insertIdx].LayerDef.Priority > def.Priority)
                        {
                            _layers.Insert(insertIdx, (def, new List<Layer> {layer}));
                            break;
                        }
                        if (_layers[insertIdx].LayerDef == def)
                        {
                            _layers[insertIdx].Layers.Add(layer);
                            break;
                        }
                    }

                    if (initializer != null)
                    {
                        try
                        {
                            layer.Unlock();
                            initializer.Invoke(layer);
                        }
                        finally
                        {
                            layer.Lock();
                        }
                    }
                }
            );
        }

        public void ModifyLayer(object layerOwner, InputActionHandlersLayerDef def, Action<IInputActionLayer> modifier)
        {
            if (layerOwner == null) throw new ArgumentNullException(nameof(layerOwner));
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (modifier == null) throw new ArgumentNullException(nameof(modifier));

            EnqueueJob(Logger.IsTraceEnabled ? $"Modify layer | Owner:{layerOwner}" : null, () =>
                {
                    foreach (var (layerDef, layers) in _layers)
                        if (layerDef == def)
                            for (int i = layers.Count - 1; i >= 0; --i)
                            {
                                var layer = layers[i];
                                if (layer.Owner.Equals(layerOwner))
                                {
                                    try
                                    {
                                        layer.Unlock();
                                        modifier.Invoke(layer);
                                    }
                                    finally
                                    {
                                        layer.Lock();
                                    }
                                    return;
                                }
                            }
                    Logger.IfError()?.Message($"{Mark} | No layer for modification | Layer:{def.____GetDebugAddress()} Owner:{layerOwner}").Write();
                }
            );
        }

        public void DeleteLayer(object layerOwner, InputActionHandlersLayerDef def)
        {
            if (layerOwner == null) throw new ArgumentNullException(nameof(layerOwner));
            if (def == null) throw new ArgumentNullException(nameof(def));

            EnqueueJob(Logger.IsTraceEnabled ? $"Delete layer | Owner:{layerOwner}" : null, () =>
                _layers.RemoveAll(x =>
                {
                    if (x.LayerDef == def)
                    {
                        if (x.Layers.RemoveAll(y => y.Owner.Equals(layerOwner)) == 0)
                            Logger.IfError()?.Message($"{Mark} | No layer for removing | Layer:{def.____GetDebugAddress()} Owner:{layerOwner}").Write();
                        return x.Layers.Count == 0;
                    }
                    return false;
                })
            );
        }

        private void EnqueueJob(string title, Action job)
        {
            if (IsDisposed)
                return;

            _bindingsWait.Value = true;
            
            lock (_queueLock)
            {
                _gatherCancellation?.Cancel();
                _gatherCancellation?.Dispose();
                _gatherCancellation = new CancellationTokenSource();
                var cancellationToken = _gatherCancellation.Token;
                var jobId = ++_jobCounter;
                _queueTail = _queueTail.ContinueWith(t =>
                    {
                        try
                        {
                            if (title != null) Logger.IfTrace()?.Message($"{Mark} | Start #{jobId} | {title}").Write();
                            job();
                            if (title != null) Logger.IfTrace()?.Message($"{Mark} | Finish #{jobId} | {title}").Write();
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Exception(e).Write();
                        }
                    }, TaskContinuationOptions.RunContinuationsAsynchronously)
                    .ContinueWith(t => {
                        try
                        {
                            GatherBindings(cancellationToken, jobId);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        
                        bool done = false;
                        do
                        {
                            try
                            {
                                _bindingsWait.Value = false;
                                done = true;
                            }
                            catch (TimeoutException) {}
                        } while (!done);
                    }, cancellationToken, TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);

                if (Logger.IsTraceEnabled)
                    _queueTail = _queueTail.ContinueWith(_ => Logger.IfTrace()?.Message($"{Mark} | Gather bindings #{jobId} was cancelled").Write(), TaskContinuationOptions.OnlyOnCanceled);
            }
        }

        private bool IsDisposed => _disposed != 0;
        
        private string Mark => _isMaster ? "Master" : "Client"; 
 
        private void Gather(in InputActionBinding binding)
        {
            if (!_stacks.TryGetValue(binding.Action, out var holder))
                _stacks.Add(binding.Action, holder = new HandlersStackHolder(_disposables));
            ++holder.Count;
            if (holder.Stack.Count < holder.Count || holder.Stack[holder.Count - 1].Id != binding.Id)
            {
                holder.Stack.Insert(holder.Count - 1, binding);
                holder.Modified = true;
            }
        }
        
        private void GatherBindings(CancellationToken cancellationToken, int gatherId)
        {
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{Mark} | Gather bindings #{gatherId} started").Write();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var holder in _stacks.Values)
                holder.Count = 0;
            
            for (int i = _layers.Count - 1; i >= 0; --i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var layerStack = _layers[i];
                var layer = layerStack.Layers.Last();
                layer.GatherHandlers(_gatherDelegate);
            }

            foreach (var holder in _stacks.Values)
            {
                cancellationToken.ThrowIfCancellationRequested();
                for (int i = holder.Stack.Count - 1; i >= holder.Count; --i)
                {
                    holder.Stack.RemoveAt(i);
                    holder.Modified = true;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            _bindings.Value = _stacks.Select(x => new InputActionBindingsStream(x.Key, x.Value.Stream)).ToArray();

            foreach (var holder in _stacks.Values)
            {
                if (holder.Modified)
                {
                    holder.Stream.OnNext(holder.Stack);
                    holder.Modified = false;
                }
            }
            
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{Mark} | Gather bindings #{gatherId} finished").Write();
        }

        private class HandlersStackHolder
        {
            public readonly List<InputActionBinding> Stack = new List<InputActionBinding>();
            public readonly StreamProxy<IEnumerable<InputActionBinding>> Stream;
            public int Count;
            public bool Modified;

            public HandlersStackHolder(IDisposableCollection disposables)
            {
                Stream = new StreamProxy<IEnumerable<InputActionBinding>>(lstnr => lstnr.OnNext(Stack /* tuple.Stack.ToArray() */));
                disposables.Add(Stream);
            }
        }
        
        private delegate void GatherDelegate(in InputActionBinding t);
    }
}