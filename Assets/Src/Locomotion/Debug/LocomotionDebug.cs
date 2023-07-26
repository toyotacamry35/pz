using System;

namespace Src.Locomotion
{
    public static partial class LocomotionDebug
    {
        public static ILocomotionDebugAgent DebugAgent { get; private set; }
        
        public static Guid EntityId { get; private set; }

        public class Context
        {
            private readonly Guid _entityId;
            private readonly ILocomotionDebugAgent _agent;

            public Context(Guid entityId, ILocomotionDebugAgent agent)
            {
                _entityId = entityId;
                _agent = agent;
            }
            
            public void BeginOfFrame()
            {
                LocomotionDebug.DebugAgent = _agent;
                LocomotionDebug.EntityId = _entityId;
                _agent?.BeginOfFrame();
            }
            
            public void EndOfFrame()
            {
                _agent?.EndOfFrame();
                if(LocomotionDebug.DebugAgent == _agent)
                    LocomotionDebug.DebugAgent = null;                    
            }
        }
    }
}