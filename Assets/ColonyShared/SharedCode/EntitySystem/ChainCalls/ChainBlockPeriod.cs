using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using ProtoBuf;
using SharedCode.Entities.Service;
using SharedCode.Utils;
using SharedCode.Logging;
using MongoDB.Bson.Serialization.Attributes;
using NLog;
using SharedCode.Entities.Core;

namespace SharedCode.EntitySystem.ChainCalls
{
    [ProtoContract]
    public class ChainBlockPeriod : ChainBlockBase
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly NLog.Logger DebugLogger = LogManager.GetLogger("ChainCallDebug");

        [ProtoMember(1)]
        public float Duration { get;  set; }

        [ProtoMember(2)]
        public int Repeat { get; set; }//-1 - бесконечное число раз, > 0 - ограниченное число раз

        [ProtoMember(3)]
        public bool FromUtcNow { get;  set; } //если true, время следющего запуска считается DateTime.UtcNow + Duration

        public ChainBlockPeriod()
        {
        }

        public ChainBlockPeriod(float duration, int repeat, bool fromUtcNow)
        {
            Duration = duration;
            Repeat = repeat;
            FromUtcNow = fromUtcNow;
        }

        public override async Task<bool> Execute(IEntityMethodsCallsChain chainCall, IChainCallServiceEntityInternal chainCallService)
        {
            if (Duration <= 0)
            {
                Logger.IfError()?.Message("ChainBlockPeriod incorrect duration {0} Repeat {1}", Duration, Repeat).Write();
                return true;
            }

            var utcNow = DateTime.UtcNow;
            var oldTimestamp = FromUtcNow ? utcNow : UnixTimeHelper.DateTimeFromUnix(chainCall.NextTimeToCall);
            int count = 0;
            if (!FromUtcNow)
                count = (int)Math.Floor((utcNow - oldTimestamp).TotalSeconds / Duration) + 1;
            count = Math.Max(1, count);

            var forks = new List<IEntityMethodsCallsChain>();
            for (int i = 0; i < count; i++)
            {
                await chainCall.SetNextTimeToCall(oldTimestamp.AddSeconds(Duration).ToUnix());
                if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                    DebugLogger.IfInfo()?.Message("DEFFERED: {0} NEXT TIME {1} {2}", chainCall.Id, oldTimestamp.AddSeconds(Duration).ToString(), FromUtcNow ? "FROMUTCNOW" : "").Write();

                if (Repeat != 0)//-1 - бесконечное число раз, > 0 - ограниченное число раз
                {
                    if (Repeat > 0)
                        Repeat--;

                    if (Repeat != 0)
                    {
                        var fork = await chainCall.CreateFork(chainCall.CurrentChainIndex + 1);
                        forks.Add(fork);
                        if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                            DebugLogger.IfInfo()?.Message("FORK: {0} FORKID {1} REPEAT LEFT {2}", chainCall.Id, fork.Id, Repeat).Write();
                    }
                }
            }

            if (forks.Count > 0)
            {
                using (var wrapper = await chainCallService.EntitiesRepository.Get(chainCallService.TypeId, chainCallService.Id))
                {
                    await chainCallService.ChainCallBatch(forks);
                }

                return false;
            }

            return true;
        }

        public override void AppendToStringBuilder(StringBuilder sb)
        {
            sb.Append("P,");
            sb.AppendFormat("{0:0.##},", Duration);
            sb.Append(Repeat);
            sb.Append(",");
            sb.Append(FromUtcNow);
        }
    }
}

