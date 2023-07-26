using System;
using Assets.Src.BuildingSystem;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Entities.Engine;
using SharedCode.Wizardry;
using Src.Aspects.Doings;
using Src.InputActions;
using Src.Locomotion;

namespace Assets.Src.Aspects.Doings
{
    public interface ICharacterBrain : 
        ILocomotionInputSource<CharacterInputs>, 
        ILocomotionInputReceiver,
        IGuideProvider,
        IInvestigateObject
    {
  //      GameObject GameObject { get; }

        bool IsBot { get; }
        
        event InputActionHandlerInteraction.Delegate TryToInteract;

        event SpellDoerFinsihDelegate InteractionFinished;
        
        IInputActionStatesGenerator InputActionStatesGenerator { get; }
        
        ICharacterBuildInterface BuildInterface { get; }
    }

    public interface IDisposableCharacterBrain : ICharacterBrain
    {
        void Dispose();
    } 
}