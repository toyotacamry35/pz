using Assets.Src.Character;

namespace Assets.Src.Aspects
{
    public interface ITwistableView
    {
         BodyTwistIK TwistMotor { get; }
         TurningWithStepping TurningMotor { get; }
    }
}