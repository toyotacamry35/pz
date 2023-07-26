using Assets.Src.Character;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;

namespace Src.Locomotion.Unity
{
    public class CharacterBodyTurning : ILocomotionPipelineCommitNode
    {
        [CanBeNull] private readonly BodyTwistIK _twistIK;
        [CanBeNull] private readonly TurningWithStepping _turningWithStepping;

        public CharacterBodyTurning([CanBeNull] BodyTwistIK twistIk, [CanBeNull] TurningWithStepping turningWithStepping)
        {
            _twistIK = twistIk;
            _turningWithStepping = turningWithStepping;
        }

        public bool IsReady => true;

        public void Commit(ref LocomotionVariables inVars, float dt)
        {
            if (_twistIK)
            {
                if (inVars.Flags.Any(LocomotionFlags.Direct | LocomotionFlags.Dodge | LocomotionFlags.Falling | LocomotionFlags.Slipping))
                    _twistIK.Disable();
                else
                    _twistIK.Enable();
            }

            if (_turningWithStepping)
            {
                if (inVars.Flags.Any(LocomotionFlags.Direct | LocomotionFlags.Dodge | LocomotionFlags.Falling | LocomotionFlags.Moving | LocomotionFlags.Slipping | LocomotionFlags.Airborne | LocomotionFlags.Jumping| LocomotionFlags.Landing | LocomotionFlags.CheatMode))
                    _turningWithStepping.Disable();
                else
                    _turningWithStepping.Enable();
            }
        }
    }
}