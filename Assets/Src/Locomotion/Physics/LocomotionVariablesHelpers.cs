namespace Src.Locomotion
{
    public static class LocomotionVariablesHelpers
    {
        public static LocomotionVector CalculateNextPosition(in LocomotionVariables vars, float dt)
        {
            var velocity = vars.Velocity + vars.ExtraVelocity;
            var nextPosition = vars.Position + vars.ExtraPosition + velocity * dt;
            return nextPosition;
        }
        
        public static LocomotionVector CalculateMoveDelta(in LocomotionVariables vars, float dt)
        {
            var velocity = vars.Velocity + vars.ExtraVelocity;
            return vars.ExtraPosition + velocity * dt;
        }
        
        public static (LocomotionVector,LocomotionVector) CalculateVelocityAndMoveDelta(in LocomotionVariables vars, float dt)
        {
            var velocity = vars.Velocity + vars.ExtraVelocity;
            return (velocity, vars.ExtraPosition + velocity * dt);
        }
        
    }
}