using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ColonyShared.SharedCode.Utils
{
    public interface IAsyncChannel<TItem>
    {
        bool IsEmpty { get; }
        void Add(TItem item);
        ValueTask<TItem> Take(CancellationToken cancellationToken);
        bool TryTake(out TItem item);
    }

    public static class AsyncChannelExtensions
    {
        public static ValueTask<TItem> Take<TItem>(this IAsyncChannel<TItem> self)
        {
            return self.Take(CancellationToken.None);
        }
        
        public static async ValueTask<IEnumerable<TItem>> TakeAll<TItem>(this IAsyncChannel<TItem> self, CancellationToken cancellationToken)
        {
            var result = new List<TItem> {await self.Take(cancellationToken)};
            while (self.TryTake(out var r))
                result.Add(r);
//            while(!self.IsEmpty)
//                result.Add(await self.Take(cancellationToken));
            return result;
        } 
    } 
    
    /// <summary>
    /// Предназначен только для схемы "несколько продюсеров - ОДИН консъюмер".
    /// </summary>
    public class AsyncChannel<TItem> : IAsyncChannel<TItem>
    {
        private readonly IProducerConsumerCollection<TItem> _itemQueue = new ConcurrentQueue<TItem>();
        private readonly ConcurrentQueue<Awaiter> _awaiterQueue = new ConcurrentQueue<Awaiter>();
        private long _queueBalance = 0; // < 0 means there are free awaiters and not enough items. > 0 means the opposite case.
        private int _consume;

        public bool IsEmpty => Thread.VolatileRead(ref _queueBalance) <= 0;
        
        public void Add(TItem item)
        {
            while (!TryAdd(item)) {}
        }
        
        private bool TryAdd( TItem item )
        {
            var spin = new SpinWait();
            if ( Interlocked.Increment( ref _queueBalance ) > 0 )
            {
                while(!_itemQueue.TryAdd( item ))
                    spin.SpinOnce();
                return true;
            }
            else
            {
                Awaiter awaiter;
                while ( !_awaiterQueue.TryDequeue( out awaiter ) )
                    spin.SpinOnce();
                return awaiter.TrySetResult( item );
            }
        }

        public ValueTask<TItem> Take(CancellationToken cancellationToken)
        {
            try
            {
                ConsumerEnter();
                
                if (Interlocked.Decrement(ref _queueBalance) < 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Interlocked.Increment(ref _queueBalance);
                        return CanceledTask;
                    }

                    var awaiter = new Awaiter(cancellationToken);
                    _awaiterQueue.Enqueue(awaiter);
                    return awaiter.Task;
                }
                else
                {
                    TItem item;
                    var spin = new SpinWait();
                    while (!_itemQueue.TryTake(out item))
                        spin.SpinOnce();
                    return new ValueTask<TItem>(item);
                }
            }
            finally
            {
                ConsumerExit();
            }
        }

        public bool TryTake(out TItem item)
        {
            try
            {
                ConsumerEnter();

                if (!IsEmpty)
                {
                    Interlocked.Decrement(ref _queueBalance);
                    var spin = new SpinWait();
                    while (!_itemQueue.TryTake(out item))
                        spin.SpinOnce();
                    return true;
                }

                item = default;
            }
            finally
            {
                ConsumerExit();
            }
            return false;
        }

        private void ConsumerEnter()
        {
            if (Interlocked.Exchange(ref _consume, 1) != 0)
                throw new InvalidOperationException($"{nameof(AsyncChannel<TItem>)}: Multiple consume detected!");
        }

        private void ConsumerExit()
        {
            Thread.VolatileWrite(ref _consume, 0);
        }
 
        private class Awaiter
        {
            private readonly TaskCompletionSource<TItem> _completionSource;
            private readonly CancellationTokenRegistration _registration;

            public Awaiter( CancellationToken cancellationToken )
            {
                _completionSource = new TaskCompletionSource<TItem>();
                Task = new ValueTask<TItem>(TaskWithYield(_completionSource.Task));
                _registration = cancellationToken.Register(state => (state as TaskCompletionSource<TItem>).TrySetCanceled(), _completionSource, useSynchronizationContext: false );
            }

            public bool TrySetResult( TItem result )
            {
                _registration.Dispose();
                return _completionSource.TrySetResult( result );
            }

            public ValueTask<TItem> Task { get; }
            
            private static async Task<T> TaskWithYield<T>(Task<T> task )
            {
                var result = await task.ConfigureAwait( false );
                await System.Threading.Tasks.Task.Yield();
                return result;
            }
        }
        
        private static readonly ValueTask<TItem> CanceledTask = CreateCanceledTask();

        private static ValueTask<TItem> CreateCanceledTask()
        {
            var tcs = new TaskCompletionSource<TItem>();
            tcs.SetCanceled();
            return new ValueTask<TItem>( tcs.Task );
        }
    }
}