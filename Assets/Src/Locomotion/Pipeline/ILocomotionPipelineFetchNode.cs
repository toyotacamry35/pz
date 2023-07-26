namespace Src.Locomotion
{
    public interface ILocomotionPipelineFetchNode
    {
        bool IsReady { get; }
        
        LocomotionVariables Fetch(float dt);
    }
}