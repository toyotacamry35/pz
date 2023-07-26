using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;

namespace Core.Environment
{
    public static class PidWatcher
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<int> Watch(int pid)
        {
            Logger.IfInfo()?.Message("Installing watcher for PID {0}", pid).Write();

            try
            {
                var process = Process.GetProcessById(pid);
                return await process;
            }
            catch (ArgumentException)
            {
                Logger.IfWarn()?.Message("Cannot find process with pid PID {0}", pid).Write();
                throw;
            }
            catch (InvalidOperationException)
            {
                Logger.IfWarn()?.Message("Cannot find process with pid PID {0}", pid).Write();
                throw;
            }
            finally
            {
                Logger.IfInfo()?.Message("Parent with PID {0} is dead or not found", pid).Write();
            }
        }

        public static TaskAwaiter<int> GetAwaiter(this Process process)
        {
            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            process.EnableRaisingEvents = true;

            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

            if (process.HasExited)
            {
                tcs.TrySetResult(process.ExitCode);
            }

            return tcs.Task.GetAwaiter();
        }
    }
}
