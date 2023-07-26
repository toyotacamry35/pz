using System;

namespace Assets.Src.Aspects
{
    // Used for debugging
    public interface IInvestigateObject
    {
        event Action OnFrameStart;
        event Action OnFrameEnd;
    }
}
