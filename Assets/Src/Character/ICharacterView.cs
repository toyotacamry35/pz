using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Character.Events;
using ResourceSystem.Utils;
using SharedCode.Entities.Engine;
using Src.Aspects.Doings;

namespace Assets.Src.Character
{
    public interface ICharacterView : ISubjectWithDollView, IPitchableView, ITwistableView
    {
        AttackEventSubscriptionHandler[] AttackSubscriptionHandlers { get; }
        
        IAttackDoerSupport AttackDoerSupport { get; }
        
   //     MutationStageDef MutationStage { get; }

        bool HasAuthority { get; }
        
        void SetGuideProvider(IGuideProvider cameraGuideProvider);

        void InitClientAuthority(OuterRef entityRef);

            //        void Init(MutationStageDef mutationStage, bool isResurrectionAfterDeath, OuterRef entityRef);
       
        void InitClient(OuterRef entityRef);

        void SetMutationStage(MutationStageDef mutationStage);
    }
}