using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Transactions;

namespace GeneratedCode.Transactions
{
    public abstract class BaseTransaction<T>: ITransaction<T>
    {
        public Guid Id { get; } = Guid.NewGuid();

        protected IEntitiesRepository Repository;

        protected BaseTransaction(IEntitiesRepository repository)
        {
            Repository = repository;
        }

        public abstract Task<T> ExecuteTransaction();
    }
}
