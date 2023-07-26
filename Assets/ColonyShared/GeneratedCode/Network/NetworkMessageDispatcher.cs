using System;
using System.Threading.Channels;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem.Statistics;
using GeneratedCode.Network.Statistic;
using SharedCode.Cloud;
using SharedCode.Utils.Threads;

namespace GeneratedCode.Network
{
    public readonly struct NetworkMessageDispatcher
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly NetworkMessageDispatcherQueueStatistics.NetworkMessageDispatcherQueueSize _queueSizeStat;

        private readonly ChannelWriter<IFuncInfo> _writer;
        private readonly ChannelReader<IFuncInfo> _reader;

        public NetworkMessageDispatcher(Guid repoId, CloudNodeType nodeType)
        {
            _queueSizeStat = new NetworkMessageDispatcherQueueStatistics.NetworkMessageDispatcherQueueSize(repoId, nodeType);

            Statistics<NetworkMessageDispatcherQueueStatistics>.Instance.Register(_queueSizeStat);

            var channel = Channel.CreateUnbounded<IFuncInfo>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = true });
            _writer = channel.Writer;
            _reader = channel.Reader;
            TaskEx.Run(Process);
        }

        private async void Process()
        {
            while (await _reader.WaitToReadAsync())
            {
                if (_reader.TryRead(out var fi))
                {
                    try
                    {
                        fi.Run();
                        _queueSizeStat.MessageProcessed();
                    }
                    catch(Exception e)
                    {
                        Logger.IfError()?.Exception(e).Write();
                    }
                }
            }
        }

        internal void Add(IFuncInfo funcInfo)
        {
            _queueSizeStat.MessageIncoming();
            if (!_writer.TryWrite(funcInfo))
                _writer.WriteAsync(funcInfo).AsTask().Wait();
        }

        public void Dispose()
        {
            _writer.Complete();
            Statistics<NetworkMessageDispatcherQueueStatistics>.Instance.Unregister(_queueSizeStat);
        }
    }

    internal interface IFuncInfo
    {
        void Run();
    }
}
