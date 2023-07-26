using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountTypeServiceEntity
    {
        public Task<long> GetAccountTypeImpl(Guid userId)
        {
            if (userId == Guid.Empty)
                return Task.FromResult<long>(0);

            long result = 0;
            if (!AccountTypes.TryGetValue(userId, out result))
                Log.Logger.IfError()?.Message("Account type for user {0} not found", userId).Write();
            return Task.FromResult(result);
        }

        public Task SetAccountTypeImpl(Guid userId, long accountType)
        {
            if (userId == Guid.Empty)
                return Task.CompletedTask;

            AccountTypes[userId] = accountType;
            return Task.CompletedTask;
        }

        public Task RemoveAccountTypeImpl(Guid userId)
        {
            AccountTypes.Remove(userId);
            return Task.CompletedTask;
        }
    }
}
