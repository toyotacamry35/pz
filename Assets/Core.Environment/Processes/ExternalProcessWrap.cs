using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Core.Environment
{
    public static class ExternalProcessWrap
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static Process Start(FileInfo executable, string args)
        {
            Logger.IfInfo()?.Message("Starting process \"{0}\" with arguments \"{1}\"", executable, args).Write();
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = executable.FullName,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = System.Environment.CurrentDirectory,
                CreateNoWindow = true
            };

            var process = Process.Start(info);
            if (process == null)
                Logger.IfError()?.Message("Error starting process \"{0}\" with arguments \"{1}\"", executable, args).Write();
            else
                Logger.IfInfo()?.Message("Process \"{0}\" with arguments \"{1}\" started. ProcessId {2}", executable, args, process.Id).Write();
            WrapProcess(process);

            return process;
        }
        private static async void WrapProcess(Process process)
        {
            try
            {
                using (process)
                {
                    var logger = NLog.LogManager.GetLogger("Process-" + process.Id);
                    var stdOutTask = DumpToLoggerLineByLine(process.StandardOutput, logger, NLog.LogLevel.Info);
                    var stdErrTask = DumpToLoggerLineByLine(process.StandardError, logger, NLog.LogLevel.Error);
                    await Task.WhenAll(stdOutTask);
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        private static async Task DumpToLoggerLineByLine(TextReader reader, NLog.Logger logger, NLog.LogLevel level)
        {
            var str = await reader.ReadLineAsync();
            while (str != null)
            {
                logger.Log(level, str);
                str = await reader.ReadLineAsync();
            }
        }
    }
}
