using NLog.Fluent;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using UnityEngine;

public static class AwaitExtensions
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static async Task<T> AwaitAsyncOp<T>(Func<T> fn) where T : AsyncOperation
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        await UnityQueueHelper.RunInUnityThread(() => { var op = fn(); op.completed += (v) => tcs.SetResult(op); });
        return await tcs.Task;
    }

    // Any time you call an async method from sync code, you can either use this wrapper
    // method or you can define your own `async void` method that performs the await
    // on the given Task
    public static async void WrapErrors(this Task task)
    {
        try
        {
            await task;
        }
        catch(OperationCanceledException e)
        {
            Logger.IfWarn()?.Exception(e).Write();
        }
        catch(Exception e)
        {
            Logger.IfError()?.Exception(e).Write();
        }
    }
}
