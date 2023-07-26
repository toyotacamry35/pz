using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using NLog;
using SharedCode.Entities.Core;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class EntityMethodsCallsChain
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        string _cacheDescription = null;

        public static EntityMethodsCallsChain CreateFromBatch(EntityMethodsCallsChainBatch batch)
        {
            var result = new EntityMethodsCallsChain();
            result.Id = batch.Id;
            result.ChainContext = new ChainContext();
            result.NextTimeToCall = DateTime.UtcNow.ToUnix();
            result.Chain = new DeltaList<ChainBlockBase>(batch.Chain);
            return result;
        }

        public Task<ChainBlockBase> GetCurrentChainBlockImpl()
        {
            var result = CurrentChainIndex < Chain.Count ? Chain[CurrentChainIndex] : null;
            return Task.FromResult(result);
        }

        public Task<NextEntityToCallResult> TryGetNextEntityToCallImpl()
        {
            for (int i = CurrentChainIndex; i < Chain.Count; i++)
                if (Chain[i] is ChainBlockCall)
                {
                    var callBlock = (ChainBlockCall) Chain[i];
                    return Task.FromResult(new NextEntityToCallResult
                    {
                        Result = true,
                        TypeID = callBlock.ObjectAddress.EntityTypeId,
                        EntityId = callBlock.ObjectAddress.EntityId
                    });
                }

            return Task.FromResult(new NextEntityToCallResult
            {
                Result = false
            });
        }

        public async Task<IEntityMethodsCallsChain> CreateForkImpl(int index)
        {
            var result = new EntityMethodsCallsChain();
            result.Id = Guid.NewGuid();
            result.ChainContext = await ChainContext.CloneChainContext();
            result.NextTimeToCall = NextTimeToCall;
            result.ForkCreatorId = Id;
            for (int i = index; i < Chain.Count; i++)
                result.Chain.Add(Chain[i]);
            lock (ForksIds)
                ForksIds.Add(result.Id);
            return result;
        }

        public Task ForkFinishedImpl(Guid forkId)
        {
            ForksIds.Remove(forkId);
            return Task.CompletedTask;
        }

        public Task<string> GetDescriptionImpl()
        {
            if (string.IsNullOrEmpty(_cacheDescription))
            {
                var sb = StringBuildersPool.Get;
                for (int i = 0; i < Chain.Count; i++)
                {
                    var block = Chain[i];
                    if (CurrentChainIndex == i)
                        sb.Append("*");
                    block.AppendToStringBuilder(sb);
                    sb.Append("|");
                }

                if (ForkCreatorId != Guid.Empty)
                    sb.Append(">FORK");

                _cacheDescription = sb.ToStringAndReturn();
            }

            return Task.FromResult(_cacheDescription);
        }

        public Task SetNextTimeToCallImpl(long nextTimeToCall)
        {
            NextTimeToCall = nextTimeToCall;
            return Task.CompletedTask;
        }

        public Task IncrementCurrentChainIndexImpl()
        {
            CurrentChainIndex++;
            _cacheDescription = null;
            return Task.CompletedTask;
        }

        public Task DecrementCurrentChainIndexImpl()
        {
            CurrentChainIndex--;
            _cacheDescription = null;
            return Task.CompletedTask;
        }
    }
}
