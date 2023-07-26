using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace GeneratedCode.Manual.Repositories
{
    public struct LogOperationInfo
    {
        public LogOperationInfo(LogOperationInfoOperationType logOperationInfoOperation, long batchId, long asyncContextId, string callerName, DateTime time, int read, int write, int usagesCount, object tag)
        {
            LogOperationInfoOperation = logOperationInfoOperation;
            BatchId = batchId;
            AsyncContextId = asyncContextId;
            CallerName = callerName;
            Time = time;
            Read = read;
            Write = write;
            UsagesCount = usagesCount;
            Tag = tag;
        }

        public LogOperationInfoOperationType LogOperationInfoOperation { get; }

        public string CallerName { get; }

        public object Tag { get; }

        public DateTime Time { get; }

        public int Read { get; }

        public int Write { get; }

        public int UsagesCount { get; }

        public long BatchId { get; }

        public long AsyncContextId { get; }
    }

    public enum LogOperationInfoOperationType
    {
        None,
        UpRead,
        UpWrite,
        UpFromReadToExclusive,
        DownRead,
        DownWrite,
        DownFromReadToExclusive,
    }
}
