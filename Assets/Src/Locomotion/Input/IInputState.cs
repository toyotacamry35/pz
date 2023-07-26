namespace Src.Locomotion
{
    public interface IInputState<TInputs> where TInputs : Inputs
    {           
        float[] Axes { get; }
        
        uint Triggers { get; }
    }
}