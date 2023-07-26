using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using System;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Repositories;
using GeneratedCode.Manual.Repositories;
using Core.Cheats;
using SharedCode.Cloud;
using GeneratorAnnotations;

namespace GeneratedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client, addedByDefaultToNodeType: CloudNodeType.Server)]
    public interface ICheatRelayServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<ExecuteCheatResult> ExecuteCheatWithCheck(Guid target, string command, string[] args);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        Task<ExecuteCheatResult> ExecuteCheat(Guid target, string command, string[] args);
    }

    public partial class CheatRelayServiceEntity
    {
        public async Task<ExecuteCheatResult> ExecuteCheatWithCheckImpl(Guid target, string command, string[] args)
        {
            var methodInfo = CheatsManager.MatchCommand(command);
            if (methodInfo == null)
                return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatNotFound);

            var attr = methodInfo.CustomAttributes.FirstOrDefault(v => v.AttributeType == typeof(CheatRpcAttribute));
            if (attr == null)
                ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatNotFound, "Method is not marked as Cheat RPC");

            var requiredAccountType = (AccountType)attr.ConstructorArguments[0].Value;
            var accountType = await GetAccountType();

            if((accountType & (long)requiredAccountType) != (long)requiredAccountType)
                return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.NotAuthorized);

            return await ExecuteCheat(target, command, args);
        }

        public async Task<ExecuteCheatResult> ExecuteCheatImpl(Guid target, string command, string[] args)
        {
            var methodInfo = CheatsManager.MatchCommand(command);
            if (methodInfo == null)
                return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatNotFound);

            var type = methodInfo.DeclaringType;
            var typeId = ReplicaTypeRegistry.GetIdByType(type);

            if (target != default)
            {
                var resolverEntityAddress = await EntitiesRepository.GetAddressResolverServiceEntityId(typeId, target);
                Guid targetEntityRepositoryId;
                using (var addressResolverWrapper = await EntitiesRepository.Get<IClusterAddressResolverServiceEntityServer>(resolverEntityAddress))
                {
                    var resolver = addressResolverWrapper.Get<IClusterAddressResolverServiceEntityServer>(resolverEntityAddress);
                    targetEntityRepositoryId = await resolver.GetEntityAddressRepositoryId(typeId, target);
                    if (targetEntityRepositoryId == Guid.Empty)
                    {
                        return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.EntityNotFound);
                    }
                }

                if (targetEntityRepositoryId == Id)
                {
                    using (var targetEntityWrapper = await EntitiesRepository.Get(typeId, target))
                    {
                        var targetEntity = targetEntityWrapper.Get<IEntity>(typeId, target, ReplicationLevel.Master);
                        return await CheatsManager.ExecuteCommand(command, targetEntity);
                    }
                }
                else
                {
                    using (var cheatWrapper = await EntitiesRepository.Get<ICheatRelayServiceEntityServer>(targetEntityRepositoryId))
                    {
                        var targetCheatRelay = cheatWrapper.Get<ICheatRelayServiceEntityServer>(targetEntityRepositoryId);
                        return await targetCheatRelay.ExecuteCheat(target, command, args);
                    }
                }
            }

            if (Attribute.IsDefined(type, typeof(EntityServiceAttribute)))
            {
                using (var wrap = await EntitiesRepository.GetMasterService(type))
                {
                    if (wrap.TryGetMasterService(type, out IEntity entity))
                    {
                        return await CheatsManager.ExecuteParsed(methodInfo, args, entity);
                    }
                }

                Guid targetEntityRepositoryId = default;
                using (var wrap = await EntitiesRepository.GetFirstService(type))
                {
                    var entity = wrap.GetFirstService(type, ReplicationLevel.Always);
                    if (entity != null)
                        targetEntityRepositoryId = entity.OwnerRepositoryId;
                }

                if (targetEntityRepositoryId == default)
                {
                    var resolverEntityAddress = await EntitiesRepository.GetAddressResolverServiceEntityId(typeId, target);
                    if (resolverEntityAddress == Guid.Empty)
                        return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.EntityNotFound);
                    using (var addressResolverWrapper = await EntitiesRepository.Get<IClusterAddressResolverServiceEntityServer>(resolverEntityAddress))
                    {
                        var resolver = addressResolverWrapper.Get<IClusterAddressResolverServiceEntityServer>(resolverEntityAddress);
                        if (resolver != null)
                        {
                            var allKnownEntities = await resolver.GetAllEntitiesByType(typeId);
                            targetEntityRepositoryId = allKnownEntities.FirstOrDefault().repoId;
                        }
                    }
                }

                if (targetEntityRepositoryId != default)
                {
                    using (var cheatWrapper = await EntitiesRepository.Get<ICheatRelayServiceEntityServer>(targetEntityRepositoryId))
                    {
                        var targetCheatRelay = cheatWrapper.Get<ICheatRelayServiceEntityServer>(targetEntityRepositoryId);
                        return await targetCheatRelay.ExecuteCheat(target, command, args);
                    }
                }

                return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.EntityNotFound);
            }

            return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.ExecutionError, "Non-Service entity cheats with target inferring are not implemented");

        }

        private async ValueTask<long> GetAccountType()
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IAccountTypeServiceEntityServer>())
            {
                var accountTypeService = wrapper.GetFirstService<IAccountTypeServiceEntityServer>();
                return await accountTypeService.GetAccountType(CallbackRepositoryHolder.CurrentCallbackRepositoryId);
            }
        }
    }
}