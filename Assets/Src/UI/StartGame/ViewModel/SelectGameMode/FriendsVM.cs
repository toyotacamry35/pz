using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.App;
using Core.Environment.Logging.Extension;
using Infrastructure.Cloud;
using NLog;
using PzLauncher.Models.Dto;
using ReactivePropsNs;
using SharedCode.Utils.Threads;

namespace Uins
{
    public class FriendsVM : BindingVmodel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const int UpdatePeriodMilliseconds = 60000;

        private readonly string _realmId;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _requestProcess;
        private readonly ReactiveProperty<IReadOnlyList<Friend>> _friends = new ReactiveProperty<IReadOnlyList<Friend>>();
        public IStream<IReadOnlyList<Friend>> Friends => _friends;

        public FriendsVM(string realmId = null)
        {
            _realmId = realmId;
            _cancellationTokenSource = new CancellationTokenSource();
            _requestProcess = TaskEx.Run(RequestFriends, _cancellationTokenSource.Token);
        }

        private async void RequestFriends()
        {
            if (!PzApiHolder.Connected)
                Logger.IfWarn()?.Message("PzApi Not Connected Can't Get Friends").Write();
            else
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var friendsResponse = await PzApiHolder.Communicator.GetFriends(new GetFriendsRequestData {RealmId = _realmId});
                        if (friendsResponse.Success)
                        {
                            var result = friendsResponse.Result;
                            await UnityQueueHelper.RunInUnityThread(
                                () => { _friends.Value = result.Friends; }
                            );
                        }
                        else
                            Logger.IfError()?.Message("Error Request Friends " + friendsResponse.Error).Write();
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message("Error Request Friends").Exception(e).Write();
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(UpdatePeriodMilliseconds));
                }
        }

        public override void Dispose()
        {
            AsyncProcessExtensions.ShutdownProcess(_cancellationTokenSource, _requestProcess);
            base.Dispose();
        }
    }
}