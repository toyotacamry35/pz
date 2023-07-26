using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Transactions
{
    public enum TransactionStatus
    {
        None,
        ProcessPrepare,
        SuccessPrepare,
        FailPrepare,
        ProcessCommit,
        SuccessCommit,
        FailCommit,
        ProcessRollback,
        SuccessRollback,
        FailRollback,
        Success,
        Rollback,
        Fail
    }

    public interface ITransaction<T>
    {
        Guid Id { get; }

        Task<T> ExecuteTransaction();
    }

    public interface ITransactionUnit
    {
        Task<bool> Prepare();

        Task<bool> Commit();

        Task<bool> Rollback();
    }
}
