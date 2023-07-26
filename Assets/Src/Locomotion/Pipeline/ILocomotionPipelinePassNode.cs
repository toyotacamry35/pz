namespace Src.Locomotion
{
    public interface ILocomotionPipelinePassNode
    {
        bool IsReady { get; }
        
        LocomotionVariables Pass(LocomotionVariables vars, float dt);
    }
}