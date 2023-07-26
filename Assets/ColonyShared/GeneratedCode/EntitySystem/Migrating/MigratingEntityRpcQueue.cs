using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.EntitySystem.Migrating
{
    public class MigratingEntityRpcQueue
    {
        private ConcurrentQueue<byte[]> _defferedRpc = new ConcurrentQueue<byte[]>();

        public void AddDefferedRpc(byte[] data)
        {
            _defferedRpc.Enqueue(data);
        }

        public IEnumerable<byte[]> TakeAll()
        {
            var defferedRpcCopy = _defferedRpc;
            _defferedRpc = null;
            return defferedRpcCopy;
        }
    }
}
