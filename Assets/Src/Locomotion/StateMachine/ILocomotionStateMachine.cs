using System;

namespace Src.Locomotion
{
    public interface ILocomotionStateMachine : ILocomotionPipelinePassNode, ILocomotionDebugable
    {
        string CurrentStateName { get; }

        event Action OnStartNewStateEvent;
        event Action OnFinishCurrStateEvent;
    }
}
