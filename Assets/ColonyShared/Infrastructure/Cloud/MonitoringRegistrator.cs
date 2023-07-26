using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Infrastructure.Config;
using Monitoring.Contract;
using NLog;
using SharedCode.Config;

namespace GeneratedCode.Infrastructure.Cloud
{
    public class MonitoringRegistrator
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static async Task Register(int currentProcessId, string name, string pushGwAddress)
        {
            var monitoringApi = new MonitoringApi(pushGwAddress);
            var registerModel = new TargetProcessRegisterModel(
                currentProcessId, name,
                new[] {TargetProcessRegisterModel.MonitoringType.Gc, TargetProcessRegisterModel.MonitoringType.Thread},
                1);
            int tryNumber = 0;
            while (tryNumber < 10)
            {
                try
                {
                    var registerResponse = await monitoringApi.Register(registerModel);
                    if (registerResponse.Code == TargetProcessRegisterModel.Result.ResultCode.Ok)
                    {
                        Logger.IfInfo()?.Message("Container was registred to monitoring").Write();
                        break;
                    }
                    else
                    {
                        Logger.Error("Error when was trying to register to monitoring ErrorCode={ErrorCode}",
                            registerResponse.Code);
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Error when was trying to connect to monitoring").Write();;
                }

                tryNumber++;
                await Task.Delay(500);
            }
        }
    }
}