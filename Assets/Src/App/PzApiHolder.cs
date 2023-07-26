using System;
using Core.Environment.Logging.Extension;
using NLog;
using PzApi;
using ILogger = PzApi.ILogger;

namespace Assets.Src.App
{
    public class PzApiHolder
    {
        private static readonly Logger BadLogger = LogManager.GetCurrentClassLogger();

        private static IClientApiCommunicator _communicator;

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        public static IClientApiCommunicator Communicator =>
            _communicator ?? (_communicator = new ClientApiCommunicator(new GoodLogger()));

        public static bool Connected { get; set; }

        private class GoodLogger : ILogger
        {
            public void LogInfo(string message)
            {
                BadLogger.IfInfo()?.Message(message).Write();
            }

            public void LogError(string message)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                BadLogger.IfWarn()?.Message(message).Write();
#else
                BadLogger.IfError()?.Message(message).Write();
#endif
            }

            public void LogError(Exception exception, string message)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                BadLogger.IfWarn()?.Message($"{message} {exception}").Write();
#else
                BadLogger.IfError()?.Message(message).Exception(exception).Write();
#endif
            }
        }
    }

    // public class FakeClientApiCommunicator : IClientApiCommunicator
    // {
    //     public FakeClientApiCommunicator(ILogger logger)
    //     {
    //     }
    //
    //     public Task<bool> Connect()
    //     {
    //         return Task.FromResult(true);
    //     }
    //
    //     public Task<ApiResponse<FriendsList>> GetFriends(GetFriendsRequestData data)
    //     {
    //         const string all =
    //             "{\"Success\":true,\"Error\":null,\"Result\":{\"friends\":[{\"userId\":\"733c303f-9d00-4d0a-9f30-c40decc2eeee\",\"login\":\"chillinwithya\",\"realmId\":\"37006167-bbd7-4329-846b-4e727f24abbd\",\"status\":\"offline\",\"definition\":\"/Sessions/Queries/SavannahPvEWithDropzoneRulesQuery\",\"sessionWhenCreated\":\"2020-05-22T08:08:15.887692Z\"},{\"userId\":\"f5b92db3-106e-4424-90e7-bb3ca7669dd2\",\"login\":\"PapaLegba\",\"realmId\":\"37006167-bbd7-4329-846b-4e727f24abbd\",\"status\":\"offline\",\"definition\":\"/Sessions/Queries/SavannahPvEWithDropzoneRulesQuery\",\"sessionWhenCreated\":\"2020-05-22T08:08:15.887692Z\"},{\"userId\":\"872a18e5-4409-4c6f-ba3c-ce46d74e8cd2\",\"login\":\"kam.temnaya\",\"realmId\":null,\"status\":null,\"definition\":null,\"sessionWhenCreated\":null},{\"userId\":\"c4adb241-abf6-45ac-bbd7-8bae093455b3\",\"login\":\"khoroshevj\",\"realmId\":null,\"status\":null,\"definition\":null,\"sessionWhenCreated\":null}]}}\n";
    //         const string some =
    //             "{\"Success\":true,\"Error\":null,\"Result\":{\"friends\":[{\"userId\":\"733c303f-9d00-4d0a-9f30-c40decc2eeee\",\"login\":\"chillinwithya\",\"realmId\":\"37006167-bbd7-4329-846b-4e727f24abbd\",\"status\":\"offline\",\"definition\":\"/Sessions/Queries/SavannahPvEWithDropzoneRulesQuery\",\"sessionWhenCreated\":\"2020-05-22T08:08:15.887692Z\"},{\"userId\":\"f5b92db3-106e-4424-90e7-bb3ca7669dd2\",\"login\":\"PapaLegba\",\"realmId\":\"37006167-bbd7-4329-846b-4e727f24abbd\",\"status\":\"offline\",\"definition\":\"/Sessions/Queries/SavannahPvEWithDropzoneRulesQuery\",\"sessionWhenCreated\":\"2020-05-22T08:08:15.887692Z\"}]}}\n";
    //         return Task.FromResult(
    //             data.RealmId != null
    //                 ? JsonConvert.DeserializeObject<ApiResponse<FriendsList>>(some)
    //                 : JsonConvert.DeserializeObject<ApiResponse<FriendsList>>(all));
    //     }
    //
    //     public Task<ApiResponse<JoinFriendResponse>> JoinFriend(JoinFriendRequestData data)
    //     {
    //         return Task.FromResult(
    //             JsonConvert.DeserializeObject<ApiResponse<JoinFriendResponse>>("{\"Success\":true,\"Error\":null,\"Result\":{}}\n"));
    //     }
    // }
}