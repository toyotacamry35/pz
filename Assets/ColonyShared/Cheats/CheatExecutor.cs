using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Cheats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ShareCode.Threading;
using SharedCode.EntitySystem;
using SharedCode.Repositories;

namespace Assets.Src.Cluster.Cheats
{
    public static class CheatExecutor
    {
        public static async ValueTask<ExecuteCheatResult> Execute(IEntitiesRepository repository, Guid target, string command)
        {
            (var parseResult, var methodInfo, var args) = CheatsManager.ParseCommand(command);
            if (!parseResult.IsSuccess())
                return parseResult;

            if (methodInfo.IsStatic)
            {
                var cheatResult = await CheatsManager.ExecuteParsed(methodInfo, args);
                return cheatResult;
            }

            await Awaiters.ThreadPool;

            using (var cheatWrapper = await repository.GetMasterService<ICheatRelayServiceEntityClientBroadcast>())
            {
                if(cheatWrapper.TryGetMasterService<ICheatRelayServiceEntityClientBroadcast>(out var targetCheatRelay))
                    return await targetCheatRelay.ExecuteCheatWithCheck(target, methodInfo.Name, args);
            }

            using (var cheatWrapper = await repository.GetFirstService<ICheatRelayServiceEntityClientBroadcast>())
            {
                var targetCheatRelay = cheatWrapper.GetFirstService<ICheatRelayServiceEntityClientBroadcast>();
                return await targetCheatRelay.ExecuteCheatWithCheck(target, methodInfo.Name, args);
            }
        }

        public static async ValueTask<ExecuteCheatResult> ExecuteParsedNoCheck(IEntitiesRepository repository, TaskScheduler scheduler, Guid target, string command, string[] args)
        {
            var methodInfo = CheatsManager.MatchCommand(command);
            if (methodInfo == null)
                return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatNotFound);

            if (methodInfo.IsStatic)
            {
                var cheatResult = await Task.Factory.StartNew(
                    () => CheatsManager.ExecuteParsed(methodInfo, args).AsTask(),
                    CancellationToken.None,
                    TaskCreationOptions.RunContinuationsAsynchronously,
                    scheduler).Unwrap();
                return cheatResult;
            }

            var type = methodInfo.DeclaringType;
            var typeId = ReplicaTypeRegistry.GetIdByType(type);

            using (var cheatWrapper = await repository.GetMasterService<ICheatRelayServiceEntityServer>())
            {
                if (cheatWrapper.TryGetMasterService<ICheatRelayServiceEntityServer>(out var targetCheatRelay))
                    return await targetCheatRelay.ExecuteCheat(target, methodInfo.Name, args);
            }

            using (var cheatWrapper = await repository.GetFirstService<ICheatRelayServiceEntityServer>())
            {
                var targetCheatRelay = cheatWrapper.GetFirstService<ICheatRelayServiceEntityServer>();
                return await targetCheatRelay.ExecuteCheat(target, methodInfo.Name, args);
            }
        }
    }
}