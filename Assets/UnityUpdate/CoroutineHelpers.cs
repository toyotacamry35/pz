using System.Collections;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;
using UnityUpdate;

public static class MonoBehaviourExts
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static Coroutine StartInstrumentedCoroutine(this MonoBehaviour script, IEnumerator wrapped, string name = null, [CallerMemberName] string invokeFn = null, [CallerFilePath] string invokeFile = null, [CallerLineNumber] int invokeLine = 0)
    {
        return script.StartCoroutine(new CoroutineWrapper(wrapped, name ?? wrapped.GetType().Name, invokeFn, invokeFile, invokeLine));
    }

    public static Coroutine StartInstrumentedCoroutineLiteWeight(this MonoBehaviour script, IEnumerator wrapped, [CallerMemberName] string invokeFn = null, [CallerFilePath] string invokeFile = null, [CallerLineNumber] int invokeLine = 0)
    {
        return script.StartCoroutine(new CoroutineWrapper(wrapped, invokeFn, invokeFile, invokeLine));
    }

    public static void StopCoroutineIfNotNull(this MonoBehaviour mbh, ref Coroutine coroutine)
    {
        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Stop coroutine in {mbh.GetType().Name}").Write();
        if (coroutine != null)
            mbh.StopCoroutine(coroutine);
        coroutine = null;
    }
}

internal struct CoroutineWrapper : IEnumerator
{
    private readonly IEnumerator _wrapped;

    [CanBeNull] private readonly CustomSampler _sampler;

    public CoroutineWrapper(IEnumerator wrapped, string methodName, string filePath, int line)
    {
        _sampler = CustomSamplerCache.Samplers.Get(methodName, filePath, line);
        _wrapped = wrapped;
    }

    public CoroutineWrapper(IEnumerator wrapped, string coroutineName, string methodName, string filePath, int line)
    {
        _sampler = CustomSamplerCache.Samplers.Get(coroutineName, methodName, filePath, line);
        _wrapped = wrapped;
    }
    
    public object Current => _wrapped.Current;

    public bool MoveNext()
    {
        _sampler?.Begin();
        var retVal = _wrapped.MoveNext();
        _sampler?.End();
        return retVal;
    }

    public void Reset()
    {
        _wrapped.Reset();
    }
}
