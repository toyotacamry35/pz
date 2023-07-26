using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Refs.Operations
{
    public class CompleteTaskAfterProcessingOperation : BaseEntityRefOperation
    {
        private TaskCompletionSource<bool> _tcs;

        public CompleteTaskAfterProcessingOperation(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        public override bool Do(IEntityRef entityRef, out EntityRefOperationResult? result)
        {
            result = null;
            _tcs.SetResult(true);
            return true;
        }
    }
}
