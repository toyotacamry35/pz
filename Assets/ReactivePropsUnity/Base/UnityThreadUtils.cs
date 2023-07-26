using System;
using System.Threading;
using Core.Environment.Logging.Extension;
using NLog;
using UnityAsyncAwaitUtil;

namespace ReactivePropsNs
{
    public static class UnityThreadUtils
    {
        public static void AssertRunIntoUnityThread(Logger logger)
        {
            if (SyncContextUtil.IsInUnity)
                return;
            logger.IfError()?.Message("RunAsyncTaskFromUnity Not From Unity").Write();
            throw new Exception("RunAsyncTaskFromUnity Not From Unity");
        }

        public static void AssertRunOutOfUnityThread(Logger logger)
        {
            if (!SyncContextUtil.IsInUnity)
                return;
            logger.IfError()?.Message("RunThreadPoolTask Not From ThreadPool").Write();
            throw new Exception("RunThreadPoolTask Not From ThreadPool");
        }
    }
}