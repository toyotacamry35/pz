using System;
using System.Linq;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    [Flags]
    public enum LocomotionFlags : uint
    {
        Moving =   0x0001,
        Airborne = 0x0002, 
        Sprint =   0x0004,
        Jumping =  0x0008,
        Falling =  0x0010,
        Landing =  0x0020,
        Slipping = 0x0040, 
        Dodge    = 0x0080,
        Direct   = 0x0200,
        Teleport = 0x0400,
        CheatMode =0x0800,
        Turning =  0x1000,
        FollowPath    =  0x2000,
        NoCollideWithActors  =  0x4000,
    }
    
    public static class LocomotionFlagsHelper
    {
        public static readonly int MaxBits = (int)Math.Log(Enum.GetValues(typeof(LocomotionFlags)).Cast<uint>().Max(), 2) + 1;

        public static bool Any(this LocomotionFlags flags, LocomotionFlags mask) => (flags & mask) != 0;
        
        public static bool All(this LocomotionFlags flags, LocomotionFlags mask) => (flags & mask) == mask;
    }
}