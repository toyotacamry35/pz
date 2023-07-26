using Core.Environment;
using Monitoring.Contract;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GeneratedCode.Infrastructure.Cloud
{
    public static class MonitoringCliInitializer
    {
        [Conditional("COLONY_CLUSTER")]
        public static void Start(int currentProcessId, string instanceName, string pushGwAddress)
        {
            var thisFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var directory = thisFile.Directory;
            
            var mainModuleFile = Process.GetCurrentProcess().MainModule.FileName;
            string targetFile;
            FileInfo executableName;
            if(mainModuleFile.Contains("dotnet"))
            {
                executableName = new FileInfo(mainModuleFile);
                targetFile = Path.Combine(directory.FullName, "Monitoring.Cli.dll");
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                executableName = new FileInfo(Path.Combine(directory.FullName, "Monitoring.Cli.exe"));
                targetFile = "";
            }
            else
            {
                executableName = new FileInfo(Path.Combine(directory.FullName, "Monitoring.Cli"));
                targetFile = "";
            }
            var args = $"{targetFile} --pid {currentProcessId} --instance-name {instanceName} --monitoring-type {TargetProcessRegisterModel.MonitoringType.Gc} --monitoring-type {TargetProcessRegisterModel.MonitoringType.Thread} --default-counter-refresh 1 --push-gw-endpoint {pushGwAddress}";
            ExternalProcessWrap.Start(executableName, args);
        }
    }
}
