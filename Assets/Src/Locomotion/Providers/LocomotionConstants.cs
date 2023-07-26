using ColonyShared.SharedCode.Aspects.Locomotion;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    /// <summary>
    /// Just an wrapper-getter over statical data from def
    /// </summary>
    public class LocomotionConstants : ILocomotionConstants
    {
        private readonly LocomotionConstantsDef _def;

        public LocomotionConstants(LocomotionConstantsDef def)
        {
            _def = def;
        }

        float ILocomotionConstants.InputStandingThreshold => _def.InputStandingThreshold;
        float ILocomotionConstants.InputMoveThreshold => _def.InputWalkThreshold;
        float ILocomotionConstants.InputRunThreshold => _def.InputRunThreshold;
        float ILocomotionConstants.VerticalSlopeAngle => _def.VerticalSlopeAngle * Mathf.Deg2Rad;
    }
}