using System.Collections.Concurrent;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace UnityUpdate
{
    public class CustomSamplerCache
    {
        [NotNull]
        public static readonly CustomSamplerCache Samplers = new CustomSamplerCache();
        [NotNull]
        private readonly ConcurrentDictionary<(string CoroutineName, string MethodName, string FileName, int Line), CustomSampler> _samplers = new ConcurrentDictionary<(string,string,string,int), CustomSampler>();

        private CustomSamplerCache()
        {
        }

        [CanBeNull]
        //[Conditional("ENABLE_PROFILER")]
        public CustomSampler Get(string methodName, string filePath, int line)
        {
            return Get(null, methodName, filePath, line);
        }
        
        [CanBeNull]
        [NotNull]
        //[Conditional("ENABLE_PROFILER")]
        public CustomSampler Get(string coroutineName, string methodName, string filePath, int line)
        {
#if ENABLE_PROFILER
            var key = (coroutineName, methodName, filePath, line);
            if(_samplers.TryGetValue(key, out var existing))
            {
                return existing;
            }
            lock (_samplers)
            {
                if (_samplers.TryGetValue(key, out existing))
                {
                    return existing;
                }

                string callSite;
                if (coroutineName != null)
                    callSite = $"{coroutineName} started from {methodName} ({filePath}:{line})";
                else
                    callSite = $"{methodName} ({filePath}:{line})";
                existing = CustomSampler.Create(callSite);
                _samplers[key] = existing;
            }
            return existing;
#else
            return null;
#endif
        }
    }
}
