using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;

namespace GeneratedCode.Shared.Utils
{
    public static class AccountTypeUtils
    {
        public static async Task<bool> CheckAccountType(Guid userId, AccountType accountType, IEntitiesRepository repository, [CallerMemberName] string callerTag = null)
        {
            if (repository.CloudNodeType == CloudNodeType.Client || userId == default)
                return true;

            using(var wrap = await repository.Get<IRepositoryCommunicationEntityAlways>(userId))
            {
                var entity = wrap.Get<IRepositoryCommunicationEntityAlways>(userId);
                if (entity.CloudNodeType == CloudNodeType.Server)
                    return true;
            }

            var getTask = repository.GetFirstService<IAccountTypeServiceEntityServer>();
            var wrapper = getTask.IsCompleted ? getTask.Result : await getTask;
            using (wrapper)
            {
                var accountTypeService = wrapper?.GetFirstService<IAccountTypeServiceEntityServer>();
                if (accountTypeService != null)
                {
                    var resultTask = accountTypeService.GetAccountType(userId);

                    var userAccountType = resultTask.IsCompleted ? resultTask.Result : await resultTask;
                    return (userAccountType & (long) accountType) == (long)accountType;
                }
            }
            return false;
        }
    }
}
