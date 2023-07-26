namespace Src.Locomotion
{
    public interface ILocomotionUpdateable
    {
        void Update(float deltaTime);
    }
    
//    public interface ILocomotionUpdateable<out T> : ILocomotionUpdateable
//    {
//        T Interface { get; }
//    }
}
