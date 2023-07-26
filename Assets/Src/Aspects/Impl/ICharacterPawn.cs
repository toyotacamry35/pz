using Assets.Src.Aspects;
using Assets.Src.Aspects.Doings;
using Assets.Src.BuildingSystem;
using SharedCode.Entities.Engine;
using Assets.Src.Wizardry;
using Src.Aspects.Doings;
using Src.Locomotion;

namespace Src.Aspects.Impl
{
    public interface ICharacterPawn : ISubjectPawn, ILocomotionDebugSubject, ISubjectWithSpellDoer
    {
        IGuideProvider CameraGuideProvider { get; }
        
        ICharacterBrain Brain { get; }
        
        ICharacterBuildInterface BuildInterface { get; }
    }
}