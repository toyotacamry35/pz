using System;
using ColonyShared.SharedCode;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public interface ILocomotionDebugAgent
    {
        /// <summary>
        /// Use `IsNotNullAndActive()` if could be null.
        /// </summary>
        bool IsActive { get; }
        
        void Add(DebugTag id, Value entry);

        // Is not used now any more
        void BeginOfFrame();
        
        void EndOfFrame();
    }
    
    public static class LocomotionDebugAgentMethods
    {
        public static bool IsNotNullAndActive(this ILocomotionDebugAgent self) => self.IfActive() != null;
        public static ILocomotionDebugAgent IfActive(this ILocomotionDebugAgent self) => self != null && self.IsActive ? self : null;
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, float value)            => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, LocomotionVector value) => self.IfActive()?.Add(id, value.ToValue());
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, Vector2 value)          => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, Vector3 value)          => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, int value)              => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, long value)             => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, bool value)             => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, string value)           => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, Color value)            => self.IfActive()?.Add(id, new Value(value));
        public static void Set(this ILocomotionDebugAgent self, DebugTag id, DateTime value)         => self.IfActive()?.Add(id, new Value(value));

        public static void Gather(this ILocomotionDebugAgent self, object obj)
        {
            if (self != null && self.IsActive) 
				(obj as ILocomotionDebugable)?.GatherDebug(self);
        }
    }
}
