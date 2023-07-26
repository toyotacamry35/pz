using System;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Utils.Threads;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlerInputWindow : IInputActionTriggerHandler, IInputActionValueHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly long _actionTime;
        private readonly int _bindingId;
        private readonly Action<InputActionState, int> _loopback;
        private readonly CancellationTokenSource _cancellation;
        private Task _queue = Task.CompletedTask;
        private int _jobsCount;
        private bool _passThrough;
        private bool _disposed;

        public InputActionHandlerInputWindow(long actionTime, int bindingId, Action<InputActionState,int> loopback)
        {
            _bindingId = bindingId;
            _loopback = loopback ?? throw new ArgumentNullException(nameof(loopback));
            _actionTime = actionTime > 0 ? actionTime : throw new ArgumentException($"Invalid actionTime: {actionTime}", nameof(actionTime));

            var delay = (int) (_actionTime - SyncTime.Now);
            if (delay > 0)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"With delay | Binding:#{_bindingId} Delay:{delay}").Write();
                var cancellationToken = (_cancellation = new CancellationTokenSource()).Token;
                var delayTask = TaskEx.Run(async () => { await Task.Delay(delay, cancellationToken); }, cancellationToken);
                if(Logger.IsTraceEnabled)
                {
                    delayTask.ContinueWith(t => Logger.IfTrace()?.Message($"Delay completed | Binding:#{_bindingId} ActionTime:{_actionTime} Now:{SyncTime.Now} Remains:{_actionTime - SyncTime.Now}").Write(), TaskContinuationOptions.NotOnCanceled);
                    delayTask.ContinueWith(t => Logger.IfTrace()?.Message($"Delay cancelled | Binding:#{_bindingId} ActionTime:{_actionTime} Now:{SyncTime.Now} Remains:{_actionTime - SyncTime.Now}").Write(), TaskContinuationOptions.OnlyOnCanceled);
                }
                _queue = delayTask;
            }
            else
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Without delay | Binding:#{_bindingId} Delay:{delay}").Write();
        }

        public bool PassThrough => false;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            ProcessEvent(new InputActionState(@event));
        }

        public void ProcessEvent(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive)
        {
            ProcessEvent(new InputActionState(@event));
        }

        private void ProcessEvent(InputActionState @event)
        {
            if(_disposed) throw new ObjectDisposedException(nameof(InputActionHandlerInputWindow));
            // Если таск ожидания (первый в очереди) будет cancelled, то вся накопившееся очередь не должна выполняться.
            // Но если таск ожидания завершился, то вся очередь должна выполнится полностью
            var jobId = _jobsCount++;
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Queue job #{jobId.ToString()} | Binding:#{_bindingId} Event:{@event.ToString()}").Write();
            _queue = _queue.ContinueWith(t => {
                try
                {
                    if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Executing job #{jobId.ToString()} | Binding:#{_bindingId} Event:{@event.ToString()}").Write();
                    _loopback(@event, _bindingId);
                } catch (Exception e) { Logger.IfError()?.Exception(e).Write(); } 
            }, TaskContinuationOptions.NotOnCanceled);
        }

        public void Dispose()
        {
            if(_disposed) throw new ObjectDisposedException(nameof(InputActionHandlerInputWindow));
            _disposed = true;   
            if (_cancellation != null)
            {
                _cancellation.Cancel();
                _cancellation.Dispose();
            }
            if (Logger.IsTraceEnabled) _queue.ContinueWith(t => Logger.IfTrace()?.Message($"All jobs complete | Binding:#{_bindingId}").Write());
        }

        public override string ToString() => $"{nameof(InputActionHandlerInputWindow)}(Time:{_actionTime.ToString()})";
    }
}