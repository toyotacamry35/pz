using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode;

namespace Src.Locomotion
{
    public interface ILocomotionDebugInfoProvider
    {
        Value this[DebugTag id] { get; }

        bool Contains(DebugTag id);
        
        IEnumerable<DebugTag> Tags { get; }
        
        bool IsDemanded { set; }
    }
    
    public static class ILocomotionDebugInfoProviderMethods
    {
        public static DebugTag NextId(this ILocomotionDebugInfoProvider provider, DebugTag id)
        {
            var itr = provider.Tags.GetEnumerator();
            while(itr.MoveNext())
                if (itr.Current == id)
                {
                    if (itr.MoveNext())
                        return itr.Current;
                    break;
                }
            return provider.Tags.FirstOrDefault();
        }

        public static DebugTag PrevId(this ILocomotionDebugInfoProvider provider, DebugTag id)
        {
            var reverse = provider.Tags.Reverse();
            var itr = reverse.GetEnumerator();
            while(itr.MoveNext())
                if (itr.Current == id)
                {
                    if (itr.MoveNext())
                        return itr.Current;
                    break;
                }
            return reverse.FirstOrDefault();
        }
    }
}
