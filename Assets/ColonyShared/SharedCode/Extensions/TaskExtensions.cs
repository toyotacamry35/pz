using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace SharedCode.Extensions
{
    public static class TaskExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void WrapErrors(this Task task)
        {
            task.ContinueWith(LogTaskStatus);
        }

        private static void LogTaskStatus(Task task)
        {
            if (task.IsCanceled)
                Logger.IfWarn()?.Exception(task.Exception).Write();
            else if (task.IsFaulted)
                Logger.IfError()?.Exception(task.Exception).Write();
        }
    }
}
