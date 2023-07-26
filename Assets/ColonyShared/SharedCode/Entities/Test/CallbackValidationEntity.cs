using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SharedCode.Entities.Test
{
    [GenerateDeltaObjectCode]
    public interface ICallbackValidationEntity : IEntity
    {
    }
}
namespace GeneratedCode.DeltaObjects
{
    public partial class CallbackValidationEntity : IHookOnReplicationLevelChanged //, IHookOnInit, IHookOnDatabaseLoad, IHookOnDestroy, IHookOnUnload, 
    {
        private static readonly Dictionary<CallbackValidationEntity, long> KnownReplicationMasks = new Dictionary<CallbackValidationEntity, long>();
        public static event Action<Exception> OnException;

        public Task OnDatabaseLoad()
        {
            throw new NotImplementedException();
        }

        public Task OnDestroy()
        {
            throw new NotImplementedException();
        }

        public Task OnInit()
        {
            throw new NotImplementedException();
        }

        public Task OnUnload()
        {
            throw new NotImplementedException();
        }


        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            try
            {
                lock (KnownReplicationMasks)
                {
                    KnownReplicationMasks.TryGetValue(this, out var existingMask);

                    if (existingMask != oldReplicationMask)
                        throw new InvalidOperationException($"RepLevel changed {oldReplicationMask}->{newReplicationMask}, but old value was {existingMask}");

                    KnownReplicationMasks[this] = newReplicationMask;
                }
            }
            catch(Exception e)
            {
                OnException?.Invoke(e);
                throw;
            }
        }
    }
}
